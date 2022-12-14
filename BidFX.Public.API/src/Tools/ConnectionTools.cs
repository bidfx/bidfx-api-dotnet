using System;
using System.IO;
using System.Net.Security;
using System.Reflection;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace BidFX.Public.API.Price.Tools
{
    internal class ConnectionTools
    {
        private static readonly ILogger Log = Serilog.Log.ForContext(Constants.SourceContextPropertyName, "ConnectionTools");
        public static void UpgradeToSsl(ref Stream stream, string host, bool disableHostnameSslChecks)
        {
            SslStream sslStream = disableHostnameSslChecks
                ? new SslStream(stream, false, AllowCertsFromTs)
                : new SslStream(stream, false);
            sslStream.AuthenticateAsClient(host, null, SslProtocols.Tls12, false);
            if (sslStream.IsAuthenticated)
            {
                stream = sslStream;
                Log.Information("Upgraded stream to SSL - {protocol}", sslStream.SslProtocol);
            }
            else
            {
                throw new TunnelException("Failed to upgrade stream to SSL");
            }
        }

        private static bool AllowCertsFromTs(Object sender, X509Certificate cert, X509Chain chain,
            SslPolicyErrors errors)
        {
            return chain.ChainStatus.Length == 0 && Regex.IsMatch(cert.Subject, ".*CN=.*\\.bidfx\\.(com|biz).*");
        }

        public static string CreateTunnelHeader(string username, string password, string service, GUID guid)
        {
            string auth = Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ':' + password));
            return "CONNECT " + service + " HTTP/1.1\r\nAuthorization: Basic " + auth + "\r\n" +
                   "GUID: " + guid + "\r\n\r\n";
        }

        public static void SendMessage(Stream stream, string message)
        {
            if (Log.IsEnabled(LogEventLevel.Debug))
            {
                #if DEBUG
                    Log.Debug("sending: " + message);
                #else
                    if (Log.IsEnabled(LogEventLevel.Debug) && !message.Contains("Authorization: "))
                    {
                        Log.Debug("sending: {message}", message);
                    }
                #endif
            }

            stream.Write(Encoding.ASCII.GetBytes(message), 0, message.Length);
            stream.Flush();
        }

        public static void ReadTunnelResponse(Stream stream)
        {
            ByteBuffer buffer = new ByteBuffer();
            string response = buffer.ReadLineFromStream(stream);
            Log.Debug("received: {response}", response);

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