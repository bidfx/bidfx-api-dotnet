using System.IO;

namespace BidFX.Public.API.Price.Plugin.Pixie.Messages
{
    public class UncompressedBufferCompressor : IBufferCompressor
    {
        private readonly MemoryStream _stream = new MemoryStream();
        
        public void Compress(MemoryStream fragment)
        {
            fragment.CopyTo(_stream);
            fragment.SetLength(0);
        }

        public byte[] GetCompressed()
        {
            return _stream.ToArray();
        }
    }
}