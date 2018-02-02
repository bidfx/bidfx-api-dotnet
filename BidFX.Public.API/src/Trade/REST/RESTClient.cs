using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using log4net;

namespace BidFX.Public.API.Trade.REST
{
    public class RESTClient
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private const string ApiPath = "/api/om/v2beta/fx"; //TODO: set to not-beta
        private readonly string _authHeader;
        private readonly string _baseAddress;

        public RESTClient(string baseAddress, string username, string password)
        {
            baseAddress = baseAddress.ToLower();
            if (!(baseAddress.StartsWith("http://") || baseAddress.StartsWith("https://")))
            {
                throw new ArgumentException("baseAddress must start with http:// or https://");
            }

            if (baseAddress.EndsWith("/"))
            {
                baseAddress = baseAddress.Substring(0, baseAddress.Length - 1);
            }
            
            _baseAddress = baseAddress + ApiPath;
            _authHeader = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(username + ":" + password));
        }

        public HttpWebResponse SendMessage(string method, string path)
        {
            if (!path.StartsWith("/"))
            {
                path = "/" + path;
            }
            Log.DebugFormat("Sending REST message to {0}{1}", _baseAddress, path);
            var req = (HttpWebRequest) WebRequest.Create(_baseAddress + path);
            req.Method = method;
            req.Headers["Authorization"] = _authHeader;

            req.ContentType = "application/json";
            
            var response = (HttpWebResponse) req.GetResponse();
            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("Response Received, status {0}", response.StatusCode);
            }
            return response;
        }

        public HttpWebResponse SendJSON(string method, string path, string json)
        {
            if (!path.StartsWith("/"))
            {
                path = "/" + path;
            }
            Log.DebugFormat("Sending REST message with JSON to {0}{1}", _baseAddress, path);
            var req = (HttpWebRequest) WebRequest.Create(_baseAddress + path);
            req.Method = method;
            req.Headers["Authorization"] = _authHeader;
                
            req.ContentType = "application/json";
            var streamWriter = new StreamWriter(req.GetRequestStream());
            streamWriter.Write(json);
            streamWriter.Flush();
            streamWriter.Close();
            var response = (HttpWebResponse) req.GetResponse();
            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("Response Received, status {0}", response.StatusCode);
            }
            return response;
        }
    }
}