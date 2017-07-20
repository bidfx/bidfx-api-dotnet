using System;
using Moq;
using NUnit.Framework;

namespace BidFX.Public.API.Price.Subject
{
    public class SubjectFormatterTest
    {
        private SubjectFormatter _subjectFormatter = new SubjectFormatter();
        private Mock<IComponentHandler> _mockHandler = new Mock<IComponentHandler>();

        private IComponentHandler Handler
        {
            get { return _mockHandler.Object; }
        }

        [SetUp]
        public void Before()
        {
            _subjectFormatter = new SubjectFormatter();
            _mockHandler = new Mock<IComponentHandler>();
        }

        [Test]
        public void NullIsAnInvalidSubject()
        {
            Assert.Throws<ArgumentNullException>(() => _subjectFormatter.ParseSubject(null, Handler));
        }

        [Test]
        public void DoNotAllowANullHandler()
        {
            Assert.Throws<ArgumentNullException>(() => _subjectFormatter.ParseSubject("A=1", null));
        }

        [Test]
        public void AllSubjectComponentsAreParsedInTurn()
        {
            _subjectFormatter.ParseSubject(
                "AssetClass=Equity,Exchange=LSE,Level=1,LiquidityProvider=Reuters,Symbol=ABC",
                Handler);
            _mockHandler.Verify(handler => handler.SubjectComponent("AssetClass", "Equity"));
            _mockHandler.Verify(handler => handler.SubjectComponent("Exchange", "LSE"));
            _mockHandler.Verify(handler => handler.SubjectComponent("Level", "1"));
            _mockHandler.Verify(handler => handler.SubjectComponent("LiquidityProvider", "Reuters"));
            _mockHandler.Verify(handler => handler.SubjectComponent("Symbol", "ABC"));
        }

        [Test]
        public void NumericKeysArePermitted()
        {
            _subjectFormatter.ParseSubject("test22=1,f25=2,88=twofatladies", Handler);
            _mockHandler.Verify(handler => handler.SubjectComponent("test22", "1"));
            _mockHandler.Verify(handler => handler.SubjectComponent("f25", "2"));
            _mockHandler.Verify(handler => handler.SubjectComponent("88", "twofatladies"));
        }

        [Test]
        public void ValuesMayContainValueSeparator()
        {
            _subjectFormatter.ParseSubject("a=b=", Handler);
            _mockHandler.Verify(handler => handler.SubjectComponent("a", "b="));
            _subjectFormatter.ParseSubject("a==", Handler);
            _subjectFormatter.ParseSubject("a=b=c", Handler);
        }

        [Test]
        public void ValuesMayHaveHtmlEscapedSpaces()
        {
            _subjectFormatter.ParseSubject("key1=has&#32;space,key2=has&#32;a&#32;few&#32;spaces", Handler);
            _mockHandler.Verify(handler => handler.SubjectComponent("key1", "has space"));
            _mockHandler.Verify(handler => handler.SubjectComponent("key2", "has a few spaces"));
        }

        [Test]
        public void ValuesMayHaveHtmlEscapedComponentSeparators()
        {
            _subjectFormatter.ParseSubject("key=1&#44;2&#44;3&#44;4", Handler);
            _mockHandler.Verify(handler => handler.SubjectComponent("key", "1,2,3,4"));
        }

        [Test]
        public void UseSpaceAsSeparatorForSubjectFilter()
        {
            var formatter = new SubjectFormatter(' ', '=');
            formatter.ParseSubject("LiquidityProvider=Reuters|ComStock Exchange=[A-Z]{3} AssetClass=Future|Option",
                Handler);
            _mockHandler.Verify(handler => handler.SubjectComponent("AssetClass", "Future|Option"));
            _mockHandler.Verify(handler => handler.SubjectComponent("Exchange", "[A-Z]{3}"));
            _mockHandler.Verify(handler => handler.SubjectComponent("LiquidityProvider", "Reuters|ComStock"));
        }

        [Test]
        public void BlankIsAnInvalidSubject()
        {
            Assert.Throws<IllegalSubjectException>(() => _subjectFormatter.ParseSubject("", Handler));
        }

        [Test]
        public void ValueMustNotContainComponentSeparator()
        {
            Assert.Throws<IllegalSubjectException>(() => _subjectFormatter.ParseSubject("Key=one,two", Handler));
        }
    }
}