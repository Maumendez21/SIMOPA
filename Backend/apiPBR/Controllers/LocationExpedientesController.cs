using apiPBR.Models.Request.Auditors;
using apiPBR.Models.Response;
using credentialsPBR.Models.Expedientes.ObraPublica;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace apiPBR.Controllers
{
    public class LocationExpedientesController : ApiController
    {
        [Authorize(Roles = "Auditor, Coordinador, Ejecutivo")]
        [HttpPost]
        [Route("api/expedients/location/{tipoExpediente}/{id}")]
        public async System.Threading.Tasks.Task<IHttpActionResult> ActualizacionLocationExpedientes([FromBody]InformacionComplementariaExpediente informacionComplementariaExpediente, string tipoExpediente, string id)
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

                    List<double> coordenadas = new List<double>();
                    coordenadas.Add(Convert.ToDouble(informacionComplementariaExpediente.longitud));
                    coordenadas.Add(Convert.ToDouble(informacionComplementariaExpediente.latitud));

                    var update = Builders<ObraPublicaV1>.Update.Set(x => x.Location.Type, "Point").Set(x => x.Location.Coordinates, coordenadas);
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

            genericClass.success = false;
            genericClass.messages.Add("Aun no habilitado la opción para Adquisiciones");
            return Ok(genericClass);

        }
    }
}
