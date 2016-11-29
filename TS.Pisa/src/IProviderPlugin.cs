namespace TS.Pisa
{
    /// <summary>
    /// Interface for market data provider plugin components that can access and normalise sources of price data.
    /// </summary>
    public interface IProviderPlugin : ISubscriber, IBackground
    {
        /// <summary>
        /// The name of the provider plugin. Each should have a unique name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The provider status.
        /// </summary>
        ProviderStatus ProviderStatus { get; }

        /// <summary>
        /// Additional descriptive text associated with the provider status.
        /// </summary>
        string ProviderStatusText { get; }

        /// <summary>
        /// The event handler used for propagating events to the users of the Pisa API.
        /// </summary>
        IPisaEventHandler PisaEventHandler { get; set; }

        /// <summary>
        /// Checks if a given subject is compatible with the provider plugin.
        /// </summary>
        /// <param name="subject">the subject to check</param>
        /// <returns>true if the subject is compatible and false otherwise</returns>
        bool IsSubjectCompatible(string subject);
    }
}