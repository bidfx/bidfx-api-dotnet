using System;

namespace BidFX.Public.API.Trade.Rest.Json
{
    public class JsonException : Exception
    {
        public JsonException()
        {}

        public JsonException(string message) : base(message)
        {}
        
        public JsonException(string message, Exception inner) :  base(message, inner)
        {}
    }
}