using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using System;
using CoreCommon.Infra.Helpers;

namespace CoreCommon.Data.MongoDBBase.Serializers
{
    public class MyObjectSerializer : IBsonSerializer<object>
    {
        public Type ValueType => typeof(object);

        public object Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            if (context.Reader.GetCurrentBsonType() != BsonType.Document) throw new Exception("Not document");
            var serializer = BsonSerializer.LookupSerializer(typeof(BsonDocument));
            var document = serializer.Deserialize(context, args);

            //var bsonDocument = document.ToBsonDocument();
            //var result = BsonExtensionMethods.ToJson(bsonDocument);
            //return JsonConvert.DeserializeObject<IDictionary<string, object>>(result);

            try
            {
                return ConversionHelper.DerializeObject<object>(document.ToJson());
            }
            catch (Exception)
            {
                return document;
            }
        }

        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
        {
            var serializer = BsonSerializer.LookupSerializer(typeof(BsonDocument));

            var actualType = value?.GetType();

            if(actualType == typeof(BsonDocument))
            {
                serializer.Serialize(context, value);
                return;
            }

            var json = (value == null) ? "{}" : ConversionHelper.SerializeObject(value);
            ////var bsonDocument = BsonDocument.Parse(json);
            var bsonDocument = BsonSerializer.Deserialize<BsonDocument>(json);
            //
            serializer.Serialize(context, bsonDocument.AsBsonValue);
        }
    }
}
