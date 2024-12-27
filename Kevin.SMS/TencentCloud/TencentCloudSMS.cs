using Kevin.SMS.TencentCloud.Models;
using Microsoft.Extensions.Options;
using TencentCloud.Common;
using TencentCloud.Sms.V20190711;
using TencentCloud.Sms.V20190711.Models;

namespace Kevin.SMS.TencentCloud
{
    public class TencentCloudSMS : ISMS
    {

        /// <summary>
        /// SDK AppId (非账号APPId)
        /// </summary>
        private readonly string appId;


        /// <summary>
        /// 账号密钥ID
        /// </summary>
        private readonly string secretId;


        /// <summary>
        /// 账号密钥Key
        /// </summary>
        private readonly string secretKey;



        public TencentCloudSMS(IOptionsMonitor<SMSSetting> config)
        {
            appId = config.CurrentValue.AppId;
            secretId = config.CurrentValue.SecretId;
            secretKey = config.CurrentValue.SecretKey;
        }


        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="signName"></param>
        /// <param name="phone"></param>
        /// <param name="templateCode"></param>
        /// <param name="templateParams">逗号分割</param>
        /// <returns></returns>
        public bool SendSMS(string signName, string phone, string templateCode, string templateParams)
        {
            try
            {
                var templateParamsArray = templateParams.Split(",").ToArray();

                Credential cred = new()
                {
                    SecretId = secretId,
                    SecretKey = secretKey
                };

                SmsClient client = new(cred, "ap-guangzhou");

                SendSmsRequest req = new()
                {
                    SmsSdkAppid = appId,

                    Sign = signName,

                    PhoneNumberSet = new string[] { "+86" + phone },

                    TemplateID = templateCode,

                    TemplateParamSet = templateParamsArray
                };


                client.SendSms(req);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
