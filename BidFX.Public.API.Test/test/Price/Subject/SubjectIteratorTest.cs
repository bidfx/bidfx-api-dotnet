using NUnit.Framework;

namespace BidFX.Public.API.Price.Subject
{
    public class SubjectIteratorTest
    {
        private SubjectIterator _subjectIterator;

        [SetUp]
        public void Before()
        {
            _subjectIterator = new SubjectIterator(new[] {"a", "1", "b", "2", "c", "3"});
        }

        [Test]
        public void TestACommonIteration()
        {
            Assert.IsTrue(_subjectIterator.HasNext());
            Assert.IsTrue(_subjectIterator.HasNext());
            Assert.IsTrue(_subjectIterator.MoveNext());
            Assert.AreEqual(new SubjectComponent("a", "1"), _subjectIterator.Current);
            Assert.IsTrue(_subjectIterator.HasNext());
            Assert.IsTrue(_subjectIterator.MoveNext());
            Assert.AreEqual(new SubjectComponent("b", "2"), _subjectIterator.Current);
            Assert.IsTrue(_subjectIterator.HasNext());
            Assert.IsTrue(_subjectIterator.MoveNext());
            Assert.AreEqual(new SubjectComponent("c", "3"), _subjectIterator.Current);
            Assert.IsFalse(_subjectIterator.HasNext());
            Assert.IsFalse(_subjectIterator.MoveNext());
        }

        [Test]
        public void TestIterateOfEnd()
        {
            Assert.IsTrue(_subjectIterator.MoveNext());
            Assert.IsTrue(_subjectIterator.MoveNext());
            Assert.IsTrue(_subjectIterator.MoveNext());
            Assert.IsFalse(_subjectIterator.MoveNext());
        }
    }
}