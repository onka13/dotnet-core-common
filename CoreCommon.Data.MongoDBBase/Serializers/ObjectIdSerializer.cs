using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace CoreCommon.Data.MongoDBBase.Serializers
{
    public class ObjectIdSerializer : BsonObjectIdSerializer
    {
        protected override BsonObjectId DeserializeValue(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var bsonReader = context.Reader;
            return new BsonObjectId(bsonReader.ReadObjectId());
        }

        protected override void SerializeValue(BsonSerializationContext context, BsonSerializationArgs args, BsonObjectId value)
        {
            var bsonWriter = context.Writer;
            bsonWriter.WriteObjectId(value.Value);
        }
    }
}
