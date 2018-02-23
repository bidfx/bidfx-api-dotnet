namespace BidFX.Public.API.Price
{
    /// <summary>
    /// This enumeration represents the status of a provider plug-in module.
    /// </summary>
    /// <author>Paul Sweeny</author>
    public enum ProviderStatus
    {
        /// <summary>Indicates that the provider is available and ready for use.</summary>
        /// <remarks>
        /// NAPIClient normally subscribes to instruments via a provider only when the provider is Ready.
        /// </remarks>
        Ready,

        /// <summary>Indicates that the provider is temporarily down.</summary>
        /// <remarks>
        /// Most commonly a network connection failure results in a provider being in this state.
        /// The provider will automatically revert to an Ready status once the
        /// failure has been resolved.
        /// </remarks>
        TemporarilyDown,

        /// <summary>Indicates that the provider is unavailable due to scheduled downtime.</summary>
        /// <remarks>
        /// Some price feeds require a disconnect period at end of day or over the weekend, so they
        /// can do system maintenance, or testing on the production lines. This status result during these periods.
        /// </remarks>
        ScheduledDowntime,

        /// <summary>Indicates that the provider is unavailable.</summary>
        /// <remarks>
        /// A provider may be unavailable when the client host does not have access to the services
        /// required by the provider. Alternatively, the user may not
        /// have the correct credentials to access the resources of the provider. A
        /// provide will also be unavailable when its dependent plug-in library is not available.
        /// </remarks>
        Unavailable,

        /// <summary>Indicates that the provider is invalid.</summary>
        /// <remarks>
        /// A provider is invalid when its configuration is corrupted. Only a reconfiguration of the NAPIClient Session
        /// or the closure of the provider can result in this state changing.
        /// </remarks>
        Invalid,

        /// <summary>Indicates that the provider is closed.</summary>
        /// <remarks>
        /// This status is set when a provider is being disposed of and once set the status will never again
        /// change. Because the closure of a provider is initiated by the NAPIClient
        /// Session it is not normal for a provider to notify NAPIClient when its status
        /// changes to Closed.
        /// </remarks>
        Closed,
        
        /// <summary>Indicated that the provider recieved an unauthorized response from the server</summary>
        Unauthorized
    }
}