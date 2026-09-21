namespace kevin.Domain.Interfaces.IServices.AI
{
    /// <summary>
    /// 聊天处理过程中的流式输出通道抽象（端口）：应用服务的主流程只依赖这个接口，
    /// 具体是走 SignalR 推送还是 SSE 写回响应由应用层的适配器实现决定，
    /// 从而让普通新增与 SSE 新增共用同一套处理逻辑，只有输出端不同。
    /// </summary>
    public interface IChatStreamOutput
    {
        /// <summary>
        /// 下发一条流式分片。
        /// </summary>
        /// <param name="channel">通道/事件名，与前端监听名一一对应：processmsg / aimsg / aIToolsContentMsg / aIReasoningContentMsg</param>
        /// <param name="message">分片内容</param>
        Task WriteAsync(string channel, string message);
    }
}
