using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductTracking.Models.Core.ViewModels
{
    public class DeliveryReportAbstract
    {
        public DateTime BatchReceiveddate { get; set; }
        public int DaysDiffFrBWBatches { get; set; }
        public string SpreadSheetName { get; set; }
        public string batRange { get; set; }
        public Version ToolVersion { get; set; }
        public int TANCount { get; set; }
        public int NumsCount { get; set; }
        public int RXNCount { get; set; }
        public int ZeroRxnCount { get; set; }
        public int NumsDensity { get; set; }
        public int RXNDensity { get; set; }
        public int JournelTanCount { get; set; }
        public int JournelRxnCount { get; set; }
        public int JournelZeroRxnTanCount { get; set; }
        public int PatentTanCount { get; set; }
        public int PatentRxnCount { get; set; }
        public int PatentZeroRxnTanCount { get; set; }
        public DateTime DestroyDate { get; set; }
        public int NoOfTansDelivered { get; set; }
        public int NoOfPendingTans { get; set; }
        public DateTime CompletionDateFromClient { get; set; }
        public DateTime CompletedBatchDelieverydate { get; set; }
        public int PriorityDays { get; set; }
        public int BatchDeliveredWithIn { get; set; }
        public int DeviationFromPriorityDays { get; set; }
        public string Remarks { get; set; }
        public string Comments { get; set; }
    }
}