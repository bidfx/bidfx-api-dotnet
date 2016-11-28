using System;

namespace TS.Pisa
{
    public interface ISubscriber
    {
        /// <summary>
        /// The event handler for price updates.
        /// A consumer will receive all price updates from the session when they subscribe to this.
        /// </summary>
        event EventHandler<PriceUpdateEventArgs> PriceUpdate;

        /// <summary>
        /// The event handler for status updates.
        /// A consumer will receive all price status updates from the session when they subscribe to this.
        /// </summary>
        event EventHandler<PriceStatusEventArgs> PriceStatus;

        /// <summary>
        /// The event handler for provider plugin updates.
        /// </summary>
        event EventHandler<ProviderPluginEventArgs> ProviderPlugin;

        /// <summary>
        /// Subscribe to price updates for a given subject.
        /// </summary>
        /// <param name="subject">The subject.</param>
        void Subscribe(string subject);

        /// <summary>
        /// Unsubscribe to prices updates for a given subject.
        /// </summary>
        /// <param name="subject">The subject.</param>
        void Unsubscribe(string subject);

        /// <summary>
        /// Unsubscribe from price updates from all subjects.
        /// </summary>
        void UnsubscribeAll();

        /// <summary>
        /// Resubscribe to price updates from all subjects.
        /// </summary>
        void ResubscribeAll();

        /// <summary>
        /// Close the session down and stop receiving price updates.
        /// </summary>
        void Close();
    }
}