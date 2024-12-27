using Aliyun.OSS;
using Aliyun.OSS.Util;
using kevin.FileStorage.AliCloud.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.FileStorage.AliCloud
{
    public class AliCloudStorage: IFileStorage
    {
        private readonly string endpoint;
        private readonly string accessKeyId;
        private readonly string accessKeySecret;
        private readonly string bucketName;
        public AliCloudStorage(IOptionsMonitor<FileStorageSetting> config)
        {
            endpoint = config.CurrentValue.Endpoint;
            accessKeyId = config.CurrentValue.AccessKeyId;
            accessKeySecret = config.CurrentValue.AccessKeySecret;
            bucketName = config.CurrentValue.BucketName;
        }

        public bool FileDelete(string remotePath)
        {
            try
            {
                remotePath = remotePath.Replace("\\", "/");

                OssClient client = new(endpoint, accessKeyId, accessKeySecret);

                client.DeleteObject(bucketName, remotePath);

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

                OssClient client = new(endpoint, accessKeyId, accessKeySecret);

                // 下载文件到流。OssObject 包含了文件的各种信息，如文件所在的存储空间、文件名、元信息以及一个输入流。
                var obj = client.GetObject(bucketName, remotePath);
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
                OssClient client = new(endpoint, accessKeyId, accessKeySecret);

                if (fileName != null)
                {
                    ObjectMetadata metaData = new()
                    {
                        ContentDisposition = string.Format("attachment;filename*=utf-8''{0}", HttpUtils.EncodeUri(fileName, "utf-8"))
                    };

                    client.PutObject(bucketName, objectName, localPath, metaData);
                }
                else
                {
                    client.PutObject(bucketName, objectName, localPath);
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

                OssClient client = new(endpoint, accessKeyId, accessKeySecret);

                GeneratePresignedUriRequest req = new(bucketName, remotePath);

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
    }
}
