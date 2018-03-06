using Client.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels.Core
{

    public class PullTaskVM : OrderableVM
    {
        public PullTaskVM()
        {
            UserRanks = new ObservableCollection<RankVM>();
            TanRanks = new ObservableCollection<RankVM>();
        }

       
        private int displayOrder;
        private ObservableCollection<RankVM> userRanks;
        private ObservableCollection<RankVM> tanRanks;
        private string tanNumber;
        private int? userRank;
        private int? allottedTanRank;

        public override int DisplayOrder
        {
            get { return displayOrder; }
            set
            {
                SetProperty(ref displayOrder, value);
            }
        }

        public string TanNumber { get { return tanNumber; } set { SetProperty(ref tanNumber, value); } }
        public int? UserRank { get { return userRank; } set { SetProperty(ref userRank, value); } }
        public int? AllottedTanRank { get { return allottedTanRank; } set { SetProperty(ref allottedTanRank, value); } }
        public ObservableCollection<RankVM> UserRanks { get { return userRanks; } set { SetProperty(ref userRanks, value); } }
        public ObservableCollection<RankVM> TanRanks { get { return tanRanks; } set { SetProperty(ref tanRanks, value); } }

    }
}
