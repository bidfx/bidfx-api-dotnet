namespace BidFX.Public.NAPI.PriceManager
{
    /// <summary>
    /// Interface used by components that can subscribe to pricing using NAPI subjects.
    /// </summary>
    public interface ISubscriber
    {
        /// <summary>
        /// Subscribe to price updates for a given subject.
        /// </summary>
        /// <param name="subject">The subject to subscribe to.</param>
        void Subscribe(string subject);

        /// <summary>
        /// Unsubscribe to price updates for a given subject.
        /// </summary>
        /// <param name="subject">The subject to unsubscribe from.</param>
        void Unsubscribe(string subject);
    }
}