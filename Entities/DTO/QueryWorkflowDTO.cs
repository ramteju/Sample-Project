using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTO
{
    public class QueryWorkflowDTO
    {
        public bool AllowSubmit { get; set; }
        public bool AllowReject { get; set; }
        public string PreviousUser { get; set; }
        public string CurrentUser { get; set; }
        public string NextUser { get; set; }
    }
}
