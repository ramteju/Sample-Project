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
    /// Interaction logic for DeliveryReport.xaml
    /// </summary>
    public partial class DeliveryReport : Window
    {
        private static DeliveryReport thisInstance;
        public DeliveryReport()
        {
            InitializeComponent();
        }

        public static void ShowWindow()
        {
            if (thisInstance == null)
                thisInstance = new DeliveryReport();
            ((thisInstance.DataContext) as DeliveryReportVM).Clear();
            thisInstance.Show();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
    }
}
