using System;
using System.IO;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Authentication;
using System.Threading;
using BidFX.Public.API.Price.Plugin.Pixie.Messages;
using BidFX.Public.API.Price.Subject;
using BidFX.Public.API.Price.Tools;
using log4net;

namespace BidFX.Public.API.Price.Plugin.Pixie
{
    internal class PixieProviderPlugin : IProviderPlugin
    {
        private static readonly ILog Log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Thread _outputThread;
        private readonly AtomicBoolean _running = new AtomicBoolean(false);
        private readonly GUID _guid = new GUID();
        private readonly PixieProtocolOptions _protocolOptions = new PixieProtocolOptions();

        public string Name { get; private set; }
        public string Service { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public bool Tunnel { set; get; }
        public bool DisableHostnameSslChecks { get; set; }
        public ProviderStatus ProviderStatus { get; private set; }
        public IApiEventHandler InapiEventHandler { get; set; }
        public TimeSpan ReconnectInterval { get; set; }
        public string StatusReason { get; private set; }

        private PixieConnection _pixieConnection;
        private Stream _stream;

        public PixieProviderPlugin()
        {
            var name =
                NameCache.Default().CreateUniqueName(MethodBase.GetCurrentMethod().DeclaringType);
            Name = name;
            ProviderStatus = ProviderStatus.TemporarilyDown;
            StatusReason = "not started";
            Service = "static://highway";
            _outputThread = new Thread(RunningLoop) {Name = name};
        }

        public void Subscribe(Subject.Subject subject, bool refresh = false)
        {
            if (Log.IsDebugEnabled) Log.Debug("subscribing to " + subject);
            if (_pixieConnection == null)
            {
                InapiEventHandler.OnSubscriptionStatus(subject, SubscriptionStatus.STALE,
                    "Pixie connection is down");
            }
            else
            {
                _pixieConnection.Subscribe(subject, refresh);
            }
        }

        public void Unsubscribe(Subject.Subject subject)
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

        private void RunningLoop()
        {
            Log.Info("started thread for GUID " + _guid);
            while (_running.Value)
            {
                ManagePixieConnection();
                if (_running.Value)
                {
                    Log.Info(Name + " will try reconnecting in " + ReconnectInterval);
                    Thread.Sleep(ReconnectInterval);
                }
            }
            Log.Info("thread stopped");
        }

        private void ManagePixieConnection()
        {
            try
            {
                HandshakeWithServer();
                _pixieConnection = new PixieConnection(_stream, this, _protocolOptions);
                NotifyStatusChange(ProviderStatus.Ready, "connected to Pixie price server");
                _pixieConnection.ProcessIncommingMessages();
                NotifyStatusChange(ProviderStatus.TemporarilyDown, "lost connection to Pixie price server");
            }
            catch (Exception e)
            {
                if (Log.IsDebugEnabled) Log.Debug("connection terminated by " + e.Message);
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
                    ConnectionTools.UpgradeToSsl(ref _stream, Host, DisableHostnameSslChecks);
                    var tunnelHeader = ConnectionTools.CreateTunnelHeader(Username, Password, Service, _guid);
                    ConnectionTools.SendMessage(_stream, tunnelHeader);
                    ConnectionTools.SendMessage(_stream, _protocolOptions.GetProtocolSignature());
                    ConnectionTools.ReadTunnelResponse(_stream);
                }
                else
                {
                    ConnectionTools.SendMessage(_stream, _protocolOptions.GetProtocolSignature());
                }
                _protocolOptions.ConfigureStream(_stream);
                var welcome = ReadWelcomeMessage();
                Log.Info("After sending URL signature, received welcome: " + welcome);
                _protocolOptions.Version = (int) welcome.Version;
                var login = new LoginMessage(Username, Password, ServiceProperties.Username(), PublicApi.Name,
                    PublicApi.Version);
                WriteFrame(login);
                var grantMessage = ReadGrantMessage();
                Log.Info("Received grant: " + grantMessage);
                if (!grantMessage.Granted)
                    throw new AuthenticationException("Access was not granted: " + grantMessage.Reason);
                Log.Info("Authenticated with Pixie server, client is logged in.");
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

        private static void CheckType(Stream stream, byte expectedType)
        {
            var receivedType = (byte) stream.ReadByte();
            if (receivedType != expectedType)
            {
                throw new ArgumentException("received a message of type " + (char) receivedType +
                                            " when expecting a message of type " + (char) expectedType);
            }
        }

        private void WriteFrame(IOutgoingPixieMessage message)
        {
            var encodedMessage = message.Encode(_protocolOptions.Version);
            var frameLength = Convert.ToInt32(encodedMessage.Length);
            var buffer = encodedMessage.GetBuffer();
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
                int got = _stream.Read(frameBuffer, totalRead, (int) frameLength - totalRead);
                if (got == -1)
                {
                    throw new IOException("end of message stream reached (perhaps the server closed the connection)");
                }
                totalRead += got;
            }
            return new MemoryStream(frameBuffer);
        }

        // TODO implement subject check
        public bool IsSubjectCompatible(Subject.Subject subject)
        {
            return subject.LookupValue("Source") == null;
        }

        private void NotifyStatusChange(ProviderStatus status, string reason)
        {
            var previousStatus = ProviderStatus;
            if (previousStatus == status && string.Equals(StatusReason, reason)) return;
            ProviderStatus = status;
            StatusReason = reason;
            InapiEventHandler.OnProviderStatus(this, previousStatus);
        }

        public PixieProtocolOptions PixieProtocolOptions
        {
            get { return _protocolOptions; }
        }
    }
}