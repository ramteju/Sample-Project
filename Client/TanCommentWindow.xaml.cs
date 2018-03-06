using Client.Logging;
using Client.ViewModels;
using Client.ViewModels.Core;
using System;
using System.Windows;

namespace Client
{
    /// <summary>
    /// Interaction logic for TanCommentWindow.xaml
    /// </summary>
    public partial class TanCommentWindow : Window
    {
        static private TanCommentWindow thisWindow;
        static TanCommentWindow()
        {
            thisWindow = new TanCommentWindow();
        }

        public TanCommentWindow()
        {
            InitializeComponent();
        }

        static public void ShowComments(TanCommentsVM commentsVM)
        {
            try
            {
                thisWindow.DataContext = commentsVM;
                commentsVM.ClearVM(null);
                thisWindow.Show();
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Hide();
            e.Cancel = true;
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ((App.Current.MainWindow as MainWindow).DataContext as MainVM).TanVM.PerformAutoSave("Added TanComments");
                Hide();
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }
    }
}
