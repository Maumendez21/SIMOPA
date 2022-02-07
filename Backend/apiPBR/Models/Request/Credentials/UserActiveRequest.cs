using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace apiPBR.Models.Request.Credentials
{
    public class UserActiveRequest
    {
        public string Id { get; set; }
        public bool Active { get; set; }
    }
}