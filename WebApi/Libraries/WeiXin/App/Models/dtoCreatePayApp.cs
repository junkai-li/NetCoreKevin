using System;

namespace WebApi.Libraries.WeiXin.App.Models
{

    /// <summary>
    /// APP微信支付统一下单接口返回类型
    /// </summary>
    public class dtoCreatePayApp
    {

        public dtoCreatePayApp()
        {
            TimeSpan cha = (DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)));
            long t = (long)cha.TotalSeconds;
            timestamp = t.ToString();
        }


        /// <summary>
        /// 应用ID
        /// </summary>
        public string appid { get; set; }



        /// <summary>
        /// 商户号
        /// </summary>
        public string partnerid { get; set; }



        /// <summary>
        /// 预支付交易会话ID
        /// </summary>
        public string prepayid { get; set; }



        /// <summary>
        /// 扩展字段
        /// </summary>
        public string package { get; set; }


        /// <summary>
        /// 随机字符串，长度为32个字符以下
        /// </summary>
        public string noncestr { get; set; }



        /// <summary>
        /// 时间戳，从 1970 年 1 月 1 日 00:00:00 至今的秒数，即当前的时间
        /// </summary>
        public string timestamp { get; set; }



        /// <summary>
        /// 签名
        /// </summary>
        public string sign { get; set; }

    }
}
