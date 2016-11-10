using System.Net.Sockets;
using System.Text;
using System.Threading;
using TS.Pisa.Tools;
using System;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography;
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
    public class PuffinProviderPlugin : IProviderPlugin
    {
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public string Name { get; }
        public ProviderStatus ProviderStatus { get; }
        public string ProviderStatusText { get; }

        public string Host { get; set; }
        public int Port { get; set; }
        public string Service { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool Tunnel { get; set; }
        public GUID Guid { get; } = new GUID();

        private readonly AtomicBoolean _running = new AtomicBoolean(false);
        private readonly Thread _outputThread;
        private readonly ByteBuffer _buffer = new ByteBuffer();
        private Stream _stream;


        public PuffinProviderPlugin()
        {
            Name = NameCache.Default().CreateUniqueName(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            ProviderStatus = ProviderStatus.TemporarilyDown;
            ProviderStatusText = "waiting for remote connection";
            Host = "ny-tunnel.uatdev.tradingscreen.com";
            Port = 443;
            Service = "static://puffin";
            Username = "username_unknown";
            Password = "password_unset";
            Tunnel = true;
            _outputThread = new Thread(RunningLoop) {Name = Name + "-read"};

        }

        private void RunningLoop()
        {
            Log.Info("starting thread for GUID " + Guid);
            while (_running.Value)
            {
                Log.Info(Name + " running");
                EstablishPuffinConnection();
                _stream.Close();
                _buffer.Clear();
                Thread.Sleep(10000);
            }
            Log.Info("thread stopped");
        }


        public void Subscribe(string subject)
        {
            Log.Info(Name + " subscribing to " + subject);
        }

        public void Unsubscribe(string subject)
        {
            Log.Info(Name + " unsubscribing from " + subject);
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
                    UpgradeToSSL();
                    TunnelToPuffin();
                }
                else
                {
                    SendPuffinURL();
                }
                var welcomeMessage = ReadMessage();
                XmlDocument welcomeXML = new XmlDocument();
                welcomeXML.LoadXml(welcomeMessage);
                XmlNode selectSingleNode = welcomeXML.SelectSingleNode("Welcome");
                var publicKey = selectSingleNode.Attributes["PublicKey"].InnerText;
                var encryptedPassword = LoginEncryption.EncryptWithPublicKey(publicKey, Password);
                SendMessage("<Login Alias=\""+Username+"\" Name=\""+Username+"\" Password=\""+encryptedPassword+"\" Description=\"PuffinNET\" Version=\"8\"/>");
                ReadMessage();
            }
            catch (Exception e)
            {
                Log.Warn("failed to read from socket", e);
            }
        }

        private void UpgradeToSSL()
        {
            Log.Info("upgrading to SSL");
            var sslStream = new SslStream(_stream, false);
            sslStream.AuthenticateAsClient(Host);
            _stream = sslStream;
        }

        private void TunnelToPuffin()
        {
            SendTunnelHeader();
            SendPuffinURL();
            ReadTunnelResponse();
        }

        private void SendPuffinURL()
        {
            SendMessage("puffin://"+Username+"@puffin:9901\n");
        }

        private void ReadPuffinWelcomeMessage()
        {
        }

        private void SendPuffinLogin()
        {
        }

        private void SendTunnelHeader()
        {
            var auth = Convert.ToBase64String(Encoding.ASCII.GetBytes(Username + ':' + Password));
            SendMessage("CONNECT " + Service + " HTTP/1.1\r\nAuthorization: Basic " + auth + "\r\n" +
                         "GUID: " + Guid + "\r\n\r\n");
        }

        private void ReadTunnelResponse()
        {
            var response = _buffer.ReadLineFrom(_stream);
            Log.Info("received response from tunnel '" + response + "'");
            if (!"HTTP/1.1 200 OK".Equals(response) || _buffer.ReadLineFrom(_stream).Length != 0)
            {
                throw new TunnelException("failed to tunnel to Puffin with response: " + response);
            }
        }

        private void SendMessage(string message)
        {
            if (Log.IsDebugEnabled)
            {
                Log.Debug("sending: " + message);
            }
            _stream.Write(Encoding.ASCII.GetBytes(message),0,message.Length);
            _stream.Flush();
        }

        private string ReadMessage()
        {
            var message = _buffer.ReadUntil(_stream, '>');
            if (Log.IsDebugEnabled)
            {
                Log.Debug("received: " + message);
            }
            return message;
        }
    }
}