using Client.ViewModels.Reports;
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

namespace Client.Views.Report
{
    /// <summary>
    /// Interaction logic for ErrorReport.xaml
    /// </summary>
    public partial class ErrorReportWindow : Window
    {
        public ErrorReportWindow()
        {
            InitializeComponent();
            Closing += ErrorReportWindow_Closing;
        }
        public static bool Result;
        private void ErrorReportWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            thisWindow.Hide();
        }

        private static ErrorReportWindow thisWindow;
        public static bool? showWindow(ErrorReportVM vm)
        {
            if (thisWindow == null)
                thisWindow = new ErrorReportWindow();
            thisWindow.DataContext = vm;
            var result = thisWindow.ShowDialog();
            return result;
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            Result = true;
            DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Result = false;
            DialogResult = false;
        }
    }
}
