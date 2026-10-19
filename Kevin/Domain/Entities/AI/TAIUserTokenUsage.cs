using Microsoft.EntityFrameworkCore;

namespace kevin.Domain.Entities.AI
{
    /// <summary>
    /// 用户 Token 使用明细（聊天完成后由领域事件按聊天历史记录 Id 写入，一条聊天记录对应一条明细）
    /// </summary>
    [Table("TAIUserTokenUsage")]
    [Description("用户Token使用明细")]
    [Index(nameof(UserId), nameof(QuotaMonth))]
    [Index(nameof(AIChatHistorysId), IsUnique = true)]
    public class TAIUserTokenUsage : CD
    {
        /// <summary>
        /// 使用人Id
        /// </summary>
        [Description("使用人Id")]
        public long UserId { get; set; }

        /// <summary>
        /// 额度月份，格式 yyyyMM（如 202610）
        /// </summary>
        [Description("额度月份 yyyyMM")]
        public int QuotaMonth { get; set; }

        /// <summary>
        /// 对应的聊天历史记录Id（回复记录）
        /// </summary>
        [Description("聊天历史记录Id")]
        public long AIChatHistorysId { get; set; }

        /// <summary>
        /// 输入消耗的 Token 数
        /// </summary>
        [Description("输入消耗Token数")]
        public long InputTokenCount { get; set; }

        /// <summary>
        /// 输出消耗的 Token 数
        /// </summary>
        [Description("输出消耗Token数")]
        public long OutputTokenCount { get; set; }

        /// <summary>
        /// 本轮合计消耗的 Token 数（输入+输出）
        /// </summary>
        [Description("合计消耗Token数")]
        public long TotalTokenCount { get; set; }
    }
}
