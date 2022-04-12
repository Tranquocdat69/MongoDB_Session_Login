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
        public string ALoginName { get; set; }
        public string AToken { get; set; }
        public string AIpServer { get; set; }
        public string AIpClient { get; set; }
        public string AUserAgent { get; set; }
        public string ABrowser { get; set; }
        public string ALoginTime { get; set; }
        public string ALogoutTime { get; set; }
        public string ASessionNo { get; set; }
        public int ASessionFirstLogin { get; set; }
        public string AIsMobile { get; set; }
        public string ABrowserName { get; set; }
        public string ABrowserVers { get; set; }
        public int ACHKPass2 { get; set; }
    }
}
