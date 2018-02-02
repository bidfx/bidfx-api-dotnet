using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using BidFX.Public.API.Price.Tools;
using Newtonsoft.Json;

namespace BidFX.Public.API.Trade.REST
{
    public abstract class AbstractRESTResponse : EventArgs
    {
        protected readonly HttpStatusCode StatusCode;
        private List<Dictionary<string, object>> _responses;
        
        protected AbstractRESTResponse(HttpWebResponse webResponse)
        {
            Params.NotNull(webResponse);
            StatusCode = webResponse.StatusCode;
            var responseStream = webResponse.GetResponseStream();
            if (responseStream == null)
            {
                throw new IOException("No Response Stream from webResponse");
            }
            var responseReader = new StreamReader(responseStream);
            var jsonString = responseReader.ReadToEnd();
           ParseJsonResponse(jsonString);
        }

        internal AbstractRESTResponse(string jsonString)
        {
            ParseJsonResponse(jsonString);
        }

        private void ParseJsonResponse(string jsonString)
        {
            _responses = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jsonString);
        }
        
        protected string GetField(string fieldname)
        {
            object retval;
            return _responses[0].TryGetValue(fieldname, out retval) ? retval.ToString() : null;
        }

        public int GetSize()
        {
            return _responses.Count;
        }
        
        protected HttpStatusCode GetStatusCode()
        {
            return StatusCode;
        }

        public override string ToString()
        {
            var formattedOrders = _responses.Select(
                order => string.Join(", ", 
                    order.Select(
                        kv => "\"" + 
                              kv.Key + 
                              "\": " + 
                              (IsNumericType(kv.Value) ? kv.Value : "\"" + kv.Value + "\"")
                    )
                )
            );
            return "[{" + string.Join("}, {", formattedOrders) + "}]";
        }
        
        private static bool IsNumericType(object o)
        {   
            switch (Type.GetTypeCode(o.GetType()))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }
    }
}