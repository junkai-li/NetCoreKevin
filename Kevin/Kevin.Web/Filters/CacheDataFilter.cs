using Common.Json;
using kevin.Cache.Service;
using Kevin.Common.App.Global;
using Kevin.Common.Extension;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

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
        void IActionFilter.OnActionExecuting(ActionExecutingContext context)
        {
            string key = context.ActionDescriptor.DisplayName + "_" + context.HttpContext.Request.QueryString + "_"
                        + (UseToken ? context.HttpContext.Request.Headers.Where(t => t.Key == "Authorization").Select(t => t.Value).FirstOrDefault() : "");
            key = "CacheData_" + Common.CryptoHelper.GetMd5(key);
            try
            {
                var cacheInfo = context.HttpContext.RequestServices.GetService<ICacheService>().GetString(key);
                if (!string.IsNullOrEmpty(cacheInfo))
                {
                    var data = JsonHelper.GetValueByKeyTry(cacheInfo, "Value");
                    if (string.IsNullOrEmpty(data))
                    {
                        data = JsonHelper.GetValueByKeyTry(cacheInfo, "value");
                    }

                    if (!string.IsNullOrEmpty(data))
                    {
                        context.Result = new ObjectResult(data.ToObject<T>());
                    } 
                }
            }
            catch
            {
                Console.WriteLine("缓存模块异常");
            }
        }


        void IActionFilter.OnActionExecuted(ActionExecutedContext context)
        {
            try
            {
                string key = context.ActionDescriptor.DisplayName + "_" + context.HttpContext.Request.QueryString + "_"
                       + (UseToken ? context.HttpContext.Request.Headers.Where(t => t.Key == "Authorization").Select(t => t.Value).FirstOrDefault() : "");
                key = "CacheData_" + Common.CryptoHelper.GetMd5(key);
                context.HttpContext.RequestServices.GetService<ICacheService>().SetObject(key, context.Result, TimeSpan.FromSeconds(TTL));
            }
            catch
            {
                Console.WriteLine("缓存模块异常");
            }

        }
    }
}
