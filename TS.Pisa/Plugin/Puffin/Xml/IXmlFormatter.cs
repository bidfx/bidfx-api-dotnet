namespace TS.Pisa.Plugin.Puffin.Xml
{
    /// <summary>
    /// An XmlFormatter is an interface for classes that will format wel-formed Xml
    /// messages.
    /// </summary>
    /// <author>Paul Sweeny</author>
    public interface IXmlFormatter
    {
        void Format(XmlElement element);
        void Format(XmlToken token);
        void Flush();
    }
}