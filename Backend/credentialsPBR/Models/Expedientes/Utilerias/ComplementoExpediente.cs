using credentialsPBR.Models.Expedientes.ObraPublica;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace credentialsPBR.Models.Expedientes.Utilerias
{
    public class ComplementoExpediente
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("idExpediente")]
        public string idExpediente { get; set; }
        [BsonElement("tipoExpediente")]
        public string tipoExpediente { get; set; }
        [BsonElement("estadoExpediente")]
        public string estatus { get; set; }
        [BsonElement("porcentajeAvance")]
        public int porcentajeAvance { get; set; }
        [BsonElement("location")]
        public Coordenate Location { get; set; }
    }

    
}