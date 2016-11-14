using System.Collections.Generic;

namespace TS.Pisa.Plugin.Puffin.Xml
{
    /// <summary>An XmlTag is a class that represents the tag name of an Xml element.</summary>
    /// <author>Paul Sweeny</author>
    public class XmlTag : XmlNameTagBase
    {
        private static readonly IDictionary<XmlToken, XmlToken> gTagTokenPool = new Dictionary<XmlToken, XmlToken>();

        /// <summary>Create a new XmlTag.</summary>
        /// <param name="name">the name of the element.</param>
        public XmlTag(string name)
            : base(name, XmlToken.StartType)
        {
        }

        public override bool Equals(object @object)
        {
            if (@object == this)
            {
                return true;
            }
            if (@object != null && @object is XmlTag)
            {
                XmlTag tag = (XmlTag) @object;
                return _token.Equals(tag._token);
            }
            return false;
        }

        /// <summary>Get the matching end-tag token given a start-tag token.</summary>
        /// <remarks>
        /// Get the matching end-tag token given a start-tag token.  The returned
        /// token is taken from a central pool of commonly used end-tag tokens.
        /// </remarks>
        /// <param name="startTag">the start-tag token to lookup with.</param>
        public static XmlToken GetEndTag(XmlToken startTag)
        {
            if (gTagTokenPool.ContainsKey(startTag))
            {
                return gTagTokenPool[startTag];
            }
            XmlToken endTag = startTag.ToEndTag();
            gTagTokenPool[startTag] = endTag;
            return endTag;
        }

        /// <summary>Get the end tag token associated with this token.</summary>
        public XmlToken GetEndTagToken()
        {
            return GetEndTag(ToToken());
        }
    }
}