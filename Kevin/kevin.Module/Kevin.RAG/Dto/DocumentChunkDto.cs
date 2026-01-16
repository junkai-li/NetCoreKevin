using DocumentFormat.OpenXml.EMMA;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Kevin.RAG.Dto
{
    /// <summary>
    /// 文档块模型，用于存储在向量数据库中
    /// </summary> 
    public class DocumentChunkDto
    {
        /// <summary>
        /// 文档块的唯一标识符
        /// </summary>
        [VectorStoreKey]
        public long Id { get; set; }

        /// <summary>
        /// 文档的文本内容
        /// </summary>
        [VectorStoreData]
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// 文档的向量表示（embedding）
        /// </summary>
        [VectorStoreVector(1536)]
        public Embedding<float> ContentVector { get; set; } = new Embedding<float>(new float[1536]);

        /// <summary>
        /// 来源文件名
        /// </summary>
        [VectorStoreData]
        public string SourceFile { get; set; } = string.Empty;

        /// <summary>
        /// 文档标题
        /// </summary>
        [VectorStoreData]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 所属章节或类别
        /// </summary>
        [VectorStoreData]
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// 块在原文档中的序号
        /// </summary>
        [VectorStoreData]
        public int ChunkIndex { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [VectorStoreData]
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        /// <summary>
        /// 相似度
        /// </summary>
        public double? Score { get; set; }
    }
}
