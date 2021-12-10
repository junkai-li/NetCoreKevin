using Common.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Web.Actions
{
    public class GlobalError
    { 
        public static Task ErrorEvent(HttpContext context,string ProjectName="WebApi")
        {
            var feature = context.Features.Get<IExceptionHandlerFeature>();
            var error = feature?.Error;

            var ret = new
            {
                code = StatusCodes.Status500InternalServerError,
                IsSuccess=false,
                errMsg = "Global internal exception of the system"
            };


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
            context.Response.StatusCode = StatusCodes.Status500InternalServerError; 
            return context.Response.WriteAsJsonAsync(ret);
        }
    }
}
