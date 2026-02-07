namespace Safi.Helpers
{
    public class AesHelper
    {
        private static AesEncryptionHelper _helper;

        public static void Init(string secretKey)
        {
            _helper = new AesEncryptionHelper(secretKey);
        }

        public static string Encrypt(string plainText) => _helper.Encrypt(plainText);
        public static string Decrypt(string cipherText) => _helper.Decrypt(cipherText);
    }
}
