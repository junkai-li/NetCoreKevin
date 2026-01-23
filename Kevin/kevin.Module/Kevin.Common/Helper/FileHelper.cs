using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
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
    }
}
