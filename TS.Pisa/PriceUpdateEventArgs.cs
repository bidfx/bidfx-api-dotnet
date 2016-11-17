using System;

namespace TS.Pisa
{
    public class PriceUpdateEventArgs : EventArgs
    {
        public string Subject { get; internal set; }
        public IPriceMap PriceImage { get; internal set; }
        public IPriceMap PriceUpdate { get; internal set; }

        public override string ToString()
        {
            return Subject + " => " + PriceImage;
        }
    }
}