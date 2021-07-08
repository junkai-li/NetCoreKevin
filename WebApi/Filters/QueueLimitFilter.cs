using Microsoft.AspNetCore.Mvc.Filters;
using System; 
using System.Linq;
using System.Threading;

namespace WebApi.Filters
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
                    if (Common.RedisHelper.Lock(key, "123456", TimeSpan.FromSeconds(60)))
                    {
                        isAction = true;
                    }
                    else
                    {
                        Thread.Sleep(500);
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

                Common.RedisHelper.UnLock(key, "123456");

            }
            catch
            {
                Console.WriteLine("队列限制模块异常");
            }

        }


    }
}
