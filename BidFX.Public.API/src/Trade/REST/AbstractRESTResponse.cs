using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using BidFX.Public.API.Price.Tools;
using Newtonsoft.Json;

namespace BidFX.Public.API.Trade.REST
{
    public abstract class AbstractRESTResponse : EventArgs
    {
        protected readonly HttpStatusCode _statusCode;
        private readonly List<Dictionary<string, string>> _responses = new List<Dictionary<string, string>>();
        
        protected AbstractRESTResponse(HttpWebResponse webResponse)
        {
            Params.NotNull(webResponse);
            _statusCode = webResponse.StatusCode;
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
            var list = JsonConvert.DeserializeObject<List<string>>(jsonString);
            foreach (var item in list)
            {
                _responses.Add(JsonConvert.DeserializeObject<Dictionary<string, string>>(item));
            }
        }
        
        protected string GetField(string fieldname)
        {
            string retval;
            return _responses[0].TryGetValue(fieldname, out retval) ? retval : null;
        }

        protected HttpStatusCode GetStatusCode()
        {
            return _statusCode;
        }

        public override string ToString()
        {
            return "{[" + string.Join("], [", _responses) + "]}";
        }
    }
}