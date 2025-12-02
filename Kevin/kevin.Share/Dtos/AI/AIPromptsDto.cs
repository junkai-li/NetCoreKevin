using kevin.Domain.Share.Dtos.Bases;
using System.ComponentModel.DataAnnotations;

namespace kevin.Domain.Share.Dtos.AI
{
    public class AIPromptsDto : CUD_User_Dto
    {
        /// <summary>
        /// 名称
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// 提示词
        /// </summary>
        [Required]
        [StringLength(1500)]
        public string Prompt { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [StringLength(500)]
        public string? Description { get; set; }
    }
}
