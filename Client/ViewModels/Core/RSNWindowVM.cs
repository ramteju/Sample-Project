using Client.Command;
using Client.Common;
using Client.Logging;
using Client.ViewModel;
using Client.ViewModels.Utils;
using Client.Views;
using Client.XML;
using Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Excelra.Utils.Library;
using Client.Util;
using Client.Validations;

namespace Client.ViewModels
{
    public class RSNWindowVM : ViewModelBase
    {
        #region CVT
        private string filterCVT;
        private CvtVM selectedCVT;
        private ObservableCollection<CvtVM> cVTData;
        private ListCollectionView cvtView;
        public string FilterCVT
        {
            get { return filterCVT; }
            set
            {
                SetProperty(ref filterCVT, value);
                UpdateCVTView();
            }
        }
        public CvtVM SelectedCVT
        {
            get { return selectedCVT; }
            set
            {
                SetProperty(ref selectedCVT, value);
            }
        }
        public ObservableCollection<CvtVM> CVTData
        {
            get { return cVTData; }
            set
            {
                SetProperty(ref cVTData, value);
                CVTView = new ListCollectionView(CVTData);
            }
        }
        public ListCollectionView CVTView { get { return cvtView; } set { SetProperty(ref cvtView, value); } }
        public string RSNTitle { get { return rSNTitle; } set { SetProperty(ref rSNTitle, value); } }
        void UpdateCVTView()
        {
            CVTView.Filter = (c) =>
            {
                if (c != null && !String.IsNullOrEmpty(FilterCVT))
                {
                    var cvtVM = c as CvtVM;
                    return (cvtVM.Text.StartsWith(FilterCVT, StringComparison.InvariantCultureIgnoreCase));
                }
                return true;
            };
        }
        #endregion

        #region Free Text
        private bool disableAppendMode = false;
        private string filterFreeText;
        private FreeTextVM selectedFreeText;
        private ObservableCollection<FreeTextVM> freeTextData;
        private ListCollectionView freeTextView;
        public string FilterFreeText
        {
            get { return filterFreeText; }
            set
            {
                SetProperty(ref filterFreeText, value);
                UpdateFreeTextView();
            }
        }
        public FreeTextVM SelectedFreeText
        {
            get { return selectedFreeText; }
            set { SetProperty(ref selectedFreeText, value); }
        }
        public ObservableCollection<FreeTextVM> FreeTextData
        {
            get { return freeTextData; }
            set
            {
                SetProperty(ref freeTextData, value);
                FreeTextView = new ListCollectionView(FreeTextData);
            }
        }
        public ListCollectionView FreeTextView { get { return freeTextView; } set { SetProperty(ref freeTextView, value); } }

        private void UpdateFreeTextView()
        {
            try
            {
                FreeTextView.Filter = (c) =>
                  {
                      if (c != null && !String.IsNullOrEmpty(FilterFreeText))
                      {
                          var freeTextVM = c as FreeTextVM;
                          return (freeTextVM.Text.StartsWith(FilterFreeText, StringComparison.InvariantCultureIgnoreCase));
                      }
                      return true;
                  };
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }
        #endregion

        #region Flags
        private Visibility stageVisible;
        private RsnLevel rsnLevel;
        private FreeTextMode freeTextMode;
        private bool rsnLevelEnable;
        public RsnLevel RsnLevel
        {
            get { return rsnLevel; }
            set
            {
                SetProperty(ref rsnLevel, value);
                StageName = RsnLevel == RsnLevel.STAGE && StageVM != null ? StageVM.Name : string.Empty;
            }
        }
        public FreeTextMode FreeTextMode { get { return freeTextMode; } set { SetProperty(ref freeTextMode, value); } }
        public Visibility StageVisible { get { return stageVisible; } set { SetProperty(ref stageVisible, value); } }
        public bool RsnLevelEnable { get { return rsnLevelEnable; } set { SetProperty(ref rsnLevelEnable, value); } }
        #endregion

        #region Grid Data
        private ObservableCollection<RsnVM> rsns;
        private ListCollectionView rsnView;
        //Selected Rsn in Grid.
        private RsnVM selectedRsn;
        //Currently editing Rsn
        private RsnVM editingRsn;
        public RsnVM EditingRsn
        {
            get { return editingRsn; }
            set
            {
                SetProperty(ref editingRsn, value);
                if (value != null)
                {
                    disableAppendMode = true;
                    CVT = value.CvtText;
                    FreeText = value.FreeText;
                    disableAppendMode = false;
                }
            }
        }
        public RsnVM SelectedRsn { get { return selectedRsn; } set { SetProperty(ref selectedRsn, value); } }
        public ObservableCollection<RsnVM> Rsns
        {
            get { return rsns; }
            private set
            {
                SetProperty(ref rsns, value);
                if (value != null)
                    foreach (var rsn in value)
                        rsn.RSNWindowVM = this;
            }
        }

        public void SetRsns(List<RsnVM> rsns)
        {
            try
            {
                Rsns = new ObservableCollection<RsnVM>(rsns);
                Rsns.CollectionChanged += Rsns_CollectionChanged;
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private void Rsns_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            try
            {
                Rsns.UpdateDisplayOrder();
                UpdateRSNView();
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        public ListCollectionView RsnsView
        {
            get { return rsnView; }
            set { SetProperty(ref rsnView, value); }
        }
        public void UpdateRSNView()
        {
            try
            {
                if (ReactionVM != null)
                {
                    RsnsView = new ListCollectionView(Rsns);
                    RsnsView.Filter = RsnFilter.Filter;
                    RsnsView.Refresh();
                    UpdateRsnLengths();
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }
        #endregion

        #region Current Editing Values
        private string cvt;
        private string freeText;
        private bool isIgnorableInDelivery;
        public string CVT { get { return cvt; } set { SetProperty(ref cvt, value); } }
        public bool IsIgnorableInDelivery { get { return isIgnorableInDelivery; } set { SetProperty(ref isIgnorableInDelivery, value); } }
        public string FreeText
        {
            get { return freeText; }
            set
            {
                var text = value;
                if (!string.IsNullOrEmpty(FreeText))
                {
                    if (FreeTextMode == FreeTextMode.REPLACE && text.ToLower().Split(new string[] { RsnLevel == RsnLevel.REACTION ? ", " : "), " }, StringSplitOptions.RemoveEmptyEntries).GroupBy(x => x).Count(x => x.Count() > 1) > 0)
                    {
                        AppInfoBox.ShowInfoMessage($"FreeText Conatins Duplicate texts");
                        return;
                    }
                    else if (FreeTextMode == FreeTextMode.APPEND)
                    {
                        string newText = text;
                        var splittedFreetexts = FreeText.Split(new String[] { RsnLevel == RsnLevel.REACTION ? ", " : "), " }, StringSplitOptions.RemoveEmptyEntries).Where(c => FreeTextWithOutStageInfo(c).Trim().ToLower().Equals(FreeTextWithOutStageInfo(newText).Trim().ToLower()));
                        if (splittedFreetexts != null && splittedFreetexts.Count() > 0)
                        {
                            AppInfoBox.ShowInfoMessage($"Added Text {newText} Already Exists in the FreeText. .");
                            return;
                        }
                    }
                }


                if (!string.IsNullOrEmpty(FreeText) && FreeText == text)
                { //MessageBox.Show("Free Text Already Exists . .");
                }
                //else if (!disableAppendMode && !String.IsNullOrEmpty(FreeText) && EditingRsn == null && !String.IsNullOrEmpty(text) && FreeText.Contains(text)) //Regex.Matches(freeText, text, RegexOptions.IgnoreCase).Count > 0
                //    MessageBox.Show("Free Text Already Exists . .");
                else
                {
                    #region The below code won't execute while resetting freetext
                    if (!disableAppendMode && FreeTextMode == FreeTextMode.APPEND)
                        text = freeText.Length > 0 ? string.IsNullOrEmpty(value) ? freeText : String.Join(", ", freeText, value) : value;
                    #endregion
                    SetProperty(ref freeText, text);
                }
            }
        }
        #endregion

        #region Level
        private ReactionVM reactionVM;
        private StageVM stageVM;
        private string stageName;
        public ReactionVM ReactionVM
        {
            get { return reactionVM; }
            set
            {
                SetProperty(ref reactionVM, value);
                if (value != null && value.Stages != null)
                    if (value.Stages.Count == 1)
                        StageVisible = Visibility.Hidden;
                    else
                        StageVisible = Visibility.Visible;
            }
        }
        public StageVM StageVM
        {
            get { return stageVM; }
            set
            {
                SetProperty(ref stageVM, value);
                if (value == null)
                    RsnLevel = RsnLevel.REACTION;
                else
                {
                    if (value.DisplayOrder == 1)
                        RsnLevel = RsnLevel.REACTION;
                    else if (value.DisplayOrder > 1)
                        RsnLevel = RsnLevel.STAGE;
                }
            }
        }
        public String StageName { get { return stageName; } set { SetProperty(ref stageName, value); } }
        #endregion

        #region Lengths
        private int currentStageRSNLength;
        private int totalRSNLength;
        private int otherStagesRSNLength;
        private string rSNTitle;

        public int CurrentStageRSNLength { get { return currentStageRSNLength; } set { SetProperty(ref currentStageRSNLength, value); } }
        public int TotalRSNLength { get { return totalRSNLength; } set { SetProperty(ref totalRSNLength, value); } }
        public int OtherStagesRSNLength { get { return otherStagesRSNLength; } set { SetProperty(ref otherStagesRSNLength, value); } }
        #endregion

        public string FreeTextWithOutStageInfo(string freetext)
        {
            if (freetext.Contains("("))
            {
                return freetext.Substring(0, freetext.IndexOf('('));
            }
            else
                return freetext;
        }

        #region Commands
        public DelegateCommand CVTSelected { get; private set; }
        public DelegateCommand FreeTextSelected { get; private set; }
        public DelegateCommand SaveForm { get; private set; }
        public DelegateCommand ClearEditForm { get; private set; }
        #endregion
        public RSNWindowVM()
        {
            RSNTitle = S.RSNTitle;
            RsnLevel = RsnLevel.REACTION;
            FreeTextMode = FreeTextMode.REPLACE;
            CVT = String.Empty;
            disableAppendMode = true;
            FreeText = String.Empty;
            disableAppendMode = false;
            CVTData = new ObservableCollection<CvtVM> { };
            Rsns = new ObservableCollection<RsnVM>();
            StageName = RsnLevel == RsnLevel.STAGE ? StageVM.Name : string.Empty;
            CVTSelected = new DelegateCommand(this.WhenCVTSelected);
            SaveForm = new DelegateCommand(this.DoSaveForm);
            FreeTextSelected = new DelegateCommand(this.WhenFreeTextSelected);

            ClearEditForm = new DelegateCommand(this.DoClearEditForm);
            RsnLevelEnable = true;
            S.DataLoadingComplete += S_DataLoadingComplete;
        }

        private void S_DataLoadingComplete(object sender, bool e)
        {
            var commentDictionary = S.CommentDictionary;

            var cvtList = new List<CvtVM>();
            foreach (var c in commentDictionary.CVT)
                cvtList.Add(new CvtVM { Text = c.CVTS, AssociatedFreeText = c.AssociatedFreeText, IsIgnorableInDelivery = c.IsIgnorableInDelivery, ForeColor = c.IsIgnorableInDelivery ? "Blue" : "Black" });
            CVTData = new ObservableCollection<CvtVM>(cvtList);

            var freeTextList = new List<FreeTextVM>();
            foreach (var fc in commentDictionary.FreeText)
                freeTextList.Add(new FreeTextVM { Text = fc.FreeTexts });
            FreeTextData = new ObservableCollection<FreeTextVM>(freeTextList);
        }

        private void UpdateRsnLengths()
        {
            try
            {
                CurrentStageRSNLength = OtherStagesRSNLength = TotalRSNLength = 0;
                var rsnIds = new List<Guid>();
                if (RsnsView != null)
                {
                    var currentRsns = RsnsView.Cast<RsnVM>();
                    rsnIds = currentRsns.Select(rsn => rsn.Id).ToList();
                    CurrentStageRSNLength = currentRsns.Select(rsn => (!string.IsNullOrEmpty(rsn.CvtText) ? rsn.CvtText.Length : 0) + (!string.IsNullOrEmpty(rsn.FreeText) ? rsn.FreeText.Length : 0)).Sum();
                }
                if (Rsns != null)
                {
                    var otherRsns = Rsns.Where(r => r.Reaction.Id == ReactionVM.Id && !rsnIds.Contains(r.Id)).ToList();
                    OtherStagesRSNLength = otherRsns.Select(rsn => (!string.IsNullOrEmpty(rsn.CvtText) ? rsn.CvtText.Length : 0) + (!string.IsNullOrEmpty(rsn.FreeText) ? rsn.FreeText.Length : 0)).Sum();
                }
                TotalRSNLength = CurrentStageRSNLength + OtherStagesRSNLength;
                Debug.WriteLine(CurrentStageRSNLength + " + " + OtherStagesRSNLength + " = " + TotalRSNLength);
            }
            catch (Exception ex)
            {
                Log.This(ex);
                AppErrorBox.ShowErrorMessage("Some error occured in RSNLength.", ex.Message);
            }
        }

        private void DoSaveForm(object obj)
        {
            try
            {
                string outMsg = string.Empty;
                if (!string.IsNullOrEmpty(FreeText.Trim()) || !string.IsNullOrEmpty(CVT))
                {
                    if (!RV.ValidateRsnFreetext(FreeText, ReactionVM, StageVM, RsnLevel, out outMsg))
                    {
                        AppInfoBox.ShowInfoMessage(outMsg);
                        return;
                    }

                    if (RV.ValidateRsnReactionLevel(ReactionVM, StageVM, RsnLevel, CVT, FreeText, Rsns.ToList(), out outMsg, EditingRsn))
                    {
                        if (EditingRsn != null)
                        {
                            EditingRsn.CvtText = CVT;
                            EditingRsn.IsIgnorableInDelivery = IsIgnorableInDelivery;
                            EditingRsn.FreeText = FreeText.Trim().TrimEnd('.');
                            EditingRsn.Stage = RsnLevel == RsnLevel.STAGE ? StageVM : null;
                            UpdateRsnLengths();
                        }
                        else
                        {
                            var rsn = new RsnVM { };
                            rsn.CvtText = CVT;
                            rsn.IsIgnorableInDelivery = IsIgnorableInDelivery;
                            rsn.FreeText = FreeText.Trim().TrimEnd('.');
                            rsn.Reaction = ReactionVM;
                            rsn.Id = Guid.NewGuid();
                            rsn.Stage = RsnLevel == RsnLevel.STAGE ? StageVM : null;
                            rsn.RSNWindowVM = this;
                            Rsns.Add(rsn);
                        }
                    }
                    else
                    {
                        AppInfoBox.ShowInfoMessage(outMsg);
                        return;
                    }
                    if (Rsns.Where(rsn => rsn.CvtText.SafeEqualsLower(S.ENZYMIC_CVT)).Count() > 0 && Rsns.Where(rsn => rsn.CvtText.SafeEqualsLower(S.BIOTRANSFORMATION_CVT)).Count() == 0)
                    {
                        var enzymicRSN = Rsns.Where(rsn => rsn.CvtText.SafeEqualsLower(S.ENZYMIC_CVT)).FirstOrDefault();
                        string freeTextToAdd = !string.IsNullOrEmpty(enzymicRSN.FreeText) ? RV.GetStageInfoWithOutFreeText(enzymicRSN.FreeText) : string.Empty;
                        Rsns.Add(new RsnVM
                        {
                            CvtText = S.BIOTRANSFORMATION_CVT,
                            FreeText = RsnLevel == RsnLevel.STAGE ? $"{S.BIOTRANSFORMATION_FREETEXT} {freeTextToAdd}" : string.Empty,
                            IsRXN = true,
                            Stage = enzymicRSN.Stage != null ? enzymicRSN.Stage : null,
                            Reaction = ReactionVM,
                            RSNWindowVM = this,
                            Id = Guid.NewGuid()
                        });
                    }
                    ClearEditForm.Execute(this);
                }
                else
                    AppInfoBox.ShowInfoMessage("Either CVT or FreeText mandatory to save RSN");

            }
            catch (Exception ex)
            {
                Log.This(ex);
                AppErrorBox.ShowErrorMessage(ex.Message, ex.ToString());
            }
        }

        private void DoClearEditForm(object obj)
        {
            try
            {
                disableAppendMode = true;
                CVT = String.Empty;
                FreeText = String.Empty;
                disableAppendMode = false;
                EditingRsn = null;
                FreeTextMode = FreeTextMode.REPLACE;
                FilterCVT = string.Empty;
                FilterFreeText = string.Empty;
                IsIgnorableInDelivery = false;
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        public void ResetWindow()
        {
            try
            {
                ClearEditForm.Execute(this);
                Rsns = null;
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private void WhenCVTSelected(object obj)
        {
            try
            {
                if (SelectedCVT != null)
                {
                    CVT = SelectedCVT.Text;
                    IsIgnorableInDelivery = SelectedCVT.IsIgnorableInDelivery;
                    if (!String.IsNullOrEmpty(SelectedCVT.AssociatedFreeText) && RsnLevel == RsnLevel.STAGE && ReactionVM.Stages.Count > 1)
                        FreeText = SelectedCVT.AssociatedFreeText.Trim() + (RsnLevel == RsnLevel.STAGE ? " (stage " + StageVM?.DisplayOrder + ")" : string.Empty);
                    else
                        FreeText = string.Empty;
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }
        private void WhenFreeTextSelected(object obj)
        {
            try
            {
                if (SelectedFreeText != null)
                    FreeText = SelectedFreeText.Text.Trim() + (RsnLevel == RsnLevel.STAGE ? " (stage " + StageVM?.DisplayOrder + ")" : string.Empty);
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        public void EditRsn(RsnVM rsnVM)
        {
            try
            {
                RsnLevel = rsnVM.Stage != null ? RsnLevel.STAGE : RsnLevel.REACTION;
                EditingRsn = rsnVM;
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }
        public void DeleteRsn(RsnVM rsnVM)
        {
            try
            {
                if (Rsns != null)
                {
                    var mainVM = (App.Current.MainWindow as MainWindow).DataContext as MainVM;
                    if (rsnVM.IsIgnorableInDelivery && rsnVM.ReactionParticipantId != null && rsnVM.ReactionParticipantId.Count > 0 && mainVM.TanVM.ReactionParticipants.OfReaction(ReactionVM.Id).Where(rp => rsnVM.ReactionParticipantId.Contains(rp.Id)).FirstOrDefault() != null)
                    {
                        var RP = mainVM.TanVM.ReactionParticipants.OfReaction(ReactionVM.Id).Where(rp => rsnVM.ReactionParticipantId.Contains(rp.Id)).FirstOrDefault();
                        StringBuilder sb = new StringBuilder();
                        sb.Append("Special CVT has assoiciated Participant : ");
                        sb.Append(RP.Reg);
                        sb.AppendLine();
                        sb.AppendLine("RXN : " + RP.ReactionVM.Name);
                        sb.AppendLine("Stage : " + RP.StageVM?.Name);
                        sb.AppendLine("Category : " + RP.ParticipantType);
                        sb.AppendLine("NUM : " + RP.Num);
                        sb.AppendLine("Series : " + RP.ChemicalType);
                        sb.AppendLine("Please remove participant from Reaction to delete Selected RSN");
                        AppInfoBox.ShowInfoMessage(sb.ToString());
                    }
                    else
                        Rsns.Remove(rsnVM);
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }
    }

    public enum RsnLevel
    {
        REACTION,
        STAGE
    }
    public enum FreeTextMode
    {
        REPLACE,
        APPEND
    }
}
