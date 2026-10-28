using Kevin.Common.Helper.FileHandleTools;
using System.IO.Compression;
using System.Text;

namespace Kevin.Unit.Test.Kevin.RAG.Tools
{
    public class PDFReaderTests
    {
        /// <summary>
        /// 手写一份最小 PDF（1 页，内嵌 1x1 RGB 图片 XObject），xref 偏移精确计算，可被 PdfPig 正常解析
        /// </summary>
        private static byte[] BuildPdfWithImage()
        {
            // 图片像素：1x1 RGB 红色，FlateDecode 压缩（必须用 ZLibStream 生成 zlib 流，PDF 规范要求；
            // DeflateStream 生成的是 raw deflate，PdfPig 无法解码）
            byte[] compressed;
            using (var ms = new MemoryStream())
            {
                using (var ds = new ZLibStream(ms, CompressionLevel.Optimal, leaveOpen: true))
                {
                    ds.Write(new byte[] { 255, 0, 0 }, 0, 3);
                }
                compressed = ms.ToArray();
            }

            // 页面内容流：绘制图片 XObject
            var contentBytes = Encoding.ASCII.GetBytes("q\n10 0 0 10 0 0 cm\n/Im1 Do\nQ");

            var objects = new List<byte[]>
            {
                // 1: Catalog
                Encoding.ASCII.GetBytes("<< /Type /Catalog /Pages 2 0 R >>"),
                // 2: Pages
                Encoding.ASCII.GetBytes("<< /Type /Pages /Kids [3 0 R] /Count 1 >>"),
                // 3: Page
                Encoding.ASCII.GetBytes("<< /Type /Page /Parent 2 0 R /MediaBox [0 0 10 10] /Resources << /XObject << /Im1 4 0 R >> >> /Contents 5 0 R >>"),
                // 4: Image XObject
                Concat(
                    Encoding.ASCII.GetBytes($"<< /Type /XObject /Subtype /Image /Width 1 /Height 1 /ColorSpace /DeviceRGB /BitsPerComponent 8 /Filter /FlateDecode /Length {compressed.Length} >>\nstream\n"),
                    compressed,
                    Encoding.ASCII.GetBytes("\nendstream")),
                // 5: Content stream
                Concat(
                    Encoding.ASCII.GetBytes($"<< /Length {contentBytes.Length} >>\nstream\n"),
                    contentBytes,
                    Encoding.ASCII.GetBytes("\nendstream")),
            };

            return AssemblePdf(objects);
        }

        /// <summary>
        /// 手写一份最小 PDF（1 页，无图片）
        /// </summary>
        private static byte[] BuildPdfWithoutImage()
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
        /// 组装 PDF：对象编号从 1 开始，xref 表偏移精确计算
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
        public void ReadPdfToMarkdown_WithGeneratedPdf_ReturnsPageMarkdown()
        {
            using var stream = new MemoryStream(BuildPdfWithImage());

            var markdown = PDFReader.ReadPdfToMarkdown(stream);

            // 图片不再写入 markdown 占位（改由整页渲染图送模型），markdown 只剩标题与分页
            Assert.Contains("## 第 1 页", markdown);
            Assert.DoesNotContain("![图片]", markdown);
        }

        [Fact]
        public void RenderPagesToPng_WithGraphicsOnlyPdf_ReturnsPngBytes()
        {
            using var stream = new MemoryStream(BuildPdfWithImage());

            var result = PDFReader.RenderPagesToPng(stream, 1.0f, 10);

            var image = Assert.Single(result);
            Assert.Equal("image/png", image.MimeType);
            // PNG 魔数 89 50 4E 47 0D 0A 1A 0A
            Assert.Equal(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }, image.Bytes.Take(8).ToArray());
        }

        [Fact]
        public void RenderPagesToPng_WithTextPdf_ReturnsPngBytes()
        {
            // PDF 一律整页渲染：有文字层的 PDF 同样渲染整页，不再按文字层分流
            using var stream = new MemoryStream(BuildPdfWithoutImage());

            var result = PDFReader.RenderPagesToPng(stream, 1.0f, 10);

            var image = Assert.Single(result);
            Assert.Equal("image/png", image.MimeType);
            Assert.True(image.Bytes.Length > 0);
        }

        [Fact]
        public void RenderPagesToPng_WithInvalidData_ReturnsEmpty()
        {
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes("This is not a pdf file"));

            var result = PDFReader.RenderPagesToPng(stream, 1.0f, 10);

            Assert.Empty(result);
        }
    }
}
