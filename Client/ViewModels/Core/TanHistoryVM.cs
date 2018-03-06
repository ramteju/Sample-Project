using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels.Core
{
    public class TanHistoryVM : ViewModelBase
    {
        private int id;
        private string text;

        public int Id { get; set; }
        public string Text { get; set; }
    }
}
