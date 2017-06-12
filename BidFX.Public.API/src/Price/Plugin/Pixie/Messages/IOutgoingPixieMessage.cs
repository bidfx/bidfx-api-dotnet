using System.IO;

namespace BidFX.Public.API.Price.Plugin.Pixie.Messages
{
    public interface IOutgoingPixieMessage
    {
        MemoryStream Encode(int version);
    }
}