using kevin.Domain.Share.Dtos.Bases;
using kevin.Domain.Share.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace kevin.Domain.Share.Dtos.AI
{
    public class AIKmssDto : CUD_User_Dto
    {
        /// <summary>
        /// 名称
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// 每个段落的最大标记数。
        /// </summary>
        [DefaultValue(299)]
        public int MaxTokensPerParagraph { get; set; } = 299;
        /// <summary>
        /// 每行，也就是每句话的最大标记数
        /// </summary>
        [DefaultValue(99)]
        public int MaxTokensPerLine { get; set; } = 99;

        /// <summary>
        /// 段落之间重叠标记的数量。
        /// </summary>
        [DefaultValue(49)]
        public int OverlappingTokens { get; set; } = 49;

        /// <summary>
        /// 矢量化模型Id
        /// </summary> 
        [Description("矢量化模型Id")]
        public long? aIModelsId { get; set; }

        public int DocumentCount { get => this.AIKmssDetailsList.Count; }
        /// <summary>
        ///  case 0: return 'orange'; // 待处理 case 1: return 'blue'; // 处理中  case 2: return 'green'; // 已完成 case 3: return 'red'; // 失败
        /// </summary>
        public int Status { get => GetStatus(); }

        public int GetStatus()
        {
            if (AIKmssDetailsList.FirstOrDefault(t => t.Status == ImportKmsStatus.Fail) != default)
            {
                return 3;
            }
            if (AIKmssDetailsList.FirstOrDefault(t => t.Status == ImportKmsStatus.Loadding) != default)
            {
                return 1;
            }
            return 2;
        }
        /// <summary>
        /// 详情列表
        /// </summary>
        public List<AIKmsDetailsDto> AIKmssDetailsList { get; set; } = new List<AIKmsDetailsDto>();

        /// <summary>
        /// Rerank重排模型Id
        /// </summary>
        [Description("Rerank重排模型Id")]
        public long? aIRerankModelsId { get; set; }
    }

    /// <summary>
    /// 知识库文件碎片分页预览结果
    /// </summary>
    public class KmsChunksDto
    {
        /// <summary>
        /// 该文件碎片总数
        /// </summary>
        public long Total { get; set; }

        /// <summary>
        /// 当前页碎片列表（按 ChunkIndex 升序）
        /// </summary>
        public List<KmsChunkItemDto> List { get; set; } = new List<KmsChunkItemDto>();
    }

    /// <summary>
    /// 知识库文件碎片项
    /// </summary>
    public class KmsChunkItemDto
    {
        /// <summary>
        /// 碎片点 Id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 碎片内容
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// 块在原文档中的序号
        /// </summary>
        public int ChunkIndex { get; set; }

        /// <summary>
        /// 所属章节或类别（payload Category，写入时为知识库名称）
        /// </summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// 入库时间
        /// </summary>
        public DateTimeOffset CreatedAt { get; set; }
    }
}
