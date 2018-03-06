using Client.ViewModels;
using Client.ViewModels.Delivery;
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

namespace Client.Views.Delivery
{
    /// <summary>
    /// Interaction logic for DeliveryFreetextReplace.xaml
    /// </summary>
    public partial class DeliveryFreetextReplace : Window
    {
        public DeliveryFreetextReplace()
        {
            InitializeComponent();
            Closing += DeliveryFreetextReplace_Closing;
        }
        private static DeliveryFreetextReplace thisWindow;
        private void DeliveryFreetextReplace_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
        public static void ShowFreetexts(FreetextReplaceVM rsnReplaceVM)
        {
            thisWindow = new DeliveryFreetextReplace();
            if ((App.Current.MainWindow) as MainWindow != null && ((App.Current.MainWindow) as MainWindow).DataContext as MainVM != null)
            {
                thisWindow.DataContext = rsnReplaceVM;
                thisWindow.ShowDialog();
            }
        }
    }
}
