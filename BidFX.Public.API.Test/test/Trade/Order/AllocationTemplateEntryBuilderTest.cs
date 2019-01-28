using System;
using NUnit.Framework;

namespace BidFX.Public.API.Trade.Order
{
    public class AllocationTemplateEntryBuilderTest
    {
        private AllocationTemplateEntryBuilder _allocationBuilder;

        [SetUp]
        public void Before()
        {
            _allocationBuilder = new AllocationTemplateEntryBuilder();
        }

        [Test]
        public void TestRatio()
        {
            AllocationTemplateEntry entry = _allocationBuilder.SetRatio(23434343).Build();
            Assert.Equals(23434343, entry.GetRatio());
            
            entry = _allocationBuilder.SetRatio(1.223).Build();
            Assert.Equals(1.223, entry.GetRatio());
        }
        
        [Test]
        public void TestClearingAccount()
        {
            AllocationTemplateEntry entry = _allocationBuilder.SetClearingAccount("FX_ACCT").Build();
            Assert.Equals("FX_ACCT", entry.GetClearingAccount());
            
            entry = _allocationBuilder.SetClearingAccount("FX_ACCT_2    ").Build();
            Assert.Equals("FX_ACCT_2", entry.GetClearingAccount());
            
            entry = _allocationBuilder.SetClearingAccount("     FX_ACCT_3    ").Build();
            Assert.Equals("FX_ACCT_3", entry.GetClearingAccount());
        }

        [Test]
        public void TestEmptyClearingAccountThrowsException()
        {
            ArgumentException exception =
                Assert.Throws<ArgumentException>(() => _allocationBuilder.SetClearingAccount(""));
            Assert.AreEqual("Clearing Account can not be empty", exception.Message);
        }

        [Test]
        public void TestClearingBroker()
        {
            AllocationTemplateEntry entry = _allocationBuilder.SetClearingBroker("FX_ACCT").Build();
            Assert.Equals("FX_ACCT", entry.GetClearingBroker());
            
            entry = _allocationBuilder.SetClearingBroker("FX_ACCT_2    ").Build();
            Assert.Equals("FX_ACCT_2", entry.GetClearingBroker());
            
            entry = _allocationBuilder.SetClearingBroker("     FX_ACCT_3    ").Build();
            Assert.Equals("FX_ACCT_3", entry.GetClearingBroker());
        }
        
        [Test]
        public void TestEmptyClearingBrokerThrowsException()
        {
            ArgumentException exception =
                Assert.Throws<ArgumentException>(() => _allocationBuilder.SetClearingBroker(""));
            Assert.AreEqual("Clearing Broker can not be empty", exception.Message);
        }
    }
}