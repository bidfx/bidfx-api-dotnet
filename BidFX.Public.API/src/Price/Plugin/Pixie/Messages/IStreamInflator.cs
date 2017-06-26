using System.IO;

namespace BidFX.Public.API.Price.Plugin.Pixie.Messages
{
    public interface IStreamInflator
    {
        Stream Inflate(MemoryStream stream);
    }
}