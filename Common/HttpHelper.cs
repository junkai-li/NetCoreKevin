using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Common
{

    /// <summary>
    /// 常用Http操作类集合
    /// </summary>
    public class HttpHelper
    {


        private static bool IsInit;
        private static IHttpClientFactory InitHttpClientFactory;



        private static IHttpClientFactory HttpClientFactory
        {
            get
            {
                if (!IsInit)
                {
                    var programType = Assembly.GetEntryAssembly().GetTypes().Where(t => t.Name == "Program").FirstOrDefault();
                    var serviceProvider = (IServiceProvider)programType.GetProperty("ServiceProvider", BindingFlags.Public | BindingFlags.Static).GetValue(programType);
                    InitHttpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
                    IsInit = true;
                }

                return InitHttpClientFactory;
            }
        }


        /// <summary>
        /// Get方式获取远程资源
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="headers">自定义Header集合</param>
        /// <param name="isSkipSslVerification">是否跳过SSL验证</param>
        /// <returns></returns>
        public static string Get(string url, Dictionary<string, string> headers = default, bool isSkipSslVerification = false)
        {
            string httpClientName = isSkipSslVerification ? "SkipSsl" : "";

            var client = HttpClientFactory.CreateClient(httpClientName);

            if (headers != default)
            {
                foreach (var header in headers)
                {
                    client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }

            using var httpResponse = client.GetStringAsync(url);
            return httpResponse.Result;
        }




        /// <summary>
        /// Model对象转换为Uri网址参数形式
        /// </summary>
        /// <param name="obj">Model对象</param>
        /// <param name="url">前部分网址</param>
        /// <returns></returns>
        public static string ModelToUriParam(object obj, string url = "")
        {
            PropertyInfo[] propertis = obj.GetType().GetProperties();
            StringBuilder sb = new();
            sb.Append(url);
            sb.Append('?');
            foreach (var p in propertis)
            {
                var v = p.GetValue(obj, null);
                if (v == null)
                    continue;

                sb.Append(p.Name);
                sb.Append('=');
                sb.Append(HttpUtility.UrlEncode(v.ToString()));
                sb.Append('&');
            }
            sb.Remove(sb.Length - 1, 1);

            return sb.ToString();
        }




        /// <summary>
        /// Post数据到指定url
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="data">数据</param>
        /// <param name="type">form,data,json,xml</param>
        /// <param name="headers">自定义Header集合</param>
        /// <param name="isSkipSslVerification">是否跳过SSL验证</param>
        /// <returns></returns>
        public static string Post(string url, string data, string type, Dictionary<string, string> headers = default, bool isSkipSslVerification = false)
        {

            string httpClientName = isSkipSslVerification ? "SkipSsl" : "";

            var client = HttpClientFactory.CreateClient(httpClientName);

            if (headers != default)
            {
                foreach (var header in headers)
                {
                    client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }

            using Stream dataStream = new MemoryStream(Encoding.UTF8.GetBytes(data));
            using HttpContent content = new StreamContent(dataStream);
            if (type == "form")
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            }
            else if (type == "data")
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("multipart/form-data");
            }
            else if (type == "json")
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            }
            else if (type == "xml")
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("text/xml");
            }

            content.Headers.ContentType.CharSet = "utf-8";

            using var httpResponse = client.PostAsync(url, content);
            return httpResponse.Result.Content.ReadAsStringAsync().Result;
        }




        /// <summary>
        /// Post数据到指定url,异步执行
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="data">数据</param>
        /// <param name="type">form,data,json,xml</param>
        /// <param name="headers">自定义Header集合</param>
        /// <param name="isSkipSslVerification">是否跳过SSL验证</param>
        /// <returns></returns>
        public async static void PostAsync(string url, string data, string type, Dictionary<string, string> headers = default, bool isSkipSslVerification = false)
        {
            await Task.Run(() =>
            {
                Post(url, data, type, headers, isSkipSslVerification);
            });
        }




        /// <summary>
        /// Post文件和数据到指定url
        /// </summary>
        /// <param name="url"></param>
        /// <param name="formItems">Post表单内容</param>
        /// <param name="headers">自定义Header集合</param>
        /// <param name="isSkipSslVerification">是否跳过SSL验证</param>
        /// <returns></returns>
        public static string PostForm(string url, List<PostFormItem> formItems, Dictionary<string, string> headers = default, bool isSkipSslVerification = false)
        {
            string httpClientName = isSkipSslVerification ? "SkipSsl" : "";

            var client = HttpClientFactory.CreateClient(httpClientName);

            if (headers != default)
            {
                foreach (var header in headers)
                {
                    client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }

            string boundary = "----" + DateTime.UtcNow.Ticks.ToString("x");

            using MultipartFormDataContent formDataContent = new(boundary);
            foreach (var item in formItems)
            {
                if (item.IsFile)
                {
                    //上传文件
                    formDataContent.Add(new StreamContent(item.FileContent), item.Key, item.FileName);
                }
                else
                {
                    //上传文本
                    formDataContent.Add(new StringContent(item.Value), item.Key);
                }
            }

            using var httpResponse = client.PostAsync(url, formDataContent);
            return httpResponse.Result.Content.ReadAsStringAsync().Result;
        }



        /// <summary>
        /// Post 提交 From 表单数据模型结构
        /// </summary>
        public class PostFormItem
        {

            /// <summary>
            /// 表单键，request["key"]
            /// </summary>
            public string Key { set; get; }



            /// <summary>
            /// 表单值,上传文件时忽略，request["key"].value
            /// </summary>
            public string Value { set; get; }



            /// <summary>
            /// 是否是文件
            /// </summary>
            public bool IsFile
            {
                get
                {
                    if (FileContent == null || FileContent.Length == 0)
                        return false;

                    if (FileContent != null && FileContent.Length > 0 && string.IsNullOrWhiteSpace(FileName))
                        throw new Exception("上传文件时 FileName 属性值不能为空");
                    return true;
                }
            }



            /// <summary>
            /// 上传的文件名
            /// </summary>
            public string FileName { set; get; }



            /// <summary>
            /// 上传的文件内容
            /// </summary>
            public Stream FileContent { set; get; }


        }

    }
}
