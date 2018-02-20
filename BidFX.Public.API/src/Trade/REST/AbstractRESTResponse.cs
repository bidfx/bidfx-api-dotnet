using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using BidFX.Public.API.Price.Tools;
using log4net;
using Newtonsoft.Json;

namespace BidFX.Public.API.Trade.REST
{
    public abstract class AbstractRESTResponse : EventArgs
    {
        private static readonly ILog Log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly HttpStatusCode StatusCode;
        private List<Dictionary<string, object>> _responses;

        protected AbstractRESTResponse(HttpWebResponse webResponse)
        {
            Params.NotNull(webResponse);
            StatusCode = webResponse.StatusCode;
            if (StatusCode.Equals(HttpStatusCode.Unauthorized))
            {
                SetUnauthorizedResponseJSON();
                return;
            }
            
            Stream responseStream = webResponse.GetResponseStream();
            if (responseStream == null)
            {
                Log.Warn("No response stream from web response. Setting responses to empty list");
                _responses = new List<Dictionary<string, object>>();
                return;
            }

            StreamReader responseReader = new StreamReader(responseStream);
            string jsonString = responseReader.ReadToEnd();
            ParseJsonResponse(jsonString);
        }

        private void ParseJsonResponse(string jsonString)
        {
            try
            {
                _responses = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jsonString);
            }
            catch (Exception e)
            {
                Log.WarnFormat("Unexpected Error occured deserializing REST response.\nMessage body: {0}\n{1}", jsonString, e);
                //Likely from a 501 server error or 408 timeout error. All body is put into the error of the first item
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
            // ReSharper disable once SwitchStatementMissingSomeCases
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

        private void SetUnauthorizedResponseJSON()
        {
            _responses = new List<Dictionary<string, object>>
            {
                new Dictionary<string, object>()
            };
            _responses[0]["errors"] = "[{\"message\": \"401 Unauthorized - Invalid Username or Password\"}]";
        }
        
        /**
         * For testing
         */
        internal AbstractRESTResponse(string jsonString, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            StatusCode = statusCode;
            if (StatusCode.Equals(HttpStatusCode.Unauthorized))
            {
                SetUnauthorizedResponseJSON();
            }
            else
            {
                ParseJsonResponse(jsonString);
            }
        }
    }
}