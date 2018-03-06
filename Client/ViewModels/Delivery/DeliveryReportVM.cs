using Client.Command;
using Client.Views;
using Entities.DTO.Delivery;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Excelra.Utils.Library;
using System.Windows;

namespace Client.ViewModels.Delivery
{
    public class DeliveryReportVM : ViewModelBase
    {
        private bool workInProgress, isZeroRxns, isQueried;
        private ObservableCollection<BatchSummaryVM> batchSummaries;
        private BatchSummaryVM batchSummary;
        private ObservableCollection<DeliveryTanVM> tans;
        private ObservableCollection<RoleVM> roles;
        private RoleVM role;
        private string deliveryMessage;
        private DeliveryTanVM tan;
        public ObservableCollection<BatchSummaryVM> BatchSummaries { get { return batchSummaries; } set { SetProperty(ref batchSummaries, value); } }
        public ObservableCollection<RoleVM> Roles { get { return roles; } set { SetProperty(ref roles, value); } }
        public RoleVM Role { get { return role; } set { SetProperty(ref role, value); } }
        public DeliveryTanVM TAN { get { return tan; } set { SetProperty(ref tan, value); } }
        public BatchSummaryVM BatchSummary { get { return batchSummary; } set { SetProperty(ref batchSummary, value); LoadTans(); } }
        public bool IsZeroRxns { get { return isZeroRxns; } set { SetProperty(ref isZeroRxns, value); LoadTans(); } }
        public bool IsQueried { get { return isQueried; } set { SetProperty(ref isQueried, value); LoadTans(); } }
        public string DeliveryMessage { get { return deliveryMessage; } set { SetProperty(ref deliveryMessage, value); } }
        public ObservableCollection<DeliveryTanVM> TANs { get { return tans; } set { SetProperty(ref tans, value); } }

        public bool WorkInProgress { get { return workInProgress; } set { SetProperty(ref workInProgress, value); } }
        public DelegateCommand Search { get; private set; }
        public DelegateCommand Revert { get; private set; }

        public DeliveryReportVM()
        {
            Search = new DelegateCommand(DoSearch);
            Revert = new DelegateCommand(DoRevert);
            Roles = new ObservableCollection<RoleVM>()
            {
                new RoleVM {Role=(int)Entities.Role.Curator,DisplayName=Entities.Role.Curator.DescriptionAttribute()},
                new RoleVM {Role=(int)Entities.Role.Reviewer,DisplayName=Entities.Role.Reviewer.DescriptionAttribute()},
                new RoleVM {Role=(int)Entities.Role.QC,DisplayName=Entities.Role.QC.DescriptionAttribute()}
            };
        }

        private async void DoRevert(object obj)
        {
            if (String.IsNullOrEmpty(DeliveryMessage))
            {
                AppInfoBox.ShowInfoMessage("Delivery message is mandatory . .");
                return;
            }
            if (System.Windows.MessageBox.Show("Confirm Revert Action . .", "Confirm", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                return;
            if (Role != null && TAN != null)
            {
                WorkInProgress = true;
                try
                {
                    var result = await RestHub.RevertDeliveryTAN(TAN.Id, Role.Role, DeliveryMessage);
                    if (result.HttpCode == System.Net.HttpStatusCode.OK)
                    {
                        AppInfoBox.ShowInfoMessage(result.StatusMessage);
                        Search.Execute(this);
                    }
                    else
                        AppErrorBox.ShowErrorMessage("Error While Reverting TAN . .", result.StatusMessage);
                }
                catch (Exception ex)
                {
                    AppErrorBox.ShowErrorMessage("Error While Reverting TAN . .", ex.ToString());
                }
                finally
                {
                    WorkInProgress = false;
                }
            }
            else
                System.Windows.MessageBox.Show("Select Role and TAN");

        }

        public void Clear()
        {
            BatchSummaries = new ObservableCollection<BatchSummaryVM>();
            TANs = new ObservableCollection<DeliveryTanVM>();
            Search.Execute(this);
        }
        private async void LoadTans()
        {
            if (BatchSummary != null)
            {
                TANs.Clear();
                WorkInProgress = true;
                try
                {
                    var result = await RestHub.TansOfDelivery(BatchSummary.Id, IsZeroRxns, IsQueried);
                    if (result.HttpCode == System.Net.HttpStatusCode.OK)
                    {
                        List<DeliveryTanDTO> dtos = (List<DeliveryTanDTO>)result.UserObject;
                        foreach (var dto in dtos)
                        {
                            TANs.Add(new DeliveryTanVM
                            {
                                Id = dto.Id,
                                TanNumber = dto.TanNumber,
                                RXNCount = dto.RXNCount,
                                IsQueried = dto.IsQueried,
                                DeliveryRevertMessage = dto.DeliveryRevertMessage
                            });
                        }
                    }
                    else
                        AppErrorBox.ShowErrorMessage("Error While Batches . .", result.StatusMessage);
                }
                catch (Exception ex)
                {
                    AppErrorBox.ShowErrorMessage("Error While Batches . .", ex.ToString());
                }
                finally
                {
                    WorkInProgress = false;
                }
            }
        }

        private async void DoSearch(object obj)
        {
            WorkInProgress = true;
            BatchSummaries.Clear();
            TANs = new ObservableCollection<DeliveryTanVM>();
            try
            {
                var result = await RestHub.DeliveryBatchSummary();
                if (result.HttpCode == System.Net.HttpStatusCode.OK)
                {
                    List<DeliveryBatchDTO> dtos = (List<DeliveryBatchDTO>)result.UserObject;
                    foreach (var dto in dtos)
                    {
                        BatchSummaries.Add(new BatchSummaryVM
                        {
                            Id = dto.Id,
                            BatchNumber = dto.BatchNumber,
                            TansCount = dto.TansCount
                        });
                    }
                }
                else
                    AppErrorBox.ShowErrorMessage("Error While Batches . .", result.StatusMessage);
            }
            catch (Exception ex)
            {
                AppErrorBox.ShowErrorMessage("Error While Batches . .", ex.ToString());
            }
            finally
            {
                WorkInProgress = false;
            }
        }
    }
}
