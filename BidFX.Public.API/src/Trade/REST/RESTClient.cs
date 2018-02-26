/// Copyright (c) 2018 BidFX Systems LTD. All Rights Reserved.

using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using log4net;

namespace BidFX.Public.API.Trade.REST
{
    internal class RESTClient
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

        /// <summary>
        /// Send a REST request with no body.
        /// </summary>
        /// <param name="method">HTTP Method, i.e. GET, DELETE</param>
        /// <param name="path">The path of the request location. Is appended to the Base Address.</param>
        /// <returns></returns>
        public HttpWebResponse SendMessage(string method, string path)
        {
            if (!path.StartsWith("/"))
            {
                path = "/" + path;
            }

            Log.DebugFormat("Sending REST message to {0}{1}", _baseAddress, path);
            HttpWebRequest req = (HttpWebRequest) WebRequest.Create(_baseAddress + path);
            req.Method = method;
            req.Headers["Authorization"] = _authHeader;

            req.ContentType = "application/json";

            HttpWebResponse response = (HttpWebResponse) req.GetResponse();
            Log.DebugFormat("Response Received, status {0}", response.StatusCode);

            return response;
        }

        /// <summary>
        /// Send a REST request with a JSON body.
        /// </summary>
        /// <param name="method">HTTP Method, i.e. GET, DELETE</param>
        /// <param name="path">The path of the request location. Is appended to the Base Address.</param>
        /// <param name="json">The JSON body to be attached to the message.</param>
        /// <returns></returns>
        public HttpWebResponse SendJSON(string method, string path, string json)
        {
            if (!path.StartsWith("/"))
            {
                path = "/" + path;
            }

            Log.DebugFormat("Sending REST message with JSON to {0}{1}", _baseAddress, path);
            HttpWebRequest req = (HttpWebRequest) WebRequest.Create(_baseAddress + path);
            req.Method = method;
            req.Headers["Authorization"] = _authHeader;

            req.ContentType = "application/json";
            using (StreamWriter streamWriter = new StreamWriter(req.GetRequestStream()))
            {
                streamWriter.Write(json);
            }

            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse) req.GetResponse();
            }
            catch (WebException e)
            {
                // Occurs on non-success codes
                response = (HttpWebResponse) e.Response;
            }
            Log.DebugFormat("Response Received, status {0}", response.StatusCode);

            return response;
        }
    }
}