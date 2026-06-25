using Microsoft.Extensions.AI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace kevin.AI.AgentFramework.Agent.KevinChatMessageStore
{
    /// <summary>
    /// AIContent 序列化/反序列化转换器
    /// 使用 System.Text.Json 处理 AIContent 本体（正确处理 JsonElement 等字段），
    /// $type 类型信息由 Newtonsoft 的 TypeNameHandling.Auto 自动管理，无需手动处理
    /// </summary>
    public class AIContentNewtonsoftConverter : JsonConverter<AIContent>
    {
        public override AIContent? ReadJson(JsonReader reader, Type objectType, AIContent? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            var jObject = JObject.Load(reader);

            var dataJson = jObject["$data"]?.Value<string>();
            if (string.IsNullOrEmpty(dataJson))
                return null;

            // 🔑 objectType 已由 Newtonsoft 的 TypeNameHandling.Auto 解析为具体类型
            // 使用 System.Text.Json 反序列化，正确初始化 JsonElement 等字段
            return (AIContent?)System.Text.Json.JsonSerializer.Deserialize(dataJson, objectType);
        }

        public override void WriteJson(JsonWriter writer, AIContent? value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var type = value.GetType();

            // 🔑 使用 System.Text.Json 序列化 AIContent，正确处理 JsonElement 等字段
            var stjJson = System.Text.Json.JsonSerializer.Serialize(value, type);

            // $type 由 Newtonsoft 的 TypeNameHandling.Auto 自动添加，这里只写 $data
            writer.WriteStartObject();
            writer.WritePropertyName("$data");
            writer.WriteValue(stjJson);
            writer.WriteEndObject();
        }
    }
}