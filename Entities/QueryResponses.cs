using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class QueryResponse
    {
        public int Id { get; set; }
        public string Response { get; set; }
        public virtual Query Query { get; set; }
        [Required]
        public int QueryId { get; set; }
        public DateTime Timestamp { get; set; }
        public virtual ApplicationUser User { get; set; }
        [Required]
        public string UserId { get; set; }
    }
}
