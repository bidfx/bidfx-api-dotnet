/// Copyright (c) 2018 BidFX Systems Ltd. All Rights Reserved.

using System;
using System.IO;
using System.IO.Compression;
using BidFX.Public.API.Price.Tools;

namespace BidFX.Public.API.Price.Plugin.Pixie.Messages
{
    internal class ZlibStreamCompressor : IStreamCompressor
    {
        private readonly MemoryStream _stream = new MemoryStream();
        private readonly DeflateStream _compressor;

        [Obsolete("Use CompressionLevel instead of int")]
        public ZlibStreamCompressor(int compressionLevel)
        {
            Params.InRange(compressionLevel, 1, 9);
            _compressor = new DeflateStream(_stream, ToCompressionLevel(compressionLevel));
        }
        
        public ZlibStreamCompressor(CompressionLevel compressionLevel)
        {
            _compressor = new DeflateStream(_stream, compressionLevel);
        }

        public void Compress(MemoryStream fragment)
        {
            byte[] buffer = fragment.ToArray();
            _compressor.Write(buffer, 0, buffer.Length);
            fragment.SetLength(0);
        }

        public byte[] GetCompressed()
        {
            _compressor.Flush();
            _compressor.Dispose();
            return _stream.ToArray();
        }

        // This is a best effort to map from old Ionic.Zip compression levels to System.IO.Compression levels
        private static CompressionLevel ToCompressionLevel(int level)
        {
            switch (level)
            {
                case 1: return CompressionLevel.Fastest;
                case 2: return CompressionLevel.Fastest;
                case 3: return CompressionLevel.Fastest;
                case 4: return CompressionLevel.Fastest;
                case 5: return CompressionLevel.Optimal;
                case 6: return CompressionLevel.Optimal;
                case 7: return CompressionLevel.Optimal;
                case 8: return CompressionLevel.Optimal;
                case 9: return CompressionLevel.Optimal;
                default: throw new ArgumentException("Level (" + level + ") was not in range 1..9");
            }
        }
    }
}