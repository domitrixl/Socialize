using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/* Unmerged change from project 'Fithub (net7.0-android)'
Before:
using System.Threading.Tasks;
After:
using System.Threading.Tasks;
using Fithub.Classes;
using Fithub;
*/

/* Unmerged change from project 'Fithub (net7.0-windows10.0.19041.0)'
Before:
using System.Threading.Tasks;
After:
using System.Threading.Tasks;
using Fithub.Classes;
using Fithub;
*/
using System.Threading.Tasks;

namespace Fithub.Classes
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
