using System;

namespace TS.Pisa.FI
{
    /// <summary>
    /// Describes a single status on a fixed income price subscription
    /// </summary>
    public class FiSubscriptionStatusEvent : EventArgs
    {
        /// <summary>
        /// The subject of the subscription.
        /// </summary>
        public FixedIncomeSubject Subject { get; internal set; }

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