﻿using Newtonsoft.Json;
using System;
using System.Globalization;

namespace CoreCommon.Infrastructure.Converters
{
    /// <summary>
    /// Decimal converter for JSON serialization and deserialization
    /// </summary>
    public class FormattedDecimalConverter : JsonConverter
    {
        private CultureInfo culture;

        public FormattedDecimalConverter(CultureInfo culture)
        {
            this.culture = culture;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(decimal) ||
                    objectType == typeof(double) ||
                    objectType == typeof(float);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(Convert.ToString(value, culture));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return reader.Value;
        }
    }
}
