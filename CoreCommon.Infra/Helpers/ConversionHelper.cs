using CoreCommon.Infra.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Globalization;
using System.IO;

namespace CoreCommon.Infra.Helpers
{
    public static class ConversionHelper
    {
        public static T ChangeType<T>(object value)
        {
            var t = typeof(T);

            if (value == null)
            {
                return default(T);
            }

            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                t = Nullable.GetUnderlyingType(t);
            }
            return (T)Convert.ChangeType(value, t);
        }

        public static string Serialize(object obj, CultureInfo culture = null, bool isCamelCase = false)
        {
            var writer = new StringWriter();
            JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings { Culture = culture, Formatting = Formatting.Indented });
            if (isCamelCase)
            {
                serializer.ContractResolver = new CamelCasePropertyNamesContractResolver();
            }
            serializer.Converters.Add(new FormattedDecimalConverter(culture));
            serializer.Converters.Add(new DateTimeConverter());
            serializer.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            serializer.Serialize(writer, obj);
            return writer.ToString();
        }

        public static T Deserialize<T>(string json, CultureInfo culture = null)
        {
            var reader = new JsonTextReader(new StringReader(json));
            JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings { Culture = culture });
            serializer.Converters.Add(new FormattedDecimalConverter(culture));
            serializer.Converters.Add(new DateTimeConverter());
            serializer.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            return serializer.Deserialize<T>(reader);
        }

        public static T JsonClone<T>(this T obj)
        {
            var deserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(obj), deserializeSettings);
        }

        public static T Cast<T>(object obj)
        {
            var deserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(obj), deserializeSettings);
        }

        public static T ConvertTo<T>(object value)
        {
            try
            {
                if (value == null) return default(T);
                return Deserialize<T>(value.ToString());
            }
            catch
            {
                try
                {
                    Newtonsoft.Json.Linq.JObject jObject = (Newtonsoft.Json.Linq.JObject)value;
                    var json = jObject.ToString();
                    return Deserialize<T>(json);
                }
                catch (Exception)
                {

                }
                return default(T);
            }
        }
    }
}
