using Kevin.Common.Extension;
using Kevin.Common.Helper.DingDing.OA.Dto;
using Kevin.HttpApiClients.Helper;
using System;

namespace Kevin.Common.Helper.DingDing.OA
{
    public class DingDingOAHelper
    {
        static string _appKey = ConfigHelper.Configuration["DingDingOAApiInfo:appKey"];
        static string _appSecret = ConfigHelper.Configuration["DingDingOAApiInfo:appSecret"];
        static string _url = ConfigHelper.Configuration["DingDingOAApiInfo:url"];

        /// <summary>
        /// 获取访问Token
        /// </summary>
        /// <returns></returns>
        public static string GetAccessToken()
        {
            string resultJson = "";
            try
            {
                resultJson = HttpClientHelper.Get(_url + $"/gettoken?appkey={_appKey}&appsecret={_appSecret}");
                return resultJson.GetValueByKeyNoTrr("access_token");
            }
            catch (Exception ex)
            {
                ex.SetFieldValue("Message", resultJson);
                throw ex;
            }
        } 
        /// <summary>
        /// 发起审批
        /// </summary>
        /// <param name="createDto"></param>
        /// <returns></returns>
        public static ProcessinstanceCreateResultDto ProcessinstanceCreate(ProcessinstanceCreateDto createDto)
        {
            string resultJson = "";
            try
            { 
                resultJson = HttpClientHelper.CreatePostHttpResponse(_url + "/topapi/processinstance/create?access_token=" + GetAccessToken(), null, createDto.ToJson());
                var result = resultJson.ToObject< ProcessinstanceCreateResultDto>();
                return result;
            }
            catch (Exception ex)
            {
                ex.SetFieldValue("Message", resultJson);
                throw ex;
            }
        }
        /// <summary>
        /// 获取审批实例详情
        /// </summary>
        /// <param name="createDto"></param>
        /// <returns></returns>
        public static ProcessinstanceGetResultDto ProcessinstanceGet(ProcessinstanceGetDto createDto)
        {
            string resultJson = "";
            try
            {
                resultJson = HttpClientHelper.CreatePostHttpResponse(_url + "/topapi/processinstance/get?access_token=" + GetAccessToken(), createDto.ToJson());
                var result = resultJson.ToObject<ProcessinstanceGetResultDto>();
                return result; 
            }
            catch (Exception ex)
            {
                ex.SetFieldValue("Message", resultJson);
                throw ex;
            }
        }
        /// <summary>
        /// 撤销审批实例详情
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static ProcessinstanceTerminateResultDto ProcessinstanceTerminate(ProcessinstanceTerminateDto data)
        {
            string resultJson = "";
            try
            {
                resultJson = HttpClientHelper.CreatePostHttpResponse(_url + "/topapi/process/instance/terminate?access_token=" + GetAccessToken(), null, data.ToJson());
                var result = resultJson.ToObject<ProcessinstanceTerminateResultDto>();
                return result;
            }
            catch (Exception ex)
            {
                ex.SetFieldValue("Message", resultJson);
                throw ex;
            }
        }
    }
}
