using apiPBR.Models.Request.Auditors;
using apiPBR.Models.Response;
using credentialsPBR.Models.Expedientes.ObraPublica;
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

namespace apiPBR.Controllers.Auditor.ObraPublica.V1
{
    public class ModObservacionObraPublicaV1Controller : ApiController
    {
        GetUserInfo getUserInfo = new GetUserInfo();

        [Authorize(Roles = "Auditor, Coordinador, Ejecutivo")]
        [HttpPut]
        [Route("api/expedients/obrapublica/document/observacion")]
        public async System.Threading.Tasks.Task<IHttpActionResult> ModExpedientDocumentAsync([FromBody]ModObservacionDocumento modObservacionDocumento)
        {
            var idUsername = string.Empty;
            var identity = (ClaimsIdentity)User.Identity;
            IEnumerable<Claim> claims = identity.Claims;

            foreach (var c in claims)
            {
                if (c.Type == "id")
                {
                    idUsername = c.Value;
                }
            }

            string constr = ConfigurationManager.AppSettings["connectionString"];
            
            GenericClass genericClass = new GenericClass();

            try
            {
                MongoClient Client = new MongoClient(constr);

                UsersPRB userPBR = new UsersPRB();
                userPBR = await getUserInfo.GetInfoUserPBRAsync(idUsername, Client);

                var DB = Client.GetDatabase("PRB");
                var collection = DB.GetCollection<ObraPublicaV1>("ObraPublica");

                var filter = Builders<ObraPublicaV1>.Filter.Eq(x => x.Id, modObservacionDocumento.id);

                var result = await collection.Find(filter).FirstOrDefaultAsync();

                if (result != null)
                {
                    foreach (var d in result.documentos)
                    {
                        if (d.clave == modObservacionDocumento.Clave)
                        {
                            d.observaciones = modObservacionDocumento.Observacion;
                            d.recomendacion = modObservacionDocumento.Recomendacion;
                        }
                    }

                    await collection.ReplaceOneAsync(filter, result);
                }

                genericClass.success = true;
                genericClass.messages.Add("Actualización exitosa");
            }
            catch (Exception ex)
            {
                genericClass.success = false;
                genericClass.messages.Add("Error inesperado");
            }

            return Ok(genericClass);
        }
    }
}
