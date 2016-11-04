

namespace TS.Pisa
{
    public interface ISession
    {
        ISubscriber CreateSubscriber();
        void SetProviders(IProviderPlugin[] providersPlugin);
        void Start();
        void Stop();
    }
}