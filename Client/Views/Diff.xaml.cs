using Client.Logging;
using Client.ViewModels;
using System;
using System.Windows;

namespace Client.Views
{
    /// <summary>
    /// Interaction logic for Diff.xaml
    /// </summary>
    public partial class Diff : Window
    {
        static private Diff thisInstance;

        public Diff()
        {
            InitializeComponent();
        }
        static public void ShowWindow()
        {
            try
            {
                if (thisInstance == null)
                    thisInstance = new Diff();
                if (!thisInstance.IsVisible)
                {
                    thisInstance.Show();
                    thisInstance.ResetWindow();
                }
                thisInstance.Focus();
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }
        private void ResetWindow()
        {
            try
            {
                var diffVm = this.DataContext as DiffVM;
                if (diffVm != null)
                {
                    diffVm.ClearState();
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
    }
}
