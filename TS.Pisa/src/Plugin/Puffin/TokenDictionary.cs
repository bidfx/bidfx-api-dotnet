using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace TS.Pisa.Plugin.Puffin
{
    /// <summary>
    /// An XmlDictionary is a dictionary used to encode and/or decode compressed Xml messages.
    /// </summary>
    /// <author>Paul Sweeny</author>
    public class TokenDictionary
    {
        private const int TokenCodes = 9;
        private const int CodeBits = 7;
        private const int IsToken = 1 << CodeBits;
        private const int MaxOneByteCode = 1 << CodeBits;
        private const int CodeMask = MaxOneByteCode - 1;
        private const int MaxCode = (MaxOneByteCode - TokenCodes) << CodeBits;

        private readonly Dictionary<PuffinToken, XmlTokenCode> _table = new Dictionary<PuffinToken, XmlTokenCode>();
        private readonly XmlTokenCode[] _tokenCodes = new XmlTokenCode[MaxCode];
        private int _nextCode;
        private int _winningPost;

        public PuffinToken OneByteToken(byte b)
        {
            return LookupToken(OneByteCode(b));
        }

        public PuffinToken TwoByteToken(byte one, byte two)
        {
            return LookupToken(TwoByteCode(one, two));
        }

        public PuffinToken LookupToken(int code)
        {
            if (code < 0 || code >= _nextCode) throw new PuffinSyntaxException("invalid XML token code (" + code + ")");
            var tokenCode = _tokenCodes[code];
            if (tokenCode == null) throw new PuffinSyntaxException("invalid XML token code (" + code + ")");
            ++tokenCode.Count;
            if (tokenCode.Code >= MaxOneByteCode && tokenCode.Count > _winningPost)
            {
                TryPromotionToOneByteToken(tokenCode);
            }
            return tokenCode.Token;
        }

        private void TryPromotionToOneByteToken(XmlTokenCode tokenCode)
        {
            for (var i = 0; i < MaxOneByteCode; ++i)
            {
                var swap = _tokenCodes[i];
                if (tokenCode.Count > swap.Count)
                {
                    _tokenCodes[tokenCode.Code] = swap;
                    swap.Code = tokenCode.Code;
                    _tokenCodes[i] = tokenCode;
                    tokenCode.Code = i;
                    //if (Log.IsDebugEnabled) Log.Debug("promoting token "+tokenCode);
                    return;
                }
            }
            _winningPost = tokenCode.Count;
        }

        public XmlTokenCode InsertToken(PuffinToken token)
        {
            if (_nextCode >= MaxCode)
            {
                PurgeTable();
                if (_nextCode >= MaxCode) return null;
            }
            var tokenCode = new XmlTokenCode(token, _nextCode++);
            _tokenCodes[tokenCode.Code] = tokenCode;
            return tokenCode;
        }

        private void PurgeTable()
        {
            //if (Log.IsDebugEnabled) Log.Debug("purging token dictionary");
            var lowerQuartile = EstimateLowerQuartile();
            var to = 0;
            for (var from = 0; from < MaxCode; ++from)
            {
                if (_tokenCodes[from].Count > lowerQuartile)
                {
                    if (to < from)
                    {
                        _tokenCodes[to] = _tokenCodes[from];
                        _tokenCodes[to].Code = to;
                    }
                    ++to;
                }
                else
                {
                    if (_table != null)
                    {
                        _table.Remove(_tokenCodes[from].Token);
                    }
                    _tokenCodes[from] = null;
                }
            }
            _nextCode = to;
        }

        private int EstimateLowerQuartile()
        {
            var samples = new int[7];
            var step = MaxCode / (samples.Length + 1);
            if (step == 0)
            {
                return _tokenCodes[MaxCode / 2].Count;
            }
            var j = step - 1;
            for (var i = 0; i < samples.Length; ++i)
            {
                samples[i] = _tokenCodes[j].Count;
                j += step;
            }
            Array.Sort(samples);
            return samples[samples.Length / 4];
        }

        public static int OneByteCode(byte b)
        {
            return b & CodeMask;
        }

        public static int TwoByteCode(byte low, byte high)
        {
            return low & CodeMask | (high - TokenCodes) << CodeBits;
        }

        public static bool IsFirstByteOfToken(byte b)
        {
            return b >= IsToken;
        }

        public static bool IsSecondByteOfToken(byte b)
        {
            return b >= TokenCodes && b < IsToken;
        }

        public static bool IsTokenType(byte b)
        {
            return (b & CodeMask) < TokenCodes;
        }

        public static bool IsPlainText(byte b)
        {
            return b >= TokenCodes && b < IsToken;
        }

        public override string ToString()
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
        public PuffinToken Token { get; internal set; }
        public int Code { get; internal set; }
        public int Count { get; internal set; }

        public XmlTokenCode(PuffinToken token, int code)
        {
            Token = token;
            Code = code;
        }

        public override string ToString()
        {
            return Token + " = " + Code + " (" + Count + ')';
        }
    }
}