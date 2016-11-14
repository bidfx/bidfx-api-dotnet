
namespace TS.Pisa
{
    public interface ISession : ISubscriber
    {
        void AddProviderPlugin(IProviderPlugin providerPlugin);
        void Start();
        void Stop();
    }
}
