namespace TS.Pisa.Plugin.Puffin.Xml
{
    /// <summary>An XmlName is a class that represents the name of an Xml attribute.</summary>
    /// <author>Paul Sweeny</author>
    public class XmlName : XmlNameTagBase
    {
        /// <summary>Create a new XmlName.</summary>
        /// <param name="name">the name of the attribute.</param>
        public XmlName(string name)
            : base(name, XmlToken.NameType)
        {
        }

        /// <summary>Create a new XmlName.</summary>
        /// <param name="name">the name of the attribute.</param>
        public XmlName(XmlToken name)
            : base(name)
        {
        }

        public override bool Equals(object @object)
        {
            if (@object == this)
            {
                return true;
            }
            if (@object != null && @object is XmlName)
            {
                XmlName name = (XmlName) @object;
                return _token.Equals(name._token);
            }
            return false;
        }
    }
}