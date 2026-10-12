using kevin.Domain.Interfaces.IServices;
using Kevin.Common.Helper;
using Kevin.log4Net;
using Kevin.Web.Basics.AI;
using Kevin.Web.Basics.Controllers.AI.OpenAI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Web.Filters
{
    /// <summary>
    /// OpenAI 协议兼容接口的配置节（appsettings.json 的 OpenAICompatibleSetting）：
    /// 外部客户端用 Authorization: Bearer {Key} 调用 /v1/chat/completions 等接口。
    /// </summary>
    public class OpenAICompatibleSetting
    {
        /// <summary>API 密钥列表：每个密钥绑定平台内的一个用户+租户，请求内自动换成该用户的内部凭证</summary>
        public List<OpenAICompatibleApiKey> ApiKeys { get; set; } = new();

        /// <summary>会话映射（会话标识 → 内部会话）的有效期，单位小时；过期后同一会话标识会自动新建会话</summary>
        public int SessionExpirationHours { get; set; } = 24;

        /// <summary>是否把模型思考过程以 reasoning_content 字段流式下发（不支持的客户端会自动忽略该字段）</summary>
        public bool ReasoningOutput { get; set; } = true;
    }

    /// <summary>单个 API 密钥配置</summary>
    public class OpenAICompatibleApiKey
    {
        /// <summary>密钥原文（建议 sk- 开头，仅配置在服务端）</summary>
        public string Key { get; set; } = "";

        /// <summary>备注名（仅用于识别）</summary>
        public string? Name { get; set; }

        /// <summary>密钥绑定的平台用户 Id：调用时的权限（可用智能体等）与落库身份都按该用户计算</summary>
        public long UserId { get; set; }

        /// <summary>密钥绑定的租户 code（如 1000）</summary>
        public long TenantId { get; set; }
    }

    /// <summary>
    /// OpenAI 协议兼容接口的 API 密钥鉴权过滤器：
    /// 校验 Authorization: Bearer {Key} 是否命中配置（OpenAICompatibleSetting.ApiKeys），
    /// 命中后用“密钥绑定的用户+租户”签发内部 JWT 并重写请求的 Authorization 头，
    /// 使下游整条既有管线（CurrentUser / 智能体权限 / MCP 工具取数）与网页端登录用户完全同路径。
    /// <para>校验失败按 OpenAI 错误格式返回 401。</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class OpenAICompatibleApiKeyFilter : Attribute, IAsyncActionFilter
    {
        private static readonly JsonSerializerOptions ErrorJsonOptions = new()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var http = context.HttpContext;
            var authorization = http.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrWhiteSpace(authorization) || !authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                context.Result = BuildUnauthorized("缺少 Authorization: Bearer <api_key> 凭证");
                return;
            }

            var apiKey = authorization.Substring("Bearer ".Length).Trim();
            var setting = ConfigHelper.GetSection<OpenAICompatibleSetting>("OpenAICompatibleSetting");
            var matched = setting?.ApiKeys?.FirstOrDefault(k => !string.IsNullOrEmpty(k.Key) && string.Equals(k.Key, apiKey, StringComparison.Ordinal));
            if (matched == null)
            {
                context.Result = BuildUnauthorized("无效的 api_key");
                return;
            }
            if (matched.UserId <= 0 || matched.TenantId <= 0)
            {
                var keyName = string.IsNullOrEmpty(matched.Name) ? "" : $"[{matched.Name}]";
                context.Result = BuildUnauthorized($"api_key{keyName} 配置错误：未绑定有效的用户或租户");
                return;
            }

            var userService = http.RequestServices.GetService<IUserService>();
            var authorizeService = http.RequestServices.GetService<IAuthorizeService>();
            if (userService == null || authorizeService == null)
            {
                context.Result = BuildUnauthorized("认证服务未就绪，请稍后重试");
                return;
            }
            // 用户必须先校验存在：GetTokenById 对不存在的用户会签出 id=0 的“空身份”token，
            // 那样下游只会得到含糊的“权限不足”（GetSysUserWhereId 查不到时返回空 dtoUser，不抛异常）
            var user = userService.GetSysUserWhereId(matched.UserId);
            if (user == null || user.Id <= 0)
            {
                context.Result = BuildUnauthorized("api_key 绑定的用户不存在或已删除");
                return;
            }

            string token;
            try
            {
                token = await authorizeService.GetTokenById(matched.UserId, matched.TenantId);
            }
            catch (Exception ex)
            {
                LogHelper.logger.Error($"OpenAI兼容接口签发内部凭证失败，UserId={matched.UserId}，TenantId={matched.TenantId}：{ex.Message}", ex);
                context.Result = BuildUnauthorized("api_key 绑定的租户/用户校验失败：" + ex.Message);
                return;
            }

            // 重写 Authorization 头：CurrentUser（JwtToken.GetClaims）与 MCP 授权都直接读请求头，
            // 换成内部 JWT 后整条管线按该用户身份运作——这是本过滤器存在的全部意义
            http.Request.Headers["Authorization"] = "Bearer " + token;

            await next();
        }

        private static IActionResult BuildUnauthorized(string message)
        {
            var payload = JsonSerializer.Serialize(new OpenAICompatibleErrorResponse
            {
                Error = new OpenAICompatibleErrorBody
                {
                    Message = message,
                    Type = "invalid_request_error",
                    Code = "invalid_api_key"
                }
            }, ErrorJsonOptions);
            return new ContentResult
            {
                StatusCode = StatusCodes.Status401Unauthorized,
                ContentType = "application/json; charset=utf-8",
                Content = payload
            };
        }
    }
}
