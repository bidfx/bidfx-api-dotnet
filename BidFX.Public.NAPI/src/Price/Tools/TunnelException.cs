using System;

namespace BidFX.Public.NAPI.Price.Tools
{
    internal class TunnelException : Exception
    {
        public TunnelException(string msg) :
            base(msg)
        {
        }
    }
}