using apiPBR.Models;
using apiPBR.Models.Request.Expediente;
using apiPBR.Models.Response;
using apiPBR.Models.Response.Expedientes.ObraPublica;
using credentialsPBR.Models.Expedientes.ObraPublica;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace apiPBR.Controllers.Auditor.ObraPublica.V1
{
    public class PartidasPrincipalesObraController : ApiController
    {
        [Authorize(Roles = "Auditor, Coordinador, Ejecutivo")]
        [HttpGet]
        [Route("api/expedients/obrapublica/partidasprincipales/{id}")]
        public async System.Threading.Tasks.Task<IHttpActionResult> ShowPartidasPrincipalesObrasAsync(string id)
        {
            ResponsePartidaPrincipalObra response = new ResponsePartidaPrincipalObra
            {
                PartidasPrincipalesObraPublica = new List<PartidasPrincipalesObra>()
            };

            string constr = ConfigurationManager.AppSettings["connectionString"];
            var Client = new MongoClient(constr);
            var DB = Client.GetDatabase("PRB");

            try
            {
                var collection = DB.GetCollection<PartidasPrincipalesObra>("PartidasPrincipalesObra");

                var filter = Builders<PartidasPrincipalesObra>.Filter.Eq(x => x.ExpedienteId, id);

                var result = await collection.Find(filter).ToListAsync();

                List<PartidasPrincipalesObra> ListaPartidaPrincipal = new List<PartidasPrincipalesObra>();

                if (result != null)
                {
                    foreach (var r in result)
                    {
                        PartidasPrincipalesObra partidaPrincipal = new PartidasPrincipalesObra();

                        partidaPrincipal.Id = r.Id ;
                        partidaPrincipal.ExpedienteId = r.ExpedienteId;
                        partidaPrincipal.Nombre = r.Nombre;

                        ListaPartidaPrincipal.Add(partidaPrincipal);
                    }
                    response.success = true;
                    response.messages.Add("Solicitud Exitosa");
                    response.PartidasPrincipalesObraPublica = ListaPartidaPrincipal;
                }

                response.success = true;
                response.messages.Add("No hay registros");

                return Ok(response);

            }
            catch (Exception ex)
            {
                response.success = false;
                response.messages.Add(ex.ToString());

                return Ok(response);
            }
        }

        [Authorize(Roles = "Auditor, Coordinador, Ejecutivo")]
        [HttpPost]
        [Route("api/expedients/obrapublica/partidasprincipales")]
        public async System.Threading.Tasks.Task<IHttpActionResult> AgregaPartidasPrincipalesObrasAsync([FromBody]RequestPartidaPrincipal requestPartidaPrincipal)
        {
            GenericClass basicResponse = new GenericClass();

            string constr = ConfigurationManager.AppSettings["connectionString"];
            var Client = new MongoClient(constr);
            var DB = Client.GetDatabase("PRB");

            try
            {
                var collection = DB.GetCollection<PartidasPrincipalesObra>("PartidasPrincipalesObra");

                //var filter = Builders<PartidasPrincipalesObra>.Filter.Eq(x => x.ExpedienteId, id);

                //var result = await collection.Find(filter).ToListAsync();

                
                        PartidasPrincipalesObra partidaPrincipal = new PartidasPrincipalesObra();

                        partidaPrincipal.ExpedienteId = requestPartidaPrincipal.idExpediente;
                        partidaPrincipal.Nombre = requestPartidaPrincipal.Nombre;

                await collection.InsertOneAsync(partidaPrincipal);

                basicResponse.success = true;
                basicResponse.messages.Add("Registro agregado Exitosamente");
                
                return Ok(basicResponse);
            }                        
            catch (Exception ex)
            {
                basicResponse.success = false;
                basicResponse.messages.Add(ex.ToString());

                return Ok(basicResponse);
            }
        }
    }
}
