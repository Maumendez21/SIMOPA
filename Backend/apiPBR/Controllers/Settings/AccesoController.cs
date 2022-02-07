using apiPBR.Models.Request.Settings;
using apiPBR.Models.Response;
using apiPBR.Models.Response.Settings;
using credentialsPBR.Models.Settings;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography.Xml;
using System.Web.Http;

namespace apiPBR.Controllers.Settings
{
    public class AccesoController : ApiController
    {
        [Authorize(Roles = "Ejecutivo")]
        [HttpPost]
        [Route("api/settings/acceso")]
        public async System.Threading.Tasks.Task<IHttpActionResult> AgregaAccesosAsync(RequestAcceso request)
        {

            GenericClass genericClass = new GenericClass();

            //var idUsername = string.Empty;
            //var role = string.Empty;
            //var municipio = string.Empty;
            var identity = (ClaimsIdentity)User.Identity;

            try
            {
                string constr = ConfigurationManager.AppSettings["connectionString"];
                MongoClient Client = new MongoClient(constr);
                var DB = Client.GetDatabase("PRB");

                var collection = DB.GetCollection<credentialsPBR.Models.Settings.Accesos>("Accesos");
                credentialsPBR.Models.Settings.Accesos funcion = new credentialsPBR.Models.Settings.Accesos();
                funcion.IdUser = request.IdUser;
                funcion.Clave = request.Clave;

                await collection.InsertOneAsync(funcion);

                genericClass.success = true;
                genericClass.messages.Add("Registro Guardado con exito");
                return Ok(genericClass);
            }
            catch (Exception ex)
            {
                genericClass.success = false;
                genericClass.messages.Add(ex.ToString());
                return Ok(genericClass);
            }

        }



        [Authorize(Roles = "Auditor, Coordinador, Ejecutivo")]
        [HttpGet]
        [Route("api/settings/acceso/list")]
        public async System.Threading.Tasks.Task<IHttpActionResult> ConsultarAccesosAsync([FromUri] string id)
        {
            ResponseListAccesos response = new ResponseListAccesos();
            var identity = (ClaimsIdentity)User.Identity;

            try
            {
                response.ListaAccesos = ListAccess(id);

                response.success = true;
                response.messages.Add("Lista de Accesos exitoso");
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.success = false;
                response.messages.Add(ex.ToString());
                return Ok(response);
            }
        }

        public List<string> ListAccess(string id)
        {
            string constr = ConfigurationManager.AppSettings["connectionString"];
            MongoClient Client = new MongoClient(constr);
            var DB = Client.GetDatabase("PRB");

            var collection = DB.GetCollection<Accesos>("Accesos");
            var filter = Builders<Accesos>.Filter.Eq(x => x.IdUser, id);

            var result = collection.Find(filter).ToList();
            List<string> listAcceso = new List<string>();

            if (result.Count > 0)
            {
                foreach (var data in result)
                {
                    listAcceso.Add(data.Clave);
                }
            }

            return listAcceso;
        }
    }
}
