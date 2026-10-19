namespace kevin.Domain.Share.Dtos.AI
{
    /// <summary>
    /// 当前用户当月 Token 额度（个人信息页展示）
    /// </summary>
    public class AIUserTokenQuotaDto
    {
        /// <summary>用户Id</summary>
        public long UserId { get; set; }

        /// <summary>额度月份（yyyyMM，如 202610）</summary>
        public int QuotaMonth { get; set; }

        /// <summary>当月可用额度</summary>
        public long TotalQuota { get; set; }

        /// <summary>当月已用额度</summary>
        public long UsedQuota { get; set; }

        /// <summary>当月剩余额度</summary>
        public long RemainingQuota { get; set; }
    }
}
