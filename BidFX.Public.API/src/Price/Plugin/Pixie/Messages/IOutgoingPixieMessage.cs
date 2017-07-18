using System.IO;

namespace BidFX.Public.API.Price.Plugin.Pixie.Messages
{
    /// <summary>
    /// This interfaces defines a basic message in the Pixie protocol that can be sent from the client to the server.
    /// </summary>
    public interface IOutgoingPixieMessage
    {
        /// <summary>
        /// Encodes this message and returns it as a memory stream.
        /// </summary>
        /// <param name="version">the version to encode to</param>
        /// <returns>the encoded memory stream</returns>
        MemoryStream Encode(int version);
    }
}