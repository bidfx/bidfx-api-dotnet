using System;

namespace TS.Pisa.FI
{
    /// <summary>
    /// Describes a change in the status of a provider plugin component.
    /// </summary>
    public class ProviderPluginEventArgs : EventArgs
    {
        /// <summary>
        /// The new provider status.
        /// </summary>
        public ProviderStatus ProviderStatus { get; internal set; }

        /// <summary>
        /// Supplementary description of the reason for the status change.
        /// </summary>
        public string Reason { get; internal set; }

        public override string ToString()
        {
            return "provider is " + ProviderStatus
                   + " (" + Reason + ")";
        }
    }
}