
using Kevin.Common.Kevin; 
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Web.Filters
{
    /// <summary>
    /// 对外接口 密钥验证
    /// </summary>
    public class PublicPortAuthorizeFilters
    {
        public string AppId { get; }

        public string AppSecret { get; }

        public PublicPortAuthorizeFilters(string appId, string appSecret)
        {
            AppId = appId;
            AppSecret = appSecret;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {

        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var appId = KevinHttpContext.Current(context.HttpContext.RequestServices.GetService<IHttpContextAccessor>()).Request.Headers["appId"].ToString();
            var appSecret = KevinHttpContext.Current(context.HttpContext.RequestServices.GetService<IHttpContextAccessor>()).Request.Headers["appSecret"].ToString();
            if (appId != this.AppId || this.AppSecret != appSecret)
            {
                context.HttpContext.Response.StatusCode = 401;

                context.Result = new JsonResult(new { errMsg = "非法请求！" });
            }
        }
    }
}
