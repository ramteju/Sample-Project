using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using ProductTracking.Models.Core;
namespace Entities
{
   public class TanHistory
    {
        public int id { get; set; }
        public virtual Tan Tan { get; set; }
        [ForeignKey("Tan")]
        public int TanId { get; set; }
        [Column(TypeName = "text")]
        public String Data { get; set; }
        public DateTime? Date { get; set; }
        public virtual ApplicationUser User { get; set; }
        [ForeignKey("User")]
        public string UserId { get; set; }
        public int UserRoleId { get; set; }
        public string Ip { get; set; }
    }
}
