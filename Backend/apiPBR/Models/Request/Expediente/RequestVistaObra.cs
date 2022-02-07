using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace apiPBR.Models.Request.Expediente
{
    public class RequestVistaObra
    {
        public string Id { get; set; }
        public DateTime FechaVisita { get; set; }
        public string SituacionActual { get; set; }
        public string Problematica { get; set; }
        public int AvanceFisico { get; set; }
        public int AvanceFinanciero { get; set; }
    }
}