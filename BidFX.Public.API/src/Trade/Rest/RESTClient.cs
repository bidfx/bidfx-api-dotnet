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
        private static readonly ILog Log = LogManager.GetLogger("RESTClient");
        private const string ApiPath = "/api/om/v2/order";
        private readonly string _authHeader;
        private readonly Uri _address;

        public RESTClient(Uri baseAddress, string username, string password)
        {
            if (!("https".Equals(baseAddress.Scheme) || "http".Equals(baseAddress.Scheme)))
            {
                throw new ArgumentException("Scheme must be http or https");
            }

            _address = new UriBuilder(baseAddress)
            {
                Path = ApiPath
            }.Uri;
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

            Log.InfoFormat("Sending REST message to {0}{1}", _address, path);
            HttpWebRequest req = (HttpWebRequest) WebRequest.Create(_address + path);
            req.Method = method;
            req.Headers["Authorization"] = _authHeader;

            req.ContentType = "application/json";

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
            Log.InfoFormat("Response Received, status {0}", response.StatusCode);

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

            Log.InfoFormat("Sending REST message with JSON to {0}{1}", _address, path);
            HttpWebRequest req = (HttpWebRequest) WebRequest.Create(_address + path);
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
            Log.InfoFormat("Response Received, status {0}", response.StatusCode);

            return response;
        }
    }
}