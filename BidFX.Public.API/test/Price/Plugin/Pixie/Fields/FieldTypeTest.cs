using NUnit.Framework;

namespace BidFX.Public.API.Price.Plugin.Pixie.Fields
{
    public class FieldTypeTest
    {
        [Test]
        public void TestValueOfCodeReturnsSameType()
        {
            Assert.AreEqual(FieldType.Double, FieldTypeMethods.ValueOf((int) FieldType.Double));
            Assert.AreEqual(FieldType.Integer, FieldTypeMethods.ValueOf((int) FieldType.Integer));
            Assert.AreEqual(FieldType.Long, FieldTypeMethods.ValueOf((int) FieldType.Long));
            Assert.AreEqual(FieldType.String, FieldTypeMethods.ValueOf((int) FieldType.String));
        }

        [Test]
        public void TestValueOfUnrecognisedCodeReturnsUnrecognised()
        {
            Assert.AreEqual(FieldType.Unrecognised, FieldTypeMethods.ValueOf((int) FieldType.Unrecognised));
        }

        [Test]
        public void TestValueOfDiscardCodeReturnsUnrecognised()
        {
            Assert.AreEqual(FieldType.Unrecognised, FieldTypeMethods.ValueOf((int) FieldType.Discard));
        }

        [Test]
        public void TestValueOfInvalidCodeReturnsUnrecognised()
        {
            Assert.AreEqual(FieldType.Unrecognised, FieldTypeMethods.ValueOf('x'));
            Assert.AreEqual(FieldType.Unrecognised, FieldTypeMethods.ValueOf('3'));
            Assert.AreEqual(FieldType.Unrecognised, FieldTypeMethods.ValueOf(','));
        }
    }
}