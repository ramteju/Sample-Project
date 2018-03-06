using ProductTracking.Models.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class DeliveryBatch
    {
        public DeliveryBatch()
        {
            Tans = new List<Tan>();
        }
        public int Id { get; set; }
        [Index(IsUnique = true)]
        public int BatchNumber { get; set; }
        public bool Delivered { get; set; }
        public bool IsRedelivered { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? DeliveredDate { get; set; }
        public virtual ICollection<Tan> Tans { get; set; }
    }
}
