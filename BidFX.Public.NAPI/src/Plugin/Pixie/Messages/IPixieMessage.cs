using System.IO;

namespace BidFX.Public.NAPI.Plugin.Pixie.Messages
{
    public interface IPixieMessage
    {
        MemoryStream Encode(int version);
    }
}