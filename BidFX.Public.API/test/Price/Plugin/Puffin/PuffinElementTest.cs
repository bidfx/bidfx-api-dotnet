using NUnit.Framework;

namespace BidFX.Public.API.Price.Plugin.Puffin
{
    [TestFixture]
    public class PuffinElementTest
    {
        [Test]
        public void empty_element()
        {
            var element = new PuffinElement("Price");
            Assert.False(element.Equals(null));
            Assert.False(element.Equals("Price"));
            Assert.True(element.Equals(element));
            Assert.True(element.Equals(new PuffinElement("Price")));
        }

        [Test]
        public void element_with_number_attributes()
        {
            var element = MessageFormatter.FormatToString(
                new PuffinElement("Price")
                    .AddAttribute("Ask", 12.5)
                    .AddAttribute("AskSize", 1230)
                    .AddAttribute("BidSize", 12400)
            );
            Assert.False(element.Equals(null));
            Assert.False(element.Equals("Price"));
            Assert.True(element.Equals(element));
            Assert.True(element.Equals(MessageFormatter.FormatToString(
                new PuffinElement("Price")
                    .AddAttribute("Ask", 12.5)
                    .AddAttribute("AskSize", 1230)
                    .AddAttribute("BidSize", 12400)
            )));
        }

        [Test]
        public void element_with_string_attributes()
        {
            var element = MessageFormatter.FormatToString(
                new PuffinElement("Price")
                    .AddAttribute("Name", "Sweeno")
                    .AddAttribute("FullName", "Paul A Sweeny")
            );
            Assert.False(element.Equals(null));
            Assert.False(element.Equals("Price"));
            Assert.True(element.Equals(element));
            Assert.True(element.Equals(MessageFormatter.FormatToString(
                new PuffinElement("Price")
                    .AddAttribute("Name", "Sweeno")
                    .AddAttribute("FullName", "Paul A Sweeny")
            )));
        }

        [Test]
        public void element_with_escaped_string_attributes()
        {
            var element = new PuffinElement("Price")
                .AddAttribute("Name", "AT&T")
                .AddAttribute("Quote", "\"quote text\"")
                .AddAttribute("Tag", "<XML>try this</XML>");
            Assert.False(element.Equals(null));
            Assert.False(element.Equals("Price"));
            Assert.True(element.Equals(element));
            Assert.True(element.Equals(new PuffinElement("Price")
                .AddAttribute("Name", "AT&T")
                .AddAttribute("Quote", "\"quote text\"")
                .AddAttribute("Tag", "<XML>try this</XML>")
            ));
        }

        [Test]
        public void nested_sub_elements()
        {
            var element = new PuffinElement("Update")
                .AddAttribute("Subject",
                    "AssetClass=FixedIncome,Exchange=SGC,Level=1,LiquidityProvider=Lynx,Symbol=DE000A14KK32")
                .AddElement(new PuffinElement("Price")
                    .AddAttribute("Ask", 12.5)
                    .AddAttribute("AskSize", 1230)
                    .AddAttribute("BidSize", 12400)
                );
            Assert.False(element.Equals(null));
            Assert.False(element.Equals("Price"));
            Assert.True(element.Equals(element));
            Assert.True(element.Equals(new PuffinElement("Update")
                .AddAttribute("Subject",
                    "AssetClass=FixedIncome,Exchange=SGC,Level=1,LiquidityProvider=Lynx,Symbol=DE000A14KK32")
                .AddElement(new PuffinElement("Price")
                    .AddAttribute("Ask", 12.5)
                    .AddAttribute("AskSize", 1230)
                    .AddAttribute("BidSize", 12400)
                )));
        }
    }
}