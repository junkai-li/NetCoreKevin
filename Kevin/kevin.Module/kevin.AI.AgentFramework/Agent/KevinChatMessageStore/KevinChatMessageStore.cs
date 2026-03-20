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

        public KevinChatMessageStore(
              IKevinAIChatMessageStore vectorStore,
                      string AIChatsId)
        {

            this._chatMessageStore = vectorStore ?? throw new ArgumentNullException(nameof(vectorStore));
            this.ThreadDbKey = AIChatsId;
            JsonSerializer.SerializeToElement(this.ThreadDbKey);
        }

        protected override ValueTask<IEnumerable<ChatMessage>> ProvideChatHistoryAsync(
         InvokingContext context, CancellationToken cancellationToken = default)
        {
            var data = _chatMessageStore.GetMessagesAsync(this.ThreadDbKey, cancellationToken).Result;
            var messages = data.ConvertAll(x => JsonSerializer.Deserialize<ChatMessage>(x.SerializedMessage!)!);
            messages.Reverse();
            messages = messages.OrderBy(t => t.CreatedAt).ToList();
            if (context.RequestMessages.Count() == 1)
            {
                foreach (var item in context.RequestMessages)
                {
                    item.CreatedAt= DateTimeOffset.UtcNow;
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
            var allNewMessages = context.RequestMessages.Concat(context.ResponseMessages ?? []);
            if (allNewMessages.Count() > 0)
            {
                await _chatMessageStore.AddMessagesAsync(allNewMessages.Select(x => new ChatHistoryItemDto()
                {
                    Key = this.ThreadDbKey + x.MessageId,
                    Timestamp = x.CreatedAt,
                    ThreadId = this.ThreadDbKey,
                    MessageId = x.MessageId,
                    Role = x.Role.Value,
                    SerializedMessage = JsonSerializer.Serialize(x),
                    MessageText = x.Text
                }).ToList(), cancellationToken);
            }
        }
    }
}
