using Client.Common;
using Client.Logging;
using Client.ViewModels;
using Entities;
using System.Windows;

namespace Client.Views.Delivery
{
    /// <summary>
    /// Interaction logic for XmlVerfication.xaml
    /// </summary>
    public partial class XmlVerfication : Window
    {
        private static XmlVerfication thisInstance;

        public XmlVerfication()
        {
            InitializeComponent();
        }
        static public void ShowWindow()
        {
            
            try
            {
                if (thisInstance == null)
                    thisInstance = new XmlVerfication();
                if (!thisInstance.IsVisible)
                {
                    thisInstance.Show();
                    thisInstance.ResetWindow();
                }
                thisInstance.Focus();
            }
            catch (System.Exception ex)
            {
                Log.This(ex);
            }
        }

        private void ResetWindow()
        {
            
            try
            {
                var xmlVerificationVM = this.DataContext as XMLVerificationVM;
                if (xmlVerificationVM != null)
                {
                    xmlVerificationVM.ClearState();
                }
            }
            catch (System.Exception ex)
            {
                Log.This(ex);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            
            try
            {
                e.Cancel = true;
                Hide();
            }
            catch (System.Exception ex)
            {
                Log.This(ex);
            }
        }

        private void WebBrowser_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            
            try
            {
                (DataContext as XMLVerificationVM).IsBusy = true;
            }
            catch (System.Exception ex)
            {
                Log.This(ex);
            }
        }

        private void WebBrowser_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            
            try
            {
                (DataContext as XMLVerificationVM).IsBusy = false;
            }
            catch (System.Exception ex)
            {
                Log.This(ex);
            }
        }
    }
}
