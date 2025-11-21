using Common.Json;
using Kevin.Common.Extension;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text;
using System.Threading.Tasks;
using Web.Global.Exceptions;

namespace Kevin.Common.App.Global
{
    public class GlobalError
    {
        public static Task ErrorEvent(HttpContext context, string ProjectName = "WebApi")
        {
            var feature = context.Features.Get<IExceptionHandlerFeature>();
            var error = feature?.Error;
            Kevin.log4Net.LogHelper<Exception>.logger.Error(error.Message ?? "", error);
            var ret = new
            {
                code = StatusCodes.Status500InternalServerError,
                IsSuccess = false,
                errMsg = "Global internal exception of the system"
            };
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            if (error is UserFriendlyException)
            {
                ret = new
                {
                    code = StatusCodes.Status400BadRequest,
                    IsSuccess = false,
                    errMsg = error.InnerException == null ? error.Message : error.InnerException.Message
                };
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
            else
            {

                string path = context.RequestServices.GetService<IHttpContextAccessor>().GetUrl();

                var parameter = context.RequestServices.GetService<IHttpContextAccessor>().GetParameter();

                var parameterStr = JsonHelper.ObjectToJSON(parameter);

                if (parameterStr.Length > 102400)
                {
                    parameterStr = parameterStr[0..102400];
                }

                var authorization = context.RequestServices.GetService<IHttpContextAccessor>().Current().Request.Headers["Authorization"].ToString();

                var content = new
                {
                    path,
                    parameter,
                    authorization,
                    error
                }; 
                string strContent = JsonHelper.ObjectToJSON(content);
            }
            context.Response.ContentType = "application/json; charset=utf-8";
            return context.Response.WriteAsync(ret.ToJson(), Encoding.UTF8);
        }
    }
}
