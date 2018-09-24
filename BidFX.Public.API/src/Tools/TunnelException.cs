/// Copyright (c) 2018 BidFX Systems Ltd. All Rights Reserved.

using System;

namespace BidFX.Public.API.Price.Tools
{
    internal class TunnelException : Exception
    {
        public TunnelException(string msg) :
            base(msg)
        {
        }
    }
}