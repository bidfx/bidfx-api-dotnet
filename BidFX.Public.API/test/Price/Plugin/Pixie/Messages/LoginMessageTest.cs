using System;
using System.IO;
using NUnit.Framework;

namespace BidFX.Public.API.Price.Plugin.Pixie.Messages
{
    public class LoginMessageTest
    {
        private const string Application = "PublicAPI";
        private const string ApplicationVersion = "1.0.2";
        private const string Username = "pmacdona";
        private const string Password = "thisissupersecret";
        private const string Alias = "paul";

        private readonly LoginMessage _message =
            new LoginMessage(Username, Password, Alias, Application, ApplicationVersion);

        private readonly byte[] EncodedV1 =
        {
            76, 9, 112, 109, 97, 99, 100, 111, 110, 97, 18, 116, 104, 105, 115, 105, 115, 115, 117, 112, 101, 114,
            115, 101, 99, 114, 101, 116, 5, 112, 97, 117, 108
        };

        private readonly byte[] EncodedV2 =
        {
            76, 9, 112, 109, 97, 99, 100, 111, 110, 97, 18, 116, 104, 105, 115, 105, 115, 115, 117, 112, 101, 114, 115,
            101, 99, 114, 101, 116, 5, 112, 97, 117, 108, 10, 80, 117, 98, 108, 105, 99, 65, 80, 73, 6, 49, 46, 48, 46,
            50
        };

        [Test]
        public void ThrowsExceptionIfConstructedWithNullUsername()
        {
            try
            {
                new LoginMessage(null, Password, Alias, Application, ApplicationVersion);
                Assert.Fail();
            }
            catch (ArgumentException e)
            {
            }
        }

        [Test]
        public void ThrowsExceptionIfConstructedWithNullPassword()
        {
            try
            {
                new LoginMessage(Username, null, Alias, Application, ApplicationVersion);
                Assert.Fail();
            }
            catch (ArgumentException e)
            {
            }
        }

        [Test]
        public void ThrowsExceptionIfConstructedWithNullAlias()
        {
            try
            {
                new LoginMessage(Username, Password, null, Application, ApplicationVersion);
                Assert.Fail();
            }
            catch (ArgumentException e)
            {
            }
        }

        [Test]
        public void ThrowsExceptionIfConstructedWithNullApplication()
        {
            try
            {
                new LoginMessage(Username, Password, Alias, null, ApplicationVersion);
                Assert.Fail();
            }
            catch (ArgumentException e)
            {
            }
        }

        [Test]
        public void ThrowsExceptionIfConstructedWithNullApplicationVersion()
        {
            try
            {
                new LoginMessage(Username, Password, Alias, Application, null);
                Assert.Fail();
            }
            catch (ArgumentException e)
            {
            }
        }

        [Test]
        public void EncodeVersion1()
        {
            MemoryStream memoryStream = _message.Encode(1);
            Assert.AreEqual(EncodedV1, memoryStream.ToArray());
        }

        [Test]
        public void EncodeVersion2()
        {
            MemoryStream memoryStream = _message.Encode(2);
            Assert.AreEqual(EncodedV2, memoryStream.ToArray());
        }

        [Test]
        public void EncodeVersion3()
        {
            MemoryStream memoryStream = _message.Encode(3);
            Assert.AreEqual(EncodedV2, memoryStream.ToArray());
        }
    }
}