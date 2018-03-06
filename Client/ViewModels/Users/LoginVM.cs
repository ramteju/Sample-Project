using Client.Command;
using Client.Common;
using Client.ViewModels;
using DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Client.Notify;
using Client.Views;
using Entities.DTO;
using Client.Logging;

namespace Client.ViewModel
{
    public class LoginVM : ViewModelBase
    {
        public LoginVM()
        {
            Hide = Visibility.Visible;
            roles = new ObservableCollection<ProductRole>();
            UserName = Environment.UserName;
        }
        private string userName;
        private string password;
        private ObservableCollection<ProductRole> roles;
        private int roleId;
        private int selectIndex;
        private bool loginEnable = true;
        private Visibility hide;
        private bool? dialogResult;

        public string UserName
        {
            get
            {
                return userName;
            }
            set
            {
                SetProperty(ref userName, value);
                LoadRoles(UserName).ContinueWith(r => { });
            }
        }
        public string Password { get { return password; } set { SetProperty(ref password, value); } }
        public string LoginTitle
        {
            get
            {
                return "Login - " + S.VersionInfo;
            }
        }
        public ObservableCollection<ProductRole> Roles { get { return roles; } set { SetProperty(ref roles, value); } }
        public int RoleId { get { return roleId; } set { SetProperty(ref roleId, value); } }
        public bool LoginEnable { get { return loginEnable; } set { SetProperty(ref loginEnable, value); } }
        public bool? DialogResult { get { return dialogResult; } set { SetProperty(ref dialogResult, value); } }
        public int SelectIndex { get { return selectIndex; } set { SetProperty(ref selectIndex, value); } }
        public Visibility Hide
        {
            get
            {
                return hide;
            }
            set
            {
                SetProperty(ref hide, value);
            }
        }

        public bool IsDirty
        {
            get;
            set;
        }

        private DelegateCommand login;
        public ICommand Login
        {
            get
            {
                if (login == null)
                {
                    login = new DelegateCommand(LoginAPP);
                }
                return login;
            }
        }

        private async Task loginUser()
        {

            try
            {
                RestStatus status = await RestHub.LoginUser(userName, password);
                if (status.HttpCode == System.Net.HttpStatusCode.OK)
                {
                    Hide = Visibility.Hidden;
                    U.UserName = userName;
                    U.RoleId = RoleId;
                    U.UserRole = (from r in Roles
                                                where r.RoleId == RoleId
                                                select r.RoleName).FirstOrDefault();
                    await UserPermissionInfo(RoleId);
                    await UserReactionsReports(RoleId);
                    ((App.Current.MainWindow as MainWindow).DataContext as MainVM).UserName = (U.UserName + " \\ " + U.UserRole).ToUpper();
                    try
                    {
                        HubClient.InitHub();
                        HubClient.NotificationReceived += HubClient_NotificationReceived;
                        ((App.Current.MainWindow as MainWindow).DataContext as MainVM).SignalRId = HubClient.signalRId;
                    }
                    catch (Exception ex)
                    {
                        AppErrorBox.ShowErrorMessage("Error while conneciton to live server . .", ex.ToString());
                    };
                }
                else
                {
                    AppInfoBox.ShowInfoMessage( status.StatusMessage);
                    LoginEnable = true;
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private void HubClient_NotificationReceived(object sender, string e)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                Notify.Notify.Notification(e);
            });
        }

        public void LoginAPP(object s)
        {

            try
            {
                var passwordBox = s as PasswordBox;
                Password = passwordBox.Password;
                LoginEnable = false;
                if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(RoleId.ToString()))
                {
                    MessageBox.Show("Username/Password cannot be empty", "Reactions", MessageBoxButton.OK, MessageBoxImage.Warning);
                    IsDirty = false;
                    LoginEnable = true;
                }
                else
                {
                    IsDirty = true;
                }
                if (IsDirty)
                {
                    loginUser().ContinueWith(r => { });
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);

            }
        }

        private async Task LoadRoles(string userName)
        {

            try
            {
                LoginEnable = false;
                Roles.Clear();
                RestStatus status = await RestHub.UserRoles(UserName);
                if (status.HttpCode == System.Net.HttpStatusCode.OK)
                {
                    List<ProductRole> listRoles = (List<ProductRole>)status.UserObject;
                    if (listRoles.Count > 0)
                    {
                        SelectIndex = 0;
                        foreach (ProductRole productRole in listRoles)
                            Roles.Add(productRole);
                        LoginEnable = true;
                    }
                }
                else
                {
                    AppErrorBox.ShowErrorMessage("Can't Load Roles", status.StatusMessage);
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private async Task UserPermissionInfo(int RoleId)
        {

            try
            {
                RestStatus status = await RestHub.UserPermissionsInfo(RoleId);
                if (status.HttpCode == System.Net.HttpStatusCode.OK)
                {
                    UserInfoDTO userInfoDTO = (UserInfoDTO)status.UserObject;
                    U.CanApprove = userInfoDTO.canApprove;
                    U.CanReject = userInfoDTO.canReject;
                    U.CanSubmit = userInfoDTO.canSubmit;
                }
                else
                {
                    AppErrorBox.ShowErrorMessage("Unable to load user permission", status.StatusMessage);
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);

            }
        }

        private async Task UserReactionsReports(int Roleid)
        {

            try
            {
                var mainVm = (App.Current.MainWindow as MainWindow).DataContext as MainVM;
                RestStatus status = await RestHub.UserReactionsReports(RoleId);
                if (status.HttpCode == System.Net.HttpStatusCode.OK)
                {
                    List<UserReportsDTO> userReportDto = (List<UserReportsDTO>)status.UserObject;
                    mainVm.UserReports = new ObservableCollection<ViewModels.Extended.UserResportsVM>();
                    mainVm.AllUserReports = new ObservableCollection<ViewModels.Extended.UserResportsVM>();
                    foreach (var userReport in userReportDto.Where(ur => ur.SingleUser))
                    {
                        mainVm.UserReports.Add(new ViewModels.Extended.UserResportsVM { Date = userReport.Date, ReactionsCount = userReport.ReactionsCount });
                    }
                    mainVm.UserReports.Add(new ViewModels.Extended.UserResportsVM { Date = DateTime.Now.AddDays(1).Date.ToString("dd-MM"), ReactionsCount = 0 });
                    foreach (var userReport in userReportDto.Where(ur => !ur.SingleUser))
                    {
                        mainVm.AllUserReports.Add(new ViewModels.Extended.UserResportsVM { Date = userReport.Date, ReactionsCount = userReport.ReactionsCount });
                    }
                    mainVm.AllUserReports.Add(new ViewModels.Extended.UserResportsVM { Date = DateTime.Now.AddDays(1).Date.ToString("dd-MM"), ReactionsCount = 0 });
                }
                else
                {
                    AppErrorBox.ShowErrorMessage("User Report Can't Be Loaded", status.StatusMessage);
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

    }
    public class ProductRole
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
    }

}
