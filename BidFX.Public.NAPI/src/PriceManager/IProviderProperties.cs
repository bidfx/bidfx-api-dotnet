namespace BidFX.Public.NAPI.PriceManager
{
    /// <summary>
    /// This interface defines basic properties for a provider plugin.
    /// </summary>
    /// <remarks>
    /// It is used to access information regarding the readiness of each provider in a session.
    /// </remarks>
    public interface IProviderProperties
    {
        /// <summary>
        /// The name of the provider plugin. Each should have a unique name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The current provider status.
        /// </summary>
        ProviderStatus ProviderStatus { get; }

        /// <summary>
        /// Additional descriptive text associated with the provider status giving the reason for the status.
        /// </summary>
        string StatusReason { get; }
    }
}