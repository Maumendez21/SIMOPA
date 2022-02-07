using credentialsPBR.Models.Expedientes.ObraPublica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace apiPBR.Models.Response.Expedientes.ObraPublica
{
    public class ResponseVisitaObra:BasicResponse
    {
        public HeaderObraPublica HeaderObraPublica { get; set; }
        public List<VisitaObra> ListaVisitaObra { get; set; }
    }
}