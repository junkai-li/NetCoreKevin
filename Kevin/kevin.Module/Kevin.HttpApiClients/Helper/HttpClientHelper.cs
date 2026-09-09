using Kevin.log4Net;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Web;

namespace Kevin.HttpApiClients.Helper
{
    public class HttpClientHelper
    {
        private static readonly object _initLock = new object();
        private static IHttpClientFactory? InitHttpClientFactory;

        /// <summary>
        /// 容器不可用时的兜底客户端（按命名客户端缓存并复用，避免每次 new HttpClient 导致端口耗尽）
        /// </summary>
        private static readonly Dictionary<string, HttpClient> FallbackClients = new();
        private static bool FallbackWarned;

        /// <summary>
        /// 注入容器：宿主在容器构建完成后调用一次（如 HttpClientHelper.Init(app.ApplicationServices)），
        /// 未注入时本类会自行反射探测框架容器，探测不到才退回兜底客户端
        /// </summary>
        /// <param name="serviceProvider">容器（建议传根容器）</param>
        public static void Init(IServiceProvider? serviceProvider)
        {
            if (serviceProvider == null)
            {
                return;
            }
            lock (_initLock)
            {
                InitHttpClientFactory ??= TryGetFactory(serviceProvider);
            }
        }

        /// <summary>
        /// 从容器取 IHttpClientFactory（IHttpClientFactory 是单例，取到后即可长期持有，不受容器后续释放影响）
        /// </summary>
        /// <param name="serviceProvider">容器</param>
        /// <returns></returns>
        private static IHttpClientFactory? TryGetFactory(IServiceProvider serviceProvider)
        {
            try
            {
                return serviceProvider.GetService<IHttpClientFactory>();
            }
            catch
            {
                //传进来的可能是已被释放的请求作用域容器（GlobalServices.ServiceProvider 会被请求中间件按请求覆盖），
                //此处不抛，交给下一个容器来源处理
                return null;
            }
        }

        /// <summary>
        /// 容器中的 IHttpClientFactory：只在解析成功时缓存，解析失败不能把null固化，
        /// 否则启动早期（容器尚未就绪）的第一次调用会让之后所有请求永远拿不到客户端
        /// </summary>
        private static IHttpClientFactory? HttpClientFactory
        {
            get
            {
                if (InitHttpClientFactory != null)
                {
                    return InitHttpClientFactory;
                }
                lock (_initLock)
                {
                    if (InitHttpClientFactory != null)
                    {
                        return InitHttpClientFactory;
                    }
                    foreach (var serviceProvider in FindServiceProviders())
                    {
                        var factory = TryGetFactory(serviceProvider);
                        if (factory != null)
                        {
                            InitHttpClientFactory = factory;
                            return factory;
                        }
                    }
                    return null;
                }
            }
        }

        /// <summary>
        /// 反射探测宿主容器：本程序集不能引用 Kevin.Common（Kevin.Common 反向依赖本程序集），只能按类型全名取 GlobalServices.ServiceProvider；
        /// 再兼容入口程序集 Program 上公开静态属性 ServiceProvider 的历史宿主写法
        /// </summary>
        /// <returns></returns>
        private static List<IServiceProvider> FindServiceProviders()
        {
            var providers = new List<IServiceProvider>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    var globalServicesType = assembly.GetType("Kevin.Common.App.Global.GlobalServices", false);
                    if (globalServicesType?.GetProperty("ServiceProvider", BindingFlags.Public | BindingFlags.Static)?.GetValue(null) is IServiceProvider provider)
                    {
                        providers.Add(provider);
                    }
                }
                catch
                {
                }
            }
            try
            {
                var programType = Assembly.GetEntryAssembly()?.GetTypes().FirstOrDefault(t => t.Name == "Program");
                if (programType?.GetProperty("ServiceProvider", BindingFlags.Public | BindingFlags.Static)?.GetValue(null) is IServiceProvider provider)
                {
                    providers.Add(provider);
                }
            }
            catch
            {
            }
            return providers;
        }

        /// <summary>
        /// 取命名HttpClient：容器拿不到时兜底建一个内置客户端，而不是返回null——
        /// 返回null只会让调用方表现为“响应内容为空”，把真实原因（容器未就绪、宿主没注册 AddHttpClient）吞掉
        /// </summary>
        /// <param name="name">命名客户端（与 AddKevinHttpApiClients 中注册的名字一致）</param>
        /// <returns></returns>
        private static HttpClient CreateClient(string name)
        {
            var client = HttpClientFactory?.CreateClient(name);
            if (client != null)
            {
                return client;
            }
            lock (_initLock)
            {
                if (!FallbackClients.TryGetValue(name, out var fallbackClient))
                {
                    var handler = new HttpClientHandler { AllowAutoRedirect = false };
                    if (name == "SkipSsl")
                    {
                        handler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true;
                    }
                    fallbackClient = new HttpClient(handler);
                    FallbackClients[name] = fallbackClient;
                }
                if (!FallbackWarned)
                {
                    FallbackWarned = true;
                    try
                    {
                        LogHelper.logger.Warn($"未能从容器获取 IHttpClientFactory，{nameof(HttpClientHelper)} 已退回内置兜底HttpClient（请确认宿主已调用 AddKevinHttpApiClients 注册、并在容器构建后调用 {nameof(Init)}）");
                    }
                    catch
                    {
                    }
                }
                return fallbackClient;
            }
        }


        /// <summary>
        /// Get方式获取远程资源
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="headers">自定义Header集合</param>
        /// <param name="isSkipSslVerification">是否跳过SSL验证</param>
        /// <returns></returns>
        public static string? Get(string url, Dictionary<string, string> headers = null, bool isSkipSslVerification = false)
        {
            string httpClientName = isSkipSslVerification ? "SkipSsl" : "";

            var client = CreateClient(httpClientName);

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
        /// Post Json或XML 数据到指定url
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="data">数据</param>
        /// <param name="type">json,xml</param>
        /// <param name="headers">自定义Header集合</param>
        /// <param name="isSkipSslVerification">是否跳过SSL验证</param>
        /// <returns></returns>
        public static string Post(string url, string data, string type, Dictionary<string, string> headers = default, bool isSkipSslVerification = false)
        {

            string httpClientName = isSkipSslVerification ? "SkipSsl" : "";

            var client = CreateClient(httpClientName);

            if (headers != default)
            {
                foreach (var header in headers)
                {
                    client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }

            using Stream dataStream = new MemoryStream(Encoding.UTF8.GetBytes(data));
            using HttpContent content = new StreamContent(dataStream);

            if (type == "json")
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            }
            else if (type == "xml")
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("text/xml");
            }
            if (content.Headers.ContentType != null)
            {
                content.Headers.ContentType.CharSet = "utf-8";
            }
            using var httpResponse = client.PostAsync(url, content);
            return httpResponse.Result.Content.ReadAsStringAsync().Result ?? "";
        }




        /// <summary>
        /// Post Json或XML 数据到指定url,异步执行
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="data">数据</param>
        /// <param name="type">json,xml</param>
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
        /// Post数据到指定url
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="formItems">数据</param> 
        /// <param name="headers">自定义Header集合</param>
        /// <param name="isSkipSslVerification">是否跳过SSL验证</param>
        /// <returns></returns>
        public static string? PostForm(string url, Dictionary<string, string> formItems, Dictionary<string, string> headers = default, bool isSkipSslVerification = false)
        {

            string httpClientName = isSkipSslVerification ? "SkipSsl" : "";

            var client = CreateClient(httpClientName);

            if (headers != default)
            {
                foreach (var header in headers)
                {
                    client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }
            using FormUrlEncodedContent formContent = new(formItems);
            formContent.Headers.ContentType!.CharSet = "utf-8";
            using var httpResponse = client.PostAsync(url, formContent);
            return httpResponse.Result.Content.ReadAsStringAsync().Result;
        }



        /// <summary>
        /// Post文件和数据到指定url
        /// </summary>
        /// <param name="url"></param>
        /// <param name="formItems">Post表单内容</param>
        /// <param name="headers">自定义Header集合</param>
        /// <param name="isSkipSslVerification">是否跳过SSL验证</param>
        /// <returns></returns>
        public static string? PostFormData(string url, List<PostFormItem> formItems, Dictionary<string, string> headers = default, bool isSkipSslVerification = false)
        {
            string httpClientName = isSkipSslVerification ? "SkipSsl" : "";

            var client = CreateClient(httpClientName);

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
                    if (item.FileContent != default)
                    {
                        //上传文件
                        formDataContent.Add(new StreamContent(item.FileContent), item.Key, item.FileName);
                    }
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
        public static string CreatePostHttpResponse(string url, string postData)
        {
            HttpWebRequest webrequest = (HttpWebRequest)HttpWebRequest.Create(url);
            webrequest.Method = "post";
            webrequest.ContentType = "application/json";
            webrequest.Timeout = 600000;
            WebResponse httpWebResponse;
            try
            {
                byte[] postdatabyte = Encoding.UTF8.GetBytes(postData);
                webrequest.ContentLength = postdatabyte.Length;
                Stream stream;
                stream = webrequest.GetRequestStream();
                stream.Write(postdatabyte, 0, postdatabyte.Length);
                stream.Close();

                httpWebResponse = webrequest.GetResponse();
            }
            catch (WebException ex)
            {
                return ex.Message;
                //httpWebResponse = (System.Net.HttpWebResponse)ex.Response;
            }
            if (httpWebResponse == null)
            {
                return "{status:'Error'}";
            }
            using (StreamReader responseStream = new StreamReader(httpWebResponse.GetResponseStream()))
            {

                String ret = responseStream.ReadToEnd();
                httpWebResponse.Close();
                return ret;
            }
        }


        /// <summary>
        /// 可传头部参数的post方式
        /// </summary>
        /// <param name="url"></param>
        /// <param name="headData"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        public static string CreatePostHttpResponse(string url, IDictionary<string, string> headData, string postData)
        {
            WebResponse httpWebResponse;
            try
            {
                HttpWebRequest webrequest = (HttpWebRequest)HttpWebRequest.Create(url);
                webrequest.Method = "post";
                webrequest.ContentType = "application/json";

                foreach (var item in headData)
                {
                    webrequest.Headers.Add(item.Key, item.Value);
                }

                byte[] postdatabyte = Encoding.UTF8.GetBytes(postData);
                webrequest.ContentLength = postdatabyte.Length;
                Stream stream;
                stream = webrequest.GetRequestStream();
                stream.Write(postdatabyte, 0, postdatabyte.Length);
                stream.Close();

                httpWebResponse = webrequest.GetResponse();
            }
            catch (WebException ex)
            {
                httpWebResponse = (System.Net.HttpWebResponse)ex.Response;
            }
            if (httpWebResponse == null)
            {
                return "{status:'Error'}";
            }
            using (StreamReader responseStream = new StreamReader(httpWebResponse.GetResponseStream()))
            {

                String ret = responseStream.ReadToEnd();
                httpWebResponse.Close();
                return ret;
            }

        }


        /// <summary>
        /// Post 提交 From 表单数据模型结构
        /// </summary>
        public class PostFormItem
        {

            /// <summary>
            /// 表单键，request["key"]
            /// </summary>
            public string Key { set; get; } = "";



            /// <summary>
            /// 表单值,上传文件时忽略，request["key"].value
            /// </summary>
            public string Value { set; get; } = "";



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
            public string FileName { set; get; } = "";



            /// <summary>
            /// 上传的文件内容
            /// </summary>
            public Stream? FileContent { set; get; }


        }

    }
}
