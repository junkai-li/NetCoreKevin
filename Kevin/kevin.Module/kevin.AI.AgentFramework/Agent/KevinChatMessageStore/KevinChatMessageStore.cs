using kevin.AI.AgentFramework.Dto;
using kevin.AI.AgentFramework.Interfaces;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using System.Drawing;
using System.Text.Json;


namespace kevin.AI.AgentFramework.Agent.KevinChatMessageStore
{
    public sealed class KevinChatMessageStore : ChatMessageStore
    {
        private IKevinAIChatMessageStore _chatMessageStore;
        public string ThreadDbKey { get; private set; }
        public KevinChatMessageStore(
            IKevinAIChatMessageStore vectorStore,
            JsonElement serializedStoreState,
            string AIChatsId,
            JsonSerializerOptions? jsonSerializerOptions = null)
        {
            this._chatMessageStore = vectorStore ?? throw new ArgumentNullException(nameof(vectorStore));
            this.ThreadDbKey = AIChatsId; 
        }



        public override async Task AddMessagesAsync(
            IEnumerable<ChatMessage> messages,
            CancellationToken cancellationToken)
        {
            await _chatMessageStore.AddMessagesAsync(messages.Select(x => new ChatHistoryItemDto()
            {
                Key = this.ThreadDbKey + x.MessageId,
                Timestamp = DateTimeOffset.UtcNow,
                ThreadId = this.ThreadDbKey,
                MessageId = x.MessageId,
                Role = x.Role.Value,
                SerializedMessage = JsonSerializer.Serialize(x),
                MessageText = x.Text
            }).ToList(), cancellationToken);
            // 设置前景色为红色
            // 保存原始颜色，以便之后恢复
            ConsoleColor originalColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("聊天消息记录:", Color.Red);
            messages.Select(x => x.Text).ToList().ForEach(t => Console.WriteLine(t));
            // 设置前景色为红色
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("聊天消息记录添加完成", Color.Red);
            // 恢复原始颜色
            Console.ForegroundColor = originalColor;
        }

        public override async Task<IEnumerable<ChatMessage>> GetMessagesAsync(
            CancellationToken cancellationToken)
        {
            var data = await _chatMessageStore.GetMessagesAsync(this.ThreadDbKey, cancellationToken);
            var messages = data.ConvertAll(x => JsonSerializer.Deserialize<ChatMessage>(x.SerializedMessage!)!);
            messages.Reverse();
            ConsoleColor originalColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("所有聊天消息记录开始:", Color.Red);
            messages.Select(x => x.Text).ToList().ForEach(t => Console.WriteLine(t));
            Console.WriteLine("所有聊天消息记录结束:", Color.Red);
            // 恢复原始颜色
            Console.ForegroundColor = originalColor;
            return messages;
        }

        public override JsonElement Serialize(JsonSerializerOptions? jsonSerializerOptions = null) =>
            // We have to serialize the thread id, so that on deserialization you can retrieve the messages using the same thread id.
            JsonSerializer.SerializeToElement(this.ThreadDbKey);
    }
}
