using System.Collections.Generic;

namespace TS.Pisa.Plugin.Puffin.Xml
{
    /// <summary>A XmlContentContainer is an interface for classes that contain XmlContent.</summary>
    /// <author>Paul Sweeny</author>
    public interface IXmlContentContainer
    {
        XmlElement GetElement(string key);
        XmlElement GetElement(object key);
        XmlElement GetFirstElement();
        ICollection<string> GetKeys();
        ICollection<XmlElement> GetContent();
    }
}