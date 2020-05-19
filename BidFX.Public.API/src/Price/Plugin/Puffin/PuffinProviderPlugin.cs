/// Copyright (c) 2018 BidFX Systems Ltd. All Rights Reserved.

using System;
using System.IO;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Authentication;
using System.Threading;
using BidFX.Public.API.Price.Tools;
using Serilog;

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
        private static readonly ILogger Log = Serilog.Log.ForContext<PuffinProviderPlugin>();
        public const int ProtocolVersion = 8;

        public string Name { get; private set; }
        public ProviderStatus ProviderStatus { get; private set; }
        public string StatusReason { get; private set; }
        public IApiEventHandler InapiEventHandler { get; set; }

        public UserInfo UserInfo { get; set; }
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
            Log.Debug("subscribing to {subject}", subject);
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
            Log.Debug("unsubscribing from {subject}", subject);

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

            if (_running.CompareAndSet(false, true))
            {
                _startTime = JavaTime.CurrentTimeMillis();
                _outputThread.Start();
            }
        }

        public void Stop()
        {
            if (_puffinConnection != null)
            {
                _puffinConnection.Close(Name + " stopped");
            }

            NotifyStatusChange(ProviderStatus.Closed, Name + " stopped");
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
            Log.Information("started thread for GUID {guid}", _guid);
            if (IsConfigured())
            {
                while (_running.Value)
                {
                    ManagePuffinConnection();
                    _buffer.Clear();
                    if (_running.Value)
                    {
                        Log.Information("{name} will try reconnecting in {reconnectInterval}", Name, ReconnectInterval);
                        Thread.Sleep(ReconnectInterval);
                    }
                }
            }

            Log.Information("thread stopped");
        }

        private bool IsConfigured()
        {
            if (UserInfo == null)
            {
                NotifyStatusChange(ProviderStatus.Invalid, "User info has not been set");
            }
            
            if (UserInfo.Host == null)
            {
                NotifyStatusChange(ProviderStatus.Invalid, "Host property has not been set");
                return false;
            }

            if (UserInfo.Username == null)
            {
                NotifyStatusChange(ProviderStatus.Invalid, "Username property has not been set");
                return false;
            }

            if (UserInfo.Password == null)
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
                Log.Warning("connection terminated by {error}", e.Message);
            }
        }

        private TimeSpan HandshakeWithServer()
        {
            try
            {
                Log.Information("opening socket to {host}:{port}", UserInfo.Host, UserInfo.Port);
                TcpClient client = new TcpClient(UserInfo.Host, UserInfo.Port);
                _stream = client.GetStream();
                if (Tunnel)
                {
                    ConnectionTools.UpgradeToSsl(ref _stream, UserInfo.Host, DisableHostnameSslChecks);
                    string tunnelHeader = ConnectionTools.CreateTunnelHeader(UserInfo.Username, UserInfo.Password, Service, _guid);
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
                string encryptedPassword = LoginEncryption.EncryptWithPublicKey(publicKey, UserInfo.Password);
                ConnectionTools.SendMessage(_stream, new PuffinElement(PuffinTagName.Login)
                    .AddAttribute("Alias", ServiceProperties.Username())
                    .AddAttribute("Name", UserInfo.Username)
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
                    .AddAttribute("username", UserInfo.Username)
                    .AddAttribute("userAlias", ServiceProperties.Username())
                    .AddAttribute("name", PublicApi.Name)
                    .AddAttribute("package", PublicApi.Package)
                    .AddAttribute("version", PublicApi.Version)
                    .AddAttribute("protocolVersion", ProtocolVersion)
                    .AddAttribute("environment", ServiceProperties.Environment(UserInfo.Host))
                    .AddAttribute("city", ServiceProperties.City())
                    .AddAttribute("locale", ServiceProperties.Locale())
                    .AddAttribute("host", ServiceProperties.Host())
                    .ToString());
                return TimeSpan.FromMilliseconds(int.Parse(FieldExtractor.Extract(welcome, "Interval")));
            }
            catch (Exception e)
            {
                Log.Warning("failed to handshake with Puffin price server due to {error}", e.Message);
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
                "puffin://" + UserInfo.Username + "@puffin?encrypt=false&zipPrices=true&zipRequests=false\n");
        }

        private string ReadMessage()
        {
            string message = _buffer.ReadXmlFromStream(_stream);
            Log.Debug("{name} received: {message}", Name, message);
            return message;
        }
    }
}