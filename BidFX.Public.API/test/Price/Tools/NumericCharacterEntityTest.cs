using NUnit.Framework;

namespace BidFX.Public.API.Price.Tools
{
    public class NumericCharacterEntityTest
    {
        private NumericCharacterEntity mNumericCharacterEntity = new NumericCharacterEntity();

        [SetUp]
        public void SetUp()
        {
            mNumericCharacterEntity.AddCharacterEncoding('<');
            mNumericCharacterEntity.AddCharacterEncoding(' ');
        }

//        [Test]
//        public void encodeFirst300Characters()
//        {
//            mNumericCharacterEntity.AddCharacterEncodingRange(0, 32);
//            char[] chars = new char[300];
//            for (int i = 0; i < chars.length; ++i)
//            {
//                chars[i] = (char) i;
//            }
//            string string = new string(chars);
//            string encoded = mNumericCharacterEntity.EncodeString(string);
//            Assert.AreEqual(string, mNumericCharacterEntity.decodestring(encoded));
//        }
//
//        [Test]
//        public void testCharacterEncodingRange()
//        {
//            mNumericCharacterEntity.AddCharacterEncodingRange(0, 32);
//            char[] chars = new char[64];
//            for (int i = 0; i < chars.length; ++i)
//            {
//                chars[i] = (char) i;
//            }
//            string string = new string(chars);
//            string encoded = mNumericCharacterEntity.EncodeString(string);
//
//            Assert.AreEqual(string, mNumericCharacterEntity.decodestring(encoded));
//            Assert.AreEqual("&#0;&#1;&#2;&#3;&#4;&#5;&#6;&#7;&#8;&#9;&#10;&#11;&#12;&#13;&#14;&#15;&#16;" +
//                         "&#17;&#18;&#19;&#20;&#21;&#22;&#23;&#24;&#25;&#26;&#27;&#28;&#29;&#30;&#31;&#32;" +
//                         "!\"#$%&#38;'()*+,-./0123456789:;&#60;=>?", encoded);
//
//            // performanceTest(string, encoded);
//        }
//
//        private void performanceTest(string string, string encoded)
//        {
//            long start = (long) DateTime.UtcNow.TotalMilliseconds;
//            int tests = 100000;
//
//            long last = (long) DateTime.UtcNow.TotalMilliseconds;
//            for (int i = 0; i < tests; ++i)
//            {
//                mNumericCharacterEntity.EncodeString("");
//                mNumericCharacterEntity.EncodeString("not a lot here");
//                mNumericCharacterEntity.EncodeString("Source=Reuters,AssetClass=Equity,Exchange=LSE,Symbol=ABC");
//                mNumericCharacterEntity.EncodeString("Source=ICE,AssetClass=Future,Exchange=ICE,Symbol=ABC DEF");
//                mNumericCharacterEntity.EncodeString("not sure why w\u02FF would do this");
//                mNumericCharacterEntity.EncodeString("testing testing\n1 2 3\n");
//                mNumericCharacterEntity.EncodeString(string);
//            }
//            System.out.println("EncodeString: " + TimeInterval.tostring((long) DateTime.UtcNow.TotalMilliseconds - last));
//
//
//            last = (long) DateTime.UtcNow.TotalMilliseconds;
//            for (int i = 0; i < tests; ++i)
//            {
//                mNumericCharacterEntity.decodestring("");
//                mNumericCharacterEntity.decodestring("not a lot here");
//                mNumericCharacterEntity.decodestring("Source=Reuters,AssetClass=Equity,Exchange=LSE,Symbol=ABC");
//                mNumericCharacterEntity.decodestring(
//                    "Source=Reuters,AssetClass=Equity,Exchange=LSE,Symbol=ABC&#32;DEF");
//                mNumericCharacterEntity.decodestring("not&#32;sure&#32;why&#32;w\u02FF&#32;would&#32;do&#32;this");
//                mNumericCharacterEntity.decodestring("testing&#32;testing&#10;1&#32;2&#32;3&#10;");
//                mNumericCharacterEntity.decodestring(encoded);
//            }
//            System.out.println("decodestring: " + TimeInterval.tostring((long) DateTime.UtcNow.TotalMilliseconds - last));
//
//            System.out.println("overall time: " + TimeInterval.tostring((long) DateTime.UtcNow.TotalMilliseconds - start));
//        }
//
//
//        [Test]
//        public void testAddCharacterEncoding()
//        {
//            mNumericCharacterEntity.AddCharacterEncoding('[');
//            mNumericCharacterEntity.AddCharacterEncoding(']');
//            Assert.AreEqual("array&#91;123&#93;&#32;&#38;&#32;2", mNumericCharacterEntity.EncodeString("array[123] & 2"));
//        }

        [Test]
        public void testEncodeString()
        {
//            Assert.Null(mNumericCharacterEntity.EncodeString(null));
            Assert.AreEqual("nothing", mNumericCharacterEntity.EncodeString("nothing"));
            Assert.AreEqual("this&#38;that", mNumericCharacterEntity.EncodeString("this&that"));
            Assert.AreEqual("&#60;xml>data&#32;goes&#32;here&#60;/xml>",
                mNumericCharacterEntity.EncodeString("<xml>data goes here</xml>"));
            Assert.AreEqual("&#38;&#60;&#32;", mNumericCharacterEntity.EncodeString("&< "));
            Assert.AreEqual("&#38;#38;&#38;#60;&#38;#32;", mNumericCharacterEntity.EncodeString(
                mNumericCharacterEntity.EncodeString("&< ")));
            Assert.AreEqual("a&#32;few\nlines&#32;of\ntext\n",
                mNumericCharacterEntity.EncodeString("a few\nlines of\ntext\n"));
        }

//
//        [Test]
//        public void testDecodestring()
//        {
//            assertNull(mNumericCharacterEntity.decodestring(null));
//            Assert.AreEqual("\001", mNumericCharacterEntity.decodestring("&#1;"));
//            Assert.AreEqual("\001", mNumericCharacterEntity.decodestring("&#01;"));
//            Assert.AreEqual("\001", mNumericCharacterEntity.decodestring("&#001;"));
//            Assert.AreEqual("\r\n", mNumericCharacterEntity.decodestring("&#13;&#10;"));
//            Assert.AreEqual("nothing here", mNumericCharacterEntity.decodestring("nothing here"));
//            Assert.AreEqual("this&that", mNumericCharacterEntity.decodestring("this&that"));
//            Assert.AreEqual("this&that", mNumericCharacterEntity.decodestring("this&#38;that"));
//            Assert.AreEqual("<xml>data goes here</xml>",
//                mNumericCharacterEntity.decodestring("&#60;xml>data&#32;goes&#32;here&#60;/xml>"));
//            Assert.AreEqual("&< ", mNumericCharacterEntity.decodestring("&#38;&#60;&#32;"));
//            Assert.AreEqual("&#38;&#60;&#32;", mNumericCharacterEntity.decodestring("&#38;#38;&#38;#60;&#38;#32;"));
//        }
//
//        [Test]
//        public void aPartialEntityMatch()
//        {
//            Assert.AreEqual("test ing", mNumericCharacterEntity.decodestring("test&#32;ing"));
//            Assert.AreEqual("test&#32ing", mNumericCharacterEntity.decodestring("test&#32ing"));
//            Assert.AreEqual("test&#3ing", mNumericCharacterEntity.decodestring("test&#3ing"));
//            Assert.AreEqual("test&#ing", mNumericCharacterEntity.decodestring("test&#ing"));
//            Assert.AreEqual("test&ing", mNumericCharacterEntity.decodestring("test&ing"));
//        }
//
//        [Test]
//        public void aPartialEntityMatchFollowedByMatch()
//        {
//            Assert.AreEqual("test&#32 ing", mNumericCharacterEntity.decodestring("test&#32&#32;ing"));
//            Assert.AreEqual("test&#3 ing", mNumericCharacterEntity.decodestring("test&#3&#32;ing"));
//            Assert.AreEqual("test&# ing", mNumericCharacterEntity.decodestring("test&#&#32;ing"));
//            Assert.AreEqual("test& ing", mNumericCharacterEntity.decodestring("test&&#32;ing"));
//        }
//
//        [Test]
//        public void aPartialEntityMatchAtEndOfText()
//        {
//            Assert.AreEqual("test ", mNumericCharacterEntity.decodestring("test&#32;"));
//            Assert.AreEqual("test&#3", mNumericCharacterEntity.decodestring("test&#3"));
//            Assert.AreEqual("test&#", mNumericCharacterEntity.decodestring("test&#"));
//            Assert.AreEqual("test&", mNumericCharacterEntity.decodestring("test&"));
//            Assert.AreEqual("test&#32", mNumericCharacterEntity.decodestring("test&#32"));
//            Assert.AreEqual("test&#1234", mNumericCharacterEntity.decodestring("test&#1234"));
//        }
    }
}