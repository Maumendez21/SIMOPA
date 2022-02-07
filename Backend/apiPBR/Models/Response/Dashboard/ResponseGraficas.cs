using credentialsPBR.Models.Expedientes.Utilerias;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace apiPBR.Models.Response.Dashboard
{
    public class ResponseGraficas : BasicResponse
    {
        public Graficas graficas { get; set; }
    }
}