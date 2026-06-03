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
    }
}