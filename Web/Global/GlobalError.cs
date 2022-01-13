using Common.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
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
                    code = StatusCodes.Status200OK,
                    IsSuccess = false,
                    errMsg = error.InnerException == null ? error.Message : error.InnerException.Message
                };
                context.Response.StatusCode = StatusCodes.Status200OK;
            }
            else
            {

                string path = Libraries.Http.HttpContext.GetUrl();

                var parameter = Libraries.Http.HttpContext.GetParameter();

                var parameterStr = JsonHelper.ObjectToJSON(parameter);

                if (parameterStr.Length > 102400)
                {
                    parameterStr = parameterStr[0..102400];
                }

                var authorization = Libraries.Http.HttpContext.Current().Request.Headers["Authorization"].ToString();

                var content = new
                {
                    path = path,
                    parameter = parameter,
                    authorization = authorization,
                    error = error
                };

                string strContent = JsonHelper.ObjectToJSON(content);
                Common.DBHelper.LogSet(ProjectName, "errorlog", strContent);
            }
            return context.Response.WriteAsJsonAsync(ret);
        }
    }
}
