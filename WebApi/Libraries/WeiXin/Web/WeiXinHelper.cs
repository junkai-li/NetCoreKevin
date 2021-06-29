using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebApi.Libraries.WeiXin.Web
{
    public class WeiXinHelper
    {
        private string appid;

        private string secret;

        private string mchid;

        private string mchkey;

        private string notifyurl;

        public WeiXinHelper(string in_appid, string in_secret, string in_mchid = null, string in_mchkey = null, string in_notifyurl = null)
        {
            appid = in_appid;
            secret = in_secret;
            mchid = in_mchid;
            mchkey = in_mchkey;
            notifyurl = in_notifyurl;
        }




        /// <summary>
        /// 微信网页支付商户平台下单方法
        /// </summary>
        /// <param name="productid">产品ID</param>
        /// <param name="orderno">订单号</param>
        /// <param name="body">商品描述</param>
        /// <param name="price">价格，单位为分</param>
        /// <param name="ip">服务器IP</param>
        /// <returns></returns>
        public string CreatePay(Guid productid, string orderno, string body, int price, string ip)
        {

            string nonceStr = Guid.NewGuid().ToString().Replace("-", "");


            var url = "https://api.mch.weixin.qq.com/pay/unifiedorder";//微信统一下单请求地址




            //参与统一下单签名的参数，除最后的key外，已经按参数名ASCII码从小到大排序
            var unifiedorderSignParam = string.Format("appid={0}&body={1}&mch_id={2}&nonce_str={3}&notify_url={4}&out_trade_no={5}&product_id={6}&spbill_create_ip={7}&total_fee={8}&trade_type={9}&key={10}"
                , appid, body, mchid, nonceStr, notifyurl
                , orderno, productid, ip, price, "NATIVE", mchkey);


            var unifiedorderSign = Common.CryptoHelper.GetMd5(unifiedorderSignParam).ToUpper();

            //构造统一下单的请求参数
            var zhi = string.Format(@"<xml>
                                <appid>{0}</appid>                                              
                                <body>{1}</body>
                                <mch_id>{2}</mch_id>   
                                <nonce_str>{3}</nonce_str>
                                <notify_url>{4}</notify_url>
                                <out_trade_no>{5}</out_trade_no>
                                <product_id>{6}</product_id>
                                <spbill_create_ip>{7}</spbill_create_ip>
                                <total_fee>{8}</total_fee>
                                <trade_type>{9}</trade_type>
                                <sign>{10}</sign>
                               </xml>
                    ", appid, body, mchid, nonceStr, notifyurl
                              , orderno, productid, ip, price, "NATIVE", unifiedorderSign);




            var getdata = Common.HttpHelper.Post(url, zhi, "form");

            //获取xml数据
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(getdata);


            //xml格式转json
            string json = Newtonsoft.Json.JsonConvert.SerializeXmlNode(doc);
            JObject jo = (JObject)JsonConvert.DeserializeObject(json);


            if (jo["xml"]["return_code"]["#cdata-section"].ToString() == "SUCCESS")
            {
                string prepay_id = jo["xml"]["prepay_id"]["#cdata-section"].ToString();


                string codeUrl = jo["xml"]["code_url"]["#cdata-section"].ToString();


                return codeUrl;
            }
            else
            {
                return null;
            }
        }


    }
}
