using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels.Tasks
{
    public class UsersVM : ViewModelBase
    {
        private string userId;
        private string name;
        private Role role;

        public string UserId { get { return userId; } set { SetProperty(ref userId, value); } }
        public string Name { get { return name; } set { SetProperty(ref name, value); } }
        public Role Role { get { return role; } set { SetProperty(ref role, value); } }
    }
}
