using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels.Delivery
{
    public class RoleVM : ViewModelBase
    {
        private int role;
        private string displayName;

        public int Role { get { return role; } set { SetProperty(ref role, value); } }
        public string DisplayName { get { return displayName; } set { SetProperty(ref displayName, value); } }
    }
}
