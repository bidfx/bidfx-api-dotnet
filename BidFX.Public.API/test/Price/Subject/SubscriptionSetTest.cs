using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace BidFX.Public.API.Price.Subject
{
    public class SubjectSetTest
    {
        private SubscriptionSet _subscriptionSet;

        private static readonly Subject Subject1 = new Subject("Account=FX_ACCT");
        private static readonly Subject Subject2 = new Subject("Account=TEST_ACCOUNT");
        private Subscription _subscription1;
        private Subscription _subscription2;

        [SetUp]
        public void Before()
        {
            _subscriptionSet = new SubscriptionSet();
            _subscription1 = _subscriptionSet.Add(Subject1, true);
            _subscription2 = _subscriptionSet.Add(Subject2, false);
        }

        [Test]
        public void AddSubjectCreatesAndReturnsSubscription()
        {
            Assert.AreEqual(Subject1, _subscription1.Subject);
            Assert.AreEqual(true, _subscription1.AutoRefresh);
            Assert.AreEqual(Subject2, _subscription2.Subject);
            Assert.AreEqual(false, _subscription2.AutoRefresh);
        }

        [Test]
        public void GetSubscriptionTest()
        {
            Subscription subscription1 = new Subscription(Subject1, true);
            Assert.AreEqual(subscription1.Subject, _subscriptionSet.GetSubscription(Subject1).Subject);
            Assert.AreEqual(subscription1.AllPriceFields.FieldNames,
                _subscriptionSet.GetSubscription(Subject1).AllPriceFields.FieldNames);
            Assert.AreEqual(subscription1.AllPriceFields.PriceFields,
                _subscriptionSet.GetSubscription(Subject1).AllPriceFields.PriceFields);
            Assert.AreEqual(subscription1.AutoRefresh, _subscriptionSet.GetSubscription(Subject1).AutoRefresh);
            Assert.AreEqual(subscription1.SubscriptionStatus,
                _subscriptionSet.GetSubscription(Subject1).SubscriptionStatus);
        }

        [Test]
        public void ClearReturnsListOfSubscriptions()
        {
            List<Subscription> subscriptions = _subscriptionSet.Clear().ToList();
            Assert.Contains(_subscription1, subscriptions);
            Assert.Contains(_subscription2, subscriptions);
            Assert.AreEqual(2, subscriptions.Count);
        }

        [Test]
        public void SubjectsTest()
        {
            List<Subject> subjects = _subscriptionSet.Subjects();
            Assert.Contains(Subject1, subjects);
            Assert.Contains(Subject2, subjects);
            Assert.AreEqual(2, subjects.Count);
        }

        [Test]
        public void ClearClearsSubscriptions()
        {
            _subscriptionSet.Clear();
            Assert.IsEmpty(_subscriptionSet.Subjects());
        }

        [Test]
        public void StaleSubscriptionsTest()
        {
            _subscription1.SubscriptionStatus = SubscriptionStatus.OK;
            _subscription2.SubscriptionStatus = SubscriptionStatus.OK;
            List<Subscription> staleSubscriptions = _subscriptionSet.StaleSubscriptions().ToList();
            Assert.AreEqual(0, staleSubscriptions.Count);

            _subscription1.SubscriptionStatus = SubscriptionStatus.STALE;
            staleSubscriptions = _subscriptionSet.StaleSubscriptions().ToList();
            Assert.Contains(_subscription1, staleSubscriptions);
            Assert.AreEqual(1, staleSubscriptions.Count);

            _subscription1.SubscriptionStatus = SubscriptionStatus.OK;
            staleSubscriptions = _subscriptionSet.StaleSubscriptions().ToList();
            Assert.AreEqual(0, staleSubscriptions.Count);

            _subscription1.SubscriptionStatus = SubscriptionStatus.STALE;
            _subscription2.SubscriptionStatus = SubscriptionStatus.STALE;
            staleSubscriptions = _subscriptionSet.StaleSubscriptions().ToList();
            Assert.Contains(_subscription1, staleSubscriptions);
            Assert.Contains(_subscription2, staleSubscriptions);
            Assert.AreEqual(2, staleSubscriptions.Count);
        }
    }
}