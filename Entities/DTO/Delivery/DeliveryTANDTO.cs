using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTO.Delivery
{
    public class DeliveryTanDTO
    {
        public int Id { get; set; }
        public string TanNumber { get; set; }
        public int RXNCount { get; set; }
        public bool IsQueried { get; set; }
        public string DeliveryRevertMessage { get; set; }
    }
}
