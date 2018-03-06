using Client.Common;
using Client.Logging;
using Client.ViewModels.Delivery;
using Client.ViewModels.Tasks;
using Entities;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Interactivity;
using Telerik.Windows.Controls;

namespace Client.ViewModels.Utils
{
    public class ShipmentTansMultiSelectBehavior : Behavior<RadGridView>
    {
        private RadGridView Grid
        {
            get
            {
                return AssociatedObject as RadGridView;
            }
        }

        public INotifyCollectionChanged SelectedItems
        {
            get { return (INotifyCollectionChanged)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.
            Register("SelectedItems", typeof(INotifyCollectionChanged), typeof(ShipmentTansMultiSelectBehavior), new PropertyMetadata(OnSelectedItemsPropertyChanged));


        private static void OnSelectedItemsPropertyChanged(DependencyObject target, DependencyPropertyChangedEventArgs args)
        {
            
            try
            {
                var collection = args.NewValue as INotifyCollectionChanged;
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        protected override void OnAttached()
        {
            
            try
            {
                base.OnAttached();
                if(this.AssociatedObject.DataContext.GetType() == typeof(ShipmentsVM))
                    ((ShipmentsVM)this.AssociatedObject.DataContext).SelectedTans = this.AssociatedObject.SelectedItems;
                else if(this.AssociatedObject.DataContext.GetType() == typeof(TaskAllocationVM))
                    ((TaskAllocationVM)this.AssociatedObject.DataContext).SelectedTans = this.AssociatedObject.SelectedItems;
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }
    }
}
