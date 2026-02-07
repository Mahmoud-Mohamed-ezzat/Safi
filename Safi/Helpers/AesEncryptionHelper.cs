using System.Security.Cryptography;
using System.Text;

namespace Safi.Helpers
{
    public class AesEncryptionHelper
    {
        private readonly byte[] Key;
        private readonly byte[] IV;

        public AesEncryptionHelper(string key)
        {
            // تحويل المفتاح إلى 32 بايت (AES-256)
            Key = Encoding.UTF8.GetBytes(key.PadRight(32).Substring(0, 32));
            IV = new byte[16];
        }

        public string Encrypt(string plainText)
        {
            if (plainText == null) return null;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = IV;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using MemoryStream ms = new();
                using (CryptoStream cs = new(ms, encryptor, CryptoStreamMode.Write))
                using (StreamWriter sw = new(cs))
                {
                    sw.Write(plainText);
                }
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        public string Decrypt(string cipherText)
        {
            if (cipherText == null) return null;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = IV;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using MemoryStream ms = new(Convert.FromBase64String(cipherText));
                using CryptoStream cs = new(ms, decryptor, CryptoStreamMode.Read);
                using StreamReader sr = new(cs);
                return sr.ReadToEnd();
            }
        }
    }
}
