using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Common.Json
{
    public class DateTimeNullConverter : JsonConverter<DateTime?>
    {


        private readonly string _dateFormatString;
        public DateTimeNullConverter()
        {
            _dateFormatString = "yyyy/MM/dd HH:mm:ss";
        }

        public DateTimeNullConverter(string dateFormatString)
        {
            _dateFormatString = dateFormatString;
        }


        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return string.IsNullOrEmpty(reader.GetString()) ? default(DateTime?) : DateTime.Parse(reader.GetString());
        }


        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value?.ToString(_dateFormatString));
        }
    }

}
