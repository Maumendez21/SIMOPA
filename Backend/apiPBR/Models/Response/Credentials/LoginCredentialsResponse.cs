using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Web;

namespace apiPBR.Models.Response.Credentials
{
    public class LoginCredentialsResponse : BasicResponse
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string refresh_token { get; set; }
        public string name { get; set; }
        public List<string> listAcces { get; set; }

        public LoginCredentialsResponse()
        {
            this.access_token = "";
            this.expires_in = 0;
            this.refresh_token = "";
            this.listAcces = new List<string>();
        }
    }
}