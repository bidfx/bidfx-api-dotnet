using System;
using BidFX.Public.API.Price.Subject;
using Moq;
using NUnit.Framework;

namespace BidFX.Public.API.Price.Subject
{
    public class SubjectTest
    {
        private Subject mSubject;
        private string mFormattedString;

        [SetUp]
        public void Before()
        {
            mSubject = CreateSubjectFromBuilder();
            mFormattedString = mSubject.ToString();
        }

        private static Subject CreateSubjectFromBuilder()
        {
            try
            {
                return new SubjectBuilder()
                    .SetComponent("Level", "2")
                    .SetComponent("Symbol", "IBM.N")
                    .SetComponent("Source", "ComStock")
                    .SetComponent("AssetClass", "Equity")
                    .SetComponent("Exchange", "NYS")
                    .CreateSubject();
            }
            catch (IllegalSubjectException e)
            {
                Assert.Fail("we should not throw " + e);
                return null;
            }
        }

        [Test]
        public void TestLookupValue()
        {
            Assert.AreEqual("2", mSubject.LookupValue("Level"));
            Assert.AreEqual("ComStock", mSubject.LookupValue("Source"));
            Assert.AreEqual("Equity", mSubject.LookupValue("AssetClass"));
            Assert.AreEqual("NYS", mSubject.LookupValue("Exchange"));
            Assert.AreEqual("IBM.N", mSubject.LookupValue("Symbol"));
        }

        [Test]
        public void LookupOfMissingValueReturnsNull()
        {
            Assert.AreEqual(null, mSubject.LookupValue("UserName"));
        }

        [Test]
        public void TestIterator()
        {
            var mockHandler = new Mock<IComponentHandler>();
            var handler = mockHandler.Object;
            var callOrder = 0;
            foreach (var component in mSubject)
            {
                handler.SubjectComponent(component.Key, component.Value);
            }
            mockHandler.Setup(x => x.SubjectComponent("AssetClass", "Equity"))
                .Callback(() => Assert.AreEqual(0, callOrder++));
            mockHandler.Setup(x => x.SubjectComponent("AssetClass", "Equity"))
                .Callback(() => Assert.AreEqual(1, callOrder++));
            mockHandler.Setup(x => x.SubjectComponent("Exchange", "NYS"))
                .Callback(() => Assert.AreEqual(2, callOrder++));
            mockHandler.Setup(x => x.SubjectComponent("Level", "2"))
                .Callback(() => Assert.AreEqual(3, callOrder++));
            mockHandler.Setup(x => x.SubjectComponent("Source", "ComStock"))
                .Callback(() => Assert.AreEqual(4, callOrder++));
            mockHandler.Setup(x => x.SubjectComponent("Symbol", "IBM.N"))
                .Callback(() => Assert.AreEqual(5, callOrder++));
        }

        [Test]
        public void TestSimpleToString()
        {
            Assert.AreEqual(mFormattedString, mSubject.ToString());
        }

        [Test]
        public void TestHtmlEncodingInToString()
        {
            var subject1 = new SubjectBuilder()
                .SetComponent("One", "a b")
                .SetComponent("Two", "c d e")
                .SetComponent("Three", "a,2,3")
                .CreateSubject();
            Assert.AreEqual("One=a&#32;b,Three=a&#44;2&#44;3,Two=c&#32;d&#32;e", subject1.ToString());

            var subject2 = new Subject(subject1.ToString());
            Assert.AreEqual(subject1, subject2);
            Assert.AreEqual(subject1.GetHashCode(), subject2.GetHashCode());
            Assert.AreEqual(subject1.ToString(), subject2.ToString());
        }

        [Test]
        public void TestBuildingANewInstanceFromToString()
        {
            var s = mSubject.ToString();
            var subject = new Subject(s);
            Assert.AreEqual(mSubject, subject);
            Assert.AreEqual(s, subject.ToString());
        }

        [Test]
        public void DissimilarSubjectsAreNotEqual()
        {
            Assert.IsFalse(mSubject.Equals(null));
            Assert.IsFalse(mSubject.Equals(this));
            Assert.IsFalse(this.Equals(mSubject));
            Assert.IsFalse(mSubject.Equals(123));
            Assert.IsFalse(mSubject.Equals(new Subject(mFormattedString + "2")));
        }

        [Test]
        public void EquivalentSubjectsAreEqual()
        {
            Assert.IsTrue(mSubject.Equals(mSubject));
            Assert.IsTrue(mSubject.Equals(new Subject(mFormattedString)));
            Assert.IsTrue(new Subject(mFormattedString).Equals(mSubject));
        }

        [Test]
        public void EquivalentSubjectsAreEqualEvenWithDifferentStrings()
        {
            String text = mFormattedString.Substring(0, 10) + mFormattedString.Substring(10);
            Assert.IsTrue(mSubject.Equals(new Subject(text)));
        }

        [Test]
        public void SubjectCreatedFromBuilderSameAsThatFromString()
        {
            Assert.IsTrue(CreateSubjectFromBuilder().Equals(new Subject(mFormattedString)));
        }

        [Test]
        public void TestGetHashCodeIsNotZero()
        {
            Assert.IsTrue(mSubject.GetHashCode() != 0);
        }

        [Test]
        public void TestGetHashCodeIsTheSameEachTime()
        {
            Assert.AreEqual(mSubject.GetHashCode(), mSubject.GetHashCode());
        }

        [Test]
        public void TestGetHashCodeIsTheSameForAnEqualInstance()
        {
            Assert.AreEqual(mSubject.GetHashCode(),
                new Subject(mFormattedString).GetHashCode());
        }

        [Test]
        public void TestGetHashCodeIsDifferentForADissimilarInstance()
        {
            Assert.IsFalse(mSubject.GetHashCode() == new Subject("Symbol=IBM").GetHashCode());
            Assert.IsFalse(mSubject.GetHashCode() == new Subject(
                                   "AssetClass=Equity,Exchange=NYS,Level=1,Source=ComStock,Symbol=IBM&#44;N")
                               .GetHashCode());
        }

        [Test]
        public void TestIsEmptyIsFalseForNormalSubjects()
        {
            Assert.IsFalse(mSubject.IsEmpty());
            Assert.IsFalse(new Subject("a=1,b=2,c=3").IsEmpty());
            Assert.IsFalse(new Subject("a=1,b=2").IsEmpty());
            Assert.IsFalse(new Subject("a=1").IsEmpty());
        }


        [Test]
        public void TestSize()
        {
            Assert.AreEqual(5, mSubject.Size());
            Assert.AreEqual(1, new Subject("a=1").Size());
            Assert.AreEqual(2, new Subject("a=1,b=2").Size());
        }
    }
}