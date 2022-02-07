using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace apiPBR.Models.Response.Expedientes.Adquisiciones
{
    public class DocumentosSanAndresCholula : BasicResponse
    {

        public string entidad { get; set; }
        public string proveedorAdjudicado { get; set; }
        public string origenRecurso { get; set; }
        public string tipoAdjudicacion { get; set; }
        public string numeroAdjudicacion { get; set; }
        public string numeroContrato { get; set; }
        public string objetoContrato { get; set; }
        public string montoAdjudicado { get; set; }
        public int status { get; set; }
        public string ObservacionesGenerales { get; set; }
        public string estatusExpediente { get; set; }
        public string NombreAuditor { get; set; }
        public List<DocumentosAdquisicionesSanAndresCholula> documentos { get; set; }
    }
    public class DocumentosAdquisicionesSanAndresCholula
    {
        public int clave { get; set; }
        public string grupo { get; set; }
        public string documento { get; set; }
        public string estatus { get; set; }
        public string referencia { get; set; }
        public string comentario { get; set; }
        public string recomendacion { get; set; }
        public string downloadLinkDocument { get; set; }
        public string fundamento { get; set; }
    }
}
