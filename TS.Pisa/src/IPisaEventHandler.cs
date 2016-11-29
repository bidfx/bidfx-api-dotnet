namespace TS.Pisa
{
    /// <summary>
    /// Interface for components that can handle Pisa events.
    /// </summary>
    public interface IPisaEventHandler
    {
        /// <summary>
        /// Handled a price update event from one of the connected price feeds.
        /// </summary>
        /// <param name="updateEvent">the update event</param>
        void OnPriceEvent(PriceUpdateEventArgs updateEvent);

        /// <summary>
        /// Handled a subscription status event from one of the connected price feeds.
        /// </summary>
        /// <param name="statusEvent">the update event</param>
        void OnStatusEvent(SubscriptionStatusEventArgs statusEvent);

        /// <summary>
        /// Handled a provider plugin status event from one of the providers.
        /// </summary>
        /// <param name="providerEvent">the update event</param>
        void OnProviderEvent(ProviderPluginEventArgs providerEvent);
    }
}