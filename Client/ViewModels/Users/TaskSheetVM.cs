using Client.Command;
using Client.Common;
using Client.Converters;
using Client.Logging;
using Client.ViewModel;
using Client.ViewModels.Core;
using Client.Views;
using DTO;
using Entities;
using Excelra.Utils.Library;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Client.ViewModels
{
    public class TaskSheetVM : ViewModelBase
    {
        private TaskDeatilsVM currentTask;
        private ObservableCollection<TaskDeatilsVM> tasks;
        private Visibility visibility;
        private string searchText;
        private bool buttonEnable;
        private Visibility progressBarVisibility;
        private string needsUpdate;
        private ListCollectionView userTasks;

        public string NeedsUpdate { get { return needsUpdate; } set { SetProperty(ref needsUpdate, value); } }
        public TaskDeatilsVM CurrentTask { get { return currentTask; } set { SetProperty(ref currentTask, value); } }
        public ObservableCollection<TaskDeatilsVM> Tasks { get { return tasks; } set { SetProperty(ref tasks, value); } }
        public ListCollectionView UserTasks { get { return userTasks; } set { SetProperty(ref userTasks, value); } }
        public Visibility Visibility { get { return visibility; } set { SetProperty(ref visibility, value); } }
        public Visibility ProgressBarVisibility { get { return progressBarVisibility; } set { SetProperty(ref progressBarVisibility, value); } }
        public bool ButtonEnable { get { return buttonEnable; } set { SetProperty(ref buttonEnable, value); } }
        public string SearchText { get { return searchText; } set { SetProperty(ref searchText, value); SearchFilter(); } }
        public bool RowDoubleClicked { get; set; }
        public TaskSheetVM()
        {
            RefreshClick = new Command.DelegateCommand(this.LoadTasks);
            GetTasks = new DelegateCommand(this.PullTasks);
            MouseDoubleClickCommand = new DelegateCommand(this.GetSelectedRow);
            ButtonEnable = true;
            ProgressBarVisibility = Visibility.Hidden;
            LoadTasks(U.RoleId, false).ContinueWith(r => { });
        }
        public DelegateCommand RefreshClick { get; private set; }
        public DelegateCommand MouseDoubleClickCommand { get; private set; }
        public DelegateCommand GetTasks { get; private set; }
        private void LoadTasks(object param)
        {
            try
            {
                LoadTasks(U.RoleId, false).ContinueWith(r => { });
                SearchText = string.Empty;
            }
            catch (Exception ex)
            {
                Log.This(ex);

            }
        }

        private void PullTasks(object param)
        {

            try
            {
                LoadTasks(U.RoleId, true).ContinueWith(r => { });
                SearchText = string.Empty;
            }
            catch (Exception ex)
            {
                Log.This(ex);

            }
        }
        private void GetSelectedRow(object param)
        {

            try
            {
                if (param != null)
                {
                    TaskDeatilsVM rowview = (TaskDeatilsVM)param;
                    if (rowview != null)
                    {
                        T.TanId = rowview.TanId;
                        T.TanNumber = rowview.TanName;
                        T.Curator = rowview.Analyst;
                        T.Reviewer = rowview.Reviewer;
                        T.QC = rowview.QC;
                        T.Version = rowview.Version;
                        RowDoubleClicked = true;
                        Visibility = Visibility.Hidden;
                    }
                }

            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private async Task LoadTasks(int RoleID, bool pullTasks)
        {
            try
            {
                ProgressBarVisibility = Visibility.Visible;
                ButtonEnable = false;
                ClearTaskSheet();
                if (pullTasks)
                {
                    RestStatus pullTaskResult = await RestHub.PullTask(RoleID, pullTasks);
                    PullTask pullTask = pullTaskResult.UserObject != null ? (PullTask)pullTaskResult.UserObject : null;
                    PullTaskVM pullTaskVm = new PullTaskVM();
                    if (pullTask != null)
                    {
                        pullTaskVm.TanNumber = pullTask.TanNumber;
                        if (pullTask.UserRanks != null)
                            pullTaskVm.UserRank = pullTask.UserRanks.Find(ur => ur.Key == U.UserName)?.Rank;
                        if (pullTask.TanRanks != null && pullTask.TanNumber != null)
                            pullTaskVm.AllottedTanRank = pullTask.TanRanks.Find(ur => ur.Key == pullTask.TanNumber)?.Rank;
                        List<RankVM> rankVM = new List<RankVM>();
                        int count = 1;
                        foreach (var r in pullTask.TanRanks)
                            rankVM.Add(new RankVM { DisplayOrder = count++, Key = r.Key, Rank = r.Rank, Score = r.Score.ToString() });
                        pullTaskVm.TanRanks = new ObservableCollection<RankVM>(rankVM);
                        rankVM = new List<RankVM>();
                        count = 1;
                        foreach (var r in pullTask.UserRanks)
                            rankVM.Add(new RankVM { DisplayOrder = count++, Key = r.Key, Rank = r.Rank, Score = r.Score.ToString() });
                        pullTaskVm.UserRanks = new ObservableCollection<Core.RankVM>(rankVM);
                        PullTaskDetails.ShowWindow(pullTaskVm);
                    }
                }
                RestStatus status = await RestHub.MyTans(RoleID, pullTasks);
                if (status.HttpCode == System.Net.HttpStatusCode.OK)
                {
                    List<TaskDTO> tans = status.UserObject as List<TaskDTO>;
                    if (tans != null && tans.Count > 0)
                    {
                        foreach (TaskDTO tan in tans)
                        {
                            Tasks.Add(new TaskDeatilsVM
                            {
                                TanId = tan.Id,
                                TanName = tan.TanName,
                                Status = tan.Status,
                                Analyst = tan.Analyst,
                                BatchNo = tan.BatchNo,
                                NUMsCount = tan.NUMsCount,
                                QC = tan.QC,
                                Reviewer = tan.Reviewer,
                                RXNsCount = tan.RXNsCount,
                                Version = tan.Version,
                                TanCompletionDate = tan.TanCompletionDate,
                                ProcessingNote = tan.ProcessingNote,
                                NearToTargetDate = tan.NearToTargetDate
                            });
                        }
                        Tasks.UpdateDisplayOrder();
                        UserTasks = new ListCollectionView(Tasks);
                    }
                    else
                        AppInfoBox.ShowInfoMessage("No Tasks Found. Try GetTasks After Some Time.");
                }
                else
                    AppErrorBox.ShowErrorMessage("Some error occured in Getting Tans.", status.StatusMessage);
                ButtonEnable = true;
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
            finally
            {
                ProgressBarVisibility = Visibility.Hidden;
            }
        }
        public void ClearTaskSheet()
        {
            Tasks = new ObservableCollection<TaskDeatilsVM>();
            UserTasks = new ListCollectionView(Tasks);
        }

        public void CaptureOfflineData()
        {
            NeedsUpdate = String.Empty;
            string offlineTANNumber = String.Empty;
            int offlineReactionsCount = 0;
            var mainVM = (App.Current.MainWindow as MainWindow).DataContext as MainVM;
            if (mainVM.TanVM != null)
            {
                offlineTANNumber = mainVM.TanVM.TanNumber;
                offlineReactionsCount = mainVM.TanVM.Reactions.Count;
                if (!String.IsNullOrEmpty(offlineTANNumber))
                    NeedsUpdate = $"Offline Reactions for {offlineTANNumber} : {offlineReactionsCount}";
            }
        }
        private void SearchFilter()
        {

            try
            {
                if (!string.IsNullOrEmpty(SearchText))
                {
                    if (UserTasks != null && UserTasks.Count > 0)
                    {
                        UserTasks.Filter = (n) => n != null && ((n as TaskDeatilsVM).TanId.ToString() == SearchText ||
                                                                (n as TaskDeatilsVM).TanName.SafeContainsLower(SearchText) ||
                                                                (n as TaskDeatilsVM).Shipment.SafeContainsLower(SearchText) ||
                                                                (n as TaskDeatilsVM).Analyst.SafeContainsLower(SearchText) ||
                                                                (n as TaskDeatilsVM).Reviewer.SafeContainsLower(SearchText) ||
                                                                (n as TaskDeatilsVM).QC.SafeContainsLower(SearchText));

                    }
                }
                else
                    UserTasks.Filter = null;
            }
            catch (Exception ex)
            {
                Log.This(ex);

            }
        }


    }
    public class TaskDeatilsVM : OrderableVM
    {
        private int tanId;
        private string tanName;
        private int nUMsCount;
        private int rXNsCount;
        private bool nearToTargetDate;
        private string shipment;
        private string analyst;
        private string status;
        private int batchNo;
        private string reviewer;
        private string qc;
        private int version;
        private string tanCompletionDate;
        private int displayOrder;
        private string processingNote;

        public int TanId { get { return tanId; } set { SetProperty(ref tanId, value); } }
        public string TanName { get { return tanName; } set { SetProperty(ref tanName, value); } }
        public int NUMsCount { get { return nUMsCount; } set { SetProperty(ref nUMsCount, value); } }
        public int RXNsCount { get { return rXNsCount; } set { SetProperty(ref rXNsCount, value); } }
        public bool NearToTargetDate { get { return nearToTargetDate; } set { SetProperty(ref nearToTargetDate, value); } }
        public string Shipment { get { return shipment; } set { SetProperty(ref shipment, value); } }
        public string Analyst { get { return analyst; } set { SetProperty(ref analyst, value); } }
        public string Status { get { return status; } set { SetProperty(ref status, value); } }
        public int BatchNo { get { return batchNo; } set { SetProperty(ref batchNo, value); } }
        public string Reviewer { get { return reviewer; } set { SetProperty(ref reviewer, value); } }
        public string QC { get { return qc; } set { SetProperty(ref qc, value); } }
        public int Version { get { return version; } set { SetProperty(ref version, value); } }
        public string TanCompletionDate { get { return tanCompletionDate; } set { SetProperty(ref tanCompletionDate, value); } }
        public string ProcessingNote { get { return processingNote; } set { SetProperty(ref processingNote, value); } }
        public override int DisplayOrder
        {
            get { return displayOrder; }
            set
            {
                SetProperty(ref displayOrder, value);
            }
        }
    }
}
