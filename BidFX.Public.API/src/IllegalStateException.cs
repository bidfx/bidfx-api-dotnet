/// Copyright (c) 2018 BidFX Systems LTD. All Rights Reserved.

using System;

namespace BidFX.Public.API
{
    internal class IllegalStateException : Exception
    {
        public IllegalStateException(string message) : base(message)
        {
        }

        public IllegalStateException(string message, Exception e) : base(message, e)
        {
        }
    }
}