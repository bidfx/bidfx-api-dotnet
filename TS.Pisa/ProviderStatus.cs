namespace TS.Pisa
{
    /// <summary>
    /// This enumeration represents the status of a provider plug-in module.
    /// </summary>
    /// <author>Paul Sweeny</author>
    public enum ProviderStatus
    {
        /// <summary>Indicates that the provider is available and ready for use.</summary>
        /// <remarks>
        /// Indicates that the provider is available and ready for use. Pisa
        /// normally subscribes to instruments via a provider only when the
        /// provider is Ready.
        /// </remarks>
        Ready,

        /// <summary>Indicates that the provider is unavailable due to scheduled downtime.</summary>
        /// <remarks>
        /// Indicates that the provider is unavailable due to scheduled downtime.
        /// Some exchanges require that we disconnect over the weekend, so they
        /// can do system maintenance, or stress testing on the production lines.
        /// </remarks>
        ScheduledDowntime,

        /// <summary>Indicates that the provider is temporarily down.</summary>
        /// <remarks>
        /// Indicates that the provider is temporarily down. Most commonly a
        /// network connection failure results in a provider being marked as DOWN.
        /// The provider will automatically revert to an Ready status once the
        /// failure has been resolved.
        /// </remarks>
        TemporarilyDown,

        /// <summary>Indicates that the provider is unavailable.</summary>
        /// <remarks>
        /// Indicates that the provider is unavailable. A provider may be
        /// unavailable when the client host does not have access to the services
        /// required by the provider. For example, local Bloomberg pricing is
        /// available only on a Bloomberg terminal. Alternatively, the user may not
        /// have the correct credentials to access the resources of the provider. A
        /// provide will also be unavailable when its dependent plug-in library or
        /// jar file is not available to the class loader.
        /// </remarks>
        Unavailable,

        /// <summary>Indicates that the provider is invalid.</summary>
        /// <remarks>
        /// Indicates that the provider is invalid. A provider is invalid when its
        /// configuration is corrupted. Only a reconfiguration of the Pisa Session
        /// or the closure of the provider can result in this state changing.
        /// </remarks>
        Invalid,

        /// <summary>Indicates that the provider is closed.</summary>
        /// <remarks>
        /// Indicates that the provider is closed. This status is set when a
        /// provider is being disposed of and once set the status will never again
        /// change. Because the closure of a provider is initiated by the Pisa
        /// Session it is not normal for a provider to notify Pisa when its status
        /// changes to Closed.
        /// </remarks>
        Closed
    }
}