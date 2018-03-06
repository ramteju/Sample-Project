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

namespace Client.Views.Query
{
    /// <summary>
    /// Interaction logic for QueryWorkflow.xaml
    /// </summary>
    public partial class QueryWorkflowWindow : Window
    {

        private static QueryWorkflowWindow thisInstance;
        static public void ShowWindow()
        {
            if (thisInstance == null)
                thisInstance = new QueryWorkflowWindow();
            if (!thisInstance.IsVisible)
            {
                (thisInstance.DataContext as QueryWorkflowWindowVM).ClearState();
                thisInstance.Show();
            }
        }
        public QueryWorkflowWindow()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
    }
}
