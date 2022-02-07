using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace apiPBR.Models.Response.UserPBR
{
    public class UserInformation : BasicResponse
    {
        public string Id { get; set; }
        public string username { get; set; }
        public string name { get; set; }
        public string role { get; set; }
        public string municipio { get; set; }
        public string email { get; set; }
        public DateTime date { get; set; }
        public bool active { get; set; }
    }
}