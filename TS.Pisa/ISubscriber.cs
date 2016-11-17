
using System;

namespace TS.Pisa
{
    public interface ISubscriber
    {
        event EventHandler<PriceUpdateEventArgs> PriceUpdate;
        void Subscribe(string subject);
        void Unsubscribe(string subject);
        void UnsubscribeAll();
        void ResubscribeAll();
        void Close();
    }
}
