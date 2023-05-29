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
        public string AppId { get; set; }


        /// <summary>
        /// COS存储区域
        /// </summary>
        public string Region { get; set; }


        /// <summary>
        /// 私钥id
        /// </summary>
        public string SecretId { get; set; }


        /// <summary>
        /// 私钥值
        /// </summary>
        public string SecretKey { get; set; }



        /// <summary>
        /// 存储桶名称
        /// </summary>
        public string BucketName { get; set; }
    }
}
