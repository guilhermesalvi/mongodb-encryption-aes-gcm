using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;

public class ProtectedPersonalDataConvention(DataSecurityService service)
    : ConventionBase, IMemberMapConvention
{
    public void Apply(BsonMemberMap memberMap)
    {
        if (memberMap.MemberType == typeof(string) &&
            Attribute.IsDefined(memberMap.MemberInfo, typeof(ProtectedPersonalDataAttribute)))
        {
            memberMap.SetSerializer(new EncryptedStringSerializer(service));
        }
    }
}
