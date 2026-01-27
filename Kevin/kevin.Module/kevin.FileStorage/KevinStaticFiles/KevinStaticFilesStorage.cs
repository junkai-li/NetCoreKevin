using Kevin.Common.App;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace kevin.FileStorage.KevinStaticFiles
{
    public class KevinStaticFilesStorage : IFileStorage
    {
        private readonly Models.FileStorageSetting fileStorageSetting;


        // <summary>
        /// 提取URL中第一个'/'之前的部分（协议+主机+端口）
        /// </summary>
        /// <param name="url">完整URL字符串</param>
        /// <returns>协议+主机+端口部分</returns>
        private static string ExtractProtocolAndHost(string url)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentException("URL不能为空");

            try
            {
                // 方法1: 使用Uri类解析（推荐）
                Uri uri = new Uri(url);
                string protocol = uri.Scheme;
                string host = uri.Host;
                int port = uri.Port;

                // 构造结果字符串
                if (port == 80 && protocol == "http" || port == 443 && protocol == "https")
                {
                    // 默认端口不显示
                    return $"{protocol}://{host}";
                }
                else
                {
                    // 显示指定端口
                    return $"{protocol}://{host}:{port}";
                }
            }
            catch (UriFormatException)
            {
                // 方法2: 使用正则表达式作为备选方案
                string pattern = @"^(https?://[^/]+)";
                Match match = Regex.Match(url, pattern);
                if (match.Success)
                {
                    return match.Groups[1].Value;
                }
                else
                {
                    throw new ArgumentException("无效的URL格式");
                }
            }
        }
        public KevinStaticFilesStorage(IOptionsMonitor<Models.FileStorageSetting> config, IHttpContextAccessor _httpContextAccessor)
        {
            fileStorageSetting = config.CurrentValue;
            if (string.IsNullOrEmpty(fileStorageSetting.Url))
            {
                fileStorageSetting.Url = $"{ExtractProtocolAndHost(_httpContextAccessor.GetUrl())}/" + fileStorageSetting.RequestPath;
            }

        }
        public bool FileDelete(string remotePath)
        {
            remotePath = remotePath.Replace("\\", "/");
            if (File.Exists(remotePath))
            {
                File.Delete(remotePath);
            }
            return true;
        }

        public bool FileDownload(string remotePath, string localPath)
        {
            remotePath = fileStorageSetting.Endpoint + remotePath.Replace(fileStorageSetting.Url, "").Replace("\\", "/");
            if (File.Exists(remotePath))
            {
                string localDir = Path.GetDirectoryName(localPath) ?? "";
                if (!Directory.Exists(localDir))
                {
                    Directory.CreateDirectory(localDir);
                }
                // 2. 使用 FileStream 进行流式复制，显式指定 FileShare.ReadWrite
                // 这样可以允许其他进程在读取时有一定的共享权限，或者至少确保当前操作正确释放句柄
                using (FileStream sourceStream = new FileStream(remotePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (FileStream destStream = new FileStream(localPath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        sourceStream.CopyTo(destStream);
                    }
                }
            }
            return true;
        }

        public bool FileUpload(string localPath, string remotePath, string? fileName = null)
        {
            remotePath = fileStorageSetting.Endpoint + remotePath.Replace("\\", "/");
            if (fileName != null)
            {
                remotePath = remotePath + "/" + fileName;
            }
            if (File.Exists(localPath))
            {
                string remoteDir = remotePath[..(remotePath.LastIndexOf("/") + 1)];
                Directory.CreateDirectory(remoteDir);
                File.Copy(localPath, remotePath, true);
            }
            return true;
        }

        public string? GetFileTempUrl(string remotePath, TimeSpan expiry, string? fileName = null)
        {
            return fileStorageSetting.Url + remotePath.Replace("\\", "/");
        }

        public async Task<string> GetUrl()
        {
            return await Task.Run(() => fileStorageSetting.Url ?? "");
        }
    }
}
