using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Libraries.WeiXin.H5.Models
{
    /// <summary>
    /// PC
    /// </summary>
    public class dtoCreatePayPCH
    {
       public dtoCreatePayPCH()
        {
            TimeSpan cha = DateTime.UtcNow - (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            long t = (long)cha.TotalSeconds;
            timeStamp = t.ToString();
        }

        /// <summary>
        /// 时间戳，从 1970 年 1 月 1 日 00:00:00 至今的秒数，即当前的时间
        /// </summary>
        public string timeStamp { get; set; }


        /// <summary>
        /// 随机字符串，长度为32个字符以下
        /// </summary>
        public string nonceStr { get; set; }


        /// <summary>
        /// 统一下单接口返回的 prepay_id 参数值，提交格式如：prepay_id=***
        /// </summary>
        public string package { get; set; }


        /// <summary>
        /// 签名算法
        /// </summary>
        public string signType
        {
            get
            {
                return "MD5";
            }
        }


        /// <summary>
        /// 签名
        /// </summary>
        public string paySign { get; set; }

        public string mweburl { get; set; }

        /// <summary>
        /// appid
        /// </summary>
        public string appId { get; set; }

        /// <summary>
        /// ip
        /// </summary>
        public string Ip { get; set; }
    }
}
