using System.Text;
using System.Threading;

namespace BidFX.Public.API.Price.Tools
{
    public class NumericCharacterEntity
    {
        private static readonly ThreadLocal<StringBuilder> ThreadLocalBuilder = new ThreadLocal<StringBuilder>(() => new StringBuilder());
        
        private readonly char[][] _encodingTable = new char[255][];

        public NumericCharacterEntity()
        {
            AddCharacterEncoding('&');
        }

        public void AddCharacterEncoding(char c)
        {
            _encodingTable[c] = CharacterEntity(c);
        }

        public void AddCharacterEncodingRange(int from, int to)
        {
            for (var i = from; i <= to; i++)
            {
                AddCharacterEncoding((char) i);
            }
        }

        public string EncodeString(string s)
        {
            if (s == null) return null;
            var charArray = s.ToCharArray();
            for (var i = 0; i < charArray.Length; i++)
            {
                var c = charArray[i];
                var encoded = c < _encodingTable.Length ? _encodingTable[c] : CharacterEntity(c);
                if (encoded != null)
                {
                    return EncodeRemaining(s, i, encoded);
                }
            }
            return s;
        }

        public string EncodeRemaining(string s, int start, char[] encoded)
        {
            var builder = ThreadLocalBuilder.Value;
            builder.Length = 0;
            builder.Append(s.ToCharArray(0, start));
            builder.Append(encoded);

            var charArray = s.ToCharArray();
            for (var i = start + 1; i < charArray.Length; i++)
            {
                var c = charArray[i];
                encoded = c < _encodingTable.Length ? _encodingTable[c] : CharacterEntity(c);
                if (encoded == null)
                {
                    builder.Append(c);
                }
                else
                {
                    builder.Append(encoded);
                }
            }
            return builder.ToString();
        }

        private static char[] CharacterEntity(char c)
        {
            return ("&#" + (int) c + ";").ToCharArray();
        }

        public string DecodeString(string s)
        {
            var charArray = s.ToCharArray();
            for (var i = 0; i < charArray.Length; i++)
            {
                var c = charArray[i];
                if (c == '&' && i < charArray.Length - 3)
                {
                    return DecodeRemaining(s, i);
                }
            }
            return s;
        }

        private static string DecodeRemaining(string s, int start)
        {
            var builder = ThreadLocalBuilder.Value;
            builder.Length = 0;
            builder.Append(s, 0, start);

            var charArray = s.ToCharArray();
            for (var i = start; i < charArray.Length; i++)
            {
                var c = charArray[i];
                if (c == '&' && i < charArray.Length - 3)
                {
                    i = NextDecodedChar(builder, charArray, i);
                }
                else
                {
                    builder.Append(c);
                }
            }

            return builder.ToString();
        }

        private static int NextDecodedChar(StringBuilder builder, char[] s, int start)
        {
            var i = start + 1;
            var c = s[i++];
            
            if (c == '#')
            {
                c = s[i++];
                if (c >= '0' && c <= '9')
                {
                    var code = c - '0';
                    for (c = s[i++]; c >= '0' && c <= '9'; c = s[i++])
                    {
                        if (i == s.Length) goto endofstring;
                        code *= 10;
                        code += c - '0';
                    }
                    if (c == ';')
                    {
                        builder.Append((char) code);
                        return i - 1;
                    }
                }
            }
            
            endofstring:
            builder.Append('&');
            return start;
        }
    }
}