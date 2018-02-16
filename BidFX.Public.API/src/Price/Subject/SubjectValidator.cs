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
            if (s == null)
            {
                throw new IllegalSubjectException("null " + subjectPart);
            }

            int length = s.Length;
            if (length == 0)
            {
                throw new IllegalSubjectException("zero length " + subjectPart);
            }

            int mask = 1 << (int) subjectPart;

            char[] charArray = s.ToCharArray();
            foreach (char c in charArray)
            {
                if (c >= ValidChars.Length || (ValidChars[c] & mask) == 0)
                {
                    throw new IllegalSubjectException("invalid " + subjectPart + " '" + s + "'");
                }
            }
        }
    }
}