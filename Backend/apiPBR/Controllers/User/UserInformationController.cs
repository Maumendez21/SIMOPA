using apiPBR.Models.Response.UserPBR;
using credentialsPBR.Models.Users;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace apiPBR.Controllers.User
{
    public class UserInformationController : ApiController
    {
        GetUserInfo getUserInfo = new GetUserInfo();

        [Authorize(Roles = "Auditor, Coordinador, Ejecutivo")]
        [HttpGet]
        [Route("api/information/userpbr")]
        public async System.Threading.Tasks.Task<IHttpActionResult> UserPBRInformationsAsync()
        {
            var idUsername = string.Empty;
            var role = string.Empty;
            var identity = (ClaimsIdentity)User.Identity;
            IEnumerable<Claim> claims = identity.Claims;

            var roleClaimType = identity.RoleClaimType;
            var roles = claims.Where(c => c.Type == ClaimTypes.Role).ToList();

            foreach (var r in roles)
            {
                role = r.Value;
            }

            foreach (var c in claims)
            {
                if (c.Type == "id")
                {
                    idUsername = c.Value;
                }
            }
            UserInformation userInformation = new UserInformation();
            string constr = ConfigurationManager.AppSettings["connectionString"];

            try
            {
                MongoClient Client = new MongoClient(constr);

                UsersPRB userPBR = new UsersPRB();
                userPBR = await getUserInfo.GetInfoUserPBRAsync(idUsername, Client);

                

                userInformation.active = userPBR.active;
                userInformation.municipio = userPBR.municipio;
                userInformation.email = userPBR.email;
                userInformation.date = userPBR.date;
                userInformation.role = userPBR.role;
                userInformation.success = true;
                userInformation.name = userPBR.name;
                userInformation.username = userPBR.username;
                userInformation.messages.Add("Exitoso");

                return Ok(userInformation);
            }
            catch (Exception ex)
            {
                userInformation.success = false;
                userInformation.messages.Add(ex.ToString());
                return Ok(userInformation);
            }

        }


    }
}
