namespace MongoDBAPI
{
    public class Database
    {
        public string ConnectionString { get; set; } = null!;

        public string DatabaseName { get; set; } = null!;

        public string[] CollectionNames { get; set; } = null!;

        public string JWTKey { get; set; } = null!;
    }
}
