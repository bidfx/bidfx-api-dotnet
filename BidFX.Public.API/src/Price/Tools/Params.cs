using System;

namespace BidFX.Public.API.Price.Tools
{
    internal static class Params
    {
        /// <summary>
        /// Checks to ensure that the given parameter is not null
        /// </summary>
        /// <param name="p1">the 1st parameter of the calling method</param>
        /// <returns>the parameter value</returns>
        /// <exception cref="ArgumentException">if the given parameter is null</exception>
        public static T NotNull<T>(T p1)
        {
            if (p1 == null) throw new ArgumentException("method parameter may not be null");
            return p1;
        }

        /// <summary>
        /// Checks to ensure that the given string parameter is not null or an empty string
        /// </summary>
        /// <param name="p1">the 1st parameter of the calling method/param>
        /// <returns>the parameter value</returns>
        /// <exception cref="ArgumentException">if the given parameter is null or an empty string</exception>
        public static string NotEmpty(string p1)
        {
            if (NotNull(p1).Equals("")) throw new ArgumentException("method parameter may be an empty string");
            return p1;
        }

        /// <summary>
        /// Checks to ensure that the given string parameter is not null or made up of only blank spaces
        /// </summary>
        /// <param name="p1">the 1st parameter of the calling method</param>
        /// <returns>the parameter value</returns>
        /// <exception cref="ArgumentException">if the given parameter is null or made up of only blank spaces</exception>
        public static string NotBlank(string p1)
        {
            if (NotNull(p1).Trim().Equals("")) throw new ArgumentException("method parameter may be a blank string");
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
    }
}