using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fithub.Classes
{
    public class SocialManagement
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("userId")]
        public string UserId { get; set; }

        [BsonElement("username")]
        public string Username { get; set; }

        [BsonElement("profilePic")]
        public string ProfilePic { get; set; }

        [BsonElement("friends")]
        public List<string> Friends { get; set; }

        [BsonElement("fRequestsSent")]
        public List<string> FRequestsSent { get; set; }

        [BsonElement("fRequestsRecieved")]
        public List<string> FRequestsRecieved { get; set; }
    }
}
