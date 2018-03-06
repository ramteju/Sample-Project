using Client.Command;
using Client.Common;
using Client.ViewModels.Delivery;
using Entities;
using Entities.DTO;
using Excelra.Utils.Library;
using ProductTracking.Models.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Forms;
using Client.Logging;
using Client.Views;
using Telerik.Windows.Data;
using Client.ViewModel;
using System.Windows.Input;

namespace Client.ViewModels.Tasks
{
    public class TaskAllocationVM : ViewModelBase
    {
        private ObservableCollection<BatchTanVM> batchTans;
        private ObservableCollection<object> selectedTans;
        private ObservableCollection<BatchVM> batches;
        private ObservableCollection<BatchVM> selectedBatches;
        private ObservableCollection<string> roles;
        private BatchVM selectedFromBatch;
        private BatchVM selectedToBatch;
        private ListCollectionView batchTansView;
        private ObservableCollection<UsersVM> curators;
        private ListCollectionView roleWiseUsers;
        private UsersVM selectedCurator;
        private bool workInProgress;
        private string selectedRole, commentText;
        private BatchTanVM selectedTan;

        public ObservableCollection<BatchTanVM> BatchTans { get { return batchTans; } set { SetProperty(ref batchTans, value); } }
        public ObservableCollection<BatchVM> SelectedBatches { get { return selectedBatches; } set { SetProperty(ref selectedBatches, value); } }
        public ObservableCollection<object> SelectedTans { get { return selectedTans; } set { SetProperty(ref selectedTans, value); } }
        public ObservableCollection<BatchVM> Batches { get { return batches; } set { SetProperty(ref batches, value); } }
        public ObservableCollection<string> Roles { get { return roles; } set { SetProperty(ref roles, value); } }
        public BatchVM SelectedFromBatch { get { return selectedFromBatch; } set { SetProperty(ref selectedFromBatch, value); } }
        public BatchVM SelectedToBatch { get { return selectedToBatch; } set { SetProperty(ref selectedToBatch, value); } }
        public ObservableCollection<UsersVM> Curators { get { return curators; } set { SetProperty(ref curators, value); } }
        public ListCollectionView RoleWiseUsers { get { return roleWiseUsers; } set { SetProperty(ref roleWiseUsers, value); } }
        public UsersVM SelectedCurator { get { return selectedCurator; } set { SetProperty(ref selectedCurator, value); } }
        public bool WorkInProgress { get { return workInProgress; } set { SetProperty(ref workInProgress, value); } }
        public string SelectedRole
        {
            get { return selectedRole; }
            set
            {
                SetProperty(ref selectedRole, value);
                RoleWiseUsers = new ListCollectionView(Curators);
                SelectedCurator = null;
                App.Current.Dispatcher.Invoke(() =>
                {
                    RoleWiseUsers.Filter = (c) =>
                    {
                        if (c != null && !String.IsNullOrEmpty(value))
                        {
                            var user = c as UsersVM;
                            return (user.Role.ToString().Equals(value));
                        }
                        return true;
                    };
                });

            }
        }
        public string CommentText { get { return commentText; } set { SetProperty(ref commentText, value); } }
        public BatchTanVM SelectedTan { get { return selectedTan; } set { SetProperty(ref selectedTan, value); } }
        public ListCollectionView BatchTansView
        {
            get { return batchTansView; }
            set
            {
                SetProperty(ref batchTansView, value);
            }
        }

        public void RefreshVM()
        {
            BatchTans = new ObservableCollection<Delivery.BatchTanVM>();
            BatchTansView = null;
            Batches = new ObservableCollection<Delivery.BatchVM>();
            SelectedFromBatch = null;
            SelectedBatches = new ObservableCollection<BatchVM>();
            SelectedRole = null;
        }


        #region Report

        private int totalTans, extraStages, zeroRXNs;
        private int curationAssigned, curationProgress, reviewAssigned, reviewProgress, qcAssigned, qcProgress, qcCompleted,
            rxnCurationAssigned, rxnCurationProgress, rxnReviewAssigned, rxnReviewProgress, rxnQcAssigned, rxnQcProgress, rxnQcCompleted;
        private int notAssigned;

        public int TotalTans { get { return totalTans; } set { SetProperty(ref totalTans, value); } }
        public int ExtraStages { get { return extraStages; } set { SetProperty(ref extraStages, value); } }
        public int ZeroRXNs { get { return zeroRXNs; } set { SetProperty(ref zeroRXNs, value); } }
        public int CurationAssigned { get { return curationAssigned; } set { SetProperty(ref curationAssigned, value); } }
        public int CurationProgress { get { return curationProgress; } set { SetProperty(ref curationProgress, value); } }
        public int ReviewAssigned { get { return reviewAssigned; } set { SetProperty(ref reviewAssigned, value); } }
        public int ReviewProgress { get { return reviewProgress; } set { SetProperty(ref reviewProgress, value); } }
        public int QCAssigned { get { return qcAssigned; } set { SetProperty(ref qcAssigned, value); } }
        public int QCProgress { get { return qcProgress; } set { SetProperty(ref qcProgress, value); } }
        public int QCCompleted { get { return qcCompleted; } set { SetProperty(ref qcCompleted, value); } }
        public int RXNCurationAssigned { get { return rxnCurationAssigned; } set { SetProperty(ref rxnCurationAssigned, value); } }
        public int RXNCurationProgress { get { return rxnCurationProgress; } set { SetProperty(ref rxnCurationProgress, value); } }
        public int RXNReviewAssigned { get { return rxnReviewAssigned; } set { SetProperty(ref rxnReviewAssigned, value); } }
        public int RXNReviewProgress { get { return rxnReviewProgress; } set { SetProperty(ref rxnReviewProgress, value); } }
        public int RXNQCAssigned { get { return rxnQcAssigned; } set { SetProperty(ref rxnQcAssigned, value); } }
        public int RXNQCProgress { get { return rxnQcProgress; } set { SetProperty(ref rxnQcProgress, value); } }
        public int RXNQCCompleted { get { return rxnQcCompleted; } set { SetProperty(ref rxnQcCompleted, value); } }
        public int NotAssigned { get { return notAssigned; } set { SetProperty(ref notAssigned, value); } }
        #endregion

        public DelegateCommand SearchTans { get; set; }
        public DelegateCommand AssignTans { get; set; }
        public DelegateCommand MouseDoubleClickCommand { get; set; }
        public DelegateCommand AllowReview { get; private set; }
        public DelegateCommand ShowCuratorVersion { get; private set; }
        public DelegateCommand ShowReviewerVersion { get; private set; }
        public DelegateCommand ShowQcVersion { get; private set; }

        public TaskAllocationVM()
        {
            BatchTans = new ObservableCollection<Delivery.BatchTanVM>();
            SelectedTans = new ObservableCollection<object>();
            Batches = new ObservableCollection<Delivery.BatchVM>();
            Curators = new ObservableCollection<Tasks.UsersVM>();
            SelectedBatches = new ObservableCollection<BatchVM>();
            Roles = new ObservableCollection<string>(LoadRoles());
            SearchTans = new Command.DelegateCommand(DoSearchTans);
            AssignTans = new Command.DelegateCommand(DoAssignTans);
            MouseDoubleClickCommand = new Command.DelegateCommand(DoOpenTan);
            AllowReview = new DelegateCommand(DoAllowReview);
            ShowCuratorVersion = new DelegateCommand(DoShowCuratorVersion);
            ShowReviewerVersion = new DelegateCommand(DoShowReviewerVersion);
            ShowQcVersion = new DelegateCommand(DoShowQcVersion);
        }

        private void DoShowCuratorVersion(object obj)
        {
            U.TargetRole = 1;
            DoOpenTan(null);
        }
        private void DoShowReviewerVersion(object obj)
        {
            U.TargetRole = 2;
            DoOpenTan(null);
        }
        private void DoShowQcVersion(object obj)
        {
            U.TargetRole = 3;
            DoOpenTan(null);
        }

        private async void DoAllowReview(object obj)
        {
            if (SelectedBatches.Any())
            {
                var result = await RestHub.AllowForReview(SelectedBatches.Select(b => b.Name).ToList());
                if (result.UserObject != null)
                {
                    AppInfoBox.ShowInfoMessage("Status updated Succeesfully");
                }
            }
        }

        private void DoOpenTan(object obj)
        {
            if (SelectedTan != null)
            {
                DialogResult dialogResult = System.Windows.Forms.MessageBox.Show($"Are you Sure You want to open the Tan {SelectedTan.TanNumber} in curation Window?", "Reactions", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dialogResult == DialogResult.Yes)
                {
                    var mainVM = (App.Current.MainWindow as MainWindow).DataContext as MainVM;
                    T.TanId = SelectedTan.Id;
                    T.TanNumber = SelectedTan.TanNumber;
                    T.Version = SelectedTan.Version;
                    T.BatchNumber = SelectedTan.BatchNum.ToString();
                    (App.Current.MainWindow as MainWindow).tanId = SelectedTan.Id;
                    (App.Current.MainWindow as MainWindow).LoadTan(null,false,true);
                    mainVM.EnableEditor = System.Windows.Visibility.Visible;
                }
                U.TargetRole = 0;
            }
        }

        public List<string> LoadRoles()
        {
            var list = Enum.GetNames(typeof(Role)).ToList();
            return list;
        }

        private async void DoAssignTans(object obj)
        {

            try
            {
                WorkInProgress = true;
                var list = BatchTansView;
                var filteredItems = BatchTansView.Cast<BatchTanVM>();
                BatchTans = new ObservableCollection<BatchTanVM>();
                if (SelectedCurator != null)
                {
                    if (!string.IsNullOrEmpty(CommentText))
                    {
                        if (SelectedTans != null && SelectedTans.Count > 0)
                        {
                            var invalidTans = new List<object>();
                            TanState? tanstateToCheck = null;
                            if (SelectedCurator.Role == Role.Curator)
                                tanstateToCheck = TanState.Not_Assigned;
                            else if (SelectedCurator.Role == Role.Reviewer)
                                tanstateToCheck = TanState.Curation_Submitted;
                            else if (SelectedCurator.Role == Role.QC)
                                tanstateToCheck = TanState.Review_Accepted;
                            if (tanstateToCheck != null)
                            {
                                invalidTans = SelectedTans.Where(st => ((st as BatchTanVM).CurrentRole != 0 && (st as BatchTanVM).CurrentRole != SelectedCurator.Role)
                                                      || ((st as BatchTanVM).CurrentRole == 0 && (st as BatchTanVM).TanState != tanstateToCheck)).ToList();
                                if (!invalidTans.Any())
                                {
                                    var result = await RestHub.AssignTans(SelectedTans.Select(tan => (tan as BatchTanVM).Id).ToList(), SelectedCurator.UserId, U.RoleId, CommentText, SelectedRole.Equals("Curator") ? Role.Curator : SelectedRole.Equals("Reviewer") ? Role.Reviewer : Role.QC);
                                    if (result.UserObject != null)
                                    {
                                        AppInfoBox.ShowInfoMessage($"Tans Assigned to {SelectedRole} Successfully");
                                        SearchTans.Execute(null);
                                    }
                                    else
                                        AppErrorBox.ShowErrorMessage("Can't Assign TANs . .", result.StatusMessage);
                                }
                                else
                                    AppInfoBox.ShowInfoMessage($"In selected tans {string.Join(",", invalidTans.Select(tan => (tan as BatchTanVM).TanNumber + " - " + (tan as BatchTanVM).CurrentRole).ToList())} are allocated to other roles");
                            }
                            else
                                AppInfoBox.ShowInfoMessage("Please select Role");
                        }
                        else
                            AppInfoBox.ShowInfoMessage("Please select atleast One TAN From the below list to Assign TANs");
                    }
                    else
                        AppInfoBox.ShowInfoMessage("Please enter comment to Assign Selected TANs");
                }
                else
                    AppInfoBox.ShowInfoMessage("Please Select User to assign TANs");
            }
            catch (Exception ex)
            {
                Log.This(ex);

            }
            WorkInProgress = false;
        }

        private async void DoSearchTans(object obj)
        {

            try
            {
                if (obj != null)
                {
                    var conrol = (Telerik.Windows.Controls.RadGridView)obj;
                    foreach (Telerik.Windows.Controls.GridViewColumn column in conrol.Columns)
                        column.ClearFilters();
                }
                WorkInProgress = true;
                BatchTans = new ObservableCollection<BatchTanVM>();
                if (SelectedBatches.Any())
                {
                    var result = await RestHub.TansFromBatches(SelectedBatches.Select(b => b.Name).ToList(), 100);
                    if (result.UserObject != null)
                    {
                        var tans = (List<BatchTanDto>)result.UserObject;
                        foreach (var tan in tans)
                        {
                            BatchTans.Add(new BatchTanVM
                            {
                                Id = tan.Id,
                                BatchNum = tan.BatchNumber,
                                TanNumber = tan.TanNumber,
                                TanCategory = new TanCategoryVM { Value = (int)tan.TanCategory, Description = tan.TanCategory.DescriptionAttribute() },
                                TanType = tan.TanType,
                                Nums = tan.Nums,
                                Rxns = tan.Rxns,
                                Curator = tan.Curator,
                                Reviewer = tan.Reviewer,
                                QC = tan.QC,
                                TanState = tan.TanState,
                                CurrentRole = tan.CurrentRole,
                                Version = tan.Version,
                                Stages = tan.Stages,
                                NearToTargetDate = tan.NearToTargetDate,
                                IsDoubtRaised = tan.IsDoubtRaised.ToString(),
                                TargetedDate = tan.TargetDate,
                                ProcessingNote = tan.ProcessingNote
                            });
                        }
                        BatchTans.UpdateDisplayOrder();
                        BatchTansView = new ListCollectionView(BatchTans);
                    }
                    else
                        AppInfoBox.ShowInfoMessage("Can't Load TANs . .");
                }
                else
                    AppInfoBox.ShowInfoMessage("From Batch, To Batch, Category Are Required . .");
                UpdateSummary(BatchTans);
            }
            catch (Exception ex)
            {
                Log.This(ex);

            }
            WorkInProgress = false;
        }
        public void UpdateSummary(ObservableCollection<BatchTanVM> filteredTans)
        {

            try
            {
                if (filteredTans != null)
                {
                    TotalTans = filteredTans.Count;
                    NotAssigned = filteredTans.Count(t => t.TanState == null || t.TanState == TanState.Not_Assigned);

                    CurationAssigned = filteredTans.Count(t => t.TanState != null && t.TanState == TanState.Curation_Assigned);
                    RXNCurationAssigned = filteredTans.Where(t => t.TanState != null && t.TanState == TanState.Curation_Assigned).Sum(t => t.Rxns);
                    CurationProgress = filteredTans.Count(t => t.TanState != null && t.TanState == TanState.Curation_InProgress);
                    RXNCurationProgress = filteredTans.Where(t => t.TanState != null && t.TanState == TanState.Curation_InProgress).Sum(t => t.Rxns);

                    ReviewAssigned = filteredTans.Count(t => t.TanState != null && t.TanState == TanState.Review_Assigned);
                    RXNReviewAssigned = filteredTans.Where(t => t.TanState != null && t.TanState == TanState.Review_Assigned).Sum(t => t.Rxns);
                    ReviewProgress = filteredTans.Count(t => t.TanState != null && t.TanState == TanState.Review_InProgress);
                    RXNReviewProgress = filteredTans.Where(t => t.TanState != null && t.TanState == TanState.Review_InProgress).Sum(t => t.Rxns);

                    QCAssigned = filteredTans.Count(t => t.TanState != null && t.TanState == TanState.QC_Assigned);
                    RXNQCAssigned = filteredTans.Where(t => t.TanState != null && t.TanState == TanState.QC_Assigned).Sum(t => t.Rxns);
                    QCProgress = filteredTans.Count(t => t.TanState != null && t.TanState == TanState.QC_InProgress);
                    RXNQCProgress = filteredTans.Where(t => t.TanState != null && t.TanState == TanState.QC_InProgress).Sum(t => t.Rxns);
                    QCCompleted = filteredTans.Count(t => t.TanState != null && t.TanState == TanState.QC_Accepted);
                    RXNQCCompleted = filteredTans.Where(t => t.TanState != null && t.TanState == TanState.QC_Accepted).Sum(t => t.Rxns);
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        public async Task LoadData()
        {

            try
            {
                RestStatus status = await RestHub.Shipments();
                if (status.UserObject != null)
                {
                    List<BatchDTO> batches = (List<BatchDTO>)status.UserObject;
                    Batches = new ObservableCollection<Delivery.BatchVM>();
                    foreach (var batch in batches)
                    {
                        Batches.Add(new BatchVM
                        {
                            DateCreated = batch.DateCreated,
                            DocumentsPath = batch.DocumentsPath,
                            Id = batch.Id,
                            Name = batch.Name
                        });
                    }
                }
                status = await RestHub.GetAllCurators();
                if (status.UserObject != null)
                {
                    List<UserDTO> curators = (List<UserDTO>)status.UserObject;
                    Curators = new ObservableCollection<Tasks.UsersVM>();
                    foreach (var user in curators)
                    {
                        Curators.Add(new UsersVM
                        {
                            UserId = user.UserID,
                            Role = user.Role,
                            Name = user.Name
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }
    }
}
