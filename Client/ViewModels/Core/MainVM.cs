using Client.Command;
using Client.Common;
using Client.Logging;
using Client.Models;
using Client.Styles;
using Client.ViewModels.Extended;
using Client.Views;
using Client.Util;
using Client.Views.Pdf;
using DTO;
using Entities;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Client.Export;
using System.Collections.Generic;

namespace Client.ViewModels
{
    public partial class MainVM : ViewModelBase
    {
        private static readonly string WEB_SEARCH = "Web Search";
        public TaskSheet taskSheet;
        public LoginWindow loginForm;
        public TanCommentWindow tanCommentwindow;
        private bool approveVisible;
        private bool rejectVisible;
        private bool submitVisible;
        private Visibility isTanLoaded;
        private Visibility cancleTan;
        private Visibility showDeliveryTab;
        private ValidationVM validationVM;
        private TanVM tanVm;
        private ViewAnalogousVM analogVM;
        private string browserTitle, browserUrl;
        private String reactionLevelTitle;
        private int previewTabIndex, bottomTabIndex;
        private ParticipantType participantType;
        private bool showLinkVisible;
        private string userName, unUsedNumsText, signalRId;
        private Visibility progressBarVisibility;
        private string progressText, qcOrReviewView;
        private TanChemicalVM selected8000Chemical;
        private bool browserPaneActive;
        private bool mainWindowEnable;
        private bool addS8000Marking, saveEnabled, submitEnabled;
        private Visibility isYieldVisible;
        private Visibility enableEditor, enableLanding, analogourVisibility, notificationVisible, numSearchVisibility, numsSplitterVisibility, settingsVisible, enableTaskAllocation;
        private bool assignTaskVisble;
        private bool isRsnWindowOpened;

        private ObservableCollection<CvtVM> cVTData;
        private ObservableCollection<String> freeTextData;
        private ObservableCollection<TanChemicalVM> s8000Chemicals;
        private ChemicalUsedPlacesVM chemicalUsedPlacesVM;
        private ObservableCollection<UserResportsVM> userReports;
        private ObservableCollection<UserResportsVM> allUserReports;

        public event EventHandler<int> WorkingAreaTabChanged = delegate { };
        public event EventHandler<TanVM> TanClosed = delegate { };
        public event EventHandler<ReactionParticipantVM> RowDoubleClicked = delegate { };
        public event EventHandler<Common.Action> BaforeTanSubmiting = delegate { };
        public event EventHandler ChooseClicked = delegate { };
        public event EventHandler<ParticipantType> mainViewModelParticipantTypeChanged = delegate { };
        public event EventHandler<ChemicalUsedPlacesVM> ShowNumUsedInfoClicked = delegate { };
        public MainVM()
        {
            ApproveVisible = false;
            RejectVisible = false;
            IsTanLoaded = Visibility.Collapsed;
            CancleVisible = Visibility.Collapsed;
            SubmitVisible = false;
            SubmitEnabled = true;
            EnableEditor = Visibility.Hidden;
            EnableTaskAllocation = Visibility.Hidden;
            ProgressBarVisibility = Visibility.Hidden;
            AnalogourVisibility = Visibility.Hidden;
            NumSearchVisibility = Visibility.Hidden;
            AssignTaskVisble = false;
            ShowDeliveryTab = Visibility.Hidden;
            ParticipantType = ParticipantType.All;
            MainWindowEnable = true;
            CVTData = new ObservableCollection<CvtVM> { };
            FreeTextData = new ObservableCollection<String> { };
            ValidationVM = new ValidationVM();
            TanVM = new TanVM();
            NotificationVisible = Visibility.Visible;
            NumsSplitterVisibility = Visibility.Hidden;
            UnUsedNumsText = "Show UnUsed Nums";
            S8000Chemicals = new ObservableCollection<Models.TanChemicalVM>();
            UserReports = new ObservableCollection<UserResportsVM>();
            AllUserReports = new ObservableCollection<Extended.UserResportsVM>();

            #region Commands
            Refresh = new DelegateCommand(this.DoRefresh);
            AddReaction = new DelegateCommand(this.AddNewReaction);
            GotoFirst = new DelegateCommand(this.DoGotoReaction);
            AddStage = new Command.DelegateCommand(this.AddNewStage);
            DeleteReaction = new DelegateCommand(this.DeleteSelectedReaction);
            DeleteStage = new Command.DelegateCommand(this.RemoveStage);
            CopyStages = new Command.DelegateCommand(this.OpenCopystageWindow);
            OpenTaskSheet = new Command.DelegateCommand(this.LoadTasks);
            ApproveTan = new Command.DelegateCommand(this.ApproveSelectedTan);
            ShowTanComments = new Command.DelegateCommand(this.DoShowTanComments);
            ShowAnalogous = new Command.DelegateCommand(this.DoShowAnalogous);
            RejectTan = new Command.DelegateCommand(this.RejectSelectedTan);
            CancleTan = new Command.DelegateCommand(this.CloseTan);
            GenerateXML = new Command.DelegateCommand(this.DoGenerateXML);
            OpenTanPdf = new DelegateCommand(this.DoOpenTanPdf);
            OpenTanFolder = new DelegateCommand(this.DoOpenTanFolder);
            SubmitTan = new DelegateCommand(this.SubmitTanlist);
            EditSelectedParticipant = new Command.DelegateCommand(this.EditParticipant);
            ChooseChemical = new Command.DelegateCommand(this.ChooseParticipant);
            ToggleSplitter = new DelegateCommand(this.ToggleNumSplitter);
            ShowUnUsedNums = new DelegateCommand(this.ShowUnUsedNumsFromTan);
            DeleteParticipant = new Command.DelegateCommand(this.DeleteParticipantFromReaction);
            NameOfYourCommand = new Command.DelegateCommand(this.newmethod);
            ShowNumUsedPlaces = new Command.DelegateCommand(this.DoShowNumUsedPlaces);
            ShowNumInfo = new Command.DelegateCommand(this.DoShowNumInfo);
            Settings = new Command.DelegateCommand(this.DoSettings);
            ShowTans = new DelegateCommand(this.DoShowTans);
            SetMarkupPdf = new Command.DelegateCommand(this.DoSetMarkupPdf);
            OpenDocumentInOtherWindow = new DelegateCommand(this.DoOpenDocumentInOtherWindow);
            MergeTwoPDF = new DelegateCommand(this.DoMergePDF);
            SearchPdf = new DelegateCommand(this.DoSearchPdf);
            SearchAnnotations = new DelegateCommand(DoSearchAnnotations);
            ChangeTabIndex = new Command.DelegateCommand(this.DoChangeTabIndex);
            ShowSolventBoilingPoints = new DelegateCommand(this.DoShowSolventBoilingPoints);
            NumsExportToPdf = new DelegateCommand(this.DoNumsExportToPdf);
            RefreshNums = new DelegateCommand(DoRefreshNums);

            #endregion

            ChemicalUsedPlacesVM = new ChemicalUsedPlacesVM();
        }

        private void DoRefreshNums(object obj)
        {
            if (TanVM != null && TanVM.NUMPreviewView != null)
                TanVM.NUMPreviewView.Filter = (r) => r != null && (r as NUMVM).Num != 0;
        }

        private void DoNumsExportToPdf(object obj)
        {
            if (TanVM != null && TanVM.TanNums != null && TanVM.TanNums.Any())
            {
                if (!File.Exists(T.NumsExportPdfPath))
                {
                    NumsToPdf.Nums = TanVM.TanNums;
                    NumsToPdf.PdfFilePath = T.NumsExportPdfPath;
                    NumsToPdf.ExportTanNUMsToPDF();
                }
                else
                    ExportedPDF.OpenDocument(T.NumsExportPdfPath);
            }
        }

        private void DoShowSolventBoilingPoints(object obj)
        {
            SolventBoilingPointsWindow.ShowSolventBoilingPoints();
        }

        private void DoSearchPdf(object obj)
        {
            PdfSearch.ShowWindow();
        }
        private void DoSearchAnnotations(object obj)
        {
            PdfAnnotations.ShowWindow();
        }

        private void DoMergePDF(object obj)
        {
            if (TanVM.SelectedDocumentVM != null)
            {
                if (!TanVM.SelectedDocumentVM.KeyPath)
                {
                    if (!PdfMerger.CombineMultiplePDFs(new string[] { TanVM.PdfDocumentsList.Where(pdf => pdf.KeyPath).Select(pdf => pdf.LocalPath).FirstOrDefault(), TanVM.SelectedDocumentVM.LocalPath }, TanVM.PdfDocumentsList.Where(pdf => pdf.KeyPath).Select(pdf => pdf.LocalPath).FirstOrDefault()))
                        AppInfoBox.ShowInfoMessage("Failed to Merge Document.");
                }
                else
                    AppInfoBox.ShowInfoMessage("You Selected markUp Document. Please select another file to mege with MarkUp PDF.");
            }
            else
                AppInfoBox.ShowInfoMessage("Please select any document other than MarkUp Document to Merge with MarkUp Pdf.");
        }

        private void DoChangeTabIndex(object obj)
        {
            if (PreviewTabIndex == 4)
                PreviewTabIndex = 0;
            else
                PreviewTabIndex = 4;
        }

        private void DoOpenDocumentInOtherWindow(object obj)
        {
            try
            {
                if (TanVM != null && TanVM.SelectedDocumentVM != null)
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        new PdfReaderForm().OpenDocument(TanVM.SelectedDocumentVM.Path, false);
                    });
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private void DoSetMarkupPdf(object obj)
        {

            try
            {
                if (TanVM != null && TanVM.SelectedDocumentVM != null)
                {
                    var tanDocument = TanVM.PdfDocumentsList.Where(td => td.KeyPath).FirstOrDefault();
                    if (tanDocument != null)
                    {
                        tanDocument.KeyPath = false;
                    }
                    TanVM.SelectedDocumentVM.KeyPath = true;
                    TanVM.DocumentPath = TanVM.SelectedDocumentVM.Path;
                    (App.Current.MainWindow as MainWindow).UpdateMarkupDocument(TanVM.SelectedDocumentVM.Path, string.Join(",", TanVM.PdfDocumentsList.Where(k => !k.KeyPath).Select(s => s.Path).ToList()));
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private void DoShowTans(object obj)
        {
            EnableEditor = Visibility.Hidden;
            EnableLanding = Visibility.Visible;
            TaskAllocationWindow.showWindow();
        }

        private void DoSettings(object obj)
        {
        }

        public void DoRefresh(object obj)
        {

            try
            {
                TanVM.UpdateParticipantsView();
                TanVM.UpdateReactionPreview(true);
            }
            catch (Exception ex)
            {
                Log.This(ex);

            }
        }

        private void DoShowNumInfo(object obj)
        {

            try
            {
                var participant = obj as ReactionParticipantVM;
                if (participant != null)
                {
                    PrepareInfo(participant);
                    CallChemicalUsedPlacesWindow(ChemicalUsedPlacesVM);
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private void DoShowNumUsedPlaces(object obj)
        {

            try
            {
                var participant = obj as ReactionParticipantVM;
                if (participant != null)
                {
                    PrepareInfo(participant);
                    CallChemicalUsedPlacesWindow(ChemicalUsedPlacesVM);
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        public void CallChemicalUsedPlacesWindow(ChemicalUsedPlacesVM ChemicalUsedPlacesVM)
        {
            ChemicalUsedPlacesVM.PreviewTabIndex = 0;
            ShowNumUsedInfoClicked.Invoke(this, ChemicalUsedPlacesVM);
        }

        public void PrepareInfo(ReactionParticipantVM participant)
        {

            try
            {
                var participants = TanVM.ReactionParticipants.OfNum(participant.Num);
                ChemicalUsedPlacesVM.Selectedparticipant = participant;
                ChemicalUsedPlacesVM.SelectedParticipantsList = new ObservableCollection<ViewModels.ReactionParticipantVM>(participants);
                for (int i = 0; i < ChemicalUsedPlacesVM.SelectedParticipantsList.Count; i++)
                    ChemicalUsedPlacesVM.SelectedParticipantsList[i].DisplayOrder = i + 1;
                var num = TanVM.TanChemicals.Where(tc => tc.Id == participant.TanChemicalId).FirstOrDefault();
                var chemical = ViewModelToModel.GetTanChemicalVMFromTanchemical(num);

                ChemicalUsedPlacesVM.TanChemical = new ViewModels.NUMVM
                {
                    Num = num.NUM,
                    Name = !string.IsNullOrEmpty(num.Name) ? num.Name : string.Empty,
                    Formula = !string.IsNullOrEmpty(num.Formula) ? num.Formula : string.Empty,
                    RegNumber = !string.IsNullOrEmpty(num.RegNumber) ? num.RegNumber : string.Empty,
                    ChemicalType = num.ChemicalType,
                    Chemical = chemical.Item1,
                    Images = chemical.Item2
                };
            }
            catch (Exception ex)
            {
                Log.This(ex);

            }

        }

        private void ShowUnUsedNumsFromTan(object obj)
        {

            try
            {
                TanVM.NumSearch = string.Empty;
                TanVM.NumSearchduplicate = string.Empty;
                if (UnUsedNumsText.Equals("Show UnUsed Nums"))
                {
                    TanVM.NUMPreviewView.Filter = (r) => r != null && !(TanVM.ReactionParticipants.Select(rp => rp.Num).Contains((r as NUMVM).Num));
                    TanVM.NUMPreviewViewduplicate.Filter = (r) => r != null && !(TanVM.ReactionParticipants.Select(rp => rp.Num).Contains((r as NUMVM).Num));
                    UnUsedNumsText = "Show Shipment Nums";
                }
                else
                {
                    TanVM.NUMPreviewView.Filter = (r) => r != null && (r as NUMVM).ChemicalType == ChemicalType.NUM;
                    TanVM.NUMPreviewViewduplicate.Filter = (r) => r != null && (r as NUMVM).ChemicalType == ChemicalType.NUM;
                    UnUsedNumsText = "Show UnUsed Nums";
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private void ToggleNumSplitter(object obj)
        {

            try
            {
                if (NumsSplitterVisibility == Visibility.Hidden)
                    NumsSplitterVisibility = Visibility.Visible;
                else
                    NumsSplitterVisibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                Log.This(ex);

            }
        }

        private void DoGenerateXML(object obj)
        {

            var tanData = (App.Current.MainWindow as MainWindow).TanData;
            if (tanData != null)
            {
                try
                {
                    var xmlString = XmlUtils.GenerateXML(tanData);
                    var tempXmlFile = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".xml";
                    File.WriteAllText(tempXmlFile, xmlString);
                    Process.Start(tempXmlFile);
                }
                catch (Exception ex)
                {
                    Log.This(ex);
                    AppErrorBox.ShowErrorMessage("Error while generating XML : ", ex.ToString());
                }
            }
        }

        private void newmethod(object obj)
        {

            try
            {
                NumSearchVisibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                Log.This(ex);

            }
        }

        private void ChooseParticipant(object obj)
        {
            try
            {
                ChooseClicked.Invoke(this, null);
            }
            catch (Exception ex)
            {
                Log.This(ex);

            }
        }

        private void EditParticipant(object obj)
        {

            try
            {
                if (TanVM.SelectedReaction != null && TanVM.SelectedParticipant != null && !TanVM.SelectedParticipant.KeyProduct)
                {
                    TanVM.EditingParticipant = TanVM.SelectedParticipant;
                    RowDoubleClicked.Invoke(this, TanVM.SelectedParticipant);
                    if (U.RoleId == 1)
                        TanVM.SelectedReaction.LastupdatedDate = DateTime.Now;
                    else if (U.RoleId == 2)
                        TanVM.SelectedReaction.ReviewLastupdatedDate = DateTime.Now;
                    else if (U.RoleId == 3)
                        TanVM.SelectedReaction.QCLastupdatedDate = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);

            }
        }

        public DelegateCommand DeleteParticipant { get; private set; }
        public DelegateCommand NameOfYourCommand { get; private set; }
        public DelegateCommand ShowNumUsedPlaces { get; private set; }
        public DelegateCommand ShowNumInfo { get; private set; }
        public DelegateCommand Settings { get; private set; }
        public DelegateCommand ShowTans { get; private set; }
        public DelegateCommand SetMarkupPdf { get; private set; }
        public DelegateCommand OpenDocumentInOtherWindow { get; private set; }
        public DelegateCommand MergeTwoPDF { get; private set; }
        public DelegateCommand SearchPdf { get; private set; }
        public DelegateCommand SearchAnnotations { get; private set; }
        public DelegateCommand ChangeTabIndex { get; private set; }
        public DelegateCommand ShowSolventBoilingPoints { get; private set; }
        public DelegateCommand NumsExportToPdf { get; private set; }
        public DelegateCommand RefreshNums { get; private set; }

        public string Title
        {
            get
            {
                return S.VersionInfo;
            }
        }

        public string AppVersion
        {
            get
            {
                return S.Version;
            }
        }
        private void DoShowAnalogous(object obj)
        {

            try
            {
                if (TanVM != null)
                {
                    if (this.TanVM != null && this.TanVM.Reactions.Count > 0 && TanVM.SelectedReaction != null)
                    {
                        if (AnalogourVisibility == Visibility.Hidden)
                        {
                            var analogousViewVM = new ViewModels.ViewAnalogousVM();
                            analogousViewVM.MasterReactions = new ObservableCollection<ReactionVM>(TanVM.Reactions);
                            analogousViewVM.MasterReactionChanged += AnalogousVM_MasterReactionChanged;
                            AnalogVM = analogousViewVM;
                            analogousViewVM.SelectedMasterReaction = TanVM.SelectedReaction;
                            AnalogourVisibility = Visibility.Visible;
                            PreviewTabIndex = 6;
                        }
                        else
                        {
                            AnalogourVisibility = Visibility.Hidden;
                            PreviewTabIndex = 0;
                        }
                    }
                    else
                        AppInfoBox.ShowInfoMessage("No Reactions Found!");
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private void AnalogousVM_MasterReactionChanged(object sender, ReactionVM e)
        {
            try
            {
                (App.Current.MainWindow as MainWindow).PublishMasterReactionChangeInAnalogousTab(e);
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private void DoShowTanComments(object obj)
        {

            try
            {
                if (TanVM != null)
                    TanCommentWindow.ShowComments(TanVM.TanComments);
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private void DoOpenTanFolder(object obj)
        {

            try
            {
                if (TanVM != null && !String.IsNullOrEmpty(TanVM.DocumentPath))
                    System.Diagnostics.Process.Start(Directory.GetParent(Path.Combine(C.SHAREDPATH, TanVM.DocumentPath)).ToString());
            }
            catch (Exception ex)
            {
                Log.This(ex);
                System.Windows.MessageBox.Show("Error while opening pdf . . " + Environment.NewLine + ex.Message);
            }
        }

        public string ReviewOrQCView
        {
            get
            {
                return qcOrReviewView;
            }
            set
            {
                SetProperty(ref qcOrReviewView, value);
            }
        }

        private void DeleteParticipantFromReaction(object obj)
        {
            try
            {
                var start = DateTime.Now;
                if (obj != null)
                {
                    var participant = obj as ReactionParticipantVM;
                    if (!participant.KeyProduct)
                    {
                        DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("Are you Sure You want to Delete Selected Participant", "Reactions", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (dialogResult == DialogResult.Yes)
                        {
                            var rsn = (from r in TanVM.Rsns where r.Reaction.Id == TanVM.SelectedReaction.Id && r.IsIgnorableInDelivery && r.ReactionParticipantId != null && r.ReactionParticipantId.Contains(participant.Id) select r).FirstOrDefault();
                            if (rsn != null)
                                rsn.ReactionParticipantId.Remove(participant.Id);
                            TanVM.ReactionParticipants.Remove(participant);
                            if (participant.ChemicalType == ChemicalType.S8000 && TanVM.ReactionParticipants.Where(rp => rp.TanChemicalId == participant.TanChemicalId).Count() == 0)
                                S8000Chemicals.Remove(S8000Chemicals.Where(s => s.Id == participant.TanChemicalId).FirstOrDefault());
                            if (U.RoleId == 1)
                            {
                                TanVM.SelectedReaction.LastupdatedDate = DateTime.Now;
                                TanVM.SelectedReaction.IsCurationCompleted = false;
                            }
                            else if (U.RoleId == 2)
                            {
                                TanVM.SelectedReaction.ReviewLastupdatedDate = DateTime.Now;
                                TanVM.SelectedReaction.IsReviewCompleted = false;
                            }
                            else if (U.RoleId == 3)
                                TanVM.SelectedReaction.QCLastupdatedDate = DateTime.Now;
                            TanVM.PerformAutoSave("Participants Added");
                            Validate(true);
                            Debug.WriteLine($"Delete Participant Done in {(DateTime.Now - start).TotalSeconds} Seconds");
                        }
                    }
                    else
                    {
                        AppInfoBox.ShowInfoMessage("You can't delete the product from reaction. To delete Please delete the reaction.");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private void DoOpenTanPdf(object obj)
        {

            try
            {
                if (TanVM != null && TanVM.SelectedDocumentVM != null && !String.IsNullOrEmpty(TanVM.SelectedDocumentVM.Path))
                    System.Diagnostics.Process.Start(TanVM.SelectedDocumentVM.Path);
                else
                    AppInfoBox.ShowInfoMessage("Please Select TanDocument from Tree View");
            }
            catch (Exception ex)
            {
                Log.This(ex);
                AppErrorBox.ShowErrorMessage("Error while opening pdf . . ", ex.ToString());
            }
        }

        private void ApproveSelectedTan(object obj)
        {

            try
            {
                if (T.TanId != 0)
                {
                    DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("Are you Sure You want to Approve TAN", "Reactions", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (dialogResult == DialogResult.Yes)
                        IsQueryActive(Common.Action.APPROVE).ContinueWith(e => { });
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }
        private void RejectSelectedTan(object obj)
        {

            try
            {
                if (T.TanId != 0)
                {
                    DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("Are you Sure You want to Reject TAN", "Reactions", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (dialogResult == DialogResult.Yes)
                        IsQueryActive(Common.Action.REJECT).ContinueWith(e => { });
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }
        private void SubmitTanlist(object obj)
        {

            try
            {
                if (T.TanId != 0)
                {
                    DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("Are you Sure You want to Submit TAN", "Reactions", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (dialogResult == DialogResult.Yes)
                        IsQueryActive(Common.Action.SUBMIT).ContinueWith(e => { });
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        public async Task IsQueryActive(Common.Action action)
        {
            var result = await RestHub.IsQueryActive(T.TanId);
            if (result.UserObject != null && result.StatusMessage == "false")
            {
                if (action == Common.Action.SUBMIT)
                    BaforeTanSubmiting.Invoke(this, action);
                else if (action == Common.Action.APPROVE)
                    BaforeTanSubmiting.Invoke(this, action);
                else if (action == Common.Action.REJECT)
                    ((App.Current.MainWindow) as MainWindow).RejectTan();
            }
            else
                AppInfoBox.ShowInfoMessage("It seems you have some Query is in Active on this Tan.");
        }
        public void CloseTan(object obj)
        {
            try
            {
                CancleVisible = Visibility.Collapsed;
                IsTanLoaded = Visibility.Collapsed;
                TanVM = null;
                ValidationVM = null;
                AnalogourVisibility = Visibility.Collapsed;
                AnalogVM = null;
                previewTabIndex = 0;
                TanClosed.Invoke(this, TanVM);
                EnableLanding = Visibility.Visible;
                EnableEditor = Visibility.Collapsed;
                EnableTaskAllocation = Visibility.Collapsed;
                ValidationVM = new ValidationVM();
                S.CloseAllWindows();
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }


        private void LoadTasks(object obj)
        {
            try
            {
                if (U.RoleId != 0 && !string.IsNullOrEmpty(U.UserName))
                {

                    if (taskSheet == null)
                    {
                        taskSheet = new TaskSheet();
                        TaskSheetVM taskVM = new ViewModels.TaskSheetVM();
                        taskSheet.DataContext = taskVM;
                    }
                    (taskSheet.DataContext as TaskSheetVM).CaptureOfflineData();
                    taskSheet.ShowDialog();
                    if ((taskSheet.DataContext as TaskSheetVM).RowDoubleClicked)
                    {
                        ApproveVisible = U.CanApprove;
                        RejectVisible = U.CanReject;
                        SubmitVisible = U.CanSubmit;
                        CancleVisible = Visibility.Visible;
                        IsTanLoaded = Visibility.Visible;
                        EnableLanding = Visibility.Hidden;
                        ((App.Current.MainWindow) as MainWindow).tanId = T.TanId;
                        ((App.Current.MainWindow) as MainWindow).LoadTan();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        public ValidationVM ValidationVM { get { return validationVM; } set { SetProperty(ref validationVM, value); } }
        public Visibility IsTanLoaded { get { return isTanLoaded; } set { SetProperty(ref isTanLoaded, value); } }
        public bool ApproveVisible { get { return approveVisible; } set { SetProperty(ref approveVisible, value); } }
        public bool RejectVisible { get { return rejectVisible; } set { SetProperty(ref rejectVisible, value); } }
        public bool SubmitVisible { get { return submitVisible; } set { SetProperty(ref submitVisible, value); } }
        public Visibility CancleVisible { get { return cancleTan; } set { SetProperty(ref cancleTan, value); } }
        public Visibility IsYieldVisible { get { return isYieldVisible; } set { SetProperty(ref isYieldVisible, value); } }
        public Visibility NumsSplitterVisibility { get { return numsSplitterVisibility; } set { SetProperty(ref numsSplitterVisibility, value); } }
        public Visibility SettingsVisible { get { return settingsVisible; } set { SetProperty(ref settingsVisible, value); } }//AssignTaskVisble
        public Visibility ShowDeliveryTab { get { return showDeliveryTab; } set { SetProperty(ref showDeliveryTab, value); } }
        public bool AssignTaskVisble { get { return assignTaskVisble; } set { SetProperty(ref assignTaskVisble, value); } }
        public bool IsRsnWindowOpened { get { return isRsnWindowOpened; } set { SetProperty(ref isRsnWindowOpened, value); } }
        public ChemicalUsedPlacesVM ChemicalUsedPlacesVM { get { return chemicalUsedPlacesVM; } set { SetProperty(ref chemicalUsedPlacesVM, value); } }
        public String BrowserTitle
        {
            get { return browserTitle; }
            set
            {
                SetProperty(ref browserTitle, value);
                if (!String.IsNullOrEmpty(value))
                    browserTitle = WEB_SEARCH;
                else
                    browserTitle = WEB_SEARCH + " - " + value;
            }
        }
        #region Commands
        public DelegateCommand Refresh { get; private set; }
        public DelegateCommand AddReaction { get; private set; }
        public DelegateCommand GotoFirst { get; private set; }
        public DelegateCommand AddStage { get; private set; }
        public DelegateCommand DeleteReaction { get; private set; }
        public DelegateCommand DeleteStage { get; private set; }
        public DelegateCommand CopyStages { get; private set; }
        public DelegateCommand OpenTaskSheet { get; private set; }
        public DelegateCommand ApproveTan { get; private set; }
        public DelegateCommand ShowTanComments { get; private set; }
        public DelegateCommand ShowAnalogous { get; private set; }
        public DelegateCommand RejectTan { get; private set; }
        public DelegateCommand OpenTanPdf { get; private set; }
        public DelegateCommand CancleTan { get; private set; }
        public DelegateCommand GenerateXML { get; private set; }
        public DelegateCommand SubmitTan { get; private set; }
        public DelegateCommand OpenTanFolder { get; private set; }
        public DelegateCommand EditSelectedParticipant { get; private set; }
        public DelegateCommand ChooseChemical { get; private set; }
        public DelegateCommand ToggleSplitter { get; private set; }
        public DelegateCommand ShowUnUsedNums { get; private set; }
        public DelegateCommand SelectedUserManual { get; private set; }
        #endregion
        public String BrowserUrl { get { return browserUrl; } set { SetProperty(ref browserUrl, value); } }
        public string UserName { get { return userName; } set { SetProperty(ref userName, value); } }
        public string SignalRId { get { return signalRId; } set { SetProperty(ref signalRId, value); } }
        public int PreviewTabIndex
        {
            get { return previewTabIndex; }
            set
            {
                SetProperty(ref previewTabIndex, value);
                AddS8000Marking = false;
                WorkingAreaTabChanged.Invoke(this, value);
            }
        }
        public int BottomTabIndex { get { return bottomTabIndex; } set { SetProperty(ref bottomTabIndex, value); } }
        public Visibility ProgressBarVisibility { get { return progressBarVisibility; } set { SetProperty(ref progressBarVisibility, value); } }
        public Visibility AnalogourVisibility { get { return analogourVisibility; } set { SetProperty(ref analogourVisibility, value); } }
        public Visibility NotificationVisible { get { return notificationVisible; } set { SetProperty(ref notificationVisible, value); } }
        public Visibility NumSearchVisibility { get { return numSearchVisibility; } set { SetProperty(ref numSearchVisibility, value); } }
        public string ProgressText { get { return progressText; } set { SetProperty(ref progressText, value); } }
        public bool BrowserPaneActive { get { return browserPaneActive; } set { SetProperty(ref browserPaneActive, value); } }
        public bool MainWindowEnable { get { return mainWindowEnable; } set { SetProperty(ref mainWindowEnable, value); } }
        public Visibility EnableEditor
        {
            get { return enableEditor; }
            set
            {
                SetProperty(ref enableEditor, value);
                if (value == Visibility.Visible)
                {
                    EnableLanding = Visibility.Hidden;
                    EnableTaskAllocation = Visibility.Hidden;
                }
                if (value == Visibility.Hidden)
                    EnableLanding = Visibility.Visible;
            }
        }
        public Visibility EnableLanding
        {
            get { return enableLanding; }
            set { SetProperty(ref enableLanding, value); }
        }
        public Visibility EnableTaskAllocation { get { return enableTaskAllocation; } set { SetProperty(ref enableTaskAllocation, value); } }
        public ObservableCollection<CvtVM> CVTData { get { return cVTData; } set { SetProperty(ref cVTData, value); } }
        public ObservableCollection<String> FreeTextData { get { return freeTextData; } set { SetProperty(ref freeTextData, value); } }
        public ObservableCollection<TanChemicalVM> S8000Chemicals { get { return s8000Chemicals; } set { SetProperty(ref s8000Chemicals, value); } }
        public ObservableCollection<UserResportsVM> UserReports { get { return userReports; } set { SetProperty(ref userReports, value); } }
        public ObservableCollection<UserResportsVM> AllUserReports { get { return allUserReports; } set { SetProperty(ref allUserReports, value); } }
        public Batch Batch { get; set; }
        public ParticipantType ParticipantType
        {
            get { return participantType; }
            set
            {
                SetProperty(ref participantType, value);
                if (TanVM != null)
                {
                    if (TanVM.SelectedReaction != null)
                    {
                        TanVM.UpdateParticipantsView();

                        if (TanVM.SelectedReaction.SelectedStage != null && IsParticipatTypeSelected)
                            ShowLinkVisible = true;
                        else if (TanVM.SelectedReaction.SelectedStage == null && ParticipantType == ParticipantType.Product)
                            ShowLinkVisible = true;
                        else
                            ShowLinkVisible = false;
                    }
                }
                else
                    ShowLinkVisible = false;
                mainViewModelParticipantTypeChanged.Invoke(this, ParticipantType);
            }
        }

        public bool IsParticipatTypeSelected
        {
            get
            {
                return ParticipantType == ParticipantType.Product ||
                    ParticipantType == ParticipantType.Reactant ||
                    ParticipantType == ParticipantType.Solvent ||
                    ParticipantType == ParticipantType.Agent ||
                    ParticipantType == ParticipantType.Catalyst;
            }
        }
        public bool ShowLinkVisible { get { return showLinkVisible; } set { SetProperty(ref showLinkVisible, value); } }
        public TanVM TanVM { get { return tanVm; } set { SetProperty(ref tanVm, value); } }
        public ViewAnalogousVM AnalogVM { get { return analogVM; } set { SetProperty(ref analogVM, value); } }
        public String ReactionLevelTitle { get { return reactionLevelTitle; } set { SetProperty(ref reactionLevelTitle, value); } }
        public String UnUsedNumsText { get { return unUsedNumsText; } set { SetProperty(ref unUsedNumsText, value); } }
        public bool AddS8000Marking { get { return addS8000Marking; } set { SetProperty(ref addS8000Marking, value); } }
        public bool SaveEnabled { get { return saveEnabled; } set { SetProperty(ref saveEnabled, value); } }
        public bool SubmitEnabled { get { return submitEnabled; } set { SetProperty(ref submitEnabled, value); } }
        public TanChemicalVM Selected8000Chemical { get { return selected8000Chemical; } set { SetProperty(ref selected8000Chemical, value); } }


        public bool Validate(bool fromCompleteReaction = false)
        {

            try
            {
                if (ValidationVM == null)
                {
                    ValidationVM = new ValidationVM();
                }
                ValidationVM.ValidationErrors.Clear();
                ValidationVM.ValidationWarnings.Clear();
                var groupedParticipants = TanVM.ReactionParticipants.GroupBy(rp => rp.ReactionVM.Id).ToDictionary(rp => rp.Key, rp => rp.ToList());
                var groupedRsns = TanVM.Rsns.GroupBy(rp => rp.Reaction.Id).ToDictionary(rp => rp.Key, rp => rp.ToList());
                var participants = (TanVM.SelectedReaction != null && groupedParticipants.ContainsKey(TanVM.SelectedReaction.Id)) ? groupedParticipants[TanVM.SelectedReaction.Id] : new List<ReactionParticipantVM>();
                var ReactionRsns = (TanVM.SelectedReaction != null && groupedRsns.ContainsKey(TanVM.SelectedReaction.Id)) ? groupedRsns[TanVM.SelectedReaction.Id].ToCollection() : new Collection<RsnVM>();
                var ReactionRsnsWithoutStages = (TanVM.SelectedReaction != null && groupedRsns.ContainsKey(TanVM.SelectedReaction.Id)) ? groupedRsns[TanVM.SelectedReaction.Id].Where(r => r.Stage == null).ToCollection() : new Collection<RsnVM>();
                ValidationVM.ValidationErrors = (fromCompleteReaction && TanVM.SelectedReaction != null) ?
                                                new ObservableCollection<ValidationError>(Common.ReactionValidations.IsValidReaction(TanVM.SelectedReaction, participants, ReactionRsns, ReactionRsnsWithoutStages)) :
                                                new ObservableCollection<ValidationError>(Common.ReactionValidations.ValidateTan(TanVM));
                if (ValidationVM.ValidationWarnings.Count > 0)
                {
                    ValidationVM.WarningInfo = ValidationVM.ValidationWarnings.Count + " Warnings";
                    ValidationVM.WarningTabColor = StyleConstants.WarningBrush;
                    BottomTabIndex = 2;
                }
                else
                {
                    ValidationVM.WarningInfo = "No Warnings";
                    ValidationVM.WarningTabColor = StyleConstants.NormalBrush;
                    BottomTabIndex = 0;
                }
                if (ValidationVM.ValidationErrors.Count > 0)
                {
                    ValidationVM.ErrorsInfo = ValidationVM.ValidationErrors.Count + " Errors";
                    ValidationVM.ErrorTabColor = StyleConstants.ErrorBrush;
                    BottomTabIndex = 1;
                    return false;
                }
                else
                {
                    ValidationVM.ErrorsInfo = "No Errors";
                    ValidationVM.ErrorTabColor = StyleConstants.NormalBrush;
                    return true;
                }

            }
            catch (Exception ex)
            {
                Log.This(ex);
                return false;
            }

        }


        private void DoGotoReaction(object obj)
        {
            try
            {
                if (obj != null && TanVM != null && TanVM.Reactions != null && TanVM.SelectedReaction != null)
                {
                    int selectedReactionOrder = TanVM.SelectedReaction.DisplayOrder;
                    var but = (System.Windows.Controls.Button)obj;
                    if (but.Name.Equals("BtnGotoFirst"))
                        TanVM.SelectedReaction = TanVM.Reactions.OrderBy(r => r.DisplayOrder).FirstOrDefault();
                    else if (but.Name.Equals("BtnGotoPrevious"))
                    {
                        if (TanVM.Reactions.Where(r => r.DisplayOrder == selectedReactionOrder - 1).Any())
                            TanVM.SelectedReaction = TanVM.Reactions.Where(r => r.DisplayOrder == selectedReactionOrder - 1).FirstOrDefault();
                    }
                    else if (but.Name.Equals("BtnGotoNext"))
                    {
                        if (TanVM.Reactions.Where(r => r.DisplayOrder == selectedReactionOrder + 1).Any())
                            TanVM.SelectedReaction = TanVM.Reactions.Where(r => r.DisplayOrder == selectedReactionOrder + 1).FirstOrDefault();
                    }
                    else if (but.Name.Equals("BtnGotoLast"))
                        TanVM.SelectedReaction = TanVM.Reactions.OrderByDescending(r => r.DisplayOrder).FirstOrDefault();

                }

            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }
        private void AddNewReaction(object s)
        {
            try
            {

                var but = (System.Windows.Controls.Button)s;
                if (TanVM != null)
                {
                    if (TanVM.SelectedReaction != null && !Validate(true))
                        return;
                    //TanVM.EnablePreview = true;
                    var reaction = new ReactionVM
                    {
                        Id = Guid.NewGuid(),
                        TanVM = TanVM
                    };
                    if (U.RoleId == 1)
                        reaction.CuratorCreatedDate = DateTime.Now;
                    else if (U.RoleId == 2)
                        reaction.ReviewerCreatedDate = DateTime.Now;
                    if (TanVM.SelectedReaction != null)
                    {
                        var reactionList = TanVM.Reactions.ToList();
                        var index = reactionList.Count() >= 1 ? reactionList.FindIndex(x => x.Id == TanVM.SelectedReaction.Id) : 0;
                        if (but.Name == "BeforeInsert")
                            TanVM.Reactions.Insert(index, reaction);
                        else if (but.Name == "AfterInsert")
                            TanVM.Reactions.Insert(index + 1, reaction);
                    }
                    else
                        TanVM.Reactions.Add(reaction);
                    TanVM.SelectedReaction = reaction;
                    var stage = new StageVM { Id = Guid.NewGuid(), ReactionVm = reaction };
                    TanVM.SelectedReaction.Stages.Add(stage);
                    TanVM.SelectedReaction.SelectedStage = stage;
                    TanVM.PerformAutoSave("Reaction Added");
                    if (U.RoleId == 1)
                        TanVM.SelectedReaction.IsCurationCompleted = false;
                    else if (U.RoleId == 2)
                        TanVM.SelectedReaction.IsReviewCompleted = false;
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }
        private void AddNewStage(object s)
        {

            try
            {
                if (TanVM.SelectedReaction != null)
                {
                    var stage = new StageVM { Id = Guid.NewGuid(), ReactionVm = TanVM.SelectedReaction };
                    if (TanVM.SelectedReaction.SelectedStage != null)
                    {
                        var but = (System.Windows.Controls.Button)s;
                        var stagesList = TanVM.SelectedReaction.Stages.ToList();
                        var index = stagesList.Count() >= 1 ? stagesList.FindIndex(x => x.Id == TanVM.SelectedReaction.SelectedStage.Id) : 0;
                        if (but.Name == "BeforeStage")
                            TanVM.SelectedReaction.Stages.Insert(index, stage);
                        else if (but.Name == "AfterStage")
                            TanVM.SelectedReaction.Stages.Insert(index + 1, stage);
                    }
                    else
                        TanVM.SelectedReaction.Stages.Insert(TanVM.SelectedReaction.Stages.Count(), stage);
                    TanVM.SelectedReaction.SelectedStage = stage;
                    if (U.RoleId == 1)
                    {
                        TanVM.SelectedReaction.LastupdatedDate = DateTime.Now;
                        TanVM.SelectedReaction.IsCurationCompleted = false;
                    }
                    else if (U.RoleId == 2)
                    {
                        TanVM.SelectedReaction.ReviewLastupdatedDate = DateTime.Now;
                        TanVM.SelectedReaction.IsReviewCompleted = false;
                    }
                    else if (U.RoleId == 3)
                        TanVM.SelectedReaction.QCLastupdatedDate = DateTime.Now;
                    TanVM.PerformAutoSave("Stage Added");
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private void DeleteSelectedReaction(object s)
        {
            try
            {
                if (TanVM != null && TanVM.SelectedReaction != null)
                {
                    DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("Are you Sure You want to Delete Selected Reaction", "Reactions", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (dialogResult == DialogResult.Yes)
                    {
                        var reactionParticipants = TanVM.ReactionParticipants.OfReaction(TanVM.SelectedReaction.Id).ToList();
                        TanVM.EnablePreview = false;
                        if (reactionParticipants != null)
                            foreach (var participant in reactionParticipants)
                                TanVM.ReactionParticipants.Remove(participant);
                        var reactionRsn = TanVM.Rsns.OfReaction(TanVM.SelectedReaction.Id).ToList();
                        foreach (var rsn in reactionRsn)
                            TanVM.Rsns.Remove(rsn);
                        TanVM.EnablePreview = true;
                        TanVM.Reactions.Remove(TanVM.SelectedReaction);
                        TanVM.PerformAutoSave("Reaction Deleted");
                    }
                }
                else
                    AppInfoBox.ShowInfoMessage("Please select Reaction to Delete.");
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private void RemoveStage(object s)
        {

            try
            {
                if (TanVM != null && TanVM.SelectedReaction != null && TanVM.SelectedReaction.SelectedStage != null)
                {
                    DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("Are you Sure You want to Delete Selected Stage", "Reactions", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (dialogResult == DialogResult.Yes)
                    {
                        var reactionParticipants = TanVM.ReactionParticipants.OfReactionAndStage(TanVM.SelectedReaction.Id, TanVM.SelectedReaction.SelectedStage.Id).ToList();
                        TanVM.EnablePreview = false;
                        if (reactionParticipants != null)
                            foreach (var participant in reactionParticipants)
                                TanVM.ReactionParticipants.Remove(participant);
                        var reactionRsn = (from rsn in TanVM.Rsns where rsn.Reaction.Id == TanVM.SelectedReaction.Id && rsn.Stage != null && rsn.Stage.Id == TanVM.SelectedReaction.SelectedStage.Id select rsn).ToList();
                        foreach (var rsn in reactionRsn)
                            TanVM.Rsns.Remove(rsn);
                        TanVM.EnablePreview = true;
                        TanVM.SelectedReaction.Stages.Remove(TanVM.SelectedReaction.SelectedStage);
                        TanVM.PerformAutoSave("Stage Removed");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private void OpenCopystageWindow(object s)
        {

            try
            {
                if (TanVM != null && TanVM.Reactions.Count > 0 && TanVM.SelectedReaction != null)
                {
                    CopyStageVM copyVm = new CopyStageVM();
                    CopyStages copyWindow = new Client.CopyStages();
                    copyWindow.DataContext = copyVm;
                    copyWindow.ShowDialog();
                }
                else
                    AppInfoBox.ShowInfoMessage("No Reactions Found!");
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }
    }

    public class Batch
    {
        public int Id { get; set; }
        public int Name { get; set; }
        public DateTime? DateCreated { get; set; }
        public string DocumentsPath { get; set; }
        public string GifImagesPath { get; set; }
    }
}
