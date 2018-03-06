using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public enum TanCategory
    {
        [Description("In Progress")]
        Progress = 1,
        [Description("Patents")]
        Patents = 2,
        [Description("Journals")]
        Journals = 3,
        [Description("Ready To Deliver")]
        ReadyToDeliver = 4,
        [Description("Delivered")]
        Delivered = 5,
        All = 100
    }
}
