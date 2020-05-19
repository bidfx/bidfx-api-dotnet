/// Copyright (c) 2018 BidFX Systems Ltd. All Rights Reserved.

using System;
using System.Globalization;
using System.Reflection;
using Serilog;

namespace BidFX.Public.API.Price
{
    internal class ValueParser
    {
        private const NumberStyles DecimalStyle =
            NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent;

        internal static decimal? ParseDecimal(string s, decimal? defaultValue)
        {
            try
            {
                return decimal.Parse(s, DecimalStyle, CultureInfo.InvariantCulture);
            }
            catch (Exception e)
            {
                Log.Debug(e, "cannot convert {string} to decimal", s);
                return defaultValue;
            }
        }

        internal static long? ParseLong(string s, long? defaultValue)
        {
            try
            {
                return long.Parse(s, NumberStyles.AllowLeadingSign);
            }
            catch (Exception e)
            {
                Log.Debug(e, "cannot convert {string} to long", s);

                return defaultValue;
            }
        }

        internal static int? ParseInt(string s, int? defaultValue)
        {
            try
            {
                return int.Parse(s, NumberStyles.AllowLeadingSign);
            }
            catch (Exception e)
            {
                Log.Debug(e, "cannot convert {string} to int", s);
                return defaultValue;
            }
        }

        internal static decimal ParseFraction(string fraction)
        {
            try
            {
                int slash = fraction.IndexOf('/');
                if (slash == -1)
                {
                    return decimal.Parse(fraction, DecimalStyle, CultureInfo.InvariantCulture);
                }

                string s = fraction.Substring(slash + 1);
                int denominator = int.Parse(s);
                int space = fraction.IndexOf(' ');
                if (space == -1)
                {
                    return (decimal) int.Parse(fraction.Substring(0, slash)) / denominator;
                }

                int whole = Math.Abs(int.Parse(fraction.Substring(0, space)));
                int numerator = int.Parse(fraction.Substring(space + 1, slash - (space + 1)));
                decimal abs = whole + (decimal) numerator / denominator;
                return fraction[0] == '-' ? -abs : abs;
            }
            catch (Exception e)
            {
                Log.Debug(e, "cannot convert fraction {fraction} to decimal", fraction);
                return 0m;
            }
        }
    }
}