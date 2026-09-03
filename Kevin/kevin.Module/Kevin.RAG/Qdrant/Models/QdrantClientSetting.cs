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

        /// <summary>
        /// 智能体记忆向量维度（需与 Embedding 模型输出维度一致，默认 2048 对应智谱 embedding-3）
        /// </summary>
        public int MemoryEmbeddingSize { get; set; } = 2048;
    }
}
