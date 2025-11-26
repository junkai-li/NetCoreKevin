using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.FileStorage.TencentCloud.Models
{
    public class FileStorageSetting
    {
        /// <summary>
        /// 腾讯云账户 appid
        /// </summary>
        public required string AppId { get; set; }


        /// <summary>
        /// COS存储区域
        /// </summary>
        public required string Region { get; set; }


        /// <summary>
        /// 私钥id
        /// </summary>
        public required string SecretId { get; set; }


        /// <summary>
        /// 私钥值
        /// </summary>
        public required string SecretKey { get; set; }



        /// <summary>
        /// 存储桶名称
        /// </summary>
        public required string BucketName { get; set; }

        /// <summary>
        /// 访问前缀
        /// </summary>
        public required string Url { get; set; }
    }
}
