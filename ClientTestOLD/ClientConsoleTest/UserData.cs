using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ClientConsoleTest
{
    internal class UserData
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("username")]
        public string username { get; set; }

        [BsonElement("password")]
        public string password { get; set; }
    }
}