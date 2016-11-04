namespace TS.Pisa
{
    internal interface ISubscriberSession
    {
        void Subscribe(ISubscriber subscriber, string subject);
        void Unsubscribe(ISubscriber subscriber, string subject);
    }
}