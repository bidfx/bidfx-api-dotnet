

namespace TS.Pisa
{
    public interface ISession
    {
        ISubscriber CreateSubscriber();
        void AddProviderPlugin(IProviderPlugin providerPlugin);
        void Start();
        void Stop();
    }
}