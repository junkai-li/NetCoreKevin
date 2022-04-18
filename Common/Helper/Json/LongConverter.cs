using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Common.Json
{
    public class LongConverter : JsonConverter<long>
    {

        public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                if (long.TryParse(reader.GetString(), out long l))
                {
                    return l;
                }
            }
            return reader.GetInt64();
        }


        public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }

}
