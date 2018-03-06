using Client.Logging;
using System;
using System.Windows;

namespace Client.Views
{
    /// <summary>
    /// Interaction logic for Edit8500Window.xaml
    /// </summary>
    public partial class Edit8500Window : Window
    {
        public Edit8500Window()
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
    }
}
