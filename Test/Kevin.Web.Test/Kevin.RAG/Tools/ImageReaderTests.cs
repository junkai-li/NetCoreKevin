using Kevin.Common.Helper.FileHandleTools;

namespace Kevin.Unit.Test.Kevin.RAG.Tools
{
    public class ImageReaderTests
    {
        [Fact]
        public void IsImageFile_WithValidImageExtensions_ReturnsTrue()
        {
            Assert.True(ImageReader.IsImageFile("test.png"));
            Assert.True(ImageReader.IsImageFile("test.jpg"));
            Assert.True(ImageReader.IsImageFile("test.jpeg"));
            Assert.True(ImageReader.IsImageFile("test.gif"));
            Assert.True(ImageReader.IsImageFile("test.bmp"));
            Assert.True(ImageReader.IsImageFile("test.webp"));
            Assert.True(ImageReader.IsImageFile("test.tiff"));
            Assert.True(ImageReader.IsImageFile("test.tif"));
        }

        [Fact]
        public void IsImageFile_WithInvalidExtensions_ReturnsFalse()
        {
            Assert.False(ImageReader.IsImageFile("test.txt"));
            Assert.False(ImageReader.IsImageFile("test.pdf"));
            Assert.False(ImageReader.IsImageFile("test.doc"));
            Assert.False(ImageReader.IsImageFile("test.docx"));
        }

        [Fact]
        public void IsImageFile_WithNullOrEmpty_ReturnsFalse()
        {
            Assert.False(ImageReader.IsImageFile(null));
            Assert.False(ImageReader.IsImageFile(""));
            Assert.False(ImageReader.IsImageFile("   "));
        }

        [Fact]
        public void IsImageFileByContentType_WithImageContentType_ReturnsTrue()
        {
            Assert.True(ImageReader.IsImageFileByContentType("image/png"));
            Assert.True(ImageReader.IsImageFileByContentType("image/jpeg"));
            Assert.True(ImageReader.IsImageFileByContentType("IMAGE/GIF"));
            Assert.True(ImageReader.IsImageFileByContentType("image/webp"));
        }

        [Fact]
        public void IsImageFileByContentType_WithNonImageContentType_ReturnsFalse()
        {
            Assert.False(ImageReader.IsImageFileByContentType("application/pdf"));
            Assert.False(ImageReader.IsImageFileByContentType("text/html"));
            Assert.False(ImageReader.IsImageFileByContentType("application/json"));
        }

        [Fact]
        public void IsImageFileByContentType_WithNullOrEmpty_ReturnsFalse()
        {
            Assert.False(ImageReader.IsImageFileByContentType(null));
            Assert.False(ImageReader.IsImageFileByContentType(""));
            Assert.False(ImageReader.IsImageFileByContentType("   "));
        }

        [Fact]
        public void ExtractTextFromImage_WithEmptyBytes_ReturnsEmptyString()
        {
            var result = ImageReader.ExtractTextFromImage(Array.Empty<byte>());

            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void ExtractTextFromImage_WithNullBytes_ReturnsEmptyString()
        {
            var result = ImageReader.ExtractTextFromImage((byte[])null);

            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public async Task ExtractTextFromImageAsync_WithEmptyBytes_ReturnsEmptyString()
        {
            var result = await ImageReader.ExtractTextFromImageAsync(Array.Empty<byte>());

            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public async Task ExtractTextFromImageAsync_WithNullBytes_ReturnsEmptyString()
        {
            var result = await ImageReader.ExtractTextFromImageAsync((byte[])null);

            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void DescribeImage_WithEmptyBytes_ReturnsEmptyString()
        {
            var result = ImageReader.DescribeImage(Array.Empty<byte>());

            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void DescribeImage_WithNullBytes_ReturnsEmptyString()
        {
            var result = ImageReader.DescribeImage((byte[])null);

            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void ImageToBase64_WithValidBytes_ReturnsBase64String()
        {
            var imageBytes = new byte[] { 1, 2, 3, 4, 5 };

            var result = ImageReader.ImageToBase64(imageBytes);

            Assert.Equal("AQIDBAU=", result);
        }

        [Fact]
        public void Base64ToImage_WithValidBase64_ReturnsBytes()
        {
            var base64String = "AQIDBAU=";

            var result = ImageReader.Base64ToImage(base64String);

            Assert.Equal(new byte[] { 1, 2, 3, 4, 5 }, result);
        }

        [Fact]
        public void Base64ToImage_WithNullOrEmpty_ReturnsEmptyArray()
        {
            Assert.Equal(Array.Empty<byte>(), ImageReader.Base64ToImage(null));
            Assert.Equal(Array.Empty<byte>(), ImageReader.Base64ToImage(""));
            Assert.Equal(Array.Empty<byte>(), ImageReader.Base64ToImage("   "));
        }

        [Fact]
        public async Task ExtractTextFromImageAsync_WithStream_ReturnsString()
        {
            using var stream = new MemoryStream(Array.Empty<byte>());

            var result = await ImageReader.ExtractTextFromImageAsync(stream);

            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public async Task DescribeImageAsync_WithStream_ReturnsString()
        {
            using var stream = new MemoryStream(Array.Empty<byte>());

            var result = await ImageReader.DescribeImageAsync(stream);

            Assert.Equal(string.Empty, result);
        }
    }
}