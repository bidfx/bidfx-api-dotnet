using System.IO;

namespace BidFX.Public.API.Price.Plugin.Pixie.Messages
{
    public class UncompressedStreamCompressor : IStreamCompressor
    {
        private readonly MemoryStream _stream = new MemoryStream();

        public void Compress(MemoryStream fragment)
        {
            _stream.Write(fragment.GetBuffer(), 0, (int) fragment.Length);
            fragment.SetLength(0);
        }

        public byte[] GetCompressed()
        {
            return _stream.ToArray();
        }
    }
}