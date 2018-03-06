using Client.Common;
using Client.Logging;
using Client.ViewModel;
using Client.ViewModels;
using Client.Views.Delivery;
using Client.Views.Query;
using Client.Views.Report;
using Entities;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Client.Views
{
    /// <summary>
    /// Interaction logic for ToolBar.xaml
    /// </summary>
    public partial class ToolBar : UserControl
    {

        public ToolBar()
        {
            InitializeComponent();
        }

        private void RadRibbonButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((((App.Current.MainWindow) as MainWindow).DataContext as MainVM).PreviewTabIndex == 4)
                    U.LastSelectedTab = 4;
                else
                    U.LastSelectedTab = (((App.Current.MainWindow) as MainWindow).DataContext as MainVM).PreviewTabIndex;
                ((App.Current.MainWindow) as MainWindow).SaveTan().ContinueWith(t => { });
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private void CreateDeliveryTAN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                XmlVerfication.ShowWindow();
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private void PrepareDeliveryBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Shipments.ShowShipments();
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private void CompareHistory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Diff.ShowWindow();
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private void QueryBtn_Click(object sender, RoutedEventArgs e)
        {
            QueryWindow.ShowWindow();
        }

        private void rbnUserManul_Click(object sender, RoutedEventArgs e)
        {

            Manuals.ShowUserManuls();
        }
        private void LogOut_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var mainVM = (App.Current.MainWindow as MainWindow).DataContext as MainVM;
                if (mainVM != null)
                {
                    mainVM.TanVM = null;
                    mainVM.UserName = string.Empty;
                    mainVM.ValidationVM = null;
                    mainVM.EnableEditor = Visibility.Collapsed;
                    mainVM.EnableLanding = Visibility.Visible;
                    mainVM.EnableTaskAllocation = Visibility.Collapsed;
                    mainVM.IsTanLoaded = Visibility.Collapsed;
                    S.CloseAllWindows();
                    var taskSheetVm = (mainVM.taskSheet?.DataContext as TaskSheetVM);
                    if (taskSheetVm != null)
                        taskSheetVm.ClearTaskSheet();
                    LoginVM loginVM = new LoginVM();
                    LoginWindow.OpenLoginForm(loginVM);
                    if (!String.IsNullOrEmpty(U.UserName))
                    {
                        LoginWindow.HideLoginForm();
                        var check = U.RoleId == (int)Role.ProjectManger || U.RoleId == (int)Role.ToolManager;
                        mainVM.ShowDeliveryTab = mainVM.SettingsVisible = check ? Visibility.Visible : Visibility.Collapsed;
                        mainVM.AssignTaskVisble = check;
                        mainVM.SaveEnabled = U.RoleId == (int)Role.ProjectManger || U.RoleId == (int)Role.ToolManager ? false : true;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private void QueryWorkflowBtn_Click(object sender, RoutedEventArgs e)
        {
            QueryWorkflowWindow.ShowWindow();
        }

        private void QueryReportBtn_Click(object sender, RoutedEventArgs e)
        {
            QueryReport.ShowWindow();
        }

        private void DeliveryReportBtn_Click(object sender, RoutedEventArgs e)
        {
            DeliveryReport.ShowWindow();
        }

        private void PrepareTasks_Click(object sender, RoutedEventArgs e)
        {
            UploadShipment.ShowWindow();
        }

        private void AnalystId_Click(object sender, RoutedEventArgs e)
        {
            AnalystIDReportView.ShowWindow();
        }

        private void DailyStatus_Click(object sender, RoutedEventArgs e)
        {
            DailyStatusReport.ShowWindow();
        }
    }
}
