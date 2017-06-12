using System;

namespace BidFX.Public.API.Price.Plugin.Puffin
{
    /// <summary>Signals that a Xml Syntax Exception has occured.</summary>
    /// <author>Paul Sweeny</author>
    internal class PuffinSyntaxException : Exception
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