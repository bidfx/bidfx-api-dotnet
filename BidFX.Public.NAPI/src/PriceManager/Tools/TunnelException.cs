using System;

namespace BidFX.Public.NAPI.PriceManager.Tools
{
    internal class TunnelException : Exception
    {
        public TunnelException(string msg) :
            base(msg)
        {
        }
    }
}