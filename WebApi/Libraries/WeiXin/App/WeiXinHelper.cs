using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Xml;
using WebApi.Libraries.WeiXin.App.Models;

namespace WebApi.Libraries.WeiXin.App
{
    public class WeiXinHelper
    {
        private string appid;

        private string appsecret;

        private string mchid;

        private string mchkey;

        private string notifyurl;

        public WeiXinHelper(string in_appid, string in_secret, string in_mchid = null, string in_mchkey = null, string in_notifyurl = null)
        {
            appid = in_appid;
            appsecret = in_secret;
            mchid = in_mchid;
            mchkey = in_mchkey;
            notifyurl = in_notifyurl;
        }




        /// <summary>
        /// 微信APP支付商户平台下单方法
        /// </summary>
        /// <param name="orderno">订单号</param>
        /// <param name="title">商品名称</param>
        /// <param name="body">商品描述</param>
        /// <param name="price">价格，单位为分</param>
        /// <param name="ip">服务器IP</param>
        /// <returns></returns>
        public dtoCreatePayApp CreatePay(string orderno, string title, string body, int price, string ip)
        {

            string nonceStr = Guid.NewGuid().ToString().Replace("-", "");


            var url = "https://api.mch.weixin.qq.com/pay/unifiedorder";//微信统一下单请求地址




            //参与统一下单签名的参数，除最后的key外，已经按参数名ASCII码从小到大排序
            var unifiedorderSignParam = string.Format("appid={0}&body={1}&mch_id={2}&nonce_str={3}&notify_url={4}&out_trade_no={5}&spbill_create_ip={6}&total_fee={7}&trade_type={8}&key={9}"
                , appid, body, mchid, nonceStr, notifyurl
                , orderno, ip, price, "APP", mchkey);


            var unifiedorderSign = Common.CryptoHelper.GetMd5(unifiedorderSignParam).ToUpper();

            //构造统一下单的请求参数
            var zhi = string.Format(@"<xml>
                                <appid>{0}</appid>                                              
                                <body>{1}</body>
                                <mch_id>{2}</mch_id>   
                                <nonce_str>{3}</nonce_str>
                                <notify_url>{4}</notify_url>
                                <out_trade_no>{5}</out_trade_no>
                                <spbill_create_ip>{6}</spbill_create_ip>
                                <total_fee>{7}</total_fee>
                                <trade_type>{8}</trade_type>
                                <sign>{9}</sign>
                               </xml>
                    ", appid, body, mchid, nonceStr, notifyurl
                              , orderno, ip, price, "APP", unifiedorderSign);




            var getdata = HttpHelper.Post(url, zhi, "form");

            //获取xml数据
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(getdata);


            //xml格式转json
            string json = Newtonsoft.Json.JsonConvert.SerializeXmlNode(doc);
            JObject jo = (JObject)JsonConvert.DeserializeObject(json);


            if (jo["xml"]["return_code"]["#cdata-section"].ToString() == "SUCCESS")
            {
                string prepay_id = jo["xml"]["prepay_id"]["#cdata-section"].ToString();

                var info = new dtoCreatePayApp();
                info.appid = appid;
                info.partnerid = mchid;
                info.prepayid = prepay_id;
                info.package = "Sign=WXPay";
                info.noncestr = nonceStr;

                //再次签名返回数据至APP
                string strB = "appid=" + appid + "&noncestr=" + nonceStr + "&package=Sign=WXPay&partnerid=" + info.partnerid + "&prepayid=" + prepay_id + "&timestamp=" + info.timestamp + "&key=" + mchkey;

                info.sign = Common.CryptoHelper.GetMd5(strB).ToUpper();

                return info;
            }
            else
            {
                return null;
            }
        }



        /// <summary>
        /// 获取 AccessToken
        /// </summary>
        /// <returns></returns>
        public (string accessToken, string openId) GetAccessToken(string code)
        {
            string token = Common.RedisHelper.StringGet("wxappaccesstoken" + code);
            string openid = Common.RedisHelper.StringGet("wxappopenid" + code);

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(openid))
            {

                string url = "https://api.weixin.qq.com/sns/oauth2/access_token?appid=" + appid + "&secret=" + appsecret + "&code=" + code + "&grant_type=authorization_code";

                var returnJson = HttpHelper.Post(url, "", "form");

                token = Common.Json.JsonHelper.GetValueByKey(returnJson, "access_token");
                openid = Common.Json.JsonHelper.GetValueByKey(returnJson, "openid");

                if (!string.IsNullOrEmpty(token))
                {
                    Common.RedisHelper.StringSet("wxappaccesstoken" + code, token, TimeSpan.FromSeconds(7100));
                    Common.RedisHelper.StringSet("wxappopenid" + code, openid, TimeSpan.FromSeconds(7100));
                }
            }

            return (token, openid);
        }



        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="openId"></param>
        /// <returns></returns>
        public GetUserInfo GetUserInfo(string accessToken, string openId)
        {
            string url = "https://api.weixin.qq.com/sns/userinfo?access_token=" + accessToken + "&openid=" + openId;

            var returnJson = HttpHelper.Post(url, "", "form");

            var userInfo = Common.Json.JsonHelper.JSONToObject<GetUserInfo>(returnJson);

            return userInfo;
        }
    }
}
