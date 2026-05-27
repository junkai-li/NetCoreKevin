using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.FileStorage.QiniuCloud.Models
{
    /// <summary>
    /// 七牛云存储配置
    /// </summary>
    public class FileStorageSetting
    {
        /// <summary>
        /// AccessKey
        /// </summary>
        public required string AccessKey { get; set; }

        /// <summary>
        /// SecretKey
        /// </summary>
        public required string SecretKey { get; set; }

        /// <summary>
        /// Domain
        /// </summary>
        public required string Domain { get; set; }

        /// <summary>
        /// Bucket
        /// </summary>
        public required string Bucket { get; set; }

        public string? CallbackUrl { get; set; }
    }
}
