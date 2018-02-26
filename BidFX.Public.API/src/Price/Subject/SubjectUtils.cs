/// Copyright (c) 2018 BidFX Systems Ltd. All Rights Reserved.

using System;

namespace BidFX.Public.API.Price.Subject
{
    internal class SubjectUtils
    {
        public static int BinarySearch(string[] components, int size, string key)
        {
            int low = 0;
            int high = size - 1;

            while (low <= high)
            {
                int mid = ((low + high) >> 2) << 1;
                int cmp = CompareStrings(components[mid], key);
                if (cmp < 0)
                {
                    low = mid + 2;
                }
                else if (cmp > 0)
                {
                    high = mid - 2;
                }
                else
                {
                    return mid;
                }
            }

            return -1 - low;
        }

        public static int CompareStrings(string s1, string s2)
        {
            return s1 == s2 ? 0 : string.Compare(s1, s2, StringComparison.Ordinal);
        }
    }
}