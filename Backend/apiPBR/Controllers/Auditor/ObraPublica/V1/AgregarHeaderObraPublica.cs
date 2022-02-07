using apiPBR.Models.Request.Expediente;
using apiPBR.Models.Response;
using credentialsPBR.Models.Expedientes.ObraPublica;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

namespace apiPBR.Controllers.Auditor.ObraPublica.V1
{
    public class AgregarHeaderObraPublicaController : ApiController
    {   
        [Authorize(Roles = "Auditor, Coordinador, Ejecutivo")]
        [HttpPost]
        [Route("api/expedients/obrapublica/agrega")]
        public async System.Threading.Tasks.Task<IHttpActionResult> AgregaExpedienteAdquisicionesAsync(RequestObraPublica2020 requestObraPublica)
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
            ObraPublicaV1 ObraV1 = new ObraPublicaV1
            {
                documentos = new List<DocumentosObrasV1>()
            };


            try
            {
                var collection = DB.GetCollection<ObraPublicaV1>("ObraPublica");
                var filter = Builders<ObraPublicaV1>.Filter.Eq(x => x.municipio, municipio);

                var result = await collection.Find(filter).FirstOrDefaultAsync();

                if (result != null)
                {
                    ObraV1.documentos = result.documentos;
                }
                else
                {
                    genericClass.success = false;
                    genericClass.messages.Add("Error insteperado al leer el registro");
                    return Ok(genericClass);
                }
            }
            catch (Exception ex)
            {

            }

            foreach (var documento in ObraV1.documentos)
            {
                if (documento.integracion == "SI" | documento.integracion == "N/A" | string.IsNullOrEmpty(documento.integracion))
                { documento.integracion = "NO"; }
                if (documento.integracion == "DOC.ERROR")
                { documento.integracion = "NO"; }

                if (!documento.observaciones.Equals("0 / 0"))
                {
                    documento.observaciones = "0 / 0";
                }
            }

            try
            {

                //userPBR = await getUserInfo.GetInfoUserPBRAsync(idUsername, Client);

                var collection = DB.GetCollection<ObraPublicaV1>("ObraPublica");


                var filter = Builders<ObraPublicaV1>.Filter.Eq("municipio", municipio) &
                    Builders<ObraPublicaV1>.Filter.Eq(x => x.ejercicio, requestObraPublica.Ejercicio) & //  "2020") &
                    Builders<ObraPublicaV1>.Filter.Eq(x => x.numeroProcedimiento, requestObraPublica.NumeroProcedimiento);

                var result = await collection.Find(filter).FirstOrDefaultAsync();

                if (result != null)
                {
                    genericClass.success = false;
                    genericClass.messages.Add("Registro duplicado");
                    return Ok(genericClass);
                }
                else
                {
                    ObraPublicaV1 obraV1 = new ObraPublicaV1
                    {
                        Location = new Coordenate(),
                        documentos = new List<DocumentosObrasV1>()
                    };

                    obraV1.auditor = idUsername;
                    obraV1.ejercicio = requestObraPublica.Ejercicio;// "2020";
                    obraV1.documentos = ObraV1.documentos;
                    obraV1.estado = "PUEBLA";
                    obraV1.estatusExpediente = "CARGADO";
                    obraV1.tipoExpediente = "OBRA";
                    obraV1.tipoProveedor = requestObraPublica.tipoProveedor;
                    obraV1.tipoAdjudicacion = requestObraPublica.TipoAdjudicacion;
                    obraV1.montoContrato = requestObraPublica.MontoContrato.ToString();
                    obraV1.municipio = municipio;
                    obraV1.numeroObra = requestObraPublica.NumeroObra;
                    obraV1.numeroContrato = requestObraPublica.NumeroContrato;
                    obraV1.numeroProcedimiento = requestObraPublica.NumeroProcedimiento;
                    obraV1.nombreObra = requestObraPublica.NombreObra;
                    obraV1.proveedor = requestObraPublica.Proveedor;
                    obraV1.modalidad = requestObraPublica.Modalidad;
                    obraV1.montoAsignado = requestObraPublica.MontoAsignado == null ? null : requestObraPublica.MontoAsignado.ToString();
                    obraV1.montoContrato = requestObraPublica.MontoContrato.ToString();
                    obraV1.montoEjercido = requestObraPublica.MontoEjercido == null ? null : requestObraPublica.MontoEjercido.ToString();
                    obraV1.localidad = requestObraPublica.Localidad;
                    obraV1.ejecutor = requestObraPublica.Ejecutor;
                    obraV1.responsable = requestObraPublica.Responsable;
                    obraV1.proyecto = requestObraPublica.Proyecto;
                    obraV1.programa = requestObraPublica.Programa;
                    obraV1.fechaContrato = requestObraPublica.FechaContrato.ToString();
                    obraV1.fechaProcedimiento = requestObraPublica.FechaProcedimiento.ToString();
                    obraV1.Location.Type = "Point";
                    obraV1.Location.Coordinates = null;

                    await collection.InsertOneAsync(obraV1);

                    genericClass.success = true;
                    genericClass.messages.Add("Registro Guardado con exito");
                    return Ok(genericClass);
                }
            }
            catch (Exception ex)
            {
                genericClass.success = false;
                genericClass.messages.Add(ex.ToString());
                return Ok(genericClass);
            }

        }

        [Authorize(Roles = "Auditor, Coordinador, Ejecutivo")]
        [HttpPut]
        //[Route("api/expedients/obrapublica/modifica/{id}")]
        [Route("api/expedients/obrapublica/modifica")]
        //public async System.Threading.Tasks.Task<IHttpActionResult> ActualizaExpedienteAdquisicionesAsync([FromUri] string id, [FromBody] RequestObraPublica2020 requestObraPublica)
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

                //userPBR = await getUserInfo.GetInfoUserPBRAsync(idUsername, Client);

                var collection = DB.GetCollection<ObraPublicaV1>("ObraPublica");

                var filter = Builders<ObraPublicaV1>.Filter.Eq(x => x.Id, id);

                
                var result = await collection.Find(filter).FirstOrDefaultAsync();
                

                if (result != null)
                {
                          
                    result.auditor = idUsername;
                    result.ejercicio = requestObraPublica.Ejercicio;
                    result.tipoProveedor = requestObraPublica.tipoProveedor;
                    result.tipoAdjudicacion = requestObraPublica.TipoAdjudicacion;
                    result.montoContrato = requestObraPublica.MontoContrato.ToString();
                    result.municipio = municipio;
                    result.procedimiento = requestObraPublica.Procedimiento;
                    result.tipoContrato = requestObraPublica.tipoContrato;
                    result.numeroObra = requestObraPublica.NumeroObra;
                    result.numeroContrato = requestObraPublica.NumeroContrato;
                    result.numeroProcedimiento = requestObraPublica.NumeroProcedimiento;
                    result.nombreObra = requestObraPublica.NombreObra;
                    result.proveedor = requestObraPublica.Proveedor;
                    result.modalidad = requestObraPublica.Modalidad;
                    result.montoAsignado = requestObraPublica.MontoAsignado == null ? null : requestObraPublica.MontoAsignado.ToString();
                    result.montoContrato = requestObraPublica.MontoContrato.ToString();
                    result.montoEjercido = requestObraPublica.MontoEjercido == null ? null : requestObraPublica.MontoEjercido.ToString();
                    result.localidad = requestObraPublica.Localidad;
                    result.ejecutor = requestObraPublica.Ejecutor;
                    result.responsable = requestObraPublica.Responsable;
                    result.proyecto = requestObraPublica.Proyecto;
                    result.programa = requestObraPublica.Programa;
                    result.fechaContrato = requestObraPublica.FechaContrato.ToString();
                    result.fechaProcedimiento = requestObraPublica.FechaProcedimiento.ToString();

                    await collection.ReplaceOneAsync(filter, result);

                    genericClass.success = true;
                    genericClass.messages.Add("Registro Guardado con exito");
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
                genericClass.messages.Add(ex.ToString());
                return Ok(genericClass);
            }

        }

    }
}