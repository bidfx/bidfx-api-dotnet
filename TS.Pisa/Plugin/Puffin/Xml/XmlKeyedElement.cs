namespace TS.Pisa.Plugin.Puffin.Xml
{
    /// <summary>
    /// A XmlKeyedElement is a class that represents an XmlElement that contains
    /// nothing but additional XmlElement objects.
    /// </summary>
    /// <remarks>
    /// A XmlKeyedElement is a class that represents an XmlElement that contains
    /// nothing but additional XmlElement objects. These are indexed using a key
    /// taken either from a fixed attribute of each sub-element or from the tag
    /// name of each sub-element.
    /// </remarks>
    /// <author>Paul Sweeny</author>
    public class XmlKeyedElement : XmlElement
    {
        /// <summary>The field with Xml sub-element to use as the primary key.</summary>
        public static readonly string Key = "KEY";

        /// <summary>The field with Xml sub-element to use as the notification to delete the element.</summary>
        public static readonly string Delete = "DELETE";

        private readonly string _key;

        /// <summary>Construct a new XmlKeyedElement.</summary>
        /// <param name="tag">the tag of the element.</param>
        public XmlKeyedElement(string tag)
            : this(tag, null)
        {
        }

        /// <summary>Construct a new XmlKeyedElement.</summary>
        /// <param name="tag">the tag of the element.</param>
        /// <param name="key">the key attribute name of the sub-elemenets.</param>
        public XmlKeyedElement(string tag, string key)
            : base(tag)
        {
            _key = key;
            if (key != null)
            {
                AddAttribute(Key, key);
            }
        }

        /// <summary>Construct a new XmlKeyedElement.</summary>
        /// <param name="toCopy">and element to copy.</param>
        public XmlKeyedElement(XmlElement toCopy)
            : base(toCopy.GetTag())
        {
            var token = toCopy.GetAttributeValue(Key);
            _key = token?.GetText();
            Update(toCopy, false);
        }

        protected override XmlNestedContent CreateNestedContent()
        {
            return new XmlNestedContent(_key);
        }
    }
}