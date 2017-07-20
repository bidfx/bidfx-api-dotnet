namespace BidFX.Public.API.Price.Subject
{
    internal class SubjectValidator : IComponentHandler
    {
        private static readonly int[] ValidChars =
        {
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            4, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6,
            7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 6, 6, 6, 6, 6, 6,
            6, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
            7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 6, 6, 6, 6, 7,
            6, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
            7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 6, 6, 6, 6, 0,
        };


        public void SubjectComponent(string key, string value)
        {
            ValidatePart(key, SubjectPart.Key);
            ValidatePart(value, SubjectPart.Value);
        }

        public static void ValidatePart(string s, SubjectPart subjectPart)
        {
            if (s == null) throw new IllegalSubjectException("null " + subjectPart);
            var length = s.Length;
            if (length == 0) throw new IllegalSubjectException("zero length " + subjectPart);
            var mask = 1 << (int) subjectPart;

            var charArray = s.ToCharArray();
            foreach (var c in charArray)
            {
                if (c >= ValidChars.Length || (ValidChars[c] & mask) == 0)
                {
                    throw new IllegalSubjectException("invalid " + subjectPart + " '" + s + "'");
                }
            }
        }
    }
}