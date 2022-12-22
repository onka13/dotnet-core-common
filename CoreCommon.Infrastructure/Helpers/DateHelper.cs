using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace CoreCommon.Infrastructure.Helpers
{
    public static class DateHelper
    {
        public static DateTime Now()
        {
            return DateTime.UtcNow;
        }

        /// <summary>
        /// Parse from string which date format like 20210921 or 20210921033612.
        /// </summary>
        /// <param name="dateValue"></param>
        /// <returns></returns>
        public static DateTime? ParseFromStringYMD(string dateValue)
        {
            if (string.IsNullOrWhiteSpace(dateValue))
            {
                return null;
            }

            dateValue = Regex.Replace(dateValue, "[-:\\.]", string.Empty).Trim();

            var format = dateValue.Length == 8 ? "yyyyMMdd" : "yyyyMMddHHmmss";
            return DateTime.ParseExact(dateValue, format, CultureInfo.InvariantCulture, DateTimeStyles.None);
        }

        public static IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
            {
                yield return day;
            }
        }

        /// <summary>
        /// Method to read given date and return
        /// first day of that week.
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        public static DateTime GetFirstDayOfWeek(DateTime day)
        {
            DayOfWeek firstDay = CultureInfo.GetCultureInfo("en-GB").DateTimeFormat.FirstDayOfWeek;
            DateTime firstDayInWeek = day.Date;
            while (firstDayInWeek.DayOfWeek != firstDay)
            {
                firstDayInWeek = firstDayInWeek.AddDays(-1);
            }

            return firstDayInWeek;
        }

        /// <summary>
        /// Method to read given date and return
        /// last day of that week.
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        public static DateTime GetLastDayOfWeek(DateTime day)
        {
            DayOfWeek firstDay = CultureInfo.GetCultureInfo("en-GB").DateTimeFormat.FirstDayOfWeek;
            DateTime firstDayInWeek = day.Date;
            while (firstDayInWeek.DayOfWeek != firstDay)
            {
                firstDayInWeek = firstDayInWeek.AddDays(-1);
            }

            DateTime lastDayInWeek = firstDayInWeek.AddDays(6);
            return lastDayInWeek;
        }

        public static DateTime FirstDateOfWeek(int year, int weekOfYear)
        {
            DateTime jan1 = new DateTime(year, 1, 1);

            int daysOffset = (int)CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek - (int)jan1.DayOfWeek;

            DateTime firstMonday = jan1.AddDays(daysOffset);
            CalendarWeekRule cWR = CalendarWeekRule.FirstDay;

            int firstWeek = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(jan1, cWR, CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek);

            if (firstWeek == 53)
            {
                weekOfYear = 1;
            }

            if (firstWeek <= 1)
            {
                weekOfYear -= 1;
            }

            return firstMonday.AddDays(weekOfYear * 7);
        }

        public static bool OverlappingPeriods(DateTime aStart, DateTime aEnd, DateTime bStart, DateTime bEnd)
        {
            if (aStart > aEnd)
            {
                return false;
            }

            if (bStart > bEnd)
            {
                return false;
            }

            return !(aEnd <= bStart && aStart <= bStart || bEnd <= aStart && bStart <= aStart);
        }

        public static bool IsDateInRange(this DateTime dateToCheck, DateTime startDate, DateTime endDate)
        {
            return dateToCheck >= startDate && dateToCheck <= endDate;
        }

        public static bool IsDateInRange(this DateTime dateToCheck, DateTime? startDate, DateTime? endDate)
        {
            return (startDate.HasValue ? startDate <= dateToCheck : true) && (endDate.HasValue ? endDate >= dateToCheck : true);
        }

        public static DateTime DateNumberOfWeeksBefore(int week)
        {
            return DateTime.UtcNow.AddDays(-(week * 7));
        }

        public static DateTime MinDate()
        {
            return new DateTime(1970, 1, 1, 0, 0, 0);
        }

        public static DateTime MaxDate()
        {
            return new DateTime(9999, 12, 31, 0, 0, 0);
        }

        public static DateTime ClarizenMinDate()
        {
            return new DateTime(1980, 1, 1, 0, 0, 0);
        }

        public static DateTime ClarizenMaxDate()
        {
            return new DateTime(2999, 12, 31, 0, 0, 0);
        }

        public static DateTime ToDateWithoutTimezone(this DateTime d)
        {
            var newValue = new DateTime(d.Year, d.Month, d.Day, 0, 0, 0, DateTimeKind.Utc);
            return newValue;
        }

        public static DateTime FromUnixTime(this long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTime);
        }

        public static long ToUnixTime(this DateTime date)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64((date - epoch).TotalSeconds);
        }

        public static int GetWeekNumber(DateTime date)
        {
            CultureInfo ciCurr = CultureInfo.CurrentCulture;
            int weekNum = ciCurr.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstDay, DayOfWeek.Monday);

            if (weekNum == 53)
            {
                return 1;
            }

            return weekNum;
        }

        public static int WeekNumber(this DateTime date)
        {
            return GetWeekNumber(date);
        }

        public static DateTime ParseFromString(string dateValue)
        {
            try
            {
                return DateTime.ParseExact(dateValue, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);
            }
            catch (Exception)
            {
                try
                {
                    return DateTime.ParseExact(dateValue, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
    }
}
