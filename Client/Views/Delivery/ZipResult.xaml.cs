using Client.Logging;
using Entities.DTO;
using System;
using System.Diagnostics;
using System.Windows;

namespace Client.Views.Delivery
{
    /// <summary>
    /// Interaction logic for ZipResult.xaml
    /// </summary>
    public partial class ZipResult : Window
    {
        private static ZipResult thisInstance;
        public ZipResult()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                e.Cancel = true;
                Hide();
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }
        static public void ShowResult(ZipResultDTO zipResult)
        {
            try
            {
                if (thisInstance == null)
                {
                    thisInstance = new ZipResult();
                }
                thisInstance.result.Items.Clear();
                thisInstance.TotalTans.Content = String.Empty;
                foreach (var tanNumber in zipResult.TanNumbers)
                    thisInstance.result.Items.Add(tanNumber);
                thisInstance.TotalTans.Content = zipResult.TanNumbers.Count;
                thisInstance.ZipsPath.Content = zipResult.Path;
                thisInstance.TotalZips.Content = zipResult.Count;
                thisInstance.ShowDialog();
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(thisInstance.ZipsPath.Content.ToString());
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }
    }
}
