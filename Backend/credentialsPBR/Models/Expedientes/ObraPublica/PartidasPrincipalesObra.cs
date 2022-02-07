using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace credentialsPBR.Models.Expedientes.ObraPublica
{
    public class PartidasPrincipalesObra
    {
            [BsonId]
            [BsonRepresentation(BsonType.ObjectId)]
            public string Id { get; set; }
            [BsonElement("idExpediente")]
            public string ExpedienteId { get; set; }
            [BsonElement("nombre")]
            public string Nombre { get; set; }
            [BsonElement("porcentajeCompleto")]
            public string PorcentajeCompleto { get; set; }
            [BsonElement("estatus")]
            public bool Estatus { get; set; }
    }
}