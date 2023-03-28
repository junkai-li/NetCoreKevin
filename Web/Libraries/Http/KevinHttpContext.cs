using Microsoft.AspNetCore.Http;
using Models.Dtos;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kevin.Web.Libraries.Http
{ 
    public class KevinHttpContext
    {

        public static Microsoft.AspNetCore.Http.HttpContext Current(IHttpContextAccessor httpContext)
        {
            var httpContextAccessor = httpContext;
            if (httpContextAccessor.HttpContext.Request.Body.Length > 0)
            {
                httpContextAccessor.HttpContext.Request.Body.Position = 0;
            }

            return httpContextAccessor.HttpContext;
        }


        /// <summary>
        /// 获取Url信息
        /// </summary>
        /// <returns></returns>
        public static string GetUrl(IHttpContextAccessor httpContext)
        {
            return GetBaseUrl(httpContext) + $"{Current(httpContext).Request.Path}{Current(httpContext).Request.QueryString}";
        }


        /// <summary>
        /// 获取基础Url信息
        /// </summary>
        /// <returns></returns>
        public static string GetBaseUrl(IHttpContextAccessor httpContext)
        {

            var url = $"{Current(httpContext).Request.Scheme}://{Current(httpContext).Request.Host.Host}";

            if (Current(httpContext).Request.Host.Port != null)
            {
                url = url + $":{Current(httpContext).Request.Host.Port}";
            }

            return url;
        }



        /// <summary>
        /// RequestBody中的内容
        /// </summary>
        public static string GetRequestBody(IHttpContextAccessor httpContext)
        {
            var requestContent = "";

            using (Stream requestBody = new MemoryStream())
            {
                if (Current(httpContext).Request.Body.Length > 0)
                {
                    Current(httpContext).Request.Body.CopyTo(requestBody);
                    Current(httpContext).Request.Body.Position = 0;

                    requestBody.Position = 0;

                    using (var requestReader = new StreamReader(requestBody))
                    {
                        requestContent = requestReader.ReadToEnd();
                    }
                }

            }

            return requestContent;
        }



        /// <summary>
        /// 获取 http 请求中的全部参数
        /// </summary>
        public static List<dtoKeyValue> GetParameter(IHttpContextAccessor httpContext)
        {
            var context = Current(httpContext);

            var parameters = new List<dtoKeyValue>();


            var queryList = context.Request.Query.ToList();
            foreach (var query in queryList)
            {
                parameters.Add(new dtoKeyValue { Key = query.Key, Value = query.Value });
            }

            string body = GetRequestBody(httpContext);

            if (!string.IsNullOrEmpty(body))
            {
                parameters.Add(new dtoKeyValue { Key = "body", Value = body });
            }
            else if (context.Request.HasFormContentType)
            {
                var fromlist = context.Request.Form.OrderBy(t => t.Key).ToList();

                foreach (var fm in fromlist)
                {
                    parameters.Add(new dtoKeyValue { Key = fm.Key, Value = fm.Value.ToString() });
                }
            }

            return parameters;
        }




        /// <summary>
        /// 获取IP地址
        /// </summary>
        /// <returns></returns>
        public static string GetIpAddress(IHttpContextAccessor httpContext)
        {
            return Current(httpContext).Connection.RemoteIpAddress.ToString();
        }



        /// <summary>
        /// 获取Header中的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetHeader(string key, IHttpContextAccessor httpContext)
        {
            var query = Current(httpContext).Request.Headers.Where(t => t.Key.ToLower() == key.ToLower()).Select(t => t.Value);

            var ishave = query.Count();

            if (ishave != 0)
            {
                return query.FirstOrDefault().ToString();
            }
            else
            {
                return "";
            }
        }
    }
}
