using System;

namespace BidFX.Public.NAPI.Tools
{
    internal class TunnelException : Exception
    {
        public TunnelException(string msg) :
            base(msg)
        {
        }
    }
}