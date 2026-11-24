using kevin.AI.AgentFramework.Const;
using kevin.AI.AgentFramework.Dto;
using kevin.AI.AgentFramework.Interfaces;
using kevin.AI.AgentFramework.Interfaces.Tools;
using kevin.AI.AgentFramework.Modality;
using kevin.AI.AgentFramework.Tools;
using kevin.Domain.Share.Enums;
using Microsoft.Extensions.AI;
using System.Text;

namespace Kevin.Unit.Test.Kevin.AI.AgentFramework
{
    /// <summary>
    /// ParseRemoteFileAsync 文档分支回归测试（媒体模型化改造不应影响文档解析既有行为）：
    /// 覆盖域名护栏、下载/构建失败错误前缀、文档提取文本按 ContentLengthLimit 截断、空文本兜底。
    /// 依赖均用手写 fake（与项目既有测试体系一致，不引入 mock 框架）。
    /// </summary>
    public class AgentFileParseToolsServiceTests
    {
        private const string AuthorizedUrl = "https://files.example.com/report.pdf";
        private const string UnauthorizedUrl = "https://evil.example.org/report.pdf";

        /// <summary>共享信息 fake：授权域名白名单只放行 files.example.com，ContentLengthLimit 可按用例设定</summary>
        private sealed class FakeShareInfoService : IAIShareInfoService
        {
            private readonly AIShareInfoDto _data = new AIShareInfoDto
            {
                AuthorizedDomainsList = new List<string> { "files.example.com" },
            };

            public FakeShareInfoService(int contentLengthLimit) => _data.ContentLengthLimit = contentLengthLimit;

            public AIShareInfoDto GetData() => _data;

            public void InitData(AIShareInfoDto data) { }
        }

        /// <summary>内容构造器 fake：固定返回预设结果（文档分支不触网）</summary>
        private sealed class FakeContentBuilder : IModalityContentBuilder
        {
            private readonly ModalityContentResult _result;
            public FakeContentBuilder(ModalityContentResult result) => _result = result;
            public Task<ModalityContentResult> BuildAsync(string fileUrl, string? fileName, CancellationToken cancellationToken = default)
                => Task.FromResult(_result);
        }

        private static AgentFileParseToolsService CreateService(int contentLengthLimit, ModalityContentResult result)
            => new AgentFileParseToolsService(
                new FakeShareInfoService(contentLengthLimit),
                new FakeContentBuilder(result));

        [Fact]
        public async Task ParseRemoteFile_UnauthorizedDomain_ReturnsGuardrailMessage()
        {
            // 域名护栏行为与改造前一致：未授权域名的 URL 即使构造器返回可用结果也不触达解析
            var service = CreateService(0, new ModalityContentResult(
                ModalityKind.Document, "report.pdf", null, "文档正文", null));

            var actual = await service.ParseRemoteFileAsync(UnauthorizedUrl, "report.pdf");

            Assert.Equal(CommandGuardrails.UnauthorizedDomainMessage(UnauthorizedUrl), actual);
            Assert.StartsWith("❌ 授权拦截", actual);
        }

        [Fact]
        public async Task ParseRemoteFile_BuildError_ReturnsFriendlyErrorPrefix()
        {
            // 下载/构建失败分支（错误映射 #2）行为与改造前一致：❌ 前缀 + 原始错误
            var service = CreateService(0, new ModalityContentResult(
                ModalityKind.Document, "report.pdf", null, null, "HTTP 502 Bad Gateway"));

            var actual = await service.ParseRemoteFileAsync(AuthorizedUrl, "report.pdf");

            Assert.Equal("❌ 解析失败: HTTP 502 Bad Gateway", actual);
        }

        [Fact]
        public async Task ParseRemoteFile_DocumentWithoutLimit_ReturnsExtractedTextUnchanged()
        {
            // 文档成功分支：ContentLengthLimit=0（不截断）时原文返回
            var text = "这是解析出的文档正文内容。";
            var service = CreateService(0, new ModalityContentResult(
                ModalityKind.Document, "report.pdf", null, text, null));

            var actual = await service.ParseRemoteFileAsync(AuthorizedUrl, "report.pdf");

            Assert.Equal(text, actual);
        }

        [Fact]
        public async Task ParseRemoteFile_DocumentOverLimit_TruncatedWithLimitPromptPrefix()
        {
            // 文档成功分支 + 超长文本：走 TruncateToolResult 统一口径（截断提示前缀 + 截断正文）
            var longText = new string('字', 200) + "TAIL_MARKER";
            var service = CreateService(50, new ModalityContentResult(
                ModalityKind.Document, "report.pdf", null, longText, null));

            var actual = await service.ParseRemoteFileAsync(AuthorizedUrl, "report.pdf");

            Assert.StartsWith(SystemPrompt.ContentLimitPromptText, actual);
            Assert.False(actual.Contains("TAIL_MARKER"), "超长文本尾部不应保留");
        }

        [Fact]
        public async Task ParseRemoteFile_DocumentEmptyExtractedText_ReturnsFriendlyError()
        {
            // 文档分支空文本兜底（错误映射 #6 同款文案）与改造前一致
            var service = CreateService(0, new ModalityContentResult(
                ModalityKind.Document, "report.pdf", null, "", null));

            var actual = await service.ParseRemoteFileAsync(AuthorizedUrl, "report.pdf");

            Assert.Equal("❌ 解析失败: 未能从文件中提取到文本内容。", actual);
        }

        [Fact]
        public async Task ParseRemoteFile_DocumentWithImages_NoSessionContext_ReturnsTextUnchanged()
        {
            // 文档带页面渲染图但无会话模型上下文（FakeShareInfo 未注入 AIUrl）：退化为纯文本返回，不触模型不报错
            var text = "这是文档文字层提取出的内容。";
            var images = new List<AIContent> { new DataContent("data:image/png;base64,aGVsbG8=", "image/png") };
            var service = CreateService(0, new ModalityContentResult(
                ModalityKind.Document, "scan.pdf", null, text, null, images));

            var actual = await service.ParseRemoteFileAsync(AuthorizedUrl, "scan.pdf");

            Assert.Equal(text, actual);
        }

        [Fact]
        public async Task ParseRemoteFile_ScannedPdf_NoSessionContext_ReturnsFriendlyError()
        {
            // 扫描件（文字层为空）+ 页面渲染图 + 无会话模型上下文：返回空文本友好错误（与既有行为一致）
            var images = new List<AIContent> { new DataContent("data:image/png;base64,aGVsbG8=", "image/png") };
            var service = CreateService(0, new ModalityContentResult(
                ModalityKind.Document, "scan.pdf", null, "", null, images));

            var actual = await service.ParseRemoteFileAsync(AuthorizedUrl, "scan.pdf");

            Assert.Equal("❌ 解析失败: 未能从文件中提取到文本内容。", actual);
        }
    }
}
