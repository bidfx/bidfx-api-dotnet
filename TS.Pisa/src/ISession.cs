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
        /// <remarks>
        /// The session is ready when it is running and all of its provider plugins are ready/
        /// </remarks>
        bool Ready { get; }

        /// <summary>
        /// Waits until the session is ready with all configured plugins connected, up and ready
        /// to receive subscriptions.
        /// </summary>
        /// <remarks>
        /// It is not a requirement that a client wait for the session to become ready. It is fine just to start the
        /// session and let it connect to and use the various provide plugins as they become available; the session
        /// will re-subscribe automatically to compatible providers as they become ready. Waiting for all
        /// providers to become ready however is useful in some applications, particularly those that submit
        /// orders based on market data provided by the API.
        /// </remarks>
        /// <param name="timeout"></param>
        /// <returns>true if the session is ready and false if the wait timed out</returns>
        bool WaitUntilReady(TimeSpan timeout);

        /// <summary>
        /// Gets a collection of provider properties.
        /// </summary>
        /// <returns>a collection of properties for each configured provider plugin</returns>
        ICollection<IProviderProperties> ProviderProperties();
    }
}