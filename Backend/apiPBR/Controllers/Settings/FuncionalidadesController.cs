using apiPBR.Models.Request.Expediente;
using apiPBR.Models.Request.Settings;
using apiPBR.Models.Response;
using credentialsPBR.Models.Settings;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Web.Http;

namespace apiPBR.Controllers.Settings
{
    public class FuncionalidadesController : ApiController
    {
        [Authorize(Roles = "Ejecutivo")]
        [HttpPost]
        [Route("api/settings/funcionalidades")]
        public async System.Threading.Tasks.Task<IHttpActionResult> AgregaExpedienteAdquisicionesAsync(RequestFuncionalidades request)
        {

            GenericClass genericClass = new GenericClass();
            var identity = (ClaimsIdentity)User.Identity;
            
            try
            {
                string constr = ConfigurationManager.AppSettings["connectionString"];
                MongoClient Client = new MongoClient(constr);
                var DB = Client.GetDatabase("PRB");

                var collection = DB.GetCollection<Funcionalidades>("Funcionalidades");
                Funcionalidades funcion = new Funcionalidades();
                funcion.clave = request.Clave;
                funcion.descripcion = request.Descripcion;               

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

        [Authorize(Roles = "Ejecutivo")]
        [HttpPut]
        [Route("api/expedients/obrapublica/modifica/{id}")]
        public async System.Threading.Tasks.Task<IHttpActionResult> ActualizaExpedienteAdquisicionesAsync([FromUri] string id, [FromBody] RequestObraPublica2020 requestObraPublica)
        {

            GenericClass genericClass = new GenericClass();

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
            MongoClient Client = new MongoClient(constr);
            var DB = Client.GetDatabase("PRB");

            try
            {


                //var collection = DB.GetCollection<ObraPublicaV1>("ObraPublica");

                //var filter = Builders<ObraPublicaV1>.Filter.Eq(x => x.Id, id);


                //var result = await collection.Find(filter).FirstOrDefaultAsync();


                //if (result != null)
                //{

                //    result.auditor = idUsername;
                //    result.ejercicio = requestObraPublica.Ejercicio;
                //    result.tipoProveedor = requestObraPublica.tipoProveedor;
                //    result.tipoAdjudicacion = requestObraPublica.TipoAdjudicacion;
                //    result.montoContrato = requestObraPublica.MontoContrato.ToString();
                //    result.municipio = municipio;
                //    result.procedimiento = requestObraPublica.Procedimiento;
                //    result.tipoContrato = requestObraPublica.tipoContrato;
                //    result.numeroObra = requestObraPublica.NumeroObra;
                //    result.numeroContrato = requestObraPublica.NumeroContrato;
                //    result.numeroProcedimiento = requestObraPublica.NumeroProcedimiento;
                //    result.nombreObra = requestObraPublica.NombreObra;
                //    result.proveedor = requestObraPublica.Proveedor;
                //    result.modalidad = requestObraPublica.Modalidad;
                //    result.montoAsignado = requestObraPublica.MontoAsignado == null ? null : requestObraPublica.MontoAsignado.ToString();
                //    result.montoContrato = requestObraPublica.MontoContrato.ToString();
                //    result.montoEjercido = requestObraPublica.MontoEjercido == null ? null : requestObraPublica.MontoEjercido.ToString();
                //    result.localidad = requestObraPublica.Localidad;
                //    result.ejecutor = requestObraPublica.Ejecutor;
                //    result.responsable = requestObraPublica.Responsable;
                //    result.proyecto = requestObraPublica.Proyecto;
                //    result.programa = requestObraPublica.Programa;
                //    result.fechaContrato = requestObraPublica.FechaContrato.ToString();
                //    result.fechaProcedimiento = requestObraPublica.FechaProcedimiento.ToString();

                //    await collection.ReplaceOneAsync(filter, result);

                //    genericClass.success = true;
                //    genericClass.messages.Add("Registro Guardado con exito");
                //    return Ok(genericClass);
                //}
                //else
                //{
                genericClass.success = false;
                genericClass.messages.Add("No existe el registro");
                return Ok(genericClass);
                //}

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
