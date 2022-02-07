using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace credentialsPBR.Models.Expedientes.ObraPublica
{
    public class VisitaObraHeader
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("idExpediente")]
        public string ExpedienteId { get; set; }
        [BsonElement("fechaVisita")]
        public DateTime FechaVisita { get; set; }
        [BsonElement("situacionActual")]
        public string SitutacionActual { get; set; }
        [BsonElement("problematica")]
        public string Problematica { get; set; }
        [BsonElement("avanceFisico")]
        public int AvanceFisico { get; set; }
        [BsonElement("avanceFinanciero")]
        public int AvaceFinanciero { get; set; }

    }
    public class VisitaObraImagenes
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("idVisita")]
        public string VisitaId { get; set; }
        [BsonElement("idExpediente")]
        public string ExpedienteId { get; set; }
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