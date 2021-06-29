using System;
using System.Collections.Generic;
using System.Text;

namespace WebApi.Libraries.WeiXin.MiniApp.Models
{

    /// <summary>
    /// 小程序微信支付统一下单接口返回类型
    /// </summary>
    public class dtoCreatePayMiniApp
    {

        public dtoCreatePayMiniApp()
        {
            TimeSpan cha = (DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)));
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

    }
}
