using System;

namespace TS.Pisa
{
    /// <summary>
    /// Described a status change on a subscription.
    /// </summary>
    public class SubscriptionStatusEventArgs : EventArgs
    {
        /// <summary>
        /// The subject of the subscription.
        /// </summary>
        public string Subject { get; internal set; }

        /// <summary>
        /// The new subscription status.
        /// </summary>
        public SubscriptionStatus SubscriptionStatus { get; internal set; }

        /// <summary>
        /// Supplementary description of the reason for the status change.
        /// </summary>
        public string Reason { get; internal set; }

        public override string ToString()
        {
            return Subject + " => " + SubscriptionStatus + " (" + Reason + ")";
        }
    }
}