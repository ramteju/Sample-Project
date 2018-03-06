using Client.ViewModel;
using Entities.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels.Query
{
    public class QueryWorkflowVM : OrderableVM
    {
        private string l1user, l2user, l3user, l4user, l5user;
        private int displayOrder;

        public string L1user { get { return l1user; } set { SetProperty(ref l1user, value); } }
        public string L2user { get { return l2user; } set { SetProperty(ref l2user, value); OnPropertyChanged("IsValid"); } }
        public string L3user { get { return l3user; } set { SetProperty(ref l3user, value); OnPropertyChanged("IsValid"); } }
        public string L4user { get { return l4user; } set { SetProperty(ref l4user, value); OnPropertyChanged("IsValid"); } }
        public string L5user { get { return l5user; } set { SetProperty(ref l5user, value); OnPropertyChanged("IsValid"); } }
        public string IsValid
        {
            get
            {
                return new System.Collections.Generic.List<string> { L1user, L2user, L3user, L4user, L5user }
                .Where(s => !String.IsNullOrEmpty(s))
                .GroupBy(x => x)
                .Where(group => group.Count() > 1)
                .Any() ? YesNo.No.ToString() : YesNo.Yes.ToString();
            }
        }
        public override int DisplayOrder { get { return displayOrder; } set { SetProperty(ref displayOrder, value); } }
    }
}
