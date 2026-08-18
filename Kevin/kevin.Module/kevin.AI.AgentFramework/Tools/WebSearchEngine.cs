using kevin.AI.AgentFramework.Interfaces.Tools;
using Microsoft.Extensions.Configuration;
using System.ComponentModel;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace kevin.AI.AgentFramework.Tools
{
    /// <summary>
    /// 火山引擎豆包联网搜索引擎（Custom版 / Global版）
    /// Custom版文档：https://docs.volcengine.com/docs/87772/2272953
    /// Global版文档：https://docs.volcengine.com/docs/87772/2548026
    /// 鉴权方式：API Key，HTTP Header Authorization: Bearer &lt;API_KEY&gt;
    /// </summary>
    public class WebSearchEngine : IWebSearchEngine
    {
        /// <summary>
        /// 豆包搜索Custom版（APIKey接入）地址
        /// </summary>
        private const string CustomSearchUrl = "https://open.feedcoopapi.com/search_api/web_search";
        /// <summary>
        /// 豆包搜索Global版（APIKey接入）地址
        /// </summary>
        private const string GlobalSearchUrl = "https://open.feedcoopapi.com/search_api/global_search";

        private static readonly HttpClient _httpClient = new HttpClient() { Timeout = TimeSpan.FromSeconds(30) };

        private readonly DoubaoSearchApiSetting _setting;

        public WebSearchEngine(IConfiguration configuration)
        {
            _setting = configuration.GetSection("DoubaoSearchApiSetting").Get<DoubaoSearchApiSetting>() ?? new DoubaoSearchApiSetting();
        }

        /// <summary>
        /// 豆包搜索Global版
        /// </summary>
        /// <param name="query">关键词</param>
        /// <returns></returns>
        [Description("豆包联网搜索Global版本，覆盖全球站点，摘要长度可灵活控制，综合搜索效果更好。参数：query")]
        public async Task<string> DoubaoSearchGlobalAsync([Description("关键词")] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return "❌ 搜索失败: query 不能为空";

            var apiKey = !string.IsNullOrWhiteSpace(_setting.GlobalApiKey) ? _setting.GlobalApiKey : _setting.ApiKey;
            if (string.IsNullOrWhiteSpace(apiKey))
                return "❌ 搜索失败: 未配置豆包搜索ApiKey（配置节点 DoubaoSearchApiSetting）";

            Console.WriteLine();
            Console.WriteLine($"🔧 正在调用 DoubaoSearchGlobalAsync，关键词：{query}");
            try
            {
                var body = new
                {
                    Query = query,
                    SearchType = "web",
                    DocCount = Math.Clamp(_setting.DocCount, 1, 20), // Global版返回条数上限20
                    MaxSnippetLength = _setting.MaxSnippetLength
                };
                var json = await PostSearchAsync(GlobalSearchUrl, apiKey, body);
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                // 接口层错误（鉴权失败、限流等）
                var apiError = GetResponseMetadataError(root);
                if (!string.IsNullOrEmpty(apiError))
                    return $"❌ 豆包搜索(Global版)接口错误: {apiError}";

                if (!root.TryGetProperty("Result", out var result) || result.ValueKind == JsonValueKind.Null)
                    return $"❌ 豆包搜索(Global版)未返回搜索结果，关键词：{query}";

                // 业务错误码
                if (result.TryGetProperty("ErrorCode", out var errorCode) && errorCode.ValueKind == JsonValueKind.Number && errorCode.GetInt64() != 0)
                {
                    var errorMsg = result.TryGetProperty("ErrorMsg", out var m) && m.ValueKind == JsonValueKind.String ? m.GetString() : "";
                    return $"❌ 豆包搜索(Global版)失败: ErrorCode={errorCode.GetInt64()}，ErrorMsg={errorMsg}";
                }

                if (!result.TryGetProperty("Documents", out var documents) || documents.ValueKind != JsonValueKind.Array || documents.GetArrayLength() == 0)
                    return $"未找到与「{query}」相关的搜索结果";

                var sb = new StringBuilder();
                sb.AppendLine($"关键词「{query}」的搜索结果，共 {documents.GetArrayLength()} 条：");
                var index = 1;
                foreach (var item in documents.EnumerateArray())
                {
                    var title = GetStringProperty(item, "Title");
                    var url = GetStringProperty(item, "Url", "Link");
                    var publishTime = GetPublishTime(item);
                    var snippet = GetGlobalSnippet(item);
                    sb.AppendLine($"[{index}] {title}");
                    if (!string.IsNullOrWhiteSpace(url))
                        sb.AppendLine($"链接: {url}");
                    if (!string.IsNullOrWhiteSpace(publishTime))
                        sb.AppendLine($"发布时间: {publishTime}");
                    if (!string.IsNullOrWhiteSpace(snippet))
                        sb.AppendLine($"摘要: {snippet}");
                    sb.AppendLine();
                    index++;
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                return $"❌ 豆包搜索(Global版)调用失败: {ex.Message}";
            }
        }

        /// <summary>
        /// 豆包搜索Custom版
        /// </summary>
        /// <param name="query">关键词</param>
        /// <returns></returns>
        [Description("豆包联网搜索Custom版本，时延低，控制更灵活，支持各行业高频搜索需求。参数：query")]
        public async Task<string> DoubaoSearchCustomAsync([Description("关键词")] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return "❌ 搜索失败: query 不能为空";

            var apiKey = !string.IsNullOrWhiteSpace(_setting.CustomApiKey) ? _setting.CustomApiKey : _setting.ApiKey;
            if (string.IsNullOrWhiteSpace(apiKey))
                return "❌ 搜索失败: 未配置豆包搜索ApiKey（配置节点 DoubaoSearchApiSetting）";

            Console.WriteLine();
            Console.WriteLine($"🔧 正在调用 DoubaoSearchCustomAsync，关键词：{query}");
            try
            {
                var body = new
                {
                    Query = query,
                    SearchType = "web",
                    DocCount = Math.Clamp(_setting.DocCount, 1, 50) // Custom版返回条数上限50
                };
                var json = await PostSearchAsync(CustomSearchUrl, apiKey, body);
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                // 接口层错误（鉴权失败、限流等）
                var apiError = GetResponseMetadataError(root);
                if (!string.IsNullOrEmpty(apiError))
                    return $"❌ 豆包搜索(Custom版)接口错误: {apiError}";

                if (!root.TryGetProperty("Result", out var result) || result.ValueKind == JsonValueKind.Null)
                    return $"❌ 豆包搜索(Custom版)未返回搜索结果，关键词：{query}";

                // 业务错误码
                if (result.TryGetProperty("ErrorCode", out var errorCode) && errorCode.ValueKind == JsonValueKind.Number && errorCode.GetInt64() != 0)
                {
                    var errorMsg = result.TryGetProperty("ErrorMsg", out var m) && m.ValueKind == JsonValueKind.String ? m.GetString() : "";
                    return $"❌ 豆包搜索(Custom版)失败: ErrorCode={errorCode.GetInt64()}，ErrorMsg={errorMsg}";
                }

                // Custom版web搜索结果列表字段为 WebResults，每条含 Title/Url/SiteName/Snippet/Summary/Content/PublishTime/AuthInfoDes/RankScore 等
                if (!result.TryGetProperty("WebResults", out var webResults) || webResults.ValueKind != JsonValueKind.Array || webResults.GetArrayLength() == 0)
                    return $"未找到与「{query}」相关的搜索结果";

                var sb = new StringBuilder();
                sb.AppendLine($"关键词「{query}」的搜索结果，共 {webResults.GetArrayLength()} 条：");
                var index = 1;
                foreach (var item in webResults.EnumerateArray())
                {
                    var title = GetStringProperty(item, "Title");
                    var url = GetStringProperty(item, "Url", "Link");
                    var publishTime = GetStringProperty(item, "PublishTime", "PublishedTime");
                    var siteName = GetStringProperty(item, "SiteName", "Site");
                    var authInfo = GetStringProperty(item, "AuthInfoDes");
                    var snippet = GetCustomSnippet(item);
                    sb.AppendLine($"[{index}] {title}");
                    if (!string.IsNullOrWhiteSpace(url))
                        sb.AppendLine($"链接: {url}");
                    if (!string.IsNullOrWhiteSpace(siteName))
                        sb.AppendLine($"来源: {siteName}");
                    if (!string.IsNullOrWhiteSpace(publishTime))
                        sb.AppendLine($"发布时间: {publishTime}");
                    if (!string.IsNullOrWhiteSpace(authInfo))
                        sb.AppendLine($"权威度: {authInfo}");
                    if (!string.IsNullOrWhiteSpace(snippet))
                        sb.AppendLine($"摘要: {snippet}");
                    sb.AppendLine();
                    index++;
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                return $"❌ 豆包搜索(Custom版)调用失败: {ex.Message}";
            }
        }

        /// <summary>
        /// 发起豆包搜索POST请求（Bearer鉴权）
        /// </summary>
        private static async Task<string> PostSearchAsync(string url, string apiKey, object body)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            request.Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
            using var response = await _httpClient.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"HTTP {(int)response.StatusCode}: {Truncate(json, 200)}");
            return json;
        }

        /// <summary>
        /// 提取 ResponseMetadata.Error 中的接口层错误信息
        /// </summary>
        private static string GetResponseMetadataError(JsonElement root)
        {
            if (root.TryGetProperty("ResponseMetadata", out var metadata)
                && metadata.ValueKind == JsonValueKind.Object
                && metadata.TryGetProperty("Error", out var error)
                && error.ValueKind == JsonValueKind.Object)
            {
                var code = GetStringProperty(error, "Code", "CodeN");
                var message = GetStringProperty(error, "Message");
                return $"Code={code}，Message={message}";
            }
            return null;
        }

        /// <summary>
        /// 依次尝试多个属性名获取字符串值
        /// </summary>
        private static string GetStringProperty(JsonElement element, params string[] names)
        {
            if (element.ValueKind != JsonValueKind.Object)
                return null;
            foreach (var name in names)
            {
                if (element.TryGetProperty(name, out var value))
                {
                    if (value.ValueKind == JsonValueKind.String)
                        return value.GetString();
                    if (value.ValueKind == JsonValueKind.Number)
                        return value.GetRawText();
                }
            }
            return null;
        }

        /// <summary>
        /// 提取Global版摘要：Snippet 为片段数组（元素含 Text 字段）或字符串
        /// </summary>
        private static string GetGlobalSnippet(JsonElement item)
        {
            if (!item.TryGetProperty("Snippet", out var snippet))
                return null;
            if (snippet.ValueKind == JsonValueKind.String)
                return snippet.GetString();
            if (snippet.ValueKind == JsonValueKind.Array)
            {
                var texts = new List<string>();
                foreach (var part in snippet.EnumerateArray())
                {
                    if (part.ValueKind == JsonValueKind.String)
                        texts.Add(part.GetString());
                    else if (part.ValueKind == JsonValueKind.Object)
                    {
                        var text = GetStringProperty(part, "Text", "Content");
                        if (!string.IsNullOrWhiteSpace(text))
                            texts.Add(text);
                    }
                }
                return string.Join(" ... ", texts.Where(t => !string.IsNullOrWhiteSpace(t)));
            }
            return null;
        }

        /// <summary>
        /// 提取Custom版摘要：优先较短的 Snippet，缺失时回退到 Summary 并截断（Content 为超长全文，不默认返回以免浪费Token）
        /// </summary>
        private static string GetCustomSnippet(JsonElement item)
        {
            var snippet = GetStringProperty(item, "Snippet");
            if (!string.IsNullOrWhiteSpace(snippet))
                return snippet.Replace("\n", " ").Trim();
            var summary = GetStringProperty(item, "Summary");
            if (!string.IsNullOrWhiteSpace(summary))
                return Truncate(summary.Replace("\n", " ").Trim(), 500);
            return null;
        }

        /// <summary>
        /// 提取Global版发布时间：可能在文档根级或 DocumentInfo 中
        /// </summary>
        private static string GetPublishTime(JsonElement item)
        {
            var publishTime = GetStringProperty(item, "PublishTime", "PublishedTime", "PublishDate");
            if (!string.IsNullOrWhiteSpace(publishTime))
                return publishTime;
            if (item.TryGetProperty("DocumentInfo", out var documentInfo))
                return GetStringProperty(documentInfo, "PublishTime", "PublishedTime", "PublishDate");
            return null;
        }

        private static string Truncate(string text, int maxLength)
        {
            if (string.IsNullOrEmpty(text))
                return text;
            return text.Length <= maxLength ? text : text[..maxLength] + "...";
        }
    }

    /// <summary>
    /// 豆包搜索配置（appsettings 节点：DoubaoSearchApiSetting）
    /// </summary>
    public class DoubaoSearchApiSetting
    {
        /// <summary>
        /// 默认API Key（Global版与Custom版共用）
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Global版API Key，配置后优先于 ApiKey（Global版仅支持按量后付费Key）
        /// </summary>
        public string GlobalApiKey { get; set; }

        /// <summary>
        /// Custom版API Key，配置后优先于 ApiKey（Custom版按量后付费、订阅套餐Key相互独立）
        /// </summary>
        public string CustomApiKey { get; set; }

        /// <summary>
        /// 返回结果条数（Global版上限20，Custom版上限50）
        /// </summary>
        public int DocCount { get; set; } = 10;

        /// <summary>
        /// Global版摘要最大长度
        /// </summary>
        public int MaxSnippetLength { get; set; } = 300;
    }
}
