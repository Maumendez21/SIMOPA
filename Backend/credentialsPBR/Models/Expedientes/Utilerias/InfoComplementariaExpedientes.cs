using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace credentialsPBR.Models.Expedientes.Utilerias
{
    public class InfoComplementariaExpedientes
    {
        public string latitud { get; set; }
        public string longitud { get; set; }
        public string estatusExpediente { get; set; }
        public string avanceDocumental { get; set; }
        public string avanceFinanciero { get; set; }
        public string avanceFisico { get; set; }
    }
}