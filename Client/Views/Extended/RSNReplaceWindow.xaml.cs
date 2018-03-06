using Client.ViewModels;
using Client.ViewModels.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Client.Views.Extended
{
    /// <summary>
    /// Interaction logic for RSNRelaceWindow.xaml
    /// </summary>
    public partial class RSNReplaceWindow : Window
    {
        public RSNReplaceWindow()
        {
            InitializeComponent();
            Closing += RSNReplaceWindow_Closing;
        }

        private void RSNReplaceWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private static RSNReplaceWindow thisWindow;

        public static void ShowFreetexts()
        {
            thisWindow = new RSNReplaceWindow();
            if ((App.Current.MainWindow) as MainWindow != null && ((App.Current.MainWindow) as MainWindow).DataContext as MainVM != null)
            {
                RSNReplaceVM rsnReplaceVM = new RSNReplaceVM();
                rsnReplaceVM.PrepareData((((App.Current.MainWindow) as MainWindow).DataContext as MainVM).TanVM);
                thisWindow.DataContext = rsnReplaceVM;
                thisWindow.Show();
            }
        }
    }
}
