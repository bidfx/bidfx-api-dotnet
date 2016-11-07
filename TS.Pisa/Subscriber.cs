namespace TS.Pisa
{
    internal class Subscriber : ISubscriber
    {
        private readonly ISubscriberSession _subscriberSession;

        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        internal Subscriber(ISubscriberSession subscriberSession)
        {
            _subscriberSession = subscriberSession;
        }

        public void Subscribe(string subject)
        {
            log.Info("subscribe to " + subject);
            _subscriberSession.Subscribe(this, subject);
        }

        public void Unsubscribe(string subject)
        {
            log.Info("unsubscribe from " + subject);
            _subscriberSession.Unsubscribe(this, subject);
        }

        public void UnsubscribeAll()
        {
        }

        public void Close()
        {
        }

        public IDispatcher GetDispatcher()
        {
            return null; // TODO
        }
    }
}