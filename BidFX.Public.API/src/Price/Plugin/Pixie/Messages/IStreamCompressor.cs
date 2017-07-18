using System.IO;

namespace BidFX.Public.API.Price.Plugin.Pixie.Messages
{
    /// <summary>
    /// Used to compress data. The data can be compressed a chunk at a time.
    /// </summary>
    public interface IStreamCompressor
    {
        /// <summary>
        /// Compress a stream fragment to the end of the compressed stream. The fragment is then cleared ready for reused/
        /// </summary>
        /// <param name="fragment">the fragment to append</param>
        void Compress(MemoryStream fragment);

        /// <summary>
        /// Gets a copy of the compressed stream containing the appended buffers and clear the larger buffer ready for its next use.
        /// </summary>
        /// <returns></returns>
        byte[] GetCompressed();
    }
}