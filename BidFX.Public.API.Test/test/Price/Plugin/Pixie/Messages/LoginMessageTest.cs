using System;
using System.IO;
using NUnit.Framework;

namespace BidFX.Public.API.Price.Plugin.Pixie.Messages
{
    public class LoginMessageTest
    {
        private const string Application = "PublicAPI";
        private const string ApplicationVersion = "1.0.2";
        private const string Username = "username";
        private const string Password = "thisissupersecret";
        private const string Alias = "paul";
        private const string Serial = "abc123456789";
        private const string Api = "BidFX Dotnet";
        private const string ApiVersion = "1.0.4";
        private const string Product = "BidFXDotnet";
        private const string ProductSerial = "aaa1234567890";

        private readonly LoginMessage _message =
            new LoginMessage(Username, Password, Alias, Application, ApplicationVersion, Api, ApiVersion, Product, ProductSerial);

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

        private readonly byte[] EncodedV4 =
        {
            76, 9, 112, 109, 97, 99, 100, 111, 110, 97, 18, 116, 104, 105, 115, 105, 115, 115, 117, 112, 101, 114, 115,
            101, 99, 114, 101, 116, 5, 112, 97, 117, 108, 10, 80, 117, 98, 108, 105, 99, 65, 80, 73, 6, 49, 46, 48, 46,
            50, 13, 66, 105, 100, 70, 88, 32, 68, 111, 116, 110, 101, 116, 6, 49, 46, 48, 46, 52, 12, 66, 105, 100, 70, 88,
            68, 111, 116, 110, 101, 116, 14, 97, 97, 97, 49, 50, 51, 52, 53, 54, 55, 56, 57, 48  
        };
//        private static final String ENCODED_MESSAGE_V4 = ENCODED_MESSAGE_V2 + "\026bidfx-public-api-java\0061.1.1\016BidFXPriceAPI\031aad33247deffe2aa2832001f";

        [Test]
        public void ThrowsExceptionIfConstructedWithNullUsername()
        {
            try
            {
                new LoginMessage(null, Password, Alias, Application, ApplicationVersion, Api, ApiVersion, Product, ProductSerial);
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
                new LoginMessage(Username, null, Alias, Application, ApplicationVersion, Api, ApiVersion, Product, ProductSerial);
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
                new LoginMessage(Username, Password, null, Application, ApplicationVersion, Api, ApiVersion, Product, ProductSerial);
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
                new LoginMessage(Username, Password, Alias, null, ApplicationVersion, Api, ApiVersion, Product, ProductSerial);
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
                new LoginMessage(Username, Password, Alias, Application, null, Api, ApiVersion, Product, ProductSerial);
                Assert.Fail();
            }
            catch (ArgumentException e)
            {
            }
        }

        [Test]
        public void ThrowsExceptionIfConstructedWithNullAPI()
        {
            try
            {
                new LoginMessage(Username, Password, Alias, Application, ApplicationVersion, null, ApiVersion, Product, ProductSerial);
                Assert.Fail();
            }
            catch (ArgumentException e)
            {
            }
        }

        [Test]
        public void ThrowsExceptionIfConstructedWithNullAPIVersion()
        {
            try
            {
                new LoginMessage(Username, Password, Alias, Application, ApplicationVersion, Api, null, Product, ProductSerial);
                Assert.Fail();
            }
            catch (ArgumentException e)
            {
            }
        }

        [Test]
        public void ThrowsExceptionIfConstructedWithNullProduct()
        {
            try
            {
                new LoginMessage(Username, Password, Alias, Application, ApplicationVersion, Api, ApiVersion, null, ProductSerial);
                Assert.Fail();
            }
            catch (ArgumentException e)
            {
            }
        }

        [Test]
        public void ThrowsExceptionIfConstructedWithNullProductSerial()
        {
            try
            {
                new LoginMessage(Username, Password, Alias, Application, ApplicationVersion, Api, ApiVersion, Product, null);
                Assert.Fail();
            }
            catch (ArgumentException e)
            {
            }
        }

        [Test]
        public void ThrowsExceptionIfConstructedWithEmptyProductSerial()
        {
            try
            {
                new LoginMessage(Username, Password, Alias, Application, ApplicationVersion, Api, ApiVersion, Product, "");
                Assert.Fail();
            }
            catch (ArgumentException e)
            {
            }
        }

        [Test]
        public void ThrowsExceptionIfConstructedWithBlankProductSerial()
        {
            try
            {
                new LoginMessage(Username, Password, Alias, Application, ApplicationVersion, Api, ApiVersion, Product, "     ");
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

        [Test]
        public void EncodeVersion4()
        {
            MemoryStream memoryStream = _message.Encode(4);
            Assert.AreEqual(EncodedV4, memoryStream.ToArray());
        }
    }
}