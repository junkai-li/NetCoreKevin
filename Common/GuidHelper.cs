using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{

    /// <summary>
    /// GUID自定义操作类
    /// </summary>
    public static class GuidHelper
    {

        /// <summary>
        /// 压缩GUID到22位的字符串
        /// </summary>
        /// <param name="value">待压缩的GUID</param>
        /// <returns></returns>
        public static string Reduce(Guid value)
        {

            string base64 = Convert.ToBase64String(value.ToByteArray());

            string encoded = base64.Replace("/", "_").Replace("+", "-");

            return encoded.Substring(0, 22);
        }



        /// <summary>
        /// 将22位的字符串还原为GUID
        /// </summary>
        /// <param name="vlaue">GUID压缩后的字符串</param>
        /// <returns></returns>
        public static Guid Reduction(string vlaue)
        {
            Guid result = Guid.Empty;

            if ((!string.IsNullOrEmpty(vlaue)) && (vlaue.Trim().Length == 22))
            {
                string encoded = string.Concat(vlaue.Trim().Replace("-", "+").Replace("_", "/"), "==");

                try
                {
                    byte[] base64 = Convert.FromBase64String(encoded);

                    result = new Guid(base64);
                }
                catch (FormatException)
                {
                }
            }

            return result;
        }
    }
}
