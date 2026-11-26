using System.ComponentModel;

namespace kevin.Domain.Share.Dtos.AI
{
    /// <summary>
    /// 智能体维度记忆计数（管理端左栏记忆数徽标）
    /// </summary>
    public class dtoAIAgentMemoryCount
    {
        /// <summary>
        /// 智能体Id；0 表示全局记忆（未归属具体智能体）
        /// </summary>
        [Description("智能体Id，0 表示全局记忆")]
        public long AIAppsId { get; set; }

        /// <summary>
        /// 该智能体下的有效记忆条数（未删除且未过期）
        /// </summary>
        [Description("该智能体下的有效记忆条数")]
        public int Count { get; set; }
    }
}
