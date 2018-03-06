using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductTracking.Models.Core
{
    interface Auditable
    {
        string CreatedBy { get; set; }
        DateTime? DateCreated { get; set; }
        string UpdatedBy { get; set; }
        DateTime? LastUpdated { get; set; }
    }
}
