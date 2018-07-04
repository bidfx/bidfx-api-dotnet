﻿/// Copyright (c) 2018 BidFX Systems Ltd. All Rights Reserved.

using System;
using System.IO;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Authentication;
using System.Text.RegularExpressions;
using System.Threading;
using BidFX.Public.API.Price.Plugin.Pixie.Messages;
using BidFX.Public.API.Price.Subject;
using BidFX.Public.API.Price.Tools;
using log4net;

namespace BidFX.Public.API.Price.Plugin.Pixie
{
    internal class PixieProviderPlugin : IProviderPlugin
    {
#if DEBUG
private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
#else
        private static readonly ILog Log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
#endif

        private readonly Thread _outputThread;
        private readonly AtomicBoolean _running = new AtomicBoolean(false);
        private readonly GUID _guid = new GUID();
        private readonly PixieProtocolOptions _protocolOptions = new PixieProtocolOptions();
        private readonly Regex _settlementDateRegex = new Regex("^2[0-9][0-9][0-9](0[1-9]|1[0-2])(0[1-9]|[1-2][0-9]|3[0-1])$");
        private readonly Regex _tenorRegex = new Regex("^((BD|SPOT(_NEXT)?|TO[DM])|[1-4]W|([1-9]|[1-2][0-9]|3[0-6])M|([1-9]|1[0-9]|20)Y|IMM[HMUZ]|BMF[UVXZFGHJKMNQ])$");

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
            string name =
                NameCache.Default().CreateUniqueName(MethodBase.GetCurrentMethod().DeclaringType);
            Name = name;
            ProviderStatus = ProviderStatus.TemporarilyDown;
            StatusReason = "not started";
            Service = "static://highway";
            _outputThread = new Thread(RunningLoop) {Name = name};
        }

        public void Subscribe(Subject.Subject subject, bool autoRefresh = false, bool refresh = false)
        {
            if (Log.IsDebugEnabled)
            {
                Log.Debug("subscribing to " + subject);
            }

            if (!ValidateSubject(subject))
            {
                return;
            }
            
            if (_pixieConnection == null)
            {
                InapiEventHandler.OnSubscriptionStatus(subject, SubscriptionStatus.STALE,
                    "Pixie connection is down");
            }
            else
            {
                _pixieConnection.Subscribe(subject, autoRefresh, refresh);
            }
        }

        public void Unsubscribe(Subject.Subject subject)
        {
            if (Log.IsDebugEnabled)
            {
                Log.Debug("unsubscribing from " + subject);
            }

            if (_pixieConnection != null)
            {
                _pixieConnection.Unsubscribe(subject);
            }
        }

        public void Start()
        {
            if (InapiEventHandler == null)
            {
                throw new IllegalStateException("set event handler before starting plugin");
            }

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
                if (Log.IsDebugEnabled)
                {
                    Log.Debug("connection terminated by " + e.Message);
                }
            }
        }


        private void HandshakeWithServer()
        {
            try
            {
                Log.Info("opening socket to " + Host + ':' + Port);
                TcpClient client = new TcpClient(Host, Port);
                _stream = client.GetStream();
                if (Tunnel)
                {
                    ConnectionTools.UpgradeToSsl(ref _stream, Host, DisableHostnameSslChecks);
                    string tunnelHeader = ConnectionTools.CreateTunnelHeader(Username, Password, Service, _guid);
                    ConnectionTools.SendMessage(_stream, tunnelHeader);
                    ConnectionTools.SendMessage(_stream, _protocolOptions.GetProtocolSignature());
                    ConnectionTools.ReadTunnelResponse(_stream);
                }
                else
                {
                    ConnectionTools.SendMessage(_stream, _protocolOptions.GetProtocolSignature());
                }

                _protocolOptions.ConfigureStream(_stream);
                WelcomeMessage welcome = ReadWelcomeMessage();
                Log.Info("After sending URL signature, received welcome: " + welcome);
                _protocolOptions.Version = (int) welcome.Version;
                LoginMessage login = new LoginMessage(Username, Password, ServiceProperties.Username(), PublicApi.Name,
                    PublicApi.Version);
                WriteFrame(login);
                GrantMessage grantMessage = ReadGrantMessage();
                Log.Info("Received grant: " + grantMessage);
                if (!grantMessage.Granted)
                {
                    throw new AuthenticationException("Access was not granted: " + grantMessage.Reason);
                }

                Log.Info("Authenticated with Pixie server, client is logged in.");
            }
            catch (Exception e)
            {
                Log.Warn("failed to handshake with highway server due to " + e.Message);
                if (e.Message.Contains("401 Unauthorized"))
                {
                    NotifyStatusChange(ProviderStatus.Unauthorized, "Invalid Credentials: "
                                                                    + e.Message);
                    _running.SetValue(false);
                }
                else
                {
                    NotifyStatusChange(ProviderStatus.TemporarilyDown, "failed to connect to highway server: "
                                                                       + e.Message);
                }
                throw e;
            }
        }

        private WelcomeMessage ReadWelcomeMessage()
        {
            MemoryStream memoryStream = ReadMessageFrame();
            CheckType(memoryStream, PixieMessageType.Welcome);
            return new WelcomeMessage(memoryStream);
        }

        private GrantMessage ReadGrantMessage()
        {
            MemoryStream message = ReadMessageFrame();
            CheckType(message, PixieMessageType.Grant);
            return new GrantMessage(message);
        }

        private static void CheckType(Stream stream, byte expectedType)
        {
            byte receivedType = (byte) stream.ReadByte();
            if (receivedType != expectedType)
            {
                throw new ArgumentException("received a message of type " + (char) receivedType +
                                            " when expecting a message of type " + (char) expectedType);
            }
        }

        private void WriteFrame(IOutgoingPixieMessage message)
        {
            MemoryStream encodedMessage = message.Encode(_protocolOptions.Version);
            int frameLength = Convert.ToInt32(encodedMessage.Length);
            byte[] buffer = encodedMessage.GetBuffer();
            Varint.WriteU32(_stream, frameLength);
            _stream.Write(buffer, 0, frameLength);
            _stream.Flush();
        }

        private MemoryStream ReadMessageFrame()
        {
            uint frameLength = Varint.ReadU32(_stream);
            if (frameLength == 0)
            {
                throw new IOException("unexpected empty Pixie message frame");
            }

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
            return subject.GetComponent("Source") == null;
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

        private bool ValidateSubject(Subject.Subject subject)
        {
            string dealType = subject.GetComponent(SubjectComponentName.DealType);
            if (dealType == null)
            {
                InapiEventHandler.OnSubscriptionStatus(subject, SubscriptionStatus.REJECTED, "DealType is null");
                return false;
            }

            if (dealType.Equals(CommonComponents.Forward) || dealType.Equals(CommonComponents.NDF) ||
                dealType.Equals(CommonComponents.Swap) || dealType.Equals(CommonComponents.NDS))
            {
                string tenor = subject.GetComponent(SubjectComponentName.Tenor);
                string settlementDate = subject.GetComponent(SubjectComponentName.SettlementDate);
                if (tenor == null && settlementDate == null)
                {
                    InapiEventHandler.OnSubscriptionStatus(subject, SubscriptionStatus.REJECTED, "Tenor and SettlementDate are both null");
                    return false;
                }

                if (settlementDate != null && !_settlementDateRegex.IsMatch(settlementDate.ToUpper()))
                {
                    InapiEventHandler.OnSubscriptionStatus(subject, SubscriptionStatus.REJECTED, "Invalid SettlementDate");
                    return false;
                }

                if (tenor != null && settlementDate == null && !_tenorRegex.IsMatch(tenor.ToUpper()))
                {
                    InapiEventHandler.OnSubscriptionStatus(subject, SubscriptionStatus.REJECTED, "Unsupported Tenor");
                    return false;
                }
            }

            if (dealType.Equals(CommonComponents.Swap) || dealType.Equals(CommonComponents.NDS))
            {
                string farTenor = subject.GetComponent(SubjectComponentName.FarTenor);
                string farSettlementDate = subject.GetComponent(SubjectComponentName.FarSettlementDate);
                if (farTenor == null && farSettlementDate == null)
                {
                    InapiEventHandler.OnSubscriptionStatus(subject, SubscriptionStatus.REJECTED, "FarTenor and FarSettlementDate are both null");
                    return false;
                }

                if (farSettlementDate != null && !_settlementDateRegex.IsMatch(farSettlementDate.ToUpper()))
                {
                    InapiEventHandler.OnSubscriptionStatus(subject, SubscriptionStatus.REJECTED, "Invalid SettlementDate");
                    return false;
                }

                if (farTenor != null && farSettlementDate == null && !_tenorRegex.IsMatch(farTenor.ToUpper()))
                {
                    InapiEventHandler.OnSubscriptionStatus(subject, SubscriptionStatus.REJECTED, "Unsupported Tenor");
                    return false;
                }
            }

            return true;
        }

        public PixieProtocolOptions PixieProtocolOptions
        {
            get { return _protocolOptions; }
        }
    }
}