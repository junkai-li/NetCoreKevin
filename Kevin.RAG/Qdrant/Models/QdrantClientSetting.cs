using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kevin.RAG.Qdrant.Models
{
    /// <summary>
    /// Qdrant矢量数据库客户端配置
    /// </summary>
    public class QdrantClientSetting
    {
        /// <summary>
        /// 地址
        /// </summary>
        public string Url { get; set; } = "http://localhost:6334";
        /// <summary>
        /// API 密钥认证
        /// </summary>
        public string ApiKey { get; set; } = string.Empty;
        public string CertificateThumbprint { get; set; } = string.Empty;
    }
}
