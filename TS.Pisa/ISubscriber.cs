using System.Collections.Generic;

namespace TS.Pisa
{
    public interface ISubscriber
    {
        void Subscribe(string subject);
        void Unsubscribe(string subject);
        void UnsubscribeAll();
        void Close();
        IList<string> GetSubscribedSubjects();
        IDispatcher GetDispatcher();
    }
}