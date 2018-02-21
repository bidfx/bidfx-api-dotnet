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
                SetUnauthorizedResponseJson();
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
                try
                {
                    Log.DebugFormat("Parsing JSON: {0}", jsonString);
                    _responses = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jsonString);
                    return;
                }
                catch (JsonSerializationException e)
                {
                    Log.Warn("Error parsing JSON", e);
                }
    
                try
                {
                    // Next we'll try parse as a single dictionary
                    // likely from a 501 server error or 408 timeout error.
                    _responses = new List<Dictionary<string, object>>
                    {
                        JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString)
                    };
                    return;
                }
                catch (JsonSerializationException e)
                {
                    Log.Warn("Could not parse JSON, putting response in error of first item");
                }
                
                _responses = new List<Dictionary<string, object>>
                {
                    new Dictionary<string, object>()
                };
                _responses[0]["errors"] = "[" + jsonString + "]";
                
            }
            catch (Exception e)
            {
                Log.Error("Unexpected error occurred processing JSON", e);
                _responses = new List<Dictionary<string, object>>
                {
                    new Dictionary<string, object>()
                };
                _responses[0]["errors"] = "[" + e.Message + "]";
            }
        }

        /// <summary>
        /// Get a value from the server response
        /// </summary>
        /// <param name="fieldName">Name of the field for which to get the value of</param>
        /// <returns>The string representation of the value assigned to the field specified by fieldName, or null if the field does not exist</returns>
        public string GetField(string fieldName)
        {
            object retval;
            return _responses[0].TryGetValue(fieldName, out retval) && retval != null ? retval.ToString() : null;
        }

        /// <summary>
        /// Determines if the server response has a particular field.
        /// </summary>
        /// <param name="fieldName">The field to check the existance of.</param>
        /// <returns>True if the field exists, false otherwise.</returns>
        public bool HasField(string fieldName)
        {
            return _responses[0].ContainsKey(fieldName);
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

        private void SetUnauthorizedResponseJson()
        {
            _responses = new List<Dictionary<string, object>>
            {
                new Dictionary<string, object>()
            };
            _responses[0]["errors"] = "[{\"type\": \"Unauthorized\",\"message\": \"Invalid Username or Password\"}]";
        }
        
        /**
         * For testing
         */
        internal AbstractRESTResponse(string jsonString, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            StatusCode = statusCode;
            if (StatusCode.Equals(HttpStatusCode.Unauthorized))
            {
                SetUnauthorizedResponseJson();
            }
            else
            {
                ParseJsonResponse(jsonString);
            }
        }
    }
}