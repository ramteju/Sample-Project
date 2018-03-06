using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels.Delivery
{
    public class BatchSummaryVM : ViewModelBase
    {
        public int Id { get; set; }
        public int BatchNumber { get; set; }
        public int TansCount { get; set; }
    }
}
