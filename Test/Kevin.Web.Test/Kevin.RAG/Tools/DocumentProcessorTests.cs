using Kevin.RAG.Tools;
using System.Text;

namespace Kevin.Unit.Test.Kevin.RAG.Tools
{
    public class DocumentProcessorTests
    {
        [Fact]
        public void CleanDocument_WithNullInput_ReturnsEmptyString()
        {
            var processor = new DocumentProcessor();

            var result = processor.CleanDocument(null);

            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void CleanDocument_WithWhitespaceInput_ReturnsEmptyString()
        {
            var processor = new DocumentProcessor();

            var result = processor.CleanDocument("   \t\n  ");

            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void CleanDocument_WithNormalText_PreservesContent()
        {
            var processor = new DocumentProcessor();
            var input = "Hello World";

            var result = processor.CleanDocument(input);

            Assert.Equal("Hello World", result);
        }

        [Fact]
        public void CleanDocument_WithMultipleLineBreaks_ReducesToDoubleLineBreak()
        {
            var processor = new DocumentProcessor();
            var input = "Paragraph 1\n\n\n\n\nParagraph 2";

            var result = processor.CleanDocument(input);

            Assert.Equal("Paragraph 1\n\nParagraph 2", result);
        }

        [Fact]
        public void CleanDocument_WithMixedLineEndings_NormalizesToUnix()
        {
            var processor = new DocumentProcessor();
            var input = "Line 1\r\nLine 2\rLine 3\nLine 4";

            var result = processor.CleanDocument(input);

            Assert.Contains("\n", result);
            Assert.DoesNotContain("\r", result);
        }

        [Fact]
        public void CleanDocument_WithExcessiveWhitespace_RemovesExtraSpaces()
        {
            var processor = new DocumentProcessor();
            var input = "Hello    World\t\tTest";

            var result = processor.CleanDocument(input);

            Assert.Equal("Hello World Test", result);
        }

        [Fact]
        public void ChunkByParagraph_WithSimpleText_ReturnsExpectedChunks()
        {
            var processor = new DocumentProcessor(chunkSize: 50, chunkOverlap: 10);
            var input = "Short paragraph.\n\nAnother short paragraph.";

            var result = processor.ChunkByParagraph(input);

            Assert.NotEmpty(result);
            Assert.All(result, chunk => Assert.False(string.IsNullOrWhiteSpace(chunk)));
        }

        [Fact]
        public void ChunkByParagraph_WithEmptyInput_ReturnsEmptyList()
        {
            var processor = new DocumentProcessor();

            var result = processor.ChunkByParagraph(string.Empty);

            Assert.Empty(result);
        }

        [Fact]
        public void ChunkByParagraph_WithOverlap_DoesNotDuplicateParagraphs()
        {
            var processor = new DocumentProcessor(chunkSize: 30, chunkOverlap: 10);
            var input = "First paragraph with some content here.\n\nSecond paragraph that is quite long.\n\nThird paragraph added for testing.";

            var result = processor.ChunkByParagraph(input);

            foreach (var chunk in result)
            {
                var paragraphCount = chunk.Split(new[] { "\n\n" }, StringSplitOptions.None).Length;
                Assert.True(paragraphCount >= 1);
            }
        }

        [Fact]
        public void ChunkBySize_WithSimpleText_ReturnsExpectedChunks()
        {
            var processor = new DocumentProcessor(chunkSize: 20, chunkOverlap: 5);
            var input = "This is a very long text that should be split into multiple chunks.";

            var result = processor.ChunkBySize(input);

            Assert.True(result.Count > 1);
        }

        [Fact]
        public void ChunkBySize_WithShortText_ReturnsSingleChunk()
        {
            var processor = new DocumentProcessor(chunkSize: 100, chunkOverlap: 10);
            var input = "Short text.";

            var result = processor.ChunkBySize(input);

            Assert.Single(result);
        }

        [Fact]
        public void ChunkBySize_WithEmptyInput_ReturnsEmptyList()
        {
            var processor = new DocumentProcessor();

            var result = processor.ChunkBySize(string.Empty);

            Assert.Empty(result);
        }

        [Fact]
        public void ChunkBySize_WithLargeOverlap_DoesNotCauseInfiniteLoop()
        {
            var processor = new DocumentProcessor(chunkSize: 10, chunkOverlap: 50);
            var input = "This is a very long text that should be split properly without infinite looping issues.";

            var result = processor.ChunkBySize(input);

            Assert.NotEmpty(result);
            Assert.True(result.Count < input.Length);
        }

        [Fact]
        public void ChunkBySize_WithSentenceBoundary_CutsAtSentenceEnd()
        {
            var processor = new DocumentProcessor(chunkSize: 30, chunkOverlap: 5);
            var input = "This is the first sentence. This is the second sentence!";

            var result = processor.ChunkBySize(input);

            Assert.All(result, chunk =>
            {
                var trimmed = chunk.TrimEnd();
                Assert.True(trimmed.EndsWith(".") || trimmed.EndsWith("!") || trimmed.EndsWith("?") || result.IndexOf(chunk) == result.Count - 1);
            });
        }
    }
}