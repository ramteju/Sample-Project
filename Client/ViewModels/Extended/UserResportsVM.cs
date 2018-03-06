using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels.Extended
{
    public class UserResportsVM : ViewModelBase
    {
        private int reactionsCount;
        private string date;
        public int ReactionsCount { get { return reactionsCount; } set { SetProperty(ref reactionsCount, value); } }
        public string Date { get { return date; } set { SetProperty(ref date, value); } }
        public int ReactionsCountSize { get { return reactionsCount * 10; } }
    }
}
