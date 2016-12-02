﻿namespace TS.Pisa
{
    /// <summary>
    /// Interface for components that can handle Pisa events.
    /// </summary>
    public interface IPisaEventHandler
    {
        /// <summary>
        /// Handled a price update event from one of the connected price feeds.
        /// </summary>
        /// <param name="subject">the subscribed subject</param>
        /// <param name="priceUpdate">the price update</param>
        /// <param name="replaceAllFields">flags if all fields should change</param>
        void OnPriceEvent(string subject, IPriceMap priceUpdate, bool replaceAllFields);

        /// <summary>
        /// Handled a subscription status event from one of the connected price feeds.
        /// </summary>
        /// <param name="subject">the subject of the updated subscription</param>
        /// <param name="status">the new status</param>
        /// <param name="reason">the reason for the status change</param>
        void OnStatusEvent(string subject, SubscriptionStatus status, string reason);

        /// <summary>
        /// Handled a provider plugin status event from one of the providers.
        /// </summary>
        /// <param name="providerPlugin">the provider that changed status</param>
        void OnProviderEvent(IProviderPlugin providerPlugin);
    }
}