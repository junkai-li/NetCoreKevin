using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace kevin.AI.AgentFramework.Interfaces.Tools
{
    /// <summary>
    /// 给智能体调用的通用 HTTP 工具（支持增删查改：GET/POST/PUT/DELETE）
    /// 可传入 URL、查询参数、Body、以及自定义 Header。
    /// 返回响应文本，发生错误时以 "❌ " 开头的描述。
    /// </summary>
    public interface IAgentHttpClientToolsService : IBaseAIToolService
    {

        [Description("发送 GET 请求。参数：url, queryParams, headers, timeoutSeconds, cancellationToken。")]
        Task<string> GetAsync(
            [Description("目标完整 URL 或相对 URL")] [Required] string url,
            [Description("查询参数字典，映射为 URL 查询字符串（可为 null）")] IDictionary<string, string> queryParams = null,
            [Description("自定义请求头字典（可为 null），Key/Value 均为字符串")] IDictionary<string, string>? headers = null,
            [Description("请求超时（秒），最小为 1 秒,默认三十秒")] int timeoutSeconds = 30,
            [Description("用于取消请求的 CancellationToken")] CancellationToken cancellationToken = default);

        [Description("发送 POST 请求。参数：url, body (字符串), contentType, queryParams, headers, timeoutSeconds, cancellationToken。")]
        Task<string> PostAsync(
           [Description("目标完整 URL 或相对 URL")][Required] string url,
           [Description("请求体字符串（通常为 JSON 或表单），可为 null")] string? body = null,
           [Description("Content-Type，默认 \"application/json; charset=utf-8\"")] string contentType = "application/json; charset=utf-8",
           [Description("查询参数字典，映射为 URL 查询字符串（可为 null）")] IDictionary<string, string>? queryParams = null,
           [Description("自定义请求头字典（可为 null），Key/Value 均为字符串")] IDictionary<string, string>? headers = null,
           [Description("请求超时（秒），最小为 1 秒,默认三十秒")] int timeoutSeconds = 30,
           [Description("用于取消请求的 CancellationToken")] CancellationToken cancellationToken = default);


        [Description("发送 PUT 请求。参数同 POST。")]
        Task<string> PutAsync(
           [Description("目标完整 URL 或相对 URL")] [Required] string url,
           [Description("请求体字符串（通常为 JSON 或表单），可为 null")] string? body = null,
           [Description("Content-Type，默认 \"application/json; charset=utf-8\"")] string contentType = "application/json; charset=utf-8",
           [Description("查询参数字典，映射为 URL 查询字符串（可为 null）")] IDictionary<string, string>? queryParams = null,
           [Description("自定义请求头字典（可为 null），Key/Value 均为字符串")] IDictionary<string, string>? headers = null,
           [Description("请求超时（秒），最小为 1 秒")] int timeoutSeconds = 30,
           [Description("用于取消请求的 CancellationToken")] CancellationToken cancellationToken = default);

        [Description("发送 DELETE 请求。支持 queryParams 与 headers。")]
        Task<string> DeleteAsync(
            [Description("目标完整 URL 或相对 URL")][Required] string url,
            [Description("查询参数字典，映射为 URL 查询字符串（可为 null）")] IDictionary<string, string>? queryParams = null,
            [Description("自定义请求头字典（可为 null），Key/Value 均为字符串")] IDictionary<string, string>? headers = null,
            [Description("请求超时（秒），最小为 1 秒")] int timeoutSeconds = 30,
            [Description("用于取消请求的 CancellationToken")] CancellationToken cancellationToken = default);
    }
}
