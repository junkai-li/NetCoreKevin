using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.FileStorage.KevinStaticFiles.Models
{
    public class FileStorageSetting
    {
        /// <summary>
        /// 对象存储区域节点
        /// </summary>
        public required string Endpoint { get; set; }

        /// <summary>
        /// 访问前缀
        /// </summary>
        public required string Url { get; set; } = "http://localhost:9901/KevinFlies";
    }
}
