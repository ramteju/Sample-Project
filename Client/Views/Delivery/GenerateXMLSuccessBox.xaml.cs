using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
    /// Interaction logic for GenerateXMLSuccessBox.xaml
    /// </summary>
    public partial class GenerateXMLSuccessBox : Window
    {
        private static GenerateXMLSuccessBox thisInstance;

        public string XMLPath { get; set; }

        public GenerateXMLSuccessBox()
        {
            InitializeComponent();
        }

        public static void ShowStatus(string xmlPath)
        {
            if (thisInstance == null)
            {
                thisInstance = new GenerateXMLSuccessBox();
            }
            thisInstance.XMLPath = xmlPath;
            thisInstance.Path.Content = xmlPath;
            thisInstance.ShowDialog();
        }

        private void openBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(XMLPath))
            {
                var directory = System.IO.Path.GetDirectoryName(XMLPath);
                if (Directory.Exists(directory))
                {
                    try
                    {
                        Process.Start(directory);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Can't Open Folder");
                    }
                }
            }
        }

        private void closeBtn_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }
    }
}
