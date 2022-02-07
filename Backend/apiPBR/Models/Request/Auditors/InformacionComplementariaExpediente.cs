using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace apiPBR.Models.Request.Auditors
{
    public class InformacionComplementariaExpediente
    {
        public string estatusExpediente { get; set; }
        public string avanceFinanciero { get; set; }
        public string avanceDocumental { get; set; }
        public string avanceFisico { get; set; } 
        public string latitud { get; set; }
        public string longitud { get; set; }
    }

    public class EstatusDocumentos
    {
        public string estatusDocumento { get; set; }
    }
}