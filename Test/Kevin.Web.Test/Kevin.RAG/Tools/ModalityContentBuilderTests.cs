using kevin.AI.AgentFramework.Modality;
using Microsoft.Extensions.AI;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Kevin.Unit.Test.Kevin.RAG.Tools
{
    /// <summary>
    /// ModalityContentBuilder 文档分支图片提取全链路测试：
    /// 本地 HttpListener 模拟文档与图片的远程下载，覆盖 markdown/html 的 data URI 与远程 URL 图片源。
    /// </summary>
    public class ModalityContentBuilderTests : IAsyncLifetime
    {
        /// <summary>1x1 红色像素的合法 PNG 字节</summary>
        private static readonly byte[] PngBytes = Convert.FromBase64String(
            "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNkYPhfDwAChwGA60e6kgAAAABJRU5ErkJggg==");

        private readonly ModalityContentBuilder _builder = new();
        private HttpListener _listener = null!;
        private string _baseUrl = null!;

        public async Task InitializeAsync()
        {
            _baseUrl = $"http://127.0.0.1:{GetFreePort()}";
            _listener = new HttpListener();
            _listener.Prefixes.Add(_baseUrl + "/");
            _listener.Start();
            _ = Task.Run(ServeAsync);
            await Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            _listener?.Stop();
            _listener?.Close();
            return Task.CompletedTask;
        }

        private async Task ServeAsync()
        {
            while (_listener.IsListening)
            {
                HttpListenerContext context;
                try
                {
                    context = await _listener.GetContextAsync();
                }
                catch
                {
                    return; // listener 已停止
                }

                try
                {
                    switch (context.Request.Url.AbsolutePath)
                    {
                        case "/images/1.png":
                            context.Response.ContentType = "image/png";
                            context.Response.StatusCode = 200;
                            await context.Response.OutputStream.WriteAsync(PngBytes);
                            break;
                        case "/docs/sample.md":
                            context.Response.ContentType = "text/markdown";
                            context.Response.StatusCode = 200;
                            var markdown = $"# 示例文档\n\n正文内容 ![图片]({_baseUrl}/images/1.png)\n";
                            var mdBytes = Encoding.UTF8.GetBytes(markdown);
                            await context.Response.OutputStream.WriteAsync(mdBytes);
                            break;
                        case "/docs/datauri.md":
                            context.Response.ContentType = "text/markdown";
                            context.Response.StatusCode = 200;
                            var dataUri = $"# 示例文档\n\n正文内容 ![图片](data:image/png;base64,{Convert.ToBase64String(PngBytes)})\n";
                            var dataUriBytes = Encoding.UTF8.GetBytes(dataUri);
                            await context.Response.OutputStream.WriteAsync(dataUriBytes);
                            break;
                        case "/docs/plain.md":
                            context.Response.ContentType = "text/markdown";
                            context.Response.StatusCode = 200;
                            var plain = "# 示例文档\n\n没有图片的正文内容\n";
                            var plainBytes = Encoding.UTF8.GetBytes(plain);
                            await context.Response.OutputStream.WriteAsync(plainBytes);
                            break;
                        case "/docs/page.html":
                            context.Response.ContentType = "text/html";
                            context.Response.StatusCode = 200;
                            var html = $"<html><body><p>正文内容</p><img src=\"data:image/png;base64,{Convert.ToBase64String(PngBytes)}\"></body></html>";
                            var htmlBytes = Encoding.UTF8.GetBytes(html);
                            await context.Response.OutputStream.WriteAsync(htmlBytes);
                            break;
                        case "/docs/broken.html":
                            context.Response.ContentType = "text/html";
                            context.Response.StatusCode = 200;
                            var brokenHtml = "<html><body><p>正文内容</p><img src=\"/missing.png\"></body></html>";
                            var brokenHtmlBytes = Encoding.UTF8.GetBytes(brokenHtml);
                            await context.Response.OutputStream.WriteAsync(brokenHtmlBytes);
                            break;
                        case "/docs/curves.pdf":
                            context.Response.ContentType = "application/pdf";
                            context.Response.StatusCode = 200;
                            await context.Response.OutputStream.WriteAsync(BuildGraphicsOnlyPdf());
                            break;
                        case "/docs/text.pdf":
                            context.Response.ContentType = "application/pdf";
                            context.Response.StatusCode = 200;
                            await context.Response.OutputStream.WriteAsync(BuildTextPdf());
                            break;
                        default:
                            context.Response.StatusCode = 404;
                            break;
                    }
                }
                catch
                {
                    // 客户端中止等单请求异常忽略，服务器继续服务
                }
                finally
                {
                    context.Response.Close();
                }
            }
        }

        private static int GetFreePort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }

        /// <summary>
        /// 手写一份无文本层的 PDF（内容流只有矢量图形，没有任何文本操作），模拟转曲印刷文档；
        /// xref 偏移精确计算，可被 PdfPig 正常解析
        /// </summary>
        private static byte[] BuildGraphicsOnlyPdf()
        {
            // 内容流：只画一个填充矩形，无 BT/Tj 文本操作
            var contentBytes = Encoding.ASCII.GetBytes("0 0 10 10 re f");

            var objects = new List<byte[]>
            {
                // 1: Catalog
                Encoding.ASCII.GetBytes("<< /Type /Catalog /Pages 2 0 R >>"),
                // 2: Pages
                Encoding.ASCII.GetBytes("<< /Type /Pages /Kids [3 0 R] /Count 1 >>"),
                // 3: Page
                Encoding.ASCII.GetBytes("<< /Type /Page /Parent 2 0 R /MediaBox [0 0 10 10] /Contents 4 0 R >>"),
                // 4: Content stream
                Concat(
                    Encoding.ASCII.GetBytes($"<< /Length {contentBytes.Length} >>\nstream\n"),
                    contentBytes,
                    Encoding.ASCII.GetBytes("\nendstream")),
            };

            return AssemblePdf(objects);
        }

        /// <summary>
        /// 手写一份含文字层的 PDF（内容流有 BT/Tj 文本操作）
        /// </summary>
        private static byte[] BuildTextPdf()
        {
            var contentBytes = Encoding.ASCII.GetBytes("BT /F1 12 Tf 10 10 Td (Hello) Tj ET");

            var objects = new List<byte[]>
            {
                // 1: Catalog
                Encoding.ASCII.GetBytes("<< /Type /Catalog /Pages 2 0 R >>"),
                // 2: Pages
                Encoding.ASCII.GetBytes("<< /Type /Pages /Kids [3 0 R] /Count 1 >>"),
                // 3: Page
                Encoding.ASCII.GetBytes("<< /Type /Page /Parent 2 0 R /MediaBox [0 0 100 100] /Resources << /Font << /F1 4 0 R >> >> /Contents 5 0 R >>"),
                // 4: Font
                Encoding.ASCII.GetBytes("<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>"),
                // 5: Content stream
                Concat(
                    Encoding.ASCII.GetBytes($"<< /Length {contentBytes.Length} >>\nstream\n"),
                    contentBytes,
                    Encoding.ASCII.GetBytes("\nendstream")),
            };

            return AssemblePdf(objects);
        }

        /// <summary>
        /// 组装 PDF：对象编号从 1 开始，xref 偏移精确计算
        /// </summary>
        private static byte[] AssemblePdf(List<byte[]> objects)
        {
            using var builder = new MemoryStream();
            builder.Write(Encoding.ASCII.GetBytes("%PDF-1.4\n"));
            var offsets = new long[objects.Count];
            for (var i = 0; i < objects.Count; i++)
            {
                offsets[i] = builder.Position;
                builder.Write(Encoding.ASCII.GetBytes($"{i + 1} 0 obj\n"));
                builder.Write(objects[i]);
                builder.Write(Encoding.ASCII.GetBytes("\nendobj\n"));
            }
            var xrefOffset = builder.Position;
            builder.Write(Encoding.ASCII.GetBytes($"xref\n0 {objects.Count + 1}\n"));
            builder.Write(Encoding.ASCII.GetBytes("0000000000 65535 f \n"));
            for (var i = 0; i < objects.Count; i++)
            {
                builder.Write(Encoding.ASCII.GetBytes($"{offsets[i]:0000000000} 00000 n \n"));
            }
            builder.Write(Encoding.ASCII.GetBytes($"trailer\n<< /Size {objects.Count + 1} /Root 1 0 R >>\nstartxref\n{xrefOffset}\n%%EOF\n"));
            return builder.ToArray();
        }

        private static byte[] Concat(params byte[][] parts)
        {
            using var ms = new MemoryStream();
            foreach (var part in parts) ms.Write(part, 0, part.Length);
            return ms.ToArray();
        }

        [Fact]
        public async Task BuildAsync_WithMarkdownRemoteImage_ReturnsTextAndImage()
        {
            var result = await _builder.BuildAsync($"{_baseUrl}/docs/sample.md", "sample.md");

            Assert.NotNull(result.ExtractedText);
            Assert.Contains("正文内容", result.ExtractedText);
            var image = Assert.Single(result.Images!);
            Assert.Equal("image/png", ((DataContent)image).MediaType);
        }

        [Fact]
        public async Task BuildAsync_WithMarkdownDataUriImage_ReturnsTextAndImage()
        {
            var result = await _builder.BuildAsync($"{_baseUrl}/docs/datauri.md", "datauri.md");

            Assert.NotNull(result.ExtractedText);
            Assert.Contains("正文内容", result.ExtractedText);
            var image = Assert.Single(result.Images!);
            Assert.Equal("image/png", ((DataContent)image).MediaType);
        }

        [Fact]
        public async Task BuildAsync_WithMarkdownNoImages_ReturnsTextOnly()
        {
            var result = await _builder.BuildAsync($"{_baseUrl}/docs/plain.md", "plain.md");

            Assert.NotNull(result.ExtractedText);
            Assert.Contains("没有图片的正文内容", result.ExtractedText);
            Assert.Null(result.Images);
        }

        [Fact]
        public async Task BuildAsync_WithHtmlDataUriImage_ReturnsTextAndImage()
        {
            var result = await _builder.BuildAsync($"{_baseUrl}/docs/page.html", "page.html");

            Assert.NotNull(result.ExtractedText);
            Assert.Contains("正文内容", result.ExtractedText);
            var image = Assert.Single(result.Images!);
            Assert.Equal("image/png", ((DataContent)image).MediaType);
        }

        [Fact]
        public async Task BuildAsync_WithHtmlBrokenImage_StillReturnsText()
        {
            // HTML 里的相对图片解析成绝对地址后下载失败（404），应跳过图片但不影响文本
            var result = await _builder.BuildAsync($"{_baseUrl}/docs/broken.html", "broken.html");

            Assert.NotNull(result.ExtractedText);
            Assert.Contains("正文内容", result.ExtractedText);
            Assert.Null(result.Images);
        }

        [Fact]
        public async Task BuildAsync_WithTextlessPdf_RendersPageImages()
        {
            // 转曲 PDF（无文字层）：整页渲染，得到页面渲染 PNG
            var result = await _builder.BuildAsync($"{_baseUrl}/docs/curves.pdf", "curves.pdf");

            Assert.NotNull(result.ExtractedText);
            // 该 PDF 没有内嵌位图，若走内嵌图提取老路径 Images 应为空；非空即证明走了整页渲染
            var image = Assert.Single(result.Images!);
            Assert.Equal("image/png", ((DataContent)image).MediaType);
        }

        [Fact]
        public async Task BuildAsync_WithTextPdf_RendersPageImages()
        {
            // PDF 一律整页渲染：即使有文字层，图片也是整页渲染图而非内嵌位图提取
            var result = await _builder.BuildAsync($"{_baseUrl}/docs/text.pdf", "text.pdf");

            Assert.NotNull(result.ExtractedText);
            Assert.Contains("Hello", result.ExtractedText);
            var image = Assert.Single(result.Images!);
            Assert.Equal("image/png", ((DataContent)image).MediaType);
        }
    }
}
