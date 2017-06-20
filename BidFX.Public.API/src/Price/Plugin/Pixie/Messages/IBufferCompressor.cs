﻿using System.IO;

namespace BidFX.Public.API.Price.Plugin.Pixie.Messages
{
    public interface IBufferCompressor
    {
        void Compress(MemoryStream fragment);

        byte[] GetCompressed();
    }
}