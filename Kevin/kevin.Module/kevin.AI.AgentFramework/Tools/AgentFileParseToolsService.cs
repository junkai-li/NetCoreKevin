using kevin.AI.AgentFramework.Interfaces;
using kevin.AI.AgentFramework.Interfaces.Tools;
using kevin.AI.AgentFramework.Modality;
using kevin.AI.AgentFramework.TextLimits;
using kevin.Domain.Share.Enums;
using System.ComponentModel;

namespace kevin.AI.AgentFramework.Tools
{
    /// <summary>
    /// 给智能体调用的远程文件解析工具：下载 URL 指向的文档并解析为纯文本。
    /// 解析管线与上传附件/知识库共用 <see cref="IModalityContentBuilder"/>，支持 PDF/Word/Excel/PPT/HTML/Markdown/文本。
    /// 返回解析后的文本内容，发生错误时以 "❌ " 开头。
    /// </summary>
    public class AgentFileParseToolsService : IAgentFileParseToolsService
    {
        private readonly IAIShareInfoService _aIShareInfoService;
        private readonly IModalityContentBuilder _modalityContentBuilder;

        public AgentFileParseToolsService(IAIShareInfoService aIShareInfoService, IModalityContentBuilder modalityContentBuilder)
        {
            _aIShareInfoService = aIShareInfoService;
            _modalityContentBuilder = modalityContentBuilder;
        }

        [Description("解析远程文件。下载指定 URL 的文件并解析内容，支持 PDF、Word、Excel、PPT、HTML、Markdown、文本等文档类型；当用户提供文件链接并需要读取、总结、分析文件内容时调用。")]
        public async Task<string> ParseRemoteFileAsync(
            [Description("要解析的远程文件完整 URL 不能为 null")] string url,
            [Description("文件名（含扩展名，如 xx.pdf），不传时从 URL 提取；扩展名决定解析方式")] string? fileName = null,
            [Description("用于取消请求的 CancellationToken")] CancellationToken cancellationToken = default)
        {
            try
            {
                // 域名护栏与 HTTP 工具一致：URL 必须落在智能体授权域名名单内
                if (!CommandGuardrails.IsUrlAuthorized(url, _aIShareInfoService.GetData().AuthorizedDomainsList))
                    return CommandGuardrails.UnauthorizedDomainMessage(url);

                var result = await _modalityContentBuilder.BuildAsync(url, fileName, cancellationToken);

                if (!string.IsNullOrEmpty(result.Error))
                    return $"❌ 解析失败: {result.Error}";

                // 图片/音频走媒体通道不产出文本，明确告知模型该工具用不了
                if (result.Kind == ModalityKind.Image || result.Kind == ModalityKind.Audio)
                    return $"❌ 解析失败: 该文件是{(result.Kind == ModalityKind.Image ? "图片" : "音频")}类型，不是文档，无法解析为文本。";

                if (string.IsNullOrEmpty(result.ExtractedText))
                    return "❌ 解析失败: 未能从文件中提取到文本内容。";

                return CharLimits.TruncateToolResult(result.ExtractedText, _aIShareInfoService.GetData().ContentLengthLimit);
            }
            catch (OperationCanceledException)
            {
                throw; // 取消向上传播，让主流程走 CancellationToken 中止路径
            }
            catch (Exception ex)
            {
                return $"❌ 解析失败: {ex.Message}";
            }
        }
    }
}
