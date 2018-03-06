using Client.Logging;
using Client.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Client
{
    /// <summary>
    /// Interaction logic for CopyStages.xaml
    /// </summary>
    public partial class CopyStages : Window
    {
        public CopyStages()
        {
            InitializeComponent();
        }

        private void RadListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                CopyStageVM vm = (CopyStageVM)DataContext;

                foreach (StageVM item in e.AddedItems)
                {
                    vm.SelectedStages.Add(item);
                }

                foreach (StageVM item in e.RemovedItems)
                {
                    vm.SelectedStages.Remove(item);
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            e.Cancel = true;
            this.Hide();
        }
    }
}
