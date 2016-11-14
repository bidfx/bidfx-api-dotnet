using System;
using TS.Pisa.Orderbook;
using TS.Pisa.Plugin.Puffin.Xml;
namespace TS.Pisa.Plugin.Puffin
{
    /// <summary>Unit test.</summary>
    /// <author>Paul Sweeny</author>
    [TestFixture]
    public class XmlPartialOrderTest
    {
        private XmlElement _element = new XmlElement("Order1").Add(IPuffinFieldName.Bid, 123.45).Add(IPuffinFieldName.BidSize, 1000).Add(IPuffinFieldName.BidTime, 1217405601000L).Add(IPuffinFieldName.BidFirm, "ABC");
        private XmlPartialOrder _XmlElementOrder;

        private XmlPartialOrder _partialAskOrder;
        private XmlPartialOrder _partialBidOrder = new XmlPartialOrder(new XmlElement("Order3").Add(IPuffinFieldName.Bid, 2.5));

        private XmlPartialOrder _cancelledOrder = new XmlPartialOrder(new XmlElement("CancelledOrder").Add(XmlKeyedElement.Delete, "true"));
        private const double Delta = 0.0;

        private XmlPartialOrder CreatePartialOrder()
        {
            return new XmlPartialOrder(new XmlElement("Order2").Add(IPuffinFieldName.AskSize, 3000));
        }
        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestXmlPartialOrder()
        {
            try
            {
                new XmlPartialOrder(null);
                Assert.Fail("Should have thrown an Exception");
            }
            catch (ArgumentException)
            {
            }
        }
        /// <exception cref="System.Exception"/>
        public virtual void EmptyOrderThatIsNotCancellationIsInvalid()
        {
            new XmlPartialOrder(new XmlElement("Order3"));
        }
        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestGetOrderID()
        {
            Assert.AreEqual("Order1", _XmlElementOrder.GetOrderID());
            Assert.AreEqual("Order2", _partialAskOrder.GetOrderID());
            Assert.AreEqual("Order3", _partialBidOrder.GetOrderID());
            Assert.AreEqual("CancelledOrder", _cancelledOrder.GetOrderID());
        }
        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestGetOrderTime()
        {
            Assert.AreEqual(Sharpen.Extensions.ValueOf(1217405601000L), _XmlElementOrder.GetOrderTime());
            Assert.AreEqual(null, _partialAskOrder.GetOrderTime());
            Assert.AreEqual(null, _partialBidOrder.GetOrderTime());
            Assert.AreEqual(null, _cancelledOrder.GetOrderTime());
        }
        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestGetOrderSize()
        {
            Assert.AreEqual(Sharpen.Extensions.ValueOf(1000), _XmlElementOrder.GetOrderSize());
            Assert.AreEqual(Sharpen.Extensions.ValueOf(3000), _partialAskOrder.GetOrderSize());
            Assert.AreEqual(null, _partialBidOrder.GetOrderSize());
            Assert.AreEqual(null, _cancelledOrder.GetOrderSize());
        }
        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestGetOrderPrice()
        {
            Assert.AreEqual(_XmlElementOrder.GetOrderPrice(), Delta, 123.45);
            Assert.AreEqual(null, _partialAskOrder.GetOrderPrice());
            Assert.AreEqual(_partialBidOrder.GetOrderPrice(), Delta, 2.5);
            Assert.AreEqual(null, _cancelledOrder.GetOrderPrice());
        }
        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestGetOrderSide()
        {
            Assert.AreEqual(OrderSide.Bid, _XmlElementOrder.GetOrderSide());
            Assert.AreEqual(OrderSide.Bid, _partialBidOrder.GetOrderSide());
            Assert.AreEqual(OrderSide.Ask, _partialAskOrder.GetOrderSide());
            Assert.AreEqual(OrderSide.Ask, _cancelledOrder.GetOrderSide());
        }
        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestGetOrderFirm()
        {
            Assert.AreEqual("ABC", _XmlElementOrder.GetOrderFirm());
            Assert.AreEqual(null, _partialBidOrder.GetOrderFirm());
            Assert.AreEqual(null, _partialAskOrder.GetOrderFirm());
            Assert.AreEqual(null, _cancelledOrder.GetOrderFirm());
        }
        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestIsBidSide()
        {
            Assert.AreEqual(true, _XmlElementOrder.IsBidSide(_element));
            Assert.AreEqual(true, _XmlElementOrder.IsBidSide(new XmlElement("o").Add(IPuffinFieldName.Bid, 1.0)));
            Assert.AreEqual(true, _XmlElementOrder.IsBidSide(new XmlElement("o").Add(IPuffinFieldName.BidSize, 3000)));
            Assert.AreEqual(true, _XmlElementOrder.IsBidSide(new XmlElement("o").Add(IPuffinFieldName.BidTime, 1217405601000L)));
            Assert.AreEqual(true, _XmlElementOrder.IsBidSide(new XmlElement("o").Add(IPuffinFieldName.BidFirm, "A")));
            Assert.AreEqual(false, _XmlElementOrder.IsBidSide(new XmlElement("o").Add(IPuffinFieldName.Ask, 1.0)));
            Assert.AreEqual(false, _XmlElementOrder.IsBidSide(new XmlElement("o").Add(IPuffinFieldName.AskSize, 3000)));
            Assert.AreEqual(false, _XmlElementOrder.IsBidSide(new XmlElement("o").Add(IPuffinFieldName.AskTime, 1217405601000L)));
            Assert.AreEqual(false, _XmlElementOrder.IsBidSide(new XmlElement("o").Add(IPuffinFieldName.AskFirm, "B")));
        }
        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestIsBidSide_empty()
        {
            try
            {
                _XmlElementOrder.IsBidSide(new XmlElement("EmptyOrder"));
            }
            catch (ArgumentException)
            {
            }
        }
        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestToString()
        {
            Assert.AreEqual("ID:Order1 ABC BID 1000 @ 123.45 2008-07-30T08:13:21.000Z", _XmlElementOrder.ToString());
            Assert.AreEqual("ID:Order2 ASK 3000 @ unknown-price unknown-time", _partialAskOrder.ToString());
            Assert.AreEqual("ID:Order3 BID unknown-size @ 2.5 unknown-time", _partialBidOrder.ToString());
            Assert.AreEqual("ID:CancelledOrder ASK unknown-size @ unknown-price unknown-time cancelled", _cancelledOrder.ToString());
        }
        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestHashCode()
        {
            Assert.AreEqual(_XmlElementOrder.GetHashCode(), _XmlElementOrder.GetHashCode(), "hash never changes");
            Assert.IsTrue(_partialAskOrder.GetHashCode() != _XmlElementOrder.GetHashCode(), "hash different for dissimilar instances");
            Assert.AreEqual(_partialAskOrder.GetHashCode(), CreatePartialOrder().GetHashCode(), "hash same for equal instances");
        }
        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestEquals()
        {
            Assert.AreEqual(false, _XmlElementOrder.Equals(null));
            Assert.AreEqual(false, _XmlElementOrder.Equals(this));
            Assert.AreEqual(false, _XmlElementOrder.Equals(_partialAskOrder));
            Assert.AreEqual(false, _XmlElementOrder.Equals(_element));
            Assert.AreEqual(true, _XmlElementOrder.Equals(_XmlElementOrder));
            Assert.AreEqual(true, _partialAskOrder.Equals(_partialAskOrder));
            Assert.AreEqual(true, _partialAskOrder.Equals(CreatePartialOrder()));
        }
        /// <exception cref="System.Exception"/>
        [Test]
        public virtual void TestIsCancellation()
        {
            Assert.AreEqual(false, _XmlElementOrder.IsCancellation());
            Assert.AreEqual(false, _partialAskOrder.IsCancellation());
            Assert.AreEqual(false, _partialBidOrder.IsCancellation());
            Assert.AreEqual(true, _cancelledOrder.IsCancellation());
        }
        public XmlPartialOrderTest()
        {
            _XmlElementOrder = new XmlPartialOrder(_element);
            _partialAskOrder = CreatePartialOrder();
        }
    }
}
