namespace TS.Pisa
{
    public interface ISession : ISubscriber
    {
        /// <summary>
        /// Add a provider plugin to the session
        /// </summary>
        /// <param name="providerPlugin">The provider plugin to get price updates</param>
        void AddProviderPlugin(IProviderPlugin providerPlugin);
        /// <summary>
        /// Starts the session.
        /// </summary>
        void Start();
        /// <summary>
        /// Stops the session.
        /// </summary>
        void Stop();
    }
}