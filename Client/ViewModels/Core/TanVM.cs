using Client.Command;
using Client.Common;
using Client.Logging;
using Client.Models;
using Client.ViewModel;
using Client.ViewModels.Core;
using Client.ViewModels.Utils;
using Client.Views;
using Client.Views.Analogous;
using DTO;
using Newtonsoft.Json;
using ProductTracking.Models.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Excelra.Utils.Library;
using System.Windows;
using System.Windows.Data;
using System.Windows.Forms;
using Client.Notify;
using Client.Validations;
using Client.Util;
using Client.ViewModels.Extended;

namespace Client.ViewModels
{
    public class TanVM : ViewModelBase
    {
        private static readonly string PLUS_ICON = ConfigurationManager.AppSettings["PLUS_ICON"];
        private static readonly string ARROW_ICON = ConfigurationManager.AppSettings["ARROW_ICON"];
        private static readonly string CHOOSE_REACTION = "Select Reaction";

        #region For Communication between views
        public event EventHandler<TanChemicalVM> NumAssigned = delegate { };
        public event EventHandler<string> AutoSave = delegate { };
        #endregion

        #region Fields
        private int id;
        private string tanNumber, canNumber, snoFilter, numFilter, numSearch, numSearchDuplicate, curator, reviewer, qc;
        private int batchNumber;
        private ObservableCollection<RsnVM> rsns;
        private ObservableCollection<ReactionVM> reactions;
        private ReactionVM selectedReaction;
        private TanCommentsVM tanComments;
        private ObservableCollection<ReactionParticipantVM> reactionParticipants;
        private ObservableCollection<TanKeyWordsVM> tanKeyWords;
        private TanKeyWordsVM selectedKeyWord;
        private ReactionViewVM reactionView;
        private ListCollectionView participantsView, rsnView;
        private RsnVM selectedRsn;
        private bool enablePreview;
        private List<TanChemical> tanChemical;
        private String documentPath;
        private ReviewViewVM reviewViewVM;
        private ReactionParticipantVM selectedParticipant;
        public DelegateCommand CreateAnalogous { get; private set; }
        public DelegateCommand ShowAnalogous { get; private set; }
        public DelegateCommand DeleteReaction { get; private set; }
        public DelegateCommand CopyToClipBoard { get; private set; }
        public DelegateCommand SearchInPdf { get; private set; }
        private ParticipantComparater participantComparater = new ParticipantComparater();
        private RsnsComparater rsnsComparater = new RsnsComparater();
        private PropertyGroupDescription groupDescription = new PropertyGroupDescription { PropertyName = "StageVM" };
        private ReactionParticipantVM editingParticipant;
        private int rsdLength;
        private bool isQCCompleted;
        private ObservableCollection<TanDocumentsVM> pdfDocumentsList;
        private TanDocumentsVM tanDocumentVM;
        private string localDocumentPath;
        private NUMVM selectedNumPreview, selectedAMD;
        public bool RistrictFilter { get; set; }

        #endregion

        public TanVM()
        {
            //init data
            Reactions = new ObservableCollection<ReactionVM>();
            ReactionParticipants = new ObservableCollection<ReactionParticipantVM>();
            TankeyWords = new ObservableCollection<Core.TanKeyWordsVM>();
            Rsns = new ObservableCollection<RsnVM>();
            ReactionView = new ReactionViewVM();
            ReviewViewVM = new ReviewViewVM();
            TanChemicals = new List<TanChemical>();
            EnablePreview = true;
            //bindings
            var mainVm = (App.Current.MainWindow as MainWindow).DataContext as MainVM;
            DeleteReaction = new DelegateCommand(this.DeleteSelectedReaction);
            CreateAnalogous = new DelegateCommand(this.DoCreateAnalogous);
            ShowAnalogous = new DelegateCommand(this.DoShowAnalogous);
            CopyToClipBoard = new DelegateCommand(this.DoCopyToClipBoard);
            SearchInPdf = new DelegateCommand(DoSearchInPdf);

            //listeners            
            Reactions.CollectionChanged += Reactions_CollectionChanged;
            ReactionParticipants.CollectionChanged += ReactionParticipants_CollectionChanged;
            Rsns.CollectionChanged += Rsns_CollectionChanged;
            AutoSave += TanVM_AutoSave;
            (App.Current.MainWindow as MainWindow).PublishChemicalName += TanVM_PublishChemicalName;
        }

        public void SetTanKeywords(List<TanKeyWordsVM> keyWords)
        {
            TankeyWords = new ObservableCollection<TanKeyWordsVM>(keyWords);
            TankeyWords.UpdateDisplayOrder();
        }

        private void DoSearchInPdf(object obj)
        {
            if (SelectedKeyWord != null)
            {
                PdfSearch.SearchText(SelectedKeyWord.KeyWord);
                ((App.Current.MainWindow as MainWindow).DataContext as MainVM).PreviewTabIndex = 4;
            }
        }

        private void DoCreateAnalogous(object obj)
        {
            if (SelectedReaction != null && SelectedReaction.IsCurationCompleted)
                EditAnalogous.Show(SelectedReaction);
            else
                System.Windows.MessageBox.Show("Please complete reaction");
        }

        private void DoCopyToClipBoard(object obj)
        {
            try
            {
                if (SelectedKeyWord != null)
                    System.Windows.Forms.Clipboard.SetText(SelectedKeyWord.KeyWord);
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        public void PerformAutoSave(string Action)
        {
            try
            {
                AutoSave.Invoke(this, Action);
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private void TanVM_AutoSave(object sender, string Action)
        {
            try
            {
                if (U.RoleId <= 3)
                {
                    var TAN = ViewModelToModel.GetTanFromViewModel((App.Current.MainWindow as MainWindow).DataContext as MainVM);
                    TAN.DocumentCurrentPage = (App.Current.MainWindow as MainWindow).GetCurrentPdfPage();
                    (App.Current.MainWindow as MainWindow).TanData.Data = JsonConvert.SerializeObject(TAN);
                    System.IO.File.WriteAllText(T.TanDataFilePath, JsonConvert.SerializeObject((App.Current.MainWindow as MainWindow).TanData));
                    //((App.Current.MainWindow as MainWindow).DataContext as MainVM).Validate();
                    //                    (App.Current.MainWindow as MainWindow).TanSavedEvent(TAN);
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }


        #region Properties
        public int Id { get { return id; } set { SetProperty(ref id, value); } }
        public TanCommentsVM TanComments { get { return tanComments; } set { SetProperty(ref tanComments, value); } }
        public String NumFilter
        {
            get { return numFilter; }
            set
            {
                SetProperty(ref numFilter, value);
                if (!String.IsNullOrEmpty(value))
                    ReactionsView.Filter = FilterNum;
                else
                    ReactionsView.Filter = null;
            }
        }
        bool FilterNum(object reactionVMObject)
        {

            try
            {
                if (reactionVMObject != null && reactionVMObject is ReactionVM)
                {
                    int num = -1;
                    Int32.TryParse(NumFilter, out num);
                    var reaction = reactionVMObject as ReactionVM;
                    return ReactionParticipants.Any(rp => rp.ReactionVM.Id == reaction.Id && rp.ParticipantType == ParticipantType.Product && rp.Num == num);

                }
                return false;
            }
            catch (Exception ex)
            {
                Log.This(ex);
                return false;
            }
        }
        public String SnoFilter
        {
            get { return snoFilter; }
            set
            {
                SetProperty(ref snoFilter, value);
                if (!String.IsNullOrEmpty(value))
                {
                    int sno = -1;
                    Int32.TryParse(snoFilter, out sno);
                    ReactionsView.Filter = (r) => r != null && (r as ReactionVM).DisplayOrder == sno;
                }
                else
                    ReactionsView.Filter = null;
            }
        }

        public String NumSearch
        {
            get { return numSearch; }
            set
            {
                SetProperty(ref numSearch, value);
                if (NUMPreviewView != null && NUMPreviewView.Count > 0)
                {
                    if (!String.IsNullOrEmpty(value) && !RistrictFilter)
                    {
                        try
                        {
                            NUMPreviewView.Filter = (r) => r != null &&
                            ((r as NUMVM).Num.ToString().SafeContainsLower(value) ||
                            (r as NUMVM).Formula.SafeContainsLower(value) ||
                            (r as NUMVM).Name.SafeContainsLower(value) ||
                            (r as NUMVM).RegNumber.SafeContainsLower(value) ||
                            (r as NUMVM).ABSSterio.SafeContainsLower(value) ||
                            (r as NUMVM).PeptideSequence.SafeContainsLower(value) ||
                            (r as NUMVM).OtherName.SafeContainsLower(value) ||
                            (r as NUMVM).NuclicAcidSequence.SafeContainsLower(value));
                        }
                        catch (Exception ex)
                        {
                            Log.This(ex);
                        }
                    }
                    else
                        NUMPreviewView.Filter = (r) => r != null && (r as NUMVM).ChemicalType == ChemicalType.NUM;
                }
            }
        }

        public void SetReactions(List<ReactionVM> reactions, int InsertPosition = 0)
        {
            try
            {
                foreach (var reaction in reactions)
                {
                    if (U.RoleId == 1)
                        reaction.LastupdatedDate = DateTime.Now;
                    else if (U.RoleId == 2)
                        reaction.ReviewLastupdatedDate = DateTime.Now;
                    else if (U.RoleId == 3)
                        reaction.QCLastupdatedDate = DateTime.Now;
                    Reactions.Insert(InsertPosition, reaction);
                    InsertPosition++;
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        public bool ReplaceFreeTexts(string TargetText, string ReplaceText, MainVM mainvm, out string ResponceText, List<Guid> rsnIds = null)
        {
            if (Rsns != null && Rsns.Any())
            {
                bool FreetextUpdated = false;
                var freetexts = Rsns.Where(rsn => (rsnIds != null ? rsnIds.Contains(rsn.Id) : true) && rsn.FreeText != null && rsn.FreeText.Contains(TargetText));
                foreach (var freetext in freetexts)
                {
                    string ReplacedText = freetext.FreeText.Replace(TargetText, ReplaceText);
                    if (RV.ValidateRsnReactionLevel(freetext.Reaction, freetext.Stage, freetext.Stage == null ? RsnLevel.REACTION : RsnLevel.STAGE, freetext.CvtText, ReplacedText, Rsns.ToList(), out ResponceText, freetext))
                    {
                        if (!RV.ValidateRsnFreetext(ReplacedText, freetext.Reaction, freetext.Stage, freetext.Stage == null ? RsnLevel.REACTION : RsnLevel.STAGE, out ResponceText))
                            return false;
                    }
                    else
                        return false;
                }
                foreach (var freetext in freetexts)
                {
                    FreetextUpdated = true;
                    freetext.FreeText = freetext.FreeText.Replace(TargetText, ReplaceText);
                }
                if (FreetextUpdated)
                {
                    TanVM_AutoSave(null, null);
                    mainvm.Validate();
                }
                ResponceText = string.Empty;
                return true;
            }
            ResponceText = string.Empty;
            return true;
        }

        public String NumSearchduplicate
        {
            get { return numSearchDuplicate; }
            set
            {
                SetProperty(ref numSearchDuplicate, value);
                if (!String.IsNullOrEmpty(value))
                {
                    try
                    {
                        int sno = -1;
                        Int32.TryParse(numSearchDuplicate, out sno);
                        NUMPreviewViewduplicate.Filter = (r) => r != null &&
                                                                ((r as NUMVM).Num.ToString().SafeContainsLower(value) ||
                                                                (r as NUMVM).Formula.SafeContainsLower(value) ||
                                                                (r as NUMVM).Name.SafeContainsLower(value) ||
                                                                (r as NUMVM).RegNumber.SafeContainsLower(value) ||
                                                                (r as NUMVM).ABSSterio.SafeContainsLower(value) ||
                                                                (r as NUMVM).PeptideSequence.SafeContainsLower(value) ||
                                                                (r as NUMVM).OtherName.SafeContainsLower(value) ||
                                                                (r as NUMVM).NuclicAcidSequence.SafeContainsLower(value));
                    }
                    catch (Exception ex)
                    {
                        Log.This(ex);
                    }
                }
                else
                    NUMPreviewViewduplicate.Filter = (r) => r != null && (r as NUMVM).ChemicalType == ChemicalType.NUM;
            }
        }

        public String TanNumber { get { return tanNumber; } set { SetProperty(ref tanNumber, value); } }
        public String Curator { get { return curator; } set { SetProperty(ref curator, value); } }
        public String Reviewer { get { return reviewer; } set { SetProperty(ref reviewer, value); } }
        public String QC { get { return qc; } set { SetProperty(ref qc, value); } }
        public String DocumentPath { get { return documentPath; } set { SetProperty(ref documentPath, value); } }
        public int BatchNumber { get { return batchNumber; } set { SetProperty(ref batchNumber, value); } }
        public String CanNumber { get { return canNumber; } set { SetProperty(ref canNumber, value); } }
        public ReviewViewVM ReviewViewVM { get { return reviewViewVM; } set { SetProperty(ref reviewViewVM, value); } }
        public ObservableCollection<TanDocumentsVM> PdfDocumentsList { get { return pdfDocumentsList; } set { SetProperty(ref pdfDocumentsList, value); } }
        public TanDocumentsVM SelectedDocumentVM { get { return tanDocumentVM; } set { SetProperty(ref tanDocumentVM, value); } }
        public String LocalDocumentPath { get { return localDocumentPath; } set { SetProperty(ref localDocumentPath, value); } }
        public ReactionParticipantVM SelectedParticipant
        {
            get { return selectedParticipant; }
            set
            {
                SetProperty(ref selectedParticipant, value);
            }
        }
        public ReactionParticipantVM EditingParticipant
        {
            get { return editingParticipant; }
            set
            {
                SetProperty(ref editingParticipant, value);
            }
        }
        public DelegateCommand DeleteParticipant { get; private set; }
        public bool EnablePreview
        {
            get { return enablePreview; }
            set
            {
                SetProperty(ref enablePreview, value);
                if (value == true)
                {
                    if (Reactions != null)
                        Reactions.UpdateDisplayOrder();
                    RefreshReactionNames();
                    Rsns.UpdateDisplayOrder();
                    UpdateReactionPreview();
                }
            }
        }
        public ObservableCollection<ReactionVM> Reactions
        {
            get
            {
                return reactions;
            }
            private set
            {
                SetProperty(ref reactions, value);
            }
        }
        private ListCollectionView reactionsView;
        public ListCollectionView ReactionsView { get { return reactionsView; } set { SetProperty(ref reactionsView, value); } }
        public ReactionViewVM ReactionView { get { return reactionView; } set { SetProperty(ref reactionView, value); } }

        public RsnVM SelectedRsn
        {
            get { return selectedRsn; }
            set
            {
                SetProperty(ref selectedRsn, value);
                if (value != null && value.Stage != null && value.Stage.DisplayOrder > 1)
                    value.ChooseRXN = Visibility.Hidden;
                else if (value != null)
                {
                    value.IsRXN = true;
                }
            }
        }
        public ObservableCollection<RsnVM> Rsns
        {
            get { return rsns; }
            private set
            {
                SetProperty(ref rsns, value);
            }
        }

        public void SetRsns(ObservableCollection<RsnVM> rsns)
        {
            try
            {
                Rsns = rsns;
                UpdateRSNView();
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }
        public ObservableCollection<ReactionParticipantVM> ReactionParticipants
        {
            get { return reactionParticipants; }
            private set
            {
                SetProperty(ref reactionParticipants, value);
                RefreshReactionNames();
            }
        }

        public string Rsd { get { return rsd; } set { SetProperty(ref rsd, value); RsdLength = value.Length; } }
        public int RsdLength { get { return rsdLength; } set { SetProperty(ref rsdLength, value); } }
        public ObservableCollection<TanKeyWordsVM> TankeyWords { get { return tanKeyWords; } set { SetProperty(ref tanKeyWords, value); } }
        public TanKeyWordsVM SelectedKeyWord { get { return selectedKeyWord; } set { SetProperty(ref selectedKeyWord, value); } }
        public bool IsQCCompleted
        {
            get { return isQCCompleted; }
            set
            {
                if (value && ((App.Current.MainWindow as MainWindow).DataContext as MainVM).Validate())
                    SetProperty(ref isQCCompleted, value);
                else if (!value)
                    SetProperty(ref isQCCompleted, value);
                else
                    SetProperty(ref isQCCompleted, false);
            }
        }

        private ListCollectionView nUMPreviews;
        private ListCollectionView nUMPreviewsduplicate;
        public ListCollectionView NUMPreviewView { get { return nUMPreviews; } set { SetProperty(ref nUMPreviews, value); } }
        public List<NUMVM> TanNums { get { return tanNums; } set { SetProperty(ref tanNums, value); } }
        public ListCollectionView NUMPreviewViewduplicate { get { return nUMPreviewsduplicate; } set { SetProperty(ref nUMPreviewsduplicate, value); } }
        private ListCollectionView unusedNums;
        private ListCollectionView aMDNums;
        private string rsd;
        private List<NUMVM> tanNums;

        public ListCollectionView UnusedNums { get { return unusedNums; } set { SetProperty(ref unusedNums, value); } }
        public ListCollectionView AMDNums { get { return aMDNums; } set { SetProperty(ref aMDNums, value); } }
        public List<TanChemical> TanChemicals { get { return tanChemical; } set { SetProperty(ref tanChemical, value); } }
        public NUMVM SelectedNumPreview { get { return selectedNumPreview; } set { SetProperty(ref selectedNumPreview, value); } }

        public NUMVM SelectedAMD
        {
            get { return selectedAMD; }
            set
            {
                SetProperty(ref selectedAMD, value);
                if (value != null)
                {
                    var mainVM = (App.Current.MainWindow as MainWindow).DataContext as MainVM;
                    if (value.Num > 0)
                    {
                        mainVM.PreviewTabIndex = 3;
                        RistrictFilter = true;
                        NumSearch = value.Num.ToString();
                        RistrictFilter = false;
                        if (NUMPreviewView != null)
                            NUMPreviewView.Filter = (n) => n != null && ((n as NUMVM).Num == value.Num);
                    }
                }
            }
        }

        public void RefreshReactionNames()
        {
            var start = DateTime.Now;
            try
            {
                foreach (var reactionVM in Reactions)
                    reactionVM.RefreshName();
                //if (ReactionParticipants != null)
                //    foreach (var item in ReactionParticipants)
                //    {
                //        ReactionParticipantVM participant = item as ReactionParticipantVM;
                //        if (participant.ParticipantType == ParticipantType.Product)
                //            participant.ReactionVM.RefreshName();
                //    }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
            var end = DateTime.Now;
            Debug.WriteLine("RefreshReactionNames : " + (end - start).TotalSeconds + " Seconds");
        }

        #endregion
        #region Utility Methods
        public void UpdateParticipantsView()
        {
            var start = DateTime.Now;
            try
            {
                var mainViewModel = (App.Current.MainWindow as MainWindow).DataContext as MainVM;
                #region Curation View Update
                if (mainViewModel != null && SelectedReaction != null && EnablePreview)
                {
                    String rxnTitle = "RXN : " + SelectedReaction.Name;
                    String stageTitle = String.Empty;

                    ParticipantsView = new ListCollectionView(ReactionParticipants.Where(rp => rp.ReactionVM.Id == SelectedReaction.Id).ToList());
                    ParticipantsView.GroupDescriptions.Clear();
                    ParticipantsView.SortDescriptions.Clear();

                    //Only Reaction Selected.
                    if (SelectedReaction.SelectedStage == null)
                    {
                        stageTitle = "All Stages";
                        ParticipantsView.Filter = (p) => p != null && (p as ReactionParticipantVM).ReactionVM == SelectedReaction;
                        ParticipantsView.GroupDescriptions.Add(groupDescription);
                    }

                    //Stage also selected
                    else
                    {
                        stageTitle = SelectedReaction.SelectedStage.Name;
                        ParticipantsView.Filter = (p) => (
                        p != null &&
                        (
                         ((p as ReactionParticipantVM).ReactionVM == SelectedReaction && (p as ReactionParticipantVM).StageVM == SelectedReaction.SelectedStage) ||
                          ((p as ReactionParticipantVM).ReactionVM == SelectedReaction && (p as ReactionParticipantVM).StageVM == null)
                         ));

                    }
                    ParticipantsView.CustomSort = participantComparater;
                    ParticipantsView.Refresh();

                    UpdateRSNView();

                    mainViewModel.ReactionLevelTitle = rxnTitle + " - " + stageTitle;
                    var end = DateTime.Now;
                    Debug.WriteLine("UpdateparticipantsView : " + (end - start).TotalSeconds + " Seconds");
                }
                else
                {
                    ParticipantsView = null;
                    mainViewModel.ReactionLevelTitle = CHOOSE_REACTION;
                }
                #endregion
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }
        public void UpdateRSNView()
        {
            try
            {
                if (SelectedReaction != null)
                {
                    RsnsView = new ListCollectionView(Rsns);
                    RsnsView.Filter = RsnFilter.Filter;
                    RsnsView.Refresh();
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        public void UpdateReactionPreview(bool fromRefresh = false)
        {
            try
            {
                var start = DateTime.Now;
                var mainViewModel = (App.Current.MainWindow as MainWindow).DataContext as MainVM;
                #region Curator View
                if (ReactionView != null)
                {
                    if (SelectedReaction != null && ReactionParticipants != null)
                    {
                        ReactionView = ReactionViewBuilder.GetReactionView(SelectedReaction, mainViewModel.TanVM.ReactionParticipants.ToList());
                        var end = DateTime.Now;
                        Debug.WriteLine("RXN Preview : " + (end - start).TotalSeconds + " Seconds");
                    }
                    else
                        ReactionView.Name = "Select Reaction To Preview . .";
                }
                #endregion

                #region Review View
                if (fromRefresh)
                {
                    ReviewViewVM.IsQCCompleted = IsQCCompleted;
                    ReviewViewVM.QcCompleteCheckVisibility = U.RoleId == 3 ? Visibility.Visible : Visibility.Hidden;
                    ReviewViewVM.ReactionViews.Clear();
                    QCTAbleViewVM qc = new QCTAbleViewVM();
                    if (Reactions != null && Reactions.Count > 0 && mainViewModel.TanVM != null && mainViewModel.TanVM.ReactionParticipants != null)
                    {
                        qc.PrepareData(mainViewModel.TanVM);
                        ReviewViewVM.QCTAbleViewVM = qc;
                        foreach (var reactionVM in Reactions)
                        {
                            var view = ReactionViewBuilder.GetReactionView(reactionVM, mainViewModel.TanVM.ReactionParticipants.ToList());
                            view.EnableEdit = Visibility.Visible;
                            ReviewViewVM.ReactionViews.Add(view);
                        }
                        if (SelectedReaction != null)
                            ReviewViewVM.SelectedReviewReaction = ReviewViewVM.ReactionViews.Where(rv => rv.ReactionId == SelectedReaction.Id).FirstOrDefault();
                    }
                    var end = DateTime.Now;
                    Debug.WriteLine("RXN Preview : " + (end - start).TotalSeconds + " Seconds");
                }
                #endregion
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }
        public ListCollectionView ParticipantsView
        {
            get { return participantsView; }
            set { SetProperty(ref participantsView, value); }
        }
        public ListCollectionView RsnsView
        {
            get { return rsnView; }
            set { SetProperty(ref rsnView, value); }
        }
        public ReactionVM SelectedReaction
        {
            get { return selectedReaction; }
            set
            {
                var start = DateTime.Now;
                Debug.WriteLine("SelectedReaction Change Started : " + start);
                SetProperty(ref selectedReaction, value);

                if (EnablePreview && value != null)
                {
                    UpdateStageNames(value);
                    UpdateParticipantsView();
                    UpdateReactionPreview();
                    UpdateRSNView();
                    Rsd = Common.ReactionValidations.GetRSDString(ReactionParticipants.OfReaction(value.Id), SelectedReaction);
                    if (value.Stages != null && value.Stages.Count == 1)
                        SelectedReaction.SelectedStage = value.Stages.FirstOrDefault();
                    if (value != null && ReviewViewVM != null && ReviewViewVM.ReactionViews != null)
                        ReviewViewVM.SelectedReviewReaction = ReviewViewVM.ReactionViews.Where(rv => rv.ReactionId == value.Id).FirstOrDefault();
                    var end = DateTime.Now;
                    Debug.WriteLine($"SelectedReaction Changed at {end} : " + (end - start).TotalSeconds + " Seconds");
                }
            }
        }
        public void UpdateStageNames(ReactionVM SelectedReaction)
        {
            try
            {
                if (SelectedReaction != null)
                    foreach (var stage in SelectedReaction.Stages)
                        stage.RefreshName();
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }
        private void DoShowAnalogous(object obj)
        {
            try
            {
                ((App.Current.MainWindow as MainWindow).DataContext as MainVM).ShowAnalogous.Execute(this);
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }
        private void DeleteSelectedReaction(object obj)
        {
            try
            {
                ((App.Current.MainWindow as MainWindow).DataContext as MainVM).DeleteReaction.Execute(this);
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }
        #endregion
        #region Collection Changes
        private void Reactions_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            try
            {
                if (EnablePreview)
                {
                    Reactions.UpdateDisplayOrder();
                    RefreshReactionNames();
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }
        private void Rsns_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var start = DateTime.Now;
            try
            {
                if (EnablePreview)
                {
                    if (e.NewItems != null)
                    {
                        foreach (RsnVM rsn in e.NewItems)
                        {
                            rsn.Reaction = SelectedReaction;
                            rsn.Stage = SelectedReaction.SelectedStage != null && SelectedReaction.SelectedStage.DisplayOrder > 1 ? SelectedReaction.SelectedStage : null;
                        }
                    }
                    rsns.UpdateDisplayOrder();
                    AutoSave.Invoke(this, "Rsns Updated");
                    if (U.RoleId == 1)
                        SelectedReaction.IsCurationCompleted = false;
                    else if (U.RoleId == 2)
                        SelectedReaction.IsReviewCompleted = false;
                    var end = DateTime.Now;
                    Debug.WriteLine("Rsns_CollectionChanged : " + (end - start).TotalSeconds + " Seconds");
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }
        private void ReactionParticipants_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var start = DateTime.Now;
            try
            {
                if (EnablePreview)
                {
                    UpdateParticipantsView();
                    UpdateReactionPreview();
                    RefreshReactionNames();
                    Rsd = Common.ReactionValidations.GetRSDString(ReactionParticipants.OfReaction(SelectedReaction.Id), SelectedReaction);
                    SelectedReaction.IsCurationCompleted = false;
                    UpdateUnusedNUMs();
                    //if (U.RoleId == 1)
                    //{
                    //    SelectedReaction.LastupdatedDate = DateTime.Now;
                    //    SelectedReaction.IsCurationCompleted = false;
                    //}
                    //else if (U.RoleId == 2)
                    //{
                    //    SelectedReaction.ReviewLastupdatedDate = DateTime.Now;
                    //    SelectedReaction.IsReviewCompleted = false;
                    //}
                    //else if (U.RoleId == 3)
                    //    SelectedReaction.QCLastupdatedDate = DateTime.Now;
                    var end = DateTime.Now;
                    Debug.WriteLine("ReactionParticipantsCollectionChanged: " + (end - start).TotalSeconds + " Seconds");
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }
        public void UpdateUnusedNUMs()
        {
            var start = DateTime.Now;
            if (ReactionParticipants != null && TanChemicals != null)
            {
                var unusedNums = TanChemicals.Select(tc => tc.NUM).Except(ReactionParticipants.Select(rp => rp.Num)).Where(n => n < 9000).OrderBy(n => n);
                var chemicals = TanChemicals.Where(tc => unusedNums.Contains(tc.NUM)).OrderBy(num => num.NUM);
                var unUsedNumsList = new List<NUMVM>();
                foreach (var chemical in chemicals)
                {
                    unUsedNumsList.Add(new NUMVM
                    {
                        ChemicalType = chemical.ChemicalType,
                        Num = chemical.NUM
                    });
                }
                UnusedNums = new ListCollectionView(unUsedNumsList);
                PropertyGroupDescription groupDescription = new PropertyGroupDescription("ChemicalType");
                UnusedNums.GroupDescriptions.Add(groupDescription);
            }
            var end = DateTime.Now;
            Debug.WriteLine("UpdateUnUsedNums: " + (end - start).TotalSeconds + " Seconds");
        }

        public void UpdateNumsView()
        {
            var start = DateTime.Now;
            var nums = TanChemicals.Where(t => t.ChemicalType != ChemicalType.S9000).OrderBy(n => n.NUM);
            var numsListView = new List<NUMVM>();
            foreach (var num in nums)
            {
                var chemical = ViewModelToModel.GetTanChemicalVMFromTanchemical(num);
                numsListView.Add(new NUMVM
                {
                    Num = num.NUM,
                    Name = !string.IsNullOrEmpty(num.Name) ? num.Name : string.Empty,
                    Formula = !string.IsNullOrEmpty(num.Formula) ? num.Formula : string.Empty,
                    RegNumber = !string.IsNullOrEmpty(num.RegNumber) ? num.RegNumber : string.Empty,
                    ChemicalType = num.ChemicalType,
                    Chemical = chemical.Item1,
                    OtherName = !string.IsNullOrEmpty(num.OtherName) ? num.OtherName : string.Empty,
                    ABSSterio = !string.IsNullOrEmpty(num.ABSSterio) ? num.ABSSterio : string.Empty,
                    PeptideSequence = !string.IsNullOrEmpty(num.PeptideSequence) ? num.PeptideSequence : string.Empty,
                    NuclicAcidSequence = !string.IsNullOrEmpty(num.NuclicAcidSequence) ? num.NuclicAcidSequence : string.Empty,
                    Images = chemical.Item2
                });
            }
            NUMPreviewView = new ListCollectionView(numsListView);
            TanNums = numsListView;
            NUMPreviewViewduplicate = new ListCollectionView(numsListView);
            var end = DateTime.Now;
            Debug.WriteLine("UpdateNumsView : " + (end - start).TotalSeconds + " Seconds");
        }
        #endregion
        private void TanVM_PublishChemicalName(object sender, TanChemicalVM chemicalNameVM)
        {
            var start = DateTime.Now;
            try
            {
                var mainViewModel = (App.Current.MainWindow as MainWindow).DataContext as MainVM;
                //EnablePreview = true;
                bool IsValidState = true;
                if (mainViewModel != null &&
                    mainViewModel.TanVM != null &&
                    mainViewModel.IsParticipatTypeSelected &&
                    SelectedReaction != null && chemicalNameVM != null)
                {
                    if (mainViewModel.ParticipantType != ParticipantType.Product && mainViewModel.TanVM.SelectedReaction.SelectedStage == null)
                        IsValidState = false;
                    var tanChemical = new TanChemical
                    {
                        Id = chemicalNameVM.Id,
                        Name = chemicalNameVM.Name,
                        RegNumber = chemicalNameVM.RegNumber,
                        NUM = chemicalNameVM.NUM
                    };
                    var existingParticipant = ParticipantValidations.AlreadyContains(tanChemical, null, SelectedReaction, mainViewModel.ParticipantType, ReactionParticipants.OfReaction(SelectedReaction.Id).ToList(), Rsns.ToList());
                    Debug.WriteLine("ParticipantAdding existingParticipant Validations Done in " + (DateTime.Now - start).TotalSeconds + " Seconds");
                    if (existingParticipant != null)
                    {
                        if (existingParticipant.ChemicalType == ChemicalType.S8000 && existingParticipant.Num != chemicalNameVM.NUM)
                            existingParticipant = null;
                    }
                    if (IsValidState)
                    {
                        if (existingParticipant == null)
                        {
                            var participants = (from p in ReactionParticipants where p.ParticipantType == ParticipantType.Product && p.KeyProduct && p.Num == chemicalNameVM.NUM select p).ToList();
                            bool continueAdding = true;
                            if (participants != null && participants.Count > 0 && mainViewModel.ParticipantType == ParticipantType.Product && ReactionParticipants.Where(rp => rp.ReactionVM != null && rp.ReactionVM.Id == SelectedReaction.Id && rp.KeyProduct).Count() == 0)
                            {
                                DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("With this Product it will create Reaction Sequence/alternative preparation" + (participants.Count + 1) + ". Do you want to continue?", "Reactions", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                                if (dialogResult == DialogResult.No)
                                    continueAdding = false;
                            }
                            if (continueAdding)
                            {
                                Debug.WriteLine("ParticipantAdding Validations Done in " + (DateTime.Now - start).TotalSeconds + " Seconds");
                                if (chemicalNameVM.Id == Guid.Empty)
                                    chemicalNameVM.Id = Guid.NewGuid();
                                TanChemical tanchemical = (from p in TanChemicals where p.Id == chemicalNameVM.Id && p.NUM == chemicalNameVM.NUM select p).FirstOrDefault();
                                if (tanchemical == null)
                                    TanChemicalsCrud.AddTanChemicalToList(TanChemicals, chemicalNameVM, Id);
                                var participant = new ReactionParticipantVM
                                {
                                    Name = chemicalNameVM.Name,
                                    Num = chemicalNameVM.NUM,
                                    Reg = chemicalNameVM.RegNumber,
                                    ReactionVM = SelectedReaction,
                                    ChemicalType = chemicalNameVM.ChemicalType,
                                    TanChemicalId = chemicalNameVM.Id,
                                    Id = Guid.NewGuid(),
                                    Formula = tanchemical != null ? tanchemical.Formula : string.Empty
                                };
                                //for product
                                if (mainViewModel.ParticipantType == ParticipantType.Product)
                                {
                                    participant.StageVM = null;
                                    participant.ParticipantType = ParticipantType.Product;
                                }
                                //other participants
                                else if (SelectedReaction.SelectedStage != null)
                                {
                                    participant.StageVM = SelectedReaction.SelectedStage;
                                    participant.ParticipantType = mainViewModel.ParticipantType;
                                }
                                //display order
                                int? displayOrder = ReactionParticipants.
                                    Where(rp => rp.ReactionVM.Id == SelectedReaction.Id && rp.ParticipantType == mainViewModel.ParticipantType).
                                    Select(rp => (int?)rp.DisplayOrder).Max();
                                participant.DisplayOrder = displayOrder == null ? 1 : displayOrder.Value + 1;
                                if (participant.ParticipantType == ParticipantType.Product && !ReactionParticipants.Where(rp => rp.ParticipantType == ParticipantType.Product && rp.ReactionVM.Id == SelectedReaction.Id).Any())
                                    participant.KeyProduct = true;
                                //add finally
                                if (EditingParticipant != null)
                                {
                                    var targetParticipant = (from rp in ReactionParticipants where rp.Id == EditingParticipant.Id select rp).FirstOrDefault();
                                    if (targetParticipant != null)
                                    {
                                        targetParticipant.ChemicalType = participant.ChemicalType;
                                        targetParticipant.Formula = participant.Formula;
                                        targetParticipant.Name = participant.Name;
                                        targetParticipant.Num = participant.Num;
                                        targetParticipant.Reg = participant.Reg;
                                        targetParticipant.TanChemicalId = participant.TanChemicalId;
                                        EditingParticipant = null;
                                        mainViewModel.Validate(true);
                                    }
                                    if (U.RoleId == 1)
                                    {
                                        SelectedReaction.LastupdatedDate = DateTime.Now;
                                        SelectedReaction.IsCurationCompleted = false;
                                    }
                                    else if (U.RoleId == 2)
                                    {
                                        SelectedReaction.ReviewLastupdatedDate = DateTime.Now;
                                        SelectedReaction.IsReviewCompleted = false;
                                    }
                                    else if (U.RoleId == 3)
                                        SelectedReaction.QCLastupdatedDate = DateTime.Now;
                                    AutoSave.Invoke(this, "Participant Updated");
                                }
                                else
                                {
                                    var rsnVM = Rsns.Where(rsn => rsn.Reaction.Id == SelectedReaction.Id && rsn.Stage != null && rsn.IsIgnorableInDelivery && rsn.SelectedChemical != null && rsn.SelectedChemical.Id == chemicalNameVM.Id).FirstOrDefault();
                                    if (rsnVM != null)
                                    {
                                        rsnVM.SelectedChemical = null;
                                        if (rsnVM.ReactionParticipantId == null)
                                            rsnVM.ReactionParticipantId = new List<Guid>();
                                        rsnVM.ReactionParticipantId.Add(participant.Id);
                                    }
                                    ReactionParticipants.Add(participant);
                                    if (U.RoleId == 1)
                                    {
                                        SelectedReaction.LastupdatedDate = DateTime.Now;
                                        SelectedReaction.IsCurationCompleted = false;
                                    }
                                    else if (U.RoleId == 2)
                                    {
                                        SelectedReaction.ReviewLastupdatedDate = DateTime.Now;
                                        SelectedReaction.IsReviewCompleted = false;
                                    }
                                    else if (U.RoleId == 3)
                                        SelectedReaction.QCLastupdatedDate = DateTime.Now;
                                    if (participant.ChemicalType == ChemicalType.S8000 && mainViewModel.S8000Chemicals.Where(C => C.Id == participant.TanChemicalId).Count() == 0)
                                        mainViewModel.S8000Chemicals.Add(chemicalNameVM);
                                    AutoSave.Invoke(this, "Participant Added");
                                    mainViewModel.Validate(true);
                                }
                            }
                        }
                        else
                            existingParticipant.DisplayDuplicateString(chemicalNameVM.RegNumber);
                    }
                    else
                        AppInfoBox.ShowInfoMessage("Please Select stage to Add participants");
                    mainViewModel.PreviewTabIndex = 0;
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
            var end = DateTime.Now;
            Debug.WriteLine("Publish Chemical Preview : " + (end - start).TotalSeconds + " Seconds");
        }
        public void UpdateChemicalName(TanChemicalVM chemicalNameVM)
        {
            var start = DateTime.Now;
            try
            {
                if (TanChemicals != null)
                {
                    var tanChemicals = TanChemicals.Where(tc => tc.NUM == chemicalNameVM.NUM).FirstOrDefault();
                    tanChemicals.CompoundNo = chemicalNameVM.CompoundNo;
                    tanChemicals.GenericName = chemicalNameVM.GenericName;
                    tanChemicals.OtherName = chemicalNameVM.SearchName;
                    tanChemicals.ImagePath = chemicalNameVM.ImagePath;
                    tanChemicals.MolString = chemicalNameVM.MolString;
                    tanChemicals.Name = chemicalNameVM.Name;
                    tanChemicals.RegNumber = chemicalNameVM.RegNumber;
                    var reactionParticipants = ReactionParticipants.OfNum(chemicalNameVM.NUM);
                    if (reactionParticipants != null)
                    {
                        foreach (var participant in reactionParticipants)
                        {
                            participant.Name = chemicalNameVM.Name;
                            participant.Num = chemicalNameVM.NUM;
                            participant.Reg = chemicalNameVM.RegNumber;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
            var end = DateTime.Now;
            Debug.WriteLine("Update Chemical Name : " + (end - start).TotalSeconds + " Seconds");
        }
    }
}
