using COSXML;
using COSXML.Auth;
using COSXML.Model.Object;
using COSXML.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Common.TencentCloud
{
    public class CosHelper
    {
        string endpoint = "";
        string appId = "";
        string secretId = "";
        string secretKey = "";
        string bucketName = "";

        public CosHelper()
        {

        }


        public CosHelper(string in_endpoint, string in_appId, string in_secretId, string in_secretKey, string in_bucketName)
        {
            endpoint = in_endpoint;
            appId = in_appId;
            secretId = in_secretId;
            secretKey = in_secretKey;
            bucketName = in_bucketName;
        }


        /// <summary>
        /// 上传本地文件到 COS
        /// </summary>
        /// <param name="localpath">本地文件路径</param>
        /// <param name="remotepath">远端文件路径 以 / 分割多级文件夹</param>
        public bool FileUpload(string localpath, string remotepath)
        {
            try
            {

                CosXmlConfig config = new CosXmlConfig.Builder()
                    .SetConnectionTimeoutMs(60000)  //设置连接超时时间，单位毫秒，默认45000ms
                    .SetReadWriteTimeoutMs(40000)  //设置读写超时时间，单位毫秒，默认45000ms
                    .IsHttps(true)  //设置默认 HTTPS 请求
                    .SetAppid(appId) //设置腾讯云账户的账户标识 APPID
                    .SetRegion(endpoint) //设置一个默认的存储桶地域
                .Build();


                long durationSecond = 600;          //每次请求签名有效时长，单位为秒

                QCloudCredentialProvider qCloudCredentialProvider = new DefaultQCloudCredentialProvider(secretId, secretKey, durationSecond);

                CosXml cosXml = new CosXmlServer(config, qCloudCredentialProvider);


                try
                {
                    string bucket = bucketName; //存储桶


                    PutObjectRequest request = new PutObjectRequest(bucket, remotepath, localpath);

                    //设置签名有效时长
                    request.SetSign(TimeUtils.GetCurrentTime(TimeUnit.Seconds), durationSecond);


                    //设置进度回调
                    request.SetCosProgressCallback(delegate (long completed, long total)
                    {
                        Console.WriteLine(String.Format("progress = {0:##.##}%", completed * 100.0 / total));
                    });


                    //执行请求
                    PutObjectResult result = cosXml.PutObject(request);


                    //对象的 eTag
                    string eTag = result.eTag;


                }
                catch (COSXML.CosException.CosClientException clientEx)
                {
                    //请求失败
                    Console.WriteLine("CosClientException: " + clientEx);

                    return false;
                }
                catch (COSXML.CosException.CosServerException serverEx)
                {
                    //请求失败
                    Console.WriteLine("CosServerException: " + serverEx.GetInfo());

                    return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }


    }
}
