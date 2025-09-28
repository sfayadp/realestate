using System.Security.Cryptography;
using System.Text;

namespace RealEstate.Shared.Utils
{
    public static class SecureCryptoHelper
    {
        private static readonly byte[] Salt = new byte[8] { 1, 2, 3, 4, 5, 6, 7, 8 };

        public static string Encrypt(string plainText, string passphrase)
        {
            if (string.IsNullOrEmpty(plainText))
                return "";

            var passwordBytes = DeriveKeyFromPassphrase(passphrase);

            using var aes = Aes.Create();
            var keyIv = new Rfc2898DeriveBytes(passwordBytes, Salt, 100_000, HashAlgorithmName.SHA256);
            aes.Key = keyIv.GetBytes(32);
            aes.IV = keyIv.GetBytes(16);
            aes.Mode = CipherMode.CBC;

            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
            using (var sw = new StreamWriter(cs))
            {
                sw.Write(plainText);
            }

            return Convert.ToBase64String(ms.ToArray());
        }


        public static string Decrypt(string cipherText, string passphrase)
        {
            if (string.IsNullOrEmpty(cipherText))
                return "";

            var passwordBytes = DeriveKeyFromPassphrase(passphrase);
            var cipherBytes = Convert.FromBase64String(cipherText);

            using var aes = Aes.Create();
            var keyIv = new Rfc2898DeriveBytes(passwordBytes, Salt, 100_000, HashAlgorithmName.SHA256);

            aes.Key = keyIv.GetBytes(32);
            aes.IV = keyIv.GetBytes(16);
            aes.Mode = CipherMode.CBC;

            using var ms = new MemoryStream(cipherBytes);
            using var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);
            return sr.ReadToEnd();
        }

        private static byte[] DeriveKeyFromPassphrase(string passphrase)
        {
            if (string.IsNullOrEmpty(passphrase))
                passphrase = string.Empty;

            using var sha256 = SHA256.Create();
            return SHA256.HashData(Encoding.UTF8.GetBytes(passphrase));
        }
    }

}
