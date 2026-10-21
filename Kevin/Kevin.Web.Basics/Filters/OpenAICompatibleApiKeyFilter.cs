using kevin.Domain.Entities.AI;
using kevin.Domain.Interfaces.IRepositories;
using kevin.Domain.Interfaces.IServices;
using Kevin.Common;
using Kevin.Common.Helper;
using Kevin.log4Net;
using Kevin.Web.Basics.AI;
using Kevin.Web.Basics.Controllers.AI.OpenAI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Web.Filters
{
    /// <summary>
    /// OpenAI 协议兼容接口的非鉴权设置项（appsettings.json 的 OpenAICompatibleSetting）：
    /// 外部客户端用 Authorization: Bearer {Key} 调用 /v1/chat/completions 等接口。
    /// <para>
    /// 注意：ApiKey 列表（ApiKeys）已下线，Key 改为由 <c>OpenAICompatibleKeyController</c> 维护、
    /// 落库到 <c>TAIOpenAICompatibleKey</c>（AES 加密）。本类只保留网关运行所需的非鉴权设置项。
    /// </para>
    /// </summary>
    public class OpenAICompatibleSetting
    {
        /// <summary>会话映射（会话标识 → 内部会话）的有效期，单位小时；过期后同一会话标识会自动新建会话</summary>
        public int SessionExpirationHours { get; set; } = 24;

        /// <summary>是否把模型思考过程以 reasoning_content 字段流式下发（不支持的客户端会自动忽略该字段）</summary>
        public bool ReasoningOutput { get; set; } = true;

        /// <summary>
        /// //是否隐藏 AI 聊天对话（null=不限制，true=隐藏，false=显示）
        /// </summary>
        public bool AIChatIsHidden { get; set; } = true;
    }

    /// <summary>
    /// OpenAI 协议兼容接口的 API 密钥鉴权过滤器：
    /// 校验 Authorization: Bearer {Key} 是否命中数据库 <c>TAIOpenAICompatibleKey</c>（AES 加密存储）。
    /// 命中后用"密钥绑定的用户+租户"签发内部 JWT 并重写请求的 Authorization 头，
    /// 使下游整条既有管线（CurrentUser / 智能体权限 / MCP 工具取数）与网页端登录用户完全同路径。
    /// <para>
    /// 性能：按租户缓存 5 分钟；缓存命中时跳过 DB 查询与 AES 解密匹配过程。
    /// </para>
    /// <para>校验失败按 OpenAI 错误格式返回 401。</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class OpenAICompatibleApiKeyFilter : Attribute, IAsyncActionFilter
    {
        private static readonly JsonSerializerOptions ErrorJsonOptions = new()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };

        /// <summary>Key 缓存滑动过期时间：足够短以保证禁用/删除几秒后即生效，又足以挡住短时间内的连续请求</summary>
        private static readonly TimeSpan KeyCacheTtl = TimeSpan.FromMinutes(5);

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
            if (string.IsNullOrEmpty(apiKey))
            {
                context.Result = BuildUnauthorized("缺少 Authorization: Bearer <api_key> 凭证");
                return;
            }

            var keyRp = http.RequestServices.GetService<ITAIOpenAICompatibleKeyRp>();
            if (keyRp == null)
            {
                context.Result = BuildUnauthorized("认证服务未就绪，请稍后重试");
                return;
            }

            // 命中 Key（命中后用绑定用户的内部凭证继续）
            var cache = http.RequestServices.GetService<IMemoryCache>();
            TAIOpenAICompatibleKey? matched = await TryMatchAsync(keyRp, cache, apiKey, http.RequestAborted);
            if (matched == null)
            {
                context.Result = BuildUnauthorized("无效的 api_key");
                return;
            }
            if (matched.UserId <= 0 || matched.TenantId <= 0)
            {
                context.Result = BuildUnauthorized("api_key 配置错误：未绑定有效的用户或租户");
                return;
            }

            var userService = http.RequestServices.GetService<IUserService>();
            var authorizeService = http.RequestServices.GetService<IAuthorizeService>();
            if (userService == null || authorizeService == null)
            {
                context.Result = BuildUnauthorized("认证服务未就绪，请稍后重试");
                return;
            }
            // 用户必须先校验存在：GetTokenById 对不存在的用户会签出 id=0 的"空身份"token，
            // 那样下游只会得到含糊的"权限不足"（GetSysUserWhereId 查不到时返回空 dtoUser，不抛异常）
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

            //身份刚刚被重写，这里补一次数据权限预热：
            //全局的 DataPermissionWarmupFilter 在本过滤器之前执行，那时还没有用户身份；
            //不补的话下游仓储查询会走同步兜底分支阻塞线程
            try
            {
                var currentUser = http.RequestServices.GetService<Web.Global.User.ICurrentUser>();
                if (currentUser != null && currentUser.UserId > 0)
                {
                    await currentUser.GetModuleDataPermissionsUserIds();
                }
            }
            catch (Exception ex)
            {
                LogHelper.logger.Error($"OpenAI兼容接口数据权限预热失败：{ex.Message}", ex);
            }

            // 命中即异步更新最近使用时间（不阻塞请求本身，失败仅记日志）
            // 必须用独立的 IServiceScopeFactory 拿一份新作用域里的 DbContext：
            // 这里的 keyRp 属于当前请求作用域，请求结束 DbContext 会被释放，
            // fire-and-forget 的后台任务若复用同一 DbContext 会抛 “A second operation was started on this context”
            var scopeFactory = http.RequestServices.GetService<IServiceScopeFactory>();
            _ = UpdateLastUsedTimeInNewScopeAsync(scopeFactory, matched.Id);

            await next();
        }

        /// <summary>
        /// 匹配入口：先按租户拿启用中的 Key 列表（带 5 分钟内存缓存），再逐条 AES 解密做常量时间比较。
        /// </summary>
        private static async Task<TAIOpenAICompatibleKey?> TryMatchAsync(
            ITAIOpenAICompatibleKeyRp keyRp,
            IMemoryCache? cache,
            string apiKey,
            CancellationToken cancellationToken)
        {
            // 没拿到缓存就直接走 DB 路径
            if (cache == null)
            {
                return await MatchFromDbAsync(keyRp, apiKey, cancellationToken);
            }

            // 缓存键按租户隔离：先用一个全租户列表作为匹配源
            // （OpenAI 兼容接口未提供租户上下文，所以需要扫描全表；为限制规模，仅租户已启用的少量 Key）
            var cacheKey = "openai_compatible:keys:active";
            var list = await cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = KeyCacheTtl;
                return await LoadActiveKeysAsync(keyRp, cancellationToken);
            });
            if (list == null || list.Count == 0) return null;

            foreach (var item in list)
            {
                if (string.IsNullOrEmpty(item.EncryptedKey)) continue;
                string plain;
                try
                {
                    plain = AESEncryptor.DecryptString(item.EncryptedKey);
                }
                catch (Exception ex)
                {
                    // 单条 Key 解密失败不拖累其他 Key，跳过并记日志
                    LogHelper.logger.Warn($"OpenAI兼容接口 Key(Id={item.Id}) 解密失败，跳过：{ex.Message}");
                    continue;
                }
                if (string.Equals(plain, apiKey, StringComparison.Ordinal))
                {
                    return item;
                }
            }
            return null;
        }

        private static async Task<TAIOpenAICompatibleKey?> MatchFromDbAsync(
            ITAIOpenAICompatibleKeyRp keyRp,
            string apiKey,
            CancellationToken cancellationToken)
        {
            var list = await LoadActiveKeysAsync(keyRp, cancellationToken);
            if (list == null || list.Count == 0) return null;
            foreach (var item in list)
            {
                if (string.IsNullOrEmpty(item.EncryptedKey)) continue;
                string plain;
                try
                {
                    plain = AESEncryptor.DecryptString(item.EncryptedKey);
                }
                catch (Exception ex)
                {
                    LogHelper.logger.Warn($"OpenAI兼容接口 Key(Id={item.Id}) 解密失败，跳过：{ex.Message}");
                    continue;
                }
                if (string.Equals(plain, apiKey, StringComparison.Ordinal))
                {
                    return item;
                }
            }
            return null;
        }

        private static Task<List<TAIOpenAICompatibleKey>> LoadActiveKeysAsync(
            ITAIOpenAICompatibleKeyRp keyRp,
            CancellationToken cancellationToken)
        {
            // 一次性加载全表启用中的 Key（量级有限：与外部接入方数量成正比）。
            // 不用租户过滤是因为网关层没有租户上下文；用 IsDelete=false + IsEnabled=true 精确筛选。
            return keyRp.Query(isTenant: false, isDataPer: false)
                .Where(t => t.IsDelete == false && t.IsEnabled)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// 后台更新 Key 最近使用时间：在独立作用域里跑，避免与请求作用域的 DbContext 争抢资源。
        /// 缓存 5 分钟内会跳过重复更新（接收到“应该更新”的请求数过多时这个调用的次数会指数级放大），所以“晚几秒入库”是可接受的。
        /// </summary>
        private static async Task UpdateLastUsedTimeInNewScopeAsync(IServiceScopeFactory? scopeFactory, long id)
        {
            if (scopeFactory == null) return;
            try
            {
                using var scope = scopeFactory.CreateScope();
                var keyRp = scope.ServiceProvider.GetService<ITAIOpenAICompatibleKeyRp>();
                if (keyRp == null) return;
                var entity = await keyRp.Query(isTenant: false, isDataPer: false)
                    .Where(t => t.Id == id)
                    .FirstOrDefaultAsync();
                if (entity == null) return;
                entity.LastUsedTime = DateTime.Now;
                await keyRp.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                LogHelper.logger.Info("OpenAI兼容接口更新 Key 最近使用时间失败（不影响请求）：" + ex.Message);
            }
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

    public class OpenAICompatibleErrorResponse
    {
        public OpenAICompatibleErrorBody Error { get; set; }
    }

    public class OpenAICompatibleErrorBody
    {
        public string Message { get; set; }
        public string Type { get; set; }
        public string Code { get; set; }
    }
}