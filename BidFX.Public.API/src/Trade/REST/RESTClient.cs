using System;
using System.IO;
using System.Net;
using System.Text;

namespace BidFX.Public.API.Trade.REST
{
    public class RESTClient
    {
        private string _username;
        private string _password;
        private string _baseAddress;
        
        private static Random _random = new Random();

        public WebResponse sendJSON(string method, string path, string json)
        {
            WebRequest req = WebRequest.Create(_baseAddress + path);
            req.Method = method;
            req.Headers["Authorization"] =
                "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(_username + ":" + _password));
            req.ContentType = "application/json; charset=utf-8";
            var streamWriter = new StreamWriter(req.GetRequestStream());
            streamWriter.Write(json);
            streamWriter.Flush();
            streamWriter.Close();
            return req.GetResponse();
        }

        public void SetUsername(string username)
        {
            _username = username;
        }

        public void SetPassword(string password)
        {
            _password = password;
        }

        public void SetBaseAddress(string address)
        {
            _baseAddress = address;
        }
    }
}