﻿using CoreCommon.Infra.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Globalization;
using System.IO;

namespace CoreCommon.Infra.Helpers
{
    /// <summary>
    /// Conversion Helpers.
    /// </summary>
    public static class ConversionHelper
    {
        /// <summary>
        /// Returns an object of the specified type and whose value is equivalent to the specified object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Serializes the specified System.Object and writes the JSON structure using the specified System.IO.TextWriter.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="culture"></param>
        /// <param name="isCamelCase"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Deserializes the JSON structure contained by the specified Newtonsoft.Json.JsonReader
        //  into an instance of the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string json, CultureInfo culture = null)
        {
            var reader = new JsonTextReader(new StringReader(json));
            JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings { Culture = culture });
            serializer.Converters.Add(new FormattedDecimalConverter(culture));
            serializer.Converters.Add(new DateTimeConverter());
            serializer.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            return serializer.Deserialize<T>(reader);
        }

        /// <summary>
        /// Clones an object with using JSON deserizalitaion and serialization.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T JsonClone<T>(this T obj)
        {
            var deserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(obj), deserializeSettings);
        }

        /// <summary>
        /// Converts an object type with using JSON.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
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
