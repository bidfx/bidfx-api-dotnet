using System;

namespace BidFX.Public.NAPI.PriceManager
{
    /// <summary>
    /// Describes a status change event on a price subscription.
    /// </summary>
    public class SubscriptionStatusEvent : EventArgs
    {
        /// <summary>
        /// The subject of the subscription.
        /// </summary>
        public string Subject { get; internal set; }

        /// <summary>
        /// The new status of the subscription.
        /// </summary>
        public SubscriptionStatus SubscriptionStatus { get; internal set; }

        /// <summary>
        /// Supplementary description of the reason for the status change.
        /// </summary>
        public string StatusReason { get; internal set; }

        public override string ToString()
        {
            return Subject + " => " + SubscriptionStatus + " (" + StatusReason + ")";
        }
    }
}