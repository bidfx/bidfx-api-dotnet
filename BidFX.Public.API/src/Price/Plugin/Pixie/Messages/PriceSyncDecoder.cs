﻿using System.IO;

namespace BidFX.Public.API.Price.Plugin.Pixie.Messages
{
    internal class PriceSyncDecoder
    {
        private readonly IStreamInflator _streamInflator = new ZlibStreamInflator();

        public PriceSync DecodePriceSync(MemoryStream stream)
        {
            return new PriceSync(stream, _streamInflator);
        }
    }
}