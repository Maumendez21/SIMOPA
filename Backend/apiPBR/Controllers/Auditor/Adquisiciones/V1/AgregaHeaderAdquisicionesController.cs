using apiPBR.Models.Request.Expediente;
using apiPBR.Models.Response;
using apiPBR.Models.Response.Expedientes.Adquisiciones;
using credentialsPBR.Models.Expedientes.Adquisiciones;
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

namespace apiPBR.Controllers.Auditor.Adquisiciones.V1
{
    public class AgregaHeaderAdquisicionesController : ApiController
    {
        GetUserInfo getUserInfo = new GetUserInfo();

        [Authorize(Roles = "Auditor, Coordinador, Ejecutivo")]
        [HttpPost]
        [Route("api/expedients/adquisiciones/agrega")]
        public async System.Threading.Tasks.Task<IHttpActionResult> AgregaExpedienteAdquisicionesAsync(RequestAdquisiciones2020 requestAdquisiciones)
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


            // userPBR = new UsersPRB();
            AdquisicionesV1 AV1 = new AdquisicionesV1
            {
                documentos = new List<DocumentoAdquisicionesV1>()
            };

            try
            {
                string id = string.Empty;

                if(requestAdquisiciones.TipoAdjudicacion.Equals("ADJUDICACIÓN DIRECTA"))
                {
                    id = "5f569b334f3973399dd831ed";
                }
                else if (requestAdquisiciones.TipoAdjudicacion.Equals("CONCURSO POR INVITACIÓN"))
                {
                    id = "5f569e194f3973399ddae778";
                }
                else if (requestAdquisiciones.TipoAdjudicacion.Equals("INVITACIÓN A CUANDO MENOS TRES PERSONAS"))
                {
                    id = "5f569eb04f3973399ddb7929";
                }
                else if (requestAdquisiciones.TipoAdjudicacion.Equals("LICITACIÓN PÚBLICA"))
                {
                    id = "5f569f0a4f3973399ddbd0af";
                }
                else if (requestAdquisiciones.TipoAdjudicacion.Equals("REVISIÓN DE LAS COMPRAS URGENTES Y ESPECIALES -33 MIL"))
                {
                    id = "5f56a0554f3973399ddd0885";
                }

                //var collection = DB.GetCollection<AdquisicionesV1>("Adquisiciones");
                //var filter = Builders<AdquisicionesV1>.Filter.Eq(x => x.tipoAdjudicacion, requestAdquisiciones.TipoAdjudicacion);
                //var result = await collection.Find(filter).FirstOrDefaultAsync();

                var collection = DB.GetCollection<AdquisicionesV1>("Adquisiciones");
                var filter = Builders<AdquisicionesV1>.Filter.Eq(x => x.Id, id);
                var result = await collection.Find(filter).FirstOrDefaultAsync();

                if (result != null)
                {
                    AV1.documentos = result.documentos;
                }
                else
                {
                    genericClass.success = false;
                    genericClass.messages.Add("No existe el tipo de adjudicación seleccionado");
                    return Ok(genericClass);
                }
            }
            catch(Exception ex)
            {

            }

            foreach(var documento in AV1.documentos)
            {
                //if(role.Equals("Coordinador"))
                //{
                //    if (documento.estatus.Equals("NO"))
                //    { 
                //        documento.estatus = "POR REVISAR"; 
                //    }
                //}
                //else
                //{
                    if (documento.estatus == "SI" | documento.estatus == "N/A" | string.IsNullOrEmpty(documento.estatus))
                    { documento.estatus = "NO"; }

                    if (!documento.comentario.Equals("0 / 0"))
                    {
                        documento.comentario = "0 / 0";
                    }
                //}
            }

            try
            {
                
                //userPBR = await getUserInfo.GetInfoUserPBRAsync(idUsername, Client);

                var collection = DB.GetCollection<AdquisicionesV1>("Adquisiciones");


                var filter = Builders<AdquisicionesV1>.Filter.Eq("municipio", municipio) &
                    Builders<AdquisicionesV1>.Filter.Eq(x => x.ejercicio, requestAdquisiciones.Ejercicio) & //"2020") &
                    Builders<AdquisicionesV1>.Filter.Eq(x => x.numeroAdjudicacion, requestAdquisiciones.NumeroAdjudicacion);

                var result = await collection.Find(filter).FirstOrDefaultAsync();

                if (result != null)
                {
                    genericClass.success = false;
                    genericClass.messages.Add("Registro duplicado");
                    return Ok(genericClass);
                }
                else
                {
                    AdquisicionesV1 adquisicionesV1 = new AdquisicionesV1();

                    adquisicionesV1.auditor = idUsername;
                    adquisicionesV1.ejercicio = requestAdquisiciones.Ejercicio;// "2020";
                    adquisicionesV1.entidad = requestAdquisiciones.Entidad;
                    adquisicionesV1.estado = "PUEBLA";
                    adquisicionesV1.estatusExpediente = "CARGADO";
                    adquisicionesV1.expediente = "ADQUISICIONES";
                    adquisicionesV1.fechaRevision = "";
                    adquisicionesV1.montoAdjudicado = requestAdquisiciones.MontoAdjudicacion.ToString();
                    adquisicionesV1.municipio = municipio;
                    adquisicionesV1.numeroAdjudicacion = requestAdquisiciones.NumeroAdjudicacion;
                    adquisicionesV1.numeroContrato = requestAdquisiciones.NumeroContrato;
                    adquisicionesV1.objetoContrato = requestAdquisiciones.Objeto;
                    adquisicionesV1.observacionesGenerales = requestAdquisiciones.ObservacionesGenerales;
                    adquisicionesV1.origenRecurso = requestAdquisiciones.OrigenRecurso;
                    adquisicionesV1.proveedorAdjudicado = requestAdquisiciones.Proveedor;
                    adquisicionesV1.tipoAdjudicacion = requestAdquisiciones.TipoAdjudicacion;
                    adquisicionesV1.documentos = AV1.documentos;

                    await collection.InsertOneAsync(adquisicionesV1);

                    genericClass.success = true;
                    genericClass.messages.Add("Registro Guardado con exito");
                    return Ok(genericClass);
                }

                            }
            catch (Exception ex)
            {
                genericClass.success = false;
                genericClass.messages.Add("Error inesperado al guardar en base de datos");
                return Ok(genericClass);
            }

        }

        [Authorize(Roles = "Auditor, Coordinador, Ejecutivo")]
        [HttpPut]
        [Route("api/expedients/adquisiciones/modifica/{id}")]
        public async System.Threading.Tasks.Task<IHttpActionResult> ActualizaExpedienteAdquisicionesAsync([FromUri]string id, [FromBody]RequestAdquisiciones2020 requestAdquisiciones)
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

                //userPBR = await getUserInfo.GetInfoUserPBRAsync(idUsername, Client);

                var collection = DB.GetCollection<AdquisicionesV1>("Adquisiciones");


                var filter = Builders<AdquisicionesV1>.Filter.Eq(x => x.Id, id);

                var result = await collection.Find(filter).FirstOrDefaultAsync();

                if (result != null)
                {

                    result.auditor = idUsername;
                    result.ejercicio = requestAdquisiciones.Ejercicio;
                    result.entidad = requestAdquisiciones.Entidad;
                    result.montoAdjudicado = requestAdquisiciones.MontoAdjudicacion.ToString();
                    result.numeroAdjudicacion = requestAdquisiciones.NumeroAdjudicacion;
                    result.numeroContrato = requestAdquisiciones.NumeroContrato;
                    result.objetoContrato = requestAdquisiciones.Objeto;
                    result.observacionesGenerales = requestAdquisiciones.ObservacionesGenerales;
                    result.origenRecurso = requestAdquisiciones.OrigenRecurso;
                    result.proveedorAdjudicado = requestAdquisiciones.Proveedor;
                    result.tipoAdjudicacion = requestAdquisiciones.TipoAdjudicacion;


                    await collection.ReplaceOneAsync(filter, result);

                    genericClass.success = true;
                    genericClass.messages.Add("Registro Actualizado con exito");
                    return Ok(genericClass);
                }
                else
                {
                    genericClass.success = false;
                    genericClass.messages.Add("No existe el registro");
                    return Ok(genericClass);
                   


                }

            }
            catch (Exception ex)
            {
                genericClass.success = false;
                genericClass.messages.Add("Error inesperado al guardar en base de datos");
                return Ok(genericClass);
            }

        }
    }
}
