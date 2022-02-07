using apiPBR.Models.Request.Auditors;
using apiPBR.Models.Response;
using credentialsPBR.Models.Expedientes.Adquisiciones;
using credentialsPBR.Models.Expedientes.ObraPublica;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace apiPBR.Controllers.Auditor
{
    public class EstatusExpedientesController : ApiController
    {

        [Authorize(Roles = "Auditor, Coordinador, Ejecutivo")]
        [HttpPost]
        [Route("api/expedients/estatus/{tipoExpediente}/{id}")]
        public async System.Threading.Tasks.Task<IHttpActionResult> ActualizacionEstatusExpedientes([FromBody]InformacionComplementariaExpediente informacionComplementariaExpediente, string tipoExpediente, string id)
        {
            GenericClass genericClass = new GenericClass();

            string constr = ConfigurationManager.AppSettings["connectionString"];

            if (tipoExpediente == "obrapublica")
            {
                try
                {
                    var Client = new MongoClient(constr);

                    var DB = Client.GetDatabase("PRB");
                    var collection = DB.GetCollection<ObraPublicaV1>("ObraPublica");

                    var filter = Builders<ObraPublicaV1>.Filter.Eq(x => x.Id, id);

                    var update = Builders<ObraPublicaV1>.Update.Set(x => x.estatusExpediente, informacionComplementariaExpediente.estatusExpediente);
                    var resultComplemento = await collection.UpdateOneAsync(filter, update);

                    genericClass.success = true;
                    genericClass.messages.Add("Respuesta exitosa");
                    return Ok(genericClass);
                }
                catch (Exception ex)
                {
                    genericClass.success = false;
                    genericClass.messages.Add(ex.ToString());
                    return Ok(genericClass);
                }
            }
            else
            {
                try
                {
                    var Client = new MongoClient(constr);

                    var DB = Client.GetDatabase("PRB");
                    var collection = DB.GetCollection<AdquisicionesV1>("Adquisiciones");

                    var filter = Builders<AdquisicionesV1>.Filter.Eq(x => x.Id, id);

                    var update = Builders<AdquisicionesV1>.Update.Set(x => x.estatusExpediente, informacionComplementariaExpediente.estatusExpediente);

                    var resultComplemento = await collection.UpdateOneAsync(filter, update);

                    genericClass.success = true;
                    genericClass.messages.Add("Respuesta exitosa");
                    return Ok(genericClass);
                }
                catch (Exception ex)
                {
                    genericClass.success = false;
                    genericClass.messages.Add(ex.ToString());
                    return Ok(genericClass);
                }
            }
        }

        [Authorize(Roles = "Auditor, Coordinador, Ejecutivo")]
        [HttpPost]
        [Route("api/expedients/estatus/{tipoExpediente}/{idExpediente}/{clave}")]
        public async System.Threading.Tasks.Task<IHttpActionResult> ActualizacionEstatusExpedientesDocumentos([FromBody]EstatusDocumentos estatusDocumentos, string tipoExpediente, string idExpediente, string clave)
        {
            GenericClass genericClass = new GenericClass();

            string constr = ConfigurationManager.AppSettings["connectionString"];

            if (tipoExpediente == "obrapublica")
            {
                try
                {
                    var Client = new MongoClient(constr);

                    var DB = Client.GetDatabase("PRB");
                    var collection = DB.GetCollection<ObraPublicaV1>("ObraPublica");

                    var filter = Builders<ObraPublicaV1>.Filter.Eq(x => x.Id, idExpediente);

                    var result = await collection.Find(filter).FirstOrDefaultAsync();


                    if (result != null)
                    {
                        foreach (var d in result.documentos)
                        {
                            if (d.clave == Convert.ToInt32(clave))
                            {
                                d.integracion = estatusDocumentos.estatusDocumento;
                            }
                        }

                        var resultComplemento = collection.ReplaceOne(filter, result);

                        genericClass.success = true;
                        genericClass.messages.Add("Respuesta exitosa");
                        return Ok(genericClass);
                    }
                    genericClass.success = false;
                    genericClass.messages.Add("NO hay registros");
                    return Ok(genericClass);

                }
                catch (Exception ex)
                {
                    genericClass.success = false;
                    genericClass.messages.Add(ex.ToString());
                    return Ok(genericClass);
                }
            }
            else
            {
                try
                {
                    var Client = new MongoClient(constr);

                    var DB = Client.GetDatabase("PRB");
                    var collection = DB.GetCollection<AdquisicionesV1>("Adquisiciones");

                    var filter = Builders<AdquisicionesV1>.Filter.Eq(x => x.Id, idExpediente);

                    var result = await collection.Find(filter).FirstOrDefaultAsync();


                    if (result != null)
                    {
                        foreach (var d in result.documentos)
                        {
                            if (d.clave == Convert.ToInt32(clave))
                            {
                                d.estatus = estatusDocumentos.estatusDocumento;
                            }
                        }

                        var resultComplemento = collection.ReplaceOne(filter, result);

                        genericClass.success = true;
                        genericClass.messages.Add("Respuesta exitosa");
                        return Ok(genericClass);
                    }
                    genericClass.success = false;
                    genericClass.messages.Add("NO hay registros");
                    return Ok(genericClass);
                }
                catch (Exception ex)
                {
                    genericClass.success = false;
                    genericClass.messages.Add(ex.ToString());
                    return Ok(genericClass);
                }
            }
        }

    }
}
