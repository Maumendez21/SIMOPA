using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace apiPBR.Models.Request.Auditors
{
    public class ModObservacionDocumento
    {        
        public string id { get; set; }
        public string idExpediente { get; set; }
        public string TipoExpediente { get; set; }
        public int Clave { get; set; }
        public string Observacion { get; set; }
        public string Recomendacion { get; set; }
        public int Estatus { get; set; }
    }

    public class ObservacionGeneral
    {
        public string observacionGeneral { get; set; }
    }
}