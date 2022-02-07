using apiPBR.Models.Response;
using credentialsPBR.Models.Expedientes.Adquisiciones;
using credentialsPBR.Models.Expedientes.ObraPublica;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Security.Claims;
using System.Web.Http;

namespace apiPBR.Controllers.BitacoraAsignacion
{
    public class BitacoraAsignacionController : ApiController
    {
        [Authorize(Roles = "Auditor, Coordinador, Ejecutivo")]
        [HttpPut]
        [Route("api/bitacora/asignacion/auditor/{tipo}/{expediente}")]
        public async System.Threading.Tasks.Task<IHttpActionResult> AgregaExpedienteAdquisicionesAsync(string tipo, string expediente)
        {
            GenericClass genericClass = new GenericClass();
            var identity = (ClaimsIdentity)User.Identity;
            IEnumerable<Claim> claims = identity.Claims;
            var idUser = string.Empty;
            var propietario = string.Empty;

            foreach (var c in claims)
            {
                if (c.Type == "id")
                {
                    idUser = c.Value;
                    break;
                }
            }

            try
            {
                string constr = ConfigurationManager.AppSettings["connectionString"];
                MongoClient Client = new MongoClient(constr);
                var DB = Client.GetDatabase("PRB");

                if(tipo.Equals("Adquisiciones"))
                {
                    var collection = DB.GetCollection<AdquisicionesV1>("Adquisiciones");
                    var filter = Builders<AdquisicionesV1>.Filter.Eq(x => x.Id, expediente);
                    var result = await collection.Find(filter).FirstOrDefaultAsync();

                    if (result != null)
                    {
                        propietario = result.auditor;
                        result.auditor = idUser;
                        await collection.ReplaceOneAsync(filter, result);
                    }
                    else
                    {
                        genericClass.success = false;
                        genericClass.messages.Add("No existe el registro");
                        return Ok(genericClass);
                    }
                }
                else if (tipo.Equals("Obra"))
                {
                    var collection = DB.GetCollection<ObraPublicaV1>("ObraPublica");
                    var filter = Builders<ObraPublicaV1>.Filter.Eq(x => x.Id, expediente);
                    var result = await collection.Find(filter).FirstOrDefaultAsync();

                    if (result != null)
                    {
                        propietario = result.auditor;
                        result.auditor = idUser;
                        await collection.ReplaceOneAsync(filter, result);
                    }
                    else
                    {
                        genericClass.success = false;
                        genericClass.messages.Add("No existe el registro");
                        return Ok(genericClass);
                    }
                }
                
                DateTime timeUtc = DateTime.UtcNow;
                TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
                DateTime fecha = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, cstZone);

                var collectionBitacora = DB.GetCollection<credentialsPBR.Models.Expedientes.BitacoraAsignacion.BitacoraAsignacion>("BitacoraAsignacion");
                credentialsPBR.Models.Expedientes.BitacoraAsignacion.BitacoraAsignacion bitacora = new credentialsPBR.Models.Expedientes.BitacoraAsignacion.BitacoraAsignacion();
                bitacora.IdPropietario = propietario;
                bitacora.IdAsignado = idUser;
                bitacora.Fecha = fecha;

                await collectionBitacora.InsertOneAsync(bitacora);

                genericClass.success = true;
                genericClass.messages.Add("Registro Actualizado con exito");
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
