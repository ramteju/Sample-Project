using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public enum QueryRole
    {
        [Description("Curator")]
        L1 = 1,
        [Description("Reviewer")]
        L2 = 2,
        [Description("SME")]
        L3 = 3,
        [Description("QC")]
        L4 = 4,
        [Description("Tool Manager")]
        L5 = 5,
        [Description("Project Manager")]
        L6 = 6
    }
}
