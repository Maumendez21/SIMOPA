using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace apiPBR.Models.Request.Auditors
{
    public class RequestEvidenciaFotografica
    {
        public string idExpediente { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string url { get; set; }
        
    }
}