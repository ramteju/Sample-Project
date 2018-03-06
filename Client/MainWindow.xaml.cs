using Client.Common;
using Client.Models;
using Client.ViewModels;
using Client.Views;
using DTO;
using ProductTracking.Models.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telerik.Windows.Controls;
using System.Windows;
using Entities;
using Newtonsoft.Json;
using Client.ViewModel;
using System.Windows.Data;
using System.Windows.Input;
using Client.ViewModels.Core;
using Entities.DTO;
using System.IO;
using Client.Logging;
using Client.ViewModels.Extended;
using Entities.DTO.Core;
using AxFoxitPDFSDKProLib;
using System.Windows.Media;
using System.Diagnostics;
using Client.Views.Extended;
using Client.Views.Report;
using Client.ViewModels.Reports;
using Client.RestConnection;

namespace Client
{
    public partial class MainWindow : RadRibbonWindow
    {
        #region For Communication between views
        public event EventHandler<TanChemicalVM> PublishChemicalName = delegate { };
        public event EventHandler<TanChemicalVM> PublishNUMAssign = delegate { };
        public event EventHandler<System.Windows.Input.KeyEventArgs> PreviewKyDown = delegate { };
        public event EventHandler<Tan> MasterTanLoaded = delegate { };
        public event EventHandler<Tan> SerializedTanLoaded = delegate { };
        public event EventHandler TANSaved = delegate { };
        public event EventHandler<ReactionVM> MasterReactionChangedInAnalogousTab = delegate { };
        public event EventHandler<TanVM> TanClosed = delegate { };
        public int tanId = 1;
        public TanData TanData { get; set; }

        private Tan MasterTan;
        private TanInfoDTO TanInfoDto;
        private ReactionVM SelectedReaction;
        public bool LoadServerVersion = true;
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            S.LoadData();

            Color backgroundColour = Colors.WhiteSmoke;// Color.FromRgb(223, 245, 239);
            Office2013Palette.Palette.LowLightColor = backgroundColour;
            Office2013Palette.Palette.LowDarkColor = backgroundColour;
            Office2013Palette.Palette.MainColor = backgroundColour;
            this.Closing += MainWindow_Closing;
            this.Closed += MainWindow_Closed;
            this.PreviewKeyDown += MainWindow_PreviewKeyDown;

            U.UserName = String.Empty;
            U.RoleId = 0;
            LoginVM loginVM = new LoginVM();
            LoginWindow.OpenLoginForm(loginVM);
            if (String.IsNullOrEmpty(U.UserName))
            {
                LoginWindow.HideLoginForm();
            }
            var mainViewModel = ThisWindow.DataContext as MainVM;
            if (mainViewModel != null)
            {
                var check = U.RoleId == (int)Role.ProjectManger || U.RoleId == (int)Role.ToolManager;
                mainViewModel.ShowDeliveryTab = mainViewModel.SettingsVisible = check ? Visibility.Visible : Visibility.Hidden;
                mainViewModel.AssignTaskVisble = check;
                mainViewModel.SaveEnabled = U.RoleId == (int)Role.ProjectManger || U.RoleId == (int)Role.ToolManager ? false : true;
            }
        }

        public void UpdateMarkupDocument(string KeyPath, string TotalPaths)
        {
            if (File.Exists(T.MasterTanDataFilePath))
            {
                var TanInfoDto = JsonConvert.DeserializeObject<TanInfoDTO>(File.ReadAllText(T.MasterTanDataFilePath));
                TanInfoDto.Tan.DocumentPath = KeyPath;
                TanInfoDto.Tan.TotalDocumentsPath = TotalPaths;
                string masterTanJson = JsonConvert.SerializeObject(TanInfoDto);
                System.IO.File.WriteAllText(T.MasterTanDataFilePath, masterTanJson);
            }
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            S.CloseAllWindows();
        }

        public void PublishMasterReactionChangeInAnalogousTab(ReactionVM masterReaction)
        {
            MasterReactionChangedInAnalogousTab.Invoke(this, masterReaction);
        }
        private void MainWindow_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.W && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                    (DataContext as MainVM).CancleTan.Execute(this);
                else if (e.Key == Key.E && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                    (DataContext as MainVM).NumsExportToPdf.Execute(this);
                else if (e.Key == Key.R && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                    (DataContext as MainVM).Refresh.Execute(this);
                else if (e.Key == Key.H && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                    RSNReplaceWindow.ShowFreetexts();
                else if (e.Key == Key.O && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                    (DataContext as MainVM).OpenTaskSheet.Execute(this);
                else if (e.Key == Key.S && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && U.RoleId != 4 && U.RoleId != 5)
                {
                    if (MainVM.PreviewTabIndex == 4)
                        U.LastSelectedTab = 4;
                    else
                        U.LastSelectedTab = MainVM.PreviewTabIndex;
                    SaveTan().ContinueWith(t => { });
                }
                else if (e.Key == Key.F3)
                {
                    if (MainVM != null && MainVM.TanVM != null && MainVM.TanVM.SelectedReaction != null && MainVM.TanVM.ReactionParticipants != null)
                    {
                        var reactionPreview = ReactionViewBuilder.GetReactionView(MainVM.TanVM.SelectedReaction, MainVM.TanVM.ReactionParticipants.ToList());
                        ReactionPreviewLargeView.ShowWindow(reactionPreview);
                    }
                }
                else if (e.Key == Key.F5)
                    MainVM.ShowAnalogous.Execute(this);
                else if (((e.Key >= Key.D0 && e.Key <= Key.D6) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad6)) && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                {
                    int key = Convert.ToInt32(e.Key.ToString().Substring(e.Key.ToString().Length - 1, 1));
                    if (key == 6 && MainVM.AnalogourVisibility == Visibility.Hidden)
                        return;
                    else
                        MainVM.PreviewTabIndex = key;
                }
                else if (e.Key == Key.OemTilde)
                {
                    if (MainVM.PreviewTabIndex == 0)
                        MainVM.PreviewTabIndex = 4;
                    else
                        MainVM.PreviewTabIndex = 0;
                }
                else
                    PreviewKyDown.Invoke(this, e);
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            App.Current.Shutdown(0);
        }

        public void ChemicalNameAdded(TanChemicalVM chemicalName)
        {
            try
            {
                PublishChemicalName.Invoke(this, chemicalName);
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }
        public void NUMAssigned(TanChemicalVM chemicalName)
        {
            try
            {
                PublishNUMAssign.Invoke(this, chemicalName);
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }
        public void TanSavedEvent()
        {
            try
            {
                TANSaved.Invoke(this, null);
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }
        public void BrowserUrlChange(string text, string url)
        {
            try
            {
                var mainViewModel = ThisWindow.DataContext as MainVM;
                mainViewModel.BrowserUrl = url;
                mainViewModel.BrowserTitle = text;
                mainViewModel.PreviewTabIndex = 5;
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }
        public void SelectParticipant(ReactionParticipantVM participant)
        {
            try
            {
                var mainViewModel = DataContext as MainVM;
                if (mainViewModel != null && mainViewModel.TanVM != null && mainViewModel.TanVM.Reactions != null && participant != null)
                {
                    mainViewModel.TanVM.SelectedReaction = mainViewModel.TanVM.Reactions.Where(r => r.Id == participant.ReactionVM.Id).FirstOrDefault();
                    if (mainViewModel.TanVM.SelectedReaction != null && participant.StageVM != null && mainViewModel.TanVM.SelectedReaction.Stages != null)
                        mainViewModel.TanVM.SelectedReaction.SelectedStage = mainViewModel.TanVM.SelectedReaction.Stages.Where(s => s.Id == participant.StageVM.Id).FirstOrDefault();
                    mainViewModel.ParticipantType = ParticipantType.All;
                    mainViewModel.PreviewTabIndex = 0;
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        public async Task ShowTan(int tanId, int currentRoleId, [Optional]bool CallingFromSaveTan, [Optional] ReactionVM selectedReaction, [Optional] bool fromToolManager)
        {
            var start = DateTime.Now;
            var mainViewModel = ThisWindow.DataContext as MainVM;
            if (mainViewModel == null)
                return;
            this.SelectedReaction = selectedReaction;
            try
            {
                var startTime = DateTime.Now;
                mainViewModel.ProgressBarVisibility = Visibility.Visible;
                mainViewModel.ProgressText = "Getting TAN Details . .";
                mainViewModel.EnableTaskAllocation = Visibility.Hidden;
                LoadServerVersion = true;
                #region Local Version
                if (File.Exists(T.TanDataFilePath) && File.Exists(T.MasterTanDataFilePath) && !fromToolManager)
                {
                    try
                    {
                        Stopwatch stopWatch = new Stopwatch();
                        stopWatch.Start();
                        TanInfoDto = JsonConvert.DeserializeObject<TanInfoDTO>(File.ReadAllText(T.MasterTanDataFilePath));
                        stopWatch.Stop();
                        TimeSpan ts = stopWatch.Elapsed;
                        Debug.WriteLine($"Serialised TanInfoDTO in {ts.TotalSeconds} seconds");
                        stopWatch.Start();
                        MasterTan = TanInfoDto.Tan;
                        TanData = JsonConvert.DeserializeObject<TanData>(File.ReadAllText(T.TanDataFilePath));
                        stopWatch.Stop();
                        ts = stopWatch.Elapsed;
                        Debug.WriteLine($"Serialised TanData in {ts.TotalSeconds} seconds");
                        if (MasterTan.Version == null || T.Version <= MasterTan.Version)
                            LoadServerVersion = false;
                    }
                    catch (Exception ex)
                    {
                        Log.This(ex);
                    }
                }
                #endregion



                #region Server Version
                if (LoadServerVersion)
                {
                    RestStatus status = null;
                    if (fromToolManager && U.TargetRole != 0)
                        status = await RestHub.GetRoleBasedTan(tanId, U.TargetRole);
                    else
                        status = await RestHub.GetTan(tanId);
                    if (status.UserObject != null)
                    {
                        TanInfoDto = (TanInfoDTO)status.UserObject;
                        MasterTan = TanInfoDto.Tan;
                        TanData = TanInfoDto.TanData;
                        if (!fromToolManager)
                        {
                            System.IO.File.WriteAllText(T.MasterTanDataFilePath, JsonConvert.SerializeObject(TanInfoDto));
                            if (File.Exists(T.TanDataFilePath))
                                System.IO.File.WriteAllText(T.TanDataFilePathBackUp, JsonConvert.SerializeObject(TanData));
                            System.IO.File.WriteAllText(T.TanDataFilePath, JsonConvert.SerializeObject(TanData));
                        }
                        string DocumentPath = Path.Combine(T.TanFolderpath, Path.GetFileName(MasterTan.DocumentPath));
                        if (File.Exists(Path.Combine(C.SHAREDPATH, MasterTan.DocumentPath)))
                            File.Copy(Path.Combine(C.SHAREDPATH, MasterTan.DocumentPath), DocumentPath, true);
                        if (!string.IsNullOrEmpty(MasterTan.TotalDocumentsPath))
                        {
                            var list = MasterTan.TotalDocumentsPath.Split(',');
                            foreach (var path in list)
                            {
                                string DocumentLocalPath = Path.Combine(T.TanFolderpath, Path.GetFileName(path));
                                File.Copy(Path.Combine(C.SHAREDPATH, path), DocumentLocalPath, true);
                            }
                        }
                    }
                    else
                        AppInfoBox.ShowInfoMessage(status.StatusMessage);
                }
                #endregion

                if (TanData != null)
                {
                    LoadTanToTool(CallingFromSaveTan);
                }

                if (mainViewModel.TanVM != null)
                    mainViewModel.TanVM.NumSearch = string.Empty;
            }
            catch (Exception ex)
            {
                Log.This(ex);
                AppErrorBox.ShowErrorMessage("Error While Showing TAN ", ex.ToString());
            }
            var end = DateTime.Now;
            Debug.WriteLine($"Tan Loaded at {end} in {(end - start).TotalSeconds} seconds");
        }


        public void LoadTanToTool(bool CallingFromSaveTan)
        {
            var selectedReaction = this.SelectedReaction;
            var mainViewModel = (App.Current.MainWindow as MainWindow).DataContext as MainVM;
            if (!CallingFromSaveTan)
                mainViewModel.CloseTan(null);
            var start = DateTime.Now;
            Tan serializedTan = JsonConvert.DeserializeObject<Tan>(TanData.Data);
            Debug.WriteLine($"Serialised string to Object in {(DateTime.Now - start).TotalSeconds} Seconds");
            MasterTan.DocumentCurrentPage = serializedTan.DocumentCurrentPage;
            if (U.RoleId == (int)Role.Curator && string.IsNullOrEmpty(MasterTan.DocumentReviwedUser))
            {
                PdfReaderForm.tanId = MasterTan.Id;
                PdfReaderForm.tanNumber = MasterTan.tanNumber;
                bool? result = new PdfReaderForm().OpenDocument(Path.Combine(C.SHAREDPATH, MasterTan.DocumentPath));
                if (!PdfReaderForm.DialogStatus)
                {
                    mainViewModel.ProgressBarVisibility = Visibility.Collapsed;
                    mainViewModel.IsTanLoaded = Visibility.Collapsed;
                    mainViewModel.EnableTaskAllocation = Visibility.Collapsed;
                    mainViewModel.EnableLanding = Visibility.Visible;
                    return;
                }
            }
            foreach (var reaction in serializedTan.Reactions)
                reaction.Tan = serializedTan;
            mainViewModel.Batch = new ViewModels.Batch
            {
                Id = MasterTan.Batch.Id,
                Name = MasterTan.Batch.Name,
                DateCreated = MasterTan.Batch.DateCreated,
                DocumentsPath = MasterTan.Batch.DocumentsPath,
                GifImagesPath = MasterTan.Batch.GifImagesPath,
            };
            var tanComments = new ObservableCollection<ViewModels.Core.Comments>();
            foreach (var comment in serializedTan.TanComments)
            {
                var com = new ViewModels.Core.Comments
                {
                    Comment = comment.Comment,
                    TotalComment = comment.TotalComment,
                    CommentType = comment.CommentType,
                    Id = comment.Id,
                    Column = comment.Column,
                    Figure = comment.Figure,
                    FootNote = comment.FootNote,
                    Line = comment.Line,
                    Num = comment.Num,
                    Page = comment.Page,
                    Para = comment.Para,
                    Schemes = comment.Schemes,
                    Sheet = comment.Sheet,
                    Table = comment.Table
                };
                tanComments.Add(com);
            }

            var PdfDocumentsList = new List<TanDocumentsVM>();
            PdfDocumentsList.Add(new TanDocumentsVM
            {
                KeyPath = true,
                Path = Path.Combine(C.SHAREDPATH, MasterTan.DocumentPath),
                FileName = Path.GetFileName(Path.Combine(C.SHAREDPATH, MasterTan.DocumentPath)),
                TanId = MasterTan.Id
            });
            if (!string.IsNullOrEmpty(MasterTan.TotalDocumentsPath))
            {
                var list = MasterTan.TotalDocumentsPath.Split(',');
                foreach (var path in list)
                    PdfDocumentsList.Add(new ViewModels.Core.TanDocumentsVM
                    {
                        KeyPath = false,
                        Path = Path.Combine(C.SHAREDPATH, path),
                        FileName = Path.GetFileName(Path.Combine(C.SHAREDPATH, path)),
                        TanId = MasterTan.Id
                    });
            }
            List<TanKeyWordsVM> keyWords = new List<TanKeyWordsVM>();
            if (TanInfoDto.TanWiseKeyWords != null)
            {
                var tankeys = TanInfoDto.TanWiseKeyWords.Split(',').ToList();
                foreach (var key in tankeys)
                    keyWords.Add(new TanKeyWordsVM { KeyWord = key });
                keyWords = keyWords.AsEnumerable().OrderBy(a => a.KeyWord).ToList();
            }
            var tanVM = new TanVM
            {
                Id = MasterTan.Id,
                TanNumber = MasterTan.tanNumber,
                BatchNumber = MasterTan.Batch.Name,
                DocumentPath = MasterTan.DocumentPath,
                TanComments = new TanCommentsVM(tanComments),
                IsQCCompleted = serializedTan.IsQCCompleted,
                CanNumber = MasterTan.CAN
            };
            T.TanNumber = MasterTan.tanNumber;
            T.CanNumber = MasterTan.CAN;
            mainViewModel.TanVM = tanVM;
            mainViewModel.TanVM.PdfDocumentsList = new ObservableCollection<ViewModels.Core.TanDocumentsVM>(PdfDocumentsList);
            mainViewModel.TanVM.SelectedDocumentVM = PdfDocumentsList.Where(pdf => pdf.KeyPath).FirstOrDefault();
            mainViewModel.TanVM.SetTanKeywords(keyWords);
            serializedTan.Id = MasterTan.Id;
            var reactions = new List<ReactionVM>();
            var participants = new List<ReactionParticipantVM>();
            var RSNs = new List<RsnVM>();
            var LinqnumPreviews = new List<NUMVM>();
            var AMDNums = new List<NUMVM>();

            #region AMDs
            var AMDs = serializedTan.TanChemicals.Where(t => t.ChemicalType == ChemicalType.NUM).GroupBy(t => t.RegNumber).Where(t => t.Count() > 1);
            var AllAmds = new List<TanChemical>();
            foreach (var amd in AMDs)
                AllAmds.AddRange(amd.ToList());
            foreach (var amd in AllAmds)
            {
                AMDNums.Add(new NUMVM
                {
                    Num = amd.NUM,
                    ChemicalType = amd.ChemicalType,
                });
            }
            #endregion            
            mainViewModel.ReviewOrQCView = (U.RoleId == 3) ? "QC View" : "Review View";
            mainViewModel.AnalogVM = null;
            mainViewModel.AnalogourVisibility = Visibility.Hidden;
            mainViewModel.ProgressText = "Loading Reactions";
            #region TanChemicals
            tanVM.TanChemicals = serializedTan.TanChemicals.ToList();
            #endregion

            #region Reactions
            var tanReations = (from r in serializedTan.Reactions select r).ToList().OrderBy(d => d.DisplayOrder);
            var GroupedParticipants = serializedTan.Participants.GroupBy(p => p.ReactionId).ToDictionary(d => d.Key, d => d.ToList());
            var GroupedRsns = serializedTan.RSNs.GroupBy(p => p.ReactionId).ToDictionary(d => d.Key, d => d.ToList());
            var ReactionsAdding = DateTime.Now;
            foreach (var reaction in tanReations)
            {
                var reactionVM = new ReactionVM
                {
                    Id = reaction.Id,
                    TanVM = tanVM,
                    DisplayOrder = reaction.DisplayOrder,
                    AnalogousVMId = reaction.AnalogousFromId,
                    Yield = reaction.Yield,
                    IsCurationCompleted = reaction.IsCurationCompleted,
                    IsReviewCompleted = reaction.IsReviewCompleted,
                    CuratorCreatedDate = reaction.CuratorCreatedDate.HasValue ? reaction.CuratorCreatedDate.Value : DateTime.MinValue,
                    CuratorCompletedDate = reaction.CuratorCompletedDate.HasValue ? reaction.CuratorCompletedDate.Value : DateTime.MinValue,
                    LastupdatedDate = reaction.LastUpdatedDate,
                    ReviewerCreatedDate = reaction.ReviewerCreatedDate.HasValue ? reaction.ReviewerCreatedDate.Value : DateTime.MinValue,
                    ReviewLastupdatedDate = reaction.ReviewLastUpdatedDate,
                    ReviewerCompletedDate = reaction.ReviewerCompletedDate.HasValue ? reaction.ReviewerCompletedDate.Value : DateTime.MinValue,
                    QCLastupdatedDate = reaction.QCLastUpdatedDate,
                    QcCompletedDate = reaction.QCCompletedDate.HasValue ? reaction.QCCompletedDate.Value : DateTime.MinValue
                };
                #region ReactionParticipants
                //var products = (from p in serializedTan.Participants where p.TanId == serializedTan.Id && p.ReactionId == reaction.Id && p.StageId == null select p).ToList().OrderBy(d => d.DisplayOrder);
                var productList = GroupedParticipants.ContainsKey(reaction.Id) ? GroupedParticipants[reaction.Id].Where(rp => rp.ParticipantType == ParticipantType.Product).OrderBy(d => d.DisplayOrder).ToList() : new List<ReactionRSD>();
                foreach (var product in productList)
                {
                    var tanChemical = serializedTan.TanChemicals.Where(tc => tc.Id == product.ParticipantId).FirstOrDefault();
                    if (tanChemical != null)
                        participants.Add(new ReactionParticipantVM
                        {
                            Id = product.Id,
                            Name = product.Name,
                            Num = tanChemical.NUM,
                            Reg = tanChemical.ChemicalType == ChemicalType.S8000 ? string.Empty : tanChemical.RegNumber,
                            ParticipantType = product.ParticipantType,
                            ChemicalType = tanChemical.ChemicalType,
                            TanChemicalId = tanChemical.Id,
                            Yield = product.DisplayYield,
                            ReactionVM = reactionVM,
                            DisplayOrder = product.DisplayOrder,
                            Formula = tanChemical != null ? tanChemical.Formula : "",
                            KeyProduct = product.KeyProduct,
                            KeyProductSeqWithOutNum = product.KeyProductSeq
                        });
                }
                #endregion

                #region ReactionRSNs
                //var rsns = (from rsn in serializedTan.RSNs where rsn.TanId == serializedTan.Id && rsn.ReactionId == reaction.Id && rsn.StageId == Guid.Empty select rsn).ToList().OrderBy(d => d.DisplayOrder);
                var rsnList = (GroupedRsns.ContainsKey(reaction.Id)) ? GroupedRsns[reaction.Id].Where(r => r.StageId == Guid.Empty).ToList() : new List<ReactionRSN>();
                foreach (var rsn in rsnList)
                {
                    RSNs.Add(new RsnVM
                    {
                        CvtText = rsn.CVT,
                        FreeText = rsn.FreeText,
                        Id = rsn.Id,
                        Reaction = reactionVM,
                        Stage = null,
                        DisplayOrder = rsn.DisplayOrder
                    });
                }
                #endregion
                mainViewModel.ProgressText = "Loading Stages";
                #region Stages
                var stages = new List<StageVM>();
                var GroupedStageRsdList = GroupedParticipants.ContainsKey(reaction.Id) ? GroupedParticipants[reaction.Id].Where(r => r.StageId != null).GroupBy(t => t.StageId).ToDictionary(t => t.Key, t => t.ToList()) : new Dictionary<Guid?, List<ReactionRSD>>();
                foreach (var stage in reaction.Stages)
                {
                    var stageVM = new StageVM
                    {
                        Id = stage.Id,
                        ReactionVm = reactionVM
                    };
                    #region Conditions
                    var conditions = new List<ViewModels.StageConditionVM>();
                    foreach (var condition in stage.StageConditions.OrderBy(sc => sc.DisplayOrder))
                    {
                        conditions.Add(new ViewModels.StageConditionVM
                        {
                            Id = condition.Id,
                            DisplayOrder = condition.DisplayOrder,
                            PH = condition.PH,
                            Pressure = condition.Pressure,
                            StageId = condition.StageId,
                            Temperature = condition.Temperature,
                            Time = condition.Time,
                            TEMP_TYPE = condition.TEMP_TYPE,
                            TIME_TYPE = condition.TIME_TYPE,
                            PH_TYPE = condition.PH_TYPE,
                            PRESSURE_TYPE = condition.PRESSURE_TYPE
                        });
                    }
                    stageVM.SetConditions(conditions);
                    #endregion
                    #region StageRSN
                    //var stagersns = (from rsn in serializedTan.RSNs where rsn.TanId == serializedTan.Id && rsn.ReactionId == reaction.Id && rsn.StageId == stage.Id select rsn).ToList().OrderBy(d => d.DisplayOrder);
                    var stagersnList = (GroupedRsns.ContainsKey(reaction.Id)) ? GroupedRsns[reaction.Id].Where(r => r.StageId == stage.Id).ToList() : new List<ReactionRSN>();
                    var listRsns = new List<RsnVM>();
                    foreach (var rsn in stagersnList)
                    {
                        listRsns.Add(new RsnVM
                        {
                            Stage = stageVM,
                            Reaction = reactionVM,
                            CvtText = rsn.CVT,
                            FreeText = rsn.FreeText,
                            Id = rsn.Id,
                            IsRXN = false,
                            DisplayOrder = rsn.DisplayOrder,
                            IsIgnorableInDelivery = rsn.IsIgnorableInDelivery,
                            SelectedChemical = null,
                            ReactionParticipantId = rsn.ReactionParticipantId
                        });
                    }
                    RSNs.AddRange(listRsns);
                    #endregion
                    #region StageParticipants
                    var stageparticipants = GroupedStageRsdList.ContainsKey(stage.Id) ? GroupedStageRsdList[stage.Id].OrderBy(d => d.DisplayOrder).ToList() : new List<ReactionRSD>();
                    foreach (var participant in stageparticipants)
                    {
                        var tanChemical = serializedTan.TanChemicals.Where(tc => tc.Id == participant.ParticipantId).FirstOrDefault();
                        participants.Add(new ReactionParticipantVM
                        {
                            Id = participant.Id,
                            Name = participant.Name,
                            Num = tanChemical.NUM,
                            Reg = tanChemical.ChemicalType == ChemicalType.S8000 ? string.Empty : tanChemical.RegNumber,
                            ParticipantType = participant.ParticipantType,
                            TanChemicalId = tanChemical.Id,
                            Yield = string.Empty,
                            ReactionVM = reactionVM,
                            StageVM = stageVM,
                            ChemicalType = tanChemical.ChemicalType,
                            DisplayOrder = participant.DisplayOrder,
                            Formula = tanChemical.Formula
                        });
                    }
                    #endregion
                    stages.Add(stageVM);
                }
                reactionVM.SetStages(stages);
                #endregion
                reactions.Add(reactionVM);
            }
            Debug.WriteLine($"Reactions Adding to Local Object Done in {(DateTime.Now - ReactionsAdding).TotalSeconds} Seconds");
            #endregion
            tanVM.EnablePreview = false;
            tanVM.Curator = T.Curator?.ToUpper();
            tanVM.Reviewer = T.Reviewer?.ToUpper();
            tanVM.QC = T.QC?.ToUpper();
            var AddingToModel = DateTime.Now;
            foreach (var rxn in reactions)
                tanVM.Reactions.Add(rxn);
            Debug.WriteLine($"Reactions Adding in Foreach to Model Done in {(DateTime.Now - ReactionsAdding).TotalSeconds} Seconds");
            tanVM.ReactionsView = new ListCollectionView(tanVM.Reactions);
            Debug.WriteLine($"Reactions Adding to Model Done in {(DateTime.Now - ReactionsAdding).TotalSeconds} Seconds");
            foreach (var participant in participants)
                tanVM.ReactionParticipants.Add(participant);
            foreach (var rsn in RSNs)
                tanVM.Rsns.Add(rsn);
            tanVM.AMDNums = new ListCollectionView(AMDNums);
            tanVM.SelectedAMD = null;
            tanVM.UpdateNumsView();
            tanVM.NUMPreviewView.Filter = (r) => r != null && (r as NUMVM).ChemicalType == ChemicalType.NUM;
            tanVM.NUMPreviewViewduplicate.Filter = (r) => r != null && (r as NUMVM).ChemicalType == ChemicalType.NUM;
            tanVM.UpdateUnusedNUMs();
            mainViewModel.ProgressBarVisibility = Visibility.Hidden;
            tanVM.EnablePreview = true;
            mainViewModel.PreviewTabIndex = U.LastSelectedTab != 4 ? (U.RoleId == 2 || U.RoleId == 3) ? 1 : 0 : U.LastSelectedTab;
            mainViewModel.SubmitEnabled = U.RoleId == 1 ? true : false;
            //restore selected reaction
            mainViewModel.TanVM.SelectedReaction = selectedReaction != null ?
                (from r in mainViewModel.TanVM.Reactions where r.Id == selectedReaction.Id select r).FirstOrDefault() :
                mainViewModel.TanVM.Reactions.Count() > 0 ? mainViewModel.TanVM.Reactions[0] : null;

            if (mainViewModel.EnableEditor != Visibility.Visible)
                mainViewModel.EnableEditor = Visibility.Visible;
            //publish events
            MasterTanLoaded.Invoke(this, MasterTan);
            SerializedTanLoaded.Invoke(this, serializedTan);
            mainViewModel.IsTanLoaded = Visibility.Visible;
            if (U.RoleId == 2 || U.RoleId == 3)
                mainViewModel.DoRefresh(null);
            if (App.Current != null && App.Current.MainWindow != null)
                (App.Current.MainWindow).Focus();
        }

        public async Task Submit()
        {
            bool IsValid = true;
            List<string> IncompleteReactions = new List<string>();
            try
            {
                var mainViewModel = ThisWindow.DataContext as MainVM;
                if (U.RoleId == 1)
                {
                    if (!Common.ReactionValidations.AllreactionCurationCompleted(mainViewModel.TanVM, true, out IncompleteReactions))
                    {
                        IsValid = false;
                    }
                }
                if (IsValid)
                {
                    await SaveTan(true);
                    TanSavedEvent();
                    if ((mainViewModel.ValidationVM.ValidationErrors == null || mainViewModel.ValidationVM.ValidationErrors.Count == 0) && !mainViewModel.IsRsnWindowOpened)
                    {
                        RestStatus status = await RestHub.SubmitTan(mainViewModel.TanVM.Id, U.RoleId);
                        if (status.UserObject != null && (bool)status.UserObject == true)
                        {
                            #region Commented S3Integration Code
                            //string path = Path.Combine(TanDataFilePath, T.TanId.ToString());
                            //string copyFolderPath = $"{path}_Copy";
                            //if (!Directory.Exists(copyFolderPath))
                            //{
                            //    Directory.CreateDirectory(copyFolderPath);
                            //}
                            //foreach (string dirPath in Directory.GetDirectories(path, "*", SearchOption.AllDirectories))
                            //    Directory.CreateDirectory(dirPath.Replace(path, copyFolderPath));

                            ////Copy all the files & Replaces any files with the same name
                            //var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).Where(f => !f.ToLower().Contains(".json"));
                            //foreach (string newPath in files)
                            //    File.Copy(newPath, newPath.Replace(path, copyFolderPath), true);

                            //var zipPath = Path.Combine(path, $"{T.TanId}.zip");
                            //if (File.Exists(zipPath))
                            //    File.Delete(zipPath);
                            //ZipFile.CreateFromDirectory(copyFolderPath, zipPath);
                            //if (File.Exists(zipPath))
                            //{
                            //    FileUploadWCF fuw = new FileUploadWCF();
                            //    fuw.UploadtoServer(zipPath);
                            //} 
                            #endregion
                            mainViewModel.ProgressText = string.Empty;
                            mainViewModel.ProgressBarVisibility = Visibility.Collapsed;
                            AfterCloseActivities();
                        }
                        else
                            AppErrorBox.ShowErrorMessage("TAN Submission Failed.", status.StatusMessage);
                    }
                }
                else
                {
                    var markedReactionsToShow = new List<ListToShow>();
                    foreach (var entry in IncompleteReactions)
                        markedReactionsToShow.Add(new ListToShow { TextToShow = entry, IsValid = false });
                    PdfReactionsView.ShowWindow(markedReactionsToShow, null, "Incomplete Reactions List", "In completed Reactions List");
                }

            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }
        public async Task Approve()
        {
            bool IsValid = true;
            List<string> IncompleteReactions = new List<string>();
            try
            {
                var mainViewModel = ThisWindow.DataContext as MainVM;
                if (U.RoleId == 2)
                {
                    if (!Common.ReactionValidations.AllreactionCurationCompleted(mainViewModel.TanVM, false, out IncompleteReactions))
                    {
                        IsValid = false;
                        var markedReactionsToShow = new List<ListToShow>();
                        foreach (var entry in IncompleteReactions)
                            markedReactionsToShow.Add(new ListToShow { TextToShow = entry, IsValid = false });
                        PdfReactionsView.ShowWindow(markedReactionsToShow, null, "Review Not Completed Reactions List", "Review Not Completed Reactions List");
                    }
                }
                else if (U.RoleId == 3)
                {
                    if (!mainViewModel.TanVM.IsQCCompleted)
                    {
                        IsValid = false;
                        AppInfoBox.ShowInfoMessage("Please Check QC Complete check box to Approve the Tan");
                    }
                }
                if (IsValid)
                {
                    bool QCRequired = false;
                    if (U.RoleId == 2)
                    {
                        DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("Do you Want to Send this TAN to QC?", "Reactions", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                        if (dialogResult == System.Windows.Forms.DialogResult.Yes)
                            QCRequired = true;
                    }
                    await SaveTan(true);
                    TanSavedEvent();
                    if ((mainViewModel.ValidationVM.ValidationErrors == null || mainViewModel.ValidationVM.ValidationErrors.Count == 0) && !mainViewModel.IsRsnWindowOpened)
                    {
                        ErrorReportDto dto = new ErrorReportDto { SecondRoleId = U.RoleId, FirstRoleId = U.RoleId - 1, TanID = T.TanId };
                        RestStatus errorStatus = await ReportHub.GetErrorReportData(dto);
                        if (errorStatus.HttpCode == System.Net.HttpStatusCode.OK && errorStatus.UserObject != null)
                        {
                            dto = (ErrorReportDto)errorStatus.UserObject;
                            List<Tan> secondtan = new List<Tan>();
                            secondtan.Add(JsonConvert.DeserializeObject<Tan>(dto.SecondRoleTanData));
                            List<Tan> FirstTan = new List<Tan>();
                            FirstTan.Add(JsonConvert.DeserializeObject<Tan>(dto.FirstRoleTanData));
                            var Calculateddata = ErrorPercentageReportVM.Calculate(FirstTan, secondtan);
                            if (Calculateddata.Any())
                            {
                                var ErrorReportData = Calculateddata.First();
                                ErrorReportVM vm = ViewModelToModel.GetErrorReportVMFromModel(ErrorReportData);
                                vm.User1 = dto.FirstUserName;
                                vm.User2 = dto.SecondUserName;
                                vm.TanNumber = dto.TanNumber;
                                ErrorReportData.Role1Name = dto.FirstUserName;
                                ErrorReportData.Role2Name = dto.SecondUserName;
                                ErrorReportData.TanNumber = dto.TanNumber;
                                ErrorReportData.RecordedDate = DateTime.Now;
                                ErrorReportWindow.Result = false;
                                bool? dialogresult = ErrorReportWindow.showWindow(vm);
                                if (ErrorReportWindow.Result)
                                {
                                    RestStatus AddReportStatus = await ReportHub.AddReportData(Calculateddata.First());
                                    if (AddReportStatus.HttpCode == System.Net.HttpStatusCode.OK)
                                    {
                                        RestStatus status = await RestHub.ApproveTan(T.TanId, U.RoleId, QCRequired);
                                        if (status.UserObject != null && (bool)status.UserObject == true)
                                            AfterCloseActivities("TAN Approved Successfully");
                                    }
                                    else
                                        AppErrorBox.ShowErrorMessage("Error in saving Error Data", AddReportStatus.HttpResponse);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }
        public async Task Reject()
        {
            try
            {
                var mainViewModel = ThisWindow.DataContext as MainVM;
                await SaveTan(true);
                if ((mainViewModel.ValidationVM.ValidationErrors == null || mainViewModel.ValidationVM.ValidationErrors.Count == 0) && !mainViewModel.IsRsnWindowOpened)
                {
                    RestStatus status = await RestHub.RejectTan(T.TanId, U.RoleId);
                    if (status.UserObject != null && (bool)status.UserObject == true)
                        AfterCloseActivities("TAN Rejected Successfully");
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }
        public void AfterCloseActivities(string msg = "")
        {
            try
            {
                var mainViewModel = ThisWindow.DataContext as MainVM;
                AppInfoBox.ShowInfoMessage(!string.IsNullOrEmpty(msg) ? msg : "TAN Submitted Successfully");
                TanClosed.Invoke(this, null);
                //string TempDirectory = Path.Combine(C.LocalStoragePath, T.TanId.ToString());
                //if (Directory.Exists(TempDirectory))
                //{
                //    var files = Directory.GetFiles(TempDirectory, "*", SearchOption.AllDirectories);
                //    foreach (var file in files)
                //    {
                //        try
                //        {
                //            File.Delete(file);
                //        }
                //        catch (Exception ex)
                //        {
                //            Log.This(ex);
                //        }
                //    }
                //    try
                //    {
                //        Directory.Delete(TempDirectory, true);
                //    }
                //    catch (Exception ex)
                //    {
                //        Log.This(ex);
                //    }
                //}
                mainViewModel.CloseTan(null);
                openTaskSheet();
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }
        public void openTaskSheet()
        {
            var mainViewModel = ThisWindow.DataContext as MainVM;
            if (mainViewModel != null)
            {
                var taskSheetVm = (mainViewModel.taskSheet.DataContext as TaskSheetVM);
                if (taskSheetVm != null)
                {
                    taskSheetVm.RefreshClick.Execute(null);
                    mainViewModel.OpenTaskSheet.Execute(null);
                }
            }
        }


        public async Task SaveTan(bool OnSubmit = false)
        {
            var Start = DateTime.Now;
            var mainViewModel = ThisWindow.DataContext as MainVM;
            if (mainViewModel.IsRsnWindowOpened)
            {
                AppInfoBox.ShowInfoMessage("Please close RsnWindow to save tan");
                return;
            }
            if (mainViewModel != null && mainViewModel.TanVM != null && mainViewModel.MainWindowEnable)
            {
                try
                {
                    mainViewModel.SaveEnabled = false;
                    Debug.WriteLine($"Validation Started at {DateTime.Now}");
                    if (mainViewModel.Validate())
                    {
                        Debug.WriteLine($"Validation Done in {(DateTime.Now - Start).TotalSeconds} seconds");
                        mainViewModel.ProgressBarVisibility = Visibility.Visible;
                        Start = DateTime.Now;
                        mainViewModel.ProgressText = "Collecting the data to save  . .";
                        mainViewModel.MainWindowEnable = false;
                        var tan = ViewModelToModel.GetTanFromViewModel(mainViewModel);
                        Debug.WriteLine($"Collected the data to save  in {(DateTime.Now - Start).TotalSeconds} seconds");
                        Start = DateTime.Now;
                        tan.DocumentCurrentPage = GetCurrentPdfPage();
                        Debug.WriteLine($"Got Current PDF Page in {(DateTime.Now - Start).TotalSeconds} seconds");
                        Start = DateTime.Now;
                        foreach (var reaction in tan.Reactions)
                        {
                            if (reaction.RSD.Length > 4000)
                            {
                                AppInfoBox.ShowInfoMessage("RSD Length Exceeds 4000 in " + reaction.Name);
                                return;
                            }
                        }
                        Debug.WriteLine($"RSD Length Validation Done in {(DateTime.Now - Start).TotalSeconds} seconds");
                        Start = DateTime.Now;
                        TanData tanData = new TanData { TanId = tan.Id, Data = JsonConvert.SerializeObject(tan) };
                        Debug.WriteLine($"Performing Auto save . . ");
                        mainViewModel.ProgressText = "Saving TAN . .";
                        mainViewModel.TanVM.PerformAutoSave("Saving Tan");
                        Debug.WriteLine($"Performed auto save in {(DateTime.Now - Start).TotalSeconds} seconds");
                        Start = DateTime.Now;
                        Debug.WriteLine($"Preparing Offline data to save . .");
                        var masterTanProperties = JsonConvert.DeserializeObject<TanInfoDTO>(File.ReadAllText(T.MasterTanDataFilePath));

                        OfflineDTO offlineDTO = new OfflineDTO();
                        if (masterTanProperties != null && masterTanProperties.Tan != null && masterTanProperties.Tan.DocumentReadCompletedTime != null)
                            offlineDTO.DocumentReadEndTime = masterTanProperties.Tan.DocumentReadCompletedTime.Value;
                        if (masterTanProperties != null && masterTanProperties.Tan != null && masterTanProperties.Tan.DocumentReadStartTime != null)
                            offlineDTO.DocumentReadStartTime = masterTanProperties.Tan.DocumentReadStartTime.Value;
                        if (U.RoleId == (int)Role.Curator && !String.IsNullOrEmpty(masterTanProperties.Tan.DocumentReviwedUser))
                            offlineDTO.DocumentReviewedUserId = masterTanProperties.Tan.DocumentReviwedUser;
                        offlineDTO.TanDocumentKeyPath = masterTanProperties.Tan.DocumentPath;
                        offlineDTO.TotalPaths = masterTanProperties.Tan.TotalDocumentsPath;
                        offlineDTO.TanData = JsonConvert.SerializeObject(tanData);
                        Debug.WriteLine($"Prepared Offline data to save in {(DateTime.Now - Start).TotalSeconds} seconds");
                        Start = DateTime.Now;
                        Debug.WriteLine($"Saving tan to Server . .");
                        RestStatus status = await RestHub.SaveTan(offlineDTO, U.RoleId);
                        if (status.HttpCode == System.Net.HttpStatusCode.OK)
                        {
                            //TanSavedEvent();
                            if (masterTanProperties.Tan.Version != null)
                                masterTanProperties.Tan.Version = masterTanProperties.Tan.Version + 1;
                            else
                                masterTanProperties.Tan.Version = 1;
                            System.IO.File.WriteAllText(T.MasterTanDataFilePath, JsonConvert.SerializeObject(masterTanProperties));
                            mainViewModel.ProgressBarVisibility = Visibility.Hidden;
                            mainViewModel.ProgressText = string.Empty;
                            //if (!OnSubmit)
                            //    LoadTan(mainViewModel.TanVM.SelectedReaction,true);
                        }
                        else
                            AppErrorBox.ShowErrorMessage("Can't Save TAN", status.StatusMessage);
                        Debug.WriteLine($"Saved tan in {(DateTime.Now - Start).TotalSeconds} seconds");
                    }
                    else
                        AppInfoBox.ShowInfoMessage("Please clear the Errors in order to save the Data.");
                    mainViewModel.SaveEnabled = true;
                }
                catch (Exception ex)
                {
                    Log.This(ex);
                    AppErrorBox.ShowErrorMessage("Can't Save TAN", ex.ToString());
                    mainViewModel.SaveEnabled = true;
                }
                mainViewModel.MainWindowEnable = true;
            }
        }
        public void LoadTan([Optional] ReactionVM SelectedReaction, [Optional] bool CallingFromSaveTan, [Optional] bool fromToolManager)
        {
            try
            {
                ShowTan(tanId, U.RoleId, CallingFromSaveTan, SelectedReaction, fromToolManager).ContinueWith(t => { });
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }
        public void ApproveTan()
        {
            try
            {
                Approve().ContinueWith(t => { });
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }
        public void RejectTan()
        {
            try
            {
                Reject().ContinueWith(t => { });
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }
        public void SubmitTan()
        {
            try
            {
                Submit().ContinueWith(a => { });
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        public int GetCurrentPdfPage()
        {
            var foxit = MainEditor.PdfHost.Child as AxFoxitPDFSDK;
            return foxit == null ? 0 : foxit.CurPage;
        }
    }
}
