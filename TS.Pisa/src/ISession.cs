using System;
using System.Collections.Generic;

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

        /// <summary>
        /// Checks if the session is ready for handling subscriptions.
        /// </summary>
        bool Ready { get; }

        /// <summary>
        /// Waits until the session is ready with all configured plugins connected, up and ready
        /// to receive subscriptions.
        /// </summary>
        /// <param name="timeout">the time to wait before giving up.</param>
        /// <returns>true if the session is ready and false if the wait timed out</returns>
        bool WaitUntilReady(TimeSpan timeout);

        /// <summary>
        /// Gets a collection of provider properties.
        /// </summary>
        /// <returns>a collection of properties for each configured provider plugin</returns>
        ICollection<IProviderProperties> ProviderProperties();
    }
}