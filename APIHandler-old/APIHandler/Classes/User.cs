using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Socialize.Classes
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("username")]
        public string username { get; set; } = null!;

        [BsonElement("password")]
        public string? password { get; set; }


        [BsonElement("salt")]
        public string? salt { get; set; }
    }
}