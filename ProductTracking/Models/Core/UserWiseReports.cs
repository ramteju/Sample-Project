using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductTracking.Models.Core
{
    public class UserWiseReports
    {
        public string UserName { get; set; }
        public int CuratedReactionsCount { get; set; }
        public int CuratedStagesCount { get; set; }
        public int CuratedAnalogousReactionsCount { get; set; }

        public int ReviewedReactionsCount { get; set; }
        public int ReviewedStagesCount { get; set; }
        public int ReviewedAnalogousReactionsCount { get; set; }

        public int QcReactionsCount { get; set; }
        public int QcStagesCount { get; set; }
        public int QcAnalogousReactionsCount { get; set; }

    }

    public class ReactionsCount
    {
        public int TotalReactionsCount { get; set; }
        public int StagesCount { get; set; }
        public int AnalogousReactionsCount { get; set; }
    }
}