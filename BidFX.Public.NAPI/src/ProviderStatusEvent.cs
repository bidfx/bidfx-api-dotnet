﻿using System;

namespace BidFX.Public.NAPI
{
    /// <summary>
    /// Describes a change in the status of a NAPI provider plugin component.
    /// </summary>
    public class ProviderStatusEvent : EventArgs, IProviderProperties
    {
        /// <summary>
        /// The previous provider status.
        /// </summary>
        public ProviderStatus PreviousProviderStatus { get; internal set; }

        public string Name { get; internal set; }

        public ProviderStatus ProviderStatus { get; internal set; }

        public string StatusReason { get; internal set; }

        public override string ToString()
        {
            return Name + " changed status from " + PreviousProviderStatus
                   + " to " + ProviderStatus
                   + " because: " + StatusReason;
        }
    }
}