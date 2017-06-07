using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace BidFX.Public.NAPI.Price.Plugin.Puffin
{
    internal class LoginEncryption
    {
        /// <summary>
        /// Encrypts a message with the supplied public key.
        /// </summary>
        /// <param name="publicKey">A base 64 encoded X509 public key.</param>
        /// <param name="message">The message to encrypt.</param>
        /// <returns>The encrypted message encoded with base 64.</returns>
        public static string EncryptWithPublicKey(string publicKey, string message)
        {
            var keyBytes = Convert.FromBase64String(publicKey);
            var rsa = DecodeX509PublicKey(keyBytes);
            var plainBytes = Encoding.UTF8.GetBytes(message);
            var encryptedBytes = rsa.Encrypt(plainBytes, false);
            return Convert.ToBase64String(encryptedBytes);
        }

        private static RSACryptoServiceProvider DecodeX509PublicKey(byte[] x509Key)
        {
            byte[] seqOid = {0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01};
            var ms = new MemoryStream(x509Key);
            var reader = new BinaryReader(ms);
            if (reader.ReadByte() == 0x30)
            {
                ReadASNLength(reader); //skip the size
            }
            else
            {
                return null;
            }
            var identifierSize = 0; //total length of Object Identifier section
            if (reader.ReadByte() == 0x30)
            {
                identifierSize = ReadASNLength(reader);
            }
            else
            {
                return null;
            }
            if (reader.ReadByte() == 0x06) //is the next element an object identifier?
            {
                var oidLength = ReadASNLength(reader);
                var oidBytes = new byte[oidLength];
                reader.Read(oidBytes, 0, oidBytes.Length);
                if (oidBytes.SequenceEqual(seqOid) == false) //is the object identifier rsaEncryption PKCS#1?
                {
                    return null;
                }
                var remainingBytes = identifierSize - 2 - oidBytes.Length;
                reader.ReadBytes(remainingBytes);
            }
            if (reader.ReadByte() != 0x03) return null;
            ReadASNLength(reader); //skip the size
            reader.ReadByte(); //skip unused bits indicator
            if (reader.ReadByte() != 0x30) return null;
            ReadASNLength(reader); //skip the size
            if (reader.ReadByte() != 0x02) return null;
            var modulusSize = ReadASNLength(reader);
            var modulus = new byte[modulusSize];
            reader.Read(modulus, 0, modulus.Length);
            if (modulus[0] == 0x00) //strip off the first byte if it's 0
            {
                var tempModulus = new byte[modulus.Length - 1];
                Array.Copy(modulus, 1, tempModulus, 0, modulus.Length - 1);
                modulus = tempModulus;
            }
            if (reader.ReadByte() != 0x02) return null;
            var exponentSize = ReadASNLength(reader);
            var exponent = new byte[exponentSize];
            reader.Read(exponent, 0, exponent.Length);
            var rsa = new RSACryptoServiceProvider();
            var rsaKeyInfo = new RSAParameters
            {
                Modulus = modulus,
                Exponent = exponent
            };
            rsa.ImportParameters(rsaKeyInfo);
            return rsa;
        }

        private static int ReadASNLength(BinaryReader reader)
        {
            int length = reader.ReadByte();
            if ((length & 0x00000080) != 0x00000080) return length;
            var count = length & 0x0000000f;
            var lengthBytes = new byte[4];
            reader.Read(lengthBytes, 4 - count, count);
            Array.Reverse(lengthBytes); //
            length = BitConverter.ToInt32(lengthBytes, 0);
            return length;
        }
    }
}