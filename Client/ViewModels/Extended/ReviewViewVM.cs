using Client.ViewModels.Extended;
using System.Collections.ObjectModel;
using System.Windows;

namespace Client.ViewModels
{
    public class ReviewViewVM : ViewModelBase
    {
        private ObservableCollection<ReactionViewVM> reactionViews;
        private bool isQCCompleted;
        private Visibility qcCompleteCheckVisibility;
        private ReactionViewVM selectedReviewReaction;
        private QCTAbleViewVM qCTAbleViewVM;

        public ObservableCollection<ReactionViewVM> ReactionViews { get { return reactionViews; } set { SetProperty(ref reactionViews, value); } }
        public QCTAbleViewVM QCTAbleViewVM { get { return qCTAbleViewVM; } set { SetProperty(ref qCTAbleViewVM, value); } }
        public bool IsQCCompleted
        {
            get { return isQCCompleted; }
            set
            {
                if (((App.Current.MainWindow as MainWindow)?.DataContext as MainVM)?.TanVM != null)
                {
                    ((App.Current.MainWindow as MainWindow).DataContext as MainVM).TanVM.IsQCCompleted = value;
                    SetProperty(ref isQCCompleted, ((App.Current.MainWindow as MainWindow).DataContext as MainVM).TanVM.IsQCCompleted);
                }
            }
        }
        public Visibility QcCompleteCheckVisibility { get { return qcCompleteCheckVisibility; } set { SetProperty(ref qcCompleteCheckVisibility, value); } }
        public ReactionViewVM SelectedReviewReaction { get { return selectedReviewReaction; } set { SetProperty(ref selectedReviewReaction, value); } }

        public ReviewViewVM()
        {
            ReactionViews = new ObservableCollection<ReactionViewVM>();
        }
    }
}
