using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTO
{
    public class QueryReportEntryDTO
    {
        public string User { get; set; }
        public int Created { get; set; }
        public int Responded { get; set; }
    }
}
