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
    [Description("智能体长期记忆（用户级）")]
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
        /// 记忆类型：preference偏好/fact事实/task任务/other其他
        /// </summary>
        [Description("记忆类型：preference偏好/fact事实/task任务/other其他")]
        public string MemoryType { get; set; } = "other";

        /// <summary>
        /// 记忆关键词，逗号分隔，用于检索
        /// </summary>
        [Description("记忆关键词，逗号分隔，用于检索")]
        public string Keywords { get; set; } = "";

        /// <summary>
        /// 记忆内容
        /// </summary>
        [Description("记忆内容")]
        public string Content { get; set; } = "";

        /// <summary>
        /// 重要程度0~10，默认5，检索排序参考
        /// </summary>
        [Description("重要程度0~10")]
        public int Importance { get; set; } = 5;

        /// <summary>
        /// 过期时间，为空表示永久有效
        /// </summary>
        [Description("过期时间，为空表示永久有效")]
        public DateTime? ExpireTime { get; set; }
    }
}
