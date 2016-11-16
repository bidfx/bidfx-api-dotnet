using System;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace TS.Pisa.Plugin.Puffin.Xml
{
    /// <author>Paul Sweeny</author>
    [TestFixture]
    public class BinaryReaderTest
    {
        [Test]
        public virtual void level_one_price()
        {
            var data =
                "\u0002Update\u0004Subject\bAssetClass=FixedIncome,Exchange=SGC,Level=1,Source=Lynx,Symbol=DE000A14KK32" +
                "\u0002Price\u0004Ask\u0006102.5\u0004Bid\u0006100.5\u0004BidSize\u00051000\u0004Name" +
                "\bVodafone plc\u0004AskSize\u00053000\u0001\u0000";
            var stream = new MemoryStream(ToLatinBytes(data));
            var tokenizer = new BinaryReader(stream);
            Assert.AreEqual(
                new XmlElement("Update")
                    .AddAttribute("Subject",
                        "AssetClass=FixedIncome,Exchange=SGC,Level=1,Source=Lynx,Symbol=DE000A14KK32")
                    .AddElement(new XmlElement("Price")
                            .AddAttribute("Ask", 102.5)
                            .AddAttribute("Bid", 100.5)
                            .AddAttribute("BidSize", 1000)
                            .AddAttribute("Name", "Vodafone plc")
                            .AddAttribute("AskSize", 3000)
                    ), tokenizer.NextElement());
        }

        [Test]
        public virtual void repeat_level_one_price()
        {
            var data =
                "\u0002Update\u0004Subject\bAssetClass=FixedIncome,Exchange=SGC,Level=1,Source=Lynx,Symbol=DE000A14KK32" +
                "\u0002Price\u0004Ask\u0006102.5\u0004Bid\u0006100.5\u0004BidSize\u00051000\u0004Name\bVodafone plc" +
                "\u0004AskSize\u00053000\u0001\u0000" +
                "\u0080\u0081\u0082\u0083\u0084\u0085\u0086\u0087\u0088\u0089\u008A\u008B\u008C\u008D\u0001\u0000";
            var stream = new MemoryStream(ToLatinBytes(data));
            var tokenizer = new BinaryReader(stream);

            var element1 = tokenizer.NextElement();
            var element2 = tokenizer.NextElement();
            Assert.AreEqual(
                new XmlElement("Update")
                    .AddAttribute("Subject",
                        "AssetClass=FixedIncome,Exchange=SGC,Level=1,Source=Lynx,Symbol=DE000A14KK32")
                    .AddElement(new XmlElement("Price")
                            .AddAttribute("Ask", 102.5)
                            .AddAttribute("Bid", 100.5)
                            .AddAttribute("BidSize", 1000)
                            .AddAttribute("Name", "Vodafone plc")
                            .AddAttribute("AskSize", 3000)
                    ), element1);

            Assert.AreEqual(
                new XmlElement("Update")
                    .AddAttribute("Subject",
                        "AssetClass=FixedIncome,Exchange=SGC,Level=1,Source=Lynx,Symbol=DE000A14KK32")
                    .AddElement(new XmlElement("Price")
                            .AddAttribute("Ask", 102.5)
                            .AddAttribute("Bid", 100.5)
                            .AddAttribute("BidSize", 1000)
                            .AddAttribute("Name", "Vodafone plc")
                            .AddAttribute("AskSize", 3000)
                    ), element2);
        }

        private static byte[] ToLatinBytes(string data)
        {
            return Encoding.GetEncoding(28591).GetBytes(data);
        }

        [Test]
        public virtual void depth_price()
        {
            var data =
                "\u0002Set\u0004Subject\u0008AssetClass=Equity,Exchange=LSE,Level=Depth,Source=ComStoc" +
                "k,Symbol=E:VOD\u0002Price\u0004Name\u0008Vodafone plc\u0004Bid1\u0006199\u0004Ask1\u0006" +
                "201\u0004BidSize1\u0005999\u0004AskSize1\u00051001\u0004Bid2\u0006198\u0004Ask2\u0006" +
                "202\u0004BidSize2\u0005998\u0004AskSize2\u00051002\u0004Bid3\u0006197\u0004Ask3\u0006" +
                "203\u0004BidSize3\u0005997\u0004AskSize3\u00051003\u0004Bid4\u0006196\u0004Ask4\u0006" +
                "204\u0004BidSize4\u0005996\u0004AskSize4\u00051004\u0004Bid5\u0006195\u0004Ask5\u0006" +
                "205\u0004BidSize5\u0005995\u0004AskSize5\u00051005\u0004Bid6\u0006194\u0004Ask6\u0006" +
                "206\u0004BidSize6\u0005994\u0004AskSize6\u00051006\u0004Bid7\u0006193\u0004Ask7\u0006" +
                "207\u0004BidSize7\u0005993\u0004AskSize7\u00051007\u0004Bid8\u0006192\u0004Ask8\u0006" +
                "208\u0004BidSize8\u0005992\u0004AskSize8\u00051008\u0004Bid9\u0006191\u0004Ask9\u0006" +
                "209\u0004BidSize9\u0005991\u0004AskSize9\u00051009\u0001\u0000";
            var stream = new MemoryStream(ToLatinBytes(data));
            var tokenizer = new BinaryReader(stream);

            var price = new XmlElement("Price").AddAttribute("Name", "Vodafone plc");
            for (var i = 1; i < 10; ++i)
            {
                price.AddAttribute("Bid" + i, 200.0 - i)
                    .AddAttribute("Ask" + i, 200.0 + i)
                    .AddAttribute("BidSize" + i, 1000 - i)
                    .AddAttribute("AskSize" + i, 1000 + i);
            }
            var element = new XmlElement("Set")
                .AddAttribute("Subject",
                    "AssetClass=Equity,Exchange=LSE,Level=Depth,Source=ComStock,Symbol=E:VOD")
                .AddElement(price);
            Assert.AreEqual(element, tokenizer.NextElement());
        }

        [Test]
        public virtual void repeat_depth_price()
        {
            var data =
                "\u0002Set\u0004Subject\u0008AssetClass=Equity,Exchange=LSE,Level=Depth,Source=ComStoc" +
                "k,Symbol=E:VOD\u0002Price\u0004Name\u0008Vodafone plc\u0004Bid1\u0006199\u0004Ask1\u0006" +
                "201\u0004BidSize1\u0005999\u0004AskSize1\u00051001\u0004Bid2\u0006198\u0004Ask2\u0006" +
                "202\u0004BidSize2\u0005998\u0004AskSize2\u00051002\u0004Bid3\u0006197\u0004Ask3\u0006" +
                "203\u0004BidSize3\u0005997\u0004AskSize3\u00051003\u0004Bid4\u0006196\u0004Ask4\u0006" +
                "204\u0004BidSize4\u0005996\u0004AskSize4\u00051004\u0004Bid5\u0006195\u0004Ask5\u0006" +
                "205\u0004BidSize5\u0005995\u0004AskSize5\u00051005\u0004Bid6\u0006194\u0004Ask6\u0006" +
                "206\u0004BidSize6\u0005994\u0004AskSize6\u00051006\u0004Bid7\u0006193\u0004Ask7\u0006" +
                "207\u0004BidSize7\u0005993\u0004AskSize7\u00051007\u0004Bid8\u0006192\u0004Ask8\u0006" +
                "208\u0004BidSize8\u0005992\u0004AskSize8\u00051008\u0004Bid9\u0006191\u0004Ask9\u0006" +
                "209\u0004BidSize9\u0005991\u0004AskSize9\u00051009\u0004Bid10\u0006190\u0004Ask10\u0006" +
                "210\u0004BidSize10\u0005990\u0004AskSize10\u00051010\u0004Bid11\u0006189\u0004Ask11\u0006" +
                "211\u0004BidSize11\u0005989\u0004AskSize11\u00051011\u0004Bid12\u0006188\u0004Ask12\u0006" +
                "212\u0004BidSize12\u0005988\u0004AskSize12\u00051012\u0004Bid13\u0006187\u0004Ask13\u0006" +
                "213\u0004BidSize13\u0005987\u0004AskSize13\u00051013\u0004Bid14\u0006186\u0004Ask14\u0006" +
                "214\u0004BidSize14\u0005986\u0004AskSize14\u00051014\u0004Bid15\u0006185\u0004Ask15\u0006" +
                "215\u0004BidSize15\u0005985\u0004AskSize15\u00051015\u0004Bid16\u0006184\u0004Ask16\u0006" +
                "216\u0004BidSize16\u0005984\u0004AskSize16\u00051016\u0004Bid17\u0006183\u0004Ask17\u0006" +
                "217\u0004BidSize17\u0005983\u0004AskSize17\u00051017\u0004Bid18\u0006182\u0004Ask18\u0006" +
                "218\u0004BidSize18\u0005982\u0004AskSize18\u00051018\u0004Bid19\u0006181\u0004Ask19\u0006" +
                "219\u0004BidSize19\u0005981\u0004AskSize19\u00051019\u0004Bid20\u0006180\u0004Ask20\u0006" +
                "220\u0004BidSize20\u0005980\u0004AskSize20\u00051020\u0004Bid21\u0006179\u0004Ask21\u0006" +
                "221\u0004BidSize21\u0005979\u0004AskSize21\u00051021\u0004Bid22\u0006178\u0004Ask22\u0006" +
                "222\u0004BidSize22\u0005978\u0004AskSize22\u00051022\u0004Bid23\u0006177\u0004Ask23\u0006" +
                "223\u0004BidSize23\u0005977\u0004AskSize23\u00051023\u0004Bid24\u0006176\u0004Ask24\u0006" +
                "224\u0004BidSize24\u0005976\u0004AskSize24\u00051024\u0004Bid25\u0006175\u0004Ask25\u0006" +
                "225\u0004BidSize25\u0005975\u0004AskSize25\u00051025\u0004Bid26\u0006174\u0004Ask26\u0006" +
                "226\u0004BidSize26\u0005974\u0004AskSize26\u00051026\u0004Bid27\u0006173\u0004Ask27\u0006" +
                "227\u0004BidSize27\u0005973\u0004AskSize27\u00051027\u0004Bid28\u0006172\u0004Ask28\u0006" +
                "228\u0004BidSize28\u0005972\u0004AskSize28\u00051028\u0004Bid29\u0006171\u0004Ask29\u0006" +
                "229\u0004BidSize29\u0005971\u0004AskSize29\u00051029\u0001\u0000\u0002Update\u0081\u0082" +
                "\u0083\u0084\u0085\u0086\u0087\u0088\u0089\u008a\u008b\u008c\u008d\u008e\u008f\u0090\u0091" +
                "\u0092\u0093\u0094\u0095\u0096\u0097\u0098\u0099\u009a\u009b\u009c\u009d\u009e\u009f\u00a0" +
                "\u00a1\u00a2\u00a3\u00a4\u00a5\u00a6\u00a7\u00a8\u00a9\u00aa\u00ab\u00ac\u00ad\u00ae\u00af" +
                "\u00b0\u00b1\u00b2\u00b3\u00b4\u00b5\u00b6\u00b7\u00b8\u00b9\u00ba\u00bb\u00bc\u00bd\u00be" +
                "\u00bf\u00c0\u00c1\u00c2\u00c3\u00c4\u00c5\u00c6\u00c7\u00c8\u00c9\u00ca\u00cb\u00cc\u00cd" +
                "\u00ce\u00cf\u00d0\u00d1\u00d2\u00d3\u00d4\u00d5\u00d6\u00d7\u00d8\u00d9\u00da\u00db\u00dc" +
                "\u00dd\u00de\u00df\u00e0\u00e1\u00e2\u00e3\u00e4\u00e5\u00e6\u00e7\u00e8\u00e9\u00ea\u00eb" +
                "\u00ec\u00ed\u00ee\u00ef\u00f0\u00f1\u00f2\u00f3\u00f4\u00f5\u00f6\u00f7\u00f8\u00f9\u00fa" +
                "\u00fb\u00fc\u00fd\u00fe\u00ff\u0080\n\u0081\n\u0082\n\u0083\n\u0084\n\u0085\n\u0086\n" +
                "\u0087\n\u0088\n\u0089\n\u008a\n\u008b\n\u008c\n\u008d\n\u008e\n\u008f\n\u0090\n\u0091" +
                "\n\u0092\n\u0093\n\u0094\n\u0095\n\u0096\n\u0097\n\u0098\n\u0099\n\u009a\n\u009b\n\u009c" +
                "\n\u009d\n\u009e\n\u009f\n\u00a0\n\u00a1\n\u00a2\n\u00a3\n\u00a4\n\u00a5\n\u00a6\n\u00a7" +
                "\n\u00a8\n\u00a9\n\u00aa\n\u00ab\n\u00ac\n\u00ad\n\u00ae\n\u00af\n\u00b0\n\u00b1\n\u00b2" +
                "\n\u00b3\n\u00b4\n\u00b5\n\u00b6\n\u00b7\n\u00b8\n\u00b9\n\u00ba\n\u00bb\n\u00bc\n\u00bd" +
                "\n\u00be\n\u00bf\n\u00c0\n\u00c1\n\u00c2\n\u00c3\n\u00c4\n\u00c5\n\u00c6\n\u00c7\n\u00c8" +
                "\n\u00c9\n\u00ca\n\u00cb\n\u00cc\n\u00cd\n\u00ce\n\u00cf\n\u00d0\n\u00d1\n\u00d2\n\u00d3" +
                "\n\u00d4\n\u00d5\n\u00d6\n\u00d7\n\u00d8\n\u00d9\n\u00da\n\u00db\n\u00dc\n\u00dd\n\u00de" +
                "\n\u00df\n\u00e0\n\u00e1\n\u00e2\n\u00e3\n\u00e4\n\u00e5\n\u00e6\n\u00e7\n\u00e8\n\u00e9" +
                "\n\u00ea\n\u00eb\n\u00ec\n\u00ed\n\u0001\u0000\u00ee\n\u0081\u0082\u0083\u0084\u0085\u0086" +
                "\u0087\u0088\u0089\u008a\u008b\u008c\u008d\u008e\u008f\u0090\u0091\u0092\u0093\u0094\u0095" +
                "\u0096\u0097\u0098\u0099\u009a\u009b\u009c\u009d\u009e\u009f\u00a0\u00a1\u00a2\u00a3\u00a4" +
                "\u00a5\u00a6\u00a7\u00a8\u00a9\u00aa\u00ab\u00ac\u00ad\u00ae\u00af\u00b0\u00b1\u00b2\u00b3" +
                "\u00b4\u00b5\u00b6\u00b7\u00b8\u00b9\u00ba\u00bb\u00bc\u00bd\u00be\u00bf\u00c0\u00c1\u00c2" +
                "\u00c3\u00c4\u00c5\u00c6\u00c7\u00c8\u00c9\u00ca\u00cb\u00cc\u00cd\u00ce\u00cf\u00d0\u00d1" +
                "\u00d2\u00d3\u00d4\u00d5\u00d6\u00d7\u00d8\u00d9\u00da\u00db\u00dc\u00dd\u00de\u00df\u00e0" +
                "\u00e1\u00e2\u00e3\u00e4\u00e5\u00e6\u00e7\u00e8\u00e9\u00ea\u00eb\u00ec\u00ed\u00ee\u00ef" +
                "\u00f0\u00f1\u00f2\u00f3\u00f4\u00f5\u00f6\u00f7\u00f8\u00f9\u00fa\u00fb\u00fc\u00fd\u00fe" +
                "\u00ff\u0080\u0081\n\u0082\n\u0083\n\u0084\n\u0085\n\u0086\n\u0087\n\u0088\n\u0089\n\u008a" +
                "\n\u008b\n\u008c\n\u008d\n\u008e\n\u008f\n\u0090\n\u0091\n\u0092\n\u0093\n\u0094\n\u0095" +
                "\n\u0096\n\u0097\n\u0098\n\u0099\n\u009a\n\u009b\n\u009c\n\u009d\n\u009e\n\u009f\n\u00a0" +
                "\n\u00a1\n\u00a2\n\u00a3\n\u00a4\n\u00a5\n\u00a6\n\u00a7\n\u00a8\n\u00a9\n\u00aa\n\u00ab" +
                "\n\u00ac\n\u00ad\n\u00ae\n\u00af\n\u00b0\n\u00b1\n\u00b2\n\u00b3\n\u00b4\n\u00b5\n\u00b6" +
                "\n\u00b7\n\u00b8\n\u00b9\n\u00ba\n\u00bb\n\u00bc\n\u00bd\n\u00be\n\u00bf\n\u00c0\n\u00c1" +
                "\n\u00c2\n\u00c3\n\u00c4\n\u00c5\n\u00c6\n\u00c7\n\u00c8\n\u00c9\n\u00ca\n\u00cb\n\u00cc" +
                "\n\u00cd\n\u00ce\n\u00cf\n\u00d0\n\u00d1\n\u00d2\n\u00d3\n\u00d4\n\u00d5\n\u00d6\n\u00d7" +
                "\n\u00d8\n\u00d9\n\u00da\n\u00db\n\u00dc\n\u00dd\n\u00de\n\u00df\n\u00e0\n\u00e1\n\u00e2" +
                "\n\u00e3\n\u00e4\n\u00e5\n\u00e6\n\u00e7\n\u00e8\n\u00e9\n\u00ea\n\u00eb\n\u00ec\n\u00ed" +
                "\n\u0001\u0000";
            var stream = new MemoryStream(ToLatinBytes(data));
            var tokenizer = new BinaryReader(stream);

            var price = new XmlElement("Price").AddAttribute("Name", "Vodafone plc");
            for (var i = 1; i < 30; ++i)
            {
                price.AddAttribute("Bid" + i, 200.0 - i)
                    .AddAttribute("Ask" + i, 200.0 + i)
                    .AddAttribute("BidSize" + i, 1000 - i)
                    .AddAttribute("AskSize" + i, 1000 + i);
            }
            var subject = "AssetClass=Equity,Exchange=LSE,Level=Depth,Source=ComStock,Symbol=E:VOD";
            Assert.AreEqual(new XmlElement("Set")
                .AddAttribute("Subject", subject).AddElement(price), tokenizer.NextElement());
            Assert.AreEqual(new XmlElement("Update")
                .AddAttribute("Subject", subject).AddElement(price), tokenizer.NextElement());
            Assert.AreEqual(new XmlElement("Update")
                .AddAttribute("Subject", subject).AddElement(price), tokenizer.NextElement());
        }
    }
}