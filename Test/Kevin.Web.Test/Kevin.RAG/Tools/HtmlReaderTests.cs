using Kevin.Common.Helper.FileHandleTools;
using System.Text;

namespace Kevin.Unit.Test.Kevin.RAG.Tools
{
    public class HtmlReaderTests
    {
        [Fact]
        public async Task ExtractTextFromStreamAsync_WithNullStream_ReturnsEmptyString()
        {
            var result = await HtmlReader.ExtractTextFromStreamAsync(null);

            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public async Task ExtractTextFromStreamAsync_WithSimpleHtml_ExtractsText()
        {
            var html = "<html><body><p>Hello World</p></body></html>";
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(html));

            var result = await HtmlReader.ExtractTextFromStreamAsync(stream);

            Assert.Contains("Hello World", result);
        }

        [Fact]
        public async Task ExtractTextFromStreamAsync_WithNestedHtml_ExtractsAllText()
        {
            var html = "<div><p>First <strong>paragraph</strong></p><p>Second paragraph</p></div>";
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(html));

            var result = await HtmlReader.ExtractTextFromStreamAsync(stream);

            Assert.Contains("First", result);
            Assert.Contains("paragraph", result);
            Assert.Contains("Second", result);
        }

        [Fact]
        public async Task ExtractTextFromStreamAsync_WithScriptTags_RemovesScriptContent()
        {
            var html = "<html><body><p>Visible text</p><script>alert('hidden');</script></body></html>";
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(html));

            var result = await HtmlReader.ExtractTextFromStreamAsync(stream);

            Assert.Contains("Visible text", result);
            Assert.DoesNotContain("alert", result);
        }

        [Fact]
        public async Task ExtractTextFromStreamAsync_WithStyleTags_RemovesStyleContent()
        {
            var html = "<html><head><style>.class { color: red; }</style></head><body><p>Visible text</p></body></html>";
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(html));

            var result = await HtmlReader.ExtractTextFromStreamAsync(stream);

            Assert.Contains("Visible text", result);
            Assert.DoesNotContain("color", result);
        }

        [Fact]
        public async Task ExtractTextFromStreamAsync_WithMultipleBlockElements_PreservesStructure()
        {
            var html = "<div><p>First paragraph</p><p>Second paragraph</p></div>";
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(html));

            var result = await HtmlReader.ExtractTextFromStreamAsync(stream);

            Assert.Contains("First", result);
            Assert.Contains("Second", result);
        }

        [Fact]
        public async Task ExtractTextFromStreamAsync_WithHtmlEntities_DecodesEntities()
        {
            var html = "<html><body><p>Hello &amp; World &lt;3&gt;</p></body></html>";
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(html));

            var result = await HtmlReader.ExtractTextFromStreamAsync(stream);

            Assert.Contains("Hello & World", result);
            Assert.Contains("<3>", result);
        }

        [Fact]
        public async Task ExtractTextFromTextAsync_WithSimpleHtml_ExtractsText()
        {
            var html = "<html><body><p>Hello World</p></body></html>";

            var result = await HtmlReader.ExtractTextFromTextAsync(html);

            Assert.Contains("Hello World", result);
        }

        [Fact]
        public async Task ExtractTextFromTextAsync_WithEmptyHtml_ReturnsEmptyString()
        {
            var result = await HtmlReader.ExtractTextFromTextAsync(string.Empty);

            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public async Task ExtractTextFromTextAsync_WithWhitespaceOnlyHtml_ReturnsEmptyString()
        {
            var result = await HtmlReader.ExtractTextFromTextAsync("   \t\n  ");

            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public async Task ExtractTextFromStreamAsync_WithSeekableStream_ReadsFromBeginning()
        {
            var html = "<html><body><p>Test content</p></body></html>";
            var streamContent = Encoding.UTF8.GetBytes(html);
            using var stream = new MemoryStream();
            stream.Write(streamContent, 0, streamContent.Length);
            stream.Position = 0;

            var result = await HtmlReader.ExtractTextFromStreamAsync(stream);

            Assert.Contains("Test content", result);
        }
    }
}