using Client.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels.Core
{
    public class TanKeyWordsVM : OrderableVM
    {
        private string keyWord;
        private int displayOrder;
        public string KeyWord { get { return keyWord; } set { SetProperty(ref keyWord, value); } }
        public override int DisplayOrder { get { return displayOrder; } set { SetProperty(ref displayOrder, value); } }
    }
}
