using Aop.Api.Util;
using Common.AliPay;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.Database;
using Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using WebApi.Libraries.WeiXin.MiniApp.Models;
using WebApi.Libraries.WeiXin.Public;
using System.IO;
using WebApi.Libraries.WeiXin.App.Models;

namespace WebApi.Controllers.v1
{
    /// <summary>
    /// 第三方支付发起集合，依赖于订单号
    /// </summary>
    [ApiVersion("1")]
    [Route("api/[controller]")]
    [ApiController]
    public class PayController : ControllerBase
    {


        private readonly dbContext db;

        public PayController(dbContext context)
        {
            db = context;
        }


        /// <summary>
        /// 微信小程序商户平台下单接口
        /// </summary>
        /// <remarks>用于在微信商户平台创建订单</remarks>
        /// <returns></returns>
        [HttpGet("CreateWeiXinMiniAppPay")]
        public dtoCreatePayMiniApp CreateWeiXinMiniAppPay(string orderno, Guid weixinkeyid)
        {

            var order = db.TOrder.Where(t => t.OrderNo == orderno).Select(t => new
            {
                t.OrderNo,
                t.Price,
                ProductName = DateTime.Now.ToString("yyyyMMddHHmm") + "交易",
                t.CreateUserId,
                UserOpenId = db.TUserBindWeixin.Where(w => w.UserId == t.CreateUserId & w.WeiXinKeyId == weixinkeyid).Select(w => w.WeiXinOpenId).FirstOrDefault()
            }).FirstOrDefault();

            var weixinkey = db.TWeiXinKey.Where(t => t.Id == weixinkeyid).FirstOrDefault();

            var url = Libraries.Http.HttpContext.GetBaseUrl() + "/api/Pay/WeiXinPayNotify";

            var weiXinHelper = new Libraries.WeiXin.MiniApp.WeiXinHelper(weixinkey.WxAppId, weixinkey.WxAppSecret, weixinkey.MchId, weixinkey.MchKey, url);

            int price = Convert.ToInt32(order.Price * 100);

            var pay = weiXinHelper.CreatePay(order.UserOpenId, order.OrderNo, order.ProductName, order.ProductName, price);

            return pay;

        }



        /// <summary>
        /// 微信APP商户平台下单接口
        /// </summary>
        /// <remarks>用于在微信商户平台创建订单</remarks>
        /// <returns></returns>
        [HttpGet("CreateWeiXinAppPay")]
        public dtoCreatePayApp CreateWeiXinAppPay(string orderno, string weixinkeyid)
        {


            var order = db.TOrder.Where(t => t.OrderNo == orderno).FirstOrDefault();

            var weixinkey = db.TWeiXinKey.Where(t => t.IsDelete == false).FirstOrDefault();

            var url = Libraries.Http.HttpContext.GetBaseUrl() + "/api/Pay/WeiXinPayNotify";

            var weiXinHelper = new Libraries.WeiXin.App.WeiXinHelper(weixinkey.WxAppId, weixinkey.MchId, weixinkey.MchKey, url);

            int price = Convert.ToInt32(order.Price * 100);

            var pay = weiXinHelper.CreatePay(order.OrderNo, "订单号：" + orderno, "订单号：" + orderno, price, "119.29.29.29");

            return pay;

        }



        /// <summary>
        /// 获取微信支付PC版URL
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        [HttpGet("GetWeiXinPayPCUrl")]
        public FileResult GetWeiXinPayPCUrl(string orderNo)
        {

            string key = "wxpayPCUrl" + orderNo;

            string codeUrl = Common.RedisHelper.StringGet(key);

            if (string.IsNullOrEmpty(codeUrl))
            {


                var order = db.TOrder.Where(t => t.OrderNo == orderNo).Select(t => new { t.Id, t.OrderNo, t.Price }).FirstOrDefault();

                var weixinkey = db.TWeiXinKey.Where(t => t.IsDelete == false).FirstOrDefault();

                var url = Libraries.Http.HttpContext.GetBaseUrl() + "/api/Pay/WeiXinPayNotify";

                var weiXinHelper = new Libraries.WeiXin.Web.WeiXinHelper(weixinkey.WxAppId, weixinkey.WxAppSecret, weixinkey.MchId, weixinkey.MchKey, url);

                int price = Convert.ToInt32(order.Price * 100);

                codeUrl = weiXinHelper.CreatePay(order.Id, order.OrderNo, DateTime.Now.ToString("yyyyMMddHHmm") + "交易", price, "119.29.29.29");

                Common.RedisHelper.StringSet(key, codeUrl, TimeSpan.FromMinutes(115));

            }

            var image = Common.ImgHelper.GetQrCode(codeUrl);
            MemoryStream ms = new MemoryStream();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            return File(ms.ToArray(), "image/png");
        }



        /// <summary>
        /// 微信支付异步通知接口
        /// </summary>
        [HttpPost("WeiXinPayNotify")]
        public string WeiXinPayNotify()
        {
            WxPayData notifyData = JsApiPay.GetNotifyData(); //获取微信传过来的参数

            //构造对微信的应答信息
            WxPayData res = new WxPayData();

            if (!notifyData.IsSet("transaction_id"))
            {
                //若transaction_id不存在，则立即返回结果给微信支付后台
                res.SetValue("return_code", "FAIL");
                res.SetValue("return_msg", "支付结果中微信订单号不存在");
                return res.ToXml();
            }

            //获取订单信息
            string transaction_id = notifyData.GetValue("transaction_id").ToString(); //微信流水号
            string order_no = notifyData.GetValue("out_trade_no").ToString().ToUpper(); //商户订单号
            string total_fee = notifyData.GetValue("total_fee").ToString(); //获取总金额

            string appid = notifyData.GetValue("appid").ToString();

            string paytimeStr = notifyData.GetValue("time_end").ToString();
            var payTime = DateTime.ParseExact(paytimeStr, "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture);

            //从微信验证信息真实性
            WxPayData req = new WxPayData();
            req.SetValue("transaction_id", transaction_id);




            var weixinkey = db.TWeiXinKey.Where(t => t.WxAppId == appid).FirstOrDefault();

            JsApiPay jsApiPay = new JsApiPay(weixinkey.WxAppId, weixinkey.WxAppSecret, weixinkey.MchId, weixinkey.MchKey);

            WxPayData send = jsApiPay.OrderQuery(req);
            if (!(send.GetValue("return_code").ToString() == "SUCCESS" && send.GetValue("result_code").ToString() == "SUCCESS"))
            {
                //如果订单信息在微信后台不存在,立即返回失败
                res.SetValue("return_code", "FAIL");
                res.SetValue("return_msg", "订单查询失败");
                return res.ToXml();
            }
            else
            {

                var order = db.TOrder.Where(t => t.OrderNo == order_no).FirstOrDefault();

                if (order == null)
                {
                    res.SetValue("return_code", "FAIL");
                    res.SetValue("return_msg", "订单不存在或已删除");
                    return res.ToXml();
                }

                if (!string.IsNullOrEmpty(order.SerialNo)) //已付款
                {
                    res.SetValue("return_code", "SUCCESS");
                    res.SetValue("return_msg", "OK");
                    return res.ToXml();
                }

                try
                {
                    order.PayPrice = decimal.Parse(total_fee) / 100;
                    order.SerialNo = transaction_id;
                    order.PayState = true;
                    order.PayTime = payTime;
                    order.PayType = "微信支付";
                    order.State = "已支付";

                    db.SaveChanges();

                    if (order.Type == "")
                    {
                        //执行业务处理逻辑
                    }


                    //返回成功通知
                    res.SetValue("return_code", "SUCCESS");
                    res.SetValue("return_msg", "OK");
                    return res.ToXml();
                }
                catch
                {
                    res.SetValue("return_code", "FAIL");
                    res.SetValue("return_msg", "修改订单状态失败");
                    return res.ToXml();
                }

            }

        }




        /// <summary>
        /// 支付宝小程序商户平台下单接口
        /// </summary>
        /// <remarks>用于在支付宝商户平台创建订单</remarks>
        /// <returns></returns>
        [HttpGet("CreateAliPayMiniApp")]
        public dtoKeyValue CreateAliPayMiniApp(string orderno, Guid alipaykeyid)
        {


            var order = db.TOrder.Where(t => t.OrderNo == orderno).Select(t => new
            {
                t.OrderNo,
                t.Price,
                AliPayUserId = db.TUserBindAlipay.Where(a => a.UserId == t.CreateUserId & a.AlipayKeyId == alipaykeyid).Select(a => a.AlipayUserId).FirstOrDefault(),
                t.CreateTime
            }).FirstOrDefault();

            var alipaykey = db.TAlipayKey.Where(t => t.Id == alipaykeyid).FirstOrDefault();

            var url = Libraries.Http.HttpContext.GetBaseUrl() + "/api/Pay/AliPayNotify";

            AliPayHelper aliPayHelper = new AliPayHelper(alipaykey.AppId, alipaykey.AppPrivateKey, alipaykey.AlipayPublicKey, url);

            string price = Convert.ToString(order.Price);

            var TradeNo = aliPayHelper.AlipayTradeCreate(order.OrderNo, order.CreateTime.ToString("yyyyMMddHHmm") + "交易", price, order.AliPayUserId);

            if (string.IsNullOrEmpty(TradeNo))
            {
                HttpContext.Response.StatusCode = 400;

                HttpContext.Items.Add("errMsg", "支付宝交易订单创建失败！");
            }

            var keyValue = new dtoKeyValue
            {
                Key = "TradeNo",
                Value = TradeNo
            };

            return keyValue;

        }



        /// <summary>
        /// 通过订单号获取支付宝电脑网页付款URL
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        [HttpGet("GetAliPayWebUrl")]
        public string GetAliPayWebUrl(string orderNo)
        {

            var info = db.TAlipayKey.Where(t => t.IsDelete == false).FirstOrDefault();

            var order = db.TOrder.Where(t => t.OrderNo == orderNo).Select(t => new
            {
                t.OrderNo,
                t.Price,
                t.State,
                t.CreateTime
            }).FirstOrDefault();

            if (order != null && order.State == "待支付")
            {

                var returnUrl = Libraries.Http.HttpContext.GetBaseUrl();
                var notifyUrl = Libraries.Http.HttpContext.GetBaseUrl() + "/api/Pay/AliPayNotify";

                AliPayHelper helper = new AliPayHelper(info.AppId, info.AppPrivateKey, info.AlipayPublicKey, notifyUrl, returnUrl);

                string price = order.Price.ToString();

                string url = helper.CreatePayPC(order.OrderNo, order.CreateTime.ToString("yyyyMMddHHmm") + "交易", price, order.OrderNo);

                return url;
            }
            else
            {
                return "";
            }
        }



        /// <summary>
        /// 通过订单号获取支付宝H5网页付款URL
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        [HttpGet("GetAliPayH5Url")]
        public string GetAliPayH5Url(string orderNo)
        {

            var info = db.TAlipayKey.Where(t => t.IsDelete == false).FirstOrDefault();

            var order = db.TOrder.Where(t => t.OrderNo == orderNo).Select(t => new { t.OrderNo, t.Price, t.State, t.CreateTime }).FirstOrDefault();

            if (order != null && order.State == "待支付")
            {

                var returnUrl = Libraries.Http.HttpContext.GetBaseUrl();
                var notifyUrl = Libraries.Http.HttpContext.GetBaseUrl() + "/api/Pay/AliPayNotify";

                AliPayHelper helper = new AliPayHelper(info.AppId, info.AppPrivateKey, info.AlipayPublicKey, notifyUrl, returnUrl, "");

                string price = order.Price.ToString();

                string html = helper.CreatePayH5(order.OrderNo, order.CreateTime.ToString("yyyyMMddHHmm") + "交易", price, "");

                return html;
            }
            else
            {
                return "";
            }
        }



        /// <summary>
        /// 支付宝异步通知接口
        /// </summary>
        /// <returns></returns>
        [HttpPost("AliPayNotify")]
        public string AliPayNotify()
        {
            string retValue = "";

            //获取当前请求中的post参数
            var dict = new Dictionary<string, string>();

            var keys = Request.Form.Keys;

            if (keys != null)
            {
                foreach (string key in keys)
                {
                    dict.Add(key, Request.Form[key]);
                }
            }

            if (dict.Count > 0)
            {
                var appid = Request.Form["auth_app_id"].ToString();


                var Alipaypublickey = db.TAlipayKey.Where(t => t.AppId == appid).Select(t => t.AlipayPublicKey).FirstOrDefault();


                bool flag = AlipaySignature.RSACheckV1(dict, Alipaypublickey, "utf-8", "RSA2", false);

                if (flag)
                {
                    var orderno = Request.Form["out_trade_no"].ToString();

                    var order = db.TOrder.Where(t => t.OrderNo == orderno).FirstOrDefault();

                    order.PayPrice = decimal.Parse(Request.Form["total_amount"].ToString());
                    order.SerialNo = Request.Form["trade_no"].ToString();
                    order.PayState = true;
                    order.PayTime = Convert.ToDateTime(Request.Form["gmt_payment"].ToString());
                    order.PayType = "支付宝";
                    order.State = "已支付";

                    db.SaveChanges();

                    switch (order.Type)
                    {
                        case "业务逻辑":
                            {

                                retValue = "success";

                                break;
                            }

                    }

                }

            }

            return retValue;
        }

    }
}