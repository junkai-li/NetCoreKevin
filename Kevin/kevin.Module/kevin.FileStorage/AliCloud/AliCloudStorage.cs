using Aliyun.OSS;
using Aliyun.OSS.Util;
using Microsoft.Extensions.Options;

namespace kevin.FileStorage.AliCloud
{
    public partial class AliCloudStorage : IFileStorage
    {

        private readonly Models.FileStorageSetting fileStorageSetting;
        public AliCloudStorage(IOptionsMonitor<Models.FileStorageSetting> config)
        {
            fileStorageSetting = config.CurrentValue;
        }

        public bool FileDelete(string remotePath)
        {
            try
            {
                remotePath = remotePath.Replace("\\", "/");

                OssClient client = new(fileStorageSetting.Endpoint, fileStorageSetting.AccessKeyId, fileStorageSetting.AccessKeySecret);

                client.DeleteObject(fileStorageSetting.BucketName, remotePath);

                return true;
            }
            catch
            {
                return false;
            }
        }



        public bool FileDownload(string remotePath, string localPath)
        {
            try
            {
                remotePath = remotePath.Replace("\\", "/");

                OssClient client = new(fileStorageSetting.Endpoint, fileStorageSetting.AccessKeyId, fileStorageSetting.AccessKeySecret);

                // 下载文件到流。OssObject 包含了文件的各种信息，如文件所在的存储空间、文件名、元信息以及一个输入流。
                var obj = client.GetObject(fileStorageSetting.BucketName, remotePath);
                using var requestStream = obj.Content;
                byte[] buf = new byte[1024];
                var fs = File.Open(localPath, FileMode.OpenOrCreate);
                var len = 0;
                // 通过输入流将文件的内容读取到文件或者内存中。
                while ((len = requestStream.Read(buf, 0, 1024)) != 0)
                {
                    fs.Write(buf, 0, len);
                }
                fs.Close();

                return true;
            }
            catch
            {
                return false;
            }
        }



        public bool FileUpload(string localPath, string remotePath, string? fileName = null)
        {
            try
            {
                var objectName = Path.GetFileName(localPath);

                objectName = Path.Combine(remotePath, objectName).Replace("\\", "/");

                // 创建OssClient实例。
                OssClient client = new(fileStorageSetting.Endpoint, fileStorageSetting.AccessKeyId, fileStorageSetting.AccessKeySecret);

                if (fileName != null)
                {
                    ObjectMetadata metaData = new()
                    {
                        ContentDisposition = string.Format("attachment;filename*=utf-8''{0}", HttpUtils.EncodeUri(fileName, "utf-8"))
                    };

                    client.PutObject(fileStorageSetting.BucketName, objectName, localPath, metaData);
                }
                else
                {
                    client.PutObject(fileStorageSetting.BucketName, objectName, localPath);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }



        public string? GetFileTempUrl(string remotePath, TimeSpan expiry, string? fileName = null)
        {

            try
            {
                remotePath = remotePath.Replace("\\", "/");

                OssClient client = new(fileStorageSetting.Endpoint, fileStorageSetting.AccessKeyId, fileStorageSetting.AccessKeySecret);

                GeneratePresignedUriRequest req = new(fileStorageSetting.BucketName, remotePath);

                if (fileName != null)
                {
                    req.ResponseHeaders.ContentDisposition = string.Format("attachment;filename*=utf-8''{0}", HttpUtils.EncodeUri(fileName, "utf-8"));
                }

                req.Expiration = DateTime.UtcNow + expiry;

                var url = client.GeneratePresignedUri(req);

                return url.ToString();
            }
            catch
            {
                return null;
            }
        }

        public async Task<string> GetUrl()
        {
            return fileStorageSetting.Url;
        }
    }
}
