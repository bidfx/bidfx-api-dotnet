using System;

namespace TS.Pisa.Plugin.Puffin.Xml
{
    public class XmlParseException : Exception
    {
        public XmlParseException(string msg) :
            base(msg)
        {
        }
    }
}