using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using BidFX.Public.API.Price.Tools;
using BidFX.Public.API.Trade;
using NUnit.Framework;

namespace BidFX.Public.API.Test.test
{
    [TestFixture]
    public class LoginServiceTest
    {
        private MockEndpoint _endpoint;
        private Client _client;
        
        [SetUp]
        public void Before()
        {
            _endpoint = new MockEndpoint
            {
                LoginProductAssignments = new Dictionary<string, List<string>>
                {
                    {"lasman", new List<string> {"BidFXDotnet"}},
                    {"dtang", new List<string> {"BidFXExcel"}}
                }
            };
            _endpoint.Start();
            _client = new Client
            {
                Host = "localhost",
                Port = 10200
            };
            _client.LoginService.Https = false;
        }

        [TearDown]
        public void After()
        {
            _endpoint.Stop();
            _client.Stop();
        }

        [Test]
        public void TestInvalidCredentialsDoesntPermitLogin()
        {
            _client.Username = "akearney";
            _client.Password = "";
            TradeSession tradeSession;
            AuthenticationException exception = Assert.Throws<AuthenticationException>(() =>
                 tradeSession = _client.TradeSession);
            Assert.AreEqual("invalid credentials", exception.Message);
            Assert.IsFalse(_client.LoggedIn);
            Assert.Throws<AuthenticationException>(() => _client.PriceSession.ToString());
            Assert.Throws<AuthenticationException>(() => _client.TradeSession.ToString());
        }

        [Test]
        public void TestNoProductDoesntPermitLogin()
        {
            _client.Username = "dtang";
            _client.Password = "";
            TradeSession tradeSession;
            AuthenticationException exception = Assert.Throws<AuthenticationException>(() =>
                tradeSession = _client.TradeSession);
            Assert.AreEqual("user dtang does not have the required products (BidFXDotnet, TSWebAPI) assigned. Please contact your account manager if you believe this is a mistake", exception.Message);
            Assert.IsFalse(_client.LoggedIn);
        }

        [Test]
        public void TestHasProductAllowsLogin()
        {
            _client.Username = "lasman";
            _client.Password = "";
            TradeSession tradeSession = _client.TradeSession;
            Assert.IsTrue(_client.LoggedIn);
            Assert.IsTrue(tradeSession.Running);
        }

        [Test]
        public void TestRemovingProductCausesDisconnect()
        {
            _client.Username = "lasman";
            _client.Password = "";
            _client.LoginService.RecheckInterval = TimeSpan.FromSeconds(2);
            TradeSession tradeSession = _client.TradeSession;
            Assert.IsTrue(_client.LoggedIn);
            Assert.IsTrue(tradeSession.Running);
            _endpoint.LoginProductAssignments["lasman"].Remove("BidFXDotnet");
            Thread.Sleep(TimeSpan.FromSeconds(4));
            Assert.IsFalse(_client.LoggedIn);
            Assert.IsFalse(tradeSession.Running);
        }

        [Test]
        public void TestChangingTheRequiredProductAllowsLogin()
        {
            _client.Username = "dtang";
            _client.Password = "";
            _client.LoginService.Product = "BidFXExcel";
            TradeSession tradeSession = _client.TradeSession;
            Assert.IsTrue(_client.LoggedIn);
            Assert.IsTrue(tradeSession.Running);
        }
        
        private class MockEndpoint
        {
            public Dictionary<string, List<string>> LoginProductAssignments;
            private readonly WebServer _webserver;

            public MockEndpoint()
            {
                _webserver = new WebServer(ProcessRequest, "http://localhost:10200/api/auth/v1/product/");
            }

            public void Start()
            {
                _webserver.Run();
            }
            
            public void Stop()
            {
                _webserver.Stop();
            }

            private void ProcessRequest(HttpListenerContext ctx)
            {
                string username = GetUsernameFromHeader(ctx.Request.Headers["Authorization"]);
                if (!LoginProductAssignments.ContainsKey(username))
                {
                    ctx.Response.StatusCode = 401;
                    ctx.Response.ContentLength64 = 0;
                    return;
                }

                string product = ctx.Request.QueryString["product"];
                if (!LoginProductAssignments[username].Contains(product))
                {
                    ctx.Response.StatusCode = 403;
                    ctx.Response.ContentLength64 = 0;
                    return;
                }

                ctx.Response.StatusCode = 200;
                ctx.Response.ContentLength64 = 0;
            }

            private static string GetUsernameFromHeader(string authorizationHeader)
            {
                string base64Part = authorizationHeader.Split(' ')[1];
                return new string(Encoding.Default.GetChars(Convert.FromBase64String(base64Part))).Split(':')[0];
            }
        }
    }
}