using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace credentialsPBR.Models.Expedientes.Utilerias
{
    public class Fundamentos
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("estado")]
        public string Estado { get; set; }
        [BsonElement("municipio")]
        public string Municipio { get; set; }
        [BsonElement("procedimiento")]
        public string Procedimiento { get; set; }
        [BsonElement("clave")]
        public int Clave { get; set; }
        [BsonElement("fundamento")]
        public string Fundamento { get; set; }
        [BsonElement("documento")]
        public string Documento { get; set; }
        [BsonElement("version")]
        public string Version { get; set; }
    }
}