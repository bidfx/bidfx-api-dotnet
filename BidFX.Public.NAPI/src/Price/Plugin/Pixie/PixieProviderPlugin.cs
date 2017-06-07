using System;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using BidFX.Public.NAPI.Price.Plugin.Pixie.Messages;
using BidFX.Public.NAPI.Price.Tools;

namespace BidFX.Public.NAPI.Price.Plugin.Pixie
{
    public class PixieProviderPlugin : IProviderPlugin
    {
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public string Name { get; private set; }
        public string Service { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public bool Tunnel { set; get; }
        public bool DisableHostnameSslChecks { get; set; }
        public ProviderStatus ProviderStatus { get; private set; }
        public INAPIEventHandler InapiEventHandler { get; set; }
        public TimeSpan ReconnectInterval { get; set; }
        public string StatusReason { get; private set; }

        private readonly Thread _outputThread;
        private readonly AtomicBoolean _running = new AtomicBoolean(false);
        private PixieConnection _pixieConnection;
        private int _negotiatedVersion;
        private readonly GUID _guid = new GUID();
        private Stream _stream;
        private readonly ByteBuffer _buffer = new ByteBuffer();

        public PixieProviderPlugin()
        {
            var name =
                NameCache.Default().CreateUniqueName(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            Name = name;
            ProviderStatus = ProviderStatus.TemporarilyDown;
            StatusReason = "not started";
            Port = 443;
            Service = "static://highway";
            Tunnel = true;
            ReconnectInterval = TimeSpan.FromSeconds(10);
            _outputThread = new Thread(RunningLoop) {Name = name};
        }

        private void RunningLoop()
        {
            Log.Info("started thread for GUID " + _guid);
            while (_running.Value)
            {
                HandshakeWithServer();
                _buffer.Clear();
                if (_running.Value)
                {
                    Log.Info(Name + " will try reconnecting in " + ReconnectInterval);
                    Thread.Sleep(ReconnectInterval);
                }
            }
            Log.Info("thread stopped");
        }

        public void Subscribe(string subject)
        {
            if (Log.IsDebugEnabled) Log.Debug("subscribing to " + subject);
            if (_pixieConnection == null)
            {
                InapiEventHandler.OnSubscriptionStatus(subject, SubscriptionStatus.STALE,
                    "Pixie connection is down");
            }
            else
            {
                _pixieConnection.Subscribe(subject);
            }
        }

        public void Unsubscribe(string subject)
        {
            if (Log.IsDebugEnabled) Log.Debug("unsubscribing from " + subject);
            if (_pixieConnection != null)
            {
                _pixieConnection.Unsubscribe(subject);
            }
        }

        public void Start()
        {
            if (InapiEventHandler == null) throw new IllegalStateException("set event handler before starting plugin");
            if (_running.CompareAndSet(false, true))
            {
                _outputThread.Start();
            }
        }

        public bool Running
        {
            get { return _running.Value; }
        }

        public void Stop()
        {
            if (_running.CompareAndSet(true, false))
            {
                if (_pixieConnection != null)
                {
                    _pixieConnection.Close(Name + " stopped");
                }
                NotifyStatusChange(ProviderStatus.Closed, "client closed connection");
            }
        }

        private void HandshakeWithServer()
        {
            try
            {
                Log.Info("opening socket to " + Host + ':' + Port);
                var client = new TcpClient(Host, Port);
                _stream = client.GetStream();
                if (Tunnel)
                {
                    UpgradeToSsl();
                    TunnelToHighway();
                }
                else
                {
                    SendPixieUrl();
                }
                var welcome = ReadWelcomeMessage();
                Log.Info("After sending URL signature, received welcome: " + welcome);
                _negotiatedVersion = welcome.Version;
                var login = new LoginMessage
                {
                    Username = Username,
                    Password = Password,
                    Alias = ServiceProperties.Username(),
                    Application = NAPI.Name,
                    ApplicationVersion = NAPI.Version
                };
                WriteMesasge(login);
                var grantMessage = ReadGrantMessage();
                Log.Info("Received grant: "+grantMessage);
            }
            catch (Exception e)
            {
                Log.Warn("failed to handshake with highway server due to " + e.Message);
                NotifyStatusChange(ProviderStatus.TemporarilyDown, "failed to connect to highway server: "
                                                                   + e.Message);
                throw e;
            }
        }

        private WelcomeMessage ReadWelcomeMessage()
        {
            var memoryStream = ReadMessageFrame();
            CheckType(memoryStream, PixieMessageType.Welcome);
            return new WelcomeMessage(memoryStream);
        }

        private GrantMessage ReadGrantMessage()
        {
            var message = ReadMessageFrame();
            CheckType(message, PixieMessageType.Grant);
            return new GrantMessage(message);
        }

        private void CheckType(Stream stream, byte expectedType)
        {
            var receivedType = (byte) stream.ReadByte();
            if (receivedType != expectedType)
            {
                throw new ArgumentException("received a message of type " + (char)receivedType + " when expecting a message of type " + (char) expectedType);
            }
        }

        private void WriteMesasge(IPixieMessage message)
        {
            var memoryStream = message.Encode(_negotiatedVersion);
            var frameLength = Convert.ToInt32(memoryStream.Length);
            var buffer = memoryStream.GetBuffer();
            Varint.WriteU32(_stream, frameLength);
            _stream.Write(buffer, 0, frameLength);
            _stream.Flush();
        }

        private MemoryStream ReadMessageFrame()
        {
            var frameLength = Varint.ReadU32(_stream);
            if (frameLength == 0) throw new IOException("unexpected empty Pixie message frame");
            byte[] frameBuffer = new byte[frameLength];
            int totalRead = 0;
            while (totalRead < frameLength)
            {
                int got = _stream.Read(frameBuffer, totalRead, frameLength - totalRead);
                if (got == -1)
                {
                    throw new IOException("end of message stream reached (perhaps the server closed the connection)");
                }
                totalRead += got;
            }
            return new MemoryStream(frameBuffer);
        }

        private void UpgradeToSsl()
        {
            var sslStream = DisableHostnameSslChecks
                ? new SslStream(_stream, false, AllowCertsFromTS)
                : new SslStream(_stream, false);
            sslStream.AuthenticateAsClient(Host);
            if (sslStream.IsAuthenticated)
            {
                _stream = sslStream;
                if (Log.IsDebugEnabled) Log.Debug(Name + " upgraded stream to SSL");
            }
            else
            {
                throw new TunnelException(Name + " failed to upgrade stream to SSL, cannot tunnel to puffin");
            }
        }

        private bool AllowCertsFromTS(Object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors errors)
        {
            return chain.ChainStatus.Length == 0 && Regex.IsMatch(cert.Subject, ".*CN=.*\\.tradingscreen\\.com.*");
        }

        private void TunnelToHighway()
        {
            SendTunnelHeader();
            SendPixieUrl();
            ReadTunnelResponse();
        }

        private void SendTunnelHeader()
        {
            var auth = Convert.ToBase64String(Encoding.ASCII.GetBytes(Username + ':' + Password));
            SendMessage("CONNECT " + Service + " HTTP/1.1\r\nAuthorization: Basic " + auth + "\r\n" +
                        "GUID: " + _guid + "\r\n\r\n");
        }

        private void SendPixieUrl()
        {
            SendMessage("pixie://" + Username + "@localhost:9902?version=3&heartbeat=30&idle=60&minti=100\n");
        }

        private void ReadTunnelResponse()
        {
            var response = _buffer.ReadLineFromStream(_stream);
            if (Log.IsDebugEnabled)
            {
                Log.Debug("received: " + response);
            }
            if (!"HTTP/1.1 200 OK".Equals(response) || _buffer.ReadLineFromStream(_stream).Length != 0)
            {
                const string prefix = "HTTP/1.1 ";
                if (response.StartsWith(prefix))
                {
                    response = response.Substring(prefix.Length, response.Length - prefix.Length);
                }
                throw new TunnelException("tunnel rejected with response: " + response);
            }
        }


        private void SendMessage(string message)
        {
            if (Log.IsDebugEnabled)
            {
                Log.Debug(Name + " sending: " + message);
            }
            _stream.Write(Encoding.ASCII.GetBytes(message), 0, message.Length);
            _stream.Flush();
        }

        // TODO implement subject check
        public bool IsSubjectCompatible(string subject)
        {
            return true;
        }

        private void NotifyStatusChange(ProviderStatus status, string reason)
        {
            var previousStatus = ProviderStatus;
            if (previousStatus == status && string.Equals(StatusReason, reason)) return;
            ProviderStatus = status;
            StatusReason = reason;
            InapiEventHandler.OnProviderStatus(this, previousStatus);
        }
    }
}