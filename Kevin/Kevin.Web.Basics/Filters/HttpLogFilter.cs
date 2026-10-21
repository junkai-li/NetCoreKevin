using kevin.Domain.Interfaces.IServices;
using kevin.Domain.Share.Attributes;
using Kevin.log4Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Kevin.Web.Filters
{
    /// <summary>
    /// 请求日志过滤器（异步写入，不阻塞线程池线程）
    /// </summary>
    public class HttpLogFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //先执行 Action，再在请求结束前写日志（与原来的 OnActionExecuted 时机一致）
            var executedContext = await next();

            try
            {
                var ad = executedContext.HttpContext.GetEndpoint()?.Metadata.GetMetadata<ControllerActionDescriptor>();
                var myLog = ad?.MethodInfo.CustomAttributes.Where(x => x.AttributeType == typeof(HttpLogAttribute)).ToList().FirstOrDefault();
                string OperateType = "未知";
                string OperateRemark = "未知";
                bool islog = true;
                if (myLog != default)
                {
                    if (myLog.ConstructorArguments.Count > 1)
                    {
                        OperateType = myLog.ConstructorArguments[0].Value?.ToString() ?? "未知";
                        OperateRemark = myLog.ConstructorArguments[1].Value?.ToString() ?? "未知";
                        islog = myLog.ConstructorArguments[2].Value.ToBoolean();
                    }
                    if (myLog.ConstructorArguments.Count == 1)
                    {
                        islog = myLog.ConstructorArguments[0].Value.ToBoolean();
                    }
                }
                if (islog)
                {
                    var httpLogService = executedContext.HttpContext.RequestServices.GetService<IHttpLogService>();
                    if (httpLogService != null)
                    {
                        await httpLogService.Add(OperateType, OperateRemark);
                    }
                }
            }
            catch (Exception ex)
            {
                //日志写入失败不能影响已经成功的业务请求
                LogHelper<HttpLogFilter>.logger.Error($"请求日志写入失败: {ex.Message}", ex);
            }
        }
    }
}
