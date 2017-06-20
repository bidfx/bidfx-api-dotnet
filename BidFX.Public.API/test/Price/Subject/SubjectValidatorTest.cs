using NUnit.Framework;

namespace BidFX.Public.API.Price.Subject
{
    public class SubjectValidatorTest
    {
        [Test]
        public void TestValidSubjectKeys()
        {
            var part = SubjectPart.Key;
            try
            {
                SubjectValidator.ValidatePart("a", part);
                SubjectValidator.ValidatePart("Source", part);
                SubjectValidator.ValidatePart("AssetClass", part);
                SubjectValidator.ValidatePart("a12", part);
                SubjectValidator.ValidatePart("underscore_is_allowed", part);
                SubjectValidator.ValidatePart("numbers2", part);
                SubjectValidator.ValidatePart("12345", part);
                SubjectValidator.ValidatePart("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ_abcdefghijklmnopqrstuvwxyz", part);
            }
            catch (IllegalSubjectException e)
            {
                Assert.Fail("test should no throw InvalidSubjectException");
            }
        }

        [Test]
        public void TestValidValues()
        {
            var part = SubjectPart.Value;
            try
            {
                SubjectValidator.ValidatePart("LSE", part);
                SubjectValidator.ValidatePart("Reuters", part);
                SubjectValidator.ValidatePart("123", part);
                SubjectValidator.ValidatePart("14.56", part);
                SubjectValidator.ValidatePart("-12.66+e6", part);
                SubjectValidator.ValidatePart("spaces are permitted", part);
                SubjectValidator.ValidatePart("([A-Z]*|otc)", part);
                SubjectValidator.ValidatePart(" !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^" +
                                              "_`abcdefghijklmnopqrstuvwxyz{|}~", part);
            }
            catch (IllegalSubjectException e)
            {
                Assert.Fail("test should no throw " + e);
            }
        }

        [Test]
        public void TestValidEncodedValues()
        {
            var part = SubjectPart.Encoded;
            try
            {
                SubjectValidator.ValidatePart("LSE", part);
                SubjectValidator.ValidatePart("spaces&#32;are&#32;permitted", part);
                SubjectValidator.ValidatePart("newline&#10;", part);
                SubjectValidator.ValidatePart("!\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^" +
                                              "_`abcdefghijklmnopqrstuvwxyz{|}~", part);
            }
            catch (IllegalSubjectException e)
            {
                Assert.Fail("test should no throw " + e);
            }
        }

        [Test]
        public void KeyMayNotContainNonAlphanumericChars()
        {
            Assert.Throws<IllegalSubjectException>(() => SubjectValidator.ValidatePart("question?", SubjectPart.Key));
        }

        [Test]
        public void EncodedValueMayNotContainSpaceChars()
        {
            Assert.Throws<IllegalSubjectException>(() =>
                SubjectValidator.ValidatePart("this has a space", SubjectPart.Encoded));
        }

        [Test]
        public void KeyMayNotContainNonAsciiChars()
        {
            Assert.Throws<IllegalSubjectException>(() => SubjectValidator.ValidatePart("\u009f", SubjectPart.Key));
        }

        [Test]
        public void ValueMayNotContainNonAsciiChars()
        {
            Assert.Throws<IllegalSubjectException>(() => SubjectValidator.ValidatePart("\u00ff", SubjectPart.Value));
        }

        [Test]
        public void EncodedValueMayNotContainNonAsciiChars()
        {
            Assert.Throws<IllegalSubjectException>(() => SubjectValidator.ValidatePart("\u0080", SubjectPart.Encoded));
        }
    }
}