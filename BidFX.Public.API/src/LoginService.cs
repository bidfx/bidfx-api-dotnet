/// Copyright (C) 2018 BidFX Systems Ltd. All rights reserved

using System;
using System.Net;
using System.Reflection;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using log4net;


namespace BidFX.Public.API
{
    internal class LoginService
    {
        private static readonly ILog Log = LogManager.GetLogger("LoginService"); 
        private static readonly string ProductLookupPath = @"api/auth/v1/product";
        
        public TimeSpan RecheckInterval { get; set; }
        public bool Https { get; set; }
        public virtual bool LoggedIn { get; private set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Product { private get; set; }

        public event EventHandler<DisconnectEventArgs> OnForcedDisconnectEventHandler;

        private Timer _authorizationChecker;

        public LoginService()
        {
            Port = 443;
            Host = @"ny-tunnel.prod.tradingscreen.com";
            Https = true;
            RecheckInterval = TimeSpan.FromMinutes(5);
            Product = "BidFXDotnet";
        }

        public void Start()
        {
            if (LoggedIn)
            {
                Log.Debug("Already logged in");
                return;
            }

            HttpStatusCode statusCode;
            if (UserHasProduct(out statusCode))
            {
                Log.Debug("Succesfully validated user permissions");
                LoggedIn = true;
                StartRecurringAuthorizationCheck();
            }
            else
            {
                string failureReason = GetReasonForFailure(statusCode);
                Log.ErrorFormat("Could not validate user: {0}", failureReason);
                throw new AuthenticationException(failureReason);
            }
        }

        public void Stop()
        {
            LoggedIn = false;
            if (_authorizationChecker != null)
            {
                _authorizationChecker.Dispose();
            }
        }

        private bool UserHasProduct(out HttpStatusCode statusCode)
        {
            using (HttpWebResponse response = SendProductMessage())
            {
                if (response != null)
                {
                    statusCode = response.StatusCode;
                    return HttpStatusCode.OK.Equals(response.StatusCode);
                }

                statusCode = HttpStatusCode.NotFound;
                return false;
            }
        }

        private HttpWebResponse SendProductMessage()
        {
            Uri uri = new UriBuilder
            {
                Host = Host,
                Port = Port,
                Scheme = Https ? "https" : "http",
                Path = ProductLookupPath,
                Query = "login_id=" + Username + "&product=" + Product
            }.Uri;
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(uri);
            request.Method = "GET";
            request.Headers["Authorization"] = CreateAuthHeader();
            request.ContentType = "application/json";
            request.KeepAlive = false;

            try
            {
                return (HttpWebResponse) request.GetResponse();
            }
            catch (WebException e)
            {
                // Occurs on non-success codes
                return (HttpWebResponse) e.Response;
            }
        }

        private string CreateAuthHeader()
        {
            return "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(Username + ":" + Password));
        }

        private void StartRecurringAuthorizationCheck()
        {
            _authorizationChecker = new Timer(state =>
            {
                if (Log.IsDebugEnabled)
                {
                    Log.Debug("Rechecking user permissions...");
                }
                HttpStatusCode statusCode;
                if (!UserHasProduct(out statusCode))
                {
                    string failureReason = GetReasonForFailure(statusCode);
                    Log.WarnFormat("Could not revalidate user permissions: {0}", failureReason);
                    Stop();
                    if (OnForcedDisconnectEventHandler != null)
                    {
                        OnForcedDisconnectEventHandler(this, new DisconnectEventArgs(failureReason));
                    }
                }
            }, null, RecheckInterval, RecheckInterval);
        }

        private string GetReasonForFailure(HttpStatusCode statusCode)
        {
            if (HttpStatusCode.Unauthorized.Equals(statusCode))
            {
                return "invalid credentials";
            }
            else if (HttpStatusCode.Forbidden.Equals(statusCode))
            {
                return "user " + Username + " does not have the required products " +
                       "(" + Product + ", TSWebAPI) " +
                       "assigned. Please contact your account manager if you believe this is a mistake";
            }
            else
            {
                return "could not authorize user";
            }
        }
    }

    public class DisconnectEventArgs : EventArgs
    {
        public string Reason { get; private set; }

        public DisconnectEventArgs(string reason)
        {
            Reason = reason;
        }
    }
}