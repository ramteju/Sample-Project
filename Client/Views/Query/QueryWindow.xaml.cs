using Client.ViewModels.Query;
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
using Excelra.Utils.Library;
using System.IO;
using System.Diagnostics;

namespace Client.Views
{
    /// <summary>
    /// Interaction logic for QueryWindow.xaml
    /// </summary>
    public partial class QueryWindow : Window
    {
        private static QueryWindow thisInstance;
        private QueryWindow()
        {
            InitializeComponent();
        }
        static public void ShowWindow()
        {
            if (thisInstance == null)
                thisInstance = new QueryWindow();
            if (!thisInstance.IsVisible)
            {
                (thisInstance.DataContext as QueryWindowVM).ClearState();
                thisInstance.Show();
            }
        }
        public static void SetTan(string tanNumber, int tanId)
        {
            ShowWindow();
            var queryWindowVM = thisInstance.DataContext as QueryWindowVM;
            queryWindowVM.FormQuery.TanId = tanId;
            queryWindowVM.FormQuery.TanNumber = tanNumber;
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void CopyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var responses = (DataContext as QueryWindowVM).Responses;
            if (responses != null)
            {
                string html = HtmlUtils.EnumerableToHtmlTable(responses, r => r.Response, r => r.User, r => r.Timestamp);
                string fileName = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".xls";
                try
                {
                    File.WriteAllText(fileName, html);
                    Process.Start(fileName);
                }
                catch (Exception ex)
                {
                    AppErrorBox.ShowErrorMessage("Error while generating excel . .", ex.ToString());
                }
            }
        }
    }
}
