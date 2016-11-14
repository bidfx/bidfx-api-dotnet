namespace TS.Pisa.Plugin.Puffin.Xml
{
    /// <summary>
    /// A XmlFormatable provides a common interface for classes that are formatable
    /// via an XmlFormatter.
    /// </summary>
    /// <author>Paul Sweeny</author>
    public interface IXmlFormatable
    {
        /// <summary>Format this object using the given formatter.</summary>
        /// <param name="formatter">the formatter to use to format this object.</param>
        /// <exception cref="System.IO.IOException">if formatting fails due to an I/O error.</exception>
        /// <exception cref="XmlSyntaxException">if formatting fails due to invalid Xml.</exception>
        /// <exception cref="XmlSyntaxException"/>
        void FormatUsing(IXmlFormatter formatter);
    }
}