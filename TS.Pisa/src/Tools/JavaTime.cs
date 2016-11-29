using System;

namespace TS.Pisa.Tools
{
    public class JavaTime
    {
        private static readonly DateTime Jan1st1970 = new DateTime
            (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long CurrentTimeMillis()
        {
            return ToJavaTime(DateTime.UtcNow);
        }

        public static long ToJavaTime(DateTime dateTime)
        {
            return (long) (dateTime.ToUniversalTime() - Jan1st1970).TotalMilliseconds;
        }
    }
}