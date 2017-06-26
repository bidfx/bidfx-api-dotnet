using System;

namespace BidFX.Public.API.Price.Tools
{
    public class StringCharIterator
    {
        private string _string;
        private int _position;
        private int _end;
        private char _mDelimiter;

        public StringCharIterator(string s, char delimiter)
        {
            _string = s;
            _mDelimiter = delimiter;
            _end = _string.Length;
            Reset();
        }

        public void Reset(string s)
        {
            _string = s;
            _end = _string.Length;
            Reset();
        }

        public void Reset()
        {
            _position = _end == 0 ? 1 : 0;
        }

        public string RemainingSubstring()
        {
            return HasNext() ? _string.Substring(_position) : "";
        }

        public string Next(char delimiter)
        {
            _mDelimiter = delimiter;
            return Next();
        }

        public bool HasNext()
        {
            return _position <= _end;
        }

        public string Next()
        {
            if (!HasNext()) throw new InvalidOperationException();
            var next = _string.IndexOf(_mDelimiter, _position);
            if (next == -1) next = _end;
            var token = _string.Substring(_position, next - _position);
            _position = ++next;
            return token;
        }
    }
}