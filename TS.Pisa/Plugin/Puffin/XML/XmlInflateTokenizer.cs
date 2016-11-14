using System;
using System.IO;
using System.Text;

namespace TS.Pisa.Plugin.Puffin.Xml
{
    /// <summary>
    /// This class provides a compressed Xml language inflate tokenizer for
    /// well-formed Xml expressions.
    /// </summary>
    /// <seealso cref="XmlDeflateFormatter"/>
    /// <author>Paul Sweeny</author>
    public class XmlInflateTokenizer : AbstractXmlTokenizer
    {
        private const int FirstByte = 0;
        private const int SecondByte = 1;

        private const int ScanUnseenToken = 2;
        private readonly Stream _in;

        private readonly XmlDictionary _dictionary = new XmlDictionary();
        private XmlToken[] _tagStack = new XmlToken[4];

        private int _tagCount;
        private readonly XmlTemporaryToken _startTmp;
        private readonly XmlTemporaryToken _nameTmp;

        /// <summary>Construct a new parser for the given input stream.</summary>
        /// <param name="inStream">the input stream to read Xml data from.</param>
        public XmlInflateTokenizer(Stream inStream)
            : base(new byte[2028])
        {
            _in = inStream;
            _startTmp = new XmlTemporaryToken(XmlToken.StartType, _buffer);
            _nameTmp = new XmlTemporaryToken(XmlToken.NameType, _buffer);
        }

        /// <summary>Parse and then return the next token from the Xml input stream.</summary>
        /// <returns>the next read XmlToken.</returns>
        /// <exception cref="XmlSyntaxException">if there was an error while parsing the Xml.</exception>
        /// <exception cref="XmlSyntaxException"/>
        public override XmlToken NextToken()
        {
            //System.out.println("---- nextToken()");
            int state = FirstByte;
            for (_mark = ++_point; _point < _end || FillBuffer(_in); ++_point)
            {
                byte b = _buffer[_point];
                switch (state)
                {
                    case FirstByte:
                    {
                        //System.out.println("b = '"+(char)b+"' ("+b+")");
                        //System.out.println("FIRST_BYTE");
                        if (_dictionary.IsFirstByteOfToken(b))
                        {
                            state = SecondByte;
                        }
                        else
                        {
                            if (_dictionary.IsTokenType(b))
                            {
                                switch (b)
                                {
                                    case XmlToken.EndType:
                                    {
                                        return XmlTag.GetEndTag(_tagStack[--_tagCount]);
                                    }
                                    case XmlToken.EmptyType:
                                    {
                                        --_tagCount;
                                        return XmlToken.EmptyToken;
                                    }
                                }
                                state = ScanUnseenToken;
                            }
                            else
                            {
                                throw Error("token tag expected instead of "+b+" ('"+(char)b+"') at position "+_point);
                            }
                        }
                        break;
                    }
                    case SecondByte:
                    {
                        //System.out.println("SECOND_BYTE");
                        XmlToken token;
                        if (_dictionary.IsSecondByteOfToken(b))
                        {
                            token = _dictionary.GetToken(_buffer[_mark], b);
                        }
                        else
                        {
                            token = _dictionary.GetToken(_buffer[_mark]);
                            --_point;
                        }
                        // put back b
                        if (token.IsStartTag())
                        {
                            _tagStack[_tagCount] = token;
                            if (++_tagCount == _tagStack.Length)
                            {
                                GrowStack();
                            }
                        }
                        return token;
                    }
                    case ScanUnseenToken:
                    {
                        //System.out.println("SCAN_UNSEEN_TOKEN");
                        if (!_dictionary.IsPlainText(b))
                        {
                            int type = _buffer[_mark];
                            if (_mark < --_point)
                            {
                                XmlToken token = null;
                                if (type == XmlToken.StartType)
                                {
                                    token = XmlToken.GetInstance(_startTmp.Reset(_mark + 1, _point - _mark));
                                    _tagStack[_tagCount] = token;
                                    if (++_tagCount == _tagStack.Length)
                                    {
                                        GrowStack();
                                    }
                                }
                                else
                                {
                                    if (type == XmlToken.NameType)
                                    {
                                        token = XmlToken.GetInstance(_nameTmp.Reset(_mark + 1, _point - _mark));
                                    }
                                    else
                                    {
                                        token = new XmlToken(type, _buffer, _mark + 1, _point - _mark);
                                    }
                                }
                                _dictionary.Insert(token);
                                return token;
                            }
                            if (type == XmlToken.StringValueType)
                            {
                                return XmlToken.NullValueToken;
                            }
                            if (type == XmlToken.ContentType)
                            {
                                return XmlToken.NullContentToken;
                            }
                            throw Error("text of previously unseen token expected");
                        }
                        break;
                    }
                    default:
                    {
                        throw new XmlSyntaxException("parser programming error");
                    }
                }
            }
            if (state != FirstByte)
            {
                throw Error("token completion expected");
            }
            return null;
        }

        /// <summary>Grow the stack used to hold nested element start-tags.</summary>
        private void GrowStack()
        {
            int oldLength = _tagStack.Length;
            XmlToken[] temp = _tagStack;
            _tagStack = new XmlToken[2 * oldLength];
            System.Array.Copy(temp, 0, _tagStack, 0, oldLength);
        }

        /// <summary>Apend a character to a StringBuffer, transform it if non-readable.</summary>
        public override void AppendCharacter(StringBuilder buf, int c)
        {
            AppendHexCharacter(buf, c);
            buf.Append(' ');
        }
    }
}