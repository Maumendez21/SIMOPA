using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace credentialsPBR.Models.Expedientes.Utilerias
{
    public class ExpedienteInformacion
    {
        public string expediente { get; set; }
        public string idExpediente { get; set; }
        public string ejercicio { get; set; }
        public string tipoExpediente { get; set; }
        public string estado { get; set; }
        public string municipio { get; set; }
    }
}