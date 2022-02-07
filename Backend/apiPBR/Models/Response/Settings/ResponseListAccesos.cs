using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace apiPBR.Models.Response.Settings
{
    public class ResponseListAccesos : BasicResponse
    {
        public List<string> ListaAccesos { get; set; }
    }
}