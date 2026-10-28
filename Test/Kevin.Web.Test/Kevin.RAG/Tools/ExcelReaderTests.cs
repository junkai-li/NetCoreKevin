using Kevin.Common.Helper.FileHandleTools;
using System.Text;

namespace Kevin.Unit.Test.Kevin.RAG.Tools
{
    public class ExcelReaderTests
    {
        [Fact]
        public void IsExcelFile_WithValidExtensions_ReturnsTrue()
        {
            Assert.True(ExcelReader.IsExcelFile("test.xlsx"));
            Assert.True(ExcelReader.IsExcelFile("test.xls"));
        }

        [Fact]
        public void IsExcelFile_WithInvalidExtensions_ReturnsFalse()
        {
            Assert.False(ExcelReader.IsExcelFile("test.csv"));
            Assert.False(ExcelReader.IsExcelFile("test.txt"));
            Assert.False(ExcelReader.IsExcelFile("test.pdf"));
        }

        [Fact]
        public void IsExcelFile_WithNullOrEmpty_ReturnsFalse()
        {
            Assert.False(ExcelReader.IsExcelFile(null));
            Assert.False(ExcelReader.IsExcelFile(""));
            Assert.False(ExcelReader.IsExcelFile("   "));
        }

        [Fact]
        public void IsExcelFileByContentType_WithExcelContentType_ReturnsTrue()
        {
            Assert.True(ExcelReader.IsExcelFileByContentType("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"));
            Assert.True(ExcelReader.IsExcelFileByContentType("application/vnd.ms-excel"));
        }

        [Fact]
        public void IsExcelFileByContentType_WithNonExcelContentType_ReturnsFalse()
        {
            Assert.False(ExcelReader.IsExcelFileByContentType("text/csv"));
            Assert.False(ExcelReader.IsExcelFileByContentType("text/plain"));
            Assert.False(ExcelReader.IsExcelFileByContentType("application/json"));
        }

        [Fact]
        public void IsExcelFileByContentType_WithNullOrEmpty_ReturnsFalse()
        {
            Assert.False(ExcelReader.IsExcelFileByContentType(null));
            Assert.False(ExcelReader.IsExcelFileByContentType(""));
        }

        [Fact]
        public void ReadExcelToMarkdown_WithEmptyStream_ReturnsEmptyString()
        {
            using var stream = new MemoryStream(Array.Empty<byte>());

            var result = ExcelReader.ReadExcelToMarkdown(stream, "EmptyFile");

            Assert.Equal("# EmptyFile\n\n（无法读取Excel文件：文件为空或格式不正确）\n", result);
        }

        [Fact]
        public void ReadExcelToMarkdown_WithInvalidData_ReturnsErrorMessage()
        {
            var invalidData = Encoding.UTF8.GetBytes("This is not an Excel file");
            using var stream = new MemoryStream(invalidData);

            var result = ExcelReader.ReadExcelToMarkdown(stream, "InvalidFile");

            Assert.Contains("无法读取Excel文件", result);
        }

        [Fact]
        public async Task ReadExcelToMarkdownAsync_WithEmptyStream_ReturnsEmptyString()
        {
            using var stream = new MemoryStream(Array.Empty<byte>());

            var result = await ExcelReader.ReadExcelToMarkdownAsync(stream, "EmptyFile");

            Assert.Equal("# EmptyFile\n\n（无法读取Excel文件：文件为空或格式不正确）\n", result);
        }

        [Fact]
        public async Task ReadExcelToMarkdownAsync_WithInvalidData_ReturnsErrorMessage()
        {
            var invalidData = Encoding.UTF8.GetBytes("This is not an Excel file");
            using var stream = new MemoryStream(invalidData);

            var result = await ExcelReader.ReadExcelToMarkdownAsync(stream, "InvalidFile");

            Assert.Contains("无法读取Excel文件", result);
        }

        [Fact]
        public void ReadExcelToMarkdown_WithNullStream_ReturnsEmptyString()
        {
            var result = ExcelReader.ReadExcelToMarkdown(null, "NullFile");

            Assert.Equal(string.Empty, result);
        }

        /// <summary>1x1 红色像素的合法 PNG 字节，用作 xlsx 内嵌图片样本</summary>
        private static readonly byte[] SamplePngBytes = Convert.FromBase64String(
            "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNkYPhfDwAChwGA60e6kgAAAABJRU5ErkJggg==");

        /// <summary>用 NPOI 生成一份内嵌 1 张 PNG 的 xlsx 字节</summary>
        private static byte[] BuildXlsxWithImage()
        {
            using var ms = new MemoryStream();
            var workbook = new NPOI.XSSF.UserModel.XSSFWorkbook();
            var sheet = workbook.CreateSheet("Sheet1");
            var patriarch = sheet.CreateDrawingPatriarch();
            var pictureIndex = workbook.AddPicture(SamplePngBytes, NPOI.SS.UserModel.PictureType.PNG);
            patriarch.CreatePicture(new NPOI.XSSF.UserModel.XSSFClientAnchor(0, 0, 0, 0, 0, 0, 2, 2), pictureIndex);
            workbook.Write(ms, leaveOpen: true);
            return ms.ToArray();
        }

        /// <summary>用 NPOI 生成一份纯文本（无图片）的 xlsx 字节</summary>
        private static byte[] BuildXlsxWithoutImage()
        {
            using var ms = new MemoryStream();
            var workbook = new NPOI.XSSF.UserModel.XSSFWorkbook();
            var sheet = workbook.CreateSheet("Sheet1");
            sheet.CreateRow(0).CreateCell(0).SetCellValue("Hello Excel");
            workbook.Write(ms, leaveOpen: true);
            return ms.ToArray();
        }

        [Fact]
        public void ExtractImages_WithEmbeddedImage_ReturnsImageBytesAndMime()
        {
            using var stream = new MemoryStream(BuildXlsxWithImage());

            var result = ExcelReader.ExtractImages(stream);

            var image = Assert.Single(result);
            Assert.Equal("image/png", image.MimeType);
            Assert.Equal(SamplePngBytes, image.Bytes);
        }

        [Fact]
        public void ExtractImages_WithTextOnlyWorkbook_ReturnsEmpty()
        {
            using var stream = new MemoryStream(BuildXlsxWithoutImage());

            var result = ExcelReader.ExtractImages(stream);

            Assert.Empty(result);
        }

        [Fact]
        public void ExtractImages_WithInvalidData_ReturnsEmpty()
        {
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes("This is not an Excel file"));

            var result = ExcelReader.ExtractImages(stream);

            Assert.Empty(result);
        }

        [Fact]
        public void ExtractImages_WithNullStream_ReturnsEmpty()
        {
            var result = ExcelReader.ExtractImages(null);

            Assert.Empty(result);
        }
    }
}