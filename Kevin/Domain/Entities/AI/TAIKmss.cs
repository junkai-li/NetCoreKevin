using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.Domain.Entities.AI
{
    /// <summary>
    /// TAIKmss 知识库文档设置
    /// </summary>
    [Table("TAIKmss")]
    [Description("TAIKmss")]
    public partial class TAIKmss: CUD_User
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
        public TAIModels? aIModels { get; set; }
    }
}
