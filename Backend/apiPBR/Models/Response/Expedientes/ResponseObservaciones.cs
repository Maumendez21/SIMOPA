using credentialsPBR.Models.Expedientes.Utilerias;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace apiPBR.Models.Response.Expedientes
{
    public class ResponseObservaciones:BasicResponse
    {
        public List<ObservacionesSalida> ListadoObservaciones { get; set; }
    }
}