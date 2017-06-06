using System;

namespace BidFX.Public.NAPI.PriceManager
{
    public class IllegalStateException : Exception
    {
        public IllegalStateException(string message) : base(message)
        {
        }

        public IllegalStateException(string message, Exception e) : base(message, e)
        {
        }

    }
}