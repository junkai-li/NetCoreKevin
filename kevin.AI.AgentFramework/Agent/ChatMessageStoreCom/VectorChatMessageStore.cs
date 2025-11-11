using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using System.Drawing;
using System.Text.Json;
using Microsoft.Agents.AI;

namespace kevin.AI.AgentFramework.Agent.ChatMessageStoreCom
{
    //AddMessagesAsync - 调用以向存储区添加新消息。
    //GetMessagesAsync - 调用以从存储区检索消息。
    //GetMessagesAsync 应按时间顺序按升序返回消息。 它返回的所有消息将在调用ChatClientAgent的基础IChatClient时使用。 因此，此方法必须考虑基础模型的限制，并且只返回模型可以处理的消息数。

    //在从GetMessagesAsync返回消息之前，应完成任何聊天记录减少逻辑处理，例如摘要或修整。

    //Serialization
    //在创建线程以及从序列化状态恢复线程时，ChatMessageStore 实例会被创建并附加到 AgentThread。

    //虽然组成聊天历史记录的实际消息存储在外部， ChatMessageStore 但实例可能需要存储密钥或其他状态来标识外部存储中的聊天历史记录。

    //若要允许持久化线程，需要实现 SerializeStateAsync 类的 ChatMessageStore 方法。 还需要提供采用 JsonElement 参数的构造函数，该构造函数可用于在恢复线程时反序列化状态。

    //示例 ChatMessageStore 实现
    //以下示例实现将聊天消息存储在向量存储中。

    //AddMessagesAsync 对每个消息使用唯一键将消息插入向量存储中。

    //GetMessagesAsync 从向量存储中检索当前线程的消息，按时间戳对它们进行排序，并按升序返回它们。

    //收到第一条消息时，存储会生成线程的唯一键，然后用于标识矢量存储中的聊天历史记录以供后续调用。

    //唯一键存储在ThreadDbKey属性中，该属性通过SerializeStateAsync方法和接受JsonElement的构造函数进行序列化和反序列化。 因此，此密钥将保留为状态的 AgentThread 一部分，允许稍后恢复线程并继续使用相同的聊天历史记
    public sealed class VectorChatMessageStore : ChatMessageStore
    {
        private readonly VectorStore _vectorStore;

        public VectorChatMessageStore(
            VectorStore vectorStore,
            JsonElement serializedStoreState,
            JsonSerializerOptions? jsonSerializerOptions = null)
        {
            this._vectorStore = vectorStore ?? throw new ArgumentNullException(nameof(vectorStore));
            if (serializedStoreState.ValueKind is JsonValueKind.String)
            {
                this.ThreadDbKey = serializedStoreState.Deserialize<string>();
            }
        }

        public string? ThreadDbKey { get; private set; }

        public override async Task AddMessagesAsync(
            IEnumerable<ChatMessage> messages,
            CancellationToken cancellationToken)
        {
            this.ThreadDbKey ??= Guid.NewGuid().ToString("N");
            var collection = this._vectorStore.GetCollection<string, ChatHistoryItem>("ChatHistory");
            await collection.EnsureCollectionExistsAsync(cancellationToken);
            await collection.UpsertAsync(messages.Select(x => new ChatHistoryItem()
            {
                Key = this.ThreadDbKey + x.MessageId,
                Timestamp = DateTimeOffset.UtcNow,
                ThreadId = this.ThreadDbKey,
                SerializedMessage = JsonSerializer.Serialize(x),
                MessageText = x.Text
            }), cancellationToken);
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
            var collection = this._vectorStore.GetCollection<string, ChatHistoryItem>("ChatHistory");
            await collection.EnsureCollectionExistsAsync(cancellationToken);
            var records = await collection
                .GetAsync(
                    x => x.ThreadId == this.ThreadDbKey, 10,
                    new() { OrderBy = x => x.Descending(y => y.Timestamp) },
                    cancellationToken)
                .ToListAsync(cancellationToken);
            var messages = records.ConvertAll(x => JsonSerializer.Deserialize<ChatMessage>(x.SerializedMessage!)!);
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

        private sealed class ChatHistoryItem
        {
            [VectorStoreKey]
            public string? Key { get; set; }
            [VectorStoreData]
            public string? ThreadId { get; set; }
            [VectorStoreData]
            public DateTimeOffset? Timestamp { get; set; }
            [VectorStoreData]
            public string? SerializedMessage { get; set; }
            [VectorStoreData]
            public string? MessageText { get; set; }
        }
    }
}
