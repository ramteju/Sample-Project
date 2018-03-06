using Entities.DTO;
using System.Windows;

namespace Client.Notify
{
    /// <summary>
    /// Interaction logic for Notify.xaml
    /// </summary>
    public partial class Notify : Window
    {
        private static Notify thisInstance;
        public Notify()
        {
            InitializeComponent();
        }

        static public void Notification(string message, bool hideProgress = false)
        {
            if (thisInstance == null)
            {
                thisInstance = new Notify();
                var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
                thisInstance.Left = desktopWorkingArea.Right - thisInstance.Width;
                thisInstance.Top = desktopWorkingArea.Bottom - thisInstance.Height;
            }
            if (message == SignalRCode.DONE)
                thisInstance.Hide();
            else
            {
                thisInstance.Message.Text = message;
                thisInstance.Progressbar.Visibility = hideProgress ? Visibility.Hidden : Visibility.Visible;
                if (!thisInstance.IsVisible)
                    thisInstance.Show();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
    }
}
