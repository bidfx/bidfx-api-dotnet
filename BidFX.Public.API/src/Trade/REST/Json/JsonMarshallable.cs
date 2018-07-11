using System.Collections.Generic;

namespace BidFX.Public.API.Trade.Rest.Json
{
    public interface IJsonMarshallable
    {
        IDictionary<string, object> GetJsonMap();
    }
}