using kevin.Domain.Share.Dtos.AI;

namespace kevin.Domain.Interfaces.IServices.AI
{
    /// <summary>
    /// OpenAI 协议兼容接口的聊天服务：
    /// 解析 OpenAI Chat Completions 请求（智能体定位、会话衔接、工具透传），
    /// 复用智能体处理管线产出回答；流式分片由服务自建 chat.completion.chunk 适配器下发。
    /// </summary>
    public interface IOpenAICompatibleChatService : IBaseService
    {
        /// <summary>
        /// 非流式聊天：完整回答一次性返回（含思考内容、透传工具调用与 Token 用量）。
        /// </summary>
        /// <param name="req">OpenAI Chat Completions 请求</param>
        /// <param name="headerConversationId">请求头 X-Conversation-Id（会话键第一来源，可为空）</param>
        /// <param name="cancellationToken">请求中止令牌</param>
        Task<OpenAICompatibleChatOutcome> ChatAsync(OpenAICompatibleChatRequest req, string? headerConversationId, CancellationToken cancellationToken);

        /// <summary>
        /// 流式聊天：分片按 chat.completion.chunk 帧实时写入当前 HTTP 响应，结束补 finish 帧 + [DONE]。
        /// </summary>
        /// <param name="req">OpenAI Chat Completions 请求</param>
        /// <param name="headerConversationId">请求头 X-Conversation-Id（会话键第一来源，可为空）</param>
        /// <param name="cancellationToken">请求中止令牌</param>
        Task ChatStreamAsync(OpenAICompatibleChatRequest req, string? headerConversationId, CancellationToken cancellationToken);
    }
}
