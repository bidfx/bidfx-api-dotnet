/// Copyright (c) 2018 BidFX Systems Ltd. All Rights Reserved.

using System;
using System.Collections.Generic;

namespace BidFX.Public.API.Price.Subject
{
    internal class RequestSubjectComparator : IComparer<Subject>
    {
        private const int BothNonNull = 2;

        public int Compare(Subject x, Subject y)
        {
            int cmp = CompareObjects(x, y);
            if (cmp != BothNonNull)
            {
                return cmp;
            }

            cmp = CompareStrings(x.GetComponent("Symbol"), y.GetComponent("Symbol"));
            if (cmp != 0)
            {
                return cmp;
            }

            cmp = CompareQuantity(x.GetComponent("Quantity"), y.GetComponent("Quantity"));
            return cmp == 0 ? string.Compare(x.ToString(), y.ToString(), StringComparison.Ordinal) : cmp;
        }

        private static int CompareObjects(object o1, object o2)
        {
            if (o1 == o2)
            {
                return 0;
            }

            if (o1 == null)
            {
                return -1;
            }

            if (o2 == null)
            {
                return 1;
            }

            return BothNonNull;
        }

        private static int CompareStrings(string s1, string s2)
        {
            int cmp = CompareObjects(s1, s2);
            return cmp == BothNonNull ? string.Compare(s1, s2, StringComparison.Ordinal) : cmp;
        }

        private static int CompareQuantity(string s1, string s2)
        {
            int cmp = CompareObjects(s1, s2);
            try
            {
                if (cmp == BothNonNull)
                {
                    decimal d1 = ToQuantity(s1);
                    decimal d2 = ToQuantity(s2);
                    return d1.CompareTo(d2);
                }

                return cmp;
            }
            catch (FormatException e)
            {
                return cmp == BothNonNull ? String.Compare(s1, s2, StringComparison.Ordinal) : cmp;
            }
        }

        private static decimal ToQuantity(string s)
        {
            return decimal.Parse(s);
        }
    }
}