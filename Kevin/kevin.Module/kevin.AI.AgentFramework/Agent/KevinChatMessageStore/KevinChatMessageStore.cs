using kevin.AI.AgentFramework.Dto;
using kevin.AI.AgentFramework.Interfaces;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

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

        public KevinChatMessageStore(
              IKevinAIChatMessageStore vectorStore,
                      string aIChatsId, int maxUserTurns = 0)
        {

            this._chatMessageStore = vectorStore ?? throw new ArgumentNullException(nameof(vectorStore));
            this.ThreadDbKey = aIChatsId;
            this.MaxUserTurns = maxUserTurns;
        }
        // 🔑 使用 Newtonsoft.Json
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.None,
            NullValueHandling = NullValueHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Auto, // 🔑 自动添加类型信息
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore, // 🔑 忽略循环引用
            Converters = new List<JsonConverter>
            {
                new StringEnumConverter(),
                new AIContentNewtonsoftConverter(),
            }
        };
        protected override ValueTask<IEnumerable<ChatMessage>> ProvideChatHistoryAsync(
         InvokingContext context, CancellationToken cancellationToken = default)
        {
            var data = _chatMessageStore.GetMessagesAsync(this.ThreadDbKey, cancellationToken, MaxUserTurns).Result;
            var messages = data
                      .OrderBy(t => t.Timestamp)
                      .Select(x => JsonConvert.DeserializeObject<ChatMessage>(x.SerializedMessage!, SerializerSettings)!)
                      .ToList();
            messages.Reverse();
            messages = messages.ToList();
            if (context.RequestMessages.Count() > 0)
            {
                foreach (var item in context.RequestMessages)
                {
                    if (item.CreatedAt == null)
                    {
                        item.CreatedAt = DateTimeOffset.UtcNow;
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
            var responseMessages = context.ResponseMessages ?? Array.Empty<ChatMessage>();
            var allNewMessages = context.RequestMessages.Concat(responseMessages).ToList();
            var toolsMessages = allNewMessages.Where(x => x.Role == ChatRole.Tool).OrderBy(t => t.CreatedAt).ToList();
            var toolsMessagesI = 0;
            //修复assistant必须在tool之前CreatedAt时间问题 保证顺序正确性
            if (toolsMessages.Count > 0)
            {
                foreach (var item in allNewMessages)
                {
                    if (item.Role == ChatRole.Assistant)
                    {
                        //是否工具调用提示
                        if (string.IsNullOrEmpty(item.Text))
                        {
                            toolsMessages[toolsMessagesI].CreatedAt = item.CreatedAt!.Value.AddMilliseconds(1);
                            toolsMessagesI++;
                        }
                    }
                }
            }

            if (allNewMessages.Count() > 0)
            {
                var adddata = allNewMessages.Select(x => new ChatHistoryItemDto()
                {
                    Key = this.ThreadDbKey + x.MessageId,
                    Timestamp = x.CreatedAt,
                    ThreadId = this.ThreadDbKey,
                    MessageId = x.MessageId,
                    Role = x.Role.Value,
                    SerializedMessage = JsonConvert.SerializeObject(x, SerializerSettings),
                    MessageText = x.Text
                }).ToList();
                await _chatMessageStore.AddMessagesAsync(adddata, cancellationToken);
            }
        }



    }
}