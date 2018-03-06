using Client.ViewModel;

namespace Client.ViewModels.Core
{
    public class RankVM :  ViewModelBase
    {
        private string key;
        private int rank;
        private string score;
        private int displayOrder;

        public string Key { get { return key; } set { SetProperty(ref key, value); } }
        public int Rank { get { return rank; } set { SetProperty(ref rank, value); } }
        public string Score { get { return score; } set { SetProperty(ref score, value); } }
        public int DisplayOrder
        {
            get { return displayOrder; }
            set
            {
                SetProperty(ref displayOrder, value);
            }
        }
    }
}