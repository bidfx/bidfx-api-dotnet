﻿using System;

namespace BidFX.Public.NAPI.PriceManager
{
    public class IllegalSubjectException : Exception
    {
        public IllegalSubjectException(string message) : base(message)
        {
        }

        public IllegalSubjectException(string message, Exception e) : base(message, e)
        {
        }

        public IllegalSubjectException()
        {
        }
    }
}