using System;
using System.IO;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using log4net;

namespace BidFX.Public.API.Price.Tools
{
    public static class ConnectionTools
    {
        private static readonly ILog Log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        
        public static void UpgradeToSsl(ref Stream stream, string host, bool disableHostnameSslChecks)
        {
            var sslStream = disableHostnameSslChecks
                ? new SslStream(stream, false, AllowCertsFromTs)
                : new SslStream(stream, false);
            sslStream.AuthenticateAsClient(host);
            if (sslStream.IsAuthenticated)
            {
                stream = sslStream;
                if (Log.IsDebugEnabled) Log.Debug("Upgraded stream to SSL");
            }
            else
            {
                throw new TunnelException("Failed to upgrade stream to SSL");
            }
        }

        private static bool AllowCertsFromTs(Object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors errors)
        {
            return chain.ChainStatus.Length == 0 && Regex.IsMatch(cert.Subject, ".*CN=.*\\.tradingscreen\\.com.*");
        }

        public static string CreateTunnelHeader(string username, string password, string service, GUID guid)
        {
            var auth = Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ':' + password));
            return "CONNECT " + service + " HTTP/1.1\r\nAuthorization: Basic " + auth + "\r\n" +
                        "GUID: " + guid + "\r\n\r\n";
        }

        public static void SendMessage(Stream stream, string message)
        {
            if (Log.IsDebugEnabled)
            {
                Log.Debug("sending: " + message);
            }
            stream.Write(Encoding.ASCII.GetBytes(message), 0, message.Length);
            stream.Flush();
        }

        public static void ReadTunnelResponse(Stream stream)
        {
            var buffer = new ByteBuffer();
            var response = buffer.ReadLineFromStream(stream);
            if (Log.IsDebugEnabled)
            {
                Log.Debug("received: " + response);
            }
            if (!"HTTP/1.1 200 OK".Equals(response) || buffer.ReadLineFromStream(stream).Length != 0)
            {
                const string prefix = "HTTP/1.1 ";
                if (response.StartsWith(prefix))
                {
                    response = response.Substring(prefix.Length, response.Length - prefix.Length);
                }
                throw new TunnelException("tunnel rejected with response: " + response);
            }
        }
    }
}