using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace credentialsPBR.Models.Expedientes.BitacoraAsignacion
{
    public class BitacoraAsignacion
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("idpropietario")]
        public string IdPropietario { get; set; }
        [BsonElement("idasignado")]
        public string IdAsignado { get; set; }
        [BsonElement("fecha")]
        public DateTime Fecha { get; set; }
    }
}