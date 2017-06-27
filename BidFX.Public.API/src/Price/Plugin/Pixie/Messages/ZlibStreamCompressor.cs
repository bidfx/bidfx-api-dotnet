using System;
using System.IO;
using BidFX.Public.API.Price.Tools;
using Ionic.Zlib;

namespace BidFX.Public.API.Price.Plugin.Pixie.Messages
{
    public class ZlibStreamCompressor : IStreamCompressor
    {
        private readonly MemoryStream _stream = new MemoryStream();
        private readonly DeflateStream _compressor;

        public ZlibStreamCompressor(int compressionLevel)
        {
            Params.InRange(compressionLevel, 1, 9);
            _compressor = new DeflateStream(_stream, CompressionMode.Compress, ToCompressionLevel(compressionLevel));
        }

        public void Compress(MemoryStream fragment)
        {
            var buffer = fragment.ToArray();
            _compressor.Write(buffer, 0, buffer.Length);
            fragment.SetLength(0);
        }

        public byte[] GetCompressed()
        {
            _compressor.Flush();
            _compressor.Dispose();
            return _stream.ToArray();
        }

        private static CompressionLevel ToCompressionLevel(int level)
        {
            switch (level)
            {
                case 1: return CompressionLevel.Level1;
                case 2: return CompressionLevel.Level2;
                case 3: return CompressionLevel.Level3;
                case 4: return CompressionLevel.Level4;
                case 5: return CompressionLevel.Level5;
                case 6: return CompressionLevel.Level6;
                case 7: return CompressionLevel.Level7;
                case 8: return CompressionLevel.Level8;
                case 9: return CompressionLevel.Level9;
                default: throw new ArgumentException("Level (" + level + ") was not in range 1..9");
            }
        }
    }
}