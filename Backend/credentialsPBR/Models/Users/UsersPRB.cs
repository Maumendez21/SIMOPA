using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace credentialsPBR.Models.Users
{
    public class UsersPRB
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("username")]
        public string username { get; set; }
        [BsonElement("name")]
        public string name { get; set; }
        [BsonElement("password")]
        public string password { get; set; }
        [BsonElement("role")]
        public string role { get; set; }
        [BsonElement("municipio")]
        public string municipio { get; set; }
        [BsonElement("email")]
        public string email { get; set; }
        [BsonElement("date")]
        public DateTime date { get; set; }
        [BsonElement("active")]
        public bool active { get; set; }
    }
}