using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace credentialsPBR.Models.Expedientes.Utilerias
{
    public class Observaciones
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("idExpediente")]
        public string idExpediente { get; set; }
        [BsonElement("tipoExpediente")]
        public string tipoExpediente { get; set; }
        [BsonElement("clave")]
        public int Clave { get; set; }
        [BsonElement("observacion")]
        public string Observacion { get; set; }
        [BsonElement("recomendacion")]
        public string Recomendacion { get; set; }
        [BsonElement("estatus")]
        public int Estatus { get; set; }        
    }
    public class ObservacionesSalida
    {
        public int contador { get; set; }
        public string Id { get; set; }
        public string idExpediente { get; set; }
        public string tipoExpediente { get; set; }
        public int Clave { get; set; }
        public string Observacion { get; set; }
        public string Recomendacion { get; set; }
        public int Estatus { get; set; }
    }
}