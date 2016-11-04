using System.Collections.Generic;

namespace TS.Pisa
{
    internal class MasterSession : ISession, ISubscriberSession
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private List<IProviderPlugin> _providers = new List<IProviderPlugin>();
        private Dictionary<string, PisaSubscription> _subscriptions = new Dictionary<string, PisaSubscription>();

        public ISubscriber CreateSubscriber()
        {
            return new Subscriber(this);
        }

        public void Start()
        {
        }

        public void Stop()
        {
        }

        public void SetProviders(IProviderPlugin[] providersPlugin)
        {
            _providers.Clear();
            foreach (var provider in providersPlugin)
            {
                _providers.Add(provider);
            }
        }

        public void Subscribe(ISubscriber subscriber, string subject)
        {
            PisaSubscription subscription;
            if (_subscriptions.TryGetValue(subject, out subscription))
            {
                subscription = new PisaSubscription(subscriber, subject);
                _subscriptions[subject] = subscription;
            }
            SubscribeOnSuitableProviders(subscriber, subject);
        }

        private void SubscribeOnSuitableProviders(ISubscriber subscriber, string subject)
        {
        }

        public void Unsubscribe(ISubscriber subscriber, string subject)
        {
            throw new System.NotImplementedException();
        }
    }

    internal class PisaSubscription
    {
        private readonly ISubscriber _subscriber;
        private readonly string _subject;

        public PisaSubscription(ISubscriber subscriber, string subject)
        {
            _subscriber = subscriber;
            _subject = subject;
        }
    }
}