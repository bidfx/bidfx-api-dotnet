using System;

namespace TS.Pisa
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