using Medallion.Threading;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading;

namespace Web.Filters
{

    /// <summary>
    /// 队列过滤器
    /// </summary>
    public class QueueLimitFilter : Attribute, IActionFilter
    {



        /// <summary>
        /// 是否使用 Token
        /// </summary>
        public bool UseToken { get; set; }



        /// <summary>
        /// 是否阻断重复请求
        /// </summary>
        public bool IsBlock { get; set; }



        void IActionFilter.OnActionExecuting(ActionExecutingContext context)
        {
            string key = context.ActionDescriptor.DisplayName;

            if (UseToken)
            {
                var token = context.HttpContext.Request.Headers.Where(t => t.Key == "Authorization").Select(t => t.Value).FirstOrDefault();

                key = key + "_" + token;
            }

            key = "QueueLimit_" + Common.CryptoHelper.GetMd5(key);

            try
            {

                bool isAction = false;

                while (isAction == false)
                {
                    var lock1 = context.HttpContext.RequestServices.GetService<IDistributedLockProvider>().AcquireLock(key, TimeSpan.FromSeconds(60)); 
                    if (lock1 != null)
                    {
                        isAction = true;
                    }
                    else
                    {
                        if (IsBlock)
                        {
                            isAction = true;
                            context.Result = new BadRequestObjectResult(new { errMsg = "Please do not request frequently" });
                        }
                        else
                        {
                            Thread.Sleep(500);
                        }
                    }
                }
            }
            catch
            {
                Console.WriteLine("队列限制模块异常");
            }
        }



        void IActionFilter.OnActionExecuted(ActionExecutedContext context)
        {
            try
            {

                string key = context.ActionDescriptor.DisplayName;

                if (UseToken)
                {
                    var token = context.HttpContext.Request.Headers.Where(t => t.Key == "Authorization").Select(t => t.Value).FirstOrDefault();

                    key = key + "_" + token;
                } 
                key = "QueueLimit_" + Common.CryptoHelper.GetMd5(key); 
            }
            catch
            {
                Console.WriteLine("队列限制模块异常");
            }

        }


    }

}
