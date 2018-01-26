using System;

namespace BidFX.Public.API.Price.Tools
{
    internal class JavaTime
    {
        private const long Millisecond = 1L;
        private const long Second = 1000 * Millisecond;
        private const long Minute = 60 * Second;
        private const long Hour = 60 * Minute;
        private const long Day = 24 * Hour;

        private static readonly DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private const int DaysToEpoch = 719468;

        public static long CurrentTimeMillis()
        {
            return ToJavaTime(DateTime.UtcNow);
        }

        public static long NanoTime()
        {
            return ToJavaTimeNano(DateTime.UtcNow);
        }

        public static long ToJavaTime(DateTime dateTime)
        {
            var timeSpan = (dateTime.ToUniversalTime() - Jan1st1970);
            return (long) timeSpan.TotalMilliseconds;
        }

        public static long ToJavaTimeNano(DateTime dateTime)
        {
            return (dateTime.ToUniversalTime() - Jan1st1970).Ticks;
        }

        public static DateTime ToDateTime(long javaTime)
        {
            return Jan1st1970.AddMilliseconds(javaTime);
        }

        public static string IsoDateFormat(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        }

        private static int DayToGregorianDate(int dayNumber)
        {
            var year = (int) ((10000L * dayNumber + 14780) / 3652425);
            var ddd = dayNumber - (365 * year + year / 4 - year / 100 + year / 400);
            if (ddd < 0)
            {
                --year;
                ddd = dayNumber - (365 * year + year / 4 - year / 100 + year / 400);
            }
            var mi = (100 * ddd + 52) / 3060;
            var month = (mi + 2) % 12 + 1;
            year += (mi + 2) / 12;
            var day = ddd - (mi * 306 + 5) / 10 + 1;
            return 10000 * year + 100 * month + day;
        }

        /// <summary>
        /// Converts a Java time to an ISO formatted date, in the form 'yyyMMdd', based on the Gregorian calendar and the UTC time zone.
        /// </summary>
        /// <param name="millis">time in milliseconds since 1 January 1970</param>
        /// <returns>the ISO date for the Gregorian calendar in UTC expressed as an int</returns>
        public static int ToIsoDate(long millis)
        {
            return DayToGregorianDate((int) (millis / Day) + DaysToEpoch);
        }

        /// <summary>
        /// Converts a Java time to an ISO formatted date-time, in the form 'yyyMMddHHmmssSSS', based on the Gregorian calendar and the UTC time zone.
        /// </summary>
        /// <param name="millis">time in milliseconds since 1 January 1970</param>
        /// <returns>the ISO date-time for the Gregorian calendar in UTC expressed as a long</returns>
        public static long ToIsoTime(long millis)
        {
            long date = ToIsoDate(millis);
            date *= 100L;
            date += millis / Hour % 24;
            date *= 100L;
            date += millis / Minute % 60;
            date *= 100000L;
            date += millis % Minute;
            return date;
        }
    }
}