using System;
using NUnit.Framework;

namespace TS.Pisa.Plugin.Puffin.Xml
{
    /// <author>Paul Sweeny</author>
    [TestFixture]
    public class XmlElementTest
    {
        public static readonly XmlTag Price = new XmlTag("Price");

        public static readonly XmlTag FxPrice = new XmlTag("FxPrice");
        public static readonly XmlTag FwdPrice = new XmlTag("FwdPrice");

        public static readonly XmlTag Update = new XmlTag("Update");
        public static readonly XmlTag Subject = new XmlTag("Subject");

        public static readonly XmlName Last = new XmlName("Last");
        public static readonly XmlName High = new XmlName("High");

        public static readonly XmlName Low = new XmlName("Low");
        public static readonly XmlName Open = new XmlName("Open");

        public static readonly XmlName Bid = new XmlName("Bid");
        public static readonly XmlName Ask = new XmlName("Ask");

        public static readonly XmlName BidSize = new XmlName("BidSize");
        public static readonly XmlName AskSize = new XmlName("AskSize");

        public static readonly XmlName Volume = new XmlName("Volume");
        public static readonly XmlName Success = new XmlName("Success");

        public static readonly XmlName Name = new XmlName("Name");
        public static readonly XmlName Blank = new XmlName("Blank");

        /// <summary>Construct a new test harness for XmlElement.</summary>
        [Test]
        public virtual void Test_construction()
        {
            XmlElement element = null;
            element = new XmlElement(Price);
            Assert.AreEqual("<Price/>", element.ToString());
            element = new XmlElement("Price");
            Assert.AreEqual("<Price/>", element.ToString());
            element = new XmlElement(FxPrice).Add(Bid, 22);
            Assert.AreEqual("<FxPrice Bid=\"22\"/>", element.ToString());
        }

        [Test]
        public virtual void Test_add()
        {
            XmlElement element = new XmlElement(Price);
            Assert.AreEqual("<Price/>", element.ToString());
            element.Add(Bid, 22);
            Assert.AreEqual("<Price Bid=\"22\"/>", element.ToString());
            element.Add(Bid, 23);
            Assert.AreEqual("<Price Bid=\"23\"/>", element.ToString());
            element.Add(Ask, 24.5);
            element.Add(Name, "Paul Sweeny");
            Assert.AreEqual(element.Get(Bid, -1.0), 23.0, 0.0);
            Assert.AreEqual(element.Get(Ask, -1.0), 24.5, 0.0);
            Assert.AreEqual(element.Get(Open, -1.0), -1.0, 0.0);
            Assert.AreEqual(23, element.Get(Bid, -1));
            Assert.AreEqual(-1, element.Get(Open, -1));
        }

        [Test]
        public virtual void Test_add_String()
        {
            XmlElement element = new XmlElement(Price).Add(Bid, "1234");
            Assert.AreEqual("<Price Bid=\"1234\"/>", element.ToString());
            Assert.AreEqual(element.Get(Bid, -1.0), 0.0, 1234.0);
            Assert.AreEqual(1234, element.Get(Bid, -1));
            Assert.AreEqual("1234", element.Get(Bid, string.Empty));
        }

        [Test]
        public virtual void Test_add_escaped_String()
        {
            XmlElement element = new XmlElement(Price).Add(Name, "AT&T <UK Ltd>");
            Assert.AreEqual("<Price Name=\"AT&amp;T &lt;UK Ltd>\"/>", element.ToString());
            Assert.AreEqual("AT&T <UK Ltd>", element.Get(Name, null));
        }

        [Test]
        public virtual void Test_add_boolean()
        {
            XmlElement element = new XmlElement(Price).Add(Success, true);
            Assert.AreEqual("<Price Success=\"true\"/>", element.ToString());
            Assert.IsTrue(element.Get(Success, false));
            Assert.IsTrue(element.Get(Success, true));
            element.Add(Success, false);
            Assert.AreEqual("<Price Success=\"false\"/>", element.ToString());
            Assert.IsTrue(!element.Get(Success, false));
            Assert.IsTrue(!element.Get(Success, true));
        }

        [Test]
        public virtual void Test_add_element()
        {
            XmlElement nested = new XmlElement(FxPrice).Add(Bid, 22);
            XmlElement element = new XmlElement(Update).Add(nested);
            XmlElement first = element.GetContents().GetFirstElement();
            Assert.AreEqual("<Update><FxPrice Bid=\"22\"/></Update>", element.ToString());
            Assert.AreEqual("<FxPrice Bid=\"22\"/>", first.ToString());
        }

        [Test]
        public virtual void Test_getFirstElement()
        {
            XmlElement nested1 = new XmlElement(FxPrice).Add(Bid, 22);
            XmlElement nested2 = new XmlElement(Price).Add(Ask, 33);
            XmlElement element = new XmlElement(Update).Add(Name, "ABC").Add(nested1).Add(nested2);
            Assert.AreEqual("<Update Name=\"ABC\"><FxPrice Bid=\"22\"/><Price Ask=\"33\"/></Update>", element.ToString());
            try
            {
                XmlElement first = element.GetContents().GetFirstElement();
                Assert.AreEqual("<FxPrice Bid=\"22\"/>", first.ToString());
                Assert.AreEqual(nested1.ToString(), first.ToString());
            }
            catch (Exception)
            {
                Assert.Fail("element.getFirstElement()");
            }
        }

        [Test]
        public virtual void Test_getFirstElement_bad()
        {
            XmlElement element = new XmlElement(FxPrice).Add(Bid, 22);
            try
            {
                element.GetContents().GetFirstElement();
                Assert.Fail("element.getFirstElement() should fail");
            }
            catch (Exception)
            {
            }
        }

        [Test]
        public virtual void Test_add_attribute_with_name_token()
        {
            XmlElement element = new XmlElement(Price);
            for (int nameType = 0; nameType < XmlToken.MaxType; ++nameType)
            {
                XmlToken name = CreateToken(nameType, "bid");
                for (int valueType = 0; valueType < XmlToken.MaxType; ++valueType)
                {
                    XmlToken value = CreateToken(valueType, "22");
                    bool shouldPass = nameType == XmlToken.NameType && value.IsValueType();
                    string desc = "element.add((" + name + "), (" + value + ")) should " +
                                  (shouldPass ? "pass" : "fail");
                    try
                    {
                        element.Add(name, value);
                        Assert.IsTrue(shouldPass, desc);
                    }
                    catch (Exception)
                    {
                        Assert.IsTrue(!shouldPass, desc);
                    }
                }
            }
        }

        [Test]
        public virtual void Test_add_attribute_with_value_token()
        {
            XmlElement element = new XmlElement(Price);
            for (int valueType = 0; valueType < XmlToken.MaxType; ++valueType)
            {
                XmlToken value = CreateToken(valueType, "22");
                bool shouldPass = value.IsValueType();
                string desc = "element.add((" + Bid + "), (" + value + ")) should " + (shouldPass ? "pass" : "fail");
                try
                {
                    element.Add(Bid, value);
                    Assert.IsTrue(shouldPass, desc);
                }
                catch (Exception)
                {
                    Assert.IsTrue(!shouldPass, desc);
                }
            }
        }

        public static XmlToken CreateToken(int type, string s)
        {
            char[] t = s.ToCharArray();
            return new XmlToken(type, t, 0, t.Length);
        }

        public static XmlToken CreateToken(string s)
        {
            return CreateToken(XmlToken.StringValueType, s);
        }

        [Test]
        public virtual void Test_isA()
        {
            XmlElement element = new XmlElement(Price);
            Assert.IsTrue(element.IsA(Price), "element.isA(Price)");
            Assert.IsTrue(!element.IsA(FxPrice), "!element.isA(FxPrice)");
            Assert.IsTrue(!element.IsA(Update), "!element.isA(Update)");
        }

        [Test]
        public virtual void Test_update()
        {
            XmlElement element = new XmlElement(Price).Add(Name, "IBM");
            Assert.AreEqual("IBM", element.Get(Name, string.Empty));
            Assert.AreEqual(0, element.Get(Bid, 0));
            Assert.AreEqual(0, element.Get(Ask, 0));
            Assert.AreEqual(0, element.Get(Open, 0));
            Assert.AreEqual(0, element.Get(High, 0));
            XmlElement update1 = new XmlElement(Update).Add(Ask, 123);
            element.Update(update1, false);
            Assert.AreEqual("IBM", element.Get(Name, string.Empty));
            Assert.AreEqual(0, element.Get(Bid, 0));
            Assert.AreEqual(123, element.Get(Ask, 0));
            Assert.AreEqual(0, element.Get(Open, 0));
            Assert.AreEqual(0, element.Get(High, 0));
            XmlElement update2 = new XmlElement(Update).Add(Bid, 125).Add(Open, 100);
            element.Update(update2, false);
            Assert.AreEqual("IBM", element.Get(Name, string.Empty));
            Assert.AreEqual(125, element.Get(Bid, 0));
            Assert.AreEqual(123, element.Get(Ask, 0));
            Assert.AreEqual(100, element.Get(Open, 0));
            Assert.AreEqual(0, element.Get(High, 0));
            XmlElement update3 = new XmlElement(Update).Add(Ask, 126).Add(Bid, 127);
            element.Update(update3, false);
            Assert.AreEqual("IBM", element.Get(Name, string.Empty));
            Assert.AreEqual(127, element.Get(Bid, 0));
            Assert.AreEqual(126, element.Get(Ask, 0));
            Assert.AreEqual(100, element.Get(Open, 0));
            Assert.AreEqual(0, element.Get(High, 0));
            XmlElement update4 = new XmlElement(Update).Add(Ask, 125).Add(Ask, 124);
            element.Update(update4, false);
            Assert.AreEqual("IBM", element.Get(Name, string.Empty));
            Assert.AreEqual(127, element.Get(Bid, 0));
            Assert.AreEqual(124, element.Get(Ask, 0));
            Assert.AreEqual(100, element.Get(Open, 0));
            Assert.AreEqual(0, element.Get(High, 0));
        }

        [Test]
        public virtual void Test_delta_null()
        {
            XmlElement element = new XmlElement(Price).Add(Name, "IBM").Add(Bid, 22.2).Add(Ask, 22.6).Add(Open, 21);
            Assert.IsNull(element.Delta(element));
        }

        [Test]
        public virtual void Test_delta()
        {
            XmlElement element = new XmlElement(Price).Add(Name, "IBM").Add(Bid, 22.2).Add(Ask, 22.6).Add(Open, 21);
            XmlElement update =
                new XmlElement(Price).Add(Name, "IBM").Add(Bid, 22.3).Add(Ask, 22.7).Add(Last, 22.2).Add(Open, 21);
            XmlElement delta = new XmlElement(Price).Add(Bid, 22.3).Add(Ask, 22.7).Add(Last, 22.2);
            Assert.AreEqual(delta, element.Delta(update));
        }

        [Test]
        public virtual void Test_equals()
        {
            XmlElement e1 = new XmlElement(Price);
            Assert.IsTrue(!e1.Equals(null));
            XmlElement e2 = new XmlElement(Price);
            Assert.IsTrue(e1.Equals(e2));
            XmlElement e3 = new XmlElement(FxPrice);
            Assert.IsTrue(!e1.Equals(e3));
            e1.Add(Name, "IBM").Add(Bid, 127).Add(Ask, 124).Add(Open, 100);
            Assert.IsTrue(!e1.Equals(e2));
            e2.Add(Name, "IBM").Add(Bid, 127).Add(Ask, 124).Add(Open, 100);
            Assert.IsTrue(e1.Equals(e2));
            e1.Add(Bid, 456).Add(Ask, 789).Add(Last, 477);
            Assert.IsTrue(!e1.Equals(e2));
            e2.Add(Last, 477).Add(Ask, 789).Add(Bid, 456);
            Assert.IsTrue(e1.Equals(e2));
            e1.Add(Bid, 888);
            Assert.IsTrue(!e1.Equals(e2));
            e2.Add(Bid, 888);
            Assert.IsTrue(e1.Equals(e2));
            e1.Add(Blank, string.Empty);
            Assert.IsTrue(!e1.Equals(e2));
            e2.Add(Blank, string.Empty);
            Assert.IsTrue(e1.Equals(e2));

            e1.Add(Bid, 888).Add(Ask, 789).Add(Last, 477);
            Assert.IsTrue(e1.Equals(e2));
            e2.Add(Last, 477).Add(Ask, 789).Add(Bid, 888);
            Assert.IsTrue(e1.Equals(e2));
            e1.Add(Bid, 999).Add(Ask, 888).Add(Last, 477);
            Assert.IsTrue(!e1.Equals(e2));
            e2.Add(Last, 477).Add(Ask, 888).Add(Bid, 999);
            Assert.IsTrue(e1.Equals(e2));
        }

        [Test]
        public virtual void Test_getElement()
        {
            var e = new XmlElement(Price).Add(new XmlElement(Subject).Add(Name, "S1"))
                .Add(new XmlElement(FxPrice).Add(Bid, 123).Add(Ask, 456).Add(new XmlElement(FwdPrice).Add(Bid, 78)));
            Assert.AreEqual("S1", e.GetContents().GetElement(Subject).Get(Name, string.Empty));
            Assert.AreEqual("S1", e.GetContents().GetElement("Subject").Get(Name, string.Empty));
            Assert.AreEqual(123, e.GetContents().GetElement(FxPrice).Get(Bid, 0));
            Assert.AreEqual(456, e.GetContents().GetElement(FxPrice).Get(Ask, 0));
            Assert.AreEqual(78, e.GetContents().GetElement(FxPrice).GetContents().GetElement(FwdPrice).Get(Bid, 0));
            Assert.AreEqual(null, e.GetContents().GetElement(Update));
        }
    }
}