using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.Domain.Share.Dtos.AI
{
    public class AIAppsBindSkillToolsDto
    {
        /// <summary>
        ///技能工具ID
        /// </summary>
        public long AISkillToolManagementId { get; set; }
        /// <summary>
        /// 技能工具名称
        /// </summary>
        public string AISkillToolManagementName { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [StringLength(500)]
        [Description("描述")]
        public String? AISkillToolManagementDescription { get; set; }

        /// <summary>
        /// 是否选中
        /// </summary>
        public bool IsSelect { get; set; }
    }
}
