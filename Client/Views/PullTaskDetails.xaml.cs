using Client.ViewModels.Core;
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

namespace Client.Views
{
    /// <summary>
    /// Interaction logic for PullTaskDetails.xaml
    /// </summary>
    public partial class PullTaskDetails : Window
    {
        private static PullTaskDetails thisInstance;
        public PullTaskDetails()
        {
            InitializeComponent();
        }

        public static void ShowWindow(PullTaskVM viewModel)
        {
            if (thisInstance == null)
            {
                thisInstance = new PullTaskDetails();
            }
            thisInstance.DataContext = viewModel;
            thisInstance.Show();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
    }
}
