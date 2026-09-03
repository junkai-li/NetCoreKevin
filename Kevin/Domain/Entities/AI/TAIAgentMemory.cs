using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.Domain.Entities.AI
{
    /// <summary>
    /// TAIAgentMemory
    /// </summary>
    [Table("TAIAgentMemory")]
    [Description("智能体记忆（用户级，含长期与短期）")]
    [Index(nameof(AIAppsId))]
    [Index(nameof(AIChatsId))]
    [Index(nameof(UserId))]
    [Index(nameof(MemoryType))]
    [Index(nameof(Keywords))]
    public class TAIAgentMemory : CUD
    {
        /// <summary>
        /// 记忆归属用户Id（检索主维度，跨智能体、跨会话共享）
        /// </summary>
        [Description("记忆归属用户Id")]
        public long UserId { get; set; }

        /// <summary>
        /// 写入记忆时的智能体Id（仅记录来源）
        /// </summary>
        [Description("写入记忆时的智能体Id")]
        public long AIAppsId { get; set; }

        /// <summary>
        /// 写入记忆时的对话Id（仅记录来源）
        /// </summary>
        [Description("写入记忆时的对话Id")]
        public long AIChatsId { get; set; }

        /// <summary>
        /// 记忆类型（7 种）：preference偏好/fact事实/task短期记忆/decision决策/pitfall踩坑/skill技能/other其他
        /// </summary>
        [Description("记忆类型：preference偏好/fact事实/task短期记忆/decision决策/pitfall踩坑/skill技能/other其他")]
        [MaxLength(50)]
        public string MemoryType { get; set; } = "other";

        /// <summary>
        /// 记忆关键词，逗号分隔，用于检索
        /// </summary>
        [MaxLength(200)]
        [Description("记忆关键词，逗号分隔，用于检索")]
        public string Keywords { get; set; } = "";

        /// <summary>
        /// 记忆内容
        /// </summary>
        [Description("记忆内容")]
        public string Content { get; set; } = "";

        /// <summary>
        /// 重要程度 0~10（AI 工具调用时必填）：9-10核心约束/7-8重要决策偏好/5-6一般事实经验/3-4边缘信息/0-2低价值。检索排序参考
        /// </summary>
        [Description("重要程度 0~10")]
        public int Importance { get; set; } = 5;

        /// <summary>
        /// 短期记忆（task）的过期时间；为空表示长期记忆永久有效
        /// </summary>
        [Description("短期记忆过期时间，为空表示长期记忆永久有效")]
        public DateTime? ExpireTime { get; set; }
    }
}
