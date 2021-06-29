using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Core.Http;
using Aliyun.Acs.Core.Profile;

namespace Common.AliYun
{
    public class SmsHelper
    {
        string accessKeyId = "";
        string accessKeySecret = "";


        public SmsHelper()
        {

        }

        public SmsHelper(string in_accessKeyId, string in_accessKeySecret)
        {
            accessKeyId = in_accessKeyId;
            accessKeySecret = in_accessKeySecret;
        }



        /// <summary>
        /// 短信发送方法
        /// </summary>
        /// <param name="phoneNumbers">手机号,多个手机号以,分隔，最多1000个</param>
        /// <param name="templateCode">模板编号</param>
        /// <param name="signName">签名</param>
        /// <param name="templateParam">模板中包含得变量值，Json 格式</param>
        /// <returns></returns>
        public bool SendSms(string phoneNumbers, string templateCode, string signName, string templateParam)
        {
            IClientProfile profile = DefaultProfile.GetProfile("cn-hangzhou", accessKeyId, accessKeySecret);
            DefaultAcsClient client = new DefaultAcsClient(profile);
            CommonRequest request = new CommonRequest();
            request.Method = MethodType.POST;
            request.Domain = "dysmsapi.aliyuncs.com";
            request.AddQueryParameters("PhoneNumbers", phoneNumbers);
            request.AddQueryParameters("SignName", signName);
            request.AddQueryParameters("TemplateCode", templateCode);
            request.AddQueryParameters("TemplateParam", templateParam);
            request.Version = "2017-05-25";
            request.Action = "SendSms";
            try
            {
                CommonResponse response = client.GetCommonResponse(request);

                string retValue = System.Text.Encoding.Default.GetString(response.HttpResponse.Content);

                return true;
            }
            catch (ServerException e)
            {
                var log = new
                {
                    phoneNumbers,
                    templateCode,
                    signName,
                    templateParam,
                    error = e
                };

                var logStr = Json.JsonHelper.ObjectToJSON(log);

                DBHelper.LogSet("Sms", "error", logStr);

                return false;
            }
            catch (ClientException e)
            {

                var log = new
                {
                    phoneNumbers,
                    templateCode,
                    signName,
                    templateParam,
                    error = e
                };

                var logStr = Json.JsonHelper.ObjectToJSON(log);

                DBHelper.LogSet("Sms", "error", logStr);

                return false;
            }
        }



    }
}
