using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTO
{
    public class DiffTan
    {
        public Guid Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public string UserName { get; set; }
        public List<string> RSDs { get; set; }
        public List<DiffRSN> DiffRSNs { get; set; }
    }

    public class DiffRSN
    {
        public Guid Id { get; set; }
        public string CVT { get; set; }
        public string FreeText { get; set; }
    }

    public class TanComment
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public string Text { get; set; }
    }
}
