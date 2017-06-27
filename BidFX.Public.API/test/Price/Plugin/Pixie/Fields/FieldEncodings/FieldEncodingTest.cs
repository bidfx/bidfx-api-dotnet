using System;
using NUnit.Framework;

namespace BidFX.Public.API.Price.Plugin.Pixie.Fields.FieldEncodings
{
    public class FieldEncodingTest
    {
        [Test]
        public void TestValueOfCodeReturnsSameType()
        {
            Assert.AreEqual(typeof(NoopFieldEncoding), FieldEncodingMethods.ValueOf((char)FieldEncoding.Noop).GetFieldEncoding().GetType());
            Assert.AreEqual(typeof(Fixed1FieldEncoding), FieldEncodingMethods.ValueOf((char)FieldEncoding.Fixed1).GetFieldEncoding().GetType());
            Assert.AreEqual(typeof(Fixed2FieldEncoding), FieldEncodingMethods.ValueOf((char)FieldEncoding.Fixed2).GetFieldEncoding().GetType());
            Assert.AreEqual(typeof(Fixed3FieldEncoding), FieldEncodingMethods.ValueOf((char)FieldEncoding.Fixed3).GetFieldEncoding().GetType());
            Assert.AreEqual(typeof(Fixed4FieldEncoding), FieldEncodingMethods.ValueOf((char)FieldEncoding.Fixed4).GetFieldEncoding().GetType());
            Assert.AreEqual(typeof(Fixed8FieldEncoding), FieldEncodingMethods.ValueOf((char)FieldEncoding.Fixed8).GetFieldEncoding().GetType());
            Assert.AreEqual(typeof(Fixed16FieldEncoding), FieldEncodingMethods.ValueOf((char)FieldEncoding.Fixed16).GetFieldEncoding().GetType());
            Assert.AreEqual(typeof(ByteArrayFieldEncoding), FieldEncodingMethods.ValueOf((char)FieldEncoding.ByteArray).GetFieldEncoding().GetType());
            Assert.AreEqual(typeof(VarintFieldEncoding), FieldEncodingMethods.ValueOf((char)FieldEncoding.Varint).GetFieldEncoding().GetType());
            Assert.AreEqual(typeof(VarintStringFieldEncoding), FieldEncodingMethods.ValueOf((char)FieldEncoding.VarintString).GetFieldEncoding().GetType());
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