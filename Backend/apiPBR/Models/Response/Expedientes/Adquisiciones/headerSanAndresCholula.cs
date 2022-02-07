using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace apiPBR.Models.Response.Expedientes.Adquisiciones
{

    public class HeadersSanAndresCholulaLista : BasicResponse
    {
        public List<headerSanAndresCholula> ListaHeadersSanAndresCholula { get; set; }
    }
    public class headerSanAndresCholula
    {
        public string Id { get; set; }        
        public string tipoAdjudicacion { get; set; }
        public string numeroAdjudicacion { get; set; }
        public string numeroContrato { get; set; }
        public string objetoContrato { get; set; }
        public string montoAdjudicado { get; set; }   
        public int status { get; set; }
        public int porcentajeAvance { get; set; }
        public string ObservacionesGenerales { get; set; }
        public string estatusExpediente { get; set; }
        public string proveedor { get; set; }
    }

    


}