namespace TS.Pisa.Plugin.Puffin.Xml
{
    /// <summary>This class provides coded information for an Xml Token.</summary>
    /// <seealso cref="XmlToken"/>
    /// <author>Paul Sweeny</author>
    public class XmlTokenCode
    {
        public XmlToken _token;
        public int _code;
        public int _count;

        /// <summary>Create a new token.</summary>
        public XmlTokenCode(XmlToken token)
        {
            _token = token;
        }

        /// <summary>Get the token.</summary>
        public XmlToken GetToken()
        {
            return _token;
        }

        /// <summary>Get the code.</summary>
        public int GetCode()
        {
            return _code;
        }

        /// <summary>Create a string representation of this TokenCode (for debugging).</summary>
        public override string ToString()
        {
            return _token.ToString() + " = " + _code + " (" + _count + ')';
        }
    }
}