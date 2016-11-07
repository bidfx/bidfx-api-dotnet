namespace TS.Pisa
{
    public interface IProviderPlugin
    {
        string Name { get; }
        ProviderStatus ProviderStatus { get; }
        string ProviderStatusText { get; }
        void Subscribe(string subject);
        void Unsubscribe(string subject);
        void EventListener(IEventListener listener);
        bool IsSubjectCompatible(string subject);
        void Start();
        void Stop();
    }
}