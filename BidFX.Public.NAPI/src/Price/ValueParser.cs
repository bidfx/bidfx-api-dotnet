using System;
using System.Globalization;

namespace BidFX.Public.NAPI.Price
{
    public class ValueParser
    {
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const NumberStyles DecimalStyle =
            NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent;

        internal static decimal? ParseDecimal(string s, decimal? defaultValue)
        {
            try
            {
                return decimal.Parse(s, DecimalStyle, CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                if (Log.IsDebugEnabled) Log.Debug("cannot convert \"" + s + "\" to decimal");
                return defaultValue;
            }
        }

        internal static long? ParseLong(string s, long? defaultValue)
        {
            try
            {
                return long.Parse(s, NumberStyles.AllowLeadingSign);
            }
            catch (Exception)
            {
                if (Log.IsDebugEnabled) Log.Debug("cannot convert \"" + s + "\" to long");
                return defaultValue;
            }
        }

        internal static int? ParseInt(string s, int? defaultValue)
        {
            try
            {
                return int.Parse(s, NumberStyles.AllowLeadingSign);
            }
            catch (Exception)
            {
                if (Log.IsDebugEnabled) Log.Debug("cannot convert \"" + s + "\" to int");
                return defaultValue;
            }
        }

        internal static decimal ParseFraction(string fraction)
        {
            try
            {
                var slash = fraction.IndexOf('/');
                if (slash == -1)
                {
                    return decimal.Parse(fraction, DecimalStyle, CultureInfo.InvariantCulture);
                }
                var s = fraction.Substring(slash + 1);
                var denominator = int.Parse(s);
                var space = fraction.IndexOf(' ');
                if (space == -1)
                {
                    return (decimal) int.Parse(fraction.Substring(0, slash)) / denominator;
                }
                var whole = Math.Abs(int.Parse(fraction.Substring(0, space)));
                var numerator = int.Parse(fraction.Substring(space + 1, slash - (space + 1)));
                var abs = whole + (decimal) numerator / denominator;
                return fraction[0] == '-' ? -abs : abs;
            }
            catch (Exception)
            {
                if (Log.IsDebugEnabled) Log.Debug("cannot convert fraction \"" + fraction + "\" to decimal");
                return 0m;
            }
        }
    }
}