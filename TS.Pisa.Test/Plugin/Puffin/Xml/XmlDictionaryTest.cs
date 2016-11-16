using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace TS.Pisa.Plugin.Puffin.Xml
{
    public class XmlDictionaryTest
    {

    }

    [TestFixture]
    public class test_byte_for_being_1st_byte_of_a_token
    {
        [Test]
        public void letters_are_not_tokens()
        {
            Assert.False(XmlDictionary.IsFirstByteOfToken((byte) 'a'));
            Assert.False(XmlDictionary.IsFirstByteOfToken((byte) 's'));
            Assert.False(XmlDictionary.IsFirstByteOfToken((byte) 'c'));
            Assert.False(XmlDictionary.IsFirstByteOfToken((byte) 'i'));
            Assert.False(XmlDictionary.IsFirstByteOfToken((byte) 'z'));
            Assert.False(XmlDictionary.IsFirstByteOfToken((byte) 'A'));
            Assert.False(XmlDictionary.IsFirstByteOfToken((byte) 'Z'));
        }

        [Test]
        public void digits_are_not_tokens()
        {
            Assert.False(XmlDictionary.IsFirstByteOfToken((byte) '0'));
            Assert.False(XmlDictionary.IsFirstByteOfToken((byte) '5'));
            Assert.False(XmlDictionary.IsFirstByteOfToken((byte) '9'));
        }

        [Test]
        public void white_space_chars_are_not_tokens()
        {
            Assert.False(XmlDictionary.IsFirstByteOfToken((byte) ' '));
            Assert.False(XmlDictionary.IsFirstByteOfToken((byte) '\t'));
            Assert.False(XmlDictionary.IsFirstByteOfToken((byte) '\n'));
            Assert.False(XmlDictionary.IsFirstByteOfToken((byte) '\r'));
        }

        [Test]
        public void punctuations_chars_are_not_tokens()
        {
            Assert.False(XmlDictionary.IsFirstByteOfToken((byte) '.'));
            Assert.False(XmlDictionary.IsFirstByteOfToken((byte) ';'));
            Assert.False(XmlDictionary.IsFirstByteOfToken((byte) ','));
            Assert.False(XmlDictionary.IsFirstByteOfToken((byte) '/'));
        }

        [Test]
        public void bytes_with_bit_7_set_are_tokens()
        {
            for (var i = 128; i < 256; ++i)
            {
                Assert.True(XmlDictionary.IsFirstByteOfToken((byte) i));
            }
        }

        [Test]
        public void bytes_with_bit_7_unset_are_not_tokens()
        {
            for (var i = 0; i < 128; ++i)
            {
                Assert.False(XmlDictionary.IsFirstByteOfToken((byte) i));
            }
        }
    }

    [TestFixture]
    public class test_byte_for_being_plain_text
    {
        [Test]
        public void letters_are_plain_text()
        {
            Assert.True(XmlDictionary.IsPlainText((byte) 'a'));
            Assert.True(XmlDictionary.IsPlainText((byte) 's'));
            Assert.True(XmlDictionary.IsPlainText((byte) 'c'));
            Assert.True(XmlDictionary.IsPlainText((byte) 'i'));
            Assert.True(XmlDictionary.IsPlainText((byte) 'z'));
            Assert.True(XmlDictionary.IsPlainText((byte) 'A'));
            Assert.True(XmlDictionary.IsPlainText((byte) 'Z'));
        }

        [Test]
        public void digits_are_plain_text()
        {
            Assert.True(XmlDictionary.IsPlainText((byte) '0'));
            Assert.True(XmlDictionary.IsPlainText((byte) '5'));
            Assert.True(XmlDictionary.IsPlainText((byte) '9'));
        }

        [Test]
        public void white_space_chars_are_plain_text()
        {
            Assert.True(XmlDictionary.IsPlainText((byte) ' '));
            Assert.True(XmlDictionary.IsPlainText((byte) '\t'));
            Assert.True(XmlDictionary.IsPlainText((byte) '\n'));
            Assert.True(XmlDictionary.IsPlainText((byte) '\r'));
        }

        [Test]
        public void punctuations_chars_are_plain_text()
        {
            Assert.True(XmlDictionary.IsPlainText((byte) '.'));
            Assert.True(XmlDictionary.IsPlainText((byte) ';'));
            Assert.True(XmlDictionary.IsPlainText((byte) ','));
            Assert.True(XmlDictionary.IsPlainText((byte) '/'));
        }

        [Test]
        public void token_codes_are_not_plain_text()
        {
            Assert.False(XmlDictionary.IsPlainText((byte) XmlTokenType.TagEnd));
            Assert.False(XmlDictionary.IsPlainText((byte) XmlTokenType.TagEndEmptyContent));
            Assert.False(XmlDictionary.IsPlainText((byte) XmlTokenType.TagStart));
            Assert.False(XmlDictionary.IsPlainText((byte) XmlTokenType.NestedContent));
            Assert.False(XmlDictionary.IsPlainText((byte) XmlTokenType.AttributeName));
            Assert.False(XmlDictionary.IsPlainText((byte) XmlTokenType.AttributeValueInteger));
            Assert.False(XmlDictionary.IsPlainText((byte) XmlTokenType.AttributeValueDouble));
            Assert.False(XmlDictionary.IsPlainText((byte) XmlTokenType.AttributeValueFraction));
            Assert.False(XmlDictionary.IsPlainText((byte) XmlTokenType.AttributeValueString));
        }

        [Test]
        public void bytes_with_bit_7_set_are_not_plain_text()
        {
            Assert.False(XmlDictionary.IsPlainText(128));
            Assert.False(XmlDictionary.IsPlainText(129));
            for (var i = 128; i < 256; ++i)
            {
                Assert.False(XmlDictionary.IsPlainText((byte) i));
            }
        }

        [Test]
        public void bytes_with_bit_7_unset_are_plain_text()
        {
            for (var i = Enum.GetNames(typeof(XmlTokenType)).Length; i < 128; ++i)
            {
                Assert.True(XmlDictionary.IsPlainText((byte) i));
            }
        }
    }

    [TestFixture]
    public class test_byte_for_being_2nd_byte_of_a_token
    {
        [Test]
        public void letters_are_token()
        {
            Assert.True(XmlDictionary.IsSecondByteOfToken((byte) 'a'));
            Assert.True(XmlDictionary.IsSecondByteOfToken((byte) 's'));
            Assert.True(XmlDictionary.IsSecondByteOfToken((byte) 'c'));
            Assert.True(XmlDictionary.IsSecondByteOfToken((byte) 'i'));
            Assert.True(XmlDictionary.IsSecondByteOfToken((byte) 'z'));
            Assert.True(XmlDictionary.IsSecondByteOfToken((byte) 'A'));
            Assert.True(XmlDictionary.IsSecondByteOfToken((byte) 'Z'));
        }

        [Test]
        public void digits_are_token()
        {
            Assert.True(XmlDictionary.IsSecondByteOfToken((byte) '0'));
            Assert.True(XmlDictionary.IsSecondByteOfToken((byte) '5'));
            Assert.True(XmlDictionary.IsSecondByteOfToken((byte) '9'));
        }

        [Test]
        public void white_space_chars_are_token()
        {
            Assert.True(XmlDictionary.IsSecondByteOfToken((byte) ' '));
            Assert.True(XmlDictionary.IsSecondByteOfToken((byte) '\t'));
            Assert.True(XmlDictionary.IsSecondByteOfToken((byte) '\n'));
            Assert.True(XmlDictionary.IsSecondByteOfToken((byte) '\r'));
        }

        [Test]
        public void punctuations_chars_are_token()
        {
            Assert.True(XmlDictionary.IsSecondByteOfToken((byte) '.'));
            Assert.True(XmlDictionary.IsSecondByteOfToken((byte) ';'));
            Assert.True(XmlDictionary.IsSecondByteOfToken((byte) ','));
            Assert.True(XmlDictionary.IsSecondByteOfToken((byte) '/'));
        }

        [Test]
        public void token_codes_are_token_types()
        {
            Assert.False(XmlDictionary.IsSecondByteOfToken((byte) XmlTokenType.TagEnd));
            Assert.False(XmlDictionary.IsSecondByteOfToken((byte) XmlTokenType.TagEndEmptyContent));
            Assert.False(XmlDictionary.IsSecondByteOfToken((byte) XmlTokenType.TagStart));
            Assert.False(XmlDictionary.IsSecondByteOfToken((byte) XmlTokenType.NestedContent));
            Assert.False(XmlDictionary.IsSecondByteOfToken((byte) XmlTokenType.AttributeName));
            Assert.False(XmlDictionary.IsSecondByteOfToken((byte) XmlTokenType.AttributeValueInteger));
            Assert.False(XmlDictionary.IsSecondByteOfToken((byte) XmlTokenType.AttributeValueDouble));
            Assert.False(XmlDictionary.IsSecondByteOfToken((byte) XmlTokenType.AttributeValueFraction));
            Assert.False(XmlDictionary.IsSecondByteOfToken((byte) XmlTokenType.AttributeValueString));
        }

        [Test]
        public void bytes_with_bit_7_set_are_not_token()
        {
            for (var i = 128; i < 256; ++i)
            {
                Assert.False(XmlDictionary.IsSecondByteOfToken((byte) i));
            }
        }

        [Test]
        public void bytes_with_bit_7_unset_are_token()
        {
            for (var i = Enum.GetNames(typeof(XmlTokenType)).Length; i < 128; ++i)
            {
                Assert.True(XmlDictionary.IsSecondByteOfToken((byte) i));
            }
        }
    }

    [TestFixture]
    public class test_byte_is_a_token_type_marker
    {
        [Test]
        public void token_codes_are_not_token()
        {
            Assert.True(XmlDictionary.IsTokenType((byte) XmlTokenType.TagEnd));
            Assert.True(XmlDictionary.IsTokenType((byte) XmlTokenType.TagEndEmptyContent));
            Assert.True(XmlDictionary.IsTokenType((byte) XmlTokenType.TagStart));
            Assert.True(XmlDictionary.IsTokenType((byte) XmlTokenType.NestedContent));
            Assert.True(XmlDictionary.IsTokenType((byte) XmlTokenType.AttributeName));
            Assert.True(XmlDictionary.IsTokenType((byte) XmlTokenType.AttributeValueInteger));
            Assert.True(XmlDictionary.IsTokenType((byte) XmlTokenType.AttributeValueDouble));
            Assert.True(XmlDictionary.IsTokenType((byte) XmlTokenType.AttributeValueFraction));
            Assert.True(XmlDictionary.IsTokenType((byte) XmlTokenType.AttributeValueString));
        }

        [Test]
        public void other_ascii_bytes_with_bit_7_unset_are_not_token()
        {
            for (var i = Enum.GetNames(typeof(XmlTokenType)).Length; i < 128; ++i)
            {
                Assert.False(XmlDictionary.IsTokenType((byte) i));
            }
        }
    }

    [TestFixture]
    public class test_code_generation_from_input_bytes
    {
        [Test]
        public void one_byte_token_codes()
        {
            // all are between 0x80 and 0xff
            Assert.AreEqual(0, XmlDictionary.OneByteCode(0x80));
            Assert.AreEqual(1, XmlDictionary.OneByteCode(0x81));
            Assert.AreEqual(2, XmlDictionary.OneByteCode(0x82));
            Assert.AreEqual(22, XmlDictionary.OneByteCode(0x96));
            Assert.AreEqual(72, XmlDictionary.OneByteCode(0xc8));
            Assert.AreEqual(126, XmlDictionary.OneByteCode(0xfe));
            Assert.AreEqual(127, XmlDictionary.OneByteCode(0xff));
        }

        [Test]
        public void two_byte_token_codes()
        {
            // first bytes between 0x80 and 0xff
            // seconds bytes between 0x09 and 0x7f
            Assert.AreEqual(0, XmlDictionary.TwoByteCode(0x80, 0x09));
            Assert.AreEqual(1, XmlDictionary.TwoByteCode(0x81, 0x09));
            Assert.AreEqual(2, XmlDictionary.TwoByteCode(0x82, 0x09));
            Assert.AreEqual(128, XmlDictionary.TwoByteCode(0x80, 0x0a));
            Assert.AreEqual(129, XmlDictionary.TwoByteCode(0x81, 0x0a));
            Assert.AreEqual(130, XmlDictionary.TwoByteCode(0x82, 0x0a));
        }

        [Test]
        public void all_two_byte_token_codes_increase_in_sequence()
        {
            var expected = 0;
            // first bytes between 0x80 and 0xff
            // seconds bytes between 0x09 and 0x7f
            for (var second = 0x09; second <= 0x7f; ++second)
            {
                for (var first = 0x80; first <= 0xff; ++first)
                {
                    Assert.AreEqual(expected++, XmlDictionary.TwoByteCode((byte) first, (byte) second));
                }
            }
        }
    }

    [TestFixture]
    public class test_dictionary_token_inserts_and_lookups
    {
        [Test]
        public void each_insert_increases_the_code()
        {
            var dictionary = new XmlDictionary();
            const int maxTokens = 15232;
            for (var i = 0; i < maxTokens; ++i)
            {
                var tokenCode = dictionary.InsertToken(XmlToken.IntegerValue(i));
                Assert.AreEqual(i, tokenCode.Code);
                Assert.AreEqual(XmlTokenType.AttributeValueInteger, tokenCode.Token.TokenType());
                Assert.AreEqual(i.ToString(), tokenCode.Token.GetText());
            }
        }

        [Test]
        public void once_inserted_a_token_can_be_fetched()
        {
            var dictionary = new XmlDictionary();
            for (var i = 0; i < 5; ++i)
            {
                var tokenCode = dictionary.InsertToken(XmlToken.IntegerValue(i));
                Assert.AreEqual(tokenCode.Token, dictionary.LookupToken(tokenCode.Code));
            }
        }

        [Test]
        public void each_time_a_token_is_used_its_useage_count_increases()
        {
            var dictionary = new XmlDictionary();
            var tokenCode = dictionary.InsertToken(XmlToken.StringValue("sweeno"));
            Assert.AreEqual(0, tokenCode.Count);
            for (var i = 1; i < 10; ++i)
            {
                var token = dictionary.LookupToken(tokenCode.Code);
                Assert.AreEqual(i, tokenCode.Count);
                Assert.AreEqual("sweeno", token.GetText());
            }
        }

        [Test]
        public void fetching_a_token_before_insertion_throws_and_exception()
        {
            var dictionary = new XmlDictionary();
            for (var i = 0; i < 5; ++i)
            {
                dictionary.InsertToken(XmlToken.IntegerValue(i));
            }
            try
            {
                dictionary.LookupToken(6);
                Assert.Fail("should throw exception");
            }
            catch (PuffinSyntaxException)
            {
            }
        }

        [Test]
        public void most_frequently_used_tokens_bubble_up_to_one_byte_tokens()
        {
            var dictionary = new XmlDictionary();
            // overfill the directory in reverse
            for (var value = 20000; value-- > 0;)
            {
                var tokenCode = dictionary.InsertToken(XmlToken.IntegerValue(value));
                // access the lower value tokens the most to make them bubble up to the top.
                var usage = value < 128 ? 128 - value : 0;
                for (var i = 0; i < usage; ++i)
                {
                    dictionary.LookupToken(tokenCode.Code);
                }
            }

            var oneByteTokens = new HashSet<string>();
            for (var code = 0; code < 128; ++code)
            {
                oneByteTokens.Add(dictionary.LookupToken(code).GetText());
            }
            for (var code = 0; code < 128; ++code)
            {
                var text = code.ToString();
                Assert.True(oneByteTokens.Contains(text), "one byte tokens contains: "+text);
            }
        }
    }
}