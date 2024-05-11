using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace MongoDbEncryptionAesGcm;

public class EncryptedStringSerializer(DataSecurityService securityService)
    : SerializerBase<string?>
{
    public override string? Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var type = context.Reader.GetCurrentBsonType();

        switch (type)
        {
            case BsonType.Null:
                context.Reader.ReadNull();
                return null;
            case BsonType.Binary:
                var value = securityService.Decrypt(context.Reader.ReadBinaryData());
                return value;
            default:
                throw new NotSupportedException($"Cannot convert a {type} to a {nameof(String)}");
        }
    }

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, string? value)
    {
        if (value is null)
        {
            context.Writer.WriteNull();
            return;
        }

        var encrypted = securityService.Encrypt(value);
        context.Writer.WriteBinaryData(encrypted);
    }
}
