using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace apiPBR.Models.Response.Expedientes.ObraPublica
{
    public class CoordenadasObraPublica:BasicResponse
    {
        public List<CoordenadaObraOublica> listaCoordenadasObraOublicas = new List<CoordenadaObraOublica>();
    }
    public class CoordenadaObraOublica
    {
        public string id { get; set; }
        public string latitud { get; set; }
        public string longitud { get; set; }
    }
}