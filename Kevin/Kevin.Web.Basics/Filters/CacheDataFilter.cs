using Common.Json;
using kevin.Cache.Service;
using Kevin.Common.Extension;
using Kevin.log4Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using TencentCloud.Omics.V20221128.Models;

namespace Web.Filters
{

    /// <summary>
    /// 缓存过滤器
    /// </summary>
    public class CacheDataFilter<T> : Attribute, IActionFilter
    {
        /// <summary>
        /// 缓存时效有效期，单位 秒
        /// </summary>
        public int TTL { get; set; }
        /// <summary>
        /// 是否使用 Token
        /// </summary>
        public bool UseToken { get; set; }
        /// <summary>
        /// 是否使用 Body
        /// </summary>
        public bool UseBody { get; set; } = true;

        /// <summary>
        /// 存储在 HttpContext.Items 中的缓存键名称
        /// </summary>
        private const string CacheKeyItemName = "__CacheDataFilter_Key";

        /// <summary>
        /// 计算缓存键并存储到 HttpContext.Items 中，避免重复计算
        /// </summary>
        private string ComputeCacheKey(ActionContext context)
        {
            var httpContext = context.HttpContext;
            // 如果已经计算过，直接返回
            if (httpContext.Items.TryGetValue(CacheKeyItemName, out var existingKey) && existingKey is string cachedKey)
            {
                return cachedKey;
            }

            var body = "";
            try
            {
                if (UseBody && httpContext.Request.Body.CanSeek)
                {
                    httpContext.Request.Body.Position = 0;
                    using (var requestReader = new StreamReader(httpContext.Request.Body, encoding: Encoding.UTF8, leaveOpen: true))
                    {
                        body = requestReader.ReadToEnd();
                    }
                    httpContext.Request.Body.Position = 0;
                }
            }
            catch
            {
            }

            string key = context.ActionDescriptor.DisplayName + "_" + httpContext.Request.QueryString + "_" + body + "_"
                    + (UseToken ? httpContext.Request.Headers.Where(t => t.Key == "Authorization").Select(t => t.Value).FirstOrDefault() : "");
            key = "CacheData_" + Common.CryptoHelper.GetMd5(key);

            // 存储到 HttpContext.Items 中供后续使用
            httpContext.Items[CacheKeyItemName] = key;
            return key;
        }

        void IActionFilter.OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                string key = ComputeCacheKey(context);

                var cacheInfo = context.HttpContext.RequestServices.GetService<ICacheService>()?.GetString(key);
                if (!string.IsNullOrEmpty(cacheInfo))
                {
                    var data = JsonHelper.GetValueByKeyTry(cacheInfo, "value");
                    if (!string.IsNullOrEmpty(data))
                    {
                        context.Result = new ObjectResult(data.ToObject<T>());
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper<CacheDataFilter<T>>.logger.Error($"缓存模块异常, Exception detail: {ex.ToJson()}");
                Console.WriteLine("缓存模块异常");
            }
        }


        void IActionFilter.OnActionExecuted(ActionExecutedContext context)
        {
            try
            {
                string key = ComputeCacheKey(context);
                var data = context.HttpContext.RequestServices.GetService<ICacheService>()?.GetString(key);
                if (string.IsNullOrWhiteSpace(data))
                {
                    context.HttpContext.RequestServices.GetService<ICacheService>()?.SetString(key, context.Result.ToJson(), TimeSpan.FromSeconds(TTL));
                }
            }
            catch (Exception ex)
            {
                LogHelper<CacheDataFilter<T>>.logger.Error($"缓存模块异常, Exception detail: {ex.ToJson()}");
                Console.WriteLine("缓存模块异常");
            }

        }
    }
}
