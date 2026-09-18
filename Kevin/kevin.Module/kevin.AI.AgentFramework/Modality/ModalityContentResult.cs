using kevin.Domain.Share.Enums;
using Microsoft.Extensions.AI;

namespace kevin.AI.AgentFramework.Modality
{
    /// <summary>
    /// 单个远程文件经 <see cref="IModalityContentBuilder"/> 处理后的结果。
    /// <para>
    /// 二选一语义：<see cref="Content"/> 与 <see cref="ExtractedText"/> 只会有一个非空 ——
    /// Image/Audio 走 <see cref="Content"/>（直接以 <c>DataContent</c> 送入模型），
    /// Document/未知类型走 <see cref="ExtractedText"/>（解析为纯文本后由上层拼进 <c>TextContent</c>）。
    /// 这样上层可以按"媒体附件列表 + 文本片段列表"两桶清晰归集，不用在业务层再判 fileType。
    /// </para>
    /// </summary>
    /// <param name="Kind">模态分类，用于上层日志/统计</param>
    /// <param name="FileName">原始文件名（用于错误提示与日志）</param>
    /// <param name="Content">Image/Audio 的二进制内容载体；Document/未知类型为 null</param>
    /// <param name="ExtractedText">Document 解析后的文本；Image/Audio 为 null</param>
    /// <param name="Error">处理失败时的错误描述（成功时为 null），上层可选择拼进上下文告知模型</param>
    public sealed record ModalityContentResult(
        ModalityKind Kind,
        string FileName,
        AIContent? Content,
        string? ExtractedText,
        string? Error)
    {
        /// <summary>是否成功产出了可直接送入模型的内容（Content 或 ExtractedText 任一非空）</summary>
        public bool IsUsable => Content != null || !string.IsNullOrEmpty(ExtractedText);
    }
}
