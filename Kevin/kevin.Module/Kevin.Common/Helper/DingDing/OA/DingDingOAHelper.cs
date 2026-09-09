using Kevin.Common.Extension;
using Kevin.Common.Helper.DingDing.OA.Dto;
using Kevin.HttpApiClients.Helper;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace Kevin.Common.Helper.DingDing.OA
{
    public class DingDingOAHelper
    {
        static string _appKey = ConfigHelper.Configuration["DingDingOAApiInfo:appKey"];
        static string _appSecret = ConfigHelper.Configuration["DingDingOAApiInfo:appSecret"];
        static string _url = ConfigHelper.Configuration["DingDingOAApiInfo:url"];

        /// <summary>
        /// access_token 缓存（钉钉token有效期7200秒，频繁换取会被限流；提前10分钟过期以避开边界失效）
        /// </summary>
        static string _accessToken = "";
        static DateTime _accessTokenExpireAt = DateTime.MinValue;
        static readonly object _tokenLock = new object();

        /// <summary>
        /// 获取访问Token
        /// </summary>
        /// <returns></returns>
        public static string GetAccessToken()
        {
            lock (_tokenLock)
            {
                if (!string.IsNullOrEmpty(_accessToken) && DateTime.Now < _accessTokenExpireAt)
                {
                    return _accessToken;
                }
                string resultJson = "";
                try
                {
                    resultJson = HttpClientHelper.Get(_url + $"/gettoken?appkey={_appKey}&appsecret={_appSecret}");
                    var token = resultJson.GetValueByKeyNoTrr("access_token");
                    if (string.IsNullOrEmpty(token))
                    {
                        throw new Exception($"获取钉钉access_token失败：{resultJson}");
                    }
                    _accessToken = token;
                    _accessTokenExpireAt = DateTime.Now.AddMinutes(110);
                    return _accessToken;
                }
                catch (Exception ex)
                {
                    ex.SetFieldValue("Message", resultJson);
                    throw;
                }
            }
        }
        /// <summary>
        /// 从响应JSON里取指定节点（不是合法JSON或节点不存在时返回null，不把解析失败再抛成业务异常）
        /// </summary>
        /// <param name="json">响应原文</param>
        /// <param name="key">节点名</param>
        /// <returns></returns>
        static JToken ReadJsonNode(string json, string key)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }
            try
            {
                return JObject.Parse(json)[key];
            }
            catch
            {
                return null;
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
                resultJson = HttpClientHelper.CreatePostHttpResponse(_url + "/topapi/processinstance/create?access_token=" + GetAccessToken(), createDto.ToJson());
                var result = resultJson.ToObject<ProcessinstanceCreateResultDto>();
                // 审批实例ID在钉钉返回里有顶层 process_instance_id 与 result.process_instance_id 两种写法（各版本文档示例并不一致）：
                // 只按一种结构建模时，另一种会解析成空ID，表现为钉钉其实已建单成功却被判为失败（本地不落库、钉钉里多出一张对不上的审批单）
                if (result != null && string.IsNullOrWhiteSpace(result.process_instance_id))
                {
                    var nested = ReadJsonNode(resultJson, "result")?["process_instance_id"];
                    if (nested != null)
                    {
                        result.process_instance_id = (string)nested;
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                ex.SetFieldValue("Message", resultJson);
                throw;
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
                // 同上：实例详情可能整体挂在 result 下（result.process_instance），两种结构都兼容，
                // 否则审批状态永远读不到，已批完的授权单会一直停在审核中
                if (result != null && result.process_instance == null)
                {
                    var instance = ReadJsonNode(resultJson, "process_instance")
                        ?? ReadJsonNode(resultJson, "result")?["process_instance"];
                    if (instance != null)
                    {
                        result.process_instance = instance.ToObject<Process_instance>();
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                ex.SetFieldValue("Message", resultJson);
                throw;
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
                resultJson = HttpClientHelper.CreatePostHttpResponse(_url + "/topapi/process/instance/terminate?access_token=" + GetAccessToken(), data.ToJson());
                var result = resultJson.ToObject<ProcessinstanceTerminateResultDto>();
                return result;
            }
            catch (Exception ex)
            {
                ex.SetFieldValue("Message", resultJson);
                throw;
            }
        }
        /// <summary>
        /// 获取钉钉用户所属的第一个部门ID（发起审批实例要求传发起人部门，部门不属于发起人会被钉钉拒绝）；
        /// 查询失败（应用无通讯录权限、用户不存在等）返回null，由调用方决定兜底部门
        /// </summary>
        /// <param name="userid">钉钉userid</param>
        /// <returns></returns>
        public static long? GetUserDeptId(string userid)
        {
            if (string.IsNullOrWhiteSpace(userid))
            {
                return null;
            }
            try
            {
                var resultJson = HttpClientHelper.CreatePostHttpResponse(_url + "/topapi/v2/user/get?access_token=" + GetAccessToken(),
                    new { userid }.ToJson());
                var jo = JObject.Parse(resultJson);
                if (jo["errcode"] == null || (int)jo["errcode"] != 0)
                {
                    return null;
                }
                var deptIds = jo["result"]?["dept_id_list"] as JArray;
                var first = deptIds?.FirstOrDefault();
                return first == null ? null : (long?)first;
            }
            catch
            {
                return null;
            }
        }
    }
}
