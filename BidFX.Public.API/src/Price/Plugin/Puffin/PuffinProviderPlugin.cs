/// Copyright (c) 2018 BidFX Systems Ltd. All Rights Reserved.

using System;
using System.IO;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Authentication;
using System.Threading;
using BidFX.Public.API.Price.Tools;
using log4net;

namespace BidFX.Public.API.Price.Plugin.Puffin
{
    /// <summary>PuffinProviderPlugin provides a NAPIClient provider plug-in that connects to
    /// a remote Puffin server to obtain prices.</summary>
    /// <remarks>
    /// Connection to Puffin is made using a TCP/IP socket connection.
    /// The Puffin server may therefore be either local to the
    /// client or on a remote machine perhaps connected via a WAN.
    /// </remarks>
    /// <author>Paul Sweeny</author>
    /// <author>Paul MacDonald</author>
    internal class PuffinProviderPlugin : IProviderPlugin
    {
        private static readonly ILog Log =
            LogManager.GetLogger("PuffinProviderPlugin");

        public const int ProtocolVersion = 8;

        public string Name { get; private set; }
        public ProviderStatus ProviderStatus { get; private set; }
        public string StatusReason { get; private set; }
        public IApiEventHandler InapiEventHandler { get; set; }

        public LoginService LoginService { get; set; }
        public string Service { get; set; }
        public TimeSpan ReconnectInterval { get; set; }
        public bool Tunnel { get; set; }
        public bool DisableHostnameSslChecks { get; set; }

        private readonly GUID _guid = new GUID();
        private readonly AtomicBoolean _running = new AtomicBoolean(false);
        private readonly Thread _outputThread;
        private readonly ByteBuffer _buffer = new ByteBuffer();
        private Stream _stream;
        private PuffinConnection _puffinConnection;
        private long _startTime;

        public PuffinProviderPlugin()
        {
            string name =
                NameCache.Default().CreateUniqueName("PuffinProviderPlugin");
            Name = name;
            ProviderStatus = ProviderStatus.TemporarilyDown;
            StatusReason = "not started";
            Service = "static://puffin";
            _outputThread = new Thread(RunningLoop) {Name = name};
        }

        private void NotifyStatusChange(ProviderStatus status, string reason)
        {
            ProviderStatus previousStatus = ProviderStatus;
            if (previousStatus == status && string.Equals(StatusReason, reason))
            {
                return;
            }

            ProviderStatus = status;
            StatusReason = reason;
            InapiEventHandler.OnProviderStatus(this, previousStatus);
        }

        public void Subscribe(Subject.Subject subject, bool autoRefresh = false, bool refresh = false)
        {
            if (Log.IsDebugEnabled)
            {
                Log.Debug("subscribing to " + subject);
            }
            
            if (_puffinConnection == null)
            {
                InapiEventHandler.OnSubscriptionStatus(subject, SubscriptionStatus.STALE,
                    "Puffin price server connection is down");
            }
            else
            {
                _puffinConnection.Subscribe(subject);
            }
        }

        public void Unsubscribe(Subject.Subject subject)
        {
            if (Log.IsDebugEnabled)
            {
                Log.Debug("unsubscribing from " + subject);
            }

            if (_puffinConnection != null)
            {
                _puffinConnection.Unsubscribe(subject);
            }
        }

        public bool IsSubjectCompatible(Subject.Subject subject)
        {
            // TODO use a subject filter to route between plugins
            return subject.GetComponent("Source") != null;
        }

        public void Start()
        {
            if (InapiEventHandler == null)
            {
                throw new IllegalStateException("set event handler before starting plugin");
            }

            if (!LoginService.LoggedIn)
            {
                throw new IllegalStateException("must be logged in before starting plugin");
            }

            if (_running.CompareAndSet(false, true))
            {
                LoginService.OnForcedDisconnectEventHandler += OnForcedDisconnect;
                _startTime = JavaTime.CurrentTimeMillis();
                _outputThread.Start();
            }
        }

        public void Stop()
        {
            ForcedDisconnect("client closed connection");
        }

        public bool Running
        {
            get { return _running.Value; }
        }

        public bool Ready
        {
            get { return Running && ProviderStatus.Ready.Equals(ProviderStatus); }
        }

        private void RunningLoop()
        {
            Log.Info("started thread for GUID " + _guid);
            if (IsConfigured())
            {
                while (_running.Value)
                {
                    ManagePuffinConnection();
                    _buffer.Clear();
                    if (_running.Value)
                    {
                        Log.Info(Name + " will try reconnecting in " + ReconnectInterval);
                        Thread.Sleep(ReconnectInterval);
                    }
                }
            }

            Log.Info("thread stopped");
        }

        private bool IsConfigured()
        {
            if (LoginService == null)
            {
                NotifyStatusChange(ProviderStatus.Invalid, "Login Provider has not been set");
            }
            
            if (LoginService.Host == null)
            {
                NotifyStatusChange(ProviderStatus.Invalid, "Host property has not been set");
                return false;
            }

            if (LoginService.Username == null)
            {
                NotifyStatusChange(ProviderStatus.Invalid, "Username property has not been set");
                return false;
            }

            if (LoginService.Password == null)
            {
                NotifyStatusChange(ProviderStatus.Invalid, "Password property has not been set");
                return false;
            }

            return true;
        }

        private void ManagePuffinConnection()
        {
            try
            {
                TimeSpan heartbeatInterval = HandshakeWithServer();
                _puffinConnection = new PuffinConnection(_stream, this, heartbeatInterval);
                NotifyStatusChange(ProviderStatus.Ready, "connected to Puffin price server");
                _puffinConnection.ProcessIncommingMessages();
                NotifyStatusChange(ProviderStatus.TemporarilyDown, "lost connection to Puffin price server");
            }
            catch (Exception e)
            {
                if (Log.IsDebugEnabled)
                {
                    Log.Debug("connection terminated by " + e.Message);
                }
            }
        }

        private TimeSpan HandshakeWithServer()
        {
            try
            {
                Log.Info("opening socket to " + LoginService.Host + ':' + LoginService.Port);
                TcpClient client = new TcpClient(LoginService.Host, LoginService.Port);
                _stream = client.GetStream();
                if (Tunnel)
                {
                    ConnectionTools.UpgradeToSsl(ref _stream, LoginService.Host, DisableHostnameSslChecks);
                    string tunnelHeader = ConnectionTools.CreateTunnelHeader(LoginService.Username, LoginService.Password, Service, _guid);
                    ConnectionTools.SendMessage(_stream, tunnelHeader);
                    SendPuffinUrl();
                    ConnectionTools.ReadTunnelResponse(_stream);
                }
                else
                {
                    SendPuffinUrl();
                }

                string welcome = ReadMessage();
                string publicKey = FieldExtractor.Extract(welcome, "PublicKey");
                string encryptedPassword = LoginEncryption.EncryptWithPublicKey(publicKey, LoginService.Password);
                ConnectionTools.SendMessage(_stream, new PuffinElement(PuffinTagName.Login)
                    .AddAttribute("Alias", ServiceProperties.Username())
                    .AddAttribute("Name", LoginService.Username)
                    .AddAttribute("Password", encryptedPassword)
                    .AddAttribute("Description", PublicApi.Name)
                    .AddAttribute("Version", ProtocolVersion)
                    .ToString());
                string grant = ReadMessage();
                if (!Convert.ToBoolean(FieldExtractor.Extract(grant, "Access")))
                {
                    throw new AuthenticationException("Access was not granted: "
                                                      + FieldExtractor.Extract(grant, "Text"));
                }

                ReadMessage(); //Service description
                ConnectionTools.SendMessage(_stream, new PuffinElement(PuffinTagName.ServiceDescription)
                    .AddAttribute("GUID", _guid.ToString())
                    .AddAttribute("server", false)
                    .AddAttribute("discoverable", true)
                    .AddAttribute("startTime", _startTime)
                    .AddAttribute("username", LoginService.Username)
                    .AddAttribute("userAlias", ServiceProperties.Username())
                    .AddAttribute("name", PublicApi.Name)
                    .AddAttribute("package", PublicApi.Package)
                    .AddAttribute("version", PublicApi.Version)
                    .AddAttribute("protocolVersion", ProtocolVersion)
                    .AddAttribute("environment", ServiceProperties.Environment(LoginService.Host))
                    .AddAttribute("city", ServiceProperties.City())
                    .AddAttribute("locale", ServiceProperties.Locale())
                    .AddAttribute("host", ServiceProperties.Host())
                    .ToString());
                return TimeSpan.FromMilliseconds(int.Parse(FieldExtractor.Extract(welcome, "Interval")));
            }
            catch (Exception e)
            {
                Log.Warn("failed to handshake with Puffin price server due to " + e.Message);
                if (e.Message.Contains("401 Unauthorized"))
                {
                    NotifyStatusChange(ProviderStatus.Unauthorized, "Invalid Credentials: "
                                                                       + e.Message);
                    _running.SetValue(false);
                }
                else
                {
                    NotifyStatusChange(ProviderStatus.TemporarilyDown, "failed to connect to Puffin price server: "
                                                                       + e.Message);
                }
                throw e;
            }
        }

        private void SendPuffinUrl()
        {
            ConnectionTools.SendMessage(_stream,
                "puffin://" + LoginService.Username + "@puffin?encrypt=false&zipPrices=true&zipRequests=false\n");
        }

        private string ReadMessage()
        {
            string message = _buffer.ReadXmlFromStream(_stream);
            if (Log.IsDebugEnabled)
            {
                Log.Debug(Name + " received: " + message);
            }

            return message;
        }

        private void OnForcedDisconnect(object sender, DisconnectEventArgs e)
        {
            ForcedDisconnect(e.Reason);
        }
        
        private void ForcedDisconnect(string reason)
        {
            if (_running.CompareAndSet(true, false))
            {
                if (_puffinConnection != null)
                {
                    _puffinConnection.Close(Name + " stopped");
                }

                NotifyStatusChange(ProviderStatus.Closed, reason);
                LoginService.OnForcedDisconnectEventHandler -= OnForcedDisconnect;
            }
        }
    }
}