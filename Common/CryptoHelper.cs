using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Common
{
    public static class CryptoHelper
    {



        /// <summary>
        /// Hex 转 Byte
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        private static byte[] hexStringToByte(string hex)
        {
            int target_length = hex.Length >> 1;
            byte[] result = new byte[target_length];
            char[] achar = hex.ToCharArray();
            for (int i = 0, j = 0; i < target_length; i++)
            {
                char c = achar[j++];
                if (c >= '0' && c <= '9')
                {
                    result[i] = (byte)((c - '0') << 4);
                }
                else if (c >= 'a' && c <= 'f')
                {
                    result[i] = (byte)((c - 'a' + 10) << 4);
                }
                else if (c >= 'A' && c <= 'F')
                {
                    result[i] = (byte)((c - 'A' + 10) << 4);
                }
                else
                {
                    return result;
                }
                c = achar[j++];

                if (c >= '0' && c <= '9')
                {
                    result[i] |= (byte)(c - '0');
                }
                else if (c >= 'a' && c <= 'f')
                {
                    result[i] |= (byte)(c - 'a' + 10);
                }
                else if (c >= 'A' && c <= 'F')
                {
                    result[i] |= (byte)(c - 'A' + 10);
                }
                else
                {
                    return result;
                }
            }
            return result;
        }



        /// <summary>
        /// Byte 转 Hex
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        private static string byteToHexString(byte[] b)
        {
            char[] hex = {'0', '1', '2', '3', '4', '5', '6', '7',
                      '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'};
            char[] newChar = new char[b.Length * 2];
            for (int i = 0; i < b.Length; i++)
            {
                newChar[2 * i] = hex[(b[i] & 0xf0) >> 4];
                newChar[2 * i + 1] = hex[b[i] & 0xf];
            }
            return new string(newChar);
        }



        /// <summary>
        /// AES 加密
        /// </summary>
        /// <param name="param">字符串</param>
        /// <param name="skey">密钥</param>
        /// <returns></returns>
        public static string AESEncode(string param, string skey)
        {
            byte[] key = hexStringToByte(skey.ToLower());
            AesCryptoServiceProvider aesProvider = new AesCryptoServiceProvider();
            aesProvider.Key = key;
            aesProvider.Mode = CipherMode.ECB;
            aesProvider.Padding = PaddingMode.PKCS7;
            byte[] inputBuffers = Encoding.UTF8.GetBytes(param);
            ICryptoTransform cryptoTransform = aesProvider.CreateEncryptor();
            byte[] results = cryptoTransform.TransformFinalBlock(inputBuffers, 0, inputBuffers.Length);
            aesProvider.Clear();
            return byteToHexString(results);
        }



        /// <summary>
        /// AES 解密
        /// </summary>
        /// <param name="param">字符串</param>
        /// <param name="skey">密钥</param>
        /// <returns></returns>
        public static string AESDecode(string param, string skey)
        {
            try
            {
                byte[] key = hexStringToByte(skey.ToLower());
                AesCryptoServiceProvider aesProvider = new AesCryptoServiceProvider();
                aesProvider.Key = key;
                aesProvider.Mode = CipherMode.ECB;
                aesProvider.Padding = PaddingMode.PKCS7;
                byte[] inputBuffers = hexStringToByte(param);
                ICryptoTransform cryptoTransform = aesProvider.CreateDecryptor();
                byte[] results = cryptoTransform.TransformFinalBlock(inputBuffers, 0, inputBuffers.Length);
                aesProvider.Clear();
                return Encoding.UTF8.GetString(results);
            }
            catch
            {
                return param;
            }
        }



        /// <summary>
        /// 获取字符串的 MD5 签名
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string GetMd5(string data)
        {
            MD5CryptoServiceProvider MD5 = new MD5CryptoServiceProvider();
            return BitConverter.ToString(MD5.ComputeHash(Encoding.GetEncoding("utf-8").GetBytes(data))).Replace("-", "").ToLower();
        }



        /// <summary>
        /// 获取字符串加 Token 后的 MD5 签名
        /// </summary>
        /// <param name="data"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static string GetMd5(string data, string token)
        {
            string dataMD5 = data + token;
            MD5CryptoServiceProvider MD5 = new MD5CryptoServiceProvider();
            return BitConverter.ToString(MD5.ComputeHash(Encoding.GetEncoding("utf-8").GetBytes(dataMD5))).Replace("-", "").ToLower();
        }



        /// <summary>
        /// 通过Base64加密字符串
        /// </summary>
        /// <param name="text">要加密的字符串</param>
        /// <returns></returns>
        public static string Base64Encode(string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(bytes);
        }




        /// <summary>
        /// 通过Base64解密字符串
        /// </summary>
        /// <param name="text">被加密的字符串</param>
        /// <returns></returns>
        public static string Base64Decode(string text)
        {
            byte[] bytes = Convert.FromBase64String(text);
            return Encoding.UTF8.GetString(bytes);
        }



        /// <summary>
        /// SHA256 签名计算
        /// </summary>
        /// <param name="srcString">The string to be encrypted</param>
        /// <returns></returns>
        public static string Sha256(string srcString)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes_sha256_in = Encoding.UTF8.GetBytes(srcString);
                byte[] bytes_sha256_out = sha256.ComputeHash(bytes_sha256_in);
                string str_sha256_out = BitConverter.ToString(bytes_sha256_out);
                str_sha256_out = str_sha256_out.Replace("-", "");
                return str_sha256_out;
            }
        }



        /// <summary>
        /// Java RSA 私钥转换为 DotNet 格式
        /// </summary>
        /// <param name="privateKey">私钥内容（不要前后缀）</param>
        /// <returns></returns>
        public static string RSAPrivateKeyJava2DotNet(string privateKey)
        {
            RsaPrivateCrtKeyParameters privateKeyParam = (RsaPrivateCrtKeyParameters)PrivateKeyFactory.CreateKey(Convert.FromBase64String(privateKey));

            return string.Format("<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent><P>{2}</P><Q>{3}</Q><DP>{4}</DP><DQ>{5}</DQ><InverseQ>{6}</InverseQ><D>{7}</D></RSAKeyValue>",
                Convert.ToBase64String(privateKeyParam.Modulus.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.PublicExponent.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.P.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.Q.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.DP.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.DQ.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.QInv.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.Exponent.ToByteArrayUnsigned()));
        }



        /// <summary>
        /// 获取数据经过sha256，经过 rsa 加密之后的 hex 值
        /// </summary>
        /// <param name="contentForSign"></param>
        /// <param name="privateKey"></param>
        /// <remarks>Hex.encode(RSAWithSHA256(message)）</remarks>
        /// <returns></returns>
        public static string HexRSASha256Sign(string contentForSign, string privateKey)
        {
            //转换成适用于.Net的秘钥
            var netKey = RSAPrivateKeyJava2DotNet(privateKey);
            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(netKey);
            //创建一个空对象
            var rsaClear = new RSACryptoServiceProvider();
            var paras = rsa.ExportParameters(true);
            rsaClear.ImportParameters(paras);
            //签名返回
            using (var sha256 = new SHA256CryptoServiceProvider())
            {
                var signData = rsa.SignData(Encoding.UTF8.GetBytes(contentForSign), sha256);
                return BytesToHex(signData);
            }
        }



        /// <summary>
        /// Byte 转 Hex
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string BytesToHex(byte[] data)
        {
            StringBuilder sbRet = new StringBuilder(data.Length * 2);
            for (int i = 0; i < data.Length; i++)
            {
                sbRet.Append(Convert.ToString(data[i], 16).PadLeft(2, '0'));
            }
            return sbRet.ToString();
        }

    }
}
