using apiPBR.Models.Response.UserPBR;
using credentialsPBR.Models.Expedientes.Utilerias;
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

namespace apiPBR.Controllers.Auditor
{
    public class AuditoresController : ApiController
    {
        GetUserInfo getUserInfo = new GetUserInfo();

        [Authorize(Roles = "Auditor, Coordinador, Ejecutivo")]
        [HttpGet]
        [Route("api/auditores")]
        public async System.Threading.Tasks.Task<IHttpActionResult> ListaAuditores()
        {
            var idUsername = string.Empty;
            var role = string.Empty;
            var municipio = string.Empty;
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
                if (c.Type == "municipio")
                {
                    municipio = c.Value;
                }
            }


            string constr = ConfigurationManager.AppSettings["connectionString"];
            ResponseAuditores auditores = new ResponseAuditores
            {
                Auditores = new List<Auditores>()
            };
            

            try
            {

                MongoClient Client = new MongoClient(constr);

                UsersPRB userPBR = new UsersPRB();
                userPBR = await getUserInfo.GetInfoUserPBRAsync(idUsername, Client);

                var DB = Client.GetDatabase("PRB");
                var collection = DB.GetCollection<UsersPRB>("UsersPBR");


                var filter = Builders<UsersPRB>.Filter.Eq("municipio", municipio) &
                    Builders<UsersPRB>.Filter.Eq(x => x.active, true); 

                var result = await collection.Find(filter).ToListAsync();

                if (result != null)
                {
                    foreach (var r in result)
                    {
                        Auditores auditores1 = new Auditores();

                        auditores1.id = r.Id;
                        auditores1.nombre = r.name;
                        auditores1.username = r.username;
                        auditores1.municipio = r.municipio;

                        auditores.Auditores.Add(auditores1);
                    }
                    
                }

                Auditores auditores2 = new Auditores();

                auditores2.id = "auditorestodosdq37iq84burwwjdbsd3";
                auditores2.nombre = "Todos";
                auditores2.username = "Todos";
                auditores2.municipio = municipio;

                auditores.Auditores.Add(auditores2);

                auditores.success = true;
                auditores.messages.Add("Respuesta con exito");

                return Ok(auditores);

            }
            catch (Exception ex)
            {

                auditores.success = false;
                auditores.messages.Add(ex.ToString());
                return Ok(auditores);
            }

        }
    }
}
