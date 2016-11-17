﻿using System;

namespace TS.Pisa
{
    public interface IProviderPlugin
    {
        string Name { get; }
        ProviderStatus ProviderStatus { get; }
        string ProviderStatusText { get; }
        EventHandler<PriceUpdateEventArgs> PriceUpdate { get; set; }
        void Subscribe(string subject);
        void Unsubscribe(string subject);
        void EventListener(IEventListener listener);
        bool IsSubjectCompatible(string subject);
        void Start();
        void Stop();
    }
}
