using Moq;
using NUnit.Framework;

namespace BidFX.Public.API.Price.Subject
{
    public class SubjectBuilderTest
    {
        private SubjectBuilder _subjectBuilder;

        [SetUp]
        public void Before()
        {
            _subjectBuilder = new SubjectBuilder();
        }

        [Test]
        public void TestLookupValue()
        {
            _subjectBuilder.SetComponent("A", "1").SetComponent("B", "2");
            Assert.AreEqual("1", _subjectBuilder.LookupValue("A"));
            Assert.AreEqual("2", _subjectBuilder.LookupValue("B"));
            Assert.AreEqual(null, _subjectBuilder.LookupValue("C"));
        }

        [Test]
        public void TestIterator()
        {
            _subjectBuilder.SetComponent("A",
                "1").SetComponent("B", "2");
            Mock<IComponentHandler> handler = new Mock<IComponentHandler>();
            foreach (SubjectComponent component in _subjectBuilder)
            {
                handler.Object.SubjectComponent(component.Key, component.Value);
            }

            handler.Verify(mock => mock.SubjectComponent("A", "1"));
            handler.Verify(mock => mock.SubjectComponent("B", "2"));
        }

        [Test]
        public void ClearEmptiesAllComponents()
        {
            _subjectBuilder.SetComponent("A", "1").SetComponent("B", "2");
            Assert.IsTrue(_subjectBuilder.GetEnumerator().MoveNext());
            _subjectBuilder.Clear();
            Assert.IsFalse(_subjectBuilder.GetEnumerator().MoveNext());
        }


        [Test]
        public void BuildATypicalSubject()
        {
            Subject subject = _subjectBuilder
                .SetComponent("Level", "1")
                .SetComponent("Symbol", "VOD.L")
                .SetComponent("LiquidityProvider", "Reuters")
                .SetComponent("AssetClass", "Equity")
                .SetComponent("Exchange", "LSE")
                .CreateSubject();

            Assert.AreEqual("1", subject.GetComponent("Level"));
            Assert.AreEqual("Reuters", subject.GetComponent("LiquidityProvider"));
            Assert.AreEqual("Equity", subject.GetComponent("AssetClass"));
            Assert.AreEqual("LSE", subject.GetComponent("Exchange"));
            Assert.AreEqual("VOD.L", subject.GetComponent("Symbol"));
            Assert.AreEqual("AssetClass=Equity,Exchange=LSE,Level=1,LiquidityProvider=Reuters,Symbol=VOD.L",
                subject.ToString());
        }

        [Test]
        public void EmptyBuilderGeneratesAnEmptyComponentArray()
        {
            Assert.That(new string[] { }, Is.EquivalentTo(_subjectBuilder.GetComponents()));
        }

        [Test]
        public void OneComponentSubject()
        {
            _subjectBuilder.SetComponent("LiquidityProvider", "Reuters");
            Assert.That(new string[]
            {
                "LiquidityProvider", "Reuters"
            }, Is.EquivalentTo(_subjectBuilder.GetComponents()));
        }

        [Test]
        public void MultiComponentSubject()
        {
            _subjectBuilder
                .SetComponent("A", "1")
                .SetComponent("B", "2")
                .SetComponent("C", "3");
            Assert.That(new string[]
            {
                "A", "1", "B", "2", "C", "3"
            }, Is.EquivalentTo(_subjectBuilder.GetComponents()));
        }

        [Test]
        public void componentKeysAreOrderedAlphabetically()
        {
            _subjectBuilder
                .SetComponent("D", "4")
                .SetComponent("A", "1")
                .SetComponent("C", "3")
                .SetComponent("B", "2");
            Assert.That(new[]
            {
                "A", "1", "B", "2", "C", "3", "D", "4"
            }, Is.EquivalentTo(_subjectBuilder.GetComponents()));
        }

        [Test]
        public void subjectBuilderMayCreateManySubjects()
        {
            _subjectBuilder
                .SetComponent("A", "1")
                .SetComponent("B", "2");
            Assert.That(new[]
            {
                "A", "1", "B", "2"
            }, Is.EquivalentTo(_subjectBuilder.GetComponents()));
            Assert.That(new[]
            {
                "A", "1", "B", "2"
            }, Is.EquivalentTo(_subjectBuilder.GetComponents()));
            _subjectBuilder.SetComponent("C", "3");
            Assert.That(new[]
            {
                "A", "1", "B", "2", "C", "3"
            }, Is.EquivalentTo(_subjectBuilder.GetComponents()));
            _subjectBuilder.SetComponent("A", "a1");
            Assert.That(new[]
            {
                "A", "a1", "B", "2", "C", "3"
            }, Is.EquivalentTo(_subjectBuilder.GetComponents()));
        }

        [Test]
        public void CantSetNullKeys()
        {
            Assert.Throws<IllegalSubjectException>(() => _subjectBuilder.SetComponent(null, "Reuters"));
        }

        [Test]
        public void CantSetEmptyKeys()
        {
            Assert.Throws<IllegalSubjectException>(() => _subjectBuilder.SetComponent("", "Reuters"));
        }

        [Test]
        public void CantSetNullValues()
        {
            Assert.Throws<IllegalSubjectException>(() => _subjectBuilder.SetComponent("LiquidityProvider", null));
        }

        [Test]
        public void CantSetEmptyValues()
        {
            Assert.Throws<IllegalSubjectException>(() => _subjectBuilder.SetComponent("LiquidityProvider", ""));
        }

        [Test]
        public void TestEmptyBuilderToString()
        {
            Assert.AreEqual("", _subjectBuilder.ToString());
        }

        [Test]
        public void TestPopulatedBuilderToString()
        {
            _subjectBuilder
                .SetComponent("Level", "1")
                .SetComponent("Symbol", "VOD.L")
                .SetComponent("LiquidityProvider", "Reuters");
            Assert.AreEqual("Level=1,LiquidityProvider=Reuters,Symbol=VOD.L", _subjectBuilder.ToString());
        }

        [Test]
        public void TestCreateFromSubject()
        {
            Subject subject = new Subject("Level=1,LiquidityProvider=Reuters,Symbol=BOD.L");
            _subjectBuilder = new SubjectBuilder(subject);
            Assert.AreEqual(subject.ToString(), _subjectBuilder.ToString());
        }

        [Test]
        public void TestCreateFromSubjectBuilder()
        {
            SubjectBuilder subjectBuilder = new SubjectBuilder()
                .SetComponent("Level", "1")
                .SetComponent("Symbol", "VOD.L")
                .SetComponent("LiquidityProvider", "Reuters");
            _subjectBuilder = new SubjectBuilder(subjectBuilder);
            Assert.AreEqual(subjectBuilder.ToString(), _subjectBuilder.ToString());
            _subjectBuilder.SetComponent("Account", "FX_ACCT");
            Assert.AreNotEqual(subjectBuilder.ToString(), _subjectBuilder.ToString());
        }

        [Test]
        public void TestSetComponentIfNotNull()
        {
            SubjectBuilder subjectBuilder = new SubjectBuilder()
                .SetIfNotNull("Level", "1")
                .SetIfNotNull("Symbol", null);
            Assert.AreEqual("Level=1", subjectBuilder.ToString());
        }
    }
}