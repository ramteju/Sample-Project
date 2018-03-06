using Client.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModel
{
    public abstract class OrderableVM : ViewModelBase
    {
        public abstract int DisplayOrder { get; set; }
    }
}
