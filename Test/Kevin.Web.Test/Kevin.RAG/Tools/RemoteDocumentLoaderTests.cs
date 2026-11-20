using Kevin.Common.Helper.FileHandleTools;
using System.Net;
using System.Net.Sockets;
using System.Text;

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
        public async Task LoadDocumentStreamFromUrlAsync_WithInvalidUrl_ThrowsException()
        {
            await Assert.ThrowsAsync<HttpRequestException>(() =>
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
            // 用本地回环的极简 HTTP 服务替代外部素材地址：原用例依赖 w3.org 的样例文件，
            // 该地址已返回 404，且单元测试不应依赖公网可用性
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            var serverTask = Task.Run(async () =>
            {
                using var client = await listener.AcceptTcpClientAsync();
                using var netStream = client.GetStream();
                // 先把请求报文读干净再回写：带着未读数据关闭套接字会触发 RST，
                // 客户端会报“已建立的连接被中止”，表现为偶发失败
                var requestBuffer = new byte[1024];
                var received = new StringBuilder();
                while (!received.ToString().Contains("\r\n\r\n"))
                {
                    var read = await netStream.ReadAsync(requestBuffer);
                    if (read <= 0) break;
                    received.Append(Encoding.ASCII.GetString(requestBuffer, 0, read));
                }

                var body = Encoding.UTF8.GetBytes("hello stream");
                var header = Encoding.ASCII.GetBytes(
                    $"HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\nContent-Length: {body.Length}\r\nConnection: close\r\n\r\n");
                await netStream.WriteAsync(header);
                await netStream.WriteAsync(body);
                await netStream.FlushAsync();
            });

            try
            {
                var stream = await RemoteDocumentLoader.LoadDocumentStreamFromUrlAsync($"http://127.0.0.1:{port}/iso_8859-1.txt");

                Assert.NotNull(stream);
                Assert.True(stream.CanRead);
                stream.Dispose();
            }
            finally
            {
                listener.Stop();
                try
                {
                    await serverTask;
                }
                catch
                {
                    // 客户端按需只读了一小段就释放连接，服务端写响应时可能命中断连异常；
                    // 判定以客户端结果为准，这里不能让清理阶段覆盖真实断言
                }
            }
        }

        [Fact]
        public async Task LoadDocumentStreamFromUrlAsync_WithNullUrl_ThrowsException()
        {
            // null / 空串属于参数错误：下载器在发请求前抛 ArgumentException（不是 UriFormatException，
            // 后者只在字符串格式非法时才由 Uri 抛出）
            await Assert.ThrowsAsync<ArgumentException>(() =>
                RemoteDocumentLoader.LoadDocumentStreamFromUrlAsync(null));
        }

        [Fact]
        public async Task LoadDocumentStreamFromUrlAsync_WithEmptyUrl_ThrowsException()
        {
            await Assert.ThrowsAsync<ArgumentException>(() =>
                RemoteDocumentLoader.LoadDocumentStreamFromUrlAsync(""));
        }
    }
}