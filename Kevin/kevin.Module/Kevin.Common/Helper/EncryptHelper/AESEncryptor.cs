using Kevin.Common.Helper;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace NetCore.Util.EncryptHelper
{
    public class AESEncryptor
    {
        private static readonly byte[] Key = Encoding.UTF8.GetBytes(ConfigHelper.Configuration["AESInfo:Key"]); // 16字节密钥
        private static readonly byte[] IV = Encoding.UTF8.GetBytes(ConfigHelper.Configuration["AESInfo:IV"]); // 16字节初始化向量

        public static string EncryptString(string text)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(text);
                        }
                        return Convert.ToBase64String(msEncrypt.ToArray());
                    }
                }
            }
        }

        /// <summary>
        /// 哈希摘要（不可逆，适合去重标识）
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string GenerateHashIdentifier(string data)
        {
            using var sha256 = SHA256.Create();
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(data));
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
        public static string DecryptString(string encryptedText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                byte[] bytes = Convert.FromBase64String(encryptedText);

                using (MemoryStream msDecrypt = new MemoryStream(bytes))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }

        public static string MaskString(string text)
        {
            if (string.IsNullOrEmpty(text) || text.Length != 11)
                return text;

            return text.Substring(0, 3) + "****" + text.Substring(7);
        }
    }
}
