namespace TS.Pisa
{
    internal interface ISubscriberSession
    {
        /// <summary>
        /// Subscribe to price updates for a given subject.
        /// </summary>
        /// <param name="subject">The subject.</param>
        void Subscribe(string subject);
        /// <summary>
        /// Unsubscribe to price updates for a given subject.
        /// </summary>
        /// <param name="subject">The subject.</param>
        void Unsubscribe(string subject);
    }
}