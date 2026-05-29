using kevin.FileStorage.QiniuCloud.Models;
using Microsoft.Extensions.Options;
using Qiniu.Http;
using Qiniu.Storage;
using Qiniu.Util;

namespace kevin.FileStorage.QiniuCloud
{
    /// <summary>
    /// 七牛云存储
    /// </summary>
    public class QiniuCloudStorage : IFileStorage
    {
        private readonly string _bucket;
        private readonly string _domain;
        private readonly Mac _mac;
        private readonly Qiniu.Storage.Config Qiniuconfig;
        public QiniuCloudStorage(IOptionsMonitor<FileStorageSetting> config, IHttpClientFactory? httpClientFactory = null)
        {
            _mac = new Mac(config.CurrentValue.AccessKey, config.CurrentValue.SecretKey);
            _bucket = config.CurrentValue.Bucket;
            _domain = config.CurrentValue.Domain;
            Qiniuconfig = new Qiniu.Storage.Config();
            Qiniuconfig.Zone = Zone.ZONE_CN_South;
            Qiniuconfig.UseHttps = true;
            Qiniuconfig.UseCdnDomains = true;
            Qiniuconfig.ChunkSize = ChunkUnit.U512K;
        }

        public (bool, string) FileUpload(string localPath, string remotePath, string? fileName = null)
        {
            remotePath = remotePath.Replace("\\", "/");

            if (!File.Exists(localPath))
                throw new FileNotFoundException($"本地文件不存在: {localPath}");

            var putPolicy = new PutPolicy { Scope = _bucket };
            putPolicy.SetExpires(3600);
            string token = Auth.CreateUploadToken(_mac, putPolicy.ToJsonString());

            var uploadManager = new UploadManager(Qiniuconfig);
            var result = uploadManager.UploadFile(localPath, remotePath + fileName, token, new PutExtra());
            var url = CreatePublishUrl(remotePath + fileName);
            return (result.Code == (int)HttpCode.OK, url);
        }

        public bool FileDownload(string remotePath, string localPath)
        {
            remotePath = remotePath.Replace("\\", "/");
            var dir = Path.GetDirectoryName(localPath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            var result = DownloadManager.Download(remotePath, localPath);

            return result.Code == (int)HttpCode.OK;
        }
        public string CreatePublishUrl(string key)
        {
            string publicUrl = DownloadManager.CreatePublishUrl(_domain, key);
            return publicUrl;
            //Console.WriteLine(publicUrl);
        }
        public bool FileDelete(string remotePath)
        {
            remotePath = remotePath.Replace("\\", "/");
            var bucketManager = new BucketManager(_mac, Qiniuconfig);
            var result = bucketManager.Delete(_bucket, remotePath);
            return result.Code == (int)HttpCode.OK;
        }

        public string? GetFileTempUrl(string remotePath, TimeSpan expiry, string? fileName = null)
        {
            remotePath = remotePath.Replace("\\", "/");

            var signedUrl = DownloadManager.CreatePrivateUrl(
                _mac, _domain.TrimEnd('/'), remotePath, (int)expiry.TotalSeconds);

            if (!string.IsNullOrWhiteSpace(fileName))
            {
                var uriBuilder = new UriBuilder(signedUrl);
                var query = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);
                query["attname"] = fileName;
                uriBuilder.Query = query.ToString();
                signedUrl = uriBuilder.ToString();
            }
            return signedUrl;
        }

        public Task<string> GetUrl() => Task.FromResult(_domain);
    }
}
