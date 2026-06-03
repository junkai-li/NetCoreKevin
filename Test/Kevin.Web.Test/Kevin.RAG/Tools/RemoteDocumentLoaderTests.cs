using Kevin.Common.Helper.FileHandleTools;

namespace Kevin.Unit.Test.Kevin.RAG.Tools
{
    public class RemoteDocumentLoaderTests
    {
        [Fact]
        public void Default_ReturnsSingletonInstance()
        {
            var instance1 = RemoteDocumentLoader.Default;
            var instance2 = RemoteDocumentLoader.Default;

            Assert.Same(instance1, instance2);
        }

        [Fact]
        public void LoadDocumentStreamFromUrlAsync_WithInvalidUrl_ThrowsException()
        {
            Assert.ThrowsAsync<HttpRequestException>(() =>
                RemoteDocumentLoader.LoadDocumentStreamFromUrlAsync("https://invalid-domain-that-does-not-exist-12345.com/file.pdf"));
        }

        [Fact]
        public async Task LoadDocumentFromUrlAsync_WithInvalidUrl_ThrowsException()
        {
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                RemoteDocumentLoader.LoadDocumentFromUrlAsync("https://invalid-domain-that-does-not-exist-12345.com/file.pdf"));
        }

        [Fact]
        public async Task LoadDocumentStreamFromUrlAsync_WithValidUrl_ReturnsStream()
        {
            using var client = new HttpClient();
            var testUrl = "https://www.w3.org/TR/PNG/iso_8859-1.txt";

            var stream = await RemoteDocumentLoader.LoadDocumentStreamFromUrlAsync(testUrl);

            Assert.NotNull(stream);
            Assert.True(stream.CanRead);
            stream.Dispose();
        }

        [Fact]
        public async Task LoadDocumentStreamFromUrlAsync_WithNullUrl_ThrowsException()
        {
            await Assert.ThrowsAsync<UriFormatException>(() =>
                RemoteDocumentLoader.LoadDocumentStreamFromUrlAsync(null));
        }

        [Fact]
        public async Task LoadDocumentStreamFromUrlAsync_WithEmptyUrl_ThrowsException()
        {
            await Assert.ThrowsAsync<UriFormatException>(() =>
                RemoteDocumentLoader.LoadDocumentStreamFromUrlAsync(""));
        }
    }
}