using NetCore.Util;

namespace Kevin.Unit.Test.Kevin.Common
{
    public class FileHelperTests
    {
        [Fact]
        public void SniffImageMime_WithPngMagicBytes_ReturnsPng()
        {
            var bytes = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };

            var result = FileHelper.SniffImageMime(bytes);

            Assert.Equal("image/png", result);
        }

        [Fact]
        public void SniffImageMime_WithJpegMagicBytes_ReturnsJpeg()
        {
            var bytes = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46 };

            var result = FileHelper.SniffImageMime(bytes);

            Assert.Equal("image/jpeg", result);
        }

        [Fact]
        public void SniffImageMime_WithGifMagicBytes_ReturnsGif()
        {
            var gif87 = System.Text.Encoding.ASCII.GetBytes("GIF87a");
            var gif89 = System.Text.Encoding.ASCII.GetBytes("GIF89a");

            Assert.Equal("image/gif", FileHelper.SniffImageMime(gif87));
            Assert.Equal("image/gif", FileHelper.SniffImageMime(gif89));
        }

        [Fact]
        public void SniffImageMime_WithWebpMagicBytes_ReturnsWebp()
        {
            var bytes = new byte[12];
            bytes[0] = (byte)'R'; bytes[1] = (byte)'I'; bytes[2] = (byte)'F'; bytes[3] = (byte)'F';
            bytes[8] = (byte)'W'; bytes[9] = (byte)'E'; bytes[10] = (byte)'B'; bytes[11] = (byte)'P';

            var result = FileHelper.SniffImageMime(bytes);

            Assert.Equal("image/webp", result);
        }

        [Fact]
        public void SniffImageMime_WithBmpMagicBytes_ReturnsBmp()
        {
            var bytes = new byte[] { (byte)'B', (byte)'M', 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            var result = FileHelper.SniffImageMime(bytes);

            Assert.Equal("image/bmp", result);
        }

        [Fact]
        public void SniffImageMime_WithPdfHeader_ReturnsNull()
        {
            var bytes = System.Text.Encoding.ASCII.GetBytes("%PDF-1.4");

            var result = FileHelper.SniffImageMime(bytes);

            Assert.Null(result);
        }

        [Fact]
        public void SniffImageMime_WithNullOrTooShortBytes_ReturnsNull()
        {
            Assert.Null(FileHelper.SniffImageMime(null));
            Assert.Null(FileHelper.SniffImageMime(Array.Empty<byte>()));
            Assert.Null(FileHelper.SniffImageMime(new byte[] { 0x89, 0x50 }));
        }
    }
}
