using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace BidFX.Public.API.Price.Plugin.Pixie.Messages
{
    public class SubscriptionSyncTest
    {
        private const int Edition = 123;

        private readonly List<Subject.Subject> _subjectList = new List<Subject.Subject>()
        {
            new Subject.Subject("Symbol=EURGBP,Quantity=2500000"),
            new Subject.Subject("Symbol=USDJPY,Quantity=5600000")
        };

        [Test]
        public void TestIsChangedIfTrueByDefault()
        {
            SubscriptionSync subscriptionSync = new SubscriptionSync(Edition, _subjectList);
            Assert.AreEqual(true, subscriptionSync.IsChangedEdition());
        }

        [Test]
        public void TestSizeIsPopulatedFromTheSubjectListSize()
        {
            SubscriptionSync subscriptionSync = new SubscriptionSync(Edition, _subjectList);
            Assert.AreEqual(_subjectList.Count, subscriptionSync.Size);
        }

        [Test]
        public void TestIsCompressedIfFalseByDefault()
        {
            SubscriptionSync subscriptionSync = new SubscriptionSync(Edition, _subjectList);
            Assert.IsFalse(subscriptionSync.IsCompressed());
        }

        [Test]
        public void TestHasNoControlsByDefault()
        {
            SubscriptionSync subscriptionSync = new SubscriptionSync(Edition, _subjectList);
            Assert.IsFalse(subscriptionSync.HasControls());
        }

        [Test]
        public void testSettingValidControls_hasControlsIsTrue()
        {
            SubscriptionSync subscriptionSync = new SubscriptionSync(Edition, _subjectList);
            subscriptionSync.AddControl(0, ControlOperation.Toggle);
            Assert.IsTrue(subscriptionSync.HasControls());
        }

        [Test]
        public void testBugFix_hasControlsIsRemainsTrueAfterSettingCompression()
        {
            SubscriptionSync subscriptionSync = new SubscriptionSync(Edition, _subjectList);
            subscriptionSync.AddControl(0, ControlOperation.Toggle);
            Assert.IsTrue(subscriptionSync.HasControls());
            subscriptionSync.SetCompressed(true);
            Assert.IsTrue(subscriptionSync.HasControls());
        }

        [Test]
        public void TestEncodingWithCompressionResultsInASmallerMessageSize()
        {
            SubscriptionSync subscriptionSync =
                new SubscriptionSync(Edition, RealSubscriptionsExample.SortedSubjects.ToList());
            subscriptionSync.SetCompressed(false);
            long uncompressedSize = subscriptionSync.Encode(PixieVersion.CurrentVersion).Position;
            subscriptionSync.SetCompressed(true);
            long compressedSize = subscriptionSync.Encode(PixieVersion.CurrentVersion).Position;
            Assert.IsTrue(compressedSize < uncompressedSize);
        }

        [Test]
        public void testEncodingWithCompressionResultsInTypicalSubjectsUsingLessThan_16_Bytes()
        {
            SubscriptionSync subscriptionSync =
                new SubscriptionSync(Edition, new List<Subject.Subject>(RealSubscriptionsExample.SortedSubjects));
            subscriptionSync.SetCompressed(true);
            long compressedSize = subscriptionSync.Encode(PixieVersion.CurrentVersion).Position;
            Assert.IsTrue(compressedSize / RealSubscriptionsExample.SortedSubjects.Length <= 16);
        }

        [Test]
        public void TestSummarize()
        {
            SubscriptionSync subscriptionSync = new SubscriptionSync(Edition, _subjectList);
            Assert.AreEqual("SubscriptionSync(edition=123, compressed=False, controls=0, changed=True, subjects=2)",
                subscriptionSync.Summarize());

            subscriptionSync.AddControl(0, ControlOperation.Toggle);
            subscriptionSync.AddControl(1, ControlOperation.Refresh);
            Assert.AreEqual("SubscriptionSync(edition=123, compressed=False, controls=2, changed=True, subjects=2)",
                subscriptionSync.Summarize());

            subscriptionSync.SetCompressed(true);
            Assert.AreEqual("SubscriptionSync(edition=123, compressed=True, controls=2, changed=True, subjects=2)",
                subscriptionSync.Summarize());
        }
    }
}