using System;
using System.IO;
using System.Text;

namespace TS.Pisa.Tools
{
    public class ByteBuffer
    {
        private const byte CarriageReturn = (byte) '\r';
        private const byte NewLine = (byte) '\n';
        public int Capacity { get; }
        public int ReaderIndex { get; set; }
        public int WriterIndex { get; set; }
        private readonly byte[] _buffer;

        public ByteBuffer(int capacity = 8192)
        {
            Capacity = capacity;
            _buffer = new byte[8192];
        }

        public string ReadLineFrom(Stream stream)
        {
            WriteBytes(stream);
            var line = ReadLine();
            while (line == null)
            {
                WriteBytes(stream);
                line = ReadLine();
            }
            return line;
        }

        public void WriteBytes(Stream stream)
        {
            var received = stream.Read(_buffer, WriterIndex, Capacity - WriterIndex);
            WriterIndex += received;
        }

        public string ReadLine()
        {
            var start = ReaderIndex;
            var eol = Array.IndexOf(_buffer, NewLine, start, WriterIndex - start);
            if (eol == -1) return null;
            ReaderIndex = eol + 1;
            if (eol > 0 && _buffer[eol - 1] == CarriageReturn)
            {
                eol--;
            }
            return Encoding.ASCII.GetString(_buffer, start, eol - start);
        }
    }
}