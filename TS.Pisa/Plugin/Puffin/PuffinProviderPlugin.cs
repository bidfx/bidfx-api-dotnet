using System.Net.Sockets;
using System.Text;
using System.Threading;
using TS.Pisa.Tools;
using System;
using System.IO;
using System.Net.Security;
using System.Security;
using System.Security.Authentication;
using System.Xml;

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
        public bool Tunnel { get; set; }
        public GUID Guid { get; set; }
        private readonly AtomicBoolean _running = new AtomicBoolean(false);
        private readonly Thread _outputThread;
        private readonly ByteBuffer _buffer = new ByteBuffer();
        private Stream _stream;
        private IPuffinRequestor _puffinRequestor = new NullPuffinRequestor();
        private static readonly long StartTime = JavaTime.CurrentTimeMillis();
        public EventHandler<PriceUpdateEventArgs> PriceUpdate { get; set; }

        public PuffinProviderPlugin()
        {
            Name = NameCache.Default().CreateUniqueName(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            ProviderStatus = ProviderStatus.TemporarilyDown;
            ProviderStatusText = "waiting for remote connection";
            Host = "host_unknown";
            Port = 443;
            Guid = new GUID();
            Service = "static://puffin";
            Username = ServiceProperties.Username();
            Password = "password_unset";
            Tunnel = true;
            _outputThread = new Thread(RunningLoop) {Name = Name};
        }

        private void RunningLoop()
        {
            Log.Info("starting thread for GUID " + Guid);
            while (_running.Value)
            {
                Log.Info(Name + " running");
                EstablishPuffinConnection();
                _buffer.Clear();
                Thread.Sleep(10000);
            }
            Log.Info("thread stopped");
        }

        public void Subscribe(string subject)
        {
            Log.Info(Name + " subscribing to " + subject);
            _puffinRequestor.Subscribe(subject);
        }

        public void Unsubscribe(string subject)
        {
            Log.Info(Name + " unsubscribing from " + subject);
            _puffinRequestor.Unsubscribe(subject);
        }

        public void EventListener(IEventListener listener)
        {
        }

        public bool IsSubjectCompatible(string subject)
        {
            // TODO use a subject filter to route between plugins
            return true;
        }

        public void Start()
        {
            if (_running.CompareAndSet(false, true))
            {
                _outputThread.Start();
            }
        }

        public void Stop()
        {
            _running.SetValue(false);
        }

        private void EstablishPuffinConnection()
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
                var publicKey = GetPublicKey(ReadMessage());
                var encryptedPassword = LoginEncryption.EncryptWithPublicKey(publicKey, Password);

                SendMessage(new PuffinElement(PuffinTagName.Login)
                    .AddAttribute("Alias", ServiceProperties.Username())
                    .AddAttribute("Name", Username)
                    .AddAttribute("Password", encryptedPassword)
                    .AddAttribute("Description", Pisa.Name)
                    .AddAttribute("Version", ProtocolVersion)
                    .ToString());
                if (!IsAccessGranted(ReadMessage()))
                {
                    throw new AuthenticationException("Access was not granted.");
                }
                ReadMessage();
                SendMessage(new PuffinElement(PuffinTagName.ServiceDescription)
                    .AddAttribute("GUID", Guid.ToString())
                    .AddAttribute("server", false)
                    .AddAttribute("discoverable", true)
                    .AddAttribute("startTime", StartTime)
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
                ProviderStatus = ProviderStatus.Ready;
                PuffinClient puffinClient = new PuffinClient(_stream, Name, PriceUpdate);
                _puffinRequestor = puffinClient;
                puffinClient.Start();
            }
            catch (Exception e)
            {
                Log.Warn("failed to read from socket", e);
            }
        }

        private string GetPublicKey(string welcomeMessage)
        {
            var welcomeXml = new XmlDocument();
            welcomeXml.LoadXml(welcomeMessage);
            var welcomeNode = welcomeXml.SelectSingleNode("Welcome");
            if (welcomeNode == null)
                throw new PuffinSyntaxException("The welcome node could not be found in the message: " + welcomeMessage);
            if (welcomeNode.Attributes != null && welcomeNode.Attributes["PublicKey"] != null)
            {
                return welcomeNode.Attributes["PublicKey"].InnerText;
            }
            throw new PuffinSyntaxException("No Public Key provided in welcome message: " + welcomeMessage);
        }

        private bool IsAccessGranted(string grantMessage)
        {
            var grantXml = new XmlDocument();
            grantXml.LoadXml(grantMessage);
            var grantNode = grantXml.SelectSingleNode("Grant");
            if (grantNode == null)
                throw new XmlSyntaxException("The grant node could not be found in the message: " + grantMessage);
            if (grantNode.Attributes != null && grantNode.Attributes["Access"] != null)
            {
                return Convert.ToBoolean(grantNode.Attributes["Access"].InnerText);
            }
            throw new XmlSyntaxException("No Access tag provided in grant message: " + grantMessage);
        }

        private string GetTextFromGrant(String grantMessage)
        {
            var grantXml = new XmlDocument();
            grantXml.LoadXml(grantMessage);
            var grantNode = grantXml.SelectSingleNode("Grant");
            if (grantNode == null)
                throw new XmlSyntaxException("The grant node could not be found in the message: " + grantMessage);
            if (grantNode.Attributes != null && grantNode.Attributes["Text"] != null)
            {
                return grantNode.Attributes["Text"].InnerText;
            }
            return "";
        }

        private void UpgradeToSsl()
        {
            var sslStream = new SslStream(_stream, false);
            sslStream.AuthenticateAsClient(Host);
            if (sslStream.IsAuthenticated)
            {
                _stream = sslStream;
                Log.Info("upgraded stream to SSL");
            }
            else
            {
                throw new TunnelException("failed to upgrade stream to SSL, cannot tunnel to puffin");
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
                        "GUID: " + Guid + "\r\n\r\n");
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