using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Http;
using Aliyun.Acs.Core.Profile;
using Kevin.SMS.AliCloud.Models;
using Microsoft.Extensions.Options; 
namespace Kevin.SMS.AliCloud
{
    public class AliCloudSMS : ISMS
    {

        private readonly string accessKeyId;


        private readonly string accessKeySecret;



        public AliCloudSMS(IOptionsMonitor<SMSSetting> config)
        {
            accessKeyId = config.CurrentValue.AccessKeyId;
            accessKeySecret = config.CurrentValue.AccessKeySecret;
        }




        public bool SendSMS(string signName, string phone, string templateCode, string templateParams)
        {
            try
            {  
                IClientProfile profile = DefaultProfile.GetProfile("cn-hangzhou", accessKeyId, accessKeySecret);
                DefaultAcsClient client = new(profile);
                CommonRequest request = new()
                {
                    Method = MethodType.POST,
                    Domain = "dysmsapi.aliyuncs.com"
                };
                request.AddQueryParameters("PhoneNumbers", phone);
                request.AddQueryParameters("SignName", signName);
                request.AddQueryParameters("TemplateCode", templateCode);
                request.AddQueryParameters("TemplateParam", templateParams);
                request.Version = "2017-05-25";
                request.Action = "SendSms";

                CommonResponse response = client.GetCommonResponse(request);

                string retValue = System.Text.Encoding.Default.GetString(response.HttpResponse.Content);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
