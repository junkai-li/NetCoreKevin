using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Common
{

    /// <summary>
    /// 针对string字符串常用操作方法
    /// </summary>
    public static class StringHelper
    {


        /// <summary>
        /// 生成一个订单号
        /// </summary>
        /// <returns></returns>
        public static string GetOrderNo(string sign)
        {
            string orderno = "";

            Random ran = new Random();
            int RandKey = ran.Next(10000, 99999);


            orderno = sign + DateTime.Now.ToString("yyyyMMddHHmmssfff") + RandKey;

            return orderno;
        }



        /// <summary>
        /// 移除字符串中的全部标点符号
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string RemovePunctuation(string text)
        {
            text = new string(text.Where(c => !char.IsPunctuation(c)).ToArray());

            return text;
        }



        /// <summary>
        /// 判断字符串中是否包含中文
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool IsContainsCN(string text)
        {
            Regex reg = new Regex(@"[\u4e00-\u9fa5]");//正则表达式

            if (reg.IsMatch(text))
            {
                return true;
            }
            else
            {
                return false;
            }
        }



        /// <summary>
        /// 从传入的HTML代码中提取文本内容
        /// </summary>
        /// <param name="Htmlstring"></param>
        /// <returns></returns>
        public static string NoHtml(string Htmlstring)
        {

            //删除脚本

            Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "",

            RegexOptions.IgnoreCase);

            //删除HTML

            Htmlstring = Regex.Replace(Htmlstring, @"<(.[^>]*)>", "",

            RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"([\r\n])[\s]+", "",

            RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"-->", "", RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "\"",

            RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&",

            RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<",

            RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", ">",

            RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", " ",

            RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "\xa1",

            RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "\xa2",

            RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "\xa3",

            RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "\xa9",

            RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", "",

            RegexOptions.IgnoreCase);

            Htmlstring.Replace("<", "");

            Htmlstring.Replace(">", "");

            Htmlstring.Replace("\r\n", "");

            Htmlstring = WebUtility.HtmlEncode(Htmlstring).Trim();

            return Htmlstring;

        }


        /// <summary>
        /// 过滤删除掉字符串中的 Emoji 表情
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string NoEmoji(string value)
        {
            foreach (var a in value)
            {
                byte[] bts = Encoding.UTF32.GetBytes(a.ToString());

                if (bts[0].ToString() == "253" && bts[1].ToString() == "255")
                {
                    value = value.Replace(a.ToString(), "");
                }

            }

            return value;
        }



        /// <summary>
        /// 对文本进行指定长度截取并添加省略号
        /// </summary>
        /// <param name="NeiRong"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string SubstringText(string NeiRong, int length)
        {
            //先对字符串做一次HTML解码
            NeiRong = HttpUtility.HtmlDecode(NeiRong);

            if (NeiRong.Length > length)
            {
                NeiRong = NeiRong.Substring(0, length);

                NeiRong = NeiRong + "...";

                return NoHtml(NeiRong);
            }
            else
            {
                return NoHtml(NeiRong);
            }
        }



        /// <summary>
        /// 对字符串进行脱敏处理
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string TextStars(string text)
        {
            if (text.Length >= 3)
            {
                int group = text.Length / 3;

                string stars = text.Substring(group, group);

                string pstars = "";

                for (int i = 0; i < group; i++)
                {
                    pstars = pstars + "*";
                }

                text = text.Replace(stars, pstars);
            }
            else
            {

                string stars = text.Substring(1, 1);

                string pstars = "";

                for (int i = 0; i < 1; i++)
                {
                    pstars = pstars + "*";
                }

                text = text.Replace(stars, pstars);
            }

            return text;
        }


        /// <summary>
        /// Unicode转换中文
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string UnicodeToString(string text)
        {
            return Regex.Unescape(text);
        }




        /// <summary>
        /// 去掉字符串中的数字
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string RemoveNumber(string key)
        {
            return System.Text.RegularExpressions.Regex.Replace(key, @"\d", "");
        }



        /// <summary>
        /// 去掉字符串中的非数字
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string RemoveNotNumber(string key)
        {
            return System.Text.RegularExpressions.Regex.Replace(key, @"[^\d]*", "");
        }
        #region 特殊字符    

        /// <summary>    
        /// 检测是否有Sql危险字符    
        /// </summary>    
        /// <param name="str">要判断字符串</param>    
        /// <returns>判断结果</returns>    
        public static bool IsSafeSqlString(string str)
        {
            return !Regex.IsMatch(str, @"[-|;|,|\/|||||\}|\{|%|@|\*|!|\']");
        }

        /// <summary>    
        /// 删除SQL注入特殊字符    
        /// 加入对输入参数sql为Null的判断    
        /// </summary>    
        public static string StripSqlInjection(string sql)
        {
            if (!string.IsNullOrEmpty(sql))
            {
                //过滤 ' --    
                const string pattern1 = @"(\%27)|(\')|(\-\-)";

                //防止执行 ' or    
                const string pattern2 = @"((\%27)|(\'))\s*((\%6F)|o|(\%4F))((\%72)|r|(\%52))";

                //防止执行sql server 内部存储过程或扩展存储过程    
                const string pattern3 = @"\s+exec(\s|\+)+(s|x)p\w+";

                sql = Regex.Replace(sql, pattern1, string.Empty, RegexOptions.IgnoreCase);
                sql = Regex.Replace(sql, pattern2, string.Empty, RegexOptions.IgnoreCase);
                sql = Regex.Replace(sql, pattern3, string.Empty, RegexOptions.IgnoreCase);
            }
            return sql;
        }

        public static string SqlSafe(string parameter)
        {
            parameter = parameter.ToLower();
            parameter = parameter.Replace("'", "");
            parameter = parameter.Replace(">", ">");
            parameter = parameter.Replace("<", "<");
            parameter = parameter.Replace("\n", "<br>");
            parameter = parameter.Replace("\0", "·");
            return parameter;
        }

        /// <summary>    
        /// 清除xml中的不合法字符    
        /// </summary>    
        /// <remarks>    
        /// 无效字符：    
        /// 0x00 - 0x08    
        /// 0x0b - 0x0c    
        /// 0x0e - 0x1f    
        /// </remarks>    
        public static string CleanInvalidCharsForXml(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;
            else
            {
                StringBuilder checkedStringBuilder = new StringBuilder();
                Char[] chars = input.ToCharArray();
                for (int i = 0; i < chars.Length; i++)
                {
                    int charValue = Convert.ToInt32(chars[i]);

                    if ((charValue >= 0x00 && charValue <= 0x08) || (charValue >= 0x0b && charValue <= 0x0c) || (charValue >= 0x0e && charValue <= 0x1f))
                        continue;
                    else
                        checkedStringBuilder.Append(chars[i]);
                }

                return checkedStringBuilder.ToString();

                //string result = checkedStringBuilder.ToString();    
                //result = result.Replace("&#x0;", "");    
                //return Regex.Replace(result, @"[\?-\\ \ \-\\?-\?]", delegate(Match m) { int code = (int)m.Value.ToCharArray()[0]; return (code > 9 ? "&#" + code.ToString() : "&#0" + code.ToString()) + ";"; });    
            }
        }


        /// <summary>    
        /// 改正sql语句中的转义字符    
        /// </summary>    
        public static string MashSql(string str)
        {
            return (str == null) ? "" : str.Replace("\'", "'");
        }

        /// <summary>    
        /// 替换sql语句中的有问题符号   
        /// </summary>    
        public static string ChkSql(string str)
        {
            return (str == null) ? "" : str.Replace("'", "''");
        }

        /// <summary>    
        ///  判断是否有非法字符   
        /// </summary>    
        /// <param name="strString"></param>    
        /// <returns>返回TRUE表示有非法字符，返回FALSE表示没有非法字符。</returns>    
        public static bool CheckBadStr(string strString)
        {
            bool outValue = false;
            if (!string.IsNullOrEmpty(strString))
            {
                ArrayList bidStrlist = new ArrayList();
                bidStrlist.Add("xp_cmdshell");
                bidStrlist.Add("truncate");
                bidStrlist.Add("net user");
                bidStrlist.Add("exec");
                bidStrlist.Add("net localgroup");
                bidStrlist.Add("select");
                bidStrlist.Add("asc");
                bidStrlist.Add("char");
                bidStrlist.Add("mid");
                bidStrlist.Add("insert");
                bidStrlist.Add("order");
                bidStrlist.Add("exec");
                bidStrlist.Add("delete");
                bidStrlist.Add("drop");
                bidStrlist.Add("truncate");
                bidStrlist.Add("1=1");
                bidStrlist.Add("1=2");
                string tempStr = strString.ToLower();
                for (int i = 0; i < bidStrlist.Count; i++)
                {
                    if (tempStr.IndexOf(bidStrlist[i].ToString(), StringComparison.Ordinal) > -1)
                    {
                        outValue = true;
                        break;
                    }
                }
            }
            return outValue;
        }

        #endregion
    }
}
