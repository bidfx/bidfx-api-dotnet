namespace TS.Pisa.Plugin.Puffin.Xml
{
    /// <summary>This interface is implemented by classes that can tokenize well-formed Xml expressions.</summary>
    /// <author>Paul Sweeny</author>
    public interface IXmlTokenizer
    {
        /// <summary>Parses and returns the next Xml token.</summary>
        /// <returns>the next read token, or null when there are no more.</returns>
        /// <exception cref="XmlSyntaxException">if there was an error while parsing the Xml.</exception>
        /// <exception cref="XmlSyntaxException"/>
        XmlToken NextToken();

        /// <summary>Parses and returns the next Xml element.</summary>
        /// <returns>the next read XmlElement, or null when there are no more.</returns>
        /// <exception cref="XmlSyntaxException">if there was an error while parsing the Xml.</exception>
        /// <exception cref="XmlSyntaxException"/>
        XmlElement NextElement();
    }
}