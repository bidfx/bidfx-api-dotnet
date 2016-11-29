namespace TS.Pisa
{
    /// <summary>
    /// Interface used by components that can subscribe and unsubscribe in bulk.
    /// </summary>
    public interface IBulkSubscriber : ISubscriber
    {
        /// <summary>
        /// Bulk unsubscribe from all subjects.
        /// </summary>
        void UnsubscribeAll();

        /// <summary>
        /// Bulk resubscribe to all subjects previously subscribed to.
        /// </summary>
        void ResubscribeAll();
    }
}