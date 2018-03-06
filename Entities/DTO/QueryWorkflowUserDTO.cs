using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTO
{
    public class QueryWorkflowUserDTO
    {
        public string L1User { get; set; }
        public string L2User { get; set; }
        public string L3User { get; set; }
        public string L4User { get; set; }
        public string L5User { get; set; }

        public bool HasValidData
        {
            get
            {
                return (!String.IsNullOrEmpty(L1User))
                    &&
                    (
                    !String.IsNullOrEmpty(L2User)
                    || !String.IsNullOrEmpty(L3User)
                    || !String.IsNullOrEmpty(L4User)
                    || !String.IsNullOrEmpty(L5User)
                    );
            }
        }
    }
}
