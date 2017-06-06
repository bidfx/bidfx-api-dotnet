namespace BidFX.Public.NAPI.PriceManager
{
    /// <summary>
    /// This class provides a holder for the default NAPIClient session.
    /// Most application will share a single NAPIClient session and get access to it from here.
    /// </summary>
    public class DefaultSession
    {
        private static readonly MasterSession MasterSession = new MasterSession();

        /// <summary>
        /// The default session instance used for configuring the NAPIClient pricing API.
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