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
        void IActionFilter.OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                var body = "";
                try
                {
                    if (UseBody)
                    {
                        using (Stream requestBody = new MemoryStream())
                        {
                            if (context.HttpContext.Request.Body.Length > 0)
                            {
                                context.HttpContext.Request.Body.CopyTo(requestBody);
                                context.HttpContext.Request.Body.Position = 0;
                                requestBody.Position = 0;
                                using (var requestReader = new StreamReader(requestBody, encoding: Encoding.UTF8))
                                {
                                    body = requestReader.ReadToEnd();
                                }
                            }
                        }
                    } 
                }
                catch
                { 
                } 
                string key = context.ActionDescriptor.DisplayName + "_" + context.HttpContext.Request.QueryString + "_"+ body + "_" 
                        + (UseToken ? context.HttpContext.Request.Headers.Where(t => t.Key == "Authorization").Select(t => t.Value).FirstOrDefault() : "");
                key = "CacheData_" + Common.CryptoHelper.GetMd5(key);

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
                var body = "";
                try
                {
                    if (UseBody)
                    {
                        using (Stream requestBody = new MemoryStream())
                        {
                            if (context.HttpContext.Request.Body.Length > 0)
                            {
                                context.HttpContext.Request.Body.CopyTo(requestBody);
                                context.HttpContext.Request.Body.Position = 0;
                                requestBody.Position = 0;
                                using (var requestReader = new StreamReader(requestBody, encoding: Encoding.UTF8))
                                {
                                    body = requestReader.ReadToEnd();
                                }
                            }
                        }
                    }
                }
                catch
                {
                }
                string key = context.ActionDescriptor.DisplayName + "_" + context.HttpContext.Request.QueryString + "_" + body + "_"
                       + (UseToken ? context.HttpContext.Request.Headers.Where(t => t.Key == "Authorization").Select(t => t.Value).FirstOrDefault() : "");
                key = "CacheData_" + Common.CryptoHelper.GetMd5(key);
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
