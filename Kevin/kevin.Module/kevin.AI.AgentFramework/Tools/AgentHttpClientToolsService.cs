using kevin.AI.AgentFramework.Interfaces;
using kevin.AI.AgentFramework.Interfaces.Tools;
using kevin.AI.AgentFramework.TextLimits;
using System.ComponentModel;
using System.Net;
using System.Text;
using System.Text.Json;

namespace kevin.AI.AgentFramework.Tools
{
    /// <summary>
    /// 给智能体调用的通用 HTTP 工具（支持增删查改：GET/POST/PUT/DELETE）
    /// 可传入 URL、查询参数、Body、以及自定义 Header。
    /// 返回响应文本，发生错误时以 "❌ " 开头的描述。
    /// </summary>
    public class AgentHttpClientToolsService : IAgentHttpClientToolsService
    { 
        public readonly IAIShareInfoService aIShareInfoService;
        public AgentHttpClientToolsService(IAIShareInfoService _aIShareInfoService)
        {
            aIShareInfoService = _aIShareInfoService; 
        } 
        /// <summary>
        /// 请求
        /// </summary>
        /// <param name="timeoutSeconds">超时时间（秒）</param>
        /// <param name="isSkipSSL">是否跳过 SSL 验证</param>
        /// <returns></returns>
        private HttpClient CreateHttpClient(int timeoutSeconds, bool isSkipSSL = true)
        {
            var handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.Brotli,
                AllowAutoRedirect = true
            };
            if (isSkipSSL) handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            var client = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(Math.Max(1, timeoutSeconds))
            };
            return client;
        }

        private string BuildUrlWithQuery(string url, IDictionary<string, string> queryParams)
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

        private void ApplyHeaders(HttpClient client, IDictionary<string, string>? headers)
        {
            if (headers == null) return;
            client.DefaultRequestVersion = new Version("2.0");
            client.DefaultRequestHeaders.Add("Accept", "*/*");
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/95.0.4638.69 Safari/537.36");
            client.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9");
            foreach (var kv in headers)
            {
                // 跳过空 key
                if (string.IsNullOrWhiteSpace(kv.Key)) continue;
                var name = kv.Key.Trim();
                // 名称必须是 RFC 7230 token（纯 ASCII），否则发送阶段一样会失败，这里提前给出可读错误
                if (!IsValidHeaderName(name))
                    throw new ArgumentException($"请求头名称 '{name}' 含非法字符，只允许 ASCII 字母、数字与 !#$%&'*+-.^_`|~");
                // TryAddWithoutValidation 只跳过添加时校验，发送时仍要求值全为 ASCII，需先把非 ASCII 值转成 ASCII
                client.DefaultRequestHeaders.TryAddWithoutValidation(name, ToAsciiHeaderValue(kv.Value));
            }
        }

        /// <summary>
        /// 请求头名称是否合法（RFC 7230 token，纯 ASCII）
        /// </summary>
        private static bool IsValidHeaderName(string name)
        {
            if (string.IsNullOrEmpty(name)) return false;
            const string extraAllowed = "!#$%&'*+-.^_`|~";
            foreach (var c in name)
            {
                if (c > 0x7F) return false;
                if (!char.IsLetterOrDigit(c) && extraAllowed.IndexOf(c) < 0) return false;
            }
            return true;
        }

        /// <summary>
        /// 把请求头值转成纯 ASCII：剔除换行等控制字符，非 ASCII 字符按 UTF-8 逐字节百分号编码
        /// </summary>
        private static string ToAsciiHeaderValue(string? value)
        {
            if (string.IsNullOrEmpty(value)) return "";
            var needConvert = false;
            foreach (var c in value)
            {
                if (c > 0x7F || c == '\r' || c == '\n' || (c < 0x20 && c != '\t'))
                {
                    needConvert = true;
                    break;
                }
            }
            if (!needConvert) return value;

            var sb = new StringBuilder(value.Length + 16);
            for (var i = 0; i < value.Length; i++)
            {
                var c = value[i];
                // 剔除控制字符，避免头部注入与非法字符
                if (c == '\r' || c == '\n' || (c < 0x20 && c != '\t')) continue;
                if (c <= 0x7F)
                {
                    sb.Append(c);
                    continue;
                }
                // 代理对（如 emoji）需整体按 UTF-8 编码
                var len = char.IsHighSurrogate(c) && i + 1 < value.Length && char.IsLowSurrogate(value[i + 1]) ? 2 : 1;
                foreach (var b in Encoding.UTF8.GetBytes(value.Substring(i, len)))
                {
                    sb.Append('%').Append(b.ToString("X2"));
                }
                i += len - 1;
            }
            return sb.ToString();
        }

        private void AuthorizedDomainsCheck(string url)
        { 
            // 域名判定与 run_shell / run_python / 技能脚本共用 CommandGuardrails：
            // 旧实现是 url.Contains(entry)，https://evil.com/?to=授权域名 会因串里出现过授权域名而放行
            if (!CommandGuardrails.IsUrlAuthorized(url, aIShareInfoService.GetData().AuthorizedDomainsList))
                throw new UnauthorizedAccessException($"URL '{url}' 不在授权域名单中。");
        }

        [Description("发送 GET 请求。参数：url, queryParams, headers, timeoutSeconds, cancellationToken。")]
        public async Task<string> GetAsync(
            [Description("目标完整 URL 或相对 URL")] string url,
            [Description("查询参数字典，映射为 URL 查询字符串（可为 null）")] IDictionary<string, string>? queryParams = null,
            [Description("自定义请求头字典（可为 null），Key/Value 均为字符串；Key 必须是纯 ASCII，Value 中的非 ASCII 字符会按 UTF-8 百分号编码发送")] IDictionary<string, string>? headers = null,
            [Description("请求超时（秒），最小为 1 秒,默认三十秒")] int timeoutSeconds = 30,
            [Description("用于取消请求的 CancellationToken")] CancellationToken cancellationToken = default)
        { 
            try
            {
                queryParams ??= new Dictionary<string, string>();
                headers ??= new Dictionary<string, string>();
                AuthorizedDomainsCheck(url);
                var fullUrl = BuildUrlWithQuery(url, queryParams);
                using var http = CreateHttpClient(timeoutSeconds);
                ApplyHeaders(http, headers);

                using var resp = await http.GetAsync(fullUrl, cancellationToken).ConfigureAwait(false);
                var text = await resp.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                return CharLimits.TruncateToolResult(text, aIShareInfoService.GetData().ContentLengthLimit);
            }
            catch (Exception ex)
            {
                return $"❌ 请求失败: {ex.Message}";
            }
        }

        [Description("发送 POST 请求。参数：url, body (字符串), contentType, queryParams, headers, timeoutSeconds, cancellationToken。")]
        public async Task<string> PostAsync(
            [Description("目标完整 URL 或相对 URL")] string url,
            [Description("请求体字符串（通常为 JSON 或表单），可为 null")] string body = "",
            [Description("Content-Type，默认 \"application/json; charset=utf-8\"")] string contentType = "application/json; charset=utf-8",
            [Description("查询参数字典，映射为 URL 查询字符串（可为 null）")] IDictionary<string, string>? queryParams = null,
            [Description("自定义请求头字典（可为 null），Key/Value 均为字符串；Key 必须是纯 ASCII，Value 中的非 ASCII 字符会按 UTF-8 百分号编码发送")] IDictionary<string, string>? headers = null,
            [Description("请求超时（秒），最小为 1 秒")] int timeoutSeconds = 30,
            [Description("用于取消请求的 CancellationToken")] CancellationToken cancellationToken = default)
        { 
            try
            {
                body ??= "";
                queryParams ??= new Dictionary<string, string>();
                headers ??= new Dictionary<string, string>();
                AuthorizedDomainsCheck(url);
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
                var text = await resp.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false) ?? "";
                return CharLimits.TruncateToolResult(text, aIShareInfoService.GetData().ContentLengthLimit);
            }
            catch (Exception ex)
            {
                return $"❌ 请求失败: {ex.Message}";
            }
        }

        [Description("发送 PUT 请求。参数同 POST。")]
        public async Task<string> PutAsync(
            [Description("目标完整 URL 或相对 URL")] string url,
            [Description("请求体字符串（通常为 JSON 或表单），可为 null")] string? body = null,
            [Description("Content-Type，默认 \"application/json; charset=utf-8\"")] string contentType = "application/json; charset=utf-8",
            [Description("查询参数字典，映射为 URL 查询字符串（可为 null）")] IDictionary<string, string>? queryParams = null,
            [Description("自定义请求头字典（可为 null），Key/Value 均为字符串；Key 必须是纯 ASCII，Value 中的非 ASCII 字符会按 UTF-8 百分号编码发送")] IDictionary<string, string>? headers = null,
            [Description("请求超时（秒），最小为 1 秒")] int timeoutSeconds = 30,
            [Description("用于取消请求的 CancellationToken")] CancellationToken cancellationToken = default)
        { 
            try
            {
                body ??= "";
                queryParams ??= new Dictionary<string, string>();
                headers ??= new Dictionary<string, string>();
                AuthorizedDomainsCheck(url);
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
                var text = await resp.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false) ?? "";
                return CharLimits.TruncateToolResult(text, aIShareInfoService.GetData().ContentLengthLimit);
            }
            catch (Exception ex)
            {
                return $"❌ 请求失败: {ex.Message}";
            }
        }

        [Description("发送 DELETE 请求。支持 queryParams 与 headers。")]
        public async Task<string> DeleteAsync(
            [Description("目标完整 URL 或相对 URL")] string url,
            [Description("查询参数字典，映射为 URL 查询字符串（可为 null）")] IDictionary<string, string>? queryParams = null,
            [Description("自定义请求头字典（可为 null），Key/Value 均为字符串；Key 必须是纯 ASCII，Value 中的非 ASCII 字符会按 UTF-8 百分号编码发送")] IDictionary<string, string>? headers = null,
            [Description("请求超时（秒），最小为 1 秒")] int timeoutSeconds = 30,
            [Description("用于取消请求的 CancellationToken")] CancellationToken cancellationToken = default)
        { 
            try
            {
                queryParams ??= new Dictionary<string, string>();
                headers ??= new Dictionary<string, string>();
                AuthorizedDomainsCheck(url);
                var fullUrl = BuildUrlWithQuery(url, queryParams);
                using var http = CreateHttpClient(timeoutSeconds);
                ApplyHeaders(http, headers);

                using var resp = await http.DeleteAsync(fullUrl, cancellationToken).ConfigureAwait(false);
                var text = await resp.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false) ?? "";
                return CharLimits.TruncateToolResult(text, aIShareInfoService.GetData().ContentLengthLimit);
            }
            catch (Exception ex)
            {
                return $"❌ 请求失败: {ex.Message}";
            }
        }
    }
}