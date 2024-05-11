# mongodb-encryption-aes-gcm

An example of how you can encrypt data to be saved in MongoDb with C#.
We are using Advanced Encryption Standard algorithm (AES) with Galois Counter Mode (GCM).

With this approach you can annotate any property of string type with ```ProtectedPersonalData``` attribute:

```cs
[ProtectedPersonalData]
public string CustomerName { get; set; }
``` 

Then register ```ProtectedPersonalDataConvention``` in startup with an extension method:

```cs
public static void AddMongoDb(this IServiceCollection services)
{
    //... other services

    services.AddSingleton<DataSecurityService>();
    var securityService = services.BuildServiceProvider().GetRequiredService<DataSecurityService>();
        ConventionsRegister.RegisterConventions(securityService);
}
```