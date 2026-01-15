using kevin.Domain.Share.Dtos.Bases;
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
        /// 详情列表
        /// </summary>
        public List<AIKmsDetailsDto> AIKmssDetailsList { get; set; } = new List<AIKmsDetailsDto>();
    }
}
