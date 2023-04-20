using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialize.Classes
{
    public class SocialPost
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("postedBy")]
        public string PostedBy { get; set; } = null!;

        [BsonElement("media")]
        public string Media { get; set; } = null!;

        [BsonElement("description")]
        public string? Description { get; set; }

        [BsonElement("kindOfPost")]
        public KindOfPost KindOfPost { get; set; }

        [BsonElement("dateTime")]
        public DateTime DateTime { get; set; }
    }

    public enum KindOfPost
    {
        ProfilePicAndText,
        CustomPic,
        CustomVideo,
        Workout,
        WeightProgression
    }
}
