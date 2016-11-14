namespace TS.Pisa
{
    /// <summary>
    /// This class provides a holder for the default PisaSession that allows for safe lazy initialisation of the
    /// session in a multi-threaded environment without that need for thread syncronisation.
    /// </summary>
    public class DefaultSession
    {
        private static readonly MasterSession MasterSession = new MasterSession();
        /// <summary>
        /// Gets the default PisaSession instance. Although the PisaSession is not a singleton class, most applications
        /// will only ever require a singleton instance. To ensure that all parts of an application make use of the
        /// same session instance it is recommended that they use the default session. The default session is created,
        /// as required, when first referenced, however, it is not automatically configured.
        /// </summary>
        /// <returns>The default PisaSession instance</returns>
        public static ISession GetDefault()
        {
            return MasterSession;
        }
    }
}
