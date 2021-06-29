using Aliyun.OSS;
using Aliyun.OSS.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Common.AliYun
{
    public class OssHelper
    {
        string endpoint = IO.Config.Get()["OSSEndpoint"];
        string accessKeyId = "";
        string accessKeySecret = "";
        string bucketName = "";

        public OssHelper()
        {

        }




        public OssHelper(string in_endpoint, string in_accessKeyId, string in_accessKeySecret, string in_bucketName)
        {
            endpoint = in_endpoint;
            accessKeyId = in_accessKeyId;
            accessKeySecret = in_accessKeySecret;
            bucketName = in_bucketName;
        }




        /// <summary>
        /// 上传本地文件到 OSS
        /// </summary>
        /// <param name="localPath">本地文件路径</param>
        /// <param name="remotePath">远端文件路径 以 / 分割多级文件夹，不传默认为更目录</param>
        /// <param name="mode">访问方式,["attachment","inline"]，默认为 attachment</param>
        public bool FileUpload(string localPath, string remotePath, string fileName = null, string mode = "attachment")
        {
            try
            {
                var objectName = Path.GetFileName(localPath);

                if (remotePath != null)
                {
                    objectName = remotePath + "/" + objectName;
                }

                var localFilename = localPath;


                // 创建OssClient实例。
                var client = new OssClient(endpoint, accessKeyId, accessKeySecret);

                if (fileName != null)
                {
                    var metaData = new ObjectMetadata()
                    {
                        ContentDisposition = string.Format(mode + ";filename*=utf-8''{0}", HttpUtils.EncodeUri(fileName, "utf-8"))
                    };

                    // 上传文件。
                    client.PutObject(bucketName, objectName, localFilename, metaData);
                }
                else
                {
                    // 上传文件。
                    client.PutObject(bucketName, objectName, localFilename);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }



        /// <summary>
        /// 下载OSS的文件
        /// </summary>
        /// <param name="remotePath"></param>
        /// <param name="localPath"></param>
        /// <returns></returns>
        public bool FileDownload(string remotePath, string localPath)
        {
            var objectName = remotePath;

            var downloadFilename = localPath;
            // 创建OssClient实例。
            var client = new OssClient(endpoint, accessKeyId, accessKeySecret);
            try
            {
                // 下载文件到流。OssObject 包含了文件的各种信息，如文件所在的存储空间、文件名、元信息以及一个输入流。
                var obj = client.GetObject(bucketName, objectName);
                using (var requestStream = obj.Content)
                {
                    byte[] buf = new byte[1024];
                    var fs = File.Open(downloadFilename, FileMode.OpenOrCreate);
                    var len = 0;
                    // 通过输入流将文件的内容读取到文件或者内存中。
                    while ((len = requestStream.Read(buf, 0, 1024)) != 0)
                    {
                        fs.Write(buf, 0, len);
                    }
                    fs.Close();
                }
                Console.WriteLine("Get object succeeded");

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Get object failed. {0}", ex.Message);
                return false;
            }
        }




        /// <summary>
        /// 单个文件删除方法
        /// </summary>
        /// <param name="remotePath">远程文件地址</param>
        /// <returns></returns>
        public bool FileDelete(string remotePath)
        {

            try
            {

                var client = new OssClient(endpoint, accessKeyId, accessKeySecret);

                client.DeleteObject(bucketName, remotePath);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Delete object failed. {0}", ex.Message);
                return false;
            }
        }




        /// <summary>
        /// 文件批量删除方法
        /// </summary>
        /// <param name="remotePathList">远程文件地址集合</param>
        /// <returns></returns>
        public bool FileBatchDelete(List<string> remotePathList)
        {

            try
            {
                var client = new OssClient(endpoint, accessKeyId, accessKeySecret);

                var request = new DeleteObjectsRequest(bucketName, remotePathList, true);

                client.DeleteObjects(request);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Delete objects failed. {0}", ex.Message);
                return false;
            }
        }




        /// <summary>
        /// 创建存储空间（Bucket）
        /// </summary>
        /// <param name="bucketName"></param>
        public bool BucketCreate(string bucketName)
        {
            try
            {
                // 初始化OssClient。
                var client = new OssClient(endpoint, accessKeyId, accessKeySecret);

                var exist = client.DoesBucketExist(bucketName);

                if (exist == false)
                {
                    var request = new CreateBucketRequest(bucketName);

                    //设置存储空间访问权限ACL。
                    request.ACL = CannedAccessControlList.PublicRead;

                    //设置数据容灾类型。
                    request.DataRedundancyType = DataRedundancyType.LRS;

                    // 创建存储空间。
                    client.CreateBucket(request);
                }

                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine("Create bucket failed. {0}", ex.Message);

                return false;
            }
        }




        /// <summary>
        /// 删除存储空间（Bucket）
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="isCoerce">是否强制删除</param>
        public bool BucketDelete(string bucketName, bool isCoerce = false)
        {
            try
            {

                // 初始化OssClient。
                var client = new OssClient(endpoint, accessKeyId, accessKeySecret);

                if (isCoerce)
                {

                    while (true)
                    {
                        var listObjectsRequest = new ListObjectsRequest(bucketName);

                        var result = client.ListObjects(listObjectsRequest);

                        var keys = result.ObjectSummaries.ToList().Select(t => t.Key).ToList();

                        if (keys.Count == 0)
                        {
                            break;
                        }

                        var request = new DeleteObjectsRequest(bucketName, keys, true);
                        client.DeleteObjects(request);
                    }

                }



                var exist = client.DoesBucketExist(bucketName);

                if (exist == true)
                {
                    client.DeleteBucket(bucketName);
                }

                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine("Delete bucket failed. {0}", ex.Message);

                return false;
            }
        }




        /// <summary>
        /// 获取文件临时访问URL
        /// </summary>
        /// <param name="remotePath">文件地址</param>
        /// <param name="fileName">文件名称，不传则默认为文件物理名称</param>
        /// <param name="ExpirationTime">过期时间,默认为7天</param>
        /// <param name="mode">访问方式,["attachment","inline"]，默认为 attachment</param>
        /// <returns></returns>
        public string GetTempUrl(string remotePath, string fileName = null, DateTime ExpirationTime = default, string mode = "attachment")
        {

            var client = new OssClient(endpoint, accessKeyId, accessKeySecret);

            // 生成签名URL。
            var req = new GeneratePresignedUriRequest(bucketName, remotePath);

            if (fileName != null)
            {
                req.ResponseHeaders.ContentDisposition = string.Format(mode + ";filename*=utf-8''{0}", HttpUtils.EncodeUri(fileName, "utf-8"));
            }

            if (ExpirationTime == default)
            {
                ExpirationTime = DateTime.Now.AddDays(7);
            }

            req.Expiration = ExpirationTime;

            var url = client.GeneratePresignedUri(req);

            return url.ToString();
        }




        /// <summary>
        /// 更新文件元信息
        /// </summary>
        /// <param name="remotePath">远程文件地址</param>
        /// <param name="objectMetadata">文件元信息</param>
        public bool ModifyObjectMeta(string remotePath, ObjectMetadata objectMetadata)
        {
            // 创建OssClient实例。
            var client = new OssClient(endpoint, accessKeyId, accessKeySecret);
            try
            {
                // 通过ModifyObjectMeta方法修改文件元信息。
                client.ModifyObjectMeta(bucketName, remotePath, objectMetadata);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Put object failed, {0}", ex.Message);

                return false;
            }
        }



    }
}
