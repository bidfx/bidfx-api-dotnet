using System;

namespace TS.Pisa.Plugin.Puffin.Xml
{
    /// <summary>Signals that a Xml Syntax Exception has occured.</summary>
    /// <author>Paul Sweeny</author>
    public class XmlSyntaxException : Exception
    {
        public XmlSyntaxException()
        {
        }

        public XmlSyntaxException(string message)
            : base(message)
        {
        }
    }
}