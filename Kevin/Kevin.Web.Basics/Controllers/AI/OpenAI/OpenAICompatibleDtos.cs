using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kevin.Web.Basics.Controllers.AI.OpenAI
{
    #region 请求模型（OpenAI Chat Completions 协议子集）

    /// <summary>
    /// OpenAI 协议聊天请求：只解析本接口用到的字段，temperature/top_p/max_tokens/tools 等
    /// 其余协议字段一律忽略（交由智能体自身配置决定模型参数与工具能力）。
    /// </summary>
    public class OpenAICompatibleChatRequest
    {
        /// <summary>模型名：即智能体 Id，取值来自 GET /v1/models 返回的列表</summary>
        public string? Model { get; set; }

        /// <summary>消息列表：取最后一条 role=user 的消息作为本轮提问，其余历史仅用于识别会话</summary>
        public List<OpenAICompatibleMessage>? Messages { get; set; }

        /// <summary>是否流式输出（true 时按 SSE 下发 chat.completion.chunk 分片）</summary>
        public bool Stream { get; set; }

        /// <summary>会话标识（OpenAI 标准字段）：客户端传同一个值即复用同一会话，作为会话键的最后兜底来源</summary>
        public string? User { get; set; }

        /// <summary>会话标识扩展字段（下划线命名）：会话键的第一来源，优先于 user 字段</summary>
        [JsonPropertyName("conversation_id")]
        public string? ConversationId { get; set; }

        /// <summary>会话标识扩展字段（驼峰命名容错别名）：与 conversation_id 等价</summary>
        [JsonPropertyName("conversationId")]
        public string? ConversationIdCamel { get; set; }
    }

    /// <summary>
    /// OpenAI 协议消息：content 可能是字符串或内容块数组（如 text/image_url 块），
    /// 这里原样保留 JsonElement，由控制器按需解析。
    /// </summary>
    public class OpenAICompatibleMessage
    {
        public string? Role { get; set; }

        public JsonElement Content { get; set; }
    }

    #endregion

    #region 响应模型

    /// <summary>流式分片：chat.completion.chunk</summary>
    public class OpenAICompatibleChunk
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = "";

        [JsonPropertyName("object")]
        public string Object { get; set; } = "chat.completion.chunk";

        [JsonPropertyName("created")]
        public long Created { get; set; }

        [JsonPropertyName("model")]
        public string Model { get; set; } = "";

        [JsonPropertyName("choices")]
        public List<OpenAICompatibleChunkChoice> Choices { get; set; } = new();
    }

    public class OpenAICompatibleChunkChoice
    {
        [JsonPropertyName("index")]
        public int Index { get; set; }

        [JsonPropertyName("delta")]
        public OpenAICompatibleDelta Delta { get; set; } = new();

        [JsonPropertyName("finish_reason")]
        public string? FinishReason { get; set; }
    }

    /// <summary>流式增量：role 只在首帧出现；思考过程以 reasoning_content 字段下发（不支持的客户端会自动忽略）</summary>
    public class OpenAICompatibleDelta
    {
        [JsonPropertyName("role")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Role { get; set; }

        [JsonPropertyName("content")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Content { get; set; }

        [JsonPropertyName("reasoning_content")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ReasoningContent { get; set; }
    }

    /// <summary>非流式完整响应：chat.completion</summary>
    public class OpenAICompatibleCompletion
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = "";

        [JsonPropertyName("object")]
        public string Object { get; set; } = "chat.completion";

        [JsonPropertyName("created")]
        public long Created { get; set; }

        [JsonPropertyName("model")]
        public string Model { get; set; } = "";

        [JsonPropertyName("choices")]
        public List<OpenAICompatibleCompletionChoice> Choices { get; set; } = new();

        [JsonPropertyName("usage")]
        public OpenAICompatibleUsage Usage { get; set; } = new();
    }

    public class OpenAICompatibleCompletionChoice
    {
        [JsonPropertyName("index")]
        public int Index { get; set; }

        [JsonPropertyName("message")]
        public OpenAICompatibleCompletionMessage Message { get; set; } = new();

        [JsonPropertyName("finish_reason")]
        public string FinishReason { get; set; } = "stop";
    }

    public class OpenAICompatibleCompletionMessage
    {
        [JsonPropertyName("role")]
        public string Role { get; set; } = "assistant";

        [JsonPropertyName("content")]
        public string Content { get; set; } = "";

        /// <summary>思考过程（开启智能体思考日志时才有值）</summary>
        [JsonPropertyName("reasoning_content")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ReasoningContent { get; set; }
    }

    public class OpenAICompatibleUsage
    {
        [JsonPropertyName("prompt_tokens")]
        public long PromptTokens { get; set; }

        [JsonPropertyName("completion_tokens")]
        public long CompletionTokens { get; set; }

        [JsonPropertyName("total_tokens")]
        public long TotalTokens { get; set; }
    }

    /// <summary>模型列表：GET /v1/models 响应</summary>
    public class OpenAICompatibleModelList
    {
        [JsonPropertyName("object")]
        public string Object { get; set; } = "list";

        [JsonPropertyName("data")]
        public List<OpenAICompatibleModelItem> Data { get; set; } = new();
    }

    public class OpenAICompatibleModelItem
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = "";

        [JsonPropertyName("object")]
        public string Object { get; set; } = "model";

        [JsonPropertyName("created")]
        public long Created { get; set; }

        [JsonPropertyName("owned_by")]
        public string OwnedBy { get; set; } = "net_ai_web";
    }

    /// <summary>OpenAI 风格错误响应：{"error":{"message","type","param","code"}}</summary>
    public class OpenAICompatibleErrorResponse
    {
        [JsonPropertyName("error")]
        public OpenAICompatibleErrorBody Error { get; set; } = new();
    }

    public class OpenAICompatibleErrorBody
    {
        [JsonPropertyName("message")]
        public string Message { get; set; } = "";

        [JsonPropertyName("type")]
        public string Type { get; set; } = "invalid_request_error";

        [JsonPropertyName("param")]
        public string? Param { get; set; }

        [JsonPropertyName("code")]
        public string? Code { get; set; }
    }

    #endregion
}
