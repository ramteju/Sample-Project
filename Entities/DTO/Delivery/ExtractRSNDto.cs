using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTO
{
    public class ExtractRSNDto
    {
        public Guid Id { get; set; }
        public string TanNumber { get; set; }
        public int RXNSno { get; set; }
        public int? ProductNumber { get; set; }
        public int RxnSeq { get; set; }
        public int? Stage { get; set; }
        public string CVT { get; set; }
        public string FreeText { get; set; }
        public string RSNType { get; set; }
    }
}
