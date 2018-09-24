/// Copyright (c) 2018 BidFX Systems Ltd. All Rights Reserved.

using BidFX.Public.API.Price;
using BidFX.Public.API.Price.Subject;
using BidFX.Public.API.Price.Tools;
using NUnit.Framework;

namespace BidFX.Public.API
{
    public class PriceManagerTest
    {
        [Test]
        public void TestCantOverSubscribeOnDepth()
        {
            PriceManager priceManager = new PriceManager
            {
                LoginService = TestUtils.GetMockLoginService(),
                LevelTwoSubscriptionLimit = 1
            };

            priceManager.Start();

            bool receivedRejection = false;
            priceManager.SubscriptionStatusEventHandler += (sender, subscriptionStatusEvent) =>
            {
                if (subscriptionStatusEvent.SubscriptionStatus.Equals(SubscriptionStatus.REJECTED))
                {
                    receivedRejection = true;
                }
            };
            
            Subject s1 = new Subject("DealType=Spot,Level=2,User=lasman");
            Subject s2 = new Subject("DealType=Spot,Level=2,User=dtang");
            
            priceManager.Subscribe(s1);
            priceManager.Subscribe(s2);

            Assert.IsTrue(receivedRejection);
        }

        [Test]
        public void TestCanSubscribeLevelOneWhenFullySubscribedLevelTwo()
        {
            PriceManager priceManager = new PriceManager
            {
                LoginService = TestUtils.GetMockLoginService(),
                LevelTwoSubscriptionLimit = 0,
                LevelOneSubscriptionLimit = 1
            };
            priceManager.Start();

            int receivedRejections = 0;
            int recievedOthers = 0;
            priceManager.SubscriptionStatusEventHandler += (sender, subscriptionStatusEvent) =>
            {
                if (subscriptionStatusEvent.SubscriptionStatus.Equals(SubscriptionStatus.REJECTED))
                {
                    receivedRejections++;
                }
                else
                {
                    recievedOthers++;
                }
            };
            
            Subject s1 = new Subject("DealType=Spot,Level=2,User=lasman");
            Subject s2 = new Subject("DealType=Spot,Level=1,User=dtang");
            
            priceManager.Subscribe(s1);
            Assert.AreEqual(1, receivedRejections);
            Assert.AreEqual(0, recievedOthers);
            priceManager.Subscribe(s2);
            Assert.AreEqual(1, receivedRejections);
            Assert.LessOrEqual(1, recievedOthers);
        }

        [Test]
        public void TestCantOverSubscribeOnLevelOne()
        {
            PriceManager priceManager = new PriceManager
            {
                LoginService = TestUtils.GetMockLoginService(),
            };
            
            priceManager.LevelOneSubscriptionLimit = 1;
            priceManager.Start();

            bool receivedRejection = false;
            priceManager.SubscriptionStatusEventHandler += (sender, subscriptionStatusEvent) =>
            {
                if (subscriptionStatusEvent.SubscriptionStatus.Equals(SubscriptionStatus.REJECTED))
                {
                    receivedRejection = true;
                }
            };
            
            Subject s1 = new Subject("Level=1,User=lasman");
            Subject s2 = new Subject("Level=1,User=dtang");
            
            priceManager.Subscribe(s1);
            priceManager.Subscribe(s2);

            Assert.IsTrue(receivedRejection);
        }
        
        [Test]
        public void TestCanSubscribeLevelTwoWhenFullySubscribedLevelOne()
        {
            PriceManager priceManager = new PriceManager
            {
                LoginService = TestUtils.GetMockLoginService(),
            };
            
            priceManager.LevelOneSubscriptionLimit = 0;
            priceManager.LevelTwoSubscriptionLimit = 1;
            priceManager.Start();

            int receivedRejections = 0;
            int recievedOthers = 0;
            priceManager.SubscriptionStatusEventHandler += (sender, subscriptionStatusEvent) =>
            {
                if (subscriptionStatusEvent.SubscriptionStatus.Equals(SubscriptionStatus.REJECTED))
                {
                    receivedRejections++;
                }
                else
                {
                    recievedOthers++;
                }
            };
            
            Subject s1 = new Subject("DealType=Spot,Level=1,User=lasman");
            Subject s2 = new Subject("DealType=Spot,Level=2,User=dtang");
            
            priceManager.Subscribe(s1);
            Assert.AreEqual(1, receivedRejections);
            Assert.AreEqual(0, recievedOthers);
            priceManager.Subscribe(s2);
            Assert.AreEqual(1, receivedRejections);
            Assert.LessOrEqual(1, recievedOthers);
        }
    }
}