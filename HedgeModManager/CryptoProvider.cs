using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HedgeModManager
{
    public static class CryptoProvider
    {
        public const string PublicModulus =
            "1Zf2HKNkolr1CDKl4Y6FWkJlMu6SEHKc1lVTlHNtot1SJpwqCW9iKk3t80mOGhsv4Qc2ZEX2I0yKmRLmcnCSAXoBneyYTlE2yTWj1nI4iJv1x8PTWPiMZDnbljXj6rAIizbGoIgDx/xmmfIOCiIaS5c7A2CGnIHu+yLDlzG5gBU=";

        public const string PublicExponent = "AQAB";

        public static RSACryptoServiceProvider CryptoService = new RSACryptoServiceProvider(1024);

        static CryptoProvider()
        {
            CryptoService.ImportParameters(new RSAParameters()
            {
                Modulus = Convert.FromBase64String(PublicModulus),
                Exponent = Convert.FromBase64String(PublicExponent)
            });
        }

        public static void ImportParameters(RSAParameters parameters)
            => CryptoService.ImportParameters(parameters);

        public static byte[] Encrypt(byte[] data)
        {
            using (var source = new MemoryStream(data))
            using (var destination = new MemoryStream())
            {
                Encrypt(source, destination);
                return destination.ToArray();
            }
        }

        public static void Encrypt(Stream source, Stream destination)
        {
            using (var writer = new BinaryWriter(destination))
            using (var aes = new AesCryptoServiceProvider())
            {
                aes.GenerateKey();
                aes.GenerateIV();

                var encryptedKey = CryptoService.Encrypt(aes.Key, false);
                writer.Write(encryptedKey.Length);
                writer.Write(encryptedKey);

                var encryptedIv = CryptoService.Encrypt(aes.IV, false);
                writer.Write(encryptedIv.Length);
                writer.Write(encryptedIv);

                using (var enc = aes.CreateEncryptor())
                using (var cryptoStream = new CryptoStream(destination, enc, CryptoStreamMode.Write))
                {
                    source.CopyTo(cryptoStream);
                }
            }
        }

        public static byte[] Decrypt(byte[] data)
        {
            using (var source = new MemoryStream(data))
            using (var destination = new MemoryStream())
            {
                Decrypt(source, destination);
                return destination.ToArray();
            }
        }

        public static void Decrypt(Stream source, Stream destination)
        {
            using (var reader = new BinaryReader(source))
            using (var aes = new AesCryptoServiceProvider())
            {
                var encryptedKey = reader.ReadBytes(reader.ReadInt32());
                var encryptedIv = reader.ReadBytes(reader.ReadInt32());

                aes.Key = CryptoService.Decrypt(encryptedKey, false);
                aes.IV = CryptoService.Decrypt(encryptedIv, false);

                using (var dec = aes.CreateDecryptor())
                using (var cryptoStream = new CryptoStream(source, dec, CryptoStreamMode.Read))
                {
                    cryptoStream.CopyTo(destination);
                }
            }
        }
    }
}
