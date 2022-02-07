using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace credentialsPBR.Models.Settings
{
    public class Funcionalidades
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("clave")]
        public string clave { get; set; }
        [BsonElement("Descripcion")]
        public string descripcion { get; set; }
        
    }
}