using Client.Logging;
using Client.ViewModels.Delivery;
using Client.ViewModels.Tasks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for TaskAllocationWindow.xaml
    /// </summary>
    public partial class TaskAllocationWindow : Window
    {
        static private TaskAllocationWindow thisInstacne;
        public TaskAllocationWindow()
        {
            InitializeComponent();
            this.Closing += TaskAllocationWindow_Closing;
        }

        private void TaskAllocationWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        public static void showWindow()
        {
            if (thisInstacne == null)
                thisInstacne = new TaskAllocationWindow();

            if (!thisInstacne.IsVisible)
            {
                thisInstacne.Batches.SelectedItems = null;
                var dc = thisInstacne.DataContext as TaskAllocationVM;
                dc.RefreshVM();
                thisInstacne.Show();
                (thisInstacne.DataContext as TaskAllocationVM).LoadData().ContinueWith(r => { });
            }
            else
                thisInstacne.Activate();
        }

        private void ShipmentTans_Filtered(object sender, Telerik.Windows.Controls.GridView.GridViewFilteredEventArgs e)
        {
            var filteredData = new ObservableCollection<BatchTanVM>();
            foreach (var item in this.ShipmentTans.Items)
                filteredData.Add(item as BatchTanVM);
            var shipmentsVM = this.DataContext as TaskAllocationVM;
            if (shipmentsVM != null)
                shipmentsVM.UpdateSummary(filteredData);
        }

        private void RadAutoCompleteBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                TaskAllocationVM vm = this.DataContext as TaskAllocationVM;

                foreach (BatchVM item in e.AddedItems)
                {
                    vm.SelectedBatches.Add(item);
                }

                foreach (BatchVM item in e.RemovedItems)
                {
                    vm.SelectedBatches.Remove(item);
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }
    }
}
