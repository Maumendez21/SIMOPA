using credentialsPBR.Models.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace apiPBR.Models.Response.Dashboard
{
    public class ResponseDashboardMain : BasicResponse
    {
        public Principal Principal { get; set; }
     }
}