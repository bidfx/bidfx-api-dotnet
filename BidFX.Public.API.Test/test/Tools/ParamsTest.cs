using System;
using Microsoft.SqlServer.Server;
using NUnit.Framework;

namespace BidFX.Public.API.Price.Tools
{
    [TestFixture]
    public class ExactLengthTest
    {
        [Test]
        public void TestEmptyStringIsLengthZero()
        {
            Assert.AreEqual("", Params.ExactLength("", 0, "Empty String 0 Threw Exception"));
            ArgumentException exception = Assert.Throws<ArgumentException>(() => Params.ExactLength("", 2, "Expected Exception."));
            Assert.AreEqual("Expected Exception.", exception.Message);
        }

        [Test]
        public void TestBlankStringIsLengthZero()
        {
            Assert.AreEqual("", Params.ExactLength("   ", 0, "Blank String 0 Threw Exception"));
            ArgumentException exception = Assert.Throws<ArgumentException>(() => Params.ExactLength("   ", 2, "Expected Exception."));
            Assert.AreEqual("Expected Exception.", exception.Message);
        }

        [Test]
        public void TestStringLengthSix()
        {
            Assert.AreEqual("STRING", Params.ExactLength("STRING", 6, "\"STRING\" 6 Threw Exception"));
            ArgumentException exception =
                Assert.Throws<ArgumentException>(() => Params.ExactLength("STRING", 4, "Expected Exception."));
            Assert.AreEqual("Expected Exception.", exception.Message);
        }

        [Test]
        public void TestStringLengthTrailingWhitespaceLengthSize()
        {
            Assert.AreEqual("STRING", Params.ExactLength("STRING     ", 6, "\"STRING     \" 6 Threw Exception"));
            ArgumentException exception = Assert.Throws<ArgumentException>(() => Params.ExactLength("STRING     ", 8, "Expected Exception."));
            Assert.AreEqual("Expected Exception.", exception.Message);
        }

        [Test]
        public void TestStringLengthPrecedingWhitespaceLengthSize()
        {
            Assert.AreEqual("STRING", Params.ExactLength("     STRING", 6, "\"     STRING\" 6 Threw Exception"));
            ArgumentException exception = Assert.Throws<ArgumentException>(() => Params.ExactLength("     STRING", 3, "Expected Exception."));
            Assert.AreEqual("Expected Exception.", exception.Message);
        }

        [Test]
        public void TestStringLengthBothEndsWhitespaceLengthSize()
        {
            Assert.AreEqual("STRING", Params.ExactLength("     STRING   ", 6, "\"     STRING   \" 6 Threw Exception"));
            ArgumentException exception = Assert.Throws<ArgumentException>(() => Params.ExactLength("     STRING   ", 2, "Expected Exception."));
            Assert.AreEqual("Expected Exception.", exception.Message);
        }

        [Test]
        public void TestNullThrowsException()
        {
            ArgumentException exception = Assert.Throws<ArgumentException>(() => Params.ExactLength(null, 3, "Expected Exception."));
            Assert.AreEqual("Expected Exception.", exception.Message);
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