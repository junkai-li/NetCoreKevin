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

        public KevinChatMessageStore(
              IKevinAIChatMessageStore vectorStore,
                      string aIChatsId, int maxUserTurns = 0, int maxAskTokenBudget = 0)
        {

            this._chatMessageStore = vectorStore ?? throw new ArgumentNullException(nameof(vectorStore));
            this.ThreadDbKey = aIChatsId;
            this.MaxUserTurns = maxUserTurns;
            this.MaxAskTokenBudget = maxAskTokenBudget;
            JsonSerializer.SerializeToElement(this.ThreadDbKey);
        }

        protected override ValueTask<IEnumerable<ChatMessage>> ProvideChatHistoryAsync(
         InvokingContext context, CancellationToken cancellationToken = default)
        {
            var data = _chatMessageStore.GetMessagesAsync(this.ThreadDbKey, cancellationToken, MaxUserTurns).Result;
            var messages = data.OrderByDescending(t => t.CreateTime).ToList().ConvertAll(x => JsonSerializer.Deserialize<ChatMessage>(x.SerializedMessage!)!);
            messages.Reverse();
            messages = messages.ToList();
            // 超出提问Token预算时，从最旧的消息开始丢弃，至少保留最近一条历史（工具消息Text可能为空，按字符数估算）
            if (MaxAskTokenBudget > 0 && messages.Count > 1)
            {
                var totalTokens = messages.Sum(t => t.Text?.Length ?? 0);
                var removeCount = 0;
                while (totalTokens > MaxAskTokenBudget && removeCount < messages.Count - 1)
                {
                    totalTokens -= messages[removeCount].Text?.Length ?? 0;
                    removeCount++;
                }
                if (removeCount > 0)
                {
                    messages = messages.Skip(removeCount).ToList();
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
                    SerializedMessage = JsonSerializer.Serialize(x),
                    MessageText = x.Text
                }).ToList();
                await _chatMessageStore.AddMessagesAsync(adddata, cancellationToken);
            }
        }
    }
}