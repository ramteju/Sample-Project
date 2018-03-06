using Microsoft.Reporting.WinForms;
using System.Windows;
using System.ComponentModel;

namespace Client.Views.Report
{
    /// <summary>
    /// Interaction logic for DailyStatusReport.xaml
    /// </summary>
    public partial class DailyStatusReport : Window
    {
        private static DailyStatusReport thisWindow;

        public DailyStatusReport()
        {
            InitializeComponent();
            Closing += DailyStatusReportView_Closing;
        }

        private void DailyStatusReportView_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
        public static void ShowWindow()
        {
            thisWindow = new DailyStatusReport();
            thisWindow.Show();
        }
        private void WindowsFormsHost_Loaded(object sender, RoutedEventArgs e)
        {
            ReportDataSource reportDataSource = new ReportDataSource();
            Reactions_prodDataSet dataset = new Reactions_prodDataSet();
            dataset.BeginInit();
            reportDataSource.Name = "DailyStatusReport"; // Name of the DataSet we set in .rdlc
            reportDataSource.Value = dataset.AspNetUsers;
            this._reportViewer.LocalReport.DataSources.Add(reportDataSource);
            this._reportViewer.LocalReport.ReportEmbeddedResource = "Client.RDLC_Reports.DailyStatusReport.rdlc";
            dataset.EndInit();
            //fill data into adventureWorksDataSet
            Reactions_prodDataSetTableAdapters.AspNetUsersTableAdapter tableAdapter = new Reactions_prodDataSetTableAdapters.AspNetUsersTableAdapter();
            tableAdapter.ClearBeforeFill = true;
            tableAdapter.Fill(dataset.AspNetUsers);
            _reportViewer.RefreshReport();
        }
    }
}
