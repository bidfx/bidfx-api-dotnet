using System;
using System.Collections.Generic;
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
        private const int TokenCodes = 9;
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
        public int GetCode(XmlTokenCode tokenCode)
        {
            ++tokenCode._count;
            if (tokenCode._code < MaxOneByteCode) return tokenCode._code;
            if (tokenCode._count <= _winningPost) return tokenCode._code;
            var count = tokenCode._count;
            for (var i = 0; i < MaxOneByteCode; ++i)
            {
                var swap = _tokenCodes[i];
                if (count <= swap._count) continue;
                _tokenCodes[tokenCode._code] = swap;
                swap._code = tokenCode._code;
                _tokenCodes[i] = tokenCode;
                tokenCode._code = i;
                return swap._code;
            }
            // must use the old code!
            _winningPost = count;
            return tokenCode._code;
        }

        public XmlTokenCode Insert(XmlToken token)
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
            var tokenCode = new XmlTokenCode(token);
            var code = tokenCode._code = _nextCode++;
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
            var oldLength = _tokenCodes.Length;
            var temp = _tokenCodes;
            _tokenCodes = new XmlTokenCode[2 * oldLength];
            System.Array.Copy(temp, 0, _tokenCodes, 0, oldLength);
        }

        /// <summary>
        /// Purge the dictionary by stripping out those tokens with an occurrence
        /// count of less than the lower-quartile range.
        /// </summary>
        private void Purge()
        {
            var lowerQuartile = EstimateLowerQuartile();
            var to = 0;
            for (var from = 0; from < MaxCode; ++from)
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
        private int EstimateLowerQuartile()
        {
            var samples = new int[7];
            var step = MaxCode / (samples.Length + 1);
            if (step == 0)
            {
                return _tokenCodes[MaxCode / 2]._count;
            }
            var j = step - 1;
            for (var i = 0; i < samples.Length; ++i)
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
        public XmlToken GetToken(int code)
        {
            if (code < 0 || code >= _nextCode) throw new XmlSyntaxException("invalid Xml token code (" + code + ")");
            var tokenCode = _tokenCodes[code];
            if (tokenCode == null) throw new XmlSyntaxException("invalid Xml token code (" + code + ")");
            GetCode(tokenCode);
            return tokenCode._token;
        }

        /// <summary>Convert an input token byte to a token code.</summary>
        public static int ToCode(byte b)
        {
            return b & CodeMask;
        }

        /// <summary>Convert a pair of input token bytes to a token code.</summary>
        public static int ToCode(byte low, byte high)
        {
            var a = low & CodeMask;
            var b = (high - TokenCodes) << CodeBits;
            return a | b;
        }

        /// <summary>Test if the given byte is a token byte.</summary>
        public static bool IsFirstByteOfToken(byte b)
        {
            return b >= IsToken;
        }

        /// <summary>Test if the given byte is the second byte of a two-byte token.</summary>
        public static bool IsSecondByteOfToken(byte b)
        {
            return b >= TokenCodes && b < IsToken;
        }

        /// <summary>Test if the given byte is a token-type marker.</summary>
        public static bool IsTokenType(byte b)
        {
            return (b & CodeMask) < TokenCodes;
        }

        /// <summary>Test if the given byte is plain ASCII text.</summary>
        public static bool IsPlainText(byte b)
        {
            return b >= TokenCodes && b < IsToken;
        }

        /// <summary>Test the integrity of this object</summary>
        /// <exception cref="System.InvalidOperationException">if the test fails.</exception>
        public void TestIntegrity()
        {
            if (_tokenCodes.Length < _nextCode)
            {
                throw new XmlSyntaxException("_tokenCodes.Length < _nextCode");
            }
            if (MaxCode < _nextCode)
            {
                throw new XmlSyntaxException("MaxCode < _nextCode");
            }
            for (var i = 0; i < _nextCode; ++i)
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
        internal string Dump()
        {
            var buf = new StringBuilder();
            buf.Append("XmlDictionary");
            buf.Append("\n  maxCode ");
            buf.Append(MaxCode);
            buf.Append("\n  nextCode ");
            buf.Append(_nextCode);
            buf.Append("\n  winningPost ");
            buf.Append(_winningPost);
            buf.Append("\n  tokenCodes:");
            for (var i = 0; i < _nextCode; ++i)
            {
                buf.Append("\n\t");
                buf.Append(_tokenCodes[i]);
            }
            return buf.ToString();
        }
    }

    public class XmlTokenCode
    {
        internal XmlToken _token;
        internal int _code;
        internal int _count;

        internal XmlTokenCode(XmlToken token)
        {
            _token = token;
        }

        public XmlToken GetToken()
        {
            return _token;
        }

        public int GetCode()
        {
            return _code;
        }

        public int GetCount()
        {
            return _count;
        }

        public override string ToString()
        {
            return _token + " = " + _code + " (" + _count + ')';
        }
    }
}