/// Copyright (c) 2018 BidFX Systems Ltd. All Rights Reserved.

using System.IO;
using Ionic.Zlib;

namespace BidFX.Public.API.Price.Plugin.Pixie.Messages
{
    internal class ZlibStreamInflator : IStreamInflator
    {
        private readonly MemoryStream _outputStream = new MemoryStream();
        private readonly DeflateStream _inflator;

        public ZlibStreamInflator()
        {
            _inflator = new DeflateStream(_outputStream, CompressionMode.Decompress, CompressionLevel.Level6, true);
        }

        public Stream Inflate(MemoryStream stream)
        {
            _outputStream.Position = 0;
            _outputStream.SetLength(0);
            byte[] array = stream.ToArray();
            _inflator.Write(array, (int) stream.Position, array.Length - (int) stream.Position);
            _outputStream.Position = 0;
            return _outputStream;
        }
    }
}