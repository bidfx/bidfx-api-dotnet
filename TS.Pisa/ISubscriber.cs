
namespace TS.Pisa
{
    public interface ISubscriber
    {
        void Subscribe(string subject);
        void Unsubscribe(string subject);
        void UnsubscribeAll();
        void Close();
        IDispatcher GetDispatcher();
    }
}