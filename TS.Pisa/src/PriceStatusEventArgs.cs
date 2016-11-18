using System;

namespace TS.Pisa
{
    public class PriceStatusEventArgs : EventArgs
    {
        public string Subject { get; internal set; }
        public PriceStatus Status { get; internal set; }
        public string StatusText { get; internal set; }

        public override string ToString()
        {
            return Subject + " => " + Status + " (" + StatusText + ")";
        }
    }
}