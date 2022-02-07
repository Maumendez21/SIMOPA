using apiPBR.Models.Request.Auditors;
using apiPBR.Models.Response;
using apiPBR.Models.Response.Expedientes;
using apiPBR.Models.Response.Expedientes.Adquisiciones;
using credentialsPBR.Models.Expedientes.Adquisiciones;
using credentialsPBR.Models.Expedientes.ObraPublica;
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
    public class ModExpedientAquisicionController : ApiController
    {
        GetUserInfo getUserInfo = new GetUserInfo();

        [Authorize(Roles = "Auditor, Coordinador, Ejecutivo")]
        [HttpPost]
        [Route("api/expedients/{tipoExpediente}/observaciones/{idExpediente}/{clave}")]
        public async System.Threading.Tasks.Task<IHttpActionResult> ModExpedientDocumentAsync([FromBody]ModObservacionDocumento modObservacionDocumento, string tipoExpediente, string idExpediente, string clave)
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
            Observaciones observaciones = new Observaciones();
            
            try
            {
                MongoClient Client = new MongoClient(constr);

                //UsersPRB userPBR = new UsersPRB();
                //userPBR = await getUserInfo.GetInfoUserPBRAsync(idUsername, Client);

                var DB = Client.GetDatabase("PRB");
                var collection = DB.GetCollection<Observaciones>("Observaciones");

                //var filter = Builders<Observaciones>.Filter.Eq(x => x.Id, modObservacionDocumento.id);

                //var result = await collection.Find(filter).FirstOrDefaultAsync();

                /*if (result != null)
                {
                    foreach (var d in result.documentos)
                    {
                        if (d.clave == modObservacionDocumento.Clave)
                        {
                            d.comentario = modObservacionDocumento.Observacion;
                            d.recomendacion = modObservacionDocumento.Recomendacion;
                        }
                    }

                    await collection.ReplaceOneAsync(filter, result);
                }*/

                observaciones.idExpediente = modObservacionDocumento.idExpediente;
                observaciones.Estatus = modObservacionDocumento.Estatus;
                observaciones.Clave = modObservacionDocumento.Clave;
                observaciones.Observacion = modObservacionDocumento.Observacion;
                observaciones.Recomendacion = modObservacionDocumento.Recomendacion;
                observaciones.tipoExpediente = modObservacionDocumento.TipoExpediente;

                await collection.InsertOneAsync(observaciones);


                var filterO = Builders<Observaciones>.Filter.Eq(x => x.idExpediente, modObservacionDocumento.idExpediente) &
                  Builders<Observaciones>.Filter.Eq(x => x.Clave, modObservacionDocumento.Clave);

                var resultO = await collection.Find(filterO).ToListAsync();

                if (resultO != null)
                {

                    string valor = resultO.Where(x => x.Estatus == 1).Count() + " / " + resultO.Count();
                    if (modObservacionDocumento.TipoExpediente == "Adquisiciones")
                    {
                        await  ActulizaExpedienteAdquisicionesAsync(DB, modObservacionDocumento.idExpediente, valor, modObservacionDocumento.Clave);
                    }
                    else
                    {
                        await ActulizaExpedienteObraPublicaAsync(DB, modObservacionDocumento.idExpediente, valor, modObservacionDocumento.Clave);
                    }
                }
                

                genericClass.success = true;
                genericClass.messages.Add("Actualización exitosa");
            }
            catch (Exception ex)
            {
                genericClass.success = false;
                genericClass.messages.Add("Error inesperado " + ex.ToString() );
            }



            return Ok(genericClass);
        }

        [Authorize(Roles = "Auditor, Coordinador, Ejecutivo")]
        [HttpPut]
        [Route("api/expedients/{tipoExpediente}/observaciones/{idExpediente}/{clave}")]
        public async System.Threading.Tasks.Task<IHttpActionResult> EditExpedientDocumentAsync([FromBody]ModObservacionDocumento modObservacionDocumento, string tipoExpediente, string idExpediente, string clave)
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
            Observaciones observaciones = new Observaciones();

            try
            {
                MongoClient Client = new MongoClient(constr);

                UsersPRB userPBR = new UsersPRB();
                userPBR = await getUserInfo.GetInfoUserPBRAsync(idUsername, Client);

                var DB = Client.GetDatabase("PRB");
                var collection = DB.GetCollection<Observaciones>("Observaciones");

                var filter = Builders<Observaciones>.Filter.Eq(x => x.Id, modObservacionDocumento.id);
               

                var result = await collection.Find(filter).FirstOrDefaultAsync();

                if (result != null)
                {

                    observaciones.Id = modObservacionDocumento.id;
                    observaciones.idExpediente = modObservacionDocumento.idExpediente;
                    observaciones.Estatus = modObservacionDocumento.Estatus;
                    observaciones.Clave = modObservacionDocumento.Clave;
                    observaciones.Observacion = modObservacionDocumento.Observacion;
                    observaciones.Recomendacion = modObservacionDocumento.Recomendacion;
                    observaciones.tipoExpediente = modObservacionDocumento.TipoExpediente;


                    await collection.ReplaceOneAsync(filter, observaciones);
                }


                
                var filterO = Builders<Observaciones>.Filter.Eq(x => x.idExpediente, modObservacionDocumento.idExpediente) &
                   Builders<Observaciones>.Filter.Eq(x => x.Clave, modObservacionDocumento.Clave);


                var resultO = await collection.Find(filterO).ToListAsync();
                
                if (resultO != null)
                {

                    string valor = resultO.Where(x=>x.Estatus == 1).Count() + " / " +resultO.Count();
                    if (modObservacionDocumento.TipoExpediente == "Adquisiciones")
                    {
                        await ActulizaExpedienteAdquisicionesAsync(DB, modObservacionDocumento.idExpediente, valor, modObservacionDocumento.Clave);
                    }
                    else
                    {
                        await ActulizaExpedienteObraPublicaAsync(DB, modObservacionDocumento.idExpediente, valor, modObservacionDocumento.Clave);
                    }
                }
                



                genericClass.success = true;
                genericClass.messages.Add("Actualización exitosa");
            }
            catch (Exception ex)
            {
                genericClass.success = false;
                genericClass.messages.Add("Error inesperado " + ex.ToString());
            }

            return Ok(genericClass);
        }


        public async System.Threading.Tasks.Task ActulizaExpedienteAdquisicionesAsync (IMongoDatabase db, string id, string valor, int clave)
        {
            try
            {
                var collection = db.GetCollection<AdquisicionesV1>("Adquisiciones");

                var Efilter = Builders<AdquisicionesV1>.Filter.Eq(x => x.Id, id);
                var result = await collection.Find(Efilter).FirstOrDefaultAsync();


                if (result != null)
                {
                    result.documentos.Where(x => x.clave == clave).FirstOrDefault().comentario = valor;

                    await collection.ReplaceOneAsync(Efilter, result);
                }
            }
            catch (Exception ex)
            {

            }
        }
        public async System.Threading.Tasks.Task ActulizaExpedienteObraPublicaAsync(IMongoDatabase db, string id, string valor, int clave)
        {
            try
            {
                var collection = db.GetCollection<ObraPublicaV1>("ObraPublica");

                var Efilter = Builders<ObraPublicaV1>.Filter.Eq(x => x.Id, id);
                var result = await collection.Find(Efilter).FirstOrDefaultAsync();


                if (result != null)
                {

                    result.documentos.Where(x => x.clave == clave).FirstOrDefault().observaciones = valor;

                    await collection.ReplaceOneAsync(Efilter, result);
                }
            }
            catch (Exception ex)
            {

            }
        }

        [Authorize(Roles = "Auditor, Coordinador, Ejecutivo")]
        [HttpGet]
        [Route("api/expedients/{tipoExpediente}/observaciones/{idExpediente}/{clave}")]
        public async System.Threading.Tasks.Task<IHttpActionResult> AddExpedientDocumentAsync(string tipoExpediente, string idExpediente, string clave)
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

            ResponseObservaciones response = new ResponseObservaciones
            {
                ListadoObservaciones = new List<ObservacionesSalida>()
            };

            try
            {
                MongoClient Client = new MongoClient(constr);

                UsersPRB userPBR = new UsersPRB();
                userPBR = await getUserInfo.GetInfoUserPBRAsync(idUsername, Client);

                var DB = Client.GetDatabase("PRB");
                var collection = DB.GetCollection<Observaciones>("Observaciones");

                if (tipoExpediente == "obrapublica")
                {
                    tipoExpediente = "ObraPublica";
                }
                if (tipoExpediente == "adquisiciones")
                {
                    tipoExpediente = "Adquisiciones";
                }

                var filter = Builders<Observaciones>.Filter.Eq(x => x.idExpediente,idExpediente) & 
                    Builders<Observaciones>.Filter.Eq(x => x.tipoExpediente, tipoExpediente) &
                    Builders<Observaciones>.Filter.Eq(x => x.Clave, Convert.ToInt32(clave));

                var result = await collection.Find(filter).ToListAsync();

                if (result != null)
                {
                    int contador = 1;
                    foreach (var d in result)
                    {
                        ObservacionesSalida observaciones = new ObservacionesSalida();
                        observaciones.contador = contador;
                        observaciones.Clave = d.Clave;
                        observaciones.Estatus = d.Estatus;
                        observaciones.Id = d.Id;
                        observaciones.idExpediente = d.idExpediente;
                        observaciones.Observacion = d.Observacion;
                        observaciones.Recomendacion = d.Recomendacion;
                        observaciones.tipoExpediente = d.tipoExpediente;


                        response.ListadoObservaciones.Add(observaciones);
                        contador++;
                    }

                }
                response.success = true;
                response.messages.Add("Exito");

            }
            catch (Exception ex)
            {
                response.success = true;
                response.messages.Add(ex.ToString());
            }

            return Ok(response);
        }

        [Authorize(Roles = "Auditor, Coordinador, Ejecutivo")]
        [HttpPut]
        [Route("api/expedients/{tipoExpediente}/observaciongeneral/{idExpediente}")]
        public async System.Threading.Tasks.Task<IHttpActionResult> ModExpedienteObservacionGeneralDocumentAsync([FromBody]ObservacionGeneral observacionGeneral, string tipoExpediente, string idExpediente)
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

            AdquisicionesV1 adquisicionesV1 = new AdquisicionesV1();
            ObraPublicaV1 obraPublicaV1 = new ObraPublicaV1();

            GenericClass genericClass = new GenericClass();

            if (tipoExpediente == "adquisiciones")
            {
                try
                {
                    MongoClient Client = new MongoClient(constr);

                    UsersPRB userPBR = new UsersPRB();
                    userPBR = await getUserInfo.GetInfoUserPBRAsync(idUsername, Client);

                    var DB = Client.GetDatabase("PRB");
                    var collection = DB.GetCollection<AdquisicionesV1>("Adquisiciones");

                    var filter = Builders<AdquisicionesV1>.Filter.Eq(x => x.Id, idExpediente);

                    var result = await collection.Find(filter).FirstOrDefaultAsync();

                    if (result != null)
                    {
                        adquisicionesV1 = result;
                        adquisicionesV1.observacionesGenerales = observacionGeneral.observacionGeneral;

                        await collection.ReplaceOneAsync(filter, adquisicionesV1);
                    }

                    genericClass.success = true;
                    genericClass.messages.Add("Actualización exitosa");
                }
                catch (Exception ex)
                {
                    genericClass.success = false;
                    genericClass.messages.Add("Error inesperado");
                }
            }
            else
            {
                try
                {
                    MongoClient Client = new MongoClient(constr);

                    UsersPRB userPBR = new UsersPRB();
                    userPBR = await getUserInfo.GetInfoUserPBRAsync(idUsername, Client);

                    var DB = Client.GetDatabase("PRB");
                    var collection = DB.GetCollection<ObraPublicaV1>("ObraPublica");

                    var filter = Builders<ObraPublicaV1>.Filter.Eq(x => x.Id, idExpediente);

                    var result = await collection.Find(filter).FirstOrDefaultAsync();

                    if (result != null)
                    {
                        obraPublicaV1 = result;
                        obraPublicaV1.ObservacionesGenerales = observacionGeneral.observacionGeneral;

                        await collection.ReplaceOneAsync(filter, obraPublicaV1);
                    }

                    genericClass.success = true;
                    genericClass.messages.Add("Actualización exitosa");
                }
                catch (Exception ex)
                {
                    genericClass.success = false;
                    genericClass.messages.Add("Error inesperado");
                }
            }

            

            return Ok(genericClass);
        }
    }
}
