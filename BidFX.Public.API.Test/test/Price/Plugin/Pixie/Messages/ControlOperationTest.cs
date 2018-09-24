using System;
using NUnit.Framework;

namespace BidFX.Public.API.Price.Plugin.Pixie.Messages
{
    public class ControlOperationTest
    {
        [Test]
        public void TestGetCode()
        {
            Assert.AreEqual('R', ControlOperation.Refresh.GetCode());
            Assert.AreEqual('T', ControlOperation.Toggle.GetCode());
        }

        [Test]
        public void TestValueOf()
        {
            Assert.AreEqual(ControlOperation.Refresh, ControlOperationExtenstions.FromCode('R'));
            Assert.AreEqual(ControlOperation.Toggle, ControlOperationExtenstions.FromCode('T'));
        }

        [Test]
        public void TestValueOfUnknownCode()
        {
            try
            {
                ControlOperationExtenstions.FromCode('X');
                Assert.Fail();
            }
            catch (ArgumentException e)
            {
            }
        }
    }
}