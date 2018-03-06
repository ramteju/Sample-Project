using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

namespace Client.Views.Report
{
    /// <summary>
    /// Interaction logic for AnalystIDReportView.xaml
    /// </summary>
    public partial class AnalystIDReportView : Window
    {
        private static AnalystIDReportView thisWindow;
        public AnalystIDReportView()
        {
            InitializeComponent();
            Closing += AnalystIDReportView_Closing;
        }

        private void AnalystIDReportView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        public static void ShowWindow()
        {
            thisWindow = new AnalystIDReportView();
            thisWindow.Show();
        }



        private void WindowsFormsHost_Loaded(object sender, RoutedEventArgs e)
        {
            ReportDataSource reportDataSource = new ReportDataSource();
            Reactions_prodDataSet dataset = new Reactions_prodDataSet();
            dataset.BeginInit();
            reportDataSource.Name = "AnalystIDReport"; // Name of the DataSet we set in .rdlc
            reportDataSource.Value = dataset.AspNetUsers;
            this._reportViewer.LocalReport.DataSources.Add(reportDataSource);
            this._reportViewer.LocalReport.ReportEmbeddedResource = "Client.RDLC_Reports.AnalystIDReport.rdlc";
           
            dataset.EndInit();
            //fill data into adventureWorksDataSet
            Reactions_prodDataSetTableAdapters.AspNetUsersTableAdapter tableAdapter = new Reactions_prodDataSetTableAdapters.AspNetUsersTableAdapter();
            tableAdapter.ClearBeforeFill = true;
            tableAdapter.Fill(dataset.AspNetUsers);
            List<ReportParameter> parameters = new List<ReportParameter>();
            ReportParameter parameter = new ReportParameter();
            parameter.Name = "UserName";
            parameter.Values.Add("ramu.ankam");
            //Add parameter
            parameters.Add(parameter);
            //Set the parameter collection
            _reportViewer.LocalReport.SetParameters(parameters);
            _reportViewer.RefreshReport();
        }
    }
}
