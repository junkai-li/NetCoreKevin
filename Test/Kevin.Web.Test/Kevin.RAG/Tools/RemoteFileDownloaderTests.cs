using Kevin.Common.Helper.FileHandleTools;

namespace Kevin.Unit.Test.Kevin.RAG.Tools
{
    public class RemoteFileDownloaderTests
    {
        [Fact]
        public void DetermineDocumentType_WithPdfContentType_ReturnsPdf()
        {
            var result = RemoteFileDownloader.DetermineDocumentType("application/pdf");

            Assert.Equal(DocumentType.Pdf, result);
        }

        [Fact]
        public void DetermineDocumentType_WithWordContentType_ReturnsWord()
        {
            Assert.Equal(DocumentType.Word, RemoteFileDownloader.DetermineDocumentType("application/msword"));
            Assert.Equal(DocumentType.Word, RemoteFileDownloader.DetermineDocumentType("application/vnd.openxmlformats-officedocument.wordprocessingml.document"));
        }

        [Fact]
        public void DetermineDocumentType_WithExcelContentType_ReturnsExcel()
        {
            Assert.Equal(DocumentType.Excel, RemoteFileDownloader.DetermineDocumentType("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"));
            Assert.Equal(DocumentType.Excel, RemoteFileDownloader.DetermineDocumentType("application/vnd.ms-excel"));
        }

        [Fact]
        public void DetermineDocumentType_WithImageContentType_ReturnsImage()
        {
            Assert.Equal(DocumentType.Image, RemoteFileDownloader.DetermineDocumentType("image/png"));
            Assert.Equal(DocumentType.Image, RemoteFileDownloader.DetermineDocumentType("image/jpeg"));
            Assert.Equal(DocumentType.Image, RemoteFileDownloader.DetermineDocumentType("image/gif"));
        }

        [Fact]
        public void DetermineDocumentType_WithHtmlContentType_ReturnsHtml()
        {
            Assert.Equal(DocumentType.Html, RemoteFileDownloader.DetermineDocumentType("text/html"));
        }

        [Fact]
        public void DetermineDocumentType_WithTextContentType_ReturnsText()
        {
            Assert.Equal(DocumentType.Text, RemoteFileDownloader.DetermineDocumentType("text/plain"));
            Assert.Equal(DocumentType.Text, RemoteFileDownloader.DetermineDocumentType("text/csv"));
        }

        [Fact]
        public void DetermineDocumentType_WithUnknownContentType_ReturnsUnknown()
        {
            Assert.Equal(DocumentType.Unknown, RemoteFileDownloader.DetermineDocumentType("application/octet-stream"));
            Assert.Equal(DocumentType.Unknown, RemoteFileDownloader.DetermineDocumentType(null));
            Assert.Equal(DocumentType.Unknown, RemoteFileDownloader.DetermineDocumentType(""));
        }

        [Fact]
        public void DetermineDocumentType_WithPdfUrl_ReturnsPdf()
        {
            Assert.Equal(DocumentType.Pdf, RemoteFileDownloader.DetermineDocumentType("https://example.com/document.pdf"));
        }

        [Fact]
        public void DetermineDocumentType_WithExcelUrl_ReturnsExcel()
        {
            Assert.Equal(DocumentType.Excel, RemoteFileDownloader.DetermineDocumentType("https://example.com/spreadsheet.xlsx"));
            Assert.Equal(DocumentType.Excel, RemoteFileDownloader.DetermineDocumentType("https://example.com/data.xls"));
        }

        [Fact]
        public void DetermineDocumentType_WithWordUrl_ReturnsWord()
        {
            Assert.Equal(DocumentType.Word, RemoteFileDownloader.DetermineDocumentType("https://example.com/document.docx"));
            Assert.Equal(DocumentType.Word, RemoteFileDownloader.DetermineDocumentType("https://example.com/document.doc"));
        }

        [Fact]
        public void DetermineDocumentType_WithImageUrl_ReturnsImage()
        {
            Assert.Equal(DocumentType.Image, RemoteFileDownloader.DetermineDocumentType("https://example.com/photo.png"));
            Assert.Equal(DocumentType.Image, RemoteFileDownloader.DetermineDocumentType("https://example.com/photo.jpg"));
        }

        [Fact]
        public void DetermineDocumentType_WithHtmlUrl_ReturnsHtml()
        {
            Assert.Equal(DocumentType.Html, RemoteFileDownloader.DetermineDocumentType("https://example.com/page.html"));
        }

        [Fact]
        public void DetermineDocumentType_WithNoExtension_ReturnsUnknown()
        {
            Assert.Equal(DocumentType.Unknown, RemoteFileDownloader.DetermineDocumentType("https://example.com/document"));
        }

        [Fact]
        public void RemoteFileDownloader_Constructor_SetsTimeout()
        {
            using var downloader = new RemoteFileDownloader(60);

            Assert.NotNull(downloader);
        }

        [Fact]
        public async Task DownloadWithMetadataAsync_WithInvalidUrl_ThrowsException()
        {
            using var downloader = new RemoteFileDownloader(5);

            await Assert.ThrowsAsync<HttpRequestException>(() =>
                downloader.DownloadWithMetadataAsync("https://invalid-domain-that-does-not-exist-12345.com/file.pdf"));
        }
    }
}