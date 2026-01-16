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
        /// 对象路由前缀 例如KevinFlies
        /// </summary>
        public required string RequestPath { get; set; } = "KevinFlies";

        /// <summary>
        /// 访问前缀 为空则使用当前IP站点地址  
        /// </summary>
        public required string? Url { get; set; }
    }
}
