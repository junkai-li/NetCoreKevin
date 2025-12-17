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
        }

        public override async Task<IEnumerable<ChatMessage>> GetMessagesAsync(
            CancellationToken cancellationToken)
        {
            var data = await _chatMessageStore.GetMessagesAsync(this.ThreadDbKey, cancellationToken);
            var messages = data.ConvertAll(x => JsonSerializer.Deserialize<ChatMessage>(x.SerializedMessage!)!);
            messages.Reverse(); 
            return messages;
        }

        public override JsonElement Serialize(JsonSerializerOptions? jsonSerializerOptions = null) =>
            // We have to serialize the thread id, so that on deserialization you can retrieve the messages using the same thread id.
            JsonSerializer.SerializeToElement(this.ThreadDbKey);
    }
}
