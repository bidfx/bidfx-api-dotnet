namespace TS.Pisa
{
    /// <summary>
    /// This class provides a holder for the default Pisa session.
    /// Most application will share a single Pisa session and get access to it from here.
    /// </summary>
    public class DefaultSession
    {
        private static readonly MasterSession MasterSession = new MasterSession();

        /// <summary>
        /// The default session instance used for configuring the Pisa pricing API.
        /// </summary>
        public static ISession Session
        {
            get { return MasterSession; }
        }

        /// <summary>
        /// The default subscriber instance used for subscribing to prices.
        /// </summary>
        public static IBulkSubscriber Subscriber
        {
            get { return MasterSession; }
        }
    }
}