using Common.IO;
using Duende.IdentityModel.Client;
using kevin.Cache.Service;
using kevin.Permission.Interfaces;
using kevin.Permission.Permission.Attributes;
using kevin.Permission.Permisson.Attributes;
using Kevin.Common.App;
using Kevin.Common.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace kevin.Permission.Permission.Action
{
    public class IdentityVerification
    {
        /// <summary>
        /// 权限校验
        /// </summary>
        /// <param name="authorizationHandlerContext"></param>
        /// <returns></returns>
        public static bool Authorization(AuthorizationHandlerContext authorizationHandlerContext)
        {

            if (authorizationHandlerContext.User.Identity != default && authorizationHandlerContext.User.Identity.IsAuthenticated)
            {
                if (authorizationHandlerContext.Resource is HttpContext httpContext)
                {
                    IssueNewToken(httpContext);
                    var ad = httpContext.Features.Get<IEndpointFeature>()?.Endpoint?.Metadata?.FirstOrDefault(u => u is ControllerActionDescriptor) as ControllerActionDescriptor;
                    var isSkip = ad.MethodInfo.IsDefined(typeof(SkipAuthorityAttribute), false) || ad.ControllerTypeInfo.IsDefined(typeof(SkipAuthorityAttribute), false);
                    if (isSkip == true)
                    {
                        return true;
                    }
                    var area = "";
                    try
                    {
                        var myarea = ad.ControllerTypeInfo.CustomAttributes.Where(x => x.AttributeType == typeof(MyAreaAttribute)).ToList();
                        if (myarea.Count > 0)
                        {
                            area = ad.ControllerTypeInfo.CustomAttributes.Where(x => x.AttributeType == typeof(MyAreaAttribute)).ToList().LastOrDefault()?.ConstructorArguments[1].Value?.ToString();
                        }
                        else
                        {
                            area = ad.ControllerName;
                        }
                    }
                    catch (Exception)
                    {

                    }
                    var module = "";
                    try
                    {
                        var mymodule = ad.ControllerTypeInfo.CustomAttributes.Where(x => x.AttributeType == typeof(MyModuleAttribute)).ToList();
                        if (mymodule.Count > 0 && ad.ControllerTypeInfo.CustomAttributes.Where(x => x.AttributeType == typeof(MyModuleAttribute)).ToList().LastOrDefault().ConstructorArguments.Count > 1)
                        {
                            module = ad.ControllerTypeInfo.CustomAttributes.Where(x => x.AttributeType == typeof(MyModuleAttribute)).ToList().LastOrDefault().ConstructorArguments[1].Value.ToString();
                        }
                        else
                        {
                            module = ad.ControllerName;
                        }
                    }
                    catch (Exception)
                    {

                    }

                    var permissionKey = $"{area}/{module}/{ad.ActionName}";

                    var isPublic = ad.MethodInfo.IsDefined(typeof(AllowAnonymousAttribute), false) || ad.ControllerTypeInfo.IsDefined(typeof(AllowAnonymousAttribute), false);

                    var isAllRights = ad.MethodInfo.IsDefined(typeof(AuthorizeAttribute), false) || ad.ControllerTypeInfo.IsDefined(typeof(AuthorizeAttribute), false);
                    if (isPublic == true)
                    {
                        return true;
                    }
                    if (isAllRights)
                    {
                        var ps = httpContext.RequestServices.GetService<IKevinPermissionService>();
                        var http = httpContext.RequestServices.GetService<IHttpContextAccessor>();
                        if (ps != default && http != default)
                        {
                            bool canAccess = ps.IsAccess(permissionKey, http);
                            return canAccess;
                        }
                        return false;
                    }

                    return false;
                }
                return true;
            }
            else
            {
                return false;
            }


        }

        /// <summary>
        /// 签发新Token
        /// </summary>
        /// <param name="httpContext"></param>
        private static void IssueNewToken(HttpContext httpContext)
        {
            var exp = Convert.ToInt64(JwtToken.GetClaims("exp", httpContext.RequestServices.GetService<IHttpContextAccessor>()));
            var exptime = Common.DateTimeHelper.UnixToTime(exp);
            if (exptime > DateTime.Now && exptime < DateTime.Now.AddMinutes(20))
            {
                try
                {
                    var cacheService = httpContext.RequestServices.GetService<ICacheService>();
                    if (cacheService != default)
                    {
                        //获取刷新令牌
                        var RefreshToken = cacheService.GetString(httpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", ""));
                        var Result = RenewTokenAsync(RefreshToken, httpContext);
                        if (!string.IsNullOrEmpty(Result.Result))
                        {
                            if (httpContext != default)
                            {
                                httpContext.Response.Headers["NewToken"] = Result.Result;
                                httpContext.Response.Headers["Access-Control-Expose-Headers"] = "NewToken";  //解决 Ionic 取不到 Header中的信息问题

                            }
                        }
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
        private static async Task<string> RenewTokenAsync(string token, HttpContext httpContext)
        {
            var clinet = new HttpClient();
            var disco = await clinet.GetDiscoveryDocumentAsync(ConfigHelper.GetValue("IdentityServerUrl"));
            if (!disco.IsError)
            {
                var refreshToken = await clinet.RequestRefreshTokenAsync(new RefreshTokenRequest
                {
                    ClientId = ConfigHelper.GetValue("IdentityServerInfo:ClientId"),
                    Address = disco.TokenEndpoint,
                    ClientSecret = ConfigHelper.GetValue("IdentityServerInfo:ClientSecret"),
                    Scope = ConfigHelper.GetValue("IdentityServerInfo:Scope"),
                    GrantType = OpenIdConnectGrantTypes.RefreshToken,
                    RefreshToken = token
                });
                if (!refreshToken.IsError)
                {
                    var cache = httpContext.RequestServices.GetService<ICacheService>();
                    if (cache != default)
                    {
                        //保存刷新令牌
                        cache.SetString(refreshToken.AccessToken, refreshToken.RefreshToken, TimeSpan.FromDays(2));
                    }
                    return refreshToken.AccessToken;
                }
            }

            return "";
        }


        /// <summary>
        /// 获取权限区域和模块
        /// </summary>
        /// <param name="HttpContext"></param>
        /// <returns></returns>
        public static string GetAuthorizationAreaModule(HttpContext httpContext)
        {
            var ad = httpContext.Features.Get<IEndpointFeature>()?.Endpoint?.Metadata?.FirstOrDefault(u => u is ControllerActionDescriptor) as ControllerActionDescriptor;
            var isSkip = ad.MethodInfo.IsDefined(typeof(SkipAuthorityAttribute), false) || ad.ControllerTypeInfo.IsDefined(typeof(SkipAuthorityAttribute), false);
            var isPublic = ad.MethodInfo.IsDefined(typeof(AllowAnonymousAttribute), false) || ad.ControllerTypeInfo.IsDefined(typeof(AllowAnonymousAttribute), false);
            if (isSkip == true || isPublic == true)
            {
                return "";
            }
          
            var area = "";
            try
            {
                var myarea = ad.ControllerTypeInfo.CustomAttributes.Where(x => x.AttributeType == typeof(MyAreaAttribute)).ToList();
                if (myarea.Count > 0)
                {
                    area = ad.ControllerTypeInfo.CustomAttributes.Where(x => x.AttributeType == typeof(MyAreaAttribute)).ToList().LastOrDefault()?.ConstructorArguments[1].Value?.ToString();
                }
                else
                {
                    area = ad.ControllerName;
                }
            }
            catch (Exception)
            {

            }
            var module = "";
            try
            {
                var mymodule = ad.ControllerTypeInfo.CustomAttributes.Where(x => x.AttributeType == typeof(MyModuleAttribute)).ToList();
                if (mymodule.Count > 0 && ad.ControllerTypeInfo.CustomAttributes.Where(x => x.AttributeType == typeof(MyModuleAttribute)).ToList().LastOrDefault().ConstructorArguments.Count > 1)
                {
                    module = ad.ControllerTypeInfo.CustomAttributes.Where(x => x.AttributeType == typeof(MyModuleAttribute)).ToList().LastOrDefault().ConstructorArguments[1].Value.ToString();
                }
                else
                {
                    module = ad.ControllerName;
                }
            }
            catch (Exception)
            { 
            }
            return $"{area}/{module}";
        }  
    }
}
