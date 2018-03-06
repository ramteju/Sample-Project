using Client.Logging;
using System.Windows;

namespace Client.Views
{
    /// <summary>
    /// Interaction logic for DuplicateNamesList.xaml
    /// </summary>
    public partial class DuplicateNamesList : Window
    {
        public DuplicateNamesList()
        {
            InitializeComponent();
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                base.OnClosing(e);
                e.Cancel = true;
                this.Hide();
            }
            catch (System.Exception ex)
            {
                Log.This(ex);
            }
        }
    }
}
