using System.Collections.Generic;

namespace BidFX.Public.API.Trade.Rest.Json
{
    internal interface IJsonMarshallable
    {
        IDictionary<string, object> GetJsonMap();
    }
}