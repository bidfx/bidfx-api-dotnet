namespace BidFX.Public.NAPI.Price
{
    /// <summary>
    /// This class provides basic properties for a provider plugin.
    /// </summary>
    /// <remarks>
    /// It is used to access information regarding the readiness of each provider in a session.
    /// </remarks>
    public class ProviderProperties : IProviderProperties
    {
        public string Name { get; internal set; }

        public ProviderStatus ProviderStatus { get; internal set; }

        public string StatusReason { get; internal set; }

        public override string ToString()
        {
            return "price provider \"" + Name + "\" is " + ProviderStatus + " (" + StatusReason + ")";
        }
    }
}