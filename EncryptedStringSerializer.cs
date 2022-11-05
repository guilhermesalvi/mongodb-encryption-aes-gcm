using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace MongoDbEncryptionAesGcm;

public class EncryptedStringSerializer : SerializerBase<string?>
{
    private readonly IDataSecurityService _securityService;

    public EncryptedStringSerializer(IDataSecurityService securityService)
    {
        _securityService = securityService;
    }

    public override string? Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var type = context.Reader.GetCurrentBsonType();

        switch (type)
        {
            case BsonType.Null:
                context.Reader.ReadNull();
                return null;
            case BsonType.Binary:
                var value = _securityService.Decrypt(context.Reader.ReadBinaryData());
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

        var encrypted = _securityService.Encrypt(value);
        context.Writer.WriteBinaryData(encrypted);
    }
}
