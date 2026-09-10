namespace kevin.Domain.Share.Enums
{
    /// <summary>
    /// AI聊天记录发送状态
    /// </summary>
    public enum AIChatHistorysSendStatusEnums
    {
        /// <summary>
        /// 正常（发送成功并拿到回复）
        /// </summary>
        Success = 0,
        /// <summary>
        /// 失败（模型或上下文处理环节报错，可在前端点红色按钮重试）
        /// </summary>
        Fail = 1,
        /// <summary>
        /// 重试中：本次是对上一轮失败的重新发送，还没拿到回复；若进程中途被打断，记录会停在这个状态
        /// </summary>
        Retrying = 2
    }
}
