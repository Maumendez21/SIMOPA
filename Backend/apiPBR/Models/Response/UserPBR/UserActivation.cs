using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace apiPBR.Models.Response.UserPBR
{
    public class UserActivation
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
    }
}