using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.FileStorage.KevinStaticFiles
{
    public class KevinStaticFilesStorage : IFileStorage
    {
        private readonly Models.FileStorageSetting fileStorageSetting;
        public KevinStaticFilesStorage(IOptionsMonitor<Models.FileStorageSetting> config)
        {
            fileStorageSetting = config.CurrentValue;
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
                string localDir = localPath[..(localPath.LastIndexOf("/") + 1)];
                Directory.CreateDirectory(localDir);
                File.Copy(remotePath, localPath, true);
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
            return fileStorageSetting.Url;
        }
    }
}
