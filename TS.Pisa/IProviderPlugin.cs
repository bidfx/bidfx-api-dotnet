namespace TS.Pisa
{
    public interface IProviderPlugin
    {
        string Name { get; }
        IProviderStatus ProviderStatus { get; }
        string ProviderStatusText { get; }
        void Subscribe(string subject);
        void Unsubscribe(string subject);
        void EventListener(IEventListener listener);
        bool IsSubjectCompatible(string subject);
        void Start();
        void Stop();
    }
}