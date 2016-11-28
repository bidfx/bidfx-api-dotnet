using System;

namespace TS.Pisa
{
    public class ProviderPluginEventArgs : EventArgs
    {
        public IProviderPlugin Provider { get; internal set; }
        public ProviderStatus ProviderStatus { get; internal set; }
        public string ProviderStatusText { get; internal set; }

        public override string ToString()
        {
            return "provider " + Provider.Name
                   + " is " + ProviderStatus
                   + " (" + ProviderStatusText + ")";
        }
    }
}