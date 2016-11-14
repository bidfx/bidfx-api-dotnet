namespace TS.Pisa
{
    internal interface ISubscriberSession
    {
        void Subscribe(string subject);
        void Unsubscribe(string subject);
    }
}
