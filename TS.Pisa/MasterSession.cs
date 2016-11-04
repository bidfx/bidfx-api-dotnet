using System.Collections.Generic;

namespace TS.Pisa
{
    internal class MasterSession : ISession, ISubscriberSession
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly List<IProviderPlugin> _providerPlugins = new List<IProviderPlugin>();

        private readonly Dictionary<string, ISet<ISubscriber>> _subscriptions =
            new Dictionary<string, ISet<ISubscriber>>();

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

        public void AddProviderPlugin(IProviderPlugin providerPlugin)
        {
            _providerPlugins.Add(providerPlugin);
        }

        public void Subscribe(ISubscriber subscriber, string subject)
        {
            ISet<ISubscriber> subscription;
            if (!_subscriptions.TryGetValue(subject, out subscription))
            {
                subscription = new HashSet<ISubscriber>();
                _subscriptions[subject] = subscription;
            }
            subscription.Add(subscriber);
            SubscribeOnSuitableProviders(subscriber, subject);
        }

        private void SubscribeOnSuitableProviders(ISubscriber subscriber, string subject)
        {
            foreach (var providerPlugin in _providerPlugins)
            {
                if (providerPlugin.IsSubjectCompatible(subject))
                {
                    providerPlugin.Subscribe(subject);
                }
            }
        }

        public void Unsubscribe(ISubscriber subscriber, string subject)
        {
        }
    }
}