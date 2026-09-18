using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
namespace NetCore.Util
{
    /// <summary>
    /// 文件操作帮助类
    /// </summary>
    public class FileHelper
    {
        // 建议将 HttpClient 声明为静态，以避免端口耗尽问题
        private static readonly HttpClient _httpClient = new HttpClient();
        /// <summary>
        /// 下载远程文件并返回流
        /// </summary>
        /// <param name="url">远程文件 URL</param>
        /// <returns>包含文件内容的流</returns>
        public static async Task<Stream> GetRemoteFileStreamAsync(string url)
        {
            try
            {
                // 1. 发送 GET 请求
                // HttpCompletionOption.ResponseHeadersRead 表示收到响应头后就返回，
                // 此时我们可以立即获取流，而不必等待整个内容下载到内存。
                using (HttpResponseMessage response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
                {
                    // 2. 检查响应是否成功
                    response.EnsureSuccessStatusCode();

                    // 3. 读取流
                    // 注意：这里返回的是网络流，它依赖于底层的 HTTP 连接。 
                    byte[] data = await response.Content.ReadAsByteArrayAsync();
                    return new MemoryStream(data);
                }
            }
            catch (Exception ex)
            {
                // 处理异常（记录日志等）
                throw new Exception($"下载文件失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 下载远程文件并返回带 MIME 与文件名的结果。
        /// <para>
        /// 与 <see cref="GetRemoteFileStreamAsync"/> 的区别：这里显式携带 HTTP Content-Type，
        /// 供 <c>Microsoft.Extensions.AI.DataContent.LoadFromAsync(stream, mimeType)</c> 直接使用，
        /// 避免 MemoryStream 没有 Name 导致 MIME 推断失败（图片/音频送入模型时会被判为未知类型）。
        /// </para>
        /// <para>
        /// 兜底策略：Content-Type 为空或 <c>application/octet-stream</c> 时按 URL 扩展名推断；
        /// 都失败时回落到 <c>application/octet-stream</c>，由调用方决定是否拒收。
        /// </para>
        /// </summary>
        /// <param name="url">远程文件 URL</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>包含流、MIME、文件名、字节数的结果对象</returns>
        public static async Task<RemoteFileResult> GetRemoteFileAsync(string url, CancellationToken cancellationToken = default)
        {
            try
            {
                using var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
                response.EnsureSuccessStatusCode();
                byte[] data = await response.Content.ReadAsByteArrayAsync(cancellationToken);
                var mime = response.Content.Headers.ContentType?.MediaType;
                if (string.IsNullOrWhiteSpace(mime) || string.Equals(mime, "application/octet-stream", StringComparison.OrdinalIgnoreCase))
                {
                    mime = GuessMimeByExtension(url);
                }
                string fileName;
                try
                {
                    fileName = Path.GetFileName(new Uri(url).LocalPath);
                }
                catch
                {
                    fileName = string.Empty;
                }
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    fileName = "file";
                }
                return new RemoteFileResult(new MemoryStream(data), mime ?? "application/octet-stream", fileName, data.LongLength);
            }
            catch (Exception ex)
            {
                throw new Exception($"下载文件失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 按 URL/文件名扩展名推断 MIME 类型。
        /// <para>
        /// 用于 OSS/MinIO 返回 <c>application/octet-stream</c> 时的兜底，覆盖图片/音频/常见文档三大类；
        /// 未识别的扩展名统一回落到 <c>application/octet-stream</c>，由上层决定是否拒收。
        /// </para>
        /// </summary>
        /// <param name="urlOrFileName">URL 或文件名</param>
        /// <returns>MIME 字符串，始终非空</returns>
        public static string GuessMimeByExtension(string urlOrFileName)
        {
            if (string.IsNullOrWhiteSpace(urlOrFileName))
                return "application/octet-stream";
            string extension;
            try
            {
                // URL 可能带查询串，先用 Uri 取 LocalPath 再拿扩展名；失败则退回字符串截取
                if (Uri.TryCreate(urlOrFileName, UriKind.Absolute, out var uri))
                {
                    extension = Path.GetExtension(uri.LocalPath).ToLowerInvariant();
                }
                else
                {
                    extension = Path.GetExtension(urlOrFileName).ToLowerInvariant();
                }
            }
            catch
            {
                extension = Path.GetExtension(urlOrFileName).ToLowerInvariant();
            }
            return extension switch
            {
                // 图片
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".webp" => "image/webp",
                ".tiff" or ".tif" => "image/tiff",
                ".svg" => "image/svg+xml",
                ".ico" => "image/x-icon",
                // 音频
                ".mp3" => "audio/mpeg",
                ".wav" => "audio/wav",
                ".m4a" => "audio/mp4",
                ".ogg" or ".oga" => "audio/ogg",
                ".flac" => "audio/flac",
                ".aac" => "audio/aac",
                ".opus" => "audio/opus",
                ".amr" => "audio/amr",
                // 视频（预留，本次未实现视频模态但 MIME 表先备齐）
                ".mp4" => "video/mp4",
                ".mov" => "video/quicktime",
                ".webm" => "video/webm",
                ".avi" => "video/x-msvideo",
                ".mkv" => "video/x-matroska",
                // 文档
                ".pdf" => "application/pdf",
                ".txt" or ".log" => "text/plain",
                ".md" or ".markdown" => "text/markdown",
                ".html" or ".htm" => "text/html",
                ".csv" => "text/csv",
                ".json" => "application/json",
                ".xml" => "application/xml",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".xls" => "application/vnd.ms-excel",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".doc" => "application/msword",
                _ => "application/octet-stream"
            };
        }

        public async static Task<string> GetRealFileNameFromUrlAsync(string url)
        {
            using (var client = new HttpClient())
            {
                using (var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
                {
                    // 尝试从 Content-Disposition 头获取文件名
                    ContentDispositionHeaderValue contentDisposition;
                    if (ContentDispositionHeaderValue.TryParse(response.Content.Headers.ContentDisposition?.ToString(), out contentDisposition))
                    {
                        if (!string.IsNullOrWhiteSpace(contentDisposition.FileName))
                        {
                            // 去除文件名两端的引号（如果存在）
                            return contentDisposition.FileName.Trim('"');
                        }
                    }

                    // 如果头部没有，回退到从 URL 提取
                    return Path.GetFileName(new Uri(url).LocalPath);
                }
            }
        }
        #region 读操作

        /// <summary>
        /// 判断文件是否存在
        /// </summary>
        /// <param name="path">文件目录</param>
        /// <returns></returns>
        public static bool Exists(string path)
        {
            return File.Exists(path);
        }

        /// <summary>
        /// 获取当前程序根目录
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentDir()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        #endregion

        #region 写操作

        /// <summary>
        /// 输出字符串到文件
        /// 注：使用系统默认编码;若文件不存在则创建新的,若存在则覆盖
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="path">文件路径</param>
        public static void WriteTxt(string content, string path)
        {
            WriteTxt(content, path, null, null);
        }

        /// <summary>
        /// 输出字符串到文件
        /// 注：使用自定义编码;若文件不存在则创建新的,若存在则覆盖
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="path">文件路径</param>
        /// <param name="encoding">编码</param>
        public static void WriteTxt(string content, string path, Encoding encoding)
        {
            WriteTxt(content, path, encoding, null);
        }

        /// <summary>
        /// 输出字符串到文件
        /// 注：使用自定义模式,使用默认编码
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="path">文件路径</param>
        /// <param name="fileModel">输出方法</param>
        public static void WriteTxt(string content, string path, FileMode fileModel)
        {
            WriteTxt(content, path, null, fileModel);
        }

        /// <summary>
        /// 输出字符串到文件
        /// 注：使用自定义编码以及写入模式
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="path">文件路径</param>
        /// <param name="encoding">字符编码</param>
        /// <param name="fileModel">写入模式</param>
        public static void WriteTxt(string content, string path, Encoding encoding, FileMode fileModel)
        {
            WriteTxt(content, path, encoding, (FileMode?)fileModel);
        }

        /// <summary>
        /// 输出字符串到文件
        /// 注：使用自定义编码以及写入模式
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="path">文件路径</param>
        /// <param name="encoding">字符编码</param>
        /// <param name="fileModel">写入模式</param>
        private static void WriteTxt(string content, string path, Encoding encoding, FileMode? fileModel)
        {
            CheckDirectory(path);

            if (encoding == null)
                encoding = Encoding.Default;
            if (fileModel == null)
                fileModel = FileMode.Create;

            using (FileStream fileStream = new FileStream(path, fileModel.Value))
            {
                using (StreamWriter streamWriter = new StreamWriter(fileStream, encoding))
                {
                    streamWriter.Write(content);
                    streamWriter.Flush();
                }
            }
        }

        /// <summary>
        /// 检验目录，若目录已存在则不变
        /// </summary>
        /// <param name="path">目录位置</param>
        public static void CheckDirectory(string path)
        {
            if (path.Contains("\\"))
                Directory.CreateDirectory(GetPathDirectory(path));
        }

        /// <summary>
        /// 输出日志到指定文件
        /// </summary>
        /// <param name="msg">日志消息</param>
        /// <param name="path">日志文件位置（默认为D:\测试\a.log）</param>
        public static void WriteLog(string msg, string path = @"Log.txt")
        {
            string content = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}:{msg}\r\n";

            WriteTxt(content, $"{GetCurrentDir()}{path}", Encoding.UTF8, FileMode.Append);
        }

        /// <summary>
        /// 获取文件位置中的目录位置（不包括文件名）
        /// </summary>
        /// <param name="path">文件位置</param>
        /// <returns></returns>
        public static string GetPathDirectory(string path)
        {
            if (!path.Contains("\\"))
                return GetCurrentDir();

            string pathDirectory = string.Empty;
            string pattern = @"^(.*\\).*?$";
            Match match = Regex.Match(path, pattern);

            return match.Groups[1].ToString();
        }
        /// <summary>
        /// 七牛云上传文件
        /// </summary>

        public static async Task<(string key, string url)> UploadFile(Stream stream, string prefix, string fileName, string host)
        {
            string extension = System.IO.Path.GetExtension(fileName);
            var newFileName = $"{Guid.NewGuid()}{extension}";
            string key = $"{prefix}-{newFileName}";
            var filePath = $@"{Directory.GetCurrentDirectory()}\wwwroot\img\{key}";
            var url = @$"{host}/img/{key}";
            using (var fileStream = File.Create(filePath))
            {
                await stream.CopyToAsync(fileStream);
            }
            return (key, url);
        }
        #endregion

        #region 删除操作
        public static void DelectDir(string srcPath)
        {
            DirectoryInfo dir = new DirectoryInfo(srcPath);
            //FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
            //foreach (FileSystemInfo i in fileinfo)
            //{
            //    if (i is DirectoryInfo)            //判断是否文件夹
            //    {
            //        DirectoryInfo subdir = new DirectoryInfo(i.FullName);
            //        subdir.Delete(true);          //删除子目录和文件
            //    }
            //    else
            //    {
            //        File.Delete(i.FullName);      //删除指定文件
            //    }
            //}
            dir.Delete(true);
        }
        #endregion

        public static string DetermineFileType(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return "text";

            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".png" or ".jpg" or ".jpeg" or ".gif" or ".bmp" or ".webp" or ".tiff" or ".tif" or ".svg" or ".ico" => "image",
                // 音频：由 kevin.AI.AgentFramework/Modality 抽象层负责转成 DataContent(audio/*) 送入模型，
                // 不能落到默认的 "text" 分支（会用 TextStreamReader 把二进制字节当 UTF-8 读出乱码撑爆上下文）
                ".mp3" or ".wav" or ".m4a" or ".ogg" or ".oga" or ".flac" or ".aac" or ".opus" or ".amr" => "audio",
                ".xlsx" or ".xls" => "excel",
                ".pdf" => "pdf",
                ".doc" or ".docx" => "word",
                ".html" or ".htm" => "html",
                ".md" or ".markdown" => "markdown",
                ".txt" or ".csv" or ".json" or ".xml" or ".log" => "text",
                _ => "text"
            };
        }
    }
}
