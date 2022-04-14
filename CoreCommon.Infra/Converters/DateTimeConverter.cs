using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace CoreCommon.Infrastructure.Converters
{
    /// <summary>
    /// DateTime converter for JSON serialization and deserialization
    /// </summary>
    public class DateTimeConverter : DateTimeConverterBase
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return DateTime.Parse(reader.Value.ToString());
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            DateTime dateTime = (DateTime)value;
            writer.WriteValue(dateTime <= DateTime.MinValue ? "" : dateTime.ToString("yyyy-MM-ddTHH:mm:sszzz"));
        }
    }
}
