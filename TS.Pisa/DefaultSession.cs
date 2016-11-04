namespace TS.Pisa
{
    public class DefaultSession
    {
        private static readonly MasterSession MasterSession = new MasterSession();

        public static ISession GetDefault()
        {
            return MasterSession;
        }
    }
}