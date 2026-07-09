using Kevin.Common.Helper;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace kevin.AI.AgentFramework.Tools
{
    /// <summary>
    /// You.com 联网搜索配置
    /// </summary>
    public class YouComSetting
    {
        /// <summary>
        /// You.com API密钥
        /// </summary>
        public string? ApiKey { get; set; }

        /// <summary>
        /// You.com 搜索接口地址
        /// </summary>
        public string? Url { get; set; }

        /// <summary>
        /// 返回结果数量
        /// </summary>
        public int Count { get; set; }
    }

    /// <summary>
    /// You.com 结构化联网搜索提供程序。
    /// 相比现有的HTML抓取+LLM总结方案，You.com直接返回结构化搜索结果，
    /// 更稳定、更准确。仅当appsettings中配置了YouCom:ApiKey时才启用，
    /// 未配置或调用失败时返回空字符串，由调用方回退到原有抓取逻辑。
    /// </summary>
    public static class YouComSearchProvider
    {
        private const string SectionKey = "YouCom";
        private const int MaxCount = 20;
        private const int DefaultCount = 10;

        /// <summary>
        /// 是否已启用You.com搜索（ApiKey非空）
        /// </summary>
        public static bool IsEnabled
        {
            get
            {
                var setting = GetSetting();
                return setting != null && !string.IsNullOrWhiteSpace(setting.ApiKey);
            }
        }

        /// <summary>
        /// 调用You.com搜索接口并返回格式化后的搜索结果文本。
        /// 任何异常或空结果都会被吞掉并返回空字符串，确保调用方可以安全回退。
        /// </summary>
        /// <param name="query">用户搜索意图问题</param>
        public static async Task<string> SearchAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return string.Empty;
            }

            var setting = GetSetting();
            if (setting == null || string.IsNullOrWhiteSpace(setting.ApiKey) || string.IsNullOrWhiteSpace(setting.Url))
            {
                return string.Empty;
            }

            var count = setting.Count <= 0 ? DefaultCount : Math.Min(setting.Count, MaxCount);

            try
            {
                var handler = new HttpClientHandler()
                {
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.Brotli,
                };

                using var http = new HttpClient(handler);
                http.Timeout = TimeSpan.FromSeconds(10);
                // 注意：ApiKey仅用于请求头，绝不打印或记录
                http.DefaultRequestHeaders.Add("X-API-Key", setting.ApiKey);

                var requestUrl = $"{setting.Url}?query={Uri.EscapeDataString(query)}&count={count}";
                var response = await http.GetAsync(requestUrl).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                {
                    return string.Empty;
                }

                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return FormatResults(json, count);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"You.com联网搜索请求失败: {ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// 纯函数：将You.com接口返回的原始JSON映射为结构化的搜索结果文本。
        /// 不涉及网络请求，便于单元测试。任何解析失败或空结果都返回空字符串。
        /// </summary>
        /// <param name="json">You.com接口返回的原始JSON字符串</param>
        /// <param name="count">最多展示的结果条数（会被限制在MaxCount以内）</param>
        public static string FormatResults(string json, int count)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return string.Empty;
            }

            YouComSearchResponse? data;
            try
            {
                data = JsonSerializer.Deserialize<YouComSearchResponse>(json, JsonOptions);
            }
            catch (JsonException)
            {
                return string.Empty;
            }

            var webResults = data?.Results?.Web;
            if (webResults == null || webResults.Count == 0)
            {
                return string.Empty;
            }

            var effectiveCount = count <= 0 ? DefaultCount : Math.Min(count, MaxCount);
            effectiveCount = Math.Min(effectiveCount, webResults.Count);

            var sb = new System.Text.StringBuilder();
            for (var i = 0; i < effectiveCount; i++)
            {
                var item = webResults[i];
                var title = string.IsNullOrWhiteSpace(item.Title) ? "无标题" : item.Title;
                sb.AppendLine($"{i + 1}. {title}");

                if (!string.IsNullOrWhiteSpace(item.Url))
                {
                    sb.AppendLine($"链接: {item.Url}");
                }

                if (!string.IsNullOrWhiteSpace(item.Description))
                {
                    sb.AppendLine($"描述: {item.Description}");
                }

                if (item.Snippets != null && item.Snippets.Count > 0)
                {
                    var validSnippets = item.Snippets.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
                    if (validSnippets.Count > 0)
                    {
                        sb.AppendLine("摘要:");
                        foreach (var snippet in validSnippets)
                        {
                            sb.AppendLine($"- {snippet}");
                        }
                    }
                }

                sb.AppendLine();
            }

            sb.Append("信息来源: You.com");

            return sb.ToString();
        }

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };

        private static YouComSetting? GetSetting()
        {
            if (ConfigHelper.Configuration == null)
            {
                return null;
            }

            try
            {
                return ConfigHelper.GetSection<YouComSetting>(SectionKey);
            }
            catch
            {
                return null;
            }
        }

        private class YouComSearchResponse
        {
            [JsonPropertyName("results")]
            public YouComResults? Results { get; set; }
        }

        private class YouComResults
        {
            [JsonPropertyName("web")]
            public List<YouComWebResult>? Web { get; set; }
        }

        private class YouComWebResult
        {
            [JsonPropertyName("title")]
            public string? Title { get; set; }

            [JsonPropertyName("url")]
            public string? Url { get; set; }

            [JsonPropertyName("description")]
            public string? Description { get; set; }

            [JsonPropertyName("snippets")]
            public List<string>? Snippets { get; set; }
        }
    }
}
