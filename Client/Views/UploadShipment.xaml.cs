using Client.ViewModels.Shipment;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client.Views
{
    /// <summary>
    /// Interaction logic for UploadShipment.xaml
    /// </summary>
    public partial class UploadShipment : Window
    {
        private static UploadShipment thisInstance;
        public UploadShipment()
        {
            InitializeComponent();
        }

        public static void ShowWindow()
        {
            if (thisInstance == null)
                thisInstance = new UploadShipment();
            thisInstance.Show();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
    }
}
