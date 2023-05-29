using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.FileStorage.AliCloud.Models
{
    public class FileStorageSetting
    {
        /// <summary>
        /// 对象存储区域节点
        /// </summary>
        public string Endpoint { get; set; }


        /// <summary>
        /// 账户ID
        /// </summary>
        public string AccessKeyId { get; set; }


        /// <summary>
        /// 账户私钥
        /// </summary>
        public string AccessKeySecret { get; set; }


        /// <summary>
        /// 存储桶名称
        /// </summary>
        public string BucketName { get; set; }
    }
}
