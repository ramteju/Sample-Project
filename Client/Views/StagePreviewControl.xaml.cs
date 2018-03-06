using Client.Common;
using Client.ViewModels;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Client.Views
{
    /// <summary>
    /// Interaction logic for StagePreviewControl.xaml
    /// </summary>
    public partial class StagePreviewControl : UserControl
    {
        public StagePreviewControl()
        {
            InitializeComponent();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var mainVM = (App.Current.MainWindow as MainWindow).DataContext as MainVM;
            if (mainVM != null && mainVM.TanVM != null)
            {
                var ReactionViewVM = this.DataContext as ReactionViewVM;
                var selectedReaction = mainVM.TanVM.Reactions.Where(r => r.Id == ReactionViewVM.ReactionId).FirstOrDefault();
                if (selectedReaction != null && U.RoleId == 2)
                {
                    if (selectedReaction.ReviewerCreatedDate == DateTime.MinValue)
                        selectedReaction.ReviewerCreatedDate = DateTime.Now;
                    if (selectedReaction.ReviewerCompletedDate == DateTime.MinValue)
                        selectedReaction.ReviewerCompletedDate = DateTime.Now;
                    selectedReaction.ReviewLastupdatedDate = DateTime.Now;
                }
                mainVM.TanVM.PerformAutoSave("Review Completed");
            }
        }
    }
}
