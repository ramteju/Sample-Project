using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels.Delivery
{
    public class DeliveryBatchVM : ViewModelBase
    {
        private int id, batchNumber;
        public int Id { get { return id; } set { SetProperty(ref id, value); } }
        public int BatchNumber { get { return batchNumber; } set { SetProperty(ref batchNumber, value); } }
    }
}
