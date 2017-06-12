using System;
using System.Collections.Generic;
using BidFX.Public.API.Price.Plugin.Puffin;
using NUnit.Framework;

namespace BidFX.Public.API.test.Plugin.Puffin
{
    public class TokenDictionaryTest
    {
    }

    [TestFixture]
    public class test_byte_for_being_1st_byte_of_a_token
    {
        [Test]
        public void letters_are_not_tokens()
        {
            Assert.False(TokenDictionary.IsFirstByteOfToken((byte) 'a'));
            Assert.False(TokenDictionary.IsFirstByteOfToken((byte) 's'));
            Assert.False(TokenDictionary.IsFirstByteOfToken((byte) 'c'));
            Assert.False(TokenDictionary.IsFirstByteOfToken((byte) 'i'));
            Assert.False(TokenDictionary.IsFirstByteOfToken((byte) 'z'));
            Assert.False(TokenDictionary.IsFirstByteOfToken((byte) 'A'));
            Assert.False(TokenDictionary.IsFirstByteOfToken((byte) 'Z'));
        }

        [Test]
        public void digits_are_not_tokens()
        {
            Assert.False(TokenDictionary.IsFirstByteOfToken((byte) '0'));
            Assert.False(TokenDictionary.IsFirstByteOfToken((byte) '5'));
            Assert.False(TokenDictionary.IsFirstByteOfToken((byte) '9'));
        }

        [Test]
        public void white_space_chars_are_not_tokens()
        {
            Assert.False(TokenDictionary.IsFirstByteOfToken((byte) ' '));
            Assert.False(TokenDictionary.IsFirstByteOfToken((byte) '\t'));
            Assert.False(TokenDictionary.IsFirstByteOfToken((byte) '\n'));
            Assert.False(TokenDictionary.IsFirstByteOfToken((byte) '\r'));
        }

        [Test]
        public void punctuations_chars_are_not_tokens()
        {
            Assert.False(TokenDictionary.IsFirstByteOfToken((byte) '.'));
            Assert.False(TokenDictionary.IsFirstByteOfToken((byte) ';'));
            Assert.False(TokenDictionary.IsFirstByteOfToken((byte) ','));
            Assert.False(TokenDictionary.IsFirstByteOfToken((byte) '/'));
        }

        [Test]
        public void bytes_with_bit_7_set_are_tokens()
        {
            for (var i = 128; i < 256; ++i)
            {
                Assert.True(TokenDictionary.IsFirstByteOfToken((byte) i));
            }
        }

        [Test]
        public void bytes_with_bit_7_unset_are_not_tokens()
        {
            for (var i = 0; i < 128; ++i)
            {
                Assert.False(TokenDictionary.IsFirstByteOfToken((byte) i));
            }
        }
    }

    [TestFixture]
    public class test_byte_for_being_plain_text
    {
        [Test]
        public void letters_are_plain_text()
        {
            Assert.True(TokenDictionary.IsPlainText((byte) 'a'));
            Assert.True(TokenDictionary.IsPlainText((byte) 's'));
            Assert.True(TokenDictionary.IsPlainText((byte) 'c'));
            Assert.True(TokenDictionary.IsPlainText((byte) 'i'));
            Assert.True(TokenDictionary.IsPlainText((byte) 'z'));
            Assert.True(TokenDictionary.IsPlainText((byte) 'A'));
            Assert.True(TokenDictionary.IsPlainText((byte) 'Z'));
        }

        [Test]
        public void digits_are_plain_text()
        {
            Assert.True(TokenDictionary.IsPlainText((byte) '0'));
            Assert.True(TokenDictionary.IsPlainText((byte) '5'));
            Assert.True(TokenDictionary.IsPlainText((byte) '9'));
        }

        [Test]
        public void white_space_chars_are_plain_text()
        {
            Assert.True(TokenDictionary.IsPlainText((byte) ' '));
            Assert.True(TokenDictionary.IsPlainText((byte) '\t'));
            Assert.True(TokenDictionary.IsPlainText((byte) '\n'));
            Assert.True(TokenDictionary.IsPlainText((byte) '\r'));
        }

        [Test]
        public void punctuations_chars_are_plain_text()
        {
            Assert.True(TokenDictionary.IsPlainText((byte) '.'));
            Assert.True(TokenDictionary.IsPlainText((byte) ';'));
            Assert.True(TokenDictionary.IsPlainText((byte) ','));
            Assert.True(TokenDictionary.IsPlainText((byte) '/'));
        }

        [Test]
        public void token_codes_are_not_plain_text()
        {
            Assert.False(TokenDictionary.IsPlainText((byte) TokenType.TagEnd));
            Assert.False(TokenDictionary.IsPlainText((byte) TokenType.TagEndEmptyContent));
            Assert.False(TokenDictionary.IsPlainText((byte) TokenType.TagStart));
            Assert.False(TokenDictionary.IsPlainText((byte) TokenType.NestedContent));
            Assert.False(TokenDictionary.IsPlainText((byte) TokenType.AttributeName));
            Assert.False(TokenDictionary.IsPlainText((byte) TokenType.IntegerValue));
            Assert.False(TokenDictionary.IsPlainText((byte) TokenType.DecimalValue));
            Assert.False(TokenDictionary.IsPlainText((byte) TokenType.FractionValue));
            Assert.False(TokenDictionary.IsPlainText((byte) TokenType.StringValue));
        }

        [Test]
        public void bytes_with_bit_7_set_are_not_plain_text()
        {
            Assert.False(TokenDictionary.IsPlainText(128));
            Assert.False(TokenDictionary.IsPlainText(129));
            for (var i = 128; i < 256; ++i)
            {
                Assert.False(TokenDictionary.IsPlainText((byte) i));
            }
        }

        [Test]
        public void bytes_with_bit_7_unset_are_plain_text()
        {
            for (var i = Enum.GetNames(typeof(TokenType)).Length; i < 128; ++i)
            {
                Assert.True(TokenDictionary.IsPlainText((byte) i));
            }
        }
    }

    [TestFixture]
    public class test_byte_for_being_2nd_byte_of_a_token
    {
        [Test]
        public void letters_are_token()
        {
            Assert.True(TokenDictionary.IsSecondByteOfToken((byte) 'a'));
            Assert.True(TokenDictionary.IsSecondByteOfToken((byte) 's'));
            Assert.True(TokenDictionary.IsSecondByteOfToken((byte) 'c'));
            Assert.True(TokenDictionary.IsSecondByteOfToken((byte) 'i'));
            Assert.True(TokenDictionary.IsSecondByteOfToken((byte) 'z'));
            Assert.True(TokenDictionary.IsSecondByteOfToken((byte) 'A'));
            Assert.True(TokenDictionary.IsSecondByteOfToken((byte) 'Z'));
        }

        [Test]
        public void digits_are_token()
        {
            Assert.True(TokenDictionary.IsSecondByteOfToken((byte) '0'));
            Assert.True(TokenDictionary.IsSecondByteOfToken((byte) '5'));
            Assert.True(TokenDictionary.IsSecondByteOfToken((byte) '9'));
        }

        [Test]
        public void white_space_chars_are_token()
        {
            Assert.True(TokenDictionary.IsSecondByteOfToken((byte) ' '));
            Assert.True(TokenDictionary.IsSecondByteOfToken((byte) '\t'));
            Assert.True(TokenDictionary.IsSecondByteOfToken((byte) '\n'));
            Assert.True(TokenDictionary.IsSecondByteOfToken((byte) '\r'));
        }

        [Test]
        public void punctuations_chars_are_token()
        {
            Assert.True(TokenDictionary.IsSecondByteOfToken((byte) '.'));
            Assert.True(TokenDictionary.IsSecondByteOfToken((byte) ';'));
            Assert.True(TokenDictionary.IsSecondByteOfToken((byte) ','));
            Assert.True(TokenDictionary.IsSecondByteOfToken((byte) '/'));
        }

        [Test]
        public void token_codes_are_token_types()
        {
            Assert.False(TokenDictionary.IsSecondByteOfToken((byte) TokenType.TagEnd));
            Assert.False(TokenDictionary.IsSecondByteOfToken((byte) TokenType.TagEndEmptyContent));
            Assert.False(TokenDictionary.IsSecondByteOfToken((byte) TokenType.TagStart));
            Assert.False(TokenDictionary.IsSecondByteOfToken((byte) TokenType.NestedContent));
            Assert.False(TokenDictionary.IsSecondByteOfToken((byte) TokenType.AttributeName));
            Assert.False(TokenDictionary.IsSecondByteOfToken((byte) TokenType.IntegerValue));
            Assert.False(TokenDictionary.IsSecondByteOfToken((byte) TokenType.DecimalValue));
            Assert.False(TokenDictionary.IsSecondByteOfToken((byte) TokenType.FractionValue));
            Assert.False(TokenDictionary.IsSecondByteOfToken((byte) TokenType.StringValue));
        }

        [Test]
        public void bytes_with_bit_7_set_are_not_token()
        {
            for (var i = 128; i < 256; ++i)
            {
                Assert.False(TokenDictionary.IsSecondByteOfToken((byte) i));
            }
        }

        [Test]
        public void bytes_with_bit_7_unset_are_token()
        {
            for (var i = Enum.GetNames(typeof(TokenType)).Length; i < 128; ++i)
            {
                Assert.True(TokenDictionary.IsSecondByteOfToken((byte) i));
            }
        }
    }

    [TestFixture]
    public class test_byte_is_a_token_type_marker
    {
        [Test]
        public void token_codes_are_not_token()
        {
            Assert.True(TokenDictionary.IsTokenType((byte) TokenType.TagEnd));
            Assert.True(TokenDictionary.IsTokenType((byte) TokenType.TagEndEmptyContent));
            Assert.True(TokenDictionary.IsTokenType((byte) TokenType.TagStart));
            Assert.True(TokenDictionary.IsTokenType((byte) TokenType.NestedContent));
            Assert.True(TokenDictionary.IsTokenType((byte) TokenType.AttributeName));
            Assert.True(TokenDictionary.IsTokenType((byte) TokenType.IntegerValue));
            Assert.True(TokenDictionary.IsTokenType((byte) TokenType.DecimalValue));
            Assert.True(TokenDictionary.IsTokenType((byte) TokenType.FractionValue));
            Assert.True(TokenDictionary.IsTokenType((byte) TokenType.StringValue));
        }

        [Test]
        public void other_ascii_bytes_with_bit_7_unset_are_not_token()
        {
            for (var i = Enum.GetNames(typeof(TokenType)).Length; i < 128; ++i)
            {
                Assert.False(TokenDictionary.IsTokenType((byte) i));
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
            Assert.AreEqual(0, TokenDictionary.OneByteCode(0x80));
            Assert.AreEqual(1, TokenDictionary.OneByteCode(0x81));
            Assert.AreEqual(2, TokenDictionary.OneByteCode(0x82));
            Assert.AreEqual(22, TokenDictionary.OneByteCode(0x96));
            Assert.AreEqual(72, TokenDictionary.OneByteCode(0xc8));
            Assert.AreEqual(126, TokenDictionary.OneByteCode(0xfe));
            Assert.AreEqual(127, TokenDictionary.OneByteCode(0xff));
        }

        [Test]
        public void two_byte_token_codes()
        {
            // first bytes between 0x80 and 0xff
            // seconds bytes between 0x09 and 0x7f
            Assert.AreEqual(0, TokenDictionary.TwoByteCode(0x80, 0x09));
            Assert.AreEqual(1, TokenDictionary.TwoByteCode(0x81, 0x09));
            Assert.AreEqual(2, TokenDictionary.TwoByteCode(0x82, 0x09));
            Assert.AreEqual(128, TokenDictionary.TwoByteCode(0x80, 0x0a));
            Assert.AreEqual(129, TokenDictionary.TwoByteCode(0x81, 0x0a));
            Assert.AreEqual(130, TokenDictionary.TwoByteCode(0x82, 0x0a));
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
                    Assert.AreEqual(expected++, TokenDictionary.TwoByteCode((byte) first, (byte) second));
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
            var dictionary = new TokenDictionary();
            const int maxTokens = 15232;
            for (var i = 0; i < maxTokens; ++i)
            {
                var tokenCode = dictionary.InsertToken(PuffinToken.IntegerValue(i));
                Assert.AreEqual(i, tokenCode.Code);
                Assert.AreEqual(TokenType.IntegerValue, tokenCode.Token.TokenType);
                Assert.AreEqual(i.ToString(), tokenCode.Token.Text);
            }
        }

        [Test]
        public void once_inserted_a_token_can_be_fetched()
        {
            var dictionary = new TokenDictionary();
            for (var i = 0; i < 5; ++i)
            {
                var tokenCode = dictionary.InsertToken(PuffinToken.IntegerValue(i));
                Assert.AreEqual(tokenCode.Token, dictionary.LookupToken(tokenCode.Code));
            }
        }

        [Test]
        public void each_time_a_token_is_used_its_useage_count_increases()
        {
            var dictionary = new TokenDictionary();
            var tokenCode = dictionary.InsertToken(PuffinToken.StringValue("sweeno"));
            Assert.AreEqual(0, tokenCode.Count);
            for (var i = 1; i < 10; ++i)
            {
                var token = dictionary.LookupToken(tokenCode.Code);
                Assert.AreEqual(i, tokenCode.Count);
                Assert.AreEqual("sweeno", token.Text);
            }
        }

        [Test]
        public void fetching_a_token_before_insertion_throws_and_exception()
        {
            var dictionary = new TokenDictionary();
            for (var i = 0; i < 5; ++i)
            {
                dictionary.InsertToken(PuffinToken.IntegerValue(i));
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
            var dictionary = new TokenDictionary();
            // overfill the directory in reverse
            for (var value = 20000; value-- > 0;)
            {
                var tokenCode = dictionary.InsertToken(PuffinToken.IntegerValue(value));
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
                oneByteTokens.Add(dictionary.LookupToken(code).Text);
            }
            for (var code = 0; code < 128; ++code)
            {
                var text = code.ToString();
                Assert.True(oneByteTokens.Contains(text), "one byte tokens contains: " + text);
            }
        }
    }
}