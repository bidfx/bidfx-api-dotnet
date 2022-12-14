/// Copyright (c) 2018 BidFX Systems LTD. All Rights Reserved.

using System;
using System.IO;
using System.Net;
using System.Text;
using BidFX.Public.API.Price.Tools;
using Serilog;
using Serilog.Core;

namespace BidFX.Public.API.Trade.REST
{
    internal class RESTClient
    {
        private static readonly ILogger Log = Serilog.Log.ForContext(Constants.SourceContextPropertyName, "RESTClient");
        private readonly string _authHeader;
        private readonly Uri _address;

        public RESTClient(Uri baseAddress, string username, string password)
        {
            if (!("https".Equals(baseAddress.Scheme) || "http".Equals(baseAddress.Scheme)))
            {
                throw new ArgumentException("Scheme must be http or https");
            }

            _address = new UriBuilder(baseAddress).Uri;
            _authHeader = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(username + ":" + password));
        }

        /// <summary>
        /// Send a REST request with no body.
        /// </summary>
        /// <param name="method">HTTP Method, i.e. GET, DELETE</param>
        /// <param name="path">The path of the request location. Is appended to the Base Address.</param>
        /// <returns></returns>
        public HttpWebResponse SendMessage(string method, string path, string query)
        {
            string joiner = path.StartsWith("/") || _address.AbsolutePath.EndsWith("/") ? "" : "/";
            Uri address = new UriBuilder(_address)
            {
                Path = _address.AbsolutePath + joiner + path,
                Query = query
            }.Uri;
            
            Log.Information("Sending REST message to {address}", address);
            HttpWebRequest req = (HttpWebRequest) WebRequest.Create(address.AbsoluteUri.Trim());
            req.Method = method;
            req.Headers["Authorization"] = _authHeader;
            req.ContentType = "application/json";
            req.KeepAlive = false;
            req.ServicePoint.Expect100Continue = false;
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
            Log.Information("Response Received, status {statusCode}", response.StatusCode);

            return response;
        }

        /// <summary>
        /// Send a REST request with a JSON body.
        /// </summary>
        /// <param name="method">HTTP Method, i.e. POST, PUT</param>
        /// <param name="path">The path of the request location. Is appended to the Base Address.</param>
        /// <param name="json">The JSON body to be attached to the message.</param>
        /// <returns></returns>
        public HttpWebResponse SendJSON(string method, string path, string json)
        {
            string joiner = path.StartsWith("/") || _address.AbsolutePath.EndsWith("/") ? "" : "/";
            Uri address = new UriBuilder(_address)
            {
                Path = _address.AbsolutePath + joiner + path
            }.Uri;
            
            Log.Debug("Sending REST message with JSON to {address}", address);
            HttpWebRequest req = (HttpWebRequest) WebRequest.Create(address);
            req.Method = method;
            req.Headers["Authorization"] = _authHeader;
            req.Accept = "application/json";
            req.ContentType = "application/json";
            req.KeepAlive = false;
            req.ServicePoint.Expect100Continue = false;
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
            Log.Information("Response Received, status {statusCode}", response.StatusCode);

            return response;
        }
    }
}