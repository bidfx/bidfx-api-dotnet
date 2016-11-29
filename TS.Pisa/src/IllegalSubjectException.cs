﻿using System;

namespace TS.Pisa
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