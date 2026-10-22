using Common;
using kevin.AI.AgentFramework.Const;
using kevin.AI.AgentFramework.Dto;
using kevin.AI.AgentFramework.Interfaces;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using System.Text.Json;

namespace kevin.AI.AgentFramework.Agent.KevinChatMessageStore
{
    public sealed class KevinChatMessageStore : ChatHistoryProvider
    {

        private IKevinAIChatMessageStore _chatMessageStore;
        public string ThreadDbKey { get; private set; }
        /// <summary>
        /// 最大用户轮次
        /// </summary>
        public int MaxUserTurns { get; set; } = 0;
        /// <summary>
        /// 提问Token预算（0=不限制），超出时从最旧的消息开始丢弃，优先保留最近的历史（保守估算：1个字符≈1个Token）
        /// </summary>
        public int MaxAskTokenBudget { get; set; } = 0;
        /// <summary>
        /// 工具结果入库时的最大字符数（来自智能体的 ContentLengthLimit）：完整结果只服务当轮对话，写回历史时用截断版，
        /// 避免一次几十万字符的工具输出在后续每一轮都被重复计入模型输入。
        /// </summary>
        public int ToolResultLengthLimit { get; set; } = 0;
        /// <summary>
        /// 会话历史只读：true 时本次调用照常读到完整历史，但不把输入与回复写回去。
        /// <para>给“在一次已有会话上追加一个元任务”用（如让当前智能体生成推荐问题）：
        /// 这类指令与它的回复不是真实对话，写回去会污染后续上下文，但不开只读又拿不到会话历史。</para>
        /// </summary>
        public bool ReadOnlyHistory { get; set; } = false;
        /// <summary>
        /// 入库副本的兜底上限（字符）：ContentLengthLimit 配为 0 的含义是“发给模型时不截断”，
        /// 但历史入库不能因此完全不封顶：一条几 MB 的工具结果会把那行 INSERT 撑过 max_allowed_packet（默认 4MB），
        /// 并在后续每一轮重复全量计入模型输入。当轮交给模型的内容不受这里影响。
        /// </summary>
        private const int StoreResultHardLimit = 200000;
        /// <summary>
        /// 入库副本实际采用的工具结果上限
        /// </summary>
        private int StoreResultLengthLimit => ToolResultLengthLimit > 0 ? ToolResultLengthLimit : StoreResultHardLimit;
        /// <summary>
        /// 单条二进制内容（图片/文件）的粗估输入开销：真实 token 数由模型的分块策略决定，这里取一个与体积无关的保守固定值参与预算计算
        /// </summary>
        private const int BinaryContentEstimatedSize = 1000;

        public KevinChatMessageStore(
              IKevinAIChatMessageStore vectorStore,
                      string aIChatsId, int maxUserTurns = 0, int maxAskTokenBudget = 0, int toolResultLengthLimit = 0)
        {

            this._chatMessageStore = vectorStore ?? throw new ArgumentNullException(nameof(vectorStore));
            this.ThreadDbKey = aIChatsId;
            this.MaxUserTurns = maxUserTurns;
            this.MaxAskTokenBudget = maxAskTokenBudget;
            this.ToolResultLengthLimit = toolResultLengthLimit;
            JsonSerializer.SerializeToElement(this.ThreadDbKey);
        }

        protected override ValueTask<IEnumerable<ChatMessage>> ProvideChatHistoryAsync(
         InvokingContext context, CancellationToken cancellationToken = default)
        {
            var data = _chatMessageStore.GetMessagesAsync(this.ThreadDbKey, cancellationToken, MaxUserTurns).Result;
            var messages = data.OrderByDescending(t => t.CreateTime).ToList().ConvertAll(x => JsonSerializer.Deserialize<ChatMessage>(x.SerializedMessage!)!);
            messages.Reverse();
            messages = messages.ToList();
            // 超出提问Token预算时，从最旧的消息开始丢弃，至少保留最近一条历史；
            // 携带上传文件解析文本的 user 消息（及其之后的对话）优先保护：文件资料只存在于那一轮消息里，
            // 被整条裁掉后模型在后续轮次就再也看不到文件内容，只能回答“没看到文件”。
            var trimmed = false;
            if (MaxAskTokenBudget > 0 && messages.Count > 1)
            {
                // 不能用 ChatMessage.Text 估算：它只拼接 TextContent，工具调用参数、工具结果、二进制内容全会被算成 0，
                // 而恰恰是这些消息最容易撞爆模型输入上限（表现为模型报 "Range of input length should be [1, N]"）
                var sizes = messages.Select(EstimateMessageSize).ToList();
                var totalTokens = sizes.Sum();
                var removeCount = 0;
                // 最旧的含文件消息作为保护起点：它及其之后的对话尽量不裁
                var protectedFrom = messages.Count;
                for (var i = 0; i < messages.Count; i++)
                {
                    if (IsUploadedFileContextMessage(messages[i]))
                    {
                        protectedFrom = i;
                        break;
                    }
                }
                while (totalTokens > MaxAskTokenBudget && removeCount < messages.Count - 1)
                {
                    if (removeCount >= protectedFrom) break; // 再删就要动到文件资料了
                    totalTokens -= sizes[removeCount];
                    removeCount++;
                }
                // 保护文件资料仍放不下（文件本身太接近预算）时回退到原逻辑，保证模型输入不超限
                while (totalTokens > MaxAskTokenBudget && removeCount < messages.Count - 1)
                {
                    totalTokens -= sizes[removeCount];
                    removeCount++;
                }
                if (removeCount > 0)
                {
                    messages = messages.Skip(removeCount).ToList();
                    trimmed = true;
                }
            }
            // 只在确实丢弃过历史时重新对齐起点：切在中间会留下“工具结果找不到对应工具调用”的残缺上下文，模型侧会直接报 400。
            // 未裁剪时不能动起点（首条可能是有意入库的系统消息或开场回复）；System 也不跳过，它不属于残缺工具上下文。
            if (trimmed)
            {
                while (messages.Count > 1 && messages[0].Role != ChatRole.User && messages[0].Role != ChatRole.System)
                {
                    messages.RemoveAt(0);
                }
            }
            if (context.RequestMessages.Count() > 0)
            {
                foreach (var item in context.RequestMessages)
                {
                    if (item.CreatedAt == null)
                    {
                        item.CreatedAt = DateTime.Now.AddSeconds(-1);
                    }
                }
            }
            //新对话
            //if (messages.Count == 0)
            //{
            //    messages.Add(new ChatMessage(ChatRole.User, "请简单介绍一下你自己")); // 可以根据需要自定义系统消息
            //}
            return new(messages);
        }
        protected override async ValueTask StoreChatHistoryAsync(InvokedContext context, CancellationToken cancellationToken = default)
        {
            // 只读会话（如推荐问题这类元任务）：本次输入输出不进对话历史
            if (ReadOnlyHistory) return;
            var responseMessages = context.ResponseMessages ?? Array.Empty<ChatMessage>();
            var allNewMessages = context.RequestMessages.Concat(responseMessages).ToList();
            if (allNewMessages.Count() > 0)
            {
                var adddata = allNewMessages.Select(x => new ChatHistoryItemDto()
                {
                    Key = this.ThreadDbKey + x.MessageId,
                    Timestamp = x.CreatedAt,
                    ThreadId = this.ThreadDbKey,
                    MessageId = x.MessageId,
                    Role = x.Role.Value,
                    SerializedMessage = JsonSerializer.Serialize(ShrinkToolResultsForStore(x)),
                    MessageText = x.Text
                }).ToList();
                await _chatMessageStore.AddMessagesAsync(adddata, cancellationToken);
            }
        }

        /// <summary>
        /// 追加"框架外产生的工具执行结果"到会话历史（如终止轮中网关侧补偿执行的自带工具结果）：
        /// 使下一轮续轮时，助手消息里的工具调用与工具结果保持完整配对（模型侧要求每个调用都有对应结果）。
        /// 写入时间晚于框架已落库的助手调用帧，读取排序（按 CreateTime）天然排在调用帧之后
        /// </summary>
        public async Task AppendToolResultsAsync(IReadOnlyList<FunctionResultContent> results, CancellationToken cancellationToken = default)
        {
            // 只读会话与 StoreChatHistoryAsync 行为一致：本次调用不落库
            if (ReadOnlyHistory || results == null || results.Count == 0) return;
            var adddata = new List<ChatHistoryItemDto>(results.Count);
            foreach (var result in results)
            {
                // MessageId 必须唯一：主键 Key = ThreadDbKey + MessageId，缺省值会与其它消息撞主键
                var message = ShrinkToolResultsForStore(new ChatMessage(ChatRole.Tool, new List<AIContent> { result })
                {
                    MessageId = Guid.NewGuid().ToString("N"),
                    CreatedAt = DateTimeOffset.Now,
                });
                adddata.Add(new ChatHistoryItemDto()
                {
                    Key = this.ThreadDbKey + message.MessageId,
                    Timestamp = message.CreatedAt,
                    ThreadId = this.ThreadDbKey,
                    MessageId = message.MessageId,
                    Role = message.Role.Value,
                    SerializedMessage = JsonSerializer.Serialize(message),
                    MessageText = message.Text
                });
            }
            await _chatMessageStore.AddMessagesAsync(adddata, cancellationToken);
        }

        /// <summary>
        /// 是否为携带上传文件解析文本的 user 消息（按 AIFileUrlsHandle 写入的固定前缀识别，
        /// 兼容历史存量数据：这些消息入库时没有额外标记，只能靠文本前缀认出来）
        /// </summary>
        private static bool IsUploadedFileContextMessage(ChatMessage? message)
        {
            if (message?.Role != ChatRole.User || message.Contents == null) return false;
            foreach (var content in message.Contents)
            {
                if (content is TextContent text && text.Text != null && text.Text.Contains(SystemPrompt.UploadedFileContextHeader))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 估算一条消息实际占用的输入长度（保守按字符数≈Token数），涵盖文本、工具调用参数、工具结果与二进制内容
        /// </summary>
        private int EstimateMessageSize(ChatMessage? message)
        {
            if (message?.Contents == null) return 0;
            var size = 0;
            foreach (var content in message.Contents)
            {
                switch (content)
                {
                    case TextContent text:
                        size += text.Text?.Length ?? 0;
                        break;
                    case FunctionCallContent call:
                        size += (call.Name?.Length ?? 0) + SerializeSize(call.Arguments);
                        break;
                    case FunctionResultContent result:
                        size += result.CallId?.Length ?? 0;
                        // 历史重发时拿到的是截断后的结果，预算也按截断后的体积计入
                        size += Math.Min(SerializeSizeText(result.Result).Length, StoreResultLengthLimit);
                        break;
                    case DataContent or UriContent:
                        size += BinaryContentEstimatedSize;
                        break;
                    default:
                        size += SerializeSize(content);
                        break;
                }
            }
            return size;
        }

        /// <summary>
        /// 将任意内容转成参与预算计算的文本：字符串直接用长度，其他类型序列化后取长度
        /// </summary>
        private static string SerializeSizeText(object? value)
        {
            if (value == null) return "";
            if (value is string s) return s;
            try
            {
                return JsonSerializer.Serialize(value);
            }
            catch
            {
                return value.ToString() ?? "";
            }
        }

        private static int SerializeSize(object? value) => SerializeSizeText(value).Length;

        /// <summary>
        /// 工具结果超长时，返回一份仅用于入库的副本（不改动当轮已交给模型的原始消息），
        /// 使后续每轮重发的历史体积可控。无工具结果或未超限时直接返回原消息。
        /// </summary>
        private ChatMessage ShrinkToolResultsForStore(ChatMessage message)
        {
            var sourceContents = message.Contents;
            if (sourceContents == null || sourceContents.Count == 0) return message;
            var limit = StoreResultLengthLimit;
            // 每条结果只序列化一次（先把待截断的下标与文本收集起来）
            var oversize = new List<(int Index, FunctionResultContent Result, string Text)>();
            for (var i = 0; i < sourceContents.Count; i++)
            {
                if (sourceContents[i] is not FunctionResultContent result) continue;
                var text = SerializeSizeText(result.Result);
                if (text.Length > limit)
                {
                    oversize.Add((i, result, text));
                }
            }
            if (oversize.Count == 0) return message;

            var contents = sourceContents.ToList();
            foreach (var (index, result, text) in oversize)
            {
                contents[index] = new FunctionResultContent(result.CallId,
                    SystemPrompt.ContentLimitPromptText + StringHelper.SubstringText(text, limit));
            }
            return new ChatMessage(message.Role, contents)
            {
                AuthorName = message.AuthorName,
                CreatedAt = message.CreatedAt,
                MessageId = message.MessageId,
                AdditionalProperties = message.AdditionalProperties,
            };
        }
    }
}