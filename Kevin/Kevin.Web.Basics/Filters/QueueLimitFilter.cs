using Medallion.Threading;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Web.Filters
{

    /// <summary>
    /// 队列过滤器：同一 Action（可选按 Token 区分）同一时刻只放行一个请求，
    /// 排队模式异步等待锁不占用线程池线程；请求结束后 finally 中释放锁
    /// </summary>
    public class QueueLimitFilter : Attribute, IAsyncActionFilter
    {



        /// <summary>
        /// 是否使用 Token
        /// </summary>
        public bool UseToken { get; set; }



        /// <summary>
        /// 是否阻断重复请求
        /// </summary>
        public bool IsBlock { get; set; }



        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            string key = BuildKey(context);

            try
            {
                var lockProvider = context.HttpContext.RequestServices.GetService<IDistributedLockProvider>();
                if (lockProvider == null)
                {
                    await next();
                    return;
                }

                var distributedLock = lockProvider.CreateLock(key);

                if (IsBlock)
                {
                    // 阻断模式：拿不到锁立即返回友好提示
                    var handle = await distributedLock.TryAcquireAsync(TimeSpan.Zero, context.HttpContext.RequestAborted);
                    if (handle == null)
                    {
                        context.Result = new BadRequestObjectResult(new { errMsg = "Please do not request frequently" });
                        return;
                    }
                    try
                    {
                        await next();
                    }
                    finally
                    {
                        handle.Dispose();
                    }
                }
                else
                {
                    // 排队模式：异步等待锁（单次等待 60 秒，超时继续下一轮等待），不阻塞线程池线程
                    while (true)
                    {
                        IDistributedSynchronizationHandle? handle;
                        try
                        {
                            handle = await distributedLock.AcquireAsync(TimeSpan.FromSeconds(60), context.HttpContext.RequestAborted);
                        }
                        catch (TimeoutException)
                        {
                            // 60 秒内未获取到锁，继续等待
                            continue;
                        }
                        try
                        {
                            await next();
                            return;
                        }
                        finally
                        {
                            handle.Dispose();
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // 客户端已断开，无需处理
            }
            catch
            {
                Console.WriteLine("队列限制模块异常");
                await next();
            }
        }



        /// <summary>
        /// 构造锁 Key：Action 名（可选 Token）的 MD5
        /// </summary>
        private string BuildKey(ActionExecutingContext context)
        {
            try
            {
                string key = context.ActionDescriptor.DisplayName;

                if (UseToken)
                {
                    var token = context.HttpContext.Request.Headers.Where(t => t.Key == "Authorization").Select(t => t.Value).FirstOrDefault();

                    key = key + "_" + token;
                }
                return "QueueLimit_" + Common.CryptoHelper.GetMd5(key);
            }
            catch
            {
                Console.WriteLine("队列限制模块异常");
                return "QueueLimit_" + Guid.NewGuid().ToString("N");
            }
        }


    }

}
