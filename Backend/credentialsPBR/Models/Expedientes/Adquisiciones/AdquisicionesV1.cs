using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace credentialsPBR.Models.Expedientes.Adquisiciones
{
    public class AdquisicionesV1
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("estado")]
        public string estado { get; set; }
        [BsonElement("municipio")]
        public string municipio { get; set; }
        [BsonElement("expediente")]
        public string expediente { get; set; }
        [BsonElement("tipoAdjudicacion")]
        public string tipoAdjudicacion { get; set; }
        [BsonElement("numeroAdjudicacion")]
        public string numeroAdjudicacion { get; set; }
        [BsonElement("numeroContrato")]
        public string numeroContrato { get; set; }
        [BsonElement("ejercicio")]
        public string ejercicio { get; set; }
        [BsonElement("entidad")]
        public string entidad { get; set; }
        [BsonElement("objetoContrato")]
        public string objetoContrato { get; set; }
        [BsonElement("proveedorAdjudicado")]
        public string proveedorAdjudicado { get; set; }
        [BsonElement("montoAdjudicado")]
        public string montoAdjudicado { get; set; }
        [BsonElement("origenRecurso")]
        public string origenRecurso { get; set; }
        [BsonElement("fechaRevision")]
        public string fechaRevision { get; set; }
        [BsonElement("auditor")]
        public string auditor { get; set; }
        [BsonElement("observacionesGenerales")]
        public string observacionesGenerales { get; set; }
        [BsonElement("estatusExpediente")]
        public string estatusExpediente { get; set; }
        [BsonElement("documentos")]
        public List<DocumentoAdquisicionesV1> documentos { get; set; }
    }

    public class DocumentoAdquisicionesV1
    {
        [BsonElement("clave")]
        public int clave { get; set; }
        [BsonElement("grupo")]
        public string grupo { get; set; }
        [BsonElement("documento")]
        public string documento { get; set; }
        [BsonElement("estatus")]
        public string estatus { get; set; }
        [BsonElement("referencia")]
        public string referencia { get; set; }
        [BsonElement("comentario")]
        public string comentario { get; set; }
        [BsonElement("recomendacion")]
        public string recomendacion { get; set; }
    }

}