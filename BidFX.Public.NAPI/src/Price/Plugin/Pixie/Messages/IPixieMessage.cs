using System.IO;

namespace BidFX.Public.NAPI.Price.Plugin.Pixie.Messages
{
    public interface IPixieMessage
    {
        MemoryStream Encode(int version);
    }
}