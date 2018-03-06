using Client.Common;
using Client.Logging;
using Client.ViewModels.Delivery;
using Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    /// Interaction logic for Shipments.xaml
    /// </summary>
    public partial class Shipments : Window
    {
        static private Shipments thisInstance;
        public Shipments()
        {
            InitializeComponent();
        }

        static public void ShowShipments()
        {
            
            try
            {
                if (thisInstance == null)
                    thisInstance = new Shipments();
                if (!thisInstance.IsVisible)
                {
                    thisInstance = new Shipments();
                    thisInstance.Show();
                    thisInstance.ResetWindow();
                }
                thisInstance.Focus();
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private void ResetWindow()
        {
            
            try
            {
                var shipmentsVM = this.DataContext as ShipmentsVM;
                if (shipmentsVM != null)
                {
                    shipmentsVM.ClearState();
                    shipmentsVM.LoadData();
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private void WorkingAreaControl_Closing(object sender, System.ComponentModel.CancelEventArgs e)
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

        private void ShipmentsGrid_Filtered(object sender, Telerik.Windows.Controls.GridView.GridViewFilteredEventArgs e)
        {
            var filteredData = new ObservableCollection<BatchTanVM>();
            foreach (var item in this.ShipmentsGrid.Items)
            {
                filteredData.Add(item as BatchTanVM);
            }
            var shipmentsVM = this.DataContext as ShipmentsVM;
            if (shipmentsVM != null)
            {
                shipmentsVM.UpdateSummary(filteredData);
            }
        }
    }
}
