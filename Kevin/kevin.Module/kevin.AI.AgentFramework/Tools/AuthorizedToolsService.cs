using kevin.AI.AgentFramework.Interfaces;
using kevin.AI.AgentFramework.Interfaces.Tools;
using Kevin.Common.App;
using Kevin.Common.Extension;
using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
namespace kevin.AI.AgentFramework.Tools
{
    public class AuthorizedToolsService : IAuthorizedToolsService
    {  
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAIShareInfoService _aIShareInfoService;

        public AuthorizedToolsService(IHttpContextAccessor httpContextAccessor, IAIShareInfoService aIShareInfoService)
        { 
            _httpContextAccessor = httpContextAccessor;
            _aIShareInfoService = aIShareInfoService; 
        } 
        private void AuthorizedDomainsCheck(string url)
        { 
            if (_aIShareInfoService.GetData() != default)
            {
                // 将对象转为 JsonElement 或 Dictionary 
                if (_aIShareInfoService.GetData().AuthorizedDomainsList.Count == 0)
                    return; // 没有有效的前缀，等同于允许所有
                var isAllowed = _aIShareInfoService.GetData().AuthorizedDomainsList.Any(prefix => url.Contains(prefix, StringComparison.OrdinalIgnoreCase));
                if (!isAllowed)
                    throw new UnauthorizedAccessException($"❌ URL '{url}' 不在授权域名单中。不能请求，禁止写代码或者http请求！");
            }
        }
        /// <summary>
        /// 获取url授权码
        /// </summary>
        /// <param name="url">传入需要请求的url</param>
        /// <returns></returns>
        [Description("获取授权登录代码：当使用python，http工具发起Http请求时，需要先获取401授权代码， 返回授权码：输出JSON明确指示Token值和放置位置（URL参数或Headers） 失败异常返回以 ❌ 开头的错误信息")]
        public async Task<string> GetUrlAuthorizedCodeAsync([Description("传入完整的请求url如：https://ksiaa.com/api/product/lists"), Required] string url)
        {
            AuthorizedDomainsCheck(url);
            var Authorization = "";
            if (_httpContextAccessor.Current().Request.Headers.ContainsKey("Authorization"))
            {
                  Authorization = _httpContextAccessor.Current().Request.Headers["Authorization"].ToString(); 
            }
            if (string.IsNullOrEmpty(Authorization) || !IsBearerValidJwt(Authorization))
            {
                if (_httpContextAccessor.Current().Request.Query.ContainsKey("Authorization"))
                {
                    Authorization = _httpContextAccessor.Current().Request.Query["Authorization"].ToString();
                }
            }
            if (!string.IsNullOrEmpty(Authorization))
            {
                return new { Headers = new { Authorization = Authorization } }.ToJson();
            }
            return "接口无需授权";
        }

        /// <summary>
        /// 简单验证JWT的有效性 ，适用于从Authorization头提取的Token
        /// </summary>
        /// <param name="authorizationHeader"></param>
        /// <param name="secretKey"></param>
        /// <param name="validIssuer"></param>
        /// <param name="validAudience"></param>
        /// <returns></returns>
        public static bool IsBearerValidJwt(string authorizationHeader)
        {
            // 1. 检查 Authorization 头格式
            if (string.IsNullOrEmpty(authorizationHeader) ||
                !authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                return false;

            string token = authorizationHeader.Substring("Bearer ".Length).Trim();
            if (string.IsNullOrEmpty(token))
                return false;
            return true;
        }
    }
}
