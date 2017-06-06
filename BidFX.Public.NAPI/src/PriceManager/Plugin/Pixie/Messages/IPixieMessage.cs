using System.IO;

namespace BidFX.Public.NAPI.PriceManager.Plugin.Pixie.Messages
{
    public interface IPixieMessage
    {
        MemoryStream Encode(int version);
    }
}