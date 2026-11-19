using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace kevin.AI.AgentFramework.Interfaces.Tools
{
    /// <summary>
    /// 给智能体调用的远程文件解析工具：下载 URL 指向的文档并解析为纯文本。
    /// 复用与上传附件/知识库相同的解析管线（PDF/Word/Excel/PPT/HTML/Markdown/文本）。
    /// 返回解析后的文本内容，发生错误时以 "❌ " 开头。
    /// </summary>
    public interface IAgentFileParseToolsService
    {
        [Description("解析远程文件。参数：url, fileName, cancellationToken。")]
        Task<string> ParseRemoteFileAsync(
            [Description("要解析的远程文件完整 URL 不能为 null")][Required] string url,
            [Description("文件名（含扩展名，如 xx.pdf），不传时从 URL 提取；扩展名决定解析方式")] string? fileName = null,
            [Description("用于取消请求的 CancellationToken")] CancellationToken cancellationToken = default);
    }
}
