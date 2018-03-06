using Client.Logging;
using Client.ViewModels;
using System;
using System.Windows;

namespace Client
{
    /// <summary>
    /// Interaction logic for TaskSheet.xaml
    /// </summary>
    public partial class TaskSheet : Window
    {
        public TaskSheet()
        {
            InitializeComponent();
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                base.OnClosing(e);
                e.Cancel = true;
                (this.DataContext as TaskSheetVM).RowDoubleClicked = false;
                this.Hide();
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }
    }
}
