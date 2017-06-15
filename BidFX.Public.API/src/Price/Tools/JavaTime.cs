using System;

namespace BidFX.Public.API.Price.Tools
{
    public class JavaTime
    {
        private static readonly DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

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
            return (long) (dateTime.ToUniversalTime() - Jan1st1970).TotalMilliseconds;
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
    }
}