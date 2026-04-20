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
    }
}
