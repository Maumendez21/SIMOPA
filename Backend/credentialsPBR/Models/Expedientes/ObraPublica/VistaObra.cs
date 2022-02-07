using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace credentialsPBR.Models.Expedientes.ObraPublica
{
    public class VisitaObra
    {
        public string Id { get; set; }
        public string FechaVisita { get; set; }
        public string SituacionActual { get; set; }
        public string Problematica { get; set; }
        public int AvanceFisico { get; set; }
        public int AvanceFinanciero { get; set; }
    }
}