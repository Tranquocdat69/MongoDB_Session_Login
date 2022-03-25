using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace MongoDB_Session_Login.Models.Session
{
    public class SessionLogin
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string LoginName { get; set; }
        public string TokenSession { get; set; }
        public string IPAddress { get; set; }
    }
}
