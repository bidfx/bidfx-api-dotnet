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
        public void TestSettingDepthLimitWithValidString()
        {
            Client client = new Client();
            ISession unused = client.PriceSession;
            Assert.IsTrue(client.SetDepthSubscriptionLimit("00060-76303678645270052016889170255093682297"));
            Assert.IsTrue(client.SetDepthSubscriptionLimit("80-5555434543764398893583033760486589300"));
            Assert.IsTrue(client.SetDepthSubscriptionLimit("0150-31211767855928379875643592271457043880"));
        }

        [Test]
        public void TestSettingDepthLimitWithInvalidStrings()
        {
            Client client = new Client();
            ISession unused = client.PriceSession;
            Assert.IsFalse(client.SetDepthSubscriptionLimit("60"));
            Assert.IsFalse(client.SetDepthSubscriptionLimit("100805555434543764398893583033760486589300"));
        }

        [Test]
        public void TestSettingLevelOneLimitWithValidString()
        {
            Client client = new Client();
            ISession unused = client.PriceSession;
            Assert.IsTrue(client.SetLevelOneSubscriptionLimit("00300-48604692006149898576801843163581163962"));
            Assert.IsTrue(client.SetLevelOneSubscriptionLimit("150-31211767855928379875643592271457043880"));
            Assert.IsTrue(client.SetLevelOneSubscriptionLimit("0400-19486452728575513700892659900873492749"));
        }

        [Test]
        public void TestSettingLevelOneLimitWithInvalidStrings()
        {
            Client client = new Client();
            ISession unused = client.PriceSession;
            Assert.IsFalse(client.SetLevelOneSubscriptionLimit("300"));
            Assert.IsFalse(client.SetLevelOneSubscriptionLimit("100805555434543764398893583033760486589300"));
        }
    }
}