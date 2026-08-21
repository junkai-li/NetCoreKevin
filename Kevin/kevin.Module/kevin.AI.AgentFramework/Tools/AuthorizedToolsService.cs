using kevin.AI.AgentFramework.Interfaces.Tools;
using Kevin.Common.App;
using Kevin.Common.Extension;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Web.Global.User;
namespace kevin.AI.AgentFramework.Tools
{
    public class AuthorizedToolsService : IAuthorizedToolsService
    {
        private object? _data { get; set; }
        private int _contentLengthLimit = 0;//  内容长度限制，超过限制后会进行截断
        private List<string> _authorizedDomains = new List<string>(); // 授权域名列表
        private readonly IHttpContextAccessor _httpContextAccessor; 

        public AuthorizedToolsService(IServiceProvider serviceProvider, IHttpContextAccessor httpContextAccessor)
        { 
            _httpContextAccessor = httpContextAccessor; 
        }
        public void InitData(object data)
        {
            _data = data;
            if (_data != default)
            {
                try
                {
                    var jsonDoc = JsonDocument.Parse(JsonSerializer.Serialize(_data));
                    var authorizedDomains = jsonDoc.RootElement.GetProperty("AuthorizedDomains").GetString();
                    if (!string.IsNullOrWhiteSpace(authorizedDomains) && authorizedDomains.Trim() != "*")
                    {
                        authorizedDomains.Split(',')
                            .Select(s => s.Trim())
                            .Where(s => !string.IsNullOrEmpty(s))
                            .ToList()
                            .ForEach(domain => this._authorizedDomains.Add(domain));
                    }
                    jsonDoc.RootElement.GetProperty("ContentLengthLimit").TryGetInt32(out _contentLengthLimit);
                }
                catch (Exception)
                {
                    _contentLengthLimit = 0; // 解析失败则不限制内容长度
                }

            }
        }
        private void AuthorizedDomainsCheck(string url)
        {
            if (_data != default)
            {
                // 将对象转为 JsonElement 或 Dictionary 
                if (_authorizedDomains.Count == 0)
                    return; // 没有有效的前缀，等同于允许所有
                var isAllowed = _authorizedDomains.Any(prefix => url.Contains(prefix, StringComparison.OrdinalIgnoreCase));
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
            #region 获取Authorization
            var Authorization = "";
            if (_httpContextAccessor != default && _httpContextAccessor.Current() != default)
            {
                if (_httpContextAccessor.Current().Request.Headers.ContainsKey("Authorization"))
                {
                    Authorization = _httpContextAccessor.Current().Request.Headers["Authorization"].ToString();
                }
                if (string.IsNullOrEmpty(Authorization) || !JwtToken.IsBearerValidJwt(Authorization))
                {
                    if (_httpContextAccessor.Current().Request.Query.ContainsKey("Authorization"))
                    {
                        Authorization = _httpContextAccessor.Current().Request.Query["Authorization"].ToString();
                    }
                }
            }
            else
            {
                if (_data != default)
                {
                    long UserId = 0;
                    var TenantId = 0;
                    var jsonDoc = JsonDocument.Parse(JsonSerializer.Serialize(_data));
                    if (jsonDoc.RootElement.TryGetProperty("UserId", out var userIdEl))
                    {
                        userIdEl.TryGetInt64(out UserId);
                    }
                    if (jsonDoc.RootElement.TryGetProperty("TenantId", out var tenantEl))
                    {
                        tenantEl.TryGetInt32(out TenantId);
                    }
                    if (UserId > 0 && TenantId > 0)
                    {
                        Authorization = "Bearer ";//后台任务用
                    }
                }
            } 
            #endregion 
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
