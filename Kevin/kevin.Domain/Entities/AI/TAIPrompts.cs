using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.Domain.Entities.AI
{
    /// <summary>
    /// AI提示词配置
    /// </summary>
    [Table("TAIPrompts")]
    [Description("AI提示词配置")]
    public partial class TAIPrompts : CUD_User
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
