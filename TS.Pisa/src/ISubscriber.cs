using System;

namespace TS.Pisa
{
    public interface ISubscriber
    {
        event EventHandler<PriceUpdateEventArgs> PriceUpdate;
        event EventHandler<PriceStatusEventArgs> PriceStatus;

        void Subscribe(string subject);
        void Unsubscribe(string subject);
        void UnsubscribeAll();
        void ResubscribeAll();
        void Close();
    }
}