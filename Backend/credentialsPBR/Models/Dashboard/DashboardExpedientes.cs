using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace credentialsPBR.Models.Dashboard
{
    public class DashboardExpedientes
    {
        public int TotalExpedientes { get; set; }
        public int ExpedientesConObservaciones { get; set; }
        public int ExpedientesSinRevisar { get; set; }
        public int UniversoExpedientes { get; set; }
        public int porcentajeGlobal => TotalExpedientes == 0 ? 0:(int)((TotalExpedientes * 100) / UniversoExpedientes);
        public int Cargados { get; set; }
        public int EnRevision { get; set; }
        public int RevisadoConObservaciones { get; set; }
        public int RevisadaSinObservaciones { get; set; }
        public int RevisadoCorregido { get; set; }
        public int Solventacion { get; set; }
        public int EnProceso { get; set; }
        public int AltaSistema { get; set; }
        public double Porcentaje { get; set; }
        public List<ListaExpedientesEnRevision> ListaExpedientesEnRevision {get; set;}
    }
    public class ListaExpedientesEnRevision
    {
        public string idExpediente { get; set; }
        public string NombreExpediente { get; set; }
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
        public int avanceDocumental { get; set; }

        
    }
}