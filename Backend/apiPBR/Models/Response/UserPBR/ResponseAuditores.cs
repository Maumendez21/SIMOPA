using credentialsPBR.Models.Expedientes.Utilerias;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace apiPBR.Models.Response.UserPBR
{
    public class ResponseAuditores : BasicResponse
    {
        public List<Auditores> Auditores { get; set; }
    }
}