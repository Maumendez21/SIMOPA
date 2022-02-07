using credentialsPBR.Models.Expedientes.Utilerias;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace credentialsPBR.Models.Expedientes.ObraPublica
{
    public class ObraPublicaV1
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("procedimiento")]
        public string procedimiento { get; set; }
        [BsonElement("version")]
        public string version { get; set; }
        [BsonElement("estado")]
        public string estado { get; set; }
        [BsonElement("municipio")]
        public string municipio { get; set; }
        [BsonElement("tipoExpediente")]
        public string tipoExpediente { get; set; }
        [BsonElement("programa")]
        public string programa { get; set; }
        [BsonElement("ejercicio")]
        public string ejercicio { get; set; }
        [BsonElement("ejecutor")]
        public string ejecutor { get; set; }
        [BsonElement("fechaRevision")]
        public string fechaRevision { get; set; }
        [BsonElement("localidad")]
        public string localidad { get; set; }
        [BsonElement("nombreObra")]
        public string nombreObra { get; set; }
        [BsonElement("proyecto")]
        public string proyecto { get; set; }
        [BsonElement("numeroObra")]
        public string numeroObra { get; set; }
        [BsonElement("responsable")]
        public string responsable { get; set; }
        [BsonElement("modalidad")]
        public string modalidad { get; set; }
        [BsonElement("tipoAdjudicacion")]
        public string tipoAdjudicacion { get; set; }
        [BsonElement("tipoContrato")]
        public string tipoContrato { get; set; }
        [BsonElement("tipoProveedor")]
        public string tipoProveedor { get; set; }
        [BsonElement("proveedor")]
        public string proveedor { get; set; }
        [BsonElement("numeroProcedimiento")]
        public string numeroProcedimiento { get; set; }
        [BsonElement("fechaProcedimiento")]
        public string fechaProcedimiento { get; set; }
        [BsonElement("numeroContrato")]
        public string numeroContrato { get; set; }
        [BsonElement("fechaContrato")]
        public string fechaContrato { get; set; }
        [BsonElement("montoAsignado")]
        public string montoAsignado { get; set; }
        [BsonElement("montoContrato")]
        public string montoContrato { get; set; }
        [BsonElement("montoEjercido")]
        public string montoEjercido { get; set; }
        [BsonElement("ejecucionInicio")]
        public string ejecucionInicio { get; set; }
        [BsonElement("ejecucionTermino")]
        public string ejecucionTermino { get; set; }
        [BsonElement("ejecucionPeriodo")]
        public string ejecucionPeriodo { get; set; }
        [BsonElement("auditor")]
        public string auditor { get; set; }
        [BsonElement("estatusExpediente")]
        public string estatusExpediente { get; set; }
        [BsonElement("observacionesGenerales")]
        public string ObservacionesGenerales { get; set; }
        [BsonElement("location")]
        public Coordenate Location { get; set; }
        [BsonElement("porcentajeAvanceFisico")]
        public int porcentajeAvanceFisico { get; set; }
        [BsonElement("porcentajeAvanceFinanciero")]
        public int porcentajeAvanceFinanciero { get; set; }
        [BsonElement("documentos")]
        public List<DocumentosObrasV1> documentos { get; set; }
        [BsonElement("pagos")]
        public List<PagosObrasV1> pagos { get; set; }
    }
    public class DocumentosObrasV1
    {
        [BsonElement("clave")]
        public int clave { get; set; }
        [BsonElement("documento")]
        public string documento { get; set; }
        [BsonElement("integracion")]
        public string integracion { get; set; }
        [BsonElement("referencia")]
        public string referencia { get; set; }
        [BsonElement("pagina")]
        public string pagina { get; set; }
        [BsonElement("grupo")]
        public string grupo { get; set; }
        [BsonElement("observaciones")]
        public string observaciones { get; set; }
        [BsonElement("recomendacion")]
        public string recomendacion { get; set; }
    }

    public class PagosObrasV1
    {
        [BsonElement("numeroPago")]
        public int numeroPago { get; set; }
        [BsonElement("periodo")]
        public string periodo { get; set; }
        [BsonElement("factura")]
        public string factura { get; set; }
        [BsonElement("fecha")]
        public string fecha { get; set; }
        [BsonElement("montoPagar")]
        public string montoPagar { get; set; }
        [BsonElement("retencion")]
        public string retencion { get; set; }
        [BsonElement("devolucion")]
        public string devolucion { get; set; }
        [BsonElement("observacionPago")]
        public string observacionPago { get; set; }
        [BsonElement("pagina")]
        public string pagina { get; set; }
    }
    public class Coordenate
    {
        [BsonElement("type")]
        public string Type { get; set; }
        [BsonElement("coordinates")]
        public List<Double> Coordinates { get; set; }
    }
}