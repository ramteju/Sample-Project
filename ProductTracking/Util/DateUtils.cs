using Entities;
using ProductTracking.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductTracking.Util
{
    public static class DateUtils
    {
        public static string dateString(DateTime time)
        {
            try
            {
                if (time == null)
                    return String.Empty;
                TimeSpan difference = DateTime.Now.Subtract(time);
                int days = difference.Days;
                if (days < 0)
                    return Math.Abs(days) + " day(s) to go";
                if (days > 0)
                    return days + " day(s) before";
                return "Today";
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }
        public static string GetDaySuffix(int day)
        {
            switch (day)
            {
                case 1:
                case 21:
                case 31:
                    return "st";
                case 2:
                case 22:
                    return "nd";
                case 3:
                case 23:
                    return "rd";
                default:
                    return "th";
            }
        }

        public static IEnumerable<DateTime> DateRange(this DateTime startDate, DateTime endDate)
        {
            return Enumerable.Range(0, (endDate - startDate).Days + 1).Select(d => startDate.AddDays(d));
        }

    }
}