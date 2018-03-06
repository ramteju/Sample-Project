using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace ProductTracking.Models.Core
{
    public enum TanState
    {
        [Description("Not Assigned")]
        Not_Assigned = 0,
        [Description("Assigned To Curator")]
        Curation_Assigned = 20,
        [Description("ReAssigned To Curator")]
        Curation_ReAssigned = 22,
        [Description("Curation In Progress")]
        Curation_InProgress = 23,
        [Description("Curation Completed")]
        Curation_Submitted = 24,
        [Description("Curation-Assigned(Rejected)")]
        Curation_Assigned_Rejected = 25,
        [Description("Curation-Progress(Rejected)")]
        Curation_Progress_Rejected = 26,
        [Description("Assigned To Review")]
        Review_Assigned = 30,
        [Description("ReAssigned To Review")]
        Review_ReAssigned = 31,
        [Description("Review In Progress")]
        Review_InProgress = 32,
        [Description("Review Accepted")]
        Review_Accepted = 33,
        [Description("Review Rejected")]
        Review_Rejected = 34,
        [Description("Review-Assigned(Rejected)")]
        Review_Assigned_Rejected = 35,
        [Description("Review-Progress(Rejected)")]
        Review_Progress_Rejected = 36,
        [Description("Assigned To QC")]
        QC_Assigned = 41,
        [Description("ReAssigned To QC")]
        QC_ReAssigned = 42,
        [Description("QC In Progress")]
        QC_InProgress = 43,
        [Description("QC Accepted")]
        QC_Accepted = 44,
        [Description("QC Rejected")]
        QC_Rejected = 45,
        [Description("All")]
        ALL = 46
    }

    
}