using kevin.AI.AgentFramework.Dto;
using kevin.AI.AgentFramework.Interfaces;
using kevin.AI.AgentFramework.Interfaces.Tools;
using kevin.AI.AgentFramework.Modality;
using kevin.AI.AgentFramework.TextLimits;
using kevin.Domain.Share.Enums;
using Microsoft.Extensions.AI;
using OpenAI;
using System.ClientModel;
using System.ComponentModel;

namespace kevin.AI.AgentFramework.Tools
{
    /// <summary>
    /// 给智能体调用的远程文件解析工具：下载 URL 指向的文档并解析为纯文本；
    /// 图片/音频则调用多模态模型解析为文字描述/转录。
    /// 解析管线与上传附件/知识库共用 <see cref="IModalityContentBuilder"/>，支持 PDF/Word/Excel/PPT/HTML/Markdown/文本/图片/音频。
    /// 返回解析后的文本内容，发生错误时以 "❌ " 开头。
    /// </summary>
    public class AgentFileParseToolsService : IAgentFileParseToolsService
    {
        private readonly IAIShareInfoService _aIShareInfoService;
        private readonly IModalityContentBuilder _modalityContentBuilder;

        /// <summary>图片解析提示词：文字逐字转录/表格图表结构数据/主体场景描述</summary>
        private const string ImageParsePrompt =
            "你是一个图片内容解析助手。请详细描述这张图片的内容：若包含文字请逐字转录；" +
            "若有表格或图表，请说明其结构与关键数据；并概述主体、场景与重要细节。只输出描述文本。";

        /// <summary>音频解析提示词：完整转录语音、保留关键信息、音乐环境音简要说明</summary>
        private const string AudioParsePrompt =
            "你是一个音频内容解析助手。请完整、准确地转录这段音频中的语音内容，" +
            "保留语义与关键信息（人名、数字、结论等）；若包含音乐或环境音请简要说明。只输出转录文本。";

        /// <summary>文档页面图解析提示词：逐页完整转录页面渲染图内容，补齐扫描件/转曲文档的文字层缺口</summary>
        private const string DocumentImagesParsePrompt =
            "你是一个文档页面内容解析助手。以下是同一份文档的若干页面渲染图。请逐页完整转录页面内容：" +
            "正文文字逐字转录；表格还原为 Markdown 表格；图表说明其结构与关键数据。只输出转录与描述文本。";

        public AgentFileParseToolsService(
            IAIShareInfoService aIShareInfoService,
            IModalityContentBuilder modalityContentBuilder)
        {
            _aIShareInfoService = aIShareInfoService;
            _modalityContentBuilder = modalityContentBuilder;
        }

        [Description("解析远程文件。下载指定 URL 的文件并解析内容，支持 PDF、Word、Excel、PPT、HTML、Markdown、文本等文档类型；图片会调用视觉模型解析为文字描述，音频会调用音频理解模型转录为文字；扫描件/转曲类 PDF 等文档会结合页面渲染图调用当前会话模型逐页转录；当用户提供文件/媒体链接并需要读取、总结、分析其内容时调用。")]
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

                // 图片/音频走媒体通道：调用多模态模型解析为文字描述/转录后返回
                if (result.Kind == ModalityKind.Image || result.Kind == ModalityKind.Audio)
                    return await ParseMediaWithModelAsync(result, cancellationToken);

                // 文档分支：文本提取 + 页面渲染图经当前会话模型逐页转录（扫描件/转曲文档的文字层缺口补齐）
                return await ParseDocumentWithImagesAsync(result, cancellationToken);
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

        /// <summary>
        /// 图片/音频媒体模型化解析：复用当前会话智能体的同一模型（连接信息来自会话共享上下文 AIShareInfoDto，
        /// 由对话链路 InitData 注入）→ prompt + 媒体 DataContent 调模型 → 返回文字描述。
        /// 无会话模型上下文/不支持/失败/空描述全部映射为 ❌ 友好文本返回，不抛异常不中断对话；仅取消（OperationCanceledException）向上传播。
        /// </summary>
        private async Task<string> ParseMediaWithModelAsync(ModalityContentResult result, CancellationToken cancellationToken)
        {
            var isImage = result.Kind == ModalityKind.Image;
            var capabilityName = isImage ? "图片视觉" : "音频理解";
            var mediaName = isImage ? "图片" : "音频";

            // 1. 取当前会话智能体模型连接（对话/预热链路 InitData 注入）：缺失说明工具运行在无会话上下文的环境
            var share = _aIShareInfoService.GetData();
            if (share == null || string.IsNullOrWhiteSpace(share.AIUrl) || string.IsNullOrWhiteSpace(share.AIDefaultModel))
                return $"❌ 解析失败: 当前会话无智能体模型上下文，无法解析{mediaName}内容。";

            // 2. 音频能力门禁：当前会话模型须具备 AudioUnderstanding 能力位
            if (!isImage && !share.EnableAudioInput)
                return $"❌ 解析失败: 当前会话模型不支持音频理解能力，无法解析音频内容。";

            // 3. 媒体二进制载体缺失（理论上 BuildAsync 成功时必为 DataContent）：按空描述兜底
            if (result.Content is not DataContent data)
                return $"❌ 解析失败: 未能从{mediaName}中解析出内容描述。";

            // 4. 复用会话模型构造 IChatClient 并调用（模型调用段独立 try/catch，错误全量转友好文本）
            try
            {
                var chatClient = BuildSessionChatClient(share);

                var message = new ChatMessage(ChatRole.User, new List<AIContent>
                {
                    new TextContent(isImage ? ImageParsePrompt : AudioParsePrompt),
                    data
                });
                var response = await chatClient.GetResponseAsync(new ChatMessage[] { message }, cancellationToken: cancellationToken);
                var text = response.Text;

                if (string.IsNullOrWhiteSpace(text))
                    return $"❌ 解析失败: 未能从{mediaName}中解析出内容描述。";

                return CharLimits.TruncateToolResult(text, share.ContentLengthLimit);
            }
            catch (OperationCanceledException)
            {
                throw; // 用户取消向上传播，不打断取消语义
            }
            catch (ClientResultException cre) when ((int)cre.Status == 400)
            {
                // 400/参数不支持类异常：模型大概率不具备该模态理解能力
                return $"❌ 解析失败: 当前会话模型可能不支持{capabilityName}能力，请检查模型能力配置。";
            }
            catch (Exception ex)
            {
                return $"❌ 解析失败: 模型解析{mediaName}内容失败: {ex.Message}";
            }
        }

        /// <summary>
        /// 文档分支解析：Builder 已返回提取文本（文字层）与页面渲染图（result.Images，PDF 整页渲染、word/excel 内嵌图等）。
        /// 有渲染图且存在会话模型上下文时，把全部页面图（数量/体积已受 ModalityOptions 三重守卫约束）一次多模态请求
        /// 交给当前会话智能体模型逐页转录，与文本合并返回——补齐扫描件/转曲 PDF「文字层为空、信息只在渲染图里」的缺口。
        /// 无会话上下文（后台任务等）或无渲染图时退化为纯文本返回，不阻断既有文档解析。
        /// </summary>
        private async Task<string> ParseDocumentWithImagesAsync(ModalityContentResult result, CancellationToken cancellationToken)
        {
            var text = result.ExtractedText ?? string.Empty;

            // 无会话模型上下文 或 无页面渲染图：退化为纯文本路径（空文本仍返回友好错误，与既有行为一致）
            var share = _aIShareInfoService.GetData();
            if (share == null
                || string.IsNullOrWhiteSpace(share.AIUrl)
                || string.IsNullOrWhiteSpace(share.AIDefaultModel)
                || result.Images is not { Count: > 0 } images)
            {
                return DocumentTextFallback(text);
            }

            try
            {
                var chatClient = BuildSessionChatClient(share);
                var contents = new List<AIContent> { new TextContent(DocumentImagesParsePrompt) };
                contents.AddRange(images);

                var response = await chatClient.GetResponseAsync(
                    new ChatMessage[] { new ChatMessage(ChatRole.User, contents) },
                    cancellationToken: cancellationToken);

                if (string.IsNullOrWhiteSpace(response.Text))
                    return AppendDocumentImageNote(text, "文档页面图片未能解析出内容描述");

                return MergeDocumentTextAndImages(text, response.Text);
            }
            catch (OperationCanceledException)
            {
                throw; // 用户取消向上传播，不打断取消语义
            }
            catch (ClientResultException cre) when ((int)cre.Status == 400)
            {
                // 400/参数不支持类异常：模型大概率不具备图片视觉能力——文本可用则降级带注返回
                return AppendDocumentImageNote(text, $"当前会话模型可能不支持图片视觉能力（HTTP {(int)cre.Status}）");
            }
            catch (Exception ex)
            {
                // 其余异常：图片增强失败不掩盖文本结果，文本为空才升级为错误
                return AppendDocumentImageNote(text, $"文档页面图片解析失败: {ex.Message}");
            }
        }

        /// <summary>纯文本兜底：文本非空按工具返回口径截断；为空返回与既有行为一致的友好错误</summary>
        private string DocumentTextFallback(string text)
            => string.IsNullOrEmpty(text)
                ? "❌ 解析失败: 未能从文件中提取到文本内容。"
                : CharLimits.TruncateToolResult(text, _aIShareInfoService.GetData()?.ContentLengthLimit ?? 0);

        /// <summary>文本与页面图解析结果合并：空文本（扫描件）直接用转录；有文本则拼接分节标记后统一截断</summary>
        private string MergeDocumentTextAndImages(string text, string imageDescription)
        {
            var merged = string.IsNullOrEmpty(text)
                ? imageDescription
                : text + "\n\n【文档页面图片内容解析】\n" + imageDescription;
            return CharLimits.TruncateToolResult(merged, _aIShareInfoService.GetData()?.ContentLengthLimit ?? 0);
        }

        /// <summary>图片增强失败但文本可用：文本追加一条失败注记返回；文本为空则升级为友好错误</summary>
        private static string AppendDocumentImageNote(string text, string note)
            => string.IsNullOrEmpty(text)
                ? $"❌ 解析失败: 文档文本提取为空，且页面图片解析未成功（{note}）。"
                : text + $"\n\n(文档页面图片解析未成功: {note})";

        /// <summary>复用当前会话智能体模型构造 IChatClient（连接信息来自会话共享上下文）</summary>
        private static Microsoft.Extensions.AI.IChatClient BuildSessionChatClient(AIShareInfoDto share)
            => new OpenAIClient(
                new ApiKeyCredential(string.IsNullOrWhiteSpace(share.AIKeySecret) ? "local" : share.AIKeySecret),
                new OpenAIClientOptions { Endpoint = new Uri(share.AIUrl) })
                .GetChatClient(share.AIDefaultModel).AsIChatClient();
    }
}
