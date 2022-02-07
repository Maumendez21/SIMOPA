using apiPBR.Models.Response.UserPBR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace apiPBR.Models.Response.Credentials
{
    public class UsersResponse : BasicResponse
    {
        public List<UserActivation> ListUsers { get; set; }

        public UsersResponse()
        {
            ListUsers = new List<UserActivation>();
        }
    }
}