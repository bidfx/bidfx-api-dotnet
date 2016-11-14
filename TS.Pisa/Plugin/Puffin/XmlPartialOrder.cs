using System;
using TS.Pisa.Orderbook;
using TS.Pisa.Plugin.Puffin.Xml;
using TS.Pisa.Tools.Time;
namespace TS.Pisa.Plugin.Puffin
{
    /// <summary>This class implements a PartialOrder using a Puffin XmlElement.</summary>
    /// <author>Paul Sweeny</author>
    public class XmlPartialOrder : IPartialOrder
    {
        private readonly XmlElement _element;
        private readonly bool _bidSide;

        private long _receivedTime = ApplicationClock.GetDefault().CurrentTimeMillis();
        /// <summary>Creates an Xml order instance.</summary>
        /// <param name="element">the element to provides the fields of the order.</param>
        /// <exception cref="System.ArgumentException">if the element is null or not a valid order.</exception>
        public XmlPartialOrder(XmlElement element)
        {
            if (element == null)
            {
                throw new ArgumentException("null Xml element");
            }
            _element = element;
            _bidSide = IsBidSide(element);
        }
        public string GetOrderID()
        {
            return _element.GetTag().ToString();
        }
        public long GetOrderTime()
        {
            XmlToken token = _element.Get(_bidSide ? PuffinFieldNameConstants.BidTime : IPuffinFieldName.AskTime);
            if (token == null)
            {
                return null;
            }
            return token.ToLong();
        }
        public int GetOrderSize()
        {
            XmlToken token = _element.Get(_bidSide ? IPuffinFieldName.BidSize : IPuffinFieldName.AskSize);
            if (token == null)
            {
                return null;
            }
            return token.ToInteger();
        }
        public double GetOrderPrice()
        {
            XmlToken token = _element.Get(_bidSide ? IPuffinFieldName.Bid : IPuffinFieldName.Ask);
            if (token == null)
            {
                return null;
            }
            return token.ToDouble();
        }
        public OrderSide GetOrderSide()
        {
            return _bidSide ? OrderSide.Bid : OrderSide.Ask;
        }
        public string GetOrderFirm()
        {
            return _element.Get(_bidSide ? IPuffinFieldName.BidFirm : IPuffinFieldName.AskFirm, null);
        }
        public bool IsCancellation()
        {
            return _element.Get(XmlKeyedElement.Delete, false);
        }
        public long GetReceivedTime()
        {
            return _receivedTime;
        }
        public bool IsTopOfBook()
        {
            return _element.Get(PuffinFieldNameConstants.TopOfBook, false);
        }
        /// <exception cref="System.ArgumentException"/>
        public bool IsBidSide(XmlElement element)
        {
            // order search by most likely occurring components first
            if (element.Get(PuffinFieldNameConstants.BidTime) != null)
            {
                return true;
            }
            if (element.Get(IPuffinFieldName.AskTime) != null)
            {
                return false;
            }
            if (element.Get(IPuffinFieldName.BidSize) != null)
            {
                return true;
            }
            if (element.Get(IPuffinFieldName.AskSize) != null)
            {
                return false;
            }
            if (element.Get(IPuffinFieldName.Bid) != null)
            {
                return true;
            }
            if (element.Get(IPuffinFieldName.Ask) != null)
            {
                return false;
            }
            if (element.Get(IPuffinFieldName.BidFirm) != null)
            {
                return true;
            }
            if (element.Get(IPuffinFieldName.AskFirm) != null)
            {
                return false;
            }
            if (IsCancellation())
            {
                return false;
            }
            throw new ArgumentException("element is not an order: " + element);
        }
        public override bool Equals(object o)
        {
            if (this == o)
            {
                return true;
            }
            if (o == null || GetType() != o.GetType())
            {
                return false;
            }
            TS.Pisa.Plugin.Puffin.XmlPartialOrder that = (TS.Pisa.Plugin.Puffin.XmlPartialOrder)o;
            return _element.Equals(that._element);
        }
        public override int GetHashCode()
        {
            return _element.GetHashCode();
        }
        public override string ToString()
        {
            return PartialOrderConstants.Formatter.FormatToString(this);
        }
    }
}
