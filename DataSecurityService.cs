using System.Security.Cryptography;
using System.Text;
using MongoDB.Bson;

namespace MongoDbEncryptionAesGcm;

public class DataSecurityService : IDataSecurityService
{
    private readonly DataSecuritySettings _settings;

    public DataSecurityService(DataSecuritySettings settings)
    {
        _settings = settings;
    }

    public BsonBinaryData Encrypt(string plainText)
    {
        // Here we are getting the key from the DataSecuritySettings file.
        //
        // Instead of getting the key from the configuration file, you can
        // generate a new 16-byte cryptographically secure key with the
        // RandomNumberGenerator.Fill(key) method;
        //
        // ex:
        // var key = new byte[16];
        // RandomNumberGenerator.Fill(key)
        //
        var key = Convert.FromBase64String(_settings.EncryptionKey!);
        var iv = new byte[12];
        var tag = new byte[16];

        RandomNumberGenerator.Fill(iv);

        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var cipher = new byte[plainBytes.Length];

        using var aesGcm = new AesGcm(key);
        aesGcm.Encrypt(iv, plainBytes, cipher, tag);

        var buffer = new byte[iv.Length + tag.Length + cipher.Length];
        Buffer.BlockCopy(iv, 0, buffer, 0, iv.Length);
        Buffer.BlockCopy(tag, 0, buffer, iv.Length, tag.Length);
        Buffer.BlockCopy(cipher, 0, buffer, iv.Length + tag.Length, cipher.Length);

        return new BsonBinaryData(buffer, BsonBinarySubType.Encrypted);
    }

    public string Decrypt(BsonBinaryData encryptedData)
    {
        var iv = encryptedData.Bytes[..12];
        var tag = encryptedData.Bytes[12..28];
        var cipher = encryptedData.Bytes[28..];

        var decryptedBytes = new byte[cipher.Length];

        // Here you must pass the same key generated previously
        using var aesGcm = new AesGcm(Convert.FromBase64String(_settings.EncryptionKey!));
        aesGcm.Decrypt(iv, cipher, tag, decryptedBytes);

        return Encoding.UTF8.GetString(decryptedBytes);
    }
}
