using kevin.Domain.Share.Dtos;
using System.ComponentModel;

namespace kevin.Domain.Share.Dtos.AI
{
    /// <summary>
    /// 智能体记忆分页查询参数（继承通用分页 DTO，扩展记忆专属筛选字段；通用 dtoPagePar&lt;T&gt; 零改动）
    /// </summary>
    public class dtoAIAgentMemoryQuery : dtoPagePar<string>
    {
        /// <summary>
        /// 记忆归属智能体Id；0 表示不过滤（0 也可用于查看全局记忆）
        /// </summary>
        [Description("记忆归属智能体Id，0 表示不过滤")]
        public long AIAppsId { get; set; }

        /// <summary>
        /// 重要程度下界（含边界）；null 表示不过滤。三档映射：高=8 / 中=5 / 低=0
        /// </summary>
        [Description("重要程度下界（含边界），null 表示不过滤；三档：高8/中5/低0")]
        public int? ImportanceMin { get; set; }

        /// <summary>
        /// 重要程度上界（含边界）；null 表示不过滤。三档映射：高=10 / 中=7 / 低=4
        /// </summary>
        [Description("重要程度上界（含边界），null 表示不过滤；三档：高10/中7/低4")]
        public int? ImportanceMax { get; set; }
    }
}
