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
    public class VisitaObraController : ApiController
    {
        [Authorize(Roles = "Auditor, Coordinador, Ejecutivo")]
        [HttpGet]
        [Route("api/expedients/obrapublica/visitaobra")]
        public async System.Threading.Tasks.Task<IHttpActionResult> ShowVisitaObrasAsync(string id)
        {
            ResponseVisitaObra response = new ResponseVisitaObra
            {
                HeaderObraPublica = new HeaderObraPublica(),
                ListaVisitaObra = new List<VisitaObra>()
            };

            string constr = ConfigurationManager.AppSettings["connectionString"];
            var Client = new MongoClient(constr);
            var DB = Client.GetDatabase("PRB");
            try
            {
                var collection = DB.GetCollection<ObraPublicaV1>("ObraPublica");

                var filter = Builders<ObraPublicaV1>.Filter.Eq(x => x.Id, id);

                var result = await collection.Find(filter).FirstOrDefaultAsync();

                if (result != null)
                {


                    response.HeaderObraPublica.Id = result.Id;
                    response.HeaderObraPublica.procedimiento = result.procedimiento;
                    response.HeaderObraPublica.programa = result.programa;
                    response.HeaderObraPublica.numeroContrato = result.numeroContrato;
                    response.HeaderObraPublica.ejecutor = result.ejecutor;
                    response.HeaderObraPublica.tipoAdjudicacion = result.tipoAdjudicacion;
                    response.HeaderObraPublica.fechaRevision = result.fechaRevision;
                    response.HeaderObraPublica.localidad = result.localidad;
                    response.HeaderObraPublica.nombreObra = result.nombreObra;
                    response.HeaderObraPublica.modalidad = result.modalidad;
                    response.HeaderObraPublica.tipoContrato = result.tipoContrato;
                    response.HeaderObraPublica.tipoProveedor = result.tipoProveedor;
                    response.HeaderObraPublica.proveedor = result.proveedor;
                    response.HeaderObraPublica.numeroProcedimiento = result.numeroProcedimiento;
                    response.HeaderObraPublica.fechaProcedimiento = result.fechaProcedimiento;
                    response.HeaderObraPublica.montoAsignado = result.montoAsignado;
                    response.HeaderObraPublica.montoContrato = result.montoContrato;
                    response.HeaderObraPublica.montoEjercido = result.montoEjercido;
                    response.HeaderObraPublica.ejecucionInicio = result.ejecucionInicio;
                    response.HeaderObraPublica.ejecucionTermino = result.ejecucionTermino;
                    response.HeaderObraPublica.ejecucionPeriodo = result.ejecucionPeriodo;
                    response.HeaderObraPublica.estatusExpediente = result.estatusExpediente;

                    //GetInformationFromComplementarioExpedientes getInfoFromExpedientesComplementarios = new GetInformationFromComplementarioExpedientes();

                    //var info = await getInfoFromExpedientesComplementarios.InformacionComplementariaExpedientesPublicaAsync(result.Id, "obrapublica");

                    if (result.Location != null)
                    {
                        if (result.Location.Coordinates != null)
                        {
                            if (result.Location.Coordinates.Count > 1)
                            {
                                response.HeaderObraPublica.latitud = result.Location.Coordinates[1].ToString();
                                response.HeaderObraPublica.longitud = result.Location.Coordinates[0].ToString();
                            }
                        }
                        else
                        {
                            response.HeaderObraPublica.latitud = null;
                            response.HeaderObraPublica.longitud = null;
                        }
                    }
                    else
                    {
                        response.HeaderObraPublica.latitud = null;
                        response.HeaderObraPublica.longitud = null;
                    }

                }
                else
                {
                    response.success = false;
                    response.messages.Add("No se encontró el registro");

                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                response.success = false;
                response.messages.Add(ex.ToString());

                return Ok(response);
            }
            try
            {
                var collection = DB.GetCollection<VisitaObraHeader>("VisitaObra");

                var filter = Builders<VisitaObraHeader>.Filter.Eq(x => x.ExpedienteId, id);

                var result = await collection.Find(filter).ToListAsync();

                List<VisitaObra> ListaVisitaObra = new List<VisitaObra>();

                if (result != null)
                {
                    foreach(var r in result)
                    {
                        VisitaObra VisitaObra = new VisitaObra();
                        VisitaObra.Id = r.Id;
                        VisitaObra.AvanceFinanciero = r.AvaceFinanciero;
                        VisitaObra.AvanceFisico = r.AvanceFisico;
                        VisitaObra.FechaVisita = r.FechaVisita.ToString("yyyy-MM-dd h:mm:ss");
                        VisitaObra.SituacionActual = r.SitutacionActual;
                        VisitaObra.Problematica = r.Problematica;

                        ListaVisitaObra.Add(VisitaObra);
                    }
                }

                response.ListaVisitaObra = ListaVisitaObra;

            }
            catch(Exception ex)
            {
                response.success = false;
                response.messages.Add(ex.ToString());

                return Ok(response);
            }

            response.success = true;
            response.messages.Add("Consulta Exitosa");


            return Ok(response);
        }

        [Authorize(Roles = "Auditor, Coordinador, Ejecutivo")]
        [HttpPost]
        [Route("api/expedients/obrapublica/visitaobra/{id}")]
        public async System.Threading.Tasks.Task<IHttpActionResult> RegistraVisitaObrasAsync(RequestVistaObra requestVistaObra, string id)
        {
            GenericClass genericClass = new GenericClass();

            string constr = ConfigurationManager.AppSettings["connectionString"];
            var Client = new MongoClient(constr);
            var DB = Client.GetDatabase("PRB");
            
            try
            {
                var collection = DB.GetCollection<VisitaObraHeader>("VisitaObra");

                //var filter = Builders<VisitaObraHeader>.Filter.Eq(x => x.ExpedienteId, id);

                VisitaObraHeader visitaObraHeader = new VisitaObraHeader();

                visitaObraHeader.SitutacionActual = requestVistaObra.SituacionActual;
                visitaObraHeader.Problematica = requestVistaObra.Problematica;
                visitaObraHeader.AvaceFinanciero = requestVistaObra.AvanceFinanciero;
                visitaObraHeader.AvanceFisico = requestVistaObra.AvanceFisico;
                visitaObraHeader.ExpedienteId = id;
                visitaObraHeader.FechaVisita = requestVistaObra.FechaVisita;
                
                await collection.InsertOneAsync(visitaObraHeader);

                genericClass.success = true;
                genericClass.messages.Add("Registro Guardado con exito");

                return Ok(genericClass);
            }
            catch(Exception ex)
            {
                genericClass.success = false;
                genericClass.messages.Add(ex.ToString());

                return Ok(genericClass);
            }


        }

        [Authorize(Roles = "Auditor, Coordinador, Ejecutivo")]
        [HttpPut]
        [Route("api/expedients/obrapublica/visitaobra/{id}")]
        public async System.Threading.Tasks.Task<IHttpActionResult> ActualizaVisitaObrasAsync(RequestVistaObra requestVistaObra, string id)
        {
            GenericClass genericClass = new GenericClass();

            string constr = ConfigurationManager.AppSettings["connectionString"];
            var Client = new MongoClient(constr);
            var DB = Client.GetDatabase("PRB");

            try
            {
                var collection = DB.GetCollection<VisitaObraHeader>("VisitaObra");

                var filter = Builders<VisitaObraHeader>.Filter.Eq(x => x.Id, requestVistaObra.Id);

                var result = await collection.Find(filter).FirstOrDefaultAsync();

                if (result != null)
                {
                    VisitaObraHeader visitaObraHeader = new VisitaObraHeader();

                    visitaObraHeader.SitutacionActual = requestVistaObra.SituacionActual;
                    visitaObraHeader.Problematica = requestVistaObra.Problematica;
                    visitaObraHeader.AvaceFinanciero = requestVistaObra.AvanceFinanciero;
                    visitaObraHeader.AvanceFisico = requestVistaObra.AvanceFisico;
                    visitaObraHeader.ExpedienteId = id;
                    visitaObraHeader.Id = requestVistaObra.Id;
                    visitaObraHeader.FechaVisita = requestVistaObra.FechaVisita;


                    await collection.ReplaceOneAsync(filter, visitaObraHeader);

                }

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
    }
}
