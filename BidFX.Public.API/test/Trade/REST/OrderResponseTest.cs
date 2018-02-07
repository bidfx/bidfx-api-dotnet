using System.Collections.Generic;
using System.Net;
using NUnit.Framework;

namespace BidFX.Public.API.Trade.REST
{
    public class OrderResponseTest
    {
        [Test]
        public void SingleErrorResponseIsParsedCorrectly()
        {
            const string json = "{" +
                                "\"type\": \"Request Timeout\"," +
                                "\"message\": \"Timeout waiting for TSOM response\"" +
                                "}";
            OrderResponse orderResponse = new OrderResponse(json);
            List<string> expected = new List<string>() {"Timeout waiting for TSOM response"};
            Assert.AreEqual(expected, orderResponse.GetErrors());
            Assert.IsNull(orderResponse.GetOrderId());
            Assert.IsNull(orderResponse.GetState());
        }

        [Test]
        public void SingleOrderWithErrorIsParsedCorrectly()
        {
            const string json = "[" +
                                "{" +
                                "\"account\":\"INVALID_ACCOUNT\"," +
                                "\"ccy_pair\":\"EURGBP\"," +
                                "\"deal_type\":\"Spot\"," +
                                "\"dealt_ccy\":\"GBP\"," +
                                "\"errors\":" +
                                "[" +
                                "{" +
                                "\"message\":\"Invalid ExecutingAccount specified: INVALID_ACCOUNT:C:0001\"," +
                                "\"type\":\"Internal Server Error\"" +
                                "}" +
                                "]," +
                                "\"handling_type\":\"automatic\"," +
                                "\"order_ts_id\":\"20180206-171449888_24\"," +
                                "\"owner\":\"lasman\"," +
                                "\"price\":1.443210," +
                                "\"price_type\":\"Limit\"," +
                                "\"quantity\":1000000," +
                                "\"side\":\"Sell\"," +
                                "\"state\":\"NotValid\"" +
                                "}" +
                                "]";
            OrderResponse orderResponse = new OrderResponse(json);
            List<string> expectedError =
                new List<string>() {"Invalid ExecutingAccount specified: INVALID_ACCOUNT:C:0001"};
            Assert.AreEqual(expectedError, orderResponse.GetErrors());
            Assert.AreEqual("20180206-171449888_24", orderResponse.GetOrderId());
            Assert.AreEqual("NotValid", orderResponse.GetState());
        }

        [Test]
        public void NotAuthorizedResponseIsHandledCorrectly()
        {
            OrderResponse orderResponse = new OrderResponse("", HttpStatusCode.Forbidden);
            Assert.IsNull(orderResponse.GetOrderId());
            Assert.IsNull(orderResponse.GetState());
            List<string> expectedError = new List<string>() {"401 Forbidden - Invalid Username or Password"};
            Assert.AreEqual(expectedError, orderResponse.GetErrors());
        }
    }
    
}