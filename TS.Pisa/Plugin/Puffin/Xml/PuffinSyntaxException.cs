using System;

namespace TS.Pisa.Plugin.Puffin.Xml
{
    /// <summary>Signals that a Xml Syntax Exception has occured.</summary>
    /// <author>Paul Sweeny</author>
    public class PuffinSyntaxException : Exception
    {
        public PuffinSyntaxException()
        {
        }

        public PuffinSyntaxException(string message)
            : base(message)
        {
        }
    }
}