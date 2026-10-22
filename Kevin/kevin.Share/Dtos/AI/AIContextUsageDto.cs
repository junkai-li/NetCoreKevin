using System.ComponentModel;

namespace kevin.Domain.Share.Dtos.AI
{
    /// <summary>
    /// 当前会话的上下文使用量
    /// </summary>
    public class AIContextUsageDto
    {
        /// <summary>
        /// 当前上下文估算 Token = 未压缩历史 + 压缩摘要（统一估算：1 字符≈0.6 Token）
        /// </summary>
        [Description("当前上下文估算 Token")]
        public int UsedTokens { get; set; }

        /// <summary>
        /// 可用提问预算（模型提问上限 − 回答预留），0 表示模型未配置上限
        /// </summary>
        [Description("可用提问预算")]
        public int MaxTokens { get; set; }

        /// <summary>
        /// 未压缩历史消息估算 Token
        /// </summary>
        [Description("未压缩历史估算 Token")]
        public int HistoryTokens { get; set; }

        /// <summary>
        /// 压缩摘要估算 Token
        /// </summary>
        [Description("压缩摘要估算 Token")]
        public int CompactionTokens { get; set; }

        /// <summary>
        /// 最近一次真实请求的输入 Token（模型返回的 usage，仅供参考）
        /// </summary>
        [Description("最近一次真实输入 Token")]
        public int LastInputTokens { get; set; }

        /// <summary>
        /// 未压缩历史占可用提问预算的百分比（0-100），与自动压缩触发判定口径一致（不含压缩摘要）
        /// </summary>
        [Description("未压缩历史占可用提问预算的百分比")]
        public int Percent { get; set; }

        /// <summary>
        /// 自动压缩触发比例（未开启压缩时为 0）
        /// </summary>
        [Description("自动压缩触发比例")]
        public int TriggerRatio { get; set; }
    }
}
