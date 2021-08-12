
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApiService.Filters
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
            var appId = Libraries.Http.HttpContext.Current().Request.Headers["appId"].ToString();
            var appSecret = Libraries.Http.HttpContext.Current().Request.Headers["appSecret"].ToString();
            if (appId != this.AppId || this.AppSecret != appSecret)
            {
                context.HttpContext.Response.StatusCode = 401;

                context.Result = new JsonResult(new { errMsg = "非法请求！" });
            }
        }
    }
}
