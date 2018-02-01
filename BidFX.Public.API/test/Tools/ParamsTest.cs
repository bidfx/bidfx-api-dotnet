using Microsoft.SqlServer.Server;
using NUnit.Framework;

namespace BidFX.Public.API.Price.Tools
{
    [TestFixture]
    public class ExactLengthTest
    {
        [Test]
        [ExpectedException("System.ArgumentException", ExpectedMessage = "Expected Exception.")]
        public void TestEmptyStringIsLengthZero()
        {
            Assert.AreEqual("", Params.ExactLength("", 0, "Empty String 0 Threw Exception"));
            Params.ExactLength("", 2, "Expected Exception.");
        }

        [Test]
        [ExpectedException("System.ArgumentException", ExpectedMessage = "Expected Exception.")]
        public void TestBlankStringIsLengthZero()
        {
            Assert.AreEqual("", Params.ExactLength("   ", 0, "Blank String 0 Threw Exception"));
            Params.ExactLength("   ", 2, "Expected Exception.");
        }

        [Test]
        [ExpectedException("System.ArgumentException", ExpectedMessage = "Expected Exception.")]
        public void TestStringLengthSix()
        {
            Assert.AreEqual("STRING", Params.ExactLength("STRING", 6, "\"STRING\" 6 Threw Exception"));
            Params.ExactLength("STRING", 4, "Expected Exception.");
        }

        [Test]
        [ExpectedException("System.ArgumentException", ExpectedMessage = "Expected Exception.")]
        public void TestStringLengthTrailingWhitespaceLengthSize()
        {
            Assert.AreEqual("STRING", Params.ExactLength("STRING     ", 6, "\"STRING     \" 6 Threw Exception"));
            Params.ExactLength("STRING     ", 8, "Expected Exception.");
        }

        [Test]
        [ExpectedException("System.ArgumentException", ExpectedMessage = "Expected Exception.")]
        public void TestStringLengthPrecedingWhitespaceLengthSize()
        {
            Assert.AreEqual("STRING", Params.ExactLength("     STRING", 6, "\"     STRING\" 6 Threw Exception"));
            Params.ExactLength("     STRING", 3, "Expected Exception.");
        }
        
        [Test]
        [ExpectedException("System.ArgumentException", ExpectedMessage = "Expected Exception.")]
        public void TestStringLengthBothEndsWhitespaceLengthSize()
        {
            Assert.AreEqual("STRING", Params.ExactLength("     STRING   ", 6, "\"     STRING   \" 6 Threw Exception"));
            Params.ExactLength("     STRING   ", 2, "Expected Exception.");
        }
        
        [Test]
        [ExpectedException("System.ArgumentException", ExpectedMessage = "Expected Exception.")]
        public void TestNullThrowsException()
        {
            Params.ExactLength(null, 3, "Expected Exception.");
        }
    }

    [TestFixture]
    public class TrimTest
    {
        [Test]
        public void TestEmptyStringReturnsSameObject()
        {
            var expected = "";
            Assert.AreSame(expected, Params.Trim(expected));
        }

        [Test]
        public void TestBlankStringReturnsEmptyString()
        {
            Assert.AreEqual("", Params.Trim("   "));
        }
        
        [Test]
        public void TestStringWithNoWhitespaceReturnsSameObject()
        {
            var expected = "AString";
            Assert.AreSame(expected, Params.Trim(expected));
        }

        [Test]
        public void TestStringWithWhitespaceInMiddleReturnsSameObject()
        {
            var expected = "Another String";
            Assert.AreSame(expected, Params.Trim(expected));
        }

        [Test]
        public void TestTrimsLeadingWhitespace()
        {
            Assert.AreEqual("String", Params.Trim("   String"));
        }

        [Test]
        public void TestTrimsTrailingWhitespace()
        {
            Assert.AreEqual("String", Params.Trim("String   "));
        }

        [Test]
        public void TestTrimsWhitespaceAtBothEnds()
        {
            Assert.AreEqual("String", Params.Trim("   String     "));
        }
    }

    [TestFixture]
    public class IsNumericTest
    {
        [Test]
        public void TestIntegerReturnsTrue()
        {
            Assert.IsTrue(Params.IsNumeric("1234567890"));
        }

        [Test]
        public void TestDecimalReturnsTrue()
        {
            Assert.IsTrue(Params.IsNumeric("1234.056789"));
        }

        [Test]
        public void TestNotNumericReturnsFalse()
        {
            Assert.IsFalse(Params.IsNumeric("Hello, World!"));
        }
        
        [Test]
        public void TestNumbersWithMultiplePeriodsReturnsFalse()
        {
            Assert.IsFalse(Params.IsNumeric("123.456.7890"));
        }
    }

    [TestFixture]
    public class IsNullOrEmptyTest
    {
        [Test]
        public void TestNullReturnsTrue()
        {
            Assert.IsTrue(Params.IsNullOrEmpty(null));
        }
        
        [Test]
        public void TestEmptyReturnsTrue()
        {
            Assert.IsTrue(Params.IsNullOrEmpty(""));
        }
        
        [Test]
        public void TestLargeBlankReturnsTrue()
        {
            Assert.IsTrue(Params.IsNullOrEmpty("          "));
        }
        
        [Test]
        public void TestBlankReturnsTrue()
        {
            Assert.IsTrue(Params.IsNullOrEmpty(" "));
        }
        
        [Test]
        public void TestNotEmptyReturnsTrue()
        {
            Assert.IsFalse(Params.IsNullOrEmpty("SomeVariable"));
        }
    }
}