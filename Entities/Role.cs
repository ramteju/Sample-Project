using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public enum Role
    {
        //[Description("Not Assigned Yet")]
        //Not_Assigned_Yet = 0,
        [Description("Curator")]
        Curator = 1,
        [Description("Reviewer")]
        Reviewer = 2,
        [Description("QC")]
        QC = 3,
        [Description("Tool Manager")]
        ToolManager = 4,
        [Description("Project Manager")]
        ProjectManger = 5
    }

    public enum TanAction
    {
        [Description("TAN Assigned")]
        TAN_ASSIGNED = 1,
        [Description("TAN Submitted")]
        TAN_SUBMITTED = 2,
        [Description("TAN Approved")]
        TAN_APPROVED = 3,
        [Description("TAN Rejected")]
        TAN_REJECTED = 4
    }
}
