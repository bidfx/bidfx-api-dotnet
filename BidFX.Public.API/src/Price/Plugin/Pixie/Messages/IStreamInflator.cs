using System.IO;

namespace BidFX.Public.API.Price.Plugin.Pixie.Messages
{
    /// <summary>
    /// This inteface is used to inflate compressed data.
    /// </summary>
    internal interface IStreamInflator
    {
        /// <summary>
        /// Inflates a compressed memory stream.
        /// </summary>
        /// <param name="stream">the memory stream to inflate</param>
        /// <returns>the inflated equivalent</returns>
        Stream Inflate(MemoryStream stream);
    }
}