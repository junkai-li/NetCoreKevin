using Common.Json;
using kevin.Domain.Share.Interfaces;
using Kevin.Common.App;
using Kevin.Common.App.Global;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Security.Cryptography;
using Web.Libraries.WeiXin.H5.Models;
using Web.Libraries.WeiXin.Public;

namespace Web.Libraries.WeiXin.H5
{

    public class WeiXinHelper
    {


        private static string appid;

        private static string secret; 

        private string mchid;

        private string mchkey;

        private string notifyurl;

        /// <summary>
        /// 微信H5网页开发帮助类
        /// </summary>
        /// <param name="in_appid">微信公众号APPID</param>
        /// <param name="in_appsecret">微信公众号密钥</param>
        public WeiXinHelper(string in_appid, string in_secret, string in_mchid = null, string in_mchkey = null, string in_notifyurl = null)
        {
            appid = in_appid;
            secret = in_secret;
            mchid = in_mchid;
            mchkey = in_mchkey;
            notifyurl = in_notifyurl;
        }



        /// <summary>
        /// 获取 AccessToken
        /// </summary>
        /// <returns></returns>
        public string GetAccessToken()
        {

            string key = appid + secret + "accesstoken";

            var token = GlobalServices.ServiceProvider.GetService<ICacheService>().GetString(key);

            if (string.IsNullOrEmpty(token))
            {
                string url = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=" + appid + "&secret=" + secret;

                var returnJson = Common.HttpHelper.Post(url, "", "form");

                token = JsonHelper.GetValueByKey(returnJson, "access_token");

                GlobalServices.ServiceProvider.GetService<ICacheService>().SetString(key, token, TimeSpan.FromSeconds(6000));
            }

            return token;
        }



        /// <summary>
        /// 获取 TicketID
        /// </summary>
        /// <returns></returns>
        private string GetTicketID()
        {

            string key = appid + secret + "ticketid";

            var ticketid = GlobalServices.ServiceProvider.GetService<ICacheService>().GetString(key);

            if (string.IsNullOrEmpty(ticketid))
            {

                string getUrl = "https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token=" + GetAccessToken() + "&type=jsapi";

                string returnJson = Common.HttpHelper.Post(getUrl, "", "form");

                ticketid = JsonHelper.GetValueByKey(returnJson, "ticket");

                GlobalServices.ServiceProvider.GetService<ICacheService>().SetString(key, ticketid, TimeSpan.FromSeconds(6000));
            }

            return ticketid;
        }



        /// <summary>
        /// 获取 JsSDK 签名信息
        /// </summary>
        /// <returns></returns>
        public WeiXinJsSdkSign GetJsSDKSign()
        {
            WeiXinJsSdkSign sdkSign = new();

            sdkSign.appid = appid;
            string url = KevinHttpContext.GetUrl(GlobalServices.ServiceProvider.GetService<IHttpContextAccessor>());
            sdkSign.timestamp = Common.DateTimeHelper.TimeToUnix(DateTime.Now);
            sdkSign.noncestr = Guid.NewGuid().ToString().Replace("-", "");
            string jsapi_ticket = GetTicketID();

            string strYW = "jsapi_ticket=" + jsapi_ticket + "&noncestr=" + sdkSign.noncestr + "&timestamp=" + sdkSign.timestamp + "&url=" + url;

            using (var sha1 = SHA1.Create())
            {
                byte[] bytes_sha1_in = System.Text.UTF8Encoding.Default.GetBytes(strYW);
            byte[] bytes_sha1_out = sha1.ComputeHash(bytes_sha1_in);
            string str_sha1_out = BitConverter.ToString(bytes_sha1_out);
            str_sha1_out = str_sha1_out.Replace("-", "").ToLower();
            sdkSign.signature = str_sha1_out;

            } 
            return sdkSign;
        }

        /// <summary>
        /// 微信H5支付商户平台下单方法
        /// </summary>
        /// <param name="openid">用户 OpenId</param>
        /// <param name="orderno">订单号</param>
        /// <param name="title">商品名称</param>
        /// <param name="body">商品描述</param>
        /// <param name="price">价格，单位为分</param>
        /// <param name="ip">价格，单位为分</param>
        /// <returns></returns>
        public dtoCreatePayPCH CreatePayUrl(string openid, string orderno, string title, string body, int price, string ip)
        {
            string nonceStr = Guid.NewGuid().ToString().Replace("-", "");
            WxPayData data = new();
            data.SetValue("body", body); //商品描述 
            data.SetValue("out_trade_no", orderno); //商户订单号
            data.SetValue("total_fee", price); //订单总金额,以分为单位
            data.SetValue("trade_type", "MWEB");//交易类型 
            data.SetValue("notify_url", notifyurl); //异步通知url
            data.SetValue("spbill_create_ip", ip); //终端IP
            data.SetValue("appid", appid); //公众账号ID
            data.SetValue("mch_id", mchid); //商户号
            data.SetValue("nonce_str", nonceStr); //随机字符串
            data.SetValue("scene_info", "{\"h5_info\": {\"type\":\"wap\",\"wap_url\": \"http://m.gwbnsh.net.cn\",\"wap_name\": \"深圳酷奇会员\"}} "); //场景信息
            data.SetValue("sign", data.MakeSign(mchkey)); //签名
            string xml = data.ToXml(); //转换成XML 
            var url = "https://api.mch.weixin.qq.com/pay/unifiedorder";//微信统一下单请求地址 

            var getdata = Common.HttpHelper.Post(url, xml, "form");

            ////获取xml数据
            //XmlDocument doc = new XmlDocument();
            //doc.LoadXml(getdata);

            WxPayData result = new();
            result.FromXml(getdata, mchkey);
            //xml格式转json
            //string json = Newtonsoft.Json.JsonConvert.SerializeXmlNode(doc);
            //JObject jo = (JObject)JsonConvert.DeserializeObject(json);
            if (result.GetValue("result_code").ToString().ToUpper() == "SUCCESS")
            {
                var mweb_url = result.GetValue("mweb_url").ToString();//mweb_url为拉起微信支付收银台的中间页面，可通过访问该url来拉起微信客户端，完成支付,mweb_url的有效期为5分钟。
                dtoCreatePayPCH dtoCreatePayPCH = new();
                dtoCreatePayPCH.mweburl = result.GetValue("mweb_url").ToString();
                dtoCreatePayPCH.nonceStr = nonceStr;
                dtoCreatePayPCH.paySign = data.MakeSign(mchkey);
                dtoCreatePayPCH.package = "prepay_id=" + result.GetValue("prepay_id").ToString();
                dtoCreatePayPCH.Ip = ip;
                dtoCreatePayPCH.appId = appid;
                return dtoCreatePayPCH;                                          //Response.Redirect(mweb_url);
            }
            else
            {
                return null;
            }
        }

    }

}
