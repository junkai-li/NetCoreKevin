namespace kevin.Domain.Entities.AI
{
    /// <summary>
    /// TAIKmss 知识库文档设置
    /// </summary>
    [Table("TAIKmss")]
    [Description("TAIKmss")]
    public partial class TAIKmss : CUD_User
    {
        /// <summary>
        /// 名称
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// 每个段落的最大标记数。
        /// </summary>
        [DefaultValue(500)]
        public int MaxTokensPerParagraph { get; set; } = 500;
        /// <summary>
        /// 每行，也就是每句话的最大标记数
        /// </summary>
        [DefaultValue(120)]
        public int MaxTokensPerLine { get; set; } = 120;

        /// <summary>
        /// 段落之间重叠标记的数量。
        /// </summary>
        [DefaultValue(80)]
        public int OverlappingTokens { get; set; } = 80;

        /// <summary>
        /// 矢量化模型Id
        /// </summary> 
        [Description("矢量化模型Id")]
        public long? aIModelsId { get; set; }
        public TAIModels? aIModels { get; set; }

        /// <summary>
        /// Rerank重排模型Id
        /// </summary> 
        [Description("Rerank重排模型Id")]
        public long? aIRerankModelsId { get; set; }
        public TAIModels? aIRerankModels { get; set; }
    }
}
