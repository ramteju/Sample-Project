using ProductTracking.Models.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class DateWiseRXNCount
    {
        public int Id { get; set; }
        public virtual ApplicationUser User { get; set; }
        [ForeignKey("User")]
        public string UserId { get; set; }
        public Role Role { get; set; }
        public virtual Tan Tan { get; set; }
        [ForeignKey("Tan")]
        public int TanId { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int RxnCount { get; set; }
    }

    public class TanActionHistories
    {
        public int Id { get; set; }
        public virtual ApplicationUser User { get; set; }
        [ForeignKey("User")]
        public string UserId { get; set; }
        public Role Role { get; set; }
        public virtual Tan Tan { get; set; }
        [ForeignKey("Tan")]
        public int TanId { get; set; }
        public TanAction TanAction { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string Description { get; set; }
    }
}
