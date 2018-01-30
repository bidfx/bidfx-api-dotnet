using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml.Schema;

namespace BidFX.Public.API.Trade.REST
{
    public class RESTClient
    {
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
            var req = (HttpWebRequest) WebRequest.Create(_baseAddress + path);
            req.Method = method;
            req.Headers["Authorization"] = _authHeader;

            req.ContentType = "application/json";
            return (HttpWebResponse) req.GetResponse();
        }

        public HttpWebResponse SendJSON(string method, string path, string json)
        {
            if (!path.StartsWith("/"))
            {
                path = "/" + path;
            }
            var req = (HttpWebRequest) WebRequest.Create(_baseAddress + path);
            req.Method = method;
            req.Headers["Authorization"] = _authHeader;
                
            req.ContentType = "application/json";
            var streamWriter = new StreamWriter(req.GetRequestStream());
            streamWriter.Write(json);
            streamWriter.Flush();
            streamWriter.Close();
            return (HttpWebResponse) req.GetResponse();
        }
    }
}