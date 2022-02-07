using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace apiPBR.Controllers
{
    public class VersionConsolaSIMOPAController : ApiController
    {
        [HttpGet]
        [Route("api/simopa/version")]
        public async System.Threading.Tasks.Task<IHttpActionResult> ValidaVersion(string version)
        {
            if (version == "2.1.16")
                return Ok(true);
            return Ok(false);
        }
        
    }
}
