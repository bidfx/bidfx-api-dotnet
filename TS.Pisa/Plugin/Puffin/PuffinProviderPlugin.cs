namespace TS.Pisa.Plugin.Puffin
{
    public class PuffinProviderPlugin : IProviderPlugin
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public bool UseTunnel { get; set; } = false;
        public string Host { get; set; } = "localhost";
        public int Port { get; set; } = 9901;
        public string Name { get; }
        public IProviderStatus ProviderStatus { get; }
        public string ProviderStatusText { get; }

        public void Subscribe(string subject)
        {
            log.Info("subscribing to " + subject);
        }

        public void Unsubscribe(string subject)
        {
            log.Info("unsubscribing from " + subject);
        }

        public void EventListener(IEventListener listener)
        {
        }

        public bool IsSubjectCompatible(string subject)
        {
            return true;
        }

        public void Start()
        {
        }

        public void Stop()
        {
        }
    }
}