using System;

namespace TS.Pisa
{
    public interface IProviderPlugin
    {
        string Name { get; }
        ProviderStatus ProviderStatus { get; }
        string ProviderStatusText { get; }
        EventHandler<PriceUpdateEventArgs> PriceUpdate { get; set; }
        EventHandler<PriceStatusEventArgs> PriceStatus { get; set; }
        EventHandler<ProviderPluginEventArgs> ProviderPlugin { get; set; }
        void Subscribe(string subject);
        void Unsubscribe(string subject);
        bool IsSubjectCompatible(string subject);
        void Start();
        void Stop();
    }
}