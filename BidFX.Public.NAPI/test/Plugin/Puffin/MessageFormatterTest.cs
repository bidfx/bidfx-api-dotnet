using BidFX.Public.NAPI.Price.Plugin.Puffin;
using NUnit.Framework;

namespace BidFX.Public.NAPI.test.Plugin.Puffin
{
    [TestFixture]
    public class MessageFormatterTest
    {
        [Test]
        public void empty_element()
        {
            Assert.AreEqual("<Price/>",
                MessageFormatter.FormatToString(new PuffinElement("Price")));
        }

        [Test]
        public void element_with_number_attributes()
        {
            Assert.AreEqual("<Price Ask=\"12.5\" AskSize=\"1230\" BidSize=\"12400\"/>",
                MessageFormatter.FormatToString(
                    new PuffinElement("Price")
                        .AddAttribute("Ask", 12.5)
                        .AddAttribute("AskSize", 1230)
                        .AddAttribute("BidSize", 12400)
                ));
        }

        [Test]
        public void element_with_string_attributes()
        {
            Assert.AreEqual("<Price Name=\"Sweeno\" FullName=\"Paul A Sweeny\"/>",
                MessageFormatter.FormatToString(
                    new PuffinElement("Price")
                        .AddAttribute("Name", "Sweeno")
                        .AddAttribute("FullName", "Paul A Sweeny")
                ));
        }

        [Test]
        public void element_with_escaped_string_attributes()
        {
            Assert.AreEqual(
                "<Price Name=\"AT&amp;T\" Quote=\"&quot;quote text&quot;\" Tag=\"<XML>try this</XML>\"/>",
                MessageFormatter.FormatToString(
                    new PuffinElement("Price")
                        .AddAttribute("Name", "AT&T")
                        .AddAttribute("Quote", "\"quote text\"")
                        .AddAttribute("Tag", "<XML>try this</XML>")
                ));
        }


        [Test]
        public void nested_sub_elements()
        {
            Assert.AreEqual(
                "<Update Subject=\"AssetClass=FixedIncome,Exchange=SGC,Level=1,Source=Lynx,Symbol=DE000A14KK32\">" +
                "<Price Ask=\"12.5\" AskSize=\"1230\" BidSize=\"12400\"/>" +
                "</Update>",
                MessageFormatter.FormatToString(
                    new PuffinElement("Update")
                        .AddAttribute("Subject",
                            "AssetClass=FixedIncome,Exchange=SGC,Level=1,Source=Lynx,Symbol=DE000A14KK32")
                        .AddElement(new PuffinElement("Price")
                                .AddAttribute("Ask", 12.5)
                                .AddAttribute("AskSize", 1230)
                                .AddAttribute("BidSize", 12400)
                        ))
            );
        }
    }
}