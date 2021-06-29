using Common.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Actions
{
    public class GlobalError
    {
        public static Task ErrorEvent(HttpContext context)
        {
            var feature = context.Features.Get<IExceptionHandlerFeature>();
            var error = feature?.Error;

            var ret = new
            {
                errMsg = "Global internal exception of the system"
            };


            string path = Libraries.Http.HttpContext.GetUrl();

            var parameter = Libraries.Http.HttpContext.GetParameter();

            var parameterStr = JsonHelper.ObjectToJSON(parameter);

            if (parameterStr.Length > 102400)
            {
                parameterStr = parameterStr.Substring(0, 102400);
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

            Common.DBHelper.LogSet("WebApi", "errorlog", strContent);

            context.Response.StatusCode = 400;

            return context.Response.WriteAsync(JsonHelper.ObjectToJSON(ret));
        }
    }
}
