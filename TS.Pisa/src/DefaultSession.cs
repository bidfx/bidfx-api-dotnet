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
        /// Gets the default session instance used for configuring the pricing API.
        /// </summary>
        /// <returns>The default PisaSession instance</returns>
        public static ISession GetSession()
        {
            return MasterSession;
        }

        /// <summary>
        /// Gets the default subscriber instance used for subscribing to prices.
        /// </summary>
        /// <returns>The default PisaSession instance</returns>
        public static IBulkSubscriber GetSubscriber()
        {
            return MasterSession;
        }
    }
}