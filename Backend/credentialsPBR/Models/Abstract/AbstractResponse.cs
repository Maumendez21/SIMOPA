using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace credentialsPBR.Models.Abstract
{
    public class AbstractResponse
    {
        public bool Success { get; set; }
        public List<string> Messages { get; set; }

        public AbstractResponse()
        {
            Messages = new List<string>();
            Success = false;
        }
    }
}