using System;

namespace TS.Pisa.Plugin.Puffin
{
    /// <summary>Signals that a Xml Syntax Exception has occured.</summary>
    /// <author>Paul Sweeny</author>
    public class PuffinSyntaxException : Exception
    {
        public PuffinSyntaxException(string message)
            : base(message)
        {
        }

        public PuffinSyntaxException(string message, Exception e)
            : base(message, e)
        {
        }
    }
}