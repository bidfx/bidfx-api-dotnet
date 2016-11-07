using System;

namespace TS.Pisa.Plugin.Puffin.Xml
{
    internal class XMLParseException : Exception
    {
        public XMLParseException(string msg) :
            base(msg)
        {
        }
    }
}