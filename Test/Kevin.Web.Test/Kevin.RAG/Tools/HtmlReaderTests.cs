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

        [Fact]
        public async Task ExtractImageSourcesAsync_WithDataUri_ReturnsDataUriAsIs()
        {
            var html = "<html><body><img src=\"data:image/png;base64,iVBORw0KGgo=\"></body></html>";
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(html));

            var result = await HtmlReader.ExtractImageSourcesAsync(stream, "https://example.com/a.html");

            var source = Assert.Single(result);
            Assert.StartsWith("data:image/png;base64,", source);
        }

        [Fact]
        public async Task ExtractImageSourcesAsync_WithAbsoluteUrl_ReturnsAsIs()
        {
            var html = "<html><body><img src=\"https://cdn.example.com/img/1.png\"></body></html>";
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(html));

            var result = await HtmlReader.ExtractImageSourcesAsync(stream, null);

            Assert.Equal("https://cdn.example.com/img/1.png", Assert.Single(result));
        }

        [Fact]
        public async Task ExtractImageSourcesAsync_WithRelativePath_ResolvesAgainstBaseUrl()
        {
            var html = "<html><body><img src=\"images/1.png\"></body></html>";
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(html));

            var result = await HtmlReader.ExtractImageSourcesAsync(stream, "https://example.com/docs/page.html");

            Assert.Equal("https://example.com/docs/images/1.png", Assert.Single(result));
        }

        [Fact]
        public async Task ExtractImageSourcesAsync_WithProtocolRelativePath_UsesBaseUrlScheme()
        {
            var html = "<html><body><img src=\"//cdn.example.com/img/1.png\"></body></html>";
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(html));

            var result = await HtmlReader.ExtractImageSourcesAsync(stream, "https://example.com/page.html");

            Assert.Equal("https://cdn.example.com/img/1.png", Assert.Single(result));
        }

        [Fact]
        public async Task ExtractImageSourcesAsync_WithRelativePathAndNoBaseUrl_SkipsIt()
        {
            var html = "<html><body><img src=\"images/1.png\"></body></html>";
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(html));

            var result = await HtmlReader.ExtractImageSourcesAsync(stream, null);

            Assert.Empty(result);
        }

        [Fact]
        public async Task ExtractImageSourcesAsync_WithNoImages_ReturnsEmpty()
        {
            var html = "<html><body><p>no images here</p></body></html>";
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(html));

            var result = await HtmlReader.ExtractImageSourcesAsync(stream, "https://example.com/a.html");

            Assert.Empty(result);
        }

        [Fact]
        public async Task ExtractImageSourcesAsync_WithMixedSources_CollectsAll()
        {
            var html = "<html><body>" +
                       "<img src=\"data:image/gif;base64,R0lGOD=\">" +
                       "<img src=\"https://cdn.example.com/2.jpg\">" +
                       "<img src=\"./local.png\">" +
                       "<img src=\"//other.example.com/3.webp\">" +
                       "</body></html>";
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(html));

            var result = await HtmlReader.ExtractImageSourcesAsync(stream, "https://example.com/page.html");

            Assert.Equal(4, result.Count);
            Assert.StartsWith("data:image/gif;base64,", result[0]);
            Assert.Equal("https://cdn.example.com/2.jpg", result[1]);
            Assert.Equal("https://example.com/local.png", result[2]);
            Assert.Equal("https://other.example.com/3.webp", result[3]);
        }
    }
}