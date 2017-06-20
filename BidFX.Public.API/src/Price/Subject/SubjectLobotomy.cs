namespace BidFX.Public.API.Price.Subject
{
    public class SubjectLobotomy
    {
        public static Subject CreateUnsafeSubject(string[] components)
        {
            return new Subject(components);
        }

        public static Subject CreateUnsafeInternalisedSubject(string[] components)
        {
            InternaliseComponents(components);
            return new Subject(components);
        }

        public static string[] ExtractComponents(Subject subject)
        {
            return subject.InternalComponents();
        }

        public static void InternaliseComponents(string[] components)
        {
            var i = 0;
            while (i < components.Length)
            {
                var altKey = CommonComponents.CommonKey(components[i]);
                if (altKey != null)
                {
                    components[i] = altKey;
                }
                i++;

                var altValue = CommonComponents.CommonValue(components[i]);
                if (altValue != null)
                {
                    components[i] = altValue;
                }
                i++;
            }
        }
    }
}