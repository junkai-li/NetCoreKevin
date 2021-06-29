using Aop.Api;
using Aop.Api.Request;
using Aop.Api.Response;
using Aop.Api.Util;
using System;

namespace Common.AliPay.MiniApp
{
    public class AliPayHelper
    {

        /// <summary>
        /// AppId
        /// </summary>
        public string appid;


        /// <summary>
        /// 应用私钥
        /// </summary>
        public string appprivatekey;


        /// <summary>
        /// 支付宝公钥
        /// </summary>
        public string alipaypublickey;



        /// <summary>
        /// 支付宝加解密密钥
        /// </summary>
        public string aeskey;



        /// <param name="in_appid">AppId</param>
        /// <param name="in_appprivatekey">应用私钥</param>
        /// <param name="in_alipaypublickey">支付宝公钥</param>
        /// <param name="in_aeskey">支付宝加解密密钥</param>
        public AliPayHelper(string in_appid, string in_appprivatekey, string in_alipaypublickey, string in_aeskey = null)
        {
            appid = in_appid;
            appprivatekey = in_appprivatekey;
            alipaypublickey = in_alipaypublickey;
            aeskey = in_aeskey;
        }




        /// <summary>
        /// 通过Code获取支付宝UserId
        /// </summary>
        /// <param name="code">前端获取的临时code</param>
        /// <returns></returns>
        public string GetUserId(string code)
        {
            try
            {
                IAopClient client = new DefaultAopClient("https://openapi.alipay.com/gateway.do", appid, appprivatekey, "json", "1.0", "RSA2", alipaypublickey, "utf-8", false);
                AlipaySystemOauthTokenRequest request = new AlipaySystemOauthTokenRequest();
                request.GrantType = "authorization_code";
                request.Code = code;
                AlipaySystemOauthTokenResponse response = client.Execute(request);

                var body = response.Body;

                var userid = Json.JsonHelper.GetValueByKey(Json.JsonHelper.GetValueByKey(body, "alipay_system_oauth_token_response"), "user_id");

                return userid;
            }
            catch
            {
                return null;
            }
        }



        /// <summary>
        /// 支付宝加密数据解密
        /// </summary>
        /// <param name="sign">对response报文的签名</param>
        /// <param name="response">报文（密文）</param>
        public string DecryptionData(string sign, string response)
        {

            string signType = "RSA2";

            string charset = "UTF-8";


            //如果密文的
            bool isDataEncrypted = !response.StartsWith("{", StringComparison.Ordinal);
            bool signCheckPass = false;

            //2. 验签
            string signContent = response;

            //"你的小程序对应的支付宝公钥（为扩展考虑建议用appId+signType做密钥存储隔离）"
            string signVeriKey = alipaypublickey;


            // "你的小程序对应的加解密密钥（为扩展考虑建议用appId+encryptType做密钥存储隔离）"
            string decryptKey = aeskey;


            //如果是加密的报文则需要在密文的前后添加双引号
            if (isDataEncrypted)
            {
                signContent = "\"" + signContent + "\"";
            }
            try
            {
                signCheckPass = AlipaySignature.RSACheckContent(signContent, sign, signVeriKey, charset, signType, false);
            }
            catch (Exception ex)
            {
                //验签异常, 日志
                throw new Exception("验签失败", ex);
            }
            if (!signCheckPass)
            {
                //验签不通过（异常或者报文被篡改），终止流程（不需要做解密）
                throw new Exception("验签失败");
            }

            //3. 解密
            string plainData = null;
            if (isDataEncrypted)
            {
                try
                {
                    plainData = AlipayEncrypt.AesDencrypt(decryptKey, response, charset);
                }
                catch (Exception ex)
                {
                    //解密异常, 记录日志
                    throw new Exception("解密异常", ex);
                }
            }
            else
            {
                plainData = response;
            }

            return plainData;
        }

    }
}
