using Client.Common;
using Client.ViewModels;
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
    /// Interaction logic for ReviewView.xaml
    /// </summary>
    public partial class ReviewView : UserControl
    {
        public ReviewView()
        {
            InitializeComponent();
            ((App.Current.MainWindow as MainWindow).DataContext as MainVM).WorkingAreaTabChanged += AnalogousUC_WorkingAreaTabChanged;
        }

        private void AnalogousUC_WorkingAreaTabChanged(object sender, int e)
        {
            if (e == 1)
                ReactionView.ScrollIntoView(ReactionView.SelectedItem);
        }

        private void ListView_Selected(object sender, SelectionChangedEventArgs e)
        {
            ReactionView.ScrollIntoView(ReactionView.SelectedItem);
            var mainVM = (App.Current.MainWindow as MainWindow).DataContext as MainVM;
            if (mainVM != null && mainVM.TanVM != null && ReactionView.SelectedItem != null)
                mainVM.TanVM.SelectedReaction = mainVM.TanVM.Reactions.Where(r => r.Id == (ReactionView.SelectedItem as ReactionViewVM).ReactionId).FirstOrDefault();
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            var mainVM = (App.Current.MainWindow as MainWindow).DataContext as MainVM;
            if (mainVM != null && mainVM.TanVM != null)
            {
                if (U.RoleId == 3)
                    foreach (var reactionVM in mainVM.TanVM.Reactions)
                        reactionVM.QCLastupdatedDate = DateTime.Now;
                mainVM.TanVM.PerformAutoSave("Curation Completed");
            }
        }
    }
}
