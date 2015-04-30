using System;
using System.Collections.Generic;
using System.Linq;

namespace ElzUtilLibary.Dates
{
    public static class DateTimeExtensions
    {
        public static List<DateTime> GetDayRange(this DateTime start, DateTime end)
        {
            var dayRange = new List<DateTime>();

            for (int d = 0; d <= (end.DayOfYear - start.DayOfYear); d++)
            {
                dayRange.Add(start.AddDays(d));
            }

            return dayRange;
        }

        public static bool AreDayRangesConflicted(this IEnumerable<DateTime> dayRange, List<DateTime> dayRange2)
        {
            bool isConflicted = false;

            foreach (DateTime date1 in dayRange)
            {
                if (dayRange2.Any(date2 => date1.Date == date2.Date))
                {
                    isConflicted = true;
                }
            }

            return isConflicted;
        }
    }
}