using kevin.Share.Dtos;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kevin.Common.App
{

    public static class KevinHttpContext
    {

        public static HttpContext Current(this IHttpContextAccessor httpContext)
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
        public static string GetUrl(this IHttpContextAccessor httpContext)
        {
            return httpContext.GetBaseUrl() + $"{httpContext.Current().Request.Path}{httpContext.Current().Request.QueryString}";
        }


        /// <summary>
        /// 获取基础Url信息
        /// </summary>
        /// <returns></returns>
        public static string GetBaseUrl(this IHttpContextAccessor httpContext)
        {

            var url = $"{httpContext.Current().Request.Scheme}://{httpContext.Current().Request.Host.Host}";

            if (httpContext.Current().Request.Host.Port != null)
            {
                url = url + $":{httpContext.Current().Request.Host.Port}";
            }

            return url;
        }



        /// <summary>
        /// RequestBody中的内容
        /// </summary>
        public static string GetRequestBody(this IHttpContextAccessor httpContext)
        {
            var requestContent = "";

            using (Stream requestBody = new MemoryStream())
            {
                if (httpContext.Current().Request.Body.Length > 0)
                {
                    httpContext.Current().Request.Body.CopyTo(requestBody);
                    httpContext.Current().Request.Body.Position = 0;

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
        public static List<dtoKeyValue> GetParameter(this IHttpContextAccessor httpContext)
        {
            var context = httpContext.Current();

            var parameters = new List<dtoKeyValue>();


            var queryList = context.Request.Query.ToList();
            foreach (var query in queryList)
            {
                parameters.Add(new dtoKeyValue { Key = query.Key, Value = query.Value });
            }

            string body = httpContext.GetRequestBody();

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
        public static string GetIpAddress(this IHttpContextAccessor httpContext)
        {
            return httpContext.Current().Connection.RemoteIpAddress.ToString();
        }



        /// <summary>
        /// 获取Header中的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetHeader(this IHttpContextAccessor httpContext, string key)
        {
            var query = httpContext.Current().Request.Headers.Where(t => t.Key.ToLower() == key.ToLower()).Select(t => t.Value);

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
