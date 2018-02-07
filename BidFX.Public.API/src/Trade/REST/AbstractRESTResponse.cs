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
        private readonly HttpStatusCode StatusCode;
        private List<Dictionary<string, object>> _responses;

        protected AbstractRESTResponse(HttpWebResponse webResponse)
        {
            Params.NotNull(webResponse);
            StatusCode = webResponse.StatusCode;
            if (StatusCode.Equals(HttpStatusCode.Forbidden))
            {
                SetForbiddenResponseJSON();
                return;
            }
            
            Stream responseStream = webResponse.GetResponseStream();
            if (responseStream == null)
            {
                throw new IOException("No Response Stream from webResponse");
            }

            StreamReader responseReader = new StreamReader(responseStream);
            string jsonString = responseReader.ReadToEnd();
            ParseJsonResponse(jsonString);
        }
        
        /**
         * For testing
         */
        internal AbstractRESTResponse(string jsonString, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            StatusCode = statusCode;
            if (StatusCode.Equals(HttpStatusCode.Forbidden))
            {
                SetForbiddenResponseJSON();
            }
            else
            {
                ParseJsonResponse(jsonString);
            }
        }

        private void ParseJsonResponse(string jsonString)
        {
            try
            {
                _responses = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jsonString);
            }
            catch (Exception e)
            {
                //Likely from a 501 server error or 408 timeout error, so will be a single dictionary.
                _responses = new List<Dictionary<string, object>>
                {
                    new Dictionary<string, object>()
                };
                _responses[0]["errors"] = "[" + jsonString + "]";
            }
        }

        protected string GetField(string fieldname)
        {
            object retval;
            return _responses[0].TryGetValue(fieldname, out retval) ? retval.ToString() : null;
        }

        public bool HasField(string fieldname)
        {
            return _responses[0].ContainsKey(fieldname);
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
            IEnumerable<string> formattedOrders = _responses.Select(
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

        private void SetForbiddenResponseJSON()
        {
            _responses = new List<Dictionary<string, object>>
            {
                new Dictionary<string, object>()
            };
            _responses[0]["errors"] = "[{\"message\": \"401 Forbidden - Invalid Username or Password\"}]";
        }
    }
}