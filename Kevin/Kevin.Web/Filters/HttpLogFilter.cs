using kevin.Domain.Interfaces.IServices;
using kevin.Domain.Share.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Kevin.Web.Filters
{
    public class HttpLogFilter : IResultFilter
    {
        void IResultFilter.OnResultExecuted(ResultExecutedContext context)
        {
            var ad = context.HttpContext.GetEndpoint().Metadata.GetMetadata<ControllerActionDescriptor>();
            var myLog = ad.MethodInfo.CustomAttributes.Where(x => x.AttributeType == typeof(HttpLogAttribute)).ToList().FirstOrDefault();
            string OperateType = "未知";
            string OperateRemark = "未知";
            bool islog = true;
            if (myLog != default)
            {
                if (myLog.ConstructorArguments.Count > 1)
                {
                    OperateType = myLog.ConstructorArguments[0].Value.ToString();
                    OperateRemark = myLog.ConstructorArguments[1].Value.ToString();
                    islog = myLog.ConstructorArguments[2].Value.ToBoolean();
                }
                if (myLog.ConstructorArguments.Count == 1)
                {
                    islog = myLog.ConstructorArguments[0].Value.ToBoolean();
                }
            }
            if (islog)
            {
                var data = context.HttpContext.Request.HttpContext.RequestServices.GetService<IHttpLogService>().Add(OperateType, OperateRemark).Result;
            }
        }

        void IResultFilter.OnResultExecuting(ResultExecutingContext context)
        {
          
        }
    }
}
