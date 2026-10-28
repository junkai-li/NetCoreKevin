using DocumentFormat.OpenXml.Packaging;
using Kevin.Common.Helper.FileHandleTools;
using System.Text;

namespace Kevin.Unit.Test.Kevin.RAG.Tools
{
    public class WordReaderTests
    {
        /// <summary>1x1 红色像素的合法 PNG 字节，用作 docx 内嵌图片样本</summary>
        private static readonly byte[] SamplePngBytes = Convert.FromBase64String(
            "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNkYPhfDwAChwGA60e6kgAAAABJRU5ErkJggg==");

        /// <summary>
        /// 用 OpenXML SDK 生成一份内嵌 1 张 PNG 的 docx 字节
        /// </summary>
        private static byte[] BuildDocxWithImage()
        {
            using var ms = new MemoryStream();
            using (var wordDoc = WordprocessingDocument.Create(ms, DocumentFormat.OpenXml.WordprocessingDocumentType.Document))
            {
                var mainPart = wordDoc.AddMainDocumentPart();
                mainPart.Document = new DocumentFormat.OpenXml.Wordprocessing.Document(
                    new DocumentFormat.OpenXml.Wordprocessing.Body());
                var imagePart = mainPart.AddImagePart(ImagePartType.Png);
                imagePart.FeedData(new MemoryStream(SamplePngBytes));

                // 通过 InnerXml 注入含 a:blip(r:embed) 的段落，与 Word 实际产出的结构一致
                mainPart.Document.Body.InnerXml =
                    $"<w:p xmlns:w=\"http://schemas.openxmlformats.org/wordprocessingml/2006/main\" xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\">" +
                    $"<w:r><w:drawing><wp:inline xmlns:wp=\"http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing\">" +
                    $"<a:graphic xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\">" +
                    $"<a:graphicData uri=\"http://schemas.openxmlformats.org/drawingml/2006/picture\">" +
                    $"<pic:pic xmlns:pic=\"http://schemas.openxmlformats.org/drawingml/2006/picture\">" +
                    $"<pic:blipFill><a:blip r:embed=\"{mainPart.GetIdOfPart(imagePart)}\"/></pic:blipFill>" +
                    $"</pic:pic></a:graphicData></a:graphic></wp:inline></w:drawing></w:r></w:p>";
                mainPart.Document.Save();
            }
            return ms.ToArray();
        }

        /// <summary>
        /// 用 OpenXML SDK 生成一份纯文本（无图片）的 docx 字节
        /// </summary>
        private static byte[] BuildDocxWithoutImage()
        {
            using var ms = new MemoryStream();
            using (var wordDoc = WordprocessingDocument.Create(ms, DocumentFormat.OpenXml.WordprocessingDocumentType.Document))
            {
                var mainPart = wordDoc.AddMainDocumentPart();
                mainPart.Document = new DocumentFormat.OpenXml.Wordprocessing.Document(
                    new DocumentFormat.OpenXml.Wordprocessing.Body());
                mainPart.Document.Body.InnerXml =
                    "<w:p xmlns:w=\"http://schemas.openxmlformats.org/wordprocessingml/2006/main\">" +
                    "<w:r><w:t>Hello Word</w:t></w:r></w:p>";
                mainPart.Document.Save();
            }
            return ms.ToArray();
        }

        [Fact]
        public void ExtractImages_WithEmbeddedImage_ReturnsImageBytesAndMime()
        {
            using var stream = new MemoryStream(BuildDocxWithImage());

            var result = WordReader.ExtractImages(stream);

            var image = Assert.Single(result);
            Assert.Equal("image/png", image.MimeType);
            Assert.Equal(SamplePngBytes, image.Bytes);
        }

        [Fact]
        public void ExtractImages_WithTextOnlyDocument_ReturnsEmpty()
        {
            using var stream = new MemoryStream(BuildDocxWithoutImage());

            var result = WordReader.ExtractImages(stream);

            Assert.Empty(result);
        }

        [Fact]
        public void ExtractImages_WithInvalidData_ReturnsEmpty()
        {
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes("This is not a docx file"));

            var result = WordReader.ExtractImages(stream);

            Assert.Empty(result);
        }

        [Fact]
        public void ExtractImages_WithNullStream_ReturnsEmpty()
        {
            var result = WordReader.ExtractImages(null);

            Assert.Empty(result);
        }
    }
}
