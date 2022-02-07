using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace credentialsPBR.Models.Expedientes.Utilerias
{
    public class EvidenciaFotografica
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("idExpediente")]
        public string idExpediente { get; set; }
        [BsonElement("ejercicio")]
        public string ejercicio { get; set; }
        [BsonElement("municipio")]
        public string municipio { get; set; }
        [BsonElement("nombre")]
        public string nombre { get; set; }
        [BsonElement("titulo")]
        public string titulo { get; set; }
        [BsonElement("descripcion")]
        public string descripcion { get; set; }
        [BsonElement("fecha")]
        public DateTime fecha { get; set; }
        [BsonElement("idUser")]
        public string idUser { get; set; }
        [BsonElement("path")]
        public string path { get; set; }
    }
}