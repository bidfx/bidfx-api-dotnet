using System;
using System.IO;
using System.Text;

namespace TS.Pisa.Tools
{
    public class ByteBuffer
    {
        private const byte CarriageReturn = (byte) '\r';
        private const byte NewLine = (byte) '\n';
        private const byte XmlCloseTag = (byte) '>';
        public int Capacity { get; }
        public int ReaderIndex { get; set; }
        public int WriterIndex { get; set; }
        private readonly byte[] _buffer;

        public ByteBuffer(int capacity = 8192)
        {
            Capacity = capacity;
            _buffer = new byte[8192];
        }

        public string ReadLineFromStream(Stream stream)
        {
            return ReadFromStreamUntil(stream, NewLine);
        }

        public string ReadXmlFromStream(Stream stream)
        {
            return ReadFromStreamUntil(stream, XmlCloseTag);
        }

        public string ReadFromStreamUntil(Stream stream, byte endByte)
        {
            var line = ReadFromBufferUntil(endByte);
            while (line == null)
            {
                WriteBytesToBuffer(stream);
                line = ReadFromBufferUntil(endByte);
            }
            return line;
        }

        private void WriteBytesToBuffer(Stream stream)
        {
            var received = stream.Read(_buffer, WriterIndex, Capacity - WriterIndex);
            WriterIndex += received;
        }

        private string ReadFromBufferUntil(byte endChar)
        {
            var start = ReaderIndex;
            var eol = Array.IndexOf(_buffer, endChar, start, WriterIndex - start);
            if (eol == -1) return null;
            ReaderIndex = eol + 1;
            if (endChar.Equals(NewLine) && eol > 0 && _buffer[eol - 1] == CarriageReturn)
            {
                eol--;
                return Encoding.ASCII.GetString(_buffer, start, eol - start);
            }
            return Encoding.ASCII.GetString(_buffer, start, ReaderIndex - start);
        }

        public void Clear()
        {
            ReaderIndex = 0;
            WriterIndex = 0;
        }
    }
}