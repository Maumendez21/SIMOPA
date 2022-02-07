using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace apiPBR.Models
{
    public abstract class BasicResponse
    {
        public bool success { get; set; }
        public List<string> messages { get; set; }

        public BasicResponse()
        {
            this.success = false;
            this.messages = new List<string>();
        }
    }
}