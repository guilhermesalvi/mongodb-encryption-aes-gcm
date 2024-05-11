using MongoDB.Bson.Serialization.Conventions;

public static class ConventionsRegister
{
    public static void RegisterConventions(DataSecurityService securityService)
    {
        var pack = new ConventionPack
        {
            new ProtectedPersonalDataConvention(securityService)
        };

        ConventionRegistry.Register(
            "Global Conventions",
            pack,
            filter => filter.Namespace!.StartsWith("YourNamespace"));
    }
}
