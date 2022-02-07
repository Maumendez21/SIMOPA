using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace credentialsPBR.Models.Expedientes.ObraPublica
{
    public class HeaderObraPublica
    {
        public string Id { get; set; }
        public string procedimiento { get; set; }
        public string programa { get; set; }
        public string ejercicio { get; set; }
        public string ejecutor { get; set; }
        public string fechaRevision { get; set; }
        public string localidad { get; set; }
        public string nombreObra { get; set; }
        public string proyecto { get; set; }
        public string numeroObra { get; set; }
        public string responsable { get; set; }
        public string modalidad { get; set; }
        public string tipoAdjudicacion { get; set; }
        public string tipoContrato { get; set; }
        public string tipoProveedor { get; set; }
        public string proveedor { get; set; }
        public string numeroProcedimiento { get; set; }
        public string fechaProcedimiento { get; set; }
        public string numeroContrato { get; set; }
        public string fechaContrato { get; set; }
        public string montoAsignado { get; set; }
        public string montoContrato { get; set; }
        public string montoEjercido { get; set; }
        public string ejecucionInicio { get; set; }
        public string ejecucionTermino { get; set; }
        public string ejecucionPeriodo { get; set; }
        public string estatusExpediente { get; set; }
        public string latitud { get; set; }
        public string longitud { get; set; }
    }
}