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
    /// Interaction logic for AppInfoBox.xaml
    /// </summary>
    public partial class AppInfoBox : Window
    {
        public AppInfoBox()
        {
            InitializeComponent();
        }

        public static void ShowInfoMessage(string info)
        {
            var thisInstance = new AppInfoBox();
            thisInstance.UserMessage.Text = info;
            thisInstance.ShowDialog();
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
