using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace credentialsPBR.Models.Settings
{
    public class Accesos
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("iduser")]
        public string IdUser { get; set; }
        [BsonElement("clave")]
        public string Clave { get; set; }
    }
}