/// Copyright (c) 2018 BidFX Systems Ltd. All Rights Reserved.

using System;
using System.ComponentModel;
using System.Numerics;
using BidFX.Public.API.Price;
using NUnit.Framework;

namespace BidFX.Public.API
{
    public class ClientTest
    {
        [Test]
        public void TestDefaultDepthLimitIsTwenty()
        {
            Client client = new Client();
            Assert.AreEqual(20, ((PriceManager) client.PriceSession).LevelTwoSubscriptionLimit);
        }
        
        [Test]
        public void TestSettingDepthLimitWithValidString()
        {
            Client client = new Client
            {
                Username = "lasman"
            };
            
            client.SetLevelTwoSubscriptionLimit("50-lasman-2-8099209487947021776313298155745713500");
            Assert.AreEqual(50, ((PriceManager) client.PriceSession).LevelTwoSubscriptionLimit);
        }

        [Test]
        public void TestSettingDepthLimitWhereClientHasNoUsername()
        {
            Client client = new Client();
            AssertLevelTwoThrowsException(client, "500-lasman-2-1507375166676311792313626867324836688");
            Assert.AreNotEqual(500, ((PriceManager) client.PriceSession).LevelTwoSubscriptionLimit);
            client.Username = "lasman";
            client.SetLevelTwoSubscriptionLimit("500-lasman-2-1507375166676311792313626867324836688");
            Assert.AreEqual(500, ((PriceManager) client.PriceSession).LevelTwoSubscriptionLimit);
        }

        [Test]
        public void TestSettingDepthLimitThenChangingUserResetsLimits()
        {
            Client client = new Client
            {
                Username = "lasman"
            };
            int defaultLimit = ((PriceManager) client.PriceSession).LevelTwoSubscriptionLimit;
            client.SetLevelTwoSubscriptionLimit("500-lasman-2-1507375166676311792313626867324836688");
            client.Username = "dtang";
            Assert.AreEqual(defaultLimit, ((PriceManager) client.PriceSession).LevelTwoSubscriptionLimit);
        }

        [Test]
        public void TestSettingDepthLimitWithInvalidStrings()
        {
            Client client = new Client
            {
                Username = "lasman"
            };
            AssertLevelTwoThrowsException(client, "60");
            AssertLevelTwoThrowsException(client, "100805555434543764398893583033760486589300");
            AssertLevelTwoThrowsException(client, "00060-76303678645270052016889170255093682297");
            AssertLevelTwoThrowsException(client, "80-5555434543764398893583033760486589300");
            AssertLevelTwoThrowsException(client, "0150-31211767855928379875643592271457043880");
            AssertLevelTwoThrowsException(client, "500-dtang-2-1507375166676311792313626867324836688"); // Changed username
            AssertLevelTwoThrowsException(client, "5000-lasman-2-1507375166676311792313626867324836688"); // Changed limit
            AssertLevelTwoThrowsException(client, "5000-lasman-1-90353847172159269059393691844719013523"); // Valid level 1
            AssertLevelTwoThrowsException(client, "5000-lasman-2-90353847172159269059393691844719013523"); // Changed level of valid level 1
            AssertLevelTwoThrowsException(client, "-lasman-2-8099209487947021776313298155745713500"); // Missing limit
            AssertLevelTwoThrowsException(client, "lasman-2-8099209487947021776313298155745713500"); // Missing limit
            AssertLevelTwoThrowsException(client, "50--2-8099209487947021776313298155745713500"); // Missing username
            AssertLevelTwoThrowsException(client, "50-2-8099209487947021776313298155745713500"); // Missing username
            AssertLevelTwoThrowsException(client, "50-lasman--8099209487947021776313298155745713500"); // Missing level
            AssertLevelTwoThrowsException(client, "50-lasman-8099209487947021776313298155745713500"); // Missing level
            AssertLevelTwoThrowsException(client, "50-lasman-2-"); // Missing signature
            AssertLevelTwoThrowsException(client, "50-lasman-2"); // Missing signature
        }

        [Test]
        public void TestDefaultLevelOneLimitIsFiveHundred()
        {
            Client client = new Client();
            Assert.AreEqual(500, ((PriceManager) client.PriceSession).LevelOneSubscriptionLimit);
        }
        
        [Test]
        public void TestSettingLevelOneLimitWithValidString()
        {
            Client client = new Client
            {
                Username = "lasman"
            };
            client.SetLevelOneSubscriptionLimit("10000-lasman-1-100670548678836408097796338235541122639");
            Assert.AreEqual(10000, ((PriceManager) client.PriceSession).LevelOneSubscriptionLimit);
        }

        [Test]
        public void TestSettingLevelOneLimitWhereClientHasNoUsername()
        {
            Client client = new Client();
            AssertLevelOneThrowsException(client, "10000-lasman-1-100670548678836408097796338235541122639");
            Assert.AreNotEqual(10000, ((PriceManager) client.PriceSession).LevelOneSubscriptionLimit);
            client.Username = "lasman";
            client.SetLevelOneSubscriptionLimit("10000-lasman-1-100670548678836408097796338235541122639");
            Assert.AreEqual(10000, ((PriceManager) client.PriceSession).LevelOneSubscriptionLimit);
        }

        [Test]
        public void TestSettingLevelOneLimitThenChangingUserResetsLimits()
        {
            Client client = new Client
            {
                Username = "lasman"
            };
            int defaultLimit = ((PriceManager) client.PriceSession).LevelOneSubscriptionLimit;
            client.SetLevelOneSubscriptionLimit("10000-lasman-1-100670548678836408097796338235541122639");
            client.Username = "dtang";
            Assert.AreEqual(defaultLimit, ((PriceManager) client.PriceSession).LevelOneSubscriptionLimit);
        }

        [Test]
        public void TestSettingLevelOneLimitWithInvalidStrings()
        {
            Client client = new Client();
            AssertLevelOneThrowsException(client, "300");
            AssertLevelOneThrowsException(client, "100805555434543764398893583033760486589300");
            AssertLevelOneThrowsException(client, "00300-48604692006149898576801843163581163962");
            AssertLevelOneThrowsException(client, "150-31211767855928379875643592271457043880");
            AssertLevelOneThrowsException(client, "0400-19486452728575513700892659900873492749");
            AssertLevelOneThrowsException(client, "10000-dtang-1-100670548678836408097796338235541122639"); // Changed username
            AssertLevelOneThrowsException(client, "15000-lasman-1-100670548678836408097796338235541122639"); // Changed limit
            AssertLevelOneThrowsException(client, "500-lasman-2-1507375166676311792313626867324836688"); // Valid level 2
            AssertLevelOneThrowsException(client, "500-lasman-1-1507375166676311792313626867324836688"); // Changed level of valid level 2
            AssertLevelOneThrowsException(client, "-lasman-1-100670548678836408097796338235541122639"); // Missing limit
            AssertLevelOneThrowsException(client, "lasman-1-100670548678836408097796338235541122639"); // Missing limit
            AssertLevelOneThrowsException(client, "10000--1-100670548678836408097796338235541122639"); // Missing username
            AssertLevelOneThrowsException(client, "10000-1-100670548678836408097796338235541122639"); // Missing username
            AssertLevelOneThrowsException(client, "10000-lasman--100670548678836408097796338235541122639"); // Missing level
            AssertLevelOneThrowsException(client, "10000-lasman-100670548678836408097796338235541122639"); // Missing level
            AssertLevelOneThrowsException(client, "10000-lasman-1-"); // Missing signature
            AssertLevelOneThrowsException(client, "10000-lasman-1"); // Missing signature
        }

        private void AssertLevelOneThrowsException(Client client, string signedString)
        {
            try
            {
                client.SetLevelOneSubscriptionLimit(signedString);
            }
            catch (Exception e)
            {
                return;
            }

            Assert.Fail("No exception thrown");
        }
        
        private void AssertLevelTwoThrowsException(Client client, string signedString)
        {
            try
            {
                client.SetLevelTwoSubscriptionLimit(signedString);
            }
            catch (Exception e)
            {
                return;
            }

            Assert.Fail("No exception thrown");
        }
    }
}