using System;
using System.Text.RegularExpressions;

namespace BidFX.Public.API.Price.Tools
{
    internal static class Params
    {
        /// <summary>
        /// Checks to ensure that the given parameter is not null
        /// </summary>
        /// <param name="p1">the 1st parameter of the calling method</param>
        /// <param name="errorMessage">the message to be supplied to the exception if p1 is null</param>
        /// <returns>the parameter value</returns>
        /// <exception cref="ArgumentException">with the supplied error message if the given parameter is null</exception>
        public static T NotNull<T>(T p1, string errorMessage = "method parameter may not be null")
        {
            if (p1 == null) throw new ArgumentException(errorMessage);
            return p1;
        }

        /// <summary>
        /// Checks to ensure that the given string parameter is not null or an empty string
        /// </summary>
        /// <param name="p1">the 1st parameter of the calling method</param>
        /// <returns>the parameter value</returns>
        /// <exception cref="ArgumentException">if the given parameter is null or an empty string</exception>
        public static string NotEmpty(string p1, string errorMessage = "method parameter may not be a blank sring")
        {
            if (NotNull(p1).Equals("")) throw new ArgumentException(errorMessage);
            return p1;
        }

        /// <summary>
        /// Checks to ensure that the given string parameter is not null or made up of only blank spaces
        /// </summary>
        /// <param name="p1">the 1st parameter of the calling method</param>
        /// <param name="errorMessage">the message to be supplied to the exception if p1 is blank</param>
        /// <returns>the parameter value</returns>
        /// <exception cref="ArgumentException">if the given parameter is null or made up of only blank spaces</exception>
        public static string NotBlank(string p1, string errorMessage = "method parameter may not be a blank string")
        {
            if (NotNull(p1).Trim().Equals("")) throw new ArgumentException(errorMessage);
            return p1;
        }

        /// <summary>
        /// Checks that a value is within a given range
        /// </summary>
        /// <param name="p">the parameter value to check</param>
        /// <param name="min">the minimum inclusive value of the valid range</param>
        /// <param name="max">the maximum inclusive value of the valid range</param>
        /// <returns>the parameter value</returns>
        /// <exception cref="ArgumentException">if the given parameter is not in the allowed range</exception>
        public static int InRange(int p, int min, int max)
        {
            if (p < min || p > max)
            {
                throw new ArgumentException("method parameter " + p + " not in the ranger [" + min + ", " + max + "]");
            }
            return p;
        }

        /// <summary>
        /// Checks that a value is within a given range
        /// </summary>
        /// <param name="p">the parameter value to check</param>
        /// <param name="min">the minimum inclusive value of the valid range</param>
        /// <param name="max">the maximum inclusive value of the valid range</param>
        /// <returns>the parameter value</returns>
        /// <exception cref="ArgumentException">if the given parameter is not in the allowed range</exception>
        public static long InRange(long p, long min, long max)
        {
            if (p < min || p > max)
            {
                throw new ArgumentException("method parameter " + p + " not in the ranger [" + min + ", " + max + "]");
            }
            return p;
        }

        /// <summary>
        /// Checks to ensure that the given parameter is not negative
        /// </summary>
        /// <param name="p">the parameter value to check</param>
        /// <returns>the parameter value</returns>
        /// <exception cref="ArgumentException">if the given parameter is negative</exception>
        public static int NotNegative(int p)
        {
            if (p < 0) throw new ArgumentException("method parameter may be negative: " + p);
            return p;
        }

        /// <summary>
        /// Checks to ensure that the given parameter is not negative
        /// </summary>
        /// <param name="p">the parameter value to check</param>
        /// <returns>the parameter value</returns>
        /// <exception cref="ArgumentException">if the given parameter is negative</exception>
        public static long NotNegative(long p)
        {
            if (p < 0) throw new ArgumentException("method parameter may be negative: " + p);
            return p;
        }

        /// <summary>
        /// Checks that the supplied string is the exact length when trimmed.
        /// </summary>
        /// <param name="p1"></param>
        /// <returns>p trimmed of whitespace</returns>
        public static string ExactLength(string p, int length, string errorMessage)
        {
            if (p == null) throw new ArgumentException(errorMessage);
            p = Trim(p);
            if (p.Length != length) throw new ArgumentException(errorMessage);
            return p;
        }

        /// <summary>
        /// Trim the supplied string, returning quickly the supplied string if no trimming is necessary.
        /// </summary>
        /// <param name="p">string to trim</param>
        /// <returns>p trimmed of whitespace.</returns>
        public static string Trim(string p)
        {
            if (p.Length == 0) return p;
            if (p[0] == ' ' || p[p.Length - 1] == ' ') return p.Trim();

            return p;
        }

        /// <summary>
        ///  Determine is the supplied string represents a valid integer or decimal number.
        ///  The string is numeric if it contains only digits 0-9 and (optionally) a single period.
        /// </summary>
        /// <param name="p">string to check</param>
        /// <returns>true if p is numeric, false otherwise.</returns>
        public static bool IsNumeric(string p)
        {
            return Regex.IsMatch(p, @"^\d+(\.\d+)?$");
        }

        /// <summary>
        ///  Determine if the supplied string represents a null, empty or blank string.
        /// </summary>
        /// <param name="p"></param>
        /// <returns>True if the string is null or empty, false otherwise.</returns>
        public static bool IsNullOrEmpty(string p)
        {
            return p == null || Trim(p).Length == 0;
        }
    }
}