using Common.Json;
using Kevin.Common.Helper;
using Kevin.Common.Kevin;
using Kevin.log4Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Web.Global.Exceptions;

namespace Web.Actions
{
    public class GlobalError
    {
        public static Task ErrorEvent(HttpContext context, string ProjectName = "WebApi")
        {
            var feature = context.Features.Get<IExceptionHandlerFeature>();
            var error = feature?.Error;
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

                string path =KevinHttpContext.GetUrl(context.RequestServices.GetService<IHttpContextAccessor>());

                var parameter = KevinHttpContext.GetParameter(context.RequestServices.GetService<IHttpContextAccessor>());

                var parameterStr = JsonHelper.ObjectToJSON(parameter);

                if (parameterStr.Length > 102400)
                {
                    parameterStr = parameterStr[0..102400];
                }

                var authorization = KevinHttpContext.Current(context.RequestServices.GetService<IHttpContextAccessor>()).Request.Headers["Authorization"].ToString();

                var content = new
                {
                    path = path,
                    parameter = parameter,
                    authorization = authorization,
                    error = error
                };

                string strContent = JsonHelper.ObjectToJSON(content);
                LogHelper.logger.Error(ProjectName+strContent);
            }
            return context.Response.WriteAsJsonAsync(ret);
        }
    }
}
