/// Copyright (c) 2018 BidFX Systems LTD. All Rights Reserved.

using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using BidFX.Public.API.Trade.REST;
using log4net;
using Newtonsoft.Json;

namespace BidFX.Public.API.Trade
{
    internal class RESTOrderResponse : AbstractRESTResponse
    {
        private static readonly ILog Log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly long _messageId;

        public RESTOrderResponse(HttpWebResponse webResponse, long messageId) : base(webResponse)
        {
            _messageId = messageId;
        }

        /**
         * For testing
         */
        internal RESTOrderResponse(string json, HttpStatusCode statusCode = HttpStatusCode.OK) : base(json,statusCode)
        {
        }


        public override long GetMessageId()
        {
            return _messageId;
        }
        
        public override string GetOrderId()
        {
            return GetField("order_ts_id");
        }

        public override string ToString()
        {
            return "MessageId => " + _messageId + ", Body => " + base.ToString();
        }

        public override string GetState()
        {
            return GetField("state");
        }

        public override List<string> GetErrors()
        {
            string errors = GetField("errors");
            if (errors == null)
            {
                string type = GetField("type");
                string message = GetField("message");
                if (type == null)
                {
                    return new List<string>();
                }

                Dictionary<string, string> error = new Dictionary<string, string>
                    {
                        {"type", type}, 
                        {"message", message}
                    };

                return new List<string>
                {
                    ConvertErrorDictionaryToString(error)
                };
            }
            
            try
            {
                List<Dictionary<string, string>> list = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(errors);
                return list.ConvertAll(ConvertErrorDictionaryToString);
            }
            catch (JsonSerializationException e)
            {
                Log.Error("Could not deserialize errors, returning as string", e);
                return new List<string> {errors};
            }
        }

        private string ConvertErrorDictionaryToString(Dictionary<string, string> error)
        {
            if (error.ContainsKey("type"))
            {
                return error["type"] + " - " + error["message"];
            }

            if (error.ContainsKey("message"))
            {
                return error["message"];
            }

            return string.Join(", ", error.Values.ToList());
        }
    }
}