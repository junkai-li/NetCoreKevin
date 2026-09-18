using kevin.AI.AgentFramework.Dto;
using kevin.Domain.Share.Enums;
using Kevin.Common.Helper.FileHandleTools;
using Microsoft.Extensions.AI;
using NetCore.Util;

namespace kevin.AI.AgentFramework.Modality
{
    /// <summary>
    /// 多模态内容构造器的默认实现：远程文件 → <see cref="AIContent"/> 或纯文本片段。
    /// <para>
    /// 分类逻辑复用 <see cref="FileHelper.DetermineFileType"/>，避免"扩展名判定"两处漂移；
    /// MIME 从 <see cref="FileHelper.GetRemoteFileAsync"/> 拿到的 HTTP Content-Type，
    /// 兜底后仍是 <c>application/octet-stream</c> 时按扩展名再推一次。
    /// </para>
    /// <para>
    /// 守卫层级（每一层单独拒绝，都会返回带 Error 的结果而不是抛异常，避免整轮对话被打挂）：
    /// 1. 音频 MIME 白名单（<see cref="ModalityOptions.AllowedAudioMimeTypes"/>）；
    /// 2. 图片/音频体积上限（<see cref="ModalityOptions.MaxImageFileBytes"/> / <see cref="ModalityOptions.MaxAudioFileBytes"/>）；
    /// 3. 单文件解析失败（Reader 抛异常）→ 转为 Error 描述文本。
    /// </para>
    /// </summary>
    public class ModalityContentBuilder : IModalityContentBuilder
    {
        /// <inheritdoc />
        public async Task<ModalityContentResult> BuildAsync(string fileUrl, string? fileName, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(fileUrl))
            {
                return new ModalityContentResult(ModalityKind.Text, fileName ?? "", null, null, "文件 URL 为空");
            }

            // 文件名为空时先尝试从 URL 提取，DetermineFileType 依赖扩展名判定
            var effectiveName = !string.IsNullOrWhiteSpace(fileName)
                ? fileName!
                : SafeGetFileNameFromUrl(fileUrl);
            var fileType = FileHelper.DetermineFileType(effectiveName);

            try
            {
                switch (fileType)
                {
                    case "image":
                        return await BuildMediaAsync(fileUrl, effectiveName, ModalityKind.Image, cancellationToken);
                    case "audio":
                        return await BuildMediaAsync(fileUrl, effectiveName, ModalityKind.Audio, cancellationToken);
                    case "excel":
                    case "pdf":
                    case "word":
                    case "html":
                    case "markdown":
                    case "text":
                        return await BuildDocumentAsync(fileUrl, effectiveName, fileType, cancellationToken);
                    default:
                        // 未知类型不抛异常，返回一段说明性文本让模型知道用户上传了不能识别的东西
                        return new ModalityContentResult(
                            ModalityKind.Document,
                            effectiveName,
                            null,
                            $"[不支持的文件类型：{effectiveName}，未能解析内容]",
                            null);
                }
            }
            catch (OperationCanceledException)
            {
                throw; // 取消要向上传播，让主流程走 CancellationToken 中止路径
            }
            catch (Exception ex)
            {
                return new ModalityContentResult(
                    ModalityKind.Document,
                    effectiveName,
                    null,
                    null,
                    $"读取失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 图片/音频分支：下载 → 体积守卫 → MIME 白名单（仅音频）→ 构造 DataContent
        /// </summary>
        private static async Task<ModalityContentResult> BuildMediaAsync(string fileUrl, string fileName, ModalityKind kind, CancellationToken ct)
        {
            var remote = await FileHelper.GetRemoteFileAsync(fileUrl, ct);
            var options = ModalityOptions.Current;

            // MIME 兜底：Content-Type 是 octet-stream 时再按扩展名推一次，仍不识别就拒收
            var mime = remote.MimeType;
            if (string.IsNullOrWhiteSpace(mime) || string.Equals(mime, "application/octet-stream", StringComparison.OrdinalIgnoreCase))
            {
                mime = FileHelper.GuessMimeByExtension(fileName);
            }
            if (string.IsNullOrWhiteSpace(mime) || string.Equals(mime, "application/octet-stream", StringComparison.OrdinalIgnoreCase))
            {
                return new ModalityContentResult(kind, fileName, null, null, $"无法识别的{(kind == ModalityKind.Image ? "图片" : "音频")} MIME 类型");
            }

            // 音频 MIME 白名单：白名单为空视为不限制
            if (kind == ModalityKind.Audio
                && options.AllowedAudioMimeTypes.Count > 0
                && !options.AllowedAudioMimeTypes.Any(m => string.Equals(m, mime, StringComparison.OrdinalIgnoreCase)))
            {
                return new ModalityContentResult(kind, fileName, null, null, $"不支持的音频格式 {mime}");
            }

            // 体积守卫：图片/音频分别有自己的上限，避免大文件撑爆消息体
            var sizeLimit = kind == ModalityKind.Image ? options.MaxImageFileBytes : options.MaxAudioFileBytes;
            if (sizeLimit > 0 && remote.Length > sizeLimit)
            {
                return new ModalityContentResult(
                    kind,
                    fileName,
                    null,
                    null,
                    $"{(kind == ModalityKind.Image ? "图片" : "音频")}文件超过大小上限（{remote.Length / 1024 / 1024}MB > {sizeLimit / 1024 / 1024}MB）");
            }

            // 用带 mimeType 的重载，避免 MemoryStream 无 Name 导致 MIME 推断失败（Bug 2 根治点）
            using var stream = remote.Stream;
            var dataContent = await DataContent.LoadFromAsync(stream, mime, ct);
            return new ModalityContentResult(kind, fileName, dataContent, null, null);
        }

        /// <summary>
        /// 文档分支：下载 → 按类型路由到对应 Reader → 返回解析后的纯文本
        /// </summary>
        private static async Task<ModalityContentResult> BuildDocumentAsync(string fileUrl, string fileName, string fileType, CancellationToken ct)
        {
            var remote = await FileHelper.GetRemoteFileAsync(fileUrl, ct);
            using var stream = remote.Stream;
            string content = fileType switch
            {
                "excel" => await ExcelReader.ReadExcelToMarkdownAsync(stream, fileName, ct),
                "pdf" => PDFReader.ReadPdfToMarkdown(stream),
                "word" => WordReader.ReadParagraphs(stream),
                "html" => await HtmlReader.ExtractTextFromStreamAsync(stream),
                "markdown" => TextStreamReader.ReadMarkdownFromStream(stream).RawContent,
                _ => TextStreamReader.ReadTextFromStream(stream)
            };
            return new ModalityContentResult(ModalityKind.Document, fileName, null, content, null);
        }

        /// <summary>
        /// 从 URL 安全提取文件名，异常时返回占位 "file"
        /// </summary>
        private static string SafeGetFileNameFromUrl(string url)
        {
            try
            {
                var name = Path.GetFileName(new Uri(url).LocalPath);
                return string.IsNullOrWhiteSpace(name) ? "file" : name;
            }
            catch
            {
                return "file";
            }
        }
    }
}
