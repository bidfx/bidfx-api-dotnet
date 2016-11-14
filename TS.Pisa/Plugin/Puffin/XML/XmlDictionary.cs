using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TS.Pisa.Plugin.Puffin.Xml
{
    /// <summary>
    /// An XmlDictionary is a dictionary used to encode and/or decode compressed
    /// Xml messages.
    /// </summary>
    /// <author>Paul Sweeny</author>
    public class XmlDictionary
    {
        private const int TokenCodes = XmlToken.MaxType;
        private const int CodeBits = 7;

        private const int IsToken = 1 << CodeBits;
        private const int MaxOneByteCode = 1 << CodeBits;

        private const int CodeMask = MaxOneByteCode - 1;
        private const int MaxCode = (MaxOneByteCode - TokenCodes) << CodeBits;

        private readonly Dictionary<XmlToken, XmlTokenCode> _table = new Dictionary<XmlToken, XmlTokenCode>();
        private int _nextCode;
        private int _winningPost;

        private XmlTokenCode[] _tokenCodes = new XmlTokenCode[MaxOneByteCode];

        /// <summary>Get the Xml token with the given single byte code.</summary>
        /// <remarks>
        /// Get the Xml token with the given single byte code.  This method will
        /// increment the occurrence count of the token; since this count can be
        /// used to optimise the dictionary, be careful only to call this method
        /// once per token code received.
        /// </remarks>
        /// <param name="b">the byte code.</param>
        /// <returns>the token.</returns>
        /// <exception cref="XmlSyntaxException">
        /// if the given code could not be converted to
        /// a token.
        /// </exception>
        /// <exception cref="XmlSyntaxException"/>
        public XmlToken GetToken(byte b)
        {
            return GetToken(ToCode(b));
        }

        /// <summary>Get the Xml token with the given double byte code.</summary>
        /// <remarks>
        /// Get the Xml token with the given double byte code.  This method will
        /// increment the occurrence count of the token; since this count can be
        /// used to optimise the dictionary, be careful only to call this method
        /// once per token code received.
        /// </remarks>
        /// <param name="one">the first byte of the code.</param>
        /// <param name="two">the second byte of the code.</param>
        /// <returns>the token.</returns>
        /// <exception cref="XmlSyntaxException">
        /// if the given code could not be converted to
        /// a token.
        /// </exception>
        /// <exception cref="XmlSyntaxException"/>
        public XmlToken GetToken(byte one, byte two)
        {
            return GetToken(ToCode(one, two));
        }

        /// <summary>Get the optimal code for a given token code.</summary>
        /// <param name="tokenCode">the Xml token code to find.</param>
        /// <returns>the corresponding optimalCode for the tokenCode.</returns>
        public virtual int GetCode(XmlTokenCode tokenCode)
        {
            ++tokenCode._count;
            if (tokenCode._code >= MaxOneByteCode)
            {
                if (tokenCode._count > _winningPost)
                {
                    int count = tokenCode._count;
                    for (int i = 0; i < MaxOneByteCode; ++i)
                    {
                        XmlTokenCode swap = _tokenCodes[i];
                        if (count > swap._count)
                        {
                            _tokenCodes[tokenCode._code] = swap;
                            swap._code = tokenCode._code;
                            _tokenCodes[i] = tokenCode;
                            tokenCode._code = i;
                            return swap._code;
                        }
                    }
                    // must use the old code!
                    _winningPost = count;
                }
            }
            return tokenCode._code;
        }

        /// <summary>Insert a new token into the dictionary.</summary>
        /// <remarks>
        /// Insert a new token into the dictionary.  This method should be used to
        /// insert previously unseen tokens as they are received from an
        /// Stream.  However it may also be used to pre-load token codes into
        /// a dictionary before any compressed Xml communication takes place.  If
        /// you do this then take care to pre-load the same tokens, in the same
        /// order, on both sides of the communication.
        /// </remarks>
        /// <param name="token">the Xml token to insert.</param>
        /// <returns>the inserted token code or null if the table was full.</returns>
        public virtual XmlTokenCode Insert(XmlToken token)
        {
            XmlTokenCode tokenCode = null;
            if (TokenSpaceAvailable())
            {
                tokenCode = AddToken(token);
            }
            else
            {
                Purge();
                if (TokenSpaceAvailable())
                {
                    tokenCode = AddToken(token);
                }
            }
            return tokenCode;
        }

        /// <summary>Check if the dictionary has free space for more token-codes.</summary>
        private bool TokenSpaceAvailable()
        {
            return _nextCode < MaxCode;
        }

        /// <summary>Add a token to the dictionary and return its token-code.</summary>
        private XmlTokenCode AddToken(XmlToken token)
        {
            XmlTokenCode tokenCode = new XmlTokenCode(token);
            int code = tokenCode._code = _nextCode++;
            if (_nextCode > _tokenCodes.Length)
            {
                Extend();
            }
            _tokenCodes[code] = tokenCode;
            return tokenCode;
        }

        /// <summary>Extend the capacity of the internal token dictionary.</summary>
        private void Extend()
        {
            int oldLength = _tokenCodes.Length;
            XmlTokenCode[] temp = _tokenCodes;
            _tokenCodes = new XmlTokenCode[2 * oldLength];
            System.Array.Copy(temp, 0, _tokenCodes, 0, oldLength);
        }

        /// <summary>
        /// Purge the dictionary by stripping out those tokens with an occurrence
        /// count of less than the lower-quartile range.
        /// </summary>
        private void Purge()
        {
            int lowerQuartile = EstimateLowerQuartile();
            int to = 0;
            for (int from = 0; from < MaxCode; ++from)
            {
                if (_tokenCodes[from]._count > lowerQuartile)
                {
                    if (to < from)
                    {
                        _tokenCodes[to] = _tokenCodes[from];
                        _tokenCodes[to]._code = to;
                    }
                    ++to;
                }
                else
                {
                    _table?.Remove(_tokenCodes[from]._token);
                    _tokenCodes[from] = null;
                }
            }
            _nextCode = to;
        }

        /// <summary>
        /// Estimate the lower quartile range of tokens, in terms of occurrence
        /// rate, by means of a small sample.
        /// </summary>
        public virtual int EstimateLowerQuartile()
        {
            int[] samples = new int[7];
            int step = MaxCode / (samples.Length + 1);
            if (step == 0)
            {
                return _tokenCodes[MaxCode / 2]._count;
            }
            int j = step - 1;
            for (int i = 0; i < samples.Length; ++i)
            {
                samples[i] = _tokenCodes[j]._count;
                j += step;
            }
            Array.Sort(samples);
            return samples[samples.Length / 4];
        }

        /// <summary>Get the Xml token with the given code.</summary>
        /// <remarks>
        /// Get the Xml token with the given code.  This method will increment the
        /// occurrence count of the token; since this count can be used to optimise
        /// the dictionary, be careful only to call this method once per token code
        /// received.
        /// </remarks>
        /// <param name="code">the code to fetch the token of.</param>
        /// <returns>the token.</returns>
        /// <exception cref="XmlSyntaxException">
        /// if the given code could not be converted to
        /// a token.
        /// </exception>
        /// <exception cref="XmlSyntaxException"/>
        public virtual XmlToken GetToken(int code)
        {
            if (code >= 0 && code < _nextCode)
            {
                XmlTokenCode tokenCode = _tokenCodes[code];
                if (tokenCode != null)
                {
                    GetCode(tokenCode);
                    return tokenCode._token;
                }
            }
            throw new XmlSyntaxException("invalid Xml token code (" + code + ")");
        }

        /// <summary>Write a token code to a stream.</summary>
        /// <param name="tokenCode">the Xml token code to output.</param>
        /// <param name="stream">the stream to write to.</param>
        /// <exception cref="System.IO.IOException"/>
        public virtual void Write(XmlTokenCode tokenCode, Stream stream)
        {
            int code = GetCode(tokenCode);
            if (code < MaxOneByteCode)
            {
                stream.WriteByte((byte) (IsToken | code));
            }
            else
            {
                stream.WriteByte((byte) (IsToken | (code & CodeMask)));
                // low byte
                stream.WriteByte((byte) (((int) (((int) code) >> CodeBits)) + TokenCodes));
            }
        }

        // high byte
        /// <summary>Convert an input token byte to a token code.</summary>
        public virtual int ToCode(byte b)
        {
            return b & CodeMask;
        }

        /// <summary>Convert a pair of input token bytes to a token code.</summary>
        public virtual int ToCode(byte low, byte high)
        {
            var a = low & CodeMask;
            var b = (high - TokenCodes) << CodeBits;
            return a | b;
        }

        /// <summary>Test if the given byte is a token byte.</summary>
        public virtual bool IsFirstByteOfToken(byte b)
        {
            return b >= IsToken;
        }

        /// <summary>Test if the given byte is the second byte of a two-byte token.</summary>
        public virtual bool IsSecondByteOfToken(byte b)
        {
            return b >= TokenCodes && b < IsToken;
        }

        /// <summary>Test if the given byte is a token-type marker.</summary>
        public virtual bool IsTokenType(byte b)
        {
            return (b & CodeMask) < TokenCodes;
        }

        /// <summary>Test if the given byte is plain ASCII text.</summary>
        public virtual bool IsPlainText(byte b)
        {
            return b >= TokenCodes;
        }

        /// <summary>Test the integrity of this object</summary>
        /// <exception cref="System.InvalidOperationException">if the test fails.</exception>
        public virtual void TestIntegrity()
        {
            if (_tokenCodes.Length < _nextCode)
            {
                throw new XmlSyntaxException("mTokenCodes.length < _nextCode");
            }
            if (MaxCode < _nextCode)
            {
                throw new XmlSyntaxException("mMaxCode < _nextCode");
            }
            for (int i = 0; i < _nextCode; ++i)
            {
                if (_tokenCodes[i] == null)
                {
                    throw new XmlSyntaxException("null token-code at " + i);
                }
            }
        }

        /// <summary>Convert to a string for debug purposes.</summary>
        public override string ToString()
        {
            return Dump();
        }

        /// <summary>Dump the contents to a string for debug purposes.</summary>
        public virtual string Dump()
        {
            StringBuilder buf = new StringBuilder();
            buf.Append("XmlDictionary");
            buf.Append("\n  _maxCode ");
            buf.Append(MaxCode);
            buf.Append("\n  _nextCode ");
            buf.Append(_nextCode);
            buf.Append("\n  _winningPost ");
            buf.Append(_winningPost);
            buf.Append("\n  _tokenCodes:");
            for (int i = 0; i < _nextCode; ++i)
            {
                buf.Append("\n\t");
                buf.Append(_tokenCodes[i]);
            }
            return buf.ToString();
        }
    }
}