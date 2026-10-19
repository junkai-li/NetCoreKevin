using Microsoft.EntityFrameworkCore;

namespace kevin.Domain.Entities.AI
{
    /// <summary>
    /// 用户月度 Token 额度（每月月初由定时任务颁发，聊天完成后按输入+输出 Token 累加已用）
    /// </summary>
    [Table("TAIUserTokenQuota")]
    [Description("用户月度Token额度")]
    [Index(nameof(TenantId), nameof(UserId), nameof(QuotaMonth), IsUnique = true)]
    public class TAIUserTokenQuota : CD
    {
        /// <summary>
        /// 额度归属用户Id
        /// </summary>
        [Description("额度归属用户Id")]
        public long UserId { get; set; }

        /// <summary>
        /// 额度月份，格式 yyyyMM（如 202610）
        /// </summary>
        [Description("额度月份 yyyyMM")]
        public int QuotaMonth { get; set; }

        /// <summary>
        /// 当月可用额度（Token 数）
        /// </summary>
        [Description("当月可用额度")]
        public long TotalQuota { get; set; }

        /// <summary>
        /// 当月已用额度（Token 数，按每轮对话的输入+输出累计）
        /// </summary>
        [Description("当月已用额度")]
        public long UsedQuota { get; set; }
    }
}
