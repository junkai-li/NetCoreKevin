using Common;
using Common.IO;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Repository.Database;
using System;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Web.Libraries.Http;

namespace Web.Filters
{
    /// <summary>
    /// 身份令牌过滤器
    /// </summary>
    public class TokenVerifyFilter : Attribute, IActionFilter
    {

        void IActionFilter.OnActionExecuting(ActionExecutingContext context)
        {
            var ctrl = context.Controller as ControllerBase;
            if (ctrl == null)
            {
                return;
            }

            var ad = context.ActionDescriptor as Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor;
            var isPublic = ad.MethodInfo.IsDefined(typeof(AllowAnonymousAttribute), false) || ad.ControllerTypeInfo.IsDefined(typeof(AllowAnonymousAttribute), false);
            if (isPublic)
            {
                return;
            }
            var exp = Convert.ToInt64(Libraries.Verify.JwtToken.GetClaims("exp"));
            var exptime = Common.DateTimeHelper.UnixToTime(exp);
            if (exptime < DateTime.Now)
            {
                context.HttpContext.Response.StatusCode = 401;
                context.Result = new JsonResult(new { errMsg = "非法 Token ！" });
            }

        }


        void IActionFilter.OnActionExecuted(ActionExecutedContext context)
        {
            var ctrl = context.Controller as ControllerBase;
            if (ctrl == null)
            {
                return;
            }
            var ad = context.ActionDescriptor as Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor;
            var isPublic = ad.MethodInfo.IsDefined(typeof(AllowAnonymousAttribute), false) || ad.ControllerTypeInfo.IsDefined(typeof(AllowAnonymousAttribute), false);
            if (isPublic)
            {
                return;
            }

            var exp = Convert.ToInt64(Libraries.Verify.JwtToken.GetClaims("exp"));
            var exptime = Common.DateTimeHelper.UnixToTime(exp);
            if (exptime > DateTime.Now && exptime < DateTime.Now.AddMinutes(20))
            {
                try
                {
                    //获取刷新令牌
                    var RefreshToken = RedisHelper.StringGet(HttpContext.Current().Request.Headers["Authorization"].ToString().Replace("Bearer ", ""));
                    var Result = RenewTokenAsync(RefreshToken);
                    if (!string.IsNullOrEmpty(Result.Result))
                    {
                        context.HttpContext.Response.Headers.Add("NewToken", Result.Result);
                    }
                }
                catch (Exception)
                {
                }
            }


        }

        /// <summary>
        /// 刷新token
        /// </summary>
        /// <returns></returns>
        private async Task<string> RenewTokenAsync(string token)
        {
            var clinet = new HttpClient();
            var disco = await clinet.GetDiscoveryDocumentAsync(Config.Get()["IdentityServerUrl"]);
            if (!disco.IsError)
            {
                var refreshToken = await clinet.RequestRefreshTokenAsync(new RefreshTokenRequest
                {
                    ClientId = "YfcUserClient",
                    Address = disco.TokenEndpoint,
                    ClientSecret = "YfcUserClientSecrets",
                    Scope = "YfcUserApi YfcMeetingApi YfcMarketingApi offline_access profile openid",
                    GrantType = OpenIdConnectGrantTypes.RefreshToken,
                    RefreshToken = token
                });
                if (!refreshToken.IsError)
                {
                    //保存刷新令牌
                    RedisHelper.StringSet(refreshToken.AccessToken, refreshToken.RefreshToken, TimeSpan.FromDays(2));
                    return refreshToken.AccessToken;
                }
            }

            return "";
        }
    }
}
