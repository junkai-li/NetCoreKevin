using System.ComponentModel;
using System.Net;
using System.Text;

namespace kevin.AI.AgentFramework.Tools
{
    /// <summary>
    /// 给智能体调用的通用 HTTP 工具（支持增删查改：GET/POST/PUT/DELETE）
    /// 可传入 URL、查询参数、Body、以及自定义 Header。
    /// 返回响应文本，发生错误时以 "❌ " 开头的描述。
    /// </summary>
    public static class AgentHttpClientTools
    {
        private static HttpClient CreateHttpClient(int timeoutSeconds)
        {
            var handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.Brotli,
                AllowAutoRedirect = true
            };
            var client = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(Math.Max(1, timeoutSeconds))
            };
            return client;
        }

        private static string BuildUrlWithQuery(string url, IDictionary<string, string> queryParams)
        {
            if (queryParams == null || queryParams.Count == 0) return url;
            var sb = new StringBuilder();
            foreach (var kv in queryParams)
            {
                if (sb.Length > 0) sb.Append('&');
                sb.Append(Uri.EscapeDataString(kv.Key ?? ""));
                sb.Append('=');
                sb.Append(Uri.EscapeDataString(kv.Value ?? ""));
            }
            var qs = sb.ToString();
            return url.Contains("?") ? $"{url}&{qs}" : $"{url}?{qs}";
        }

        private static void ApplyHeaders(HttpClient client, IDictionary<string, string>? headers)
        {
            if (headers == null) return;
            foreach (var kv in headers)
            {
                try
                {
                    // 跳过空 key
                    if (string.IsNullOrWhiteSpace(kv.Key)) continue;
                    // TryAddWithoutValidation 允许自定义 header 名称和值
                    client.DefaultRequestHeaders.TryAddWithoutValidation(kv.Key, kv.Value ?? "");
                }
                catch
                {
                    // 忽略单个 header 添加失败，继续添加其它 header
                }
            }
        }

        [Description("发送 GET 请求。参数：url, queryParams, headers, timeoutSeconds, cancellationToken。")]
        public static async Task<string> GetAsync(
            [Description("目标完整 URL 或相对 URL")] string url,
            [Description("查询参数字典，映射为 URL 查询字符串（可为 null）")] IDictionary<string, string>? queryParams = null,
            [Description("自定义请求头字典（可为 null），Key/Value 均为字符串")] IDictionary<string, string>? headers = null,
            [Description("请求超时（秒），最小为 1 秒")] int timeoutSeconds = 30,
            [Description("用于取消请求的 CancellationToken")] CancellationToken cancellationToken = default)
        {
            Console.WriteLine();
            Console.WriteLine($"🔧 正在调用 AgentHttpClientTools.GetAsync -> {url}");
            try
            {
                var fullUrl = BuildUrlWithQuery(url, queryParams);
                using var http = CreateHttpClient(timeoutSeconds);
                ApplyHeaders(http, headers);

                using var resp = await http.GetAsync(fullUrl, cancellationToken).ConfigureAwait(false);
                var text = await resp.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                return text ?? string.Empty;
            }
            catch (Exception ex)
            {
                return $"❌ 请求失败: {ex.Message}";
            }
        }

        [Description("发送 POST 请求。参数：url, body (字符串), contentType, queryParams, headers, timeoutSeconds, cancellationToken。")]
        public static async Task<string> PostAsync(
            [Description("目标完整 URL 或相对 URL")] string url,
            [Description("请求体字符串（通常为 JSON 或表单），可为 null")] string? body = null,
            [Description("Content-Type，默认 \"application/json; charset=utf-8\"")] string contentType = "application/json; charset=utf-8",
            [Description("查询参数字典，映射为 URL 查询字符串（可为 null）")] IDictionary<string, string>? queryParams = null,
            [Description("自定义请求头字典（可为 null），Key/Value 均为字符串")] IDictionary<string, string>? headers = null,
            [Description("请求超时（秒），最小为 1 秒")] int timeoutSeconds = 30,
            [Description("用于取消请求的 CancellationToken")] CancellationToken cancellationToken = default)
        {
            Console.WriteLine();
            Console.WriteLine($"🔧 正在调用 AgentHttpClientTools.PostAsync -> {url}");
            try
            {
                var fullUrl = BuildUrlWithQuery(url, queryParams);
                using var http = CreateHttpClient(timeoutSeconds);
                ApplyHeaders(http, headers);

                using var content = new StringContent(body ?? string.Empty, Encoding.UTF8, contentType.Split(';')[0]);
                // 如果调用方传了完整 contentType 包含 charset，手动设置
                if (!string.IsNullOrWhiteSpace(contentType) && contentType.IndexOf("charset=", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    content.Headers.Remove("Content-Type");
                    content.Headers.TryAddWithoutValidation("Content-Type", contentType);
                }

                using var resp = await http.PostAsync(fullUrl, content, cancellationToken).ConfigureAwait(false);
                var text = await resp.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                return text ?? string.Empty;
            }
            catch (Exception ex)
            {
                return $"❌ 请求失败: {ex.Message}";
            }
        }

        [Description("发送 PUT 请求。参数同 POST。")]
        public static async Task<string> PutAsync(
            [Description("目标完整 URL 或相对 URL")] string url,
            [Description("请求体字符串（通常为 JSON 或表单），可为 null")] string? body = null,
            [Description("Content-Type，默认 \"application/json; charset=utf-8\"")] string contentType = "application/json; charset=utf-8",
            [Description("查询参数字典，映射为 URL 查询字符串（可为 null）")] IDictionary<string, string>? queryParams = null,
            [Description("自定义请求头字典（可为 null），Key/Value 均为字符串")] IDictionary<string, string>? headers = null,
            [Description("请求超时（秒），最小为 1 秒")] int timeoutSeconds = 30,
            [Description("用于取消请求的 CancellationToken")] CancellationToken cancellationToken = default)
        {
            Console.WriteLine();
            Console.WriteLine($"🔧 正在调用 AgentHttpClientTools.PutAsync -> {url}");
            try
            {
                var fullUrl = BuildUrlWithQuery(url, queryParams);
                using var http = CreateHttpClient(timeoutSeconds);
                ApplyHeaders(http, headers);

                using var content = new StringContent(body ?? string.Empty, Encoding.UTF8, contentType.Split(';')[0]);
                if (!string.IsNullOrWhiteSpace(contentType) && contentType.IndexOf("charset=", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    content.Headers.Remove("Content-Type");
                    content.Headers.TryAddWithoutValidation("Content-Type", contentType);
                }

                using var resp = await http.PutAsync(fullUrl, content, cancellationToken).ConfigureAwait(false);
                var text = await resp.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                return text ?? string.Empty;
            }
            catch (Exception ex)
            {
                return $"❌ 请求失败: {ex.Message}";
            }
        }

        [Description("发送 DELETE 请求。支持 queryParams 与 headers。")]
        public static async Task<string> DeleteAsync(
            [Description("目标完整 URL 或相对 URL")] string url,
            [Description("查询参数字典，映射为 URL 查询字符串（可为 null）")] IDictionary<string, string>? queryParams = null,
            [Description("自定义请求头字典（可为 null），Key/Value 均为字符串")] IDictionary<string, string>? headers = null,
            [Description("请求超时（秒），最小为 1 秒")] int timeoutSeconds = 30,
            [Description("用于取消请求的 CancellationToken")] CancellationToken cancellationToken = default)
        {
            Console.WriteLine();
            Console.WriteLine($"🔧 正在调用 AgentHttpClientTools.DeleteAsync -> {url}");
            try
            {
                var fullUrl = BuildUrlWithQuery(url, queryParams);
                using var http = CreateHttpClient(timeoutSeconds);
                ApplyHeaders(http, headers);

                using var resp = await http.DeleteAsync(fullUrl, cancellationToken).ConfigureAwait(false);
                var text = await resp.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                return text ?? string.Empty;
            }
            catch (Exception ex)
            {
                return $"❌ 请求失败: {ex.Message}";
            }
        }
    }
}