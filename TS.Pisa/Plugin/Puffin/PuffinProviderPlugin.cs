
namespace TS.Pisa.Plugin.Puffin
{
    public class PuffinProviderPlugin : IProviderPlugin
    {
        public bool UseTunnel { get; set; } = false;
        public string Host { get; set; } = "localhost";
        public int Port { get; set; } = 9901;
        public string Name { get; }
        public IProviderStatus ProviderStatus { get; }
        public string ProviderStatusText { get; }

        public void Subscribe(string subject)
        {
            throw new System.NotImplementedException();
        }

        public void Unsubscribe(string subject)
        {
            throw new System.NotImplementedException();
        }

        public void EventListener(IEventListener listener)
        {
            throw new System.NotImplementedException();
        }

        public bool IsSubjectCompatible(string subject)
        {
            return true;
        }

        public void Start()
        {
            throw new System.NotImplementedException();
        }

        public void Stop()
        {
            throw new System.NotImplementedException();
        }
    }
}