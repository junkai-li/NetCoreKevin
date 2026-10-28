using System.Text;
using System.Text.RegularExpressions;
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
        /// 文档分支：下载 → 按类型路由到对应 Reader → 返回解析后的纯文本，同时提取内嵌图片构造 DataContent
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

            // 模型已支持图片输入：把文档内嵌图片提取出来，与文本一起送入模型
            var images = await ExtractDocumentImagesAsync(stream, fileUrl, fileType, content, ct);
            return new ModalityContentResult(ModalityKind.Document, fileName, null, content, null, images.Count > 0 ? images : null);
        }

        /// <summary>视觉模型普遍支持的图片 MIME 白名单（jpg 为 jpeg 别名，统一按后者判定）</summary>
        private static readonly string[] SupportedImageMimes = { "image/png", "image/jpeg", "image/gif", "image/webp", "image/bmp" };

        /// <summary>
        /// 文档图片提取：pdf 整页渲染成图，word/excel 拿内嵌字节，html/markdown 从内容解析图片源（URL/data URI）再统一下载。
        /// 全部图片经 MIME 白名单、体积上限（<see cref="ModalityOptions.MaxImageFileBytes"/>）、
        /// 数量上限（<see cref="ModalityOptions.MaxImagesPerDocument"/>）三重守卫后构造 DataContent。
        /// </summary>
        private static async Task<List<AIContent>> ExtractDocumentImagesAsync(Stream stream, string fileUrl, string fileType, string content, CancellationToken ct)
        {
            var options = ModalityOptions.Current;
            var extracted = new List<(byte[] Bytes, string Mime)>();

            switch (fileType)
            {
                case "pdf":
                    // PDF 一律整页渲染成图送视觉模型：渲染图包含页面上的全部信息（文字/图片/表格），
                    // 且转曲/扫描 PDF 的正文只在渲染图里可见，内嵌位图提取反而不完整
                    TrySeekStart(stream);
                    extracted.AddRange(PDFReader.RenderPagesToPng(stream, options.PdfRenderDpi / 72f, options.MaxImagesPerDocument));
                    break;
                case "word":
                case "excel":
                    // 文本解析已消费过流，重置位置后由 Reader 从头提取内嵌图片字节
                    TrySeekStart(stream);
                    extracted.AddRange(fileType == "word"
                        ? WordReader.ExtractImages(stream)
                        : ExcelReader.ExtractImages(stream));
                    break;
                case "html":
                {
                    // HTML 图片可能是 data URI 或远程 URL，相对路径已由 Reader 结合文件 URL 解析为绝对地址
                    var sources = await HtmlReader.ExtractImageSourcesAsync(stream, fileUrl);
                    foreach (var source in sources)
                    {
                        var resolved = await ResolveImageSourceAsync(source, ct);
                        if (resolved != null) extracted.Add(resolved.Value);
                    }
                    break;
                }
                case "markdown":
                {
                    // Markdown 图片语法 ![alt](url)，只收 http(s) 远程地址与 data URI
                    foreach (var source in ExtractMarkdownImageSources(content))
                    {
                        var resolved = await ResolveImageSourceAsync(source, ct);
                        if (resolved != null) extracted.Add(resolved.Value);
                    }
                    break;
                }
            }

            var images = new List<AIContent>();
            foreach (var (bytes, mime) in extracted)
            {
                if (images.Count >= options.MaxImagesPerDocument) break;
                if (bytes.Length == 0) continue;
                var mimeType = NormalizeImageMime(mime, bytes);
                if (mimeType == null) continue;
                if (options.MaxImageFileBytes > 0 && bytes.Length > options.MaxImageFileBytes) continue;
                using var ms = new MemoryStream(bytes, writable: false);
                images.Add(await DataContent.LoadFromAsync(ms, mimeType, ct));
            }
            return images;
        }

        /// <summary>
        /// 把图片源解析为字节 + MIME：data URI 直接解码，http(s) 走统一下载（MIME 兜底按扩展名）。
        /// 单图解析失败返回 null 由调用方跳过，不影响文档整体处理。
        /// </summary>
        private static async Task<(byte[] Bytes, string Mime)?> ResolveImageSourceAsync(string source, CancellationToken ct)
        {
            try
            {
                if (source.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
                {
                    var (uriBytes, uriMime) = ParseDataUriImage(source);
                    return uriBytes != null && uriBytes.Length > 0 ? (uriBytes, uriMime ?? "application/octet-stream") : null;
                }

                var remote = await FileHelper.GetRemoteFileAsync(source, ct);
                using var stream = remote.Stream;
                using var ms = new MemoryStream();
                await stream.CopyToAsync(ms, ct);
                var bytes = ms.ToArray();
                if (bytes.Length == 0) return null;
                var mime = remote.MimeType;
                if (string.IsNullOrWhiteSpace(mime) || string.Equals(mime, "application/octet-stream", StringComparison.OrdinalIgnoreCase))
                {
                    mime = FileHelper.GuessMimeByExtension(source);
                }
                return (bytes, mime ?? "application/octet-stream");
            }
            catch (OperationCanceledException)
            {
                throw; // 取消要向上传播
            }
            catch (Exception)
            {
                return null; // 下载失败、data URI 非法等单图异常跳过
            }
        }

        /// <summary>
        /// 解析 data URI 图片（data:image/xxx;base64,xxxx 或 URL 编码形式），返回字节与声明 MIME
        /// </summary>
        private static (byte[]? Bytes, string? Mime) ParseDataUriImage(string dataUri)
        {
            var commaIndex = dataUri.IndexOf(',');
            if (commaIndex < 0) return (null, null);
            var meta = dataUri.Substring("data:".Length, commaIndex - "data:".Length);
            var payload = dataUri.Substring(commaIndex + 1);
            var mime = meta.Split(';')[0];
            try
            {
                if (meta.EndsWith(";base64", StringComparison.OrdinalIgnoreCase))
                    return (Convert.FromBase64String(payload.Trim()), mime);
                return (Encoding.UTF8.GetBytes(Uri.UnescapeDataString(payload)), mime);
            }
            catch
            {
                return (null, null);
            }
        }

        /// <summary>
        /// 从 Markdown 文本中提取图片源：匹配 ![alt](src) 语法，只收 http(s) 远程地址与 data URI
        /// </summary>
        private static IEnumerable<string> ExtractMarkdownImageSources(string markdown)
        {
            foreach (Match match in Regex.Matches(markdown, @"!\[[^\]]*\]\(([^)\s]+)"))
            {
                var source = match.Groups[1].Value.Trim();
                if (source.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                    || source.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
                    || source.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
                {
                    yield return source;
                }
            }
        }

        /// <summary>
        /// 图片 MIME 归一化：白名单内直接采用（jpg 别名归一为 jpeg）；不在白名单时用魔数嗅探兜底；
        /// 最终仍不在白名单内返回 null（如 svg/tiff/emf，视觉模型不确定支持）
        /// </summary>
        private static string? NormalizeImageMime(string? mime, byte[] bytes)
        {
            if (!string.IsNullOrWhiteSpace(mime))
            {
                var normalized = mime.Trim();
                if (string.Equals(normalized, "image/jpg", StringComparison.OrdinalIgnoreCase))
                    normalized = "image/jpeg";
                if (SupportedImageMimes.Contains(normalized, StringComparer.OrdinalIgnoreCase))
                    return normalized;
            }

            var sniffed = FileHelper.SniffImageMime(bytes);
            return sniffed != null && SupportedImageMimes.Contains(sniffed, StringComparer.OrdinalIgnoreCase) ? sniffed : null;
        }

        /// <summary>流可定位时重置到起始位置，供文档解析完文本后再次从头提取图片</summary>
        private static void TrySeekStart(Stream stream)
        {
            if (stream.CanSeek) stream.Position = 0;
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
