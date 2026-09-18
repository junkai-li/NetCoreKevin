using kevin.AI.AgentFramework.Dto;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace kevin.AI.AgentFramework.ImageGeneration
{
    /// <summary>
    /// OpenAI-compat 文生图客户端实现。
    /// <para>
    /// POST <c>{endpoint}/images/generations</c>，请求体：<c>{ model, prompt, n, size, quality, response_format }</c>，
    /// 响应体：<c>{ created, data: [{ url?, b64_json?, revised_prompt? }] }</c>。
    /// </para>
    /// <para>
    /// 覆盖：OpenAI DALL-E 3、智谱 CogView-3-Flash、通义万相 compatible-mode、SiliconFlow Kolors。
    /// DashScope 原生 wanx-v1（异步任务模式）不兼容，本次不覆盖。
    /// </para>
    /// <para>
    /// 优先请求 <c>b64_json</c> 而不是 <c>url</c>：OpenAI 官方 URL 仅 1h 有效、通义万相 24h，
    /// 拿到字节后由上层 <c>ImageGenToolFactory</c> 转存 OSS，返回稳定链接。
    /// </para>
    /// </summary>
    public class OpenAICompatImageGenerationClient : IImageGenerationClient
    {
        private readonly IHttpClientFactory _httpClientFactory;

        /// <summary>
        /// 构造注入 <see cref="IHttpClientFactory"/> 而不是自建静态 HttpClient：
        /// 文生图端点由 <see cref="ImageGenModelConfig"/> 每次调用动态决定，静态客户端无法覆盖多厂商场景，
        /// 而 IHttpClientFactory 天然按 name 管理连接池，不会端口耗尽。
        /// </summary>
        public OpenAICompatImageGenerationClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        /// <inheritdoc />
        public async Task<ImageGenerationResult> GenerateAsync(
            ImageGenModelConfig config,
            ImageGenerationRequest request,
            CancellationToken cancellationToken = default)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));
            if (string.IsNullOrWhiteSpace(config.Endpoint)) throw new ArgumentException("文生图模型 EndPoint 未配置", nameof(config));
            if (string.IsNullOrWhiteSpace(request.Prompt)) throw new ArgumentException("prompt 不能为空", nameof(request));

            var options = ImageGenerationOptions.Current;
            var url = BuildRequestUrl(config.Endpoint);

            var client = _httpClientFactory.CreateClient(nameof(OpenAICompatImageGenerationClient));
            client.Timeout = TimeSpan.FromSeconds(options.HttpTimeoutSeconds);

            using var httpReq = new HttpRequestMessage(HttpMethod.Post, url);
            if (!string.IsNullOrWhiteSpace(config.ApiKey))
            {
                httpReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", config.ApiKey);
            }
            var payload = new ImageGenRequestPayload
            {
                Model = string.IsNullOrWhiteSpace(request.Model) ? config.ModelName : request.Model,
                Prompt = request.Prompt,
                N = request.N > 0 ? request.N : options.DefaultN,
                Size = string.IsNullOrWhiteSpace(request.Size) ? options.DefaultSize : request.Size,
                Quality = string.IsNullOrWhiteSpace(request.Quality) ? options.DefaultQuality : request.Quality,
                ResponseFormat = string.IsNullOrWhiteSpace(request.ResponseFormat) ? "b64_json" : request.ResponseFormat
            };
            httpReq.Content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json");

            using var httpResp = await client.SendAsync(httpReq, cancellationToken);
            var body = await httpResp.Content.ReadAsStringAsync(cancellationToken);
            if (!httpResp.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"文生图接口返回 {(int)httpResp.StatusCode}: {TruncateForLog(body)}");
            }

            var parsed = ParseResponse(body);
            if (parsed.Images.Count == 0)
            {
                throw new InvalidOperationException($"文生图接口返回空 data 列表: {TruncateForLog(body)}");
            }
            return parsed;
        }

        /// <summary>
        /// 拼接请求 URL：endpoint 可能带或不带尾部斜杠，也可能已含 /images/generations 完整路径，做兼容处理
        /// </summary>
        private static string BuildRequestUrl(string endpoint)
        {
            var trimmed = endpoint.TrimEnd('/');
            if (trimmed.EndsWith("/images/generations", StringComparison.OrdinalIgnoreCase))
            {
                return trimmed;
            }
            // 兼容 "https://api.openai.com/v1" 与 "https://api.openai.com/v1/" 两种写法
            return trimmed + "/images/generations";
        }

        /// <summary>
        /// 解析 OpenAI-compat 响应体：{ created, data: [{ url?, b64_json?, revised_prompt? }] }
        /// </summary>
        private static ImageGenerationResult ParseResponse(string body)
        {
            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;
            var images = new List<GeneratedImage>();
            if (root.TryGetProperty("data", out var dataEl) && dataEl.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in dataEl.EnumerateArray())
                {
                    string? url = item.TryGetProperty("url", out var urlEl) && urlEl.ValueKind == JsonValueKind.String
                        ? urlEl.GetString()
                        : null;
                    byte[]? bytes = null;
                    if (item.TryGetProperty("b64_json", out var b64El) && b64El.ValueKind == JsonValueKind.String)
                    {
                        var b64 = b64El.GetString();
                        if (!string.IsNullOrWhiteSpace(b64))
                        {
                            try { bytes = Convert.FromBase64String(b64!); }
                            catch (FormatException) { bytes = null; }
                        }
                    }
                    string? revised = item.TryGetProperty("revised_prompt", out var rpEl) && rpEl.ValueKind == JsonValueKind.String
                        ? rpEl.GetString()
                        : null;
                    if (url == null && bytes == null) continue;
                    images.Add(new GeneratedImage(url, bytes, bytes != null ? "image/png" : null, revised));
                }
            }
            return new ImageGenerationResult(images);
        }

        /// <summary>
        /// 日志/异常里裁剪响应体，防止超长 HTML 错误页把日志撑爆
        /// </summary>
        private static string TruncateForLog(string body, int max = 500)
        {
            if (string.IsNullOrEmpty(body)) return "";
            return body.Length <= max ? body : body.Substring(0, max) + "...(truncated)";
        }

        /// <summary>
        /// 请求体 DTO：用 <see cref="JsonPropertyNameAttribute"/> 显式绑定 snake_case 字段名，
        /// 避免依赖全局命名策略（AI 中台其他接口用 camelCase，文生图 API 是 snake_case）。
        /// 直接用反射序列化：文生图单次调用本身耗时数秒到数十秒，反射开销可忽略；
        /// 且 DTO 上已用 <c>[JsonPropertyName]</c> 精确控制字段名，不需要 source-gen 的 naming policy。
        /// </summary>
        private sealed class ImageGenRequestPayload
        {
            [JsonPropertyName("model")] public string Model { get; set; } = "";
            [JsonPropertyName("prompt")] public string Prompt { get; set; } = "";
            [JsonPropertyName("n")] public int N { get; set; } = 1;
            [JsonPropertyName("size")] public string Size { get; set; } = "1024x1024";
            [JsonPropertyName("quality")] public string Quality { get; set; } = "standard";
            [JsonPropertyName("response_format")] public string ResponseFormat { get; set; } = "b64_json";
        }
    }
}
