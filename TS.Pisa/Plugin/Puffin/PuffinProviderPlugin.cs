using System.Net.Sockets;
using System.Text;
using System.Threading;
using TS.Pisa.Tools;
using System;
using System.IO;
using System.Net.Security;


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
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public string Name { get; }
        public ProviderStatus ProviderStatus { get; }
        public string ProviderStatusText { get; }

        public string Host { get; set; }
        public int Port { get; set; }
        public string Service { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public GUID Guid { get; } = new GUID();

        private readonly AtomicBoolean _running = new AtomicBoolean(false);
        private readonly Thread _outputThread;
        private readonly ByteBuffer _buffer = new ByteBuffer();
        private SslStream _stream;


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
            _outputThread = new Thread(RunningLoop) {Name = Name + "-read"};
        }

        private void RunningLoop()
        {
            log.Info("starting thread for GUID " + Guid);
            while (_running.Value)
            {
                log.Info(Name + " running");
                EstablishPuffinConnection();
                _stream.Close();
                _buffer.Clear();
                Thread.Sleep(10000);
            }
            log.Info("thread stopped");
        }


        public void Subscribe(string subject)
        {
            log.Info(Name + " subscribing to " + subject);
        }

        public void Unsubscribe(string subject)
        {
            log.Info(Name + " unsubscribing from " + subject);
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
                log.Info("opening socket to " + Host + ':' + Port);
                var client = new TcpClient(Host, Port);
                _stream = new SslStream(client.GetStream(), false);
                _stream.AuthenticateAsClient(Host);
                TunnelToPuffin();
                SendPuffinProtocolSignature();
                ReadPuffinWelcomeMessage();
                SendPuffinLogin();
            }
            catch (Exception e)
            {
                log.Warn("failed to read from socket", e);
            }
        }

        private void TunnelToPuffin()
        {
            SendTunnelHeader();
            ReadTunnelResponse();
        }

        private void SendPuffinProtocolSignature()
        {
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
            var header = "CONNECT " + Service + " HTTP/1.1\r\nAuthorization: Basic " + auth + "\r\n" +
                         "GUID: " + Guid + "\r\n\r\n";
            if (log.IsDebugEnabled)
            {
                log.Debug("sending tunnel header: " + header);
            }
            _stream.Write(Encoding.ASCII.GetBytes(header));
        }

        private void ReadTunnelResponse()
        {
            var response = _buffer.ReadLineFrom(_stream);
            log.Info("received response from tunnel '" + response + "'");
            if (!"HTTP/1.1 200 OK".Equals(response) || _buffer.ReadLineFrom(_stream).Length != 0)
            {
                throw new TunnelException("failed to tunnel to Puffin with response: " + response);
            }
        }
    }
}