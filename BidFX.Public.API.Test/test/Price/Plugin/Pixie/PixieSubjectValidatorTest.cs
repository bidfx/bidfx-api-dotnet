using System.Collections.Generic;
using BidFX.Public.API.Price.Subject;
using NUnit.Framework;

namespace BidFX.Public.API.Price.Plugin.Pixie
{
    public class PixieSubjectValidatorTest
    {
        private TestEventHandler _eventHandler;

        [SetUp]
        public void SetUp()
        {
            _eventHandler = new TestEventHandler();
        }

        [Test]
        public void TestValidSubjects()
        {
            Assert.IsTrue(PixieSubjectValidator.ValidateSubject(CommonSubjects.CreateLevelOneForwardQuoteSubject("FX_ACCT", "EURUSD", "BARCFX", "EUR", "1000000", "2M", null).CreateSubject(), _eventHandler));
            Assert.IsTrue(PixieSubjectValidator.ValidateSubject(CommonSubjects.CreateLevelOneForwardStreamingSubject("FX_ACCT", "EURUSD", "BARCFX", "EUR", "1000000", "TOM", null).CreateSubject(), _eventHandler));
            Assert.IsTrue(PixieSubjectValidator.ValidateSubject(CommonSubjects.CreateLevelOneNdfQuoteSubject("FX_ACCT", "GBPUSD", "BARCFX", "GBP", "5000000", "1W", "").CreateSubject(), _eventHandler));
            Assert.IsTrue(PixieSubjectValidator.ValidateSubject(CommonSubjects.CreateLevelOneNdfQuoteSubject("FX_ACCT", "EURCAD", "DBFX", "EUR", "1000", "BD", "20200505").CreateSubject(), _eventHandler));
            Assert.IsTrue(PixieSubjectValidator.ValidateSubject(CommonSubjects.CreateLevelOneSwapQuoteSubject("FX_ACCT", "EURUSD", "DBFX", "EUR", "100000", "IMMH", "", "1000000", "IMMZ", "").CreateSubject(), _eventHandler));
            Assert.IsEmpty(_eventHandler.GetStatusUpdates());
        }

        [Test]
        public void TestInvalidSubjects()
        {
            Assert.IsFalse(PixieSubjectValidator.ValidateSubject(CommonSubjects.CreateLevelTwoForwardStreamingSubject("FX_ACCT", "EURUSD", "EUR", "1000000", "BD", "").CreateSubject(), _eventHandler));
            Assert.IsFalse(PixieSubjectValidator.ValidateSubject(CommonSubjects.CreateLevelTwoForwardStreamingSubject("FX_ACCT", "EURUSD", "EUR", "1000000", "", "WORDDS").CreateSubject(), _eventHandler));
            Assert.IsFalse(PixieSubjectValidator.ValidateSubject(CommonSubjects.CreateLevelTwoForwardStreamingSubject("FX_ACCT", "EURUSD", "EUR", "1000000", "", "99999999").CreateSubject(), _eventHandler));
            Assert.AreEqual(3, _eventHandler.GetStatusUpdates().Count);
        }

        [Test]
        public void TestSpotSubjects()
        {
            Assert.IsTrue(PixieSubjectValidator.ValidateSubject(CommonSubjects.CreateLevelOneSpotStreamingSubject("FX_ACCT", "EURUSD", "RBCFX", "EUR", "1000000.11")
                .SetComponent(SubjectComponentName.SettlementDate, "20220928").SetComponent(SubjectComponentName.Tenor, "Spot").CreateSubject(), _eventHandler)); 
            Assert.IsFalse(PixieSubjectValidator.ValidateSubject(CommonSubjects.CreateLevelOneSpotStreamingSubject("FX_ACCT", "EURUSD", "RBCFX", "EUR", "1000000.11")
                 .SetComponent(SubjectComponentName.Tenor, "test_tenor").CreateSubject(), _eventHandler)); 
            Assert.IsFalse(PixieSubjectValidator.ValidateSubject(CommonSubjects.CreateLevelOneSpotStreamingSubject("FX_ACCT", "EURUSD", "RBCFX", "EUR", "1000000.11")
                 .SetComponent(SubjectComponentName.SettlementDate, "2022 09 28").CreateSubject(), _eventHandler));
            Assert.IsEmpty(_eventHandler.GetStatusUpdates());
        }

        private class TestEventHandler : IApiEventHandler
        {
            private readonly List<dynamic[]> _statusUpdates = new List<dynamic[]>();
            
            public List<dynamic[]> GetStatusUpdates()
            {
                return _statusUpdates;
            }
        
            public void OnPriceUpdate(Subject.Subject subject, IPriceMap priceUpdate, bool replaceAllFields)
            {
                _statusUpdates.Add(new dynamic[] {subject, priceUpdate, replaceAllFields});
            }

            public void OnSubscriptionStatus(Subject.Subject subject, SubscriptionStatus status, string reason)
            {
                _statusUpdates.Add(new dynamic[] {subject, status, reason});
            }

            public void OnProviderStatus(IProviderPlugin providerPlugin, ProviderStatus previousStatus)
            {
                _statusUpdates.Add(new dynamic[] {providerPlugin, previousStatus});
            }
        }
    }
}   