using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace apiPBR.Models.Response.Expedientes
{
    public class ResponseEvidenciaFotografica:BasicResponse
    {
        public List<REvidenciaFotografica> listResponseEvidenciaFotografica { get; set; }
    }
    public class REvidenciaFotografica
    {
        public string id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string url { get; set; }
        public string name { get; set; }
    }

}