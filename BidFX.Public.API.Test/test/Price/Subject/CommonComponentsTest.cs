using System;
using NUnit.Framework;

namespace BidFX.Public.API.Price.Subject
{
    public class CommonComponentsTest
    {
        [Test]
        public void TestCommonKeyWhenExists()
        {
            string actual = CommonComponents.CommonKeys[0];
            string clone = (string) actual.Clone();
            Assert.AreSame(actual, CommonComponents.CommonKey(clone));
        }

        [Test]
        public void TestCommonKeyWhenDoesntExist()
        {
            Assert.IsNull(CommonComponents.CommonKey("notacommonKey"));
        }

        [Test]
        public void NullKeyInputReturnsNull()
        {
            Assert.IsNull(CommonComponents.CommonKey(null));
        }

        [Test]
        public void TestCommonValueWhenExists()
        {
            string actual = CommonComponents.CommonValues[0];
            string clone = (string) actual.Clone();
            Assert.AreSame(actual, CommonComponents.CommonValue(clone));
        }

        [Test]
        public void TestCommonValueWhenDoesntExist()
        {
            Assert.IsNull(CommonComponents.CommonValue("notacommonValue"));
        }

        [Test]
        public void NullValueInputReturnsNull()
        {
            Assert.IsNull(CommonComponents.CommonValue(null));
        }
    }
}