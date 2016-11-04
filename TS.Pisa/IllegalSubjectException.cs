using System;

namespace TS.Pisa
{
    public class IllegalSubjectException : Exception
    {
        public IllegalSubjectException(string message) : base(message)
        {
        }

        public IllegalSubjectException()
        {
        }
    }
}