using System.Net.Sockets;
using System.Text;
using System.Threading;
using TS.Pisa.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Security;
using System.Security.Authentication;

namespace TS.Pisa.Plugin.Puffin
{
    /// <summary>PuffinProviderPlugin provides a Pisa provider plug-in that connects to a Puffin server to obtain prices.</summary>
    /// <remarks>
    /// PuffinProviderPlugin provides a Pisa provider plug-in that connects to a Puffin server to obtain prices.
    /// Connection to Puffin is made using a TCP/IP socket connection. The Puffin server may therefore be either local to the
    /// client or on a remote machine perhaps connected via a WAN.
    /// </remarks>
    /// <author>Paul Sweeny</author>
    /// <author>Paul MacDonald</author>
    public class PuffinProviderPlugin : IProviderPlugin
    {
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public const int ProtocolVersion = 8;

        public string Name { get; set; }
        public ProviderStatus ProviderStatus { get; set; }
        public string ProviderStatusText { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string Service { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public TimeSpan ReconnectInterval { get; set; }
        public bool Tunnel { get; set; }
        private readonly GUID _guid = new GUID();
        private readonly AtomicBoolean _running = new AtomicBoolean(false);
        private readonly Thread _outputThread;
        private readonly ByteBuffer _buffer = new ByteBuffer();
        private Stream _stream;
        private IPuffinRequestor _puffinRequestor = new NullPuffinRequestor();
        private long _startTime;
        private readonly HashSet<string> _subscriptions = new HashSet<string>();

        public EventHandler<ProviderPluginEventArgs> ProviderPlugin { get; set; }
        public EventHandler<PriceUpdateEventArgs> PriceUpdate { get; set; }
        public EventHandler<PriceStatusEventArgs> PriceStatus { get; set; }

        public PuffinProviderPlugin()
        {
            Name = NameCache.Default().CreateUniqueName(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            NotifyStatusChange(ProviderStatus.TemporarilyDown, "waiting for remote connection");
            Host = "host_unknown";
            Port = 443;
            Service = "static://puffin";
            Username = ServiceProperties.Username();
            Password = "password_unset";
            Tunnel = true;
            ReconnectInterval = TimeSpan.FromSeconds(10);
            _outputThread = new Thread(RunningLoop) {Name = Name};
        }

        private void NotifyStatusChange(ProviderStatus status, string reason)
        {
            ProviderStatus = status;
            ProviderStatusText = reason;
            var eventHandler = ProviderPlugin;
            if (eventHandler != null)
            {
                eventHandler(this, new ProviderPluginEventArgs
                {
                    Provider = this,
                    ProviderStatus = status,
                    ProviderStatusText = reason
                });
            }
        }

        private void RunningLoop()
        {
            Log.Info("starting thread for GUID " + _guid);
            while (_running.Value)
            {
                Log.Info(Name + " running");
                EstablishPuffinConnection();
                _buffer.Clear();
                Log.Info(Name + " will try reconnecting in " + ReconnectInterval);
                Thread.Sleep(ReconnectInterval);
            }
            Log.Info("thread stopped");
        }

        public void Subscribe(string subject)
        {
            lock (_subscriptions)
            {
                _subscriptions.Add(subject);
                _puffinRequestor.Subscribe(subject);
            }
        }

        public void Unsubscribe(string subject)
        {
            lock (_subscriptions)
            {
                _subscriptions.Remove(subject);
                _puffinRequestor.Unsubscribe(subject);
            }
        }

        public bool IsSubjectCompatible(string subject)
        {
            // TODO use a subject filter to route between plugins
            // Specific restriction for AXA
            var isSubjectCompatible = subject.Contains("AssetClass=FixedIncome") && subject.Contains("Source=Lynx");
            if (!isSubjectCompatible)
            {
                Log.Warn("Subject is not compatible: " + subject);
            }
            return isSubjectCompatible;
        }

        public void Start()
        {
            if (_running.CompareAndSet(false, true))
            {
                _startTime = JavaTime.CurrentTimeMillis();
                _outputThread.Start();
            }
        }

        public void Stop()
        {
            _running.SetValue(false);
            _puffinRequestor.CloseSession();
            NotifyStatusChange(ProviderStatus.Closed, "client closed connection");
            lock (_subscriptions)
            {
                _subscriptions.Clear();
            }
        }

        private void EstablishPuffinConnection()
        {
            var heartbeatInterval = HandshakeWithServer();
            var puffinClient = new PuffinClient(_stream, this)
            {
                HeartbeatInterval = heartbeatInterval
            };
            NotifyStatusChange(ProviderStatus.Ready, "connected to Puffin");
            _puffinRequestor = puffinClient;
            ResubscribeToAll();
            puffinClient.Start();
            NotifyStatusChange(ProviderStatus.TemporarilyDown, "lost connection - trying to reconnect in 10 seconds");
        }

        private int HandshakeWithServer()
        {
            try
            {
                Log.Info("opening socket to " + Host + ':' + Port);
                var client = new TcpClient(Host, Port);
                _stream = client.GetStream();
                if (Tunnel)
                {
                    UpgradeToSsl();
                    TunnelToPuffin();
                }
                else
                {
                    SendPuffinUrl();
                }
                var welcomeMessage = ReadMessage();
                var publicKey = AttributeValue(welcomeMessage, "PublicKey");
                var encryptedPassword = LoginEncryption.EncryptWithPublicKey(publicKey, Password);
                SendMessage(new PuffinElement(PuffinTagName.Login)
                    .AddAttribute("Alias", ServiceProperties.Username())
                    .AddAttribute("Name", Username)
                    .AddAttribute("Password", encryptedPassword)
                    .AddAttribute("Description", Pisa.Name)
                    .AddAttribute("Version", ProtocolVersion)
                    .ToString());
                var grantMessage = ReadMessage();
                if (!Convert.ToBoolean(AttributeValue(grantMessage, "Access")))
                {
                    throw new AuthenticationException("Access was not granted: " + AttributeValue(grantMessage, "Text"));
                }
                ReadMessage(); //Service description
                SendMessage(new PuffinElement(PuffinTagName.ServiceDescription)
                    .AddAttribute("GUID", _guid.ToString())
                    .AddAttribute("server", false)
                    .AddAttribute("discoverable", true)
                    .AddAttribute("startTime", _startTime)
                    .AddAttribute("username", Username)
                    .AddAttribute("userAlias", ServiceProperties.Username())
                    .AddAttribute("name", Pisa.Name)
                    .AddAttribute("package", Pisa.Package)
                    .AddAttribute("version", Pisa.Version)
                    .AddAttribute("protocolVersion", ProtocolVersion)
                    .AddAttribute("environment", ServiceProperties.Environment(Host))
                    .AddAttribute("city", ServiceProperties.City())
                    .AddAttribute("locale", ServiceProperties.Locale())
                    .AddAttribute("host", ServiceProperties.Host())
                    .ToString());
                return Convert.ToInt32(AttributeValue(welcomeMessage, "Interval"));
            }
            catch (Exception e)
            {
                Log.Warn("failed to handshake with Puffin", e);
                NotifyStatusChange(ProviderStatus.TemporarilyDown, "failed to connect to Puffin");
                throw e;
            }
        }

        private void ResubscribeToAll()
        {
            // TODO this should not be here but in the Master Pisa Session
            lock (_subscriptions)
            {
                foreach (var subject in _subscriptions)
                {
                    _puffinRequestor.Subscribe(subject);
                }
            }
        }

        private static string AttributeValue(string message, string key)
        {
            var startOfKey = message.IndexOf(key, StringComparison.Ordinal);
            var startOfValue = startOfKey + key.Length + 2;
            var substring = message.Substring(startOfValue);
            var endOfValue = substring.IndexOf("\"", StringComparison.Ordinal);

            var value = substring.Substring(0, endOfValue);
            if (Log.IsDebugEnabled) Log.Debug("Value of " + key + " is:" + value);
            return value;
        }

        private void UpgradeToSsl()
        {
            var sslStream = new SslStream(_stream, false);
            sslStream.AuthenticateAsClient(Host);
            if (sslStream.IsAuthenticated)
            {
                _stream = sslStream;
                Log.Info(Name + " upgraded stream to SSL");
            }
            else
            {
                throw new TunnelException(Name + " failed to upgrade stream to SSL, cannot tunnel to puffin");
            }
        }

        private void TunnelToPuffin()
        {
            SendTunnelHeader();
            SendPuffinUrl();
            ReadTunnelResponse();
        }

        private void SendPuffinUrl()
        {
            SendMessage("puffin://" + Username + "@puffin:9901?encrypt=false&zipPrices=true&zipRequests=false\n");
        }

        private void SendTunnelHeader()
        {
            var auth = Convert.ToBase64String(Encoding.ASCII.GetBytes(Username + ':' + Password));
            SendMessage("CONNECT " + Service + " HTTP/1.1\r\nAuthorization: Basic " + auth + "\r\n" +
                        "GUID: " + _guid + "\r\n\r\n");
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
                throw new TunnelException("failed to tunnel to Puffin with response: " + response);
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

        private string ReadMessage()
        {
            var message = _buffer.ReadXmlFromStream(_stream);
            if (Log.IsDebugEnabled)
            {
                Log.Debug(Name + " received: " + message);
            }
            return message;
        }
    }
}