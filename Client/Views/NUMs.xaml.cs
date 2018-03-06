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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client.Views
{
    /// <summary>
    /// Interaction logic for NUMs.xaml
    /// </summary>
    public partial class NUMs : UserControl
    {
        public NUMs()
        {
            InitializeComponent();
            txtNumSearch.Focus();
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (numsListView != null && numsListView.SelectedItem != null)
                numsListView.ScrollIntoView(numsListView.SelectedItem);
        }

    }
}
