using System;
using NUnit.Framework;

namespace BidFX.Public.API.Price.Plugin.Pixie.Fields
{
    public class FieldEncodingTest
    {
        [Test]
        public void TestValueOfCodeReturnsSameType()
        {
            Assert.AreEqual(FieldEncoding.Noop, FieldEncodingMethods.ValueOf((char)FieldEncoding.Noop));
            Assert.AreEqual(FieldEncoding.Fixed1, FieldEncodingMethods.ValueOf((char)FieldEncoding.Fixed1));
            Assert.AreEqual(FieldEncoding.Fixed2, FieldEncodingMethods.ValueOf((char)FieldEncoding.Fixed2));
            Assert.AreEqual(FieldEncoding.Fixed3, FieldEncodingMethods.ValueOf((char)FieldEncoding.Fixed3));
            Assert.AreEqual(FieldEncoding.Fixed4, FieldEncodingMethods.ValueOf((char)FieldEncoding.Fixed4));
            Assert.AreEqual(FieldEncoding.Fixed8, FieldEncodingMethods.ValueOf((char)FieldEncoding.Fixed8));
            Assert.AreEqual(FieldEncoding.Fixed16, FieldEncodingMethods.ValueOf((char)FieldEncoding.Fixed16));
            Assert.AreEqual(FieldEncoding.ByteArray, FieldEncodingMethods.ValueOf((char)FieldEncoding.ByteArray));
            Assert.AreEqual(FieldEncoding.Varint, FieldEncodingMethods.ValueOf((char)FieldEncoding.Varint));
            Assert.AreEqual(FieldEncoding.VarintString, FieldEncodingMethods.ValueOf((char)FieldEncoding.VarintString));
        }

        [Test]
        public void TestValueOfInvalidCodeReturnsUnrecognised()
        {
            try
            {
                FieldEncodingMethods.ValueOf('x');
                Assert.Fail("should throw exception");
            }
            catch (ArgumentException e)
            {
                
            }
        }
    }
}