using Common.Json;
using kevin.Cache.Service;
using Kevin.Common.App;
using Kevin.Common.Extension;
using Kevin.log4Net;
using Kevin.Models.JwtBearer;
using Medallion.Threading;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

namespace Web.Filters
{

    /// <summary>
    /// 缓存过滤器：Key 按「租户 + (可选)用户」隔离，未命中时用分布式锁防止缓存击穿
    /// </summary>
    public class CacheDataFilter<T> : Attribute, IAsyncActionFilter
    {
        /// <summary>
        /// 缓存时效有效期，单位 秒
        /// </summary>
        public int TTL { get; set; }
        /// <summary>
        /// 是否按用户维度缓存（开启后 Key 追加当前登录用户 Id，不开启则同租户内共享）
        /// </summary>
        public bool UseToken { get; set; }
        /// <summary>
        /// 是否使用 Body
        /// </summary>
        public bool UseBody { get; set; } = true;
        /// <summary>
        /// 缓存未命中时等待其他请求回填缓存的最长时间（毫秒）；小于等于 0 表示不等待（关闭防击穿）
        /// </summary>
        public int LockWaitMilliseconds { get; set; } = 2000;

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var cacheService = context.HttpContext.RequestServices.GetService<ICacheService>();
            string? key = null;
            IDistributedSynchronizationHandle? lockHandle = null;

            try
            {
                try
                {
                    key = BuildCacheKey(context);
                }
                catch (Exception ex)
                {
                    LogHelper<CacheDataFilter<T>>.logger.Error($"缓存模块异常, Exception detail: {ex.ToJson()}");
                    key = null;
                }

                if (!string.IsNullOrEmpty(key))
                {
                    //第一次读取：命中直接返回，不进入 Action
                    if (TryReadCache(cacheService, key, context))
                    {
                        return;
                    }

                    //未命中：抢锁后再查一次，只有拿到锁的请求去查库，其余请求等它回填后直接命中
                    lockHandle = await TryAcquireLockAsync(context, key);
                    if (lockHandle != null && TryReadCache(cacheService, key, context))
                    {
                        return;
                    }
                }

                var executedContext = await next();

                if (!string.IsNullOrEmpty(key) && executedContext.Exception == null)
                {
                    try
                    {
                        var cacheInfo = cacheService?.GetString(key);
                        if (string.IsNullOrWhiteSpace(cacheInfo))
                        {
                            cacheService?.SetString(key, executedContext.Result.ToJson(), TimeSpan.FromSeconds(TTL));
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper<CacheDataFilter<T>>.logger.Error($"缓存模块异常, Exception detail: {ex.ToJson()}");
                        Console.WriteLine("缓存模块异常");
                    }
                }
            }
            finally
            {
                if (lockHandle != null)
                {
                    await lockHandle.DisposeAsync();
                }
            }
        }

        /// <summary>
        /// 读取缓存，命中时直接短路 Action
        /// </summary>
        private static bool TryReadCache(ICacheService? cacheService, string key, ActionExecutingContext context)
        {
            try
            {
                var cacheInfo = cacheService?.GetString(key);
                if (string.IsNullOrEmpty(cacheInfo))
                {
                    return false;
                }

                var data = JsonHelper.GetValueByKeyTry(cacheInfo, "value");
                if (string.IsNullOrEmpty(data))
                {
                    return false;
                }

                context.Result = new ObjectResult(data.ToObject<T>());
                return true;
            }
            catch (Exception ex)
            {
                LogHelper<CacheDataFilter<T>>.logger.Error($"缓存模块异常, Exception detail: {ex.ToJson()}");
                return false;
            }
        }

        /// <summary>
        /// 尝试获取分布式锁；未配置 Redis 或等待超时时返回 null，降级为不加锁
        /// </summary>
        private async Task<IDistributedSynchronizationHandle?> TryAcquireLockAsync(ActionContext context, string key)
        {
            if (LockWaitMilliseconds <= 0)
            {
                return null;
            }

            var lockProvider = context.HttpContext.RequestServices.GetService<IDistributedLockProvider>();
            if (lockProvider == null)
            {
                return null;
            }

            try
            {
                var distributedLock = lockProvider.CreateLock("CacheData_Lock_" + key);
                return await distributedLock.TryAcquireAsync(TimeSpan.FromMilliseconds(LockWaitMilliseconds), context.HttpContext.RequestAborted);
            }
            catch (Exception ex)
            {
                LogHelper<CacheDataFilter<T>>.logger.Error($"缓存锁获取异常, Exception detail: {ex.ToJson()}");
                return null;
            }
        }

        /// <summary>
        /// 生成缓存 Key：{租户Id}_{用户Id}_{Action}_{QueryString}_{Body}
        /// 1.租户 Id 始终参与计算，保证同接口不同租户之间不串数据；
        /// 2.UseToken 为 true 时追加当前登录用户 Id，保证同租户不同用户之间不串数据；
        /// 3.取 Claim 而不是 Authorization 原文，Token 续签/刷新后 Key 不变，缓存仍能命中；
        /// 4.取不到身份信息时返回 null，调用方跳过缓存，避免把个人数据写给其他请求。
        /// </summary>
        private string? BuildCacheKey(ActionContext context)
        {
            var httpContext = context.HttpContext;

            // 复用项目统一的 Token 解析逻辑（Header 优先，其次 Query），避免依赖 Header 名称大小写
            var accessor = new HttpContextAccessor { HttpContext = httpContext };

            var tenantId = JwtToken.GetClaims(JwtKeinClaimTypes.TenantId, accessor);
            if (string.IsNullOrEmpty(tenantId))
            {
                //取不到租户时不缓存：此时数据查询本身没有租户过滤，缓存下来会污染其他租户
                return null;
            }

            var userPart = "";
            if (UseToken)
            {
                userPart = JwtToken.GetClaims(JwtKeinClaimTypes.UserId, accessor);
                if (string.IsNullOrEmpty(userPart))
                {
                    return null;
                }
            }

            var body = UseBody ? ReadRequestBody(httpContext) : "";
            var rawKey = $"{tenantId}_{userPart}_{context.ActionDescriptor.DisplayName}_{httpContext.Request.QueryString}_{body}";
            return "CacheData_" + Common.CryptoHelper.GetMd5(rawKey);
        }

        /// <summary>
        /// 读取请求 Body（依赖 Program 中开启的 EnableBuffering）
        /// </summary>
        private static string ReadRequestBody(HttpContext httpContext)
        {
            try
            {
                if (httpContext.Request.Body.Length <= 0)
                {
                    return "";
                }

                using (Stream requestBody = new MemoryStream())
                {
                    httpContext.Request.Body.CopyTo(requestBody);
                    httpContext.Request.Body.Position = 0;
                    requestBody.Position = 0;
                    using (var requestReader = new StreamReader(requestBody, encoding: Encoding.UTF8))
                    {
                        return requestReader.ReadToEnd();
                    }
                }
            }
            catch
            {
                return "";
            }
        }
    }
}
