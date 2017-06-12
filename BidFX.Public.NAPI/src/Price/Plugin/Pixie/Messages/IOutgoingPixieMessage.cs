using System.IO;

namespace BidFX.Public.NAPI.Price.Plugin.Pixie.Messages
{
    public interface IOutgoingPixieMessage
    {
        MemoryStream Encode(int version);
    }
}