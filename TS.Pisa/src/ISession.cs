using System;

namespace TS.Pisa
{
    /// <summary>
    /// Interface for a Pisa API session container that provides normalised access for one or more sources
    /// of realtime market data via a set of provider plugin components.
    /// </summary>
    public interface ISession : IBackground
    {
        /// <summary>
        /// Add a provider plugin to the session
        /// </summary>
        /// <param name="providerPlugin">The provider plugin to get price updates</param>
        void AddProviderPlugin(IProviderPlugin providerPlugin);

        /// <summary>
        /// The event handler for price updates.
        /// A consumer will receive all price updates from the session when they subscribe to this.
        /// </summary>
        event EventHandler<PriceUpdateEventArgs> PriceUpdate;

        /// <summary>
        /// The event handler for status updates.
        /// A consumer will receive all price status updates from the session when they subscribe to this.
        /// </summary>
        event EventHandler<SubscriptionStatusEventArgs> PriceStatus;

        /// <summary>
        /// The event handler for provider plugin updates.
        /// </summary>
        event EventHandler<ProviderPluginEventArgs> ProviderPlugin;

        /// <summary>
        /// The time interval between attemts to refresh subscriptions that have bad statuses.
        /// </summary>
        TimeSpan SubscriptionRefreshInterval { get; set; }

    }
}