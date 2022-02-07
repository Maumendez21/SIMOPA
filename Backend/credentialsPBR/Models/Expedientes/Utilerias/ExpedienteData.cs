using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace credentialsPBR.Models.Expedientes.Utilerias
{
    public class ExpedienteData
    {
        public string idExpediente { get; set; }
        public string estatus { get; set; }
        public string NombreExpediente { get; set; }
        public int AvanceDocumental { get; set; }
        public double MontoContratado { get; set; }
        public double MontoPagado { get; set; }
        public double MontoAsignado { get; set; }
        public double Porcentaje { get; set; }

        public string NombreExpedienteCorto
        {
            get
            {
                if (string.IsNullOrWhiteSpace(NombreExpediente))
                {
                    return null;
                }
                if (NombreExpediente.Length > 30)
                {
                    return NombreExpediente.Substring(0, 30) + "...";
                }
                else
                {
                    return NombreExpediente;
                }
            }
        }
    }
}