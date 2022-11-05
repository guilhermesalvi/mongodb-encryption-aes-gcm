# mongodb-encryption-aes-gcm

An example of how you can encrypt data to be saved in MongoDb with C#.
We are using Advanced Encryption Standard algorithm (AES) with Galois Counter Mode (GCM).

You can register the serializer using mongodb's mapping pattern:

```cs

BsonClassMap.RegisterClassMap<MyClass>(cm =>
{
    // Here injecting DataSecurityService
    cm.MapMember(x => x.MyProp).SetSerializer(new EncryptedStringSerializer(securityService));
});

```
