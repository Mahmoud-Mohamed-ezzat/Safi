using System.Security.Cryptography;
using System.Text;

namespace Safi.Helpers
{
    public class AesChatHelper
    {
        private static readonly string Key = "1234567890123456"; // 16 bytes (AES-128)
        private static readonly string IV = "6543210987654321"; // 16 bytes

        public static string Encrypt(string plainText)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(Key);
                aes.IV = Encoding.UTF8.GetBytes(IV);

                using var ms = new MemoryStream();
                using var crypto = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
                using (var sw = new StreamWriter(crypto))
                {
                    sw.Write(plainText);
                }

                return Convert.ToBase64String(ms.ToArray());
            }
        }

        public static string Decrypt(string cipherText)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(Key);
                aes.IV = Encoding.UTF8.GetBytes(IV);

                using var ms = new MemoryStream(Convert.FromBase64String(cipherText));
                using var crypto = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
                using var sr = new StreamReader(crypto);

                return sr.ReadToEnd();
            }
        }
    }
}
