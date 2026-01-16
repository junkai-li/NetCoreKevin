using kevin.Domain.Share.Dtos.Bases;
using kevin.Domain.Share.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace kevin.Domain.Share.Dtos.AI
{
    public class AIKmssDto : CUD_User_Dto
    {
        private int documentCount;

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
    }
}
