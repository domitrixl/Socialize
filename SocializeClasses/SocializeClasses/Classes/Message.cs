using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using static System.Net.Mime.MediaTypeNames;

namespace Socialize.Classes
{
    public class Message
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; } = null!;

        [BsonElement("userId")]
        public string Userid { get; set; } = null!;

        [BsonElement("text")]
        public string Text { get; set; } = null!;

        [BsonElement("toUser")]
        public string ToUser { get; set; } = null!;

        [BsonElement("datatype")]
        public Datatype Datatype { get; set; }

        [BsonElement("image")]
        public string ImagePath { get; set; } = null!;

        [BsonElement("datetime")]
        public DateTime DateTime { get; set; }
    }

    public enum Datatype
    {
        Text,
        Image,
        Video
    }
}
