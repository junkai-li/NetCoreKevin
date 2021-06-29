using System;
using TencentCloud.Common;
using TencentCloud.Sms.V20190711;
using TencentCloud.Sms.V20190711.Models;

namespace Common.TencentCloud
{
    public class SmsHelper
    {

        /// <summary>
        /// SDK AppId (非账号APPId)
        /// </summary>
        string appId = "";

        /// <summary>
        /// 账号密钥ID
        /// </summary>
        string secretId = "";


        /// <summary>
        /// 账号密钥Key
        /// </summary>
        string secretKey = "";


        public SmsHelper()
        {

        }

        public SmsHelper(string in_appId, string in_secretId, string in_secretKey)
        {
            appId = in_appId;
            secretId = in_secretId;
            secretKey = in_secretKey;
        }


        /// <summary>
        /// 短信发送方法
        /// </summary>
        /// <param name="Phone">手机号</param>
        /// <param name="TemplateCode">模板编号</param>
        /// <param name="SignName">签名</param>
        /// <param name="TemplateParam">模板中包含得变量值，数组形式，多个参数按照顺序传入</param>
        /// <returns></returns>
        public bool SendSms(string Phone, string TemplateCode, string SignName, string[] TemplateParam)
        {
            try
            {

                Credential cred = new Credential
                {
                    SecretId = secretId,
                    SecretKey = secretKey
                };

                SmsClient client = new SmsClient(cred, "ap-guangzhou");

                SendSmsRequest req = new SendSmsRequest();

                req.SmsSdkAppid = appId;

                req.Sign = SignName;

                req.PhoneNumberSet = new String[] { "+86" + Phone };

                req.TemplateID = TemplateCode;

                req.TemplateParamSet = TemplateParam;


                client.SendSms(req);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }

            return true;
        }
    }
}
