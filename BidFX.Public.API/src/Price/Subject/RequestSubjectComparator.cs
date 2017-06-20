using System;
using System.Collections.Generic;

namespace BidFX.Public.API.Price.Subject
{
    public class RequestSubjectComparator : IComparer<Subject>
    {
        private const int BothNonNull = 2;

        public int Compare(Subject x, Subject y)
        {
            var cmp = CompareObjects(x, y);
            if (cmp != BothNonNull) return cmp;
            cmp = CompareStrings(x.LookupValue("Symbol"), y.LookupValue("Symbol"));
            if (cmp != 0) return cmp;
            cmp = CompareQuantity(x.LookupValue("Quantity"), y.LookupValue("Quantity"));
            return cmp == 0 ? string.Compare(x.ToString(), y.ToString(), StringComparison.Ordinal) : cmp;
        }

        private static int CompareObjects(object o1, object o2)
        {
            if (o1 == o2) return 0;
            if (o1 == null) return -1;
            if (o2 == null) return 1;
            return BothNonNull;
        }

        private static int CompareStrings(string s1, string s2)
        {
            var cmp = CompareObjects(s1, s2);
            return cmp == BothNonNull ? string.Compare(s1, s2, StringComparison.Ordinal) : cmp;
        }

        private static int CompareQuantity(string s1, string s2)
        {
            var cmp = CompareObjects(s1, s2);
            try
            {
                var d1 = ToQuantity(s1);
                var d2 = ToQuantity(s2);
                return cmp == BothNonNull ? d1.CompareTo(d2) : cmp;
            }
            catch (FormatException e)
            {
                return cmp == BothNonNull ? s1.CompareTo(s2) : cmp;
            }
        }

        private static decimal ToQuantity(string s)
        {
            return decimal.Parse(s);
        }
    }
}