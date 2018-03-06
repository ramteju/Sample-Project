using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTO
{
   public class QueryResponseDTO
    {
        public int Id { get; set; }
        public string Response { get; set; }
        public string User { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
