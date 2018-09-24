using System;

namespace BidFX.Public.API.Price.Tools
{
    public class TimeFieldTools
    {
        public static DateTime ToDateTime(decimal timeValue)
        {
            return JavaTime.ToDateTime((long) timeValue);
        }
    }
}