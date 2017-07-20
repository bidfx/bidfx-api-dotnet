using System;
using System.Text;

namespace BidFX.Public.API.Price.Plugin.Pixie.Messages
{
    /// <summary>
    /// Represents a column of a grid, updated by Pixie messages.
    /// </summary>
    internal class GridColumn : IColumn
    {
        private object[] _overflow = new object[128];
        private object[] _values = { };

        private int _size = 0;

        public int Size()
        {
            return _size;
        }

        public void SetImage(object[] image, int size)
        {
            _values = image;
            _size = size;
        }

        public object Get(int i)
        {
            if (i >= _size)
            {
                throw new IndexOutOfRangeException(i + " is greater than " + (_size - 1));
            }
            return i < _values.Length ? _values[i] : _overflow[i - _values.Length];
        }

        public void Set(int i, object value)
        {
            if (i >= _size)
            {
                ExtendTo(i + 1);
            }
            if (i < _values.Length)
            {
                _values[i] = value;
            }
            else
            {
                _overflow[i - _values.Length] = value;
            }
        }

        public void DeleteFrom(int i)
        {
            if (i < _size)
            {
                _size = i;
            }
        }

        private void ExtendTo(int newSize)
        {
            _size = newSize;
            if (_overflow.Length + _values.Length < newSize)
            {
                ResizeOverflow(newSize - _values.Length);
            }
        }

        private void ResizeOverflow(int newSize)
        {
            var newLength = _overflow.Length;
            while (newLength < newSize)
            {
                newLength *= 2;
            }
            DoResizeOverflow(newLength);
        }

        private void DoResizeOverflow(int newLength)
        {
            var newSnapshot = new object[newLength];
            Array.Copy(_overflow, 0, newSnapshot, 0, _overflow.Length);
            _overflow = newSnapshot;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append("GridColumn(Size=").Append(_size).Append(',');

            builder.Append('[');
            var first = true;
            for (int i = 0; i < _size; i++)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    builder.Append(',');
                }
                builder.Append(Get(i));
            }
            builder.Append(']');

            builder.Append(')');
            return builder.ToString();
        }
    }
}