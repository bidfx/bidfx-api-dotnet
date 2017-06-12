using System;
using System.Collections.Generic;

namespace BidFX.Public.API.Price
{
    /// <summary>
    /// Interface for a NAPIClient session container that provides normalised access for one or more sources
    /// of realtime market data via a set of provider plugin components.
    /// </summary>
    public interface ISession : IBackground
    {
        /// <summary>
        /// The event handler for price updates.
        /// A consumer will receive all price updates from the session when they subscribe to this.
        /// </summary>
        event EventHandler<PriceUpdateEvent> PriceUpdateEventHandler;

        /// <summary>
        /// The event handler for subscription status updates.
        /// A consumer will receive all price status updates from the session when they subscribe to this.
        /// </summary>
        event EventHandler<SubscriptionStatusEvent> SubscriptionStatusEventHandler;

        /// <summary>
        /// The event handler for provider plugin status updates.
        /// </summary>
        event EventHandler<ProviderStatusEvent> ProviderStatusEventHandler;

        /// <summary>
        /// The time interval between attemts to refresh subscriptions that have irrecoverable statuses.
        /// </summary>
        /// <remarks>
        /// A status is deemed to be irrecoverable when the price service cannot cannot create
        /// or maintain a subscription for it.  For example an invalid subject is irrecoverable.
        /// However even irrecoverable status types can be recovered over time by making price service
        /// changes and resubscribing. For this reason the API will periodically resubscribe to them until
        /// the user does calls Unsubscribe.
        /// </remarks>
        TimeSpan SubscriptionRefreshInterval { get; set; }

        /// <summary>
        /// Checks if the session is ready for handling subscriptions.
        /// </summary>
        /// <remarks>
        /// The session is ready when it is running and all of its provider plugins are ready.
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
        /// <remarks>
        /// Provider properties give access to the name, status
        /// </remarks>
        /// <returns>S collection of properties for each configured provider plugin.</returns>
        ICollection<IProviderProperties> ProviderProperties();
    }
}