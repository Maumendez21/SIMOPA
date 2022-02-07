using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace apiPBR.Models.Request.Expediente
{
    public class RequestAdquisiciones2020
    {
        public string Ejercicio { get; set; }
        public string TipoAdjudicacion { get; set; }
        public string NumeroAdjudicacion { get; set; }
        public string NumeroContrato { get; set; }
        public string Entidad { get; set; }
        public string Objeto { get; set; }
        public string Proveedor { get; set; }
        public double MontoAdjudicacion { get; set; }
        public string OrigenRecurso { get; set; }
        public string ObservacionesGenerales { get; set; }
    }
}