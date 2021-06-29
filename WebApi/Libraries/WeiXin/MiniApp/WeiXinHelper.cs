using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using WebApi.Libraries.WeiXin.MiniApp.Models;
using WebApi.Libraries.WeiXin.Public;

namespace WebApi.Libraries.WeiXin.MiniApp
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
        /// 获取用户OpenId 和 SessionKey
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public (string openid, string sessionkey) GetOpenIdAndSessionKey(string code)
        {
            string url = "https://api.weixin.qq.com/sns/jscode2session?appid=" + appid + "&secret=" + secret + "&js_code=" + code + "&grant_type=authorization_code";

            string httpret = Common.HttpHelper.Get(url);

            try
            {
                string openid = Common.Json.JsonHelper.GetValueByKey(httpret, "openid");
                string sessionkey = Common.Json.JsonHelper.GetValueByKey(httpret, "session_key");

                Common.RedisHelper.StringSet(code, httpret, new TimeSpan(0, 0, 10));

                return (openid, sessionkey);
            }
            catch
            {
                string errcode = Common.Json.JsonHelper.GetValueByKey(httpret, "errcode");

                if (errcode == "40163")
                {
                    var cachHttpRet = Common.RedisHelper.StringGet(code);

                    if (!string.IsNullOrEmpty(cachHttpRet))
                    {
                        httpret = cachHttpRet;
                    }
                }

                string openid = Common.Json.JsonHelper.GetValueByKey(httpret, "openid");
                string sessionkey = Common.Json.JsonHelper.GetValueByKey(httpret, "session_key");

                return (openid, sessionkey);
            }
        }


        /// <summary>
        /// 微信小程序支付商户平台下单方法
        /// </summary>
        /// <param name="openid">用户 OpenId</param>
        /// <param name="orderno">订单号</param>
        /// <param name="title">商品名称</param>
        /// <param name="body">商品描述</param>
        /// <param name="price">价格，单位为分</param>
        /// <returns></returns>
        public dtoCreatePayMiniApp CreatePay(string openid, string orderno, string title, string body, int price)
        {

            string nonceStr = Guid.NewGuid().ToString().Replace("-", "");


            var url = "https://api.mch.weixin.qq.com/pay/unifiedorder";//微信统一下单请求地址


            //参与统一下单签名的参数，除最后的key外，已经按参数名ASCII码从小到大排序
            var unifiedorderSignParam = string.Format("appid={0}&body={1}&mch_id={2}&nonce_str={3}&notify_url={4}&openid={5}&out_trade_no={6}&total_fee={7}&trade_type={8}&key={9}"
                , appid, body, mchid, nonceStr, notifyurl
                , openid, orderno, price, "JSAPI", mchkey);


            var unifiedorderSign = Common.CryptoHelper.GetMd5(unifiedorderSignParam).ToUpper();

            //构造统一下单的请求参数
            var zhi = string.Format(@"<xml>
                                <appid>{0}</appid>                                              
                                <body>{1}</body>
                                <mch_id>{2}</mch_id>   
                                <nonce_str>{3}</nonce_str>
                                <notify_url>{4}</notify_url>
                                <openid>{5}</openid>
                                <out_trade_no>{6}</out_trade_no>
                                <total_fee>{7}</total_fee>
                                <trade_type>{8}</trade_type>
                                <sign>{9}</sign>
                               </xml>
                    ", appid, body, mchid, nonceStr, notifyurl, openid
                              , orderno, price, "JSAPI", unifiedorderSign);


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

                var info = new dtoCreatePayMiniApp();
                info.nonceStr = nonceStr;
                info.package = "prepay_id=" + prepay_id;

                //再次签名返回数据至小程序
                string strB = "appId=" + appid + "&nonceStr=" + nonceStr + "&package=prepay_id=" + prepay_id + "&signType=MD5&timeStamp=" + info.timeStamp + "&key=" + mchkey;

                info.paySign = Common.CryptoHelper.GetMd5(strB).ToUpper();

                return info;
            }
            else
            {
                return null;
            }
        }



        /// <summary>
        /// 微信小程序 encryptedData 解密
        /// </summary>
        /// <param name="encryptedDataStr">encryptedDataStr</param>
        /// <param name="key">session_key</param>
        /// <param name="iv">iv</param>
        /// <returns></returns>
        public static string DecryptionData(string encryptedDataStr, string key, string iv)
        {
            var rijalg = Aes.Create();
            // RijndaelManaged rijalg = new RijndaelManaged();
            //----------------- 
            //设置 cipher 格式 AES-128-CBC 

            rijalg.KeySize = 128;

            rijalg.Padding = PaddingMode.PKCS7;
            rijalg.Mode = CipherMode.CBC;

            rijalg.Key = Convert.FromBase64String(key);
            rijalg.IV = Convert.FromBase64String(iv);


            byte[] encryptedData = Convert.FromBase64String(encryptedDataStr);
            //解密 
            ICryptoTransform decryptor = rijalg.CreateDecryptor(rijalg.Key, rijalg.IV);

            string result;

            using (MemoryStream msDecrypt = new MemoryStream(encryptedData))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {

                        result = srDecrypt.ReadToEnd();
                    }
                }
            }
            return result;
        }



        /// <summary>
        /// 证书双向校验POST
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private string UseCretPost(string url, string data)
        {

            var sslPath = IO.Path.ContentRootPath() + "/ssl/apiclient_cert.p12";

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);

            X509Certificate2 cert = new X509Certificate2(sslPath, mchid, X509KeyStorageFlags.MachineKeySet);

            req.ClientCertificates.Add(cert);


            byte[] requestBytes = System.Text.Encoding.UTF8.GetBytes(data);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";

            req.ContentLength = requestBytes.Length;
            Stream requestStream = req.GetRequestStream();
            requestStream.Write(requestBytes, 0, requestBytes.Length);
            requestStream.Close();

            HttpWebResponse res = (HttpWebResponse)req.GetResponse();
            StreamReader sr = new StreamReader(res.GetResponseStream(), System.Text.Encoding.UTF8);
            string PostJie = sr.ReadToEnd();
            sr.Close();
            res.Close();

            return PostJie;
        }





        /// <summary>
        /// 微信小程序支付创建退款申请方法
        /// </summary>
        /// <param name="out_refund_no">商户退款单号</param>
        /// <param name="refund_fee">退款金额</param>
        /// <param name="total_fee">支付单总金额</param>
        /// <param name="transaction_id">微信支付订单号</param>
        /// <returns></returns>
        public dtoCreatePayRefundMiniApp CreateRefund(string out_refund_no, int refund_fee, int total_fee, string transaction_id)
        {

            string nonceStr = Guid.NewGuid().ToString().Replace("-", "");


            //微信退款接口地址
            var url = "https://api.mch.weixin.qq.com/secapi/pay/refund";


            //参与退款接口的参数，除最后的key外，已经按参数名ASCII码从小到大排序
            var unifiedorderSignParam = string.Format("appid={0}&mch_id={1}&nonce_str={2}&notify_url={3}&out_refund_no={4}&refund_fee={5}&total_fee={6}&transaction_id={7}&key={8}"
                , appid, mchid, nonceStr, notifyurl
                , out_refund_no, refund_fee, total_fee, transaction_id, mchkey);


            var unifiedorderSign = Common.CryptoHelper.GetMd5(unifiedorderSignParam).ToUpper();

            //构造退款的请求参数
            var zhi = string.Format(@"<xml>
                                <appid>{0}</appid>                                              
                                <mch_id>{1}</mch_id>   
                                <nonce_str>{2}</nonce_str>
                                <sign>{3}</sign>
                                <transaction_id>{4}</transaction_id>
                                <out_refund_no>{5}</out_refund_no>
                                <total_fee>{6}</total_fee>
                                <refund_fee>{7}</refund_fee>
                                <notify_url>{8}</notify_url>
                               </xml>
                    ", appid, mchid, nonceStr, unifiedorderSign, transaction_id, out_refund_no, total_fee, refund_fee, notifyurl);


            var getdata = UseCretPost(url, zhi);

            var wxPayData = new WxPayData();

            wxPayData.FromXml(getdata, mchkey);


            var retInfo = new dtoCreatePayRefundMiniApp();

            retInfo.return_code = wxPayData.GetValue("return_code").ToString();
            retInfo.return_msg = wxPayData.GetValue("return_msg").ToString();

            retInfo.appid = wxPayData.GetValue("appid").ToString();
            retInfo.mch_id = wxPayData.GetValue("mch_id").ToString();
            retInfo.nonce_str = wxPayData.GetValue("nonce_str").ToString();
            retInfo.sign = wxPayData.GetValue("sign").ToString();

            retInfo.result_code = wxPayData.GetValue("result_code").ToString();


            if (retInfo.result_code == "SUCCESS")
            {
                retInfo.transaction_id = wxPayData.GetValue("transaction_id").ToString();
                retInfo.out_trade_no = wxPayData.GetValue("out_trade_no").ToString();
                retInfo.out_refund_no = wxPayData.GetValue("out_refund_no").ToString();
                retInfo.refund_id = wxPayData.GetValue("refund_id").ToString();
                retInfo.refund_fee = Convert.ToInt32(wxPayData.GetValue("refund_fee"));
                retInfo.total_fee = Convert.ToInt32(wxPayData.GetValue("total_fee"));
                retInfo.cash_fee = Convert.ToInt32(wxPayData.GetValue("cash_fee"));
                retInfo.cash_refund_fee = Convert.ToInt32(wxPayData.GetValue("cash_refund_fee"));
            }
            else
            {
                retInfo.err_code = wxPayData.GetValue("err_code").ToString();
                retInfo.err_code_des = wxPayData.GetValue("err_code_des").ToString();
            }

            return retInfo;
        }


    }
}
