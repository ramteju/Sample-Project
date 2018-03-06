using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels.Delivery
{
    public class DeliveryTanVM : ViewModelBase
    {
        public int Id { get; set; }
        public string TanNumber { get; set; }
        public int RXNCount { get; set; }
        public bool IsQueried { get; set; }
        public string DeliveryRevertMessage { get; set; }
    }
}
