using System;

namespace TS.Pisa.FI
{
    public class SubscriptionStatusEventArgs : EventArgs
    {
        /// <summary>
        /// The subject of the subscription update event.
        /// </summary>
        public FixedIncomeSubject Subject { get; internal set; }

        /// <summary>
        /// The new status status of the subscription.
        /// </summary>
        public SubscriptionStatus Status { get; internal set; }

        /// <summary>
        /// The reason for the status change.
        /// </summary>
        public string Reason { get; internal set; }

        public override string ToString()
        {
            return Subject + " => " + Status + " (" + Reason + ")";
        }
    }
}