using Kevin.RAG;
using System.Text;

namespace Kevin.Unit.Test.Kevin.RAG.Tools
{
    public class TextStreamReaderTests
    {
        [Fact]
        public void ReadTextFromStream_WithNullStream_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => TextStreamReader.ReadTextFromStream(null));
        }

        [Fact]
        public void ReadTextFromStream_WithUnreadableStream_ThrowsArgumentException()
        {
            var stream = new MemoryStream();
            stream.Close();

            Assert.Throws<ArgumentException>(() => TextStreamReader.ReadTextFromStream(stream));
        }

        [Fact]
        public void ReadTextFromStream_WithUtf8Text_ReturnsCorrectContent()
        {
            var content = "Hello World Test";
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

            var result = TextStreamReader.ReadTextFromStream(stream);

            Assert.Equal(content, result);
        }

        [Fact]
        public void ReadTextFromStream_WithUtf8BomText_ReturnsCorrectContent()
        {
            var content = "Hello World";
            var bytes = new byte[] { 0xEF, 0xBB, 0xBF };
            bytes = bytes.Concat(Encoding.UTF8.GetBytes(content)).ToArray();
            using var stream = new MemoryStream(bytes);

            var result = TextStreamReader.ReadTextFromStream(stream);

            Assert.Equal(content, result);
        }

        [Fact]
        public void ReadTextFromStream_WithUtf16LeText_ReturnsCorrectContent()
        {
            var content = "Hello World";
            var bytes = new byte[] { 0xFF, 0xFE };
            bytes = bytes.Concat(Encoding.Unicode.GetBytes(content)).ToArray();
            using var stream = new MemoryStream(bytes);

            var result = TextStreamReader.ReadTextFromStream(stream);

            Assert.Equal(content, result);
        }

        [Fact]
        public void ReadTextFromStream_WithUtf16BeText_ReturnsCorrectContent()
        {
            var content = "Hello World";
            var bytes = new byte[] { 0xFE, 0xFF };
            bytes = bytes.Concat(Encoding.BigEndianUnicode.GetBytes(content)).ToArray();
            using var stream = new MemoryStream(bytes);

            var result = TextStreamReader.ReadTextFromStream(stream);

            Assert.Equal(content, result);
        }

        [Fact]
        public void ReadMarkdownFromStream_WithMarkdownContent_ReturnsCorrectStructure()
        {
            var content = "# Hello World\n\nThis is a test.";
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

            var result = TextStreamReader.ReadMarkdownFromStream(stream);

            Assert.Equal(content, result.RawContent);
            Assert.True(result.IsMarkdown);
            Assert.Equal("Hello World", result.Title);
            Assert.Equal(3, result.LineCount);
            Assert.Equal(content.Length, result.CharacterCount);
        }

        [Fact]
        public void ReadMarkdownFromStream_WithPlainText_ReturnsIsMarkdownFalse()
        {
            var content = "This is plain text without any markdown formatting.";
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

            var result = TextStreamReader.ReadMarkdownFromStream(stream);

            Assert.Equal(content, result.RawContent);
            Assert.False(result.IsMarkdown);
        }

        [Fact]
        public void ReadMarkdownFromText_WithMarkdownContent_ReturnsCorrectStructure()
        {
            var content = "# Test Title\n\n- List item 1\n- List item 2";

            var result = TextStreamReader.ReadMarkdownFromText(content);

            Assert.Equal(content, result.RawContent);
            Assert.True(result.IsMarkdown);
            Assert.Equal("Test Title", result.Title);
            Assert.Equal(3, result.LineCount);
        }

        [Fact]
        public void ReadMarkdownFromText_WithBulletList_ReturnsIsMarkdownTrue()
        {
            var content = "- Item 1\n- Item 2\n- Item 3";

            var result = TextStreamReader.ReadMarkdownFromText(content);

            Assert.True(result.IsMarkdown);
        }

        [Fact]
        public void ReadMarkdownFromText_WithNumberedList_ReturnsIsMarkdownTrue()
        {
            var content = "1. First item\n2. Second item\n3. Third item";

            var result = TextStreamReader.ReadMarkdownFromText(content);

            Assert.True(result.IsMarkdown);
        }

        [Fact]
        public void ReadMarkdownFromText_WithCodeBlock_ReturnsIsMarkdownTrue()
        {
            var content = "```\ncode here\n```";

            var result = TextStreamReader.ReadMarkdownFromText(content);

            Assert.True(result.IsMarkdown);
        }

        [Fact]
        public void ReadMarkdownFromText_WithImageLink_ReturnsIsMarkdownTrue()
        {
            var content = "![Alt text](image.png)";

            var result = TextStreamReader.ReadMarkdownFromText(content);

            Assert.True(result.IsMarkdown);
        }

        [Fact]
        public void ReadMarkdownFromText_WithHyperlink_ReturnsIsMarkdownTrue()
        {
            var content = "[Link text](https://example.com)";

            var result = TextStreamReader.ReadMarkdownFromText(content);

            Assert.True(result.IsMarkdown);
        }

        [Fact]
        public void ReadMarkdownFromText_WithoutTitle_ReturnsEmptyTitle()
        {
            var content = "Just some text without a title prefix.";

            var result = TextStreamReader.ReadMarkdownFromText(content);

            Assert.Equal(string.Empty, result.Title);
        }

        [Fact]
        public void MarkdownContent_ToString_ReturnsJson()
        {
            var content = new MarkdownContent
            {
                RawContent = "Test",
                Title = "Test Title",
                IsMarkdown = true,
                LineCount = 1,
                CharacterCount = 4
            };

            var result = content.ToString();

            Assert.Contains("Test", result);
            Assert.Contains("Test Title", result);
        }
    }
}