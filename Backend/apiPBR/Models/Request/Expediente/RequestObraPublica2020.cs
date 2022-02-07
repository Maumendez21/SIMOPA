using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace apiPBR.Models.Request.Expediente
{
    public class RequestObraPublica2020
    {
        public string Ejercicio { get; set; }
        public string TipoAdjudicacion { get; set; }
        public string NumeroProcedimiento { get; set; }
        public string NumeroContrato { get; set; }
        public string NumeroObra { get; set; }
        public string Procedimiento { get; set; }
        public string NombreObra { get; set; }
        public string Proveedor { get; set; }
        public string Programa { get; set; }
        public string Ejecutor { get; set; }
        public string Localidad { get; set; }
        public string Proyecto { get; set; }
        public string Responsable { get; set; }
        public string Modalidad { get; set; }
        public string tipoContrato { get; set; }
        public string tipoProveedor { get; set; }
        public DateTime? FechaProcedimiento { get; set; }
        public DateTime? FechaContrato { get; set; }
        public double? MontoAsignado { get; set; }
        public double MontoContrato { get; set; }
        public double? MontoEjercido { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
    }
}