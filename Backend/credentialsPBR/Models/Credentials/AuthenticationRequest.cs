using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace credentialsPBR.Models.Credentials
{
    public class AuthenticationRequest
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }
}