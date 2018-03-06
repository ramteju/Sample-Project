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
    /// Interaction logic for AppMessageBox.xaml
    /// </summary>
    public partial class AppErrorBox : Window
    {
        public string Exception { get; set; }
        public AppErrorBox()
        {
            InitializeComponent();
        }

        public static void ShowErrorMessage(string userMessage, string exception)
        {
            var errorBox = new AppErrorBox();
            errorBox.UserMessage.Text = userMessage;
            errorBox.Exception = exception;
            errorBox.ShowDialog();
        }


        private void CopyBtn_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(Exception);
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
