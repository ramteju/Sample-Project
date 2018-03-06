using Client.ViewModels.Query;
using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace Client.Views.Query
{
    /// <summary>
    /// Interaction logic for QueryReport.xaml
    /// </summary>
    public partial class QueryReport : Window
    {
        private static QueryReport thisInstance;

        public static void ShowWindow()
        {
            if (thisInstance == null)
            {
                thisInstance = new QueryReport();
                (thisInstance.DataContext as QueryReportVM).ClearState();
            }
            thisInstance.Show();
        }
        public QueryReport()
        {
            InitializeComponent();

            CultureInfo cultureInfo = new CultureInfo("en-IN");
            DateTimeFormatInfo dateInfo = new DateTimeFormatInfo();
            dateInfo.ShortDatePattern = "dd-MM-yyyy";
            cultureInfo.DateTimeFormat = dateInfo;
            From.Culture = cultureInfo;
            To.Culture = cultureInfo;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
    }
}
