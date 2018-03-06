using Client.Common;
using Client.Logging;
using Client.ViewModel;
using Client.Views;
using System;
using System.Windows;

namespace Client
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        static private LoginWindow thisWindow;
        static LoginWindow()
        {
            thisWindow = new LoginWindow();
            LoginVM loginVM = new LoginVM();
            thisWindow.DataContext = loginVM;
        }
        public LoginWindow()
        {
            InitializeComponent();
            txtPassword.Focus();
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            S.CloseAllWindows();
            App.Current.Shutdown();
        }

        private void LoginForm_Loaded(object sender, RoutedEventArgs e)
        {
            txtPassword.Focus();
            this.Focus();
        }

        static public void HideLoginForm()
        {
            thisWindow.Hide();
        }

        static public void OpenLoginForm(LoginVM loginVM)
        {
            if (thisWindow == null)
                thisWindow = new LoginWindow();
            thisWindow.txtPassword.Clear();
            thisWindow.DataContext = loginVM;
            thisWindow.ShowDialog();
        }

        private void SelfCheckBtn_Click(object sender, RoutedEventArgs e)
        {
            SelfCheck.ShowWindow();
        }
    }
}
