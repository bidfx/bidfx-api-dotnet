using System;
using System.Text.RegularExpressions;

namespace BidFX.Public.API.Price.Tools
{
    internal static class DateFormatter
    {
        private static readonly Regex FullDateRegex =
            new Regex(@"^\s*(\d\d\d\d)(?:(?:-(1[0-2]|0?[1-9])(?:-([1-2]\d|3[0-1]|0?[1-9]))?)|(?:(1[0-2]|0?[1-9])(3[0-1]|0[1-9]|[1-2]\d)?))\s*$", RegexOptions.Compiled);

        public static string FormatDateWithHyphens(string fieldName, string date, bool requireDay)
        {
            Match match = FullDateRegex.Match(date);
            if (!match.Success)
            {
                throw new ArgumentException(fieldName + " was not in valid format " +
                                            (requireDay ? "(YYYY-MM-DD)" : "(YYYY-MM)") +
                                            ": " + date);
            }

            GroupCollection groups = match.Groups;
            string year = groups[1].ToString();

            string month = groups[2].ToString();
            if ("".Equals(month))
            {
                month = groups[4].ToString();
            }

            string day = groups[3].ToString();
            if ("".Equals(day))
            {
                day = groups[5].ToString();
            }

            if (requireDay && "".Equals(day))
            {
                throw new ArgumentException(fieldName + " was not in valid format (YYYY-MM-DD): " + date);
            }

            return year + "-"
                        + (month.Length == 2 ? month : "0" + month)
                        + (!"".Equals(day) ? "-" + (day.Length == 2 ? day : "0" + day) : "");
        }
    }
}