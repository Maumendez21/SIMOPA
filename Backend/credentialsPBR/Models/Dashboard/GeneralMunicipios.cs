using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace credentialsPBR.Models.Dashboard
{
    public class GeneralMunicipios
    {
            [BsonId]
            [BsonRepresentation(BsonType.ObjectId)]
            public string Id { get; set; }
            [BsonElement("municipio")]
            public string municipio { get; set; }
            [BsonElement("ejercicio")]
            public string ejercicio { get; set; }
            [BsonElement("presupuestoAutorizadoAdquisiciones")]
            public double presupuestoAutorizadoAdquisiciones { get; set; }
            [BsonElement("presupuestoAutorizadoObras")]
            public double presupuestoAutorizadoObras { get; set; }

    }
}