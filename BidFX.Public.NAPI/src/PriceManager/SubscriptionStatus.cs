namespace BidFX.Public.NAPI.PriceManager
{
    /// <summary>
    /// Status codes associated with a price subscription.
    /// </summary>
    public enum SubscriptionStatus
    {
        /// <summary>
        /// The subscription is good and pricing is ticking OK.
        /// </summary>
        OK,

        /// <summary>
        /// The subscription is pending a reply from the remote price service.
        /// </summary>
        PENDING,

        /// <summary>
        /// The subscription is stale, possibly because of a temporary line down or failover.
        /// </summary>
        STALE,

        /// <summary>
        /// The subscription has been cancelled.
        /// </summary>
        CANCELLED,

        /// <summary>
        /// The subscription has been discontinued by the remote service.
        /// </summary>
        DISCONTINUED,

        /// <summary>
        /// The subscription has been rejected due to a lack of market data entitlements.
        /// </summary>
        PROHIBITED,

        /// <summary>
        /// The subscription is routed to a price source that is unavailble.
        /// </summary>
        UNAVAILABLE,

        /// <summary>
        /// The subscription has been rejected by the remote feed.
        /// </summary>
        REJECTED,

        /// <summary>
        /// The subscription has times out waiting on a response from the remote feed.
        /// </summary>
        TIMEOUT,

        /// <summary>
        /// The subscription is marked as inactive.
        /// </summary>
        INACTIVE,

        /// <summary>
        /// The subscription not updating because server-side resourced are exhausted.
        /// </summary>
        EXHAUSTED,

        /// <summary>
        /// The subscription has been closed either by an unsubscribe or a force server-side closure.
        /// </summary>
        CLOSED
    }
}