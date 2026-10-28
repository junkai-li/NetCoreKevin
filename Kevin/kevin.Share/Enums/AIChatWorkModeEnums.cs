namespace kevin.Domain.Share.Enums
{
    /// <summary>
    /// AI聊天会话工作模式（会话级、仅作本轮入参不落库）
    /// </summary>
    public enum AIChatWorkModeEnums
    {
        /// <summary>
        /// 默认模式：高效执行并完成任务，可调用全部已开启的能力
        /// </summary>
        Default = 0,

        /// <summary>
        /// 计划模式：本轮只输出可审阅的执行计划，等待用户确认后才执行
        /// </summary>
        Plan = 1,

        /// <summary>
        /// 仅问答模式：只回答问题、给思路，不调用执行类工具（保留知识库检索）
        /// </summary>
        Ask = 2
    }
}
