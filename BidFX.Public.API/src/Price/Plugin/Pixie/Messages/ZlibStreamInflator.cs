using System;
using System.IO;
using System.IO.Compression;

namespace BidFX.Public.API.Price.Plugin.Pixie.Messages
{
    /// <summary>
    /// Stateful wrapper around a DeflateStream that allows for fragments of deflated messages to be inflated
    /// independently in sequence. This is required as each message received from the server is deflated as part
    /// of a continuous stream, rather than as a standalone message.
    /// </summary>
    internal class ZlibStreamInflator : IStreamInflator, IDisposable
    {
        private readonly MemoryStream _outputStream = new MemoryStream();
        private readonly ReplaceableMemoryStream _bufferedInput = new ReplaceableMemoryStream();
        private readonly DeflateStream _deflateStream;
        private readonly byte[] _readBuffer = new byte[4096];

        public ZlibStreamInflator()
        {
            // Ionic.Zlib's DeflateStream wrapped an outputs stream, and you could repeatedly write to it.
            // System.IO.Compression's DeflateStream wraps an input stream, and you read from it.
            // We have to wrap a custom stream in order to replicate the Ionic.Zlib behaviour.
            _deflateStream = new DeflateStream(_bufferedInput, CompressionMode.Decompress, leaveOpen: true);
        }

        /// <summary>
        /// Inflate a fragment of deflated data within the context of the previously inflated fragments.
        /// This function should be called with each deflated message in order.
        /// </summary>
        /// <param name="stream">The deflated data fragment to inflate.</param>
        /// <returns>A stream of inflated data.</returns>
        public Stream Inflate(MemoryStream stream)
        {
            _outputStream.Position = 0;
            _outputStream.SetLength(0);
            
            // Set a new memory stream within the deflate stream so that we can deflate it
            _bufferedInput.SetInternalStream(stream);

            // Read until the inner MemoryStream is exhausted (no bytes read)
            int bytesRead;
            while ((bytesRead = _deflateStream.Read(_readBuffer, 0, _readBuffer.Length)) > 0)
            {
                _outputStream.Write(_readBuffer, 0, bytesRead);
            }

            _outputStream.Position = 0;
            return _outputStream;
        }

        public void Dispose()
        {
            _deflateStream.Dispose();
            _bufferedInput.Dispose();
            _outputStream.Dispose();
        }
        
        /// <summary>
        /// A Stream that wraps a MemoryStream, making the memory stream replaceable once it is exhausted.
        /// This allows an inversion of the way the DeflateStream works so that we can use it the same way
        /// that the Ionic.Zlib DeflateStream was used.
        /// </summary>
        private class ReplaceableMemoryStream : Stream
        {
            private MemoryStream _data;

            /// <summary>
            /// Replaces the internal memory stream with a new stream.
            /// This internal stream must be exhausted before calling this function as there is no internal buffer
            /// so some data may be lost.
            /// Note: this could have been implemented using internal buffers and the <see cref="Write"/> function
            /// but this way avoids some complexity, allocations and copying data to buffers.
            /// </summary>
            /// <param name="data">The memory stream to read next.</param>
            /// <exception cref="IllegalStateException">If the current internal stream is not exhausted.</exception>
            public void SetInternalStream(MemoryStream data)
            {
                if (_data != null && _data.Position < _data.Length)
                    throw new IllegalStateException("Previous memory stream is not exhausted!");
                _data = data;
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                return _data == null ? 0 : _data.Read(buffer, offset, count);
            }

            public override bool CanRead
            {
                get { return true; }
            }

            public override bool CanSeek
            {
                get { return false; }
            }

            public override bool CanWrite
            {
                get { return false; }
            }

            public override long Length
            {
                get { return _data.Length; }
            }

            public override long Position
            {
                get { throw new NotSupportedException(); }
                set { throw new NotSupportedException(); }
            }

            public override void Flush() { }
            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotSupportedException();
            }

            public override void SetLength(long value)
            {
                throw new NotSupportedException();
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                throw new NotSupportedException();
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    if (_data != null)
                        _data.Dispose();
                }
                base.Dispose(disposing);
            }
        }
    }
}