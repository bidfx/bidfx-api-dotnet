using System;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace TS.Pisa.Plugin.Puffin.Xml
{
    /// <author>Paul Sweeny</author>
    [TestFixture]
    public class XmlInflateTokenizerTest
    {
        [Test]
        public virtual void level_one_price()
        {
            String data =
                "\u0002Update\u0004Subject\bAssetClass=FixedIncome,Exchange=SGC,Level=1,Source=Lynx,Symbol=DE000A14KK32" +
                "\u0002Price\u0004Ask\u0006102.5\u0004Bid\u0006100.5\u0004BidSize\u00051000\u0004Name" +
                "\bVodafone plc\u0004AskSize\u00053000\u0001\u0000";
            var stream = new MemoryStream(ToLatinBytes(data));
            var tokenizer = new XmlInflateTokenizer(stream);
            Assert.AreEqual(
                new XmlElement("Update")
                    .Add(new XmlName("Subject"),
                        "AssetClass=FixedIncome,Exchange=SGC,Level=1,Source=Lynx,Symbol=DE000A14KK32")
                    .Add(new XmlElement("Price")
                        .Add(new XmlName("Bid"), 100.5)
                        .Add(new XmlName("Ask"), 102.5)
                        .Add(new XmlName("BidSize"), 1000)
                        .Add(new XmlName("AskSize"), 3000)
                        .Add(new XmlName("Name"), "Vodafone plc")),
                tokenizer.NextElement());
        }

        [Test]
        public virtual void repeat_level_one_price()
        {
            String data =
                "\u0002Update\u0004Subject\bAssetClass=FixedIncome,Exchange=SGC,Level=1,Source=Lynx,Symbol=DE000A14KK32" +
                "\u0002Price\u0004Ask\u0006102.5\u0004Bid\u0006100.5\u0004BidSize\u00051000\u0004Name\bVodafone plc" +
                "\u0004AskSize\u00053000\u0001\u0000" +
                "\u0080\u0081\u0082\u0083\u0084\u0085\u0086\u0087\u0088\u0089\u008A\u008B\u008C\u008D\u0001\u0000";
            var stream = new MemoryStream(ToLatinBytes(data));
            var tokenizer = new XmlInflateTokenizer(stream);

            var element1 = tokenizer.NextElement();
            var element2 = tokenizer.NextElement();
            Assert.AreEqual(
                new XmlElement("Update")
                    .Add(new XmlName("Subject"),
                        "AssetClass=FixedIncome,Exchange=SGC,Level=1,Source=Lynx,Symbol=DE000A14KK32")
                    .Add(new XmlElement("Price")
                        .Add(new XmlName("Bid"), 100.5)
                        .Add(new XmlName("Ask"), 102.5)
                        .Add(new XmlName("BidSize"), 1000)
                        .Add(new XmlName("AskSize"), 3000)
                        .Add(new XmlName("Name"), "Vodafone plc")),
                element1);

            Assert.AreEqual(
                new XmlElement("Update")
                    .Add(new XmlName("Subject"),
                        "AssetClass=FixedIncome,Exchange=SGC,Level=1,Source=Lynx,Symbol=DE000A14KK32")
                    .Add(new XmlElement("Price")
                        .Add(new XmlName("Bid"), 100.5)
                        .Add(new XmlName("Ask"), 102.5)
                        .Add(new XmlName("BidSize"), 1000)
                        .Add(new XmlName("AskSize"), 3000)
                        .Add(new XmlName("Name"), "Vodafone plc")),
                element2);
        }

        private static byte[] ToLatinBytes(string data)
        {
            return Encoding.GetEncoding(28591).GetBytes(data);
        }

        [Test]
        public virtual void depth_price()
        {
            String data =
                "\u0002Set\u0004Subject\bAssetClass=Equity,Exchange=LSE,Level=Depth,Source=ComStock,Symbol=E:VOD" +
                "\u0002Price\u0004Name\bVodafone plc\u0004Bid1\u0006199\u0004Ask1\u0006201\u0004BidSize1\u00051001" +
                "\u0004Bid2\u0006198\u0004Ask2\u0006202\u0004BidSize2\u00051002\u0004Bid3\u0006197\u0004Ask3\u0006203" +
                "\u0004BidSize3\u00051003\u0004Bid4\u0006196\u0004Ask4\u0006204\u0004BidSize4\u00051004\u0004Bid5" +
                "\u0006195\u0004Ask5\u0006205\u0004BidSize5\u00051005\u0004Bid6\u0006194\u0004Ask6\u0006206\u0004BidSize6" +
                "\u00051006\u0004Bid7\u0006193\u0004Ask7\u0006207\u0004BidSize7\u00051007\u0004Bid8\u0006192\u0004Ask8" +
                "\u0006208\u0004BidSize8\u00051008\u0004Bid9\u0006191\u0004Ask9\u0006209\u0004BidSize9\u00051009\u0001\u0000";
            var stream = new MemoryStream(ToLatinBytes(data));
            var tokenizer = new XmlInflateTokenizer(stream);

            var price = new XmlElement("Price").Add(new XmlName("Name"), "Vodafone plc");
            for (var i = 1; i < 10; ++i)
            {
                price.Add(new XmlName("Bid" + i), 200.0 - i)
                    .Add(new XmlName("Ask" + i), 200.0 + i)
                    .Add(new XmlName("BidSize" + i), 1000 - i)
                    .Add(new XmlName("BidSize" + i), 1000 + i);
            }
            var element = new XmlElement("Set")
                .Add(new XmlName("Subject"),
                    "AssetClass=Equity,Exchange=LSE,Level=Depth,Source=ComStock,Symbol=E:VOD")
                .Add(price);
            Assert.AreEqual(element, tokenizer.NextElement());
        }

        [Test]
        public virtual void repeat_depth_price()
        {
            String data =
                "\u0002Set\u0004Subject\u0008AssetClass=Equity,Exchange=LSE,Level=Depth,Source=ComStoc" +
                "k,Symbol=E:VOD\u0002Price\u0004Name\u0008Vodafone plc\u0004Bid1\u0006199\u0004Ask1\u0006" +
                "201\u0004BidSize1\u00051001\u0004Bid2\u0006198\u0004Ask2\u0006202\u0004BidSize2\u0005" +
                "1002\u0004Bid3\u0006197\u0004Ask3\u0006203\u0004BidSize3\u00051003\u0004Bid4\u0006196" +
                "\u0004Ask4\u0006204\u0004BidSize4\u00051004\u0004Bid5\u0006195\u0004Ask5\u0006205\u0004" +
                "BidSize5\u00051005\u0004Bid6\u0006194\u0004Ask6\u0006206\u0004BidSize6\u00051006\u0004" +
                "Bid7\u0006193\u0004Ask7\u0006207\u0004BidSize7\u00051007\u0004Bid8\u0006192\u0004Ask8" +
                "\u0006208\u0004BidSize8\u00051008\u0004Bid9\u0006191\u0004Ask9\u0006209\u0004BidSize9" +
                "\u00051009\u0004Bid10\u0006190\u0004Ask10\u0006210\u0004BidSize10\u00051010\u0004Bid1" +
                "1\u0006189\u0004Ask11\u0006211\u0004BidSize11\u00051011\u0004Bid12\u0006188\u0004Ask1" +
                "2\u0006212\u0004BidSize12\u00051012\u0004Bid13\u0006187\u0004Ask13\u0006213\u0004BidS" +
                "ize13\u00051013\u0004Bid14\u0006186\u0004Ask14\u0006214\u0004BidSize14\u00051014\u0004" +
                "Bid15\u0006185\u0004Ask15\u0006215\u0004BidSize15\u00051015\u0004Bid16\u0006184\u0004" +
                "Ask16\u0006216\u0004BidSize16\u00051016\u0004Bid17\u0006183\u0004Ask17\u0006217\u0004" +
                "BidSize17\u00051017\u0004Bid18\u0006182\u0004Ask18\u0006218\u0004BidSize18\u00051018\u0004" +
                "Bid19\u0006181\u0004Ask19\u0006219\u0004BidSize19\u00051019\u0004Bid20\u0006180\u0004" +
                "Ask20\u0006220\u0004BidSize20\u00051020\u0004Bid21\u0006179\u0004Ask21\u0006221\u0004" +
                "BidSize21\u00051021\u0004Bid22\u0006178\u0004Ask22\u0006222\u0004BidSize22\u00051022\u0004" +
                "Bid23\u0006177\u0004Ask23\u0006223\u0004BidSize23\u00051023\u0004Bid24\u0006176\u0004" +
                "Ask24\u0006224\u0004BidSize24\u00051024\u0004Bid25\u0006175\u0004Ask25\u0006225\u0004" +
                "BidSize25\u00051025\u0004Bid26\u0006174\u0004Ask26\u0006226\u0004BidSize26\u00051026\u0004" +
                "Bid27\u0006173\u0004Ask27\u0006227\u0004BidSize27\u00051027\u0004Bid28\u0006172\u0004" +
                "Ask28\u0006228\u0004BidSize28\u00051028\u0004Bid29\u0006171\u0004Ask29\u0006229\u0004" +
                "BidSize29\u00051029\u0001\u0000\u0002Update\u0081\u0082\u0083\u0084\u0085\u0086\u0087" +
                "\u0088\u0089\u008a\u008b\u008c\u008d\u008e\u008f\u0090\u0091\u0092\u0093\u0094\u0095\u0096" +
                "\u0097\u0098\u0099\u009a\u009b\u009c\u009d\u009e\u009f\u00a0\u00a1\u00a2\u00a3\u00a4\u00a5" +
                "\u00a6\u00a7\u00a8\u00a9\u00aa\u00ab\u00ac\u00ad\u00ae\u00af\u00b0\u00b1\u00b2\u00b3\u00b4" +
                "\u00b5\u00b6\u00b7\u00b8\u00b9\u00ba\u00bb\u00bc\u00bd\u00be\u00bf\u00c0\u00c1\u00c2\u00c3" +
                "\u00c4\u00c5\u00c6\u00c7\u00c8\u00c9\u00ca\u00cb\u00cc\u00cd\u00ce\u00cf\u00d0\u00d1\u00d2" +
                "\u00d3\u00d4\u00d5\u00d6\u00d7\u00d8\u00d9\u00da\u00db\u00dc\u00dd\u00de\u00df\u00e0\u00e1" +
                "\u00e2\u00e3\u00e4\u00e5\u00e6\u00e7\u00e8\u00e9\u00ea\u00eb\u00ec\u00ed\u00ee\u00ef\u00f0" +
                "\u00f1\u00f2\u00f3\u00f4\u00f5\u00f6\u00f7\u00f8\u00f9\u00fa\u00fb\u00fc\u00fd\u00fe\u00ff" +
                "\u0080\n\u0081\n\u0082\n\u0083\n\u0084\n\u0085\n\u0086\n\u0087\n\u0088\n\u0089\n\u008a" +
                "\n\u008b\n\u008c\n\u008d\n\u008e\n\u008f\n\u0090\n\u0091\n\u0092\n\u0093\n\u0094\n\u0095" +
                "\n\u0096\n\u0097\n\u0098\n\u0099\n\u009a\n\u009b\n\u009c\n\u009d\n\u009e\n\u009f\n\u00a0" +
                "\n\u00a1\n\u00a2\n\u00a3\n\u00a4\n\u00a5\n\u00a6\n\u00a7\n\u00a8\n\u00a9\n\u00aa\n\u00ab" +
                "\n\u00ac\n\u00ad\n\u00ae\n\u00af\n\u00b0\n\u00b1\n\u00b2\n\u00b3\n\u0001\u0000\u00b4\n" +
                "\u0081\u0082\u0083\u0084\u0085\u0086\u0087\u0088\u0089\u008a\u008b\u008c\u008d\u008e\u008f" +
                "\u0090\u0091\u0092\u0093\u0094\u0095\u0096\u0097\u0098\u0099\u009a\u009b\u009c\u009d\u009e" +
                "\u009f\u00a0\u00a1\u00a2\u00a3\u00a4\u00a5\u00a6\u00a7\u00a8\u00a9\u00aa\u00ab\u00ac\u00ad" +
                "\u00ae\u00af\u00b0\u00b1\u00b2\u00b3\u00b4\u00b5\u00b6\u00b7\u00b8\u00b9\u00ba\u00bb\u00bc" +
                "\u00bd\u00be\u00bf\u00c0\u00c1\u00c2\u00c3\u00c4\u00c5\u00c6\u00c7\u00c8\u00c9\u00ca\u00cb" +
                "\u00cc\u00cd\u00ce\u00cf\u00d0\u00d1\u00d2\u00d3\u00d4\u00d5\u00d6\u00d7\u00d8\u00d9\u00da" +
                "\u00db\u00dc\u00dd\u00de\u00df\u00e0\u00e1\u00e2\u00e3\u00e4\u00e5\u00e6\u00e7\u00e8\u00e9" +
                "\u00ea\u00eb\u00ec\u00ed\u00ee\u00ef\u00f0\u00f1\u00f2\u00f3\u00f4\u00f5\u00f6\u00f7\u00f8" +
                "\u00f9\u00fa\u00fb\u00fc\u00fd\u00fe\u00ff\u0080\u0081\n\u0082\n\u0083\n\u0084\n\u0085" +
                "\n\u0086\n\u0087\n\u0088\n\u0089\n\u008a\n\u008b\n\u008c\n\u008d\n\u008e\n\u008f\n\u0090" +
                "\n\u0091\n\u0092\n\u0093\n\u0094\n\u0095\n\u0096\n\u0097\n\u0098\n\u0099\n\u009a\n\u009b" +
                "\n\u009c\n\u009d\n\u009e\n\u009f\n\u00a0\n\u00a1\n\u00a2\n\u00a3\n\u00a4\n\u00a5\n\u00a6" +
                "\n\u00a7\n\u00a8\n\u00a9\n\u00aa\n\u00ab\n\u00ac\n\u00ad\n\u00ae\n\u00af\n\u00b0\n\u00b1" +
                "\n\u00b2\n\u00b3\n\u0001\u0000";
            var stream = new MemoryStream(ToLatinBytes(data));
            var tokenizer = new XmlInflateTokenizer(stream);

            var price = new XmlElement("Price").Add(new XmlName("Name"), "Vodafone plc");
            for (var i = 1; i < 30; ++i)
            {
                price.Add(new XmlName("Bid" + i), 200.0 - i)
                    .Add(new XmlName("Ask" + i), 200.0 + i)
                    .Add(new XmlName("BidSize" + i), 1000 - i)
                    .Add(new XmlName("BidSize" + i), 1000 + i);
            }
            var subject = "AssetClass=Equity,Exchange=LSE,Level=Depth,Source=ComStock,Symbol=E:VOD";
            Assert.AreEqual(new XmlElement("Set")
                .Add(new XmlName("Subject"), subject).Add(price), tokenizer.NextElement());
            Assert.AreEqual(new XmlElement("Update")
                .Add(new XmlName("Subject"), subject).Add(price), tokenizer.NextElement());
            Assert.AreEqual(new XmlElement("Update")
                .Add(new XmlName("Subject"), subject).Add(price), tokenizer.NextElement());
        }
    }
}