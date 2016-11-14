namespace TS.Pisa.Plugin.Puffin.Xml
{
    /// <summary>This class provides coded information for an Xml Token.</summary>
    /// <seealso cref="XmlToken"/>
    /// <author>Paul Sweeny</author>
    public class XmlTokenCode
    {
        internal XmlToken _token;
        internal int _code;
        internal int _count;

        public XmlTokenCode(XmlToken token)
        {
            _token = token;
        }

        public XmlToken GetToken()
        {
            return _token;
        }

        public int GetCode()
        {
            return _code;
        }

        public int GetCount()
        {
            return _count;
        }

        public override string ToString()
        {
            return _token + " = " + _code + " (" + _count + ')';
        }
    }
}