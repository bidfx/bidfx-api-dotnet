namespace BidFX.Public.API.Price
{
    /// <summary>
    /// Interface used by components that can subscribe to pricing using subjects.
    /// </summary>
    public interface ISubscriber
    {
        /// <summary>
        /// Subscribe to price updates for a given subject.
        /// </summary>
        /// <param name="subject">The subject to subscribe to.</param>
        /// <param name="autoRefresh">A flag that is set to true when the subscription should refresh automatically.</param>
        /// <param name="refresh">A flag that is set to true when the caller is attempting to refresh an existing subscription.</param>
        void Subscribe(Subject.Subject subject, bool autoRefresh = false, bool refresh = false);

        /// <summary>
        /// Unsubscribe to price updates for a given subject.
        /// </summary>
        /// <param name="subject">The subject to unsubscribe from.</param>
        void Unsubscribe(Subject.Subject subject);
    }
}