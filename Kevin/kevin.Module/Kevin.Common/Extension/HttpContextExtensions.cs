using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Http
{
    /// <summary>
    /// Http 拓展类
    /// </summary> 
    public static class HttpContextExtensions
    {
        public static string GetUserIp(this HttpContext context)
        {
            try
            {
                var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
                if (string.IsNullOrEmpty(ip))
                {
                    ip = context.Connection.RemoteIpAddress.ToString();
                }
                return ip;
            }
            catch
            {

                return "";
            }

        }
        
        public static string GetBody(this HttpContext context)
        {
            try
            {
                var body = "";
                using (Stream requestBody = new MemoryStream())
                {
                    if (context.Request.Body.Length > 0)
                    {
                        context.Request.Body.CopyTo(requestBody);
                        context.Request.Body.Position = 0;
                        requestBody.Position = 0;
                        using (var requestReader = new StreamReader(requestBody, encoding: Encoding.UTF8))
                        {
                            body = requestReader.ReadToEnd();
                        }
                    }
                }
                return body;
            }
            catch
            {

                return "";
            }

        }
        /// <summary>
        /// 获取Url信息
        /// </summary>
        /// <returns></returns>
        public static string GetUrlAction(this HttpContext context)
        {
            try
            {
                return GetBaseUrl(context) + $"{context.Request.Path}";
            }
            catch
            {

                return "";
            }

        }
        /// <summary>
        /// 获取Url信息
        /// </summary>
        /// <returns></returns>
        public static string GetUrl(this HttpContext context)
        {
            try
            {
                return GetBaseUrl(context) + $"{context.Request.Path}{context.Request.QueryString}";
            }
            catch
            {

                return "";
            }

        }

        /// <summary>
        /// 获取基础Url信息
        /// </summary>
        /// <returns></returns>
        public static string GetBaseUrl(this HttpContext context)
        {
            try
            {
                var url = $"{context.Request.Scheme}://{context.Request.Host.Host}";

                if (context.Request.Host.Port != null)
                {
                    url = url + $":{context.Request.Host.Port}";
                }
                return url;
            }
            catch
            {

                return "";
            }


        }

        /// <summary>
        /// 获取设备信息
        /// </summary>
        /// <returns></returns>
        public static string GetDevice(this HttpContext context)
        {

            try
            {
                string device = context.Request.Headers["User-Agent"]; 
                return device;
            }
            catch
            {

                return "";
            }
        }
        /// <summary>
        /// 获取 Action 特性
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public static TAttribute GetMetadata<TAttribute>(this HttpContext httpContext)
            where TAttribute : class
        {
            return httpContext.Features.Get<IEndpointFeature>().Endpoint?.Metadata?.GetMetadata<TAttribute>();
        }

        /// <summary>
        /// 获取 控制器/Action 描述器
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public static ControllerActionDescriptor GetControllerActionDescriptor(this HttpContext httpContext)
        {
            return httpContext.Features.Get<IEndpointFeature>().Endpoint?.Metadata?.FirstOrDefault(u => u is ControllerActionDescriptor) as ControllerActionDescriptor;
        }

        /// <summary>
        /// 设置规范化文档自动登录
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="accessToken"></param>
        public static void SigninToSwagger(this HttpContext httpContext, string accessToken)
        {
            // 设置 Swagger 刷新自动授权
            httpContext.Response.Headers["access-token"] = accessToken;
        }

        /// <summary>
        /// 设置规范化文档退出登录
        /// </summary>
        /// <param name="httpContext"></param>
        public static void SignoutToSwagger(this HttpContext httpContext)
        {
            httpContext.Response.Headers["access-token"] = "invalid_token";
        }

        /// <summary>
        /// 设置响应头 Tokens
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="accessToken"></param>
        /// <param name="refreshToken"></param>
        public static void SetTokensOfResponseHeaders(this HttpContext httpContext, string accessToken, string refreshToken = null)
        {
            httpContext.Response.Headers["access-token"] = accessToken;
            if (!string.IsNullOrWhiteSpace(refreshToken))
            {
                httpContext.Response.Headers["x-access-token"] = refreshToken;
            }
        }

        /// <summary>
        /// 获取本机 IPv4地址
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetLocalIpAddressToIPv4(this HttpContext context)
        {
            return context.Connection.LocalIpAddress?.MapToIPv4()?.ToString();
        }

        /// <summary>
        /// 获取本机 IPv6地址
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetLocalIpAddressToIPv6(this HttpContext context)
        {
            return context.Connection.LocalIpAddress?.MapToIPv6()?.ToString();
        }

        /// <summary>
        /// 获取远程 IPv4地址
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetRemoteIpAddressToIPv4(this HttpContext context)
        {
            return context.Connection.RemoteIpAddress?.MapToIPv4()?.ToString();
        }

        /// <summary>
        /// 获取远程 IPv6地址
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetRemoteIpAddressToIPv6(this HttpContext context)
        {
            return context.Connection.RemoteIpAddress?.MapToIPv6()?.ToString();
        }

        /// <summary>
        /// 获取完整请求地址
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetRequestUrlAddress(this HttpRequest request)
        {
            return new StringBuilder()
                    .Append(request.Scheme)
                    .Append("://")
                    .Append(request.Host)
                    .Append(request.PathBase)
                    .Append(request.Path)
                    .Append(request.QueryString)
                    .ToString();
        }

        /// <summary>
        /// 获取来源地址
        /// </summary>
        /// <param name="request"></param>
        /// <param name="refererHeaderKey"></param>
        /// <returns></returns>
        public static string GetRefererUrlAddress(this HttpRequest request, string refererHeaderKey = "Referer")
        {
            return request.Headers[refererHeaderKey].ToString();
        }

        /// <summary>
        /// 读取 Body 内容
        /// </summary>
        /// <param name="httpContext"></param>
        /// <remarks>需先在 Startup 的 Configure 中注册 app.EnableBuffering()</remarks>
        /// <returns></returns>
        public static async Task<string> ReadBodyContentAsync(this HttpContext httpContext)
        {
            if (httpContext == null) return default;
            return await httpContext.Request.ReadBodyContentAsync();
        }

        /// <summary>
        /// 读取 Body 内容
        /// </summary>
        /// <param name="request"></param>
        /// <remarks>需先在 Startup 的 Configure 中注册 app.EnableBuffering()</remarks>
        /// <returns></returns>
        public static async Task<string> ReadBodyContentAsync(this HttpRequest request)
        {
            request.Body.Seek(0, SeekOrigin.Begin);

            using var reader = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true);
            var body = await reader.ReadToEndAsync();

            // 回到顶部，解决此类问题 https://gitee.com/dotnetchina/Furion/issues/I6NX9E
            request.Body.Seek(0, SeekOrigin.Begin);
            return body;
        } 
 
        /// <summary>
        /// 判断是否是 WebSocket 请求
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static bool IsWebSocketRequest(this HttpContext context)
        {
            return context.WebSockets.IsWebSocketRequest || context.Request.Path == "/ws";
        }
    }
}
