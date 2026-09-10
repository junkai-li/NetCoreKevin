using Common;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Kevin.HttpApiClients.Helper
{
    public class HttpClientDisHelper : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        private readonly int _retryTimes;
        private readonly int _sleepMillisecondsTimeout;

        /// <summary>
        /// 初始化HttpClientHelper
        /// </summary>
        /// <param name="baseAddress">API基础地址</param>
        /// <param name="timeoutSeconds">超时时间(秒)</param>
        /// <param name="retryTimes">重试次数，默认为0不重试</param>
        /// <param name="sleepMillisecondsTimeout">重试间隔时间(毫秒)</param>
        public HttpClientDisHelper(string baseAddress = "", int timeoutSeconds = 30, int retryTimes = 0, int sleepMillisecondsTimeout = 1000)
        {
            _httpClient = new HttpClient(new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            });

            if (!string.IsNullOrEmpty(baseAddress))
            {
                _httpClient.BaseAddress = new Uri(baseAddress);
            }

            _httpClient.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = false,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping //避免中文被转义
            };
            _retryTimes = retryTimes <= 0 ? 1 : retryTimes;
            _sleepMillisecondsTimeout = sleepMillisecondsTimeout;
        }

        /// <summary>
        /// 设置授权头
        /// </summary>
        public void SetAuthorization(string scheme, string parameter)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme, parameter);
        }

        /// <summary>
        /// 添加请求头
        /// </summary>
        public void AddHeader(string name, string value)
        {
            _httpClient.DefaultRequestHeaders.Add(name, value);
        }

        /// <summary>
        /// 发送GET请求
        /// </summary>
        public async Task<T> GetAsync<T>(string url)
        {
            try
            {
                return await RetryTools.RetryAsync<T>(async () =>
                 {
                     var response = await _httpClient.GetAsync(url);
                     response.EnsureSuccessStatusCode();
                     var content = await response.Content.ReadAsStringAsync();
                     var data = JsonSerializer.Deserialize<T>(content, _jsonOptions);
                     if (data != null)
                     {
                         return data;
                     }
                     throw new Exception($"HTTP请求失败: 数据返回null" + url);
                 }, _retryTimes, _sleepMillisecondsTimeout);
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"HTTP请求失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 发送POST请求
        /// </summary>
        public async Task<T> PostAsync<T>(string url, string data)
        {
            try
            {
                return await RetryTools.RetryAsync<T>(async () =>
                {
                    var content = new StringContent(data, Encoding.UTF8, "application/json");
                    using Stream dataStream = new MemoryStream(Encoding.UTF8.GetBytes(data));
                    using HttpContent httpContent = new StreamContent(dataStream);
                    using var httpResponse = await _httpClient.PostAsync(url, content);
                    var responseContent = await httpResponse.Content.ReadAsStringAsync() ?? "";
                    var responseData = JsonSerializer.Deserialize<T>(responseContent, _jsonOptions);
                    if (responseData != null)
                    {
                        return responseData;
                    }
                    throw new Exception($"HTTP请求失败: 返回数据异常" + url);
                }, _retryTimes, _sleepMillisecondsTimeout);

            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"HTTP请求失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 发送PUT请求
        /// </summary>
        public async Task<TResponse> PutAsync<TRequest, TResponse>(string url, TRequest data)
        {
            try
            {
                return await RetryTools.RetryAsync<TResponse>(async () =>
                {
                    var json = JsonSerializer.Serialize(data, _jsonOptions);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await _httpClient.PutAsync(url, content);
                    response.EnsureSuccessStatusCode();
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var dataResponse = JsonSerializer.Deserialize<TResponse>(responseContent, _jsonOptions);
                    if (dataResponse != null)
                    {
                        return dataResponse;
                    }
                    throw new Exception($"HTTP请求失败: 返回数据异常" + url);
                }, _retryTimes, _sleepMillisecondsTimeout);
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"HTTP请求失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 发送DELETE请求
        /// </summary>
        public async Task<bool> DeleteAsync(string url)
        {
            try
            {
                return await RetryTools.RetryAsync<bool>(async () =>
                {
                    var response = await _httpClient.DeleteAsync(url);
                    return response.IsSuccessStatusCode;
                }, _retryTimes, _sleepMillisecondsTimeout);
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"HTTP请求失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 提交表单(application/x-www-form-urlencoded)，返回原始响应文本
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="formItems">表单字段集合</param>
        public async Task<string> PostFormAsync(string url, IDictionary<string, string> formItems)
        {
            if (formItems == null || formItems.Count == 0)
            {
                throw new ArgumentException("表单字段不能为空", nameof(formItems));
            }

            return await PostCoreAsync(url, () => new FormUrlEncodedContent(formItems), "表单提交失败");
        }

        /// <summary>
        /// 提交表单(application/x-www-form-urlencoded)，响应按JSON反序列化
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="formItems">表单字段集合</param>
        public async Task<TResponse> PostFormAsync<TResponse>(string url, IDictionary<string, string> formItems)
        {
            var responseContent = await PostFormAsync(url, formItems);
            return DeserializeResponse<TResponse>(responseContent, url);
        }

        /// <summary>
        /// 提交 multipart/form-data(普通字段与文件混排，文件的字段名/文件名/Content-Type 均可自定义)，返回原始响应文本
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="formItems">表单项集合，见 <see cref="FormDataItem"/></param>
        public async Task<string> PostFormDataAsync(string url, IEnumerable<FormDataItem> formItems)
        {
            if (formItems == null)
            {
                throw new ArgumentNullException(nameof(formItems));
            }
            var items = formItems.ToList();
            if (items.Count == 0)
            {
                throw new ArgumentException("表单内容不能为空", nameof(formItems));
            }

            //请求内容发出后即被框架释放，重试没有第二次可用的内容，所以先把内容准备成可重复发送的状态
            foreach (var item in items)
            {
                item.EnsureReplayable();
            }

            return await PostCoreAsync(url, () =>
            {
                var content = new MultipartFormDataContent();
                foreach (var item in items)
                {
                    if (item.IsFile)
                    {
                        content.Add(item.CreateContent(), item.Key, item.FileName);
                    }
                    else
                    {
                        content.Add(item.CreateContent(), item.Key);
                    }
                }
                return content;
            }, "表单数据提交失败");
        }

        /// <summary>
        /// 提交 multipart/form-data(普通字段与文件混排)，响应按JSON反序列化
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="formItems">表单项集合，见 <see cref="FormDataItem"/></param>
        public async Task<TResponse> PostFormDataAsync<TResponse>(string url, IEnumerable<FormDataItem> formItems)
        {
            var responseContent = await PostFormDataAsync(url, formItems);
            return DeserializeResponse<TResponse>(responseContent, url);
        }

        /// <summary>
        /// 提交 multipart/form-data，只上传多个文件并可附带普通字段
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="files">文件集合，每项可自定义表单字段名、文件名、Content-Type</param>
        /// <param name="fields">附带的普通字段</param>
        public async Task<string> PostFormDataAsync(string url, IEnumerable<FormDataItem> files, IDictionary<string, string>? fields)
        {
            var items = new List<FormDataItem>();
            if (fields != null)
            {
                foreach (var field in fields)
                {
                    items.Add(FormDataItem.Field(field.Key, field.Value));
                }
            }
            if (files != null)
            {
                items.AddRange(files);
            }
            return await PostFormDataAsync(url, items);
        }

        /// <summary>
        /// 提交 multipart/form-data，只上传多个文件并可附带普通字段，响应按JSON反序列化
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="files">文件集合，每项可自定义表单字段名、文件名、Content-Type</param>
        /// <param name="fields">附带的普通字段</param>
        public async Task<TResponse> PostFormDataAsync<TResponse>(string url, IEnumerable<FormDataItem> files, IDictionary<string, string>? fields)
        {
            var responseContent = await PostFormDataAsync(url, files, fields);
            return DeserializeResponse<TResponse>(responseContent, url);
        }

        /// <summary>
        /// POST 指定内容：每次重试都重新构造 <see cref="HttpContent"/>，避免内容被上一次尝试消耗掉
        /// </summary>
        private async Task<string> PostCoreAsync(string url, Func<HttpContent> contentFactory, string errorPrefix)
        {
            try
            {
                return await RetryTools.RetryAsync<string>(async () =>
                {
                    using var content = contentFactory();
                    using var httpResponse = await _httpClient.PostAsync(url, content);
                    httpResponse.EnsureSuccessStatusCode();
                    return await httpResponse.Content.ReadAsStringAsync() ?? "";
                }, _retryTimes, _sleepMillisecondsTimeout);
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"{errorPrefix}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 反序列化响应内容
        /// </summary>
        private TResponse DeserializeResponse<TResponse>(string responseContent, string url)
        {
            var dataResponse = JsonSerializer.Deserialize<TResponse>(responseContent, _jsonOptions);
            if (dataResponse != null)
            {
                return dataResponse;
            }
            throw new Exception($"HTTP请求失败: 返回数据异常" + url);
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        public async Task<TResponse> UploadFileAsync<TResponse>(string url, byte[] fileData, string fileName, string formDataName = "file")
        {
            try
            {
                return await RetryTools.RetryAsync<TResponse>(async () =>
                {
                    using var content = new MultipartFormDataContent();
                    content.Add(new ByteArrayContent(fileData), formDataName, fileName);

                    var response = await _httpClient.PostAsync(url, content);
                    response.EnsureSuccessStatusCode();

                    var responseContent = await response.Content.ReadAsStringAsync();
                    var dataResponse = JsonSerializer.Deserialize<TResponse>(responseContent, _jsonOptions);
                    if (dataResponse != null)
                    {
                        return dataResponse;
                    }
                    throw new Exception($"HTTP请求失败: 返回数据异常" + url);
                }, _retryTimes, _sleepMillisecondsTimeout);
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"文件上传失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        public async Task<byte[]> DownloadFileAsync(string url)
        {
            try
            {
                return await RetryTools.RetryAsync<byte[]>(async () =>
                {
                    return await _httpClient.GetByteArrayAsync(url);
                }, _retryTimes, _sleepMillisecondsTimeout);
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"文件下载失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            _httpClient?.Dispose();
        }

        /// <summary>
        /// multipart/form-data 表单项：要么是普通字段(Key+Value)，要么是文件(Key+FileName+内容)
        /// </summary>
        public class FormDataItem
        {
            /// <summary>
            /// 表单键，服务端 request["key"]；文件项就是上传控件的字段名，可自定义(如 file、media、files)
            /// </summary>
            public string Key { get; set; } = "";

            /// <summary>
            /// 字段值，文件项忽略
            /// </summary>
            public string Value { get; set; } = "";

            /// <summary>
            /// 服务端收到的文件名，文件项必填
            /// </summary>
            public string FileName { get; set; } = "";

            /// <summary>
            /// 文件的 Content-Type，为空时按 application/octet-stream 发送(如 image/png、application/pdf)
            /// </summary>
            public string? ContentType { get; set; }

            /// <summary>
            /// 文件字节内容，与 <see cref="FileContent"/> 二选一，优先使用本属性
            /// </summary>
            public byte[]? FileBytes { get; set; }

            /// <summary>
            /// 文件流内容，与 <see cref="FileBytes"/> 二选一；提交时从流的当前位置读入内存，本类不会关闭调用方的流
            /// </summary>
            public Stream? FileContent { get; set; }

            /// <summary>
            /// 是否是文件项
            /// </summary>
            public bool IsFile => FileBytes != null || FileContent != null;

            /// <summary>
            /// 普通字段
            /// </summary>
            public static FormDataItem Field(string key, string value)
            {
                return new FormDataItem { Key = key, Value = value };
            }

            /// <summary>
            /// 文件字段(字节内容)
            /// </summary>
            /// <param name="key">表单字段名</param>
            /// <param name="fileName">服务端收到的文件名</param>
            /// <param name="content">文件内容</param>
            /// <param name="contentType">文件的 Content-Type，可为空</param>
            public static FormDataItem File(string key, string fileName, byte[] content, string? contentType = null)
            {
                return new FormDataItem { Key = key, FileName = fileName, FileBytes = content, ContentType = contentType };
            }

            /// <summary>
            /// 文件字段(流内容)
            /// </summary>
            /// <param name="key">表单字段名</param>
            /// <param name="fileName">服务端收到的文件名</param>
            /// <param name="content">文件流</param>
            /// <param name="contentType">文件的 Content-Type，可为空</param>
            public static FormDataItem File(string key, string fileName, Stream content, string? contentType = null)
            {
                return new FormDataItem { Key = key, FileName = fileName, FileContent = content, ContentType = contentType };
            }

            /// <summary>
            /// 校验内容并使其可重复发送：
            /// 请求内容发送后会被框架释放（<see cref="StreamContent"/> 释放时会连带关闭它包装的流），
            /// 重试就没有第二次可用的内容，所以流形式的内容统一先固化成字节
            /// </summary>
            internal void EnsureReplayable()
            {
                if (string.IsNullOrWhiteSpace(Key))
                {
                    throw new ArgumentException("表单字段名 Key 不能为空");
                }
                if (!IsFile)
                {
                    return;
                }
                if (string.IsNullOrWhiteSpace(FileName))
                {
                    throw new ArgumentException($"上传文件时 FileName 不能为空，字段：{Key}");
                }
                if (FileBytes != null || FileContent == null)
                {
                    return;
                }

                using var buffer = new MemoryStream();
                FileContent.CopyTo(buffer);
                FileContent = null;
                FileBytes = buffer.ToArray();
            }

            /// <summary>
            /// 构造本项对应的请求内容
            /// </summary>
            internal HttpContent CreateContent()
            {
                if (!IsFile)
                {
                    return new StringContent(Value, Encoding.UTF8);
                }

                HttpContent content = new ByteArrayContent(FileBytes ?? Array.Empty<byte>());
                if (!string.IsNullOrWhiteSpace(ContentType))
                {
                    if (!MediaTypeHeaderValue.TryParse(ContentType, out var mediaType))
                    {
                        throw new ArgumentException($"文件 {FileName} 的 ContentType 格式不正确: {ContentType}");
                    }
                    content.Headers.ContentType = mediaType;
                }
                else
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                }
                return content;
            }
        }
    }
}
