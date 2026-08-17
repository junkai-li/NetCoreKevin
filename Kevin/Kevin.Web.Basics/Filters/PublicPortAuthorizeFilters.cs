
using Kevin.Common.App;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Cryptography;
using System.Text;

namespace Web.Filters
{
    /// <summary>
    /// 对外接口 密钥验证
    /// </summary>
    public class PublicPortAuthorizeFilters : Attribute, IActionFilter
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
            // 使用时序安全比较，防止时序攻击
            bool appIdValid = FixedTimeEquals(appId, this.AppId);
            bool appSecretValid = FixedTimeEquals(appSecret, this.AppSecret);
            if (!appIdValid || !appSecretValid)
            {
                context.HttpContext.Response.StatusCode = 401;

                context.Result = new JsonResult(new { errMsg = "非法请求！" });
            }
        }

        /// <summary>
        /// 时序安全的字符串比较，防止时序攻击
        /// </summary>
        private static bool FixedTimeEquals(string? value1, string? value2)
        {
            if (value1 is null || value2 is null) return false;
            var bytes1 = Encoding.UTF8.GetBytes(value1);
            var bytes2 = Encoding.UTF8.GetBytes(value2);
            return CryptographicOperations.FixedTimeEquals(bytes1, bytes2);
        }
    }
}
