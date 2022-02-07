using apiPBR.Models.Response.Dashboard;
using credentialsPBR.Models.Dashboard;
using credentialsPBR.Models.Expedientes.Utilerias;
using credentialsPBR.Models.Users;
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
    public class DashboardExpedientesController : ApiController
    {
        GetUserInfo getUserInfo = new GetUserInfo();

        [Authorize(Roles = "Auditor, Coordinador, Ejecutivo")]
        [HttpGet]
        [Route("api/dashboard/{tipoExpediente}/{ejercicio}/{idAuditor}")]
        public async System.Threading.Tasks.Task<IHttpActionResult> ShowDashboardExpedientes(string tipoExpediente, string ejercicio, string idAuditor)
        {
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

            if (role == "Auditor")
            {
                idAuditor = idUsername;
            }


            string constr = ConfigurationManager.AppSettings["connectionString"];
            ResponseDashboardExpedientes dashboard = new ResponseDashboardExpedientes
            {
                dashboardExpedientes = new DashboardExpedientes
                {
                    ListaExpedientesEnRevision = new List<ListaExpedientesEnRevision>()
                }
            };

            try
            {

                GetExpedientesData getExpedientesData = new GetExpedientesData();
                var Expedientes = new List<ExpedienteData>();
                
                

                if (tipoExpediente == "adquisiciones")
                {
                    Expedientes = await getExpedientesData.GetInformationFromExpedientsAsync("adquisiciones", municipio, idAuditor, ejercicio);
                }
                else
                {
                    Expedientes = await getExpedientesData.GetInformationFromExpedientsAsync("obrapublica", municipio, idAuditor, ejercicio);
                }

                dashboard.dashboardExpedientes.TotalExpedientes = Expedientes.Count;
                dashboard.dashboardExpedientes.ExpedientesConObservaciones = (Expedientes.Where(x => x.estatus == "REVISADO CON OBSERVACIONES").Count());
                dashboard.dashboardExpedientes.ExpedientesSinRevisar = (Expedientes.Where(x => x.estatus == "CARGADO").Count());

                /*Estatus                 
                    CARGADO
                    EN REVSION
                    REVISADO SIN OBSERVACIONES
                    REVISADO CON OBSERVACIONES
                    CORREGIDO
                    
                    ALTA EN SISTEMA
                    CARGA EN PROCESO
                    EN SOLVENTACION
               */


                dashboard.dashboardExpedientes.Cargados = (Expedientes.Where(x => x.estatus == "CARGADO").Count());
                dashboard.dashboardExpedientes.EnRevision = (Expedientes.Where(x => x.estatus == "EN REVISION").Count()); 
                dashboard.dashboardExpedientes.RevisadoConObservaciones = (Expedientes.Where(x => x.estatus == "REVISADO CON OBSERVACIONES").Count()); 
                dashboard.dashboardExpedientes.RevisadaSinObservaciones = (Expedientes.Where(x => x.estatus == "REVISADO SIN OBSERVACIONES").Count()); 
                dashboard.dashboardExpedientes.RevisadoCorregido = (Expedientes.Where(x => x.estatus == "CORREGIDO").Count());
                dashboard.dashboardExpedientes.Solventacion = (Expedientes.Where(x => x.estatus == "EN SOLVENTACION").Count());
                dashboard.dashboardExpedientes.EnProceso = (Expedientes.Where(x => x.estatus == "CARGA EN PROCESO").Count());
                dashboard.dashboardExpedientes.AltaSistema = (Expedientes.Where(x => x.estatus == "ALTA EN SISTEMA").Count());

                //Promedio

                dashboard.dashboardExpedientes.UniversoExpedientes = Expedientes.Count;
                if (municipio == "TEPEACA" & tipoExpediente == "adquisiciones" & ejercicio == "2018")
                {
                    dashboard.dashboardExpedientes.UniversoExpedientes = 13;
                }
                if (municipio == "TEPEACA" & tipoExpediente == "adquisiciones" & ejercicio == "2019")
                {
                    dashboard.dashboardExpedientes.UniversoExpedientes = 60;
                }
                if (municipio == "TEPEACA" & tipoExpediente == "obrapublica" & ejercicio == "2018")
                {
                    dashboard.dashboardExpedientes.UniversoExpedientes = 23;
                }
                if (municipio == "TEPEACA" & tipoExpediente == "obrapublica" & ejercicio == "2019")
                {
                    dashboard.dashboardExpedientes.UniversoExpedientes = 85;
                }

                double porcentaje = 0;

                dashboard.dashboardExpedientes.UniversoExpedientes = Expedientes.Count;

                if (municipio == "TEPEACA" & tipoExpediente == "adquisiciones" & ejercicio == "2018")
                {
                    dashboard.dashboardExpedientes.UniversoExpedientes = 13;
                }
                if (municipio == "TEPEACA" & tipoExpediente == "adquisiciones" & ejercicio == "2019")
                {
                    dashboard.dashboardExpedientes.UniversoExpedientes = 60;
                }
                if (municipio == "TEPEACA" & tipoExpediente == "obrapublica" & ejercicio == "2018")
                {
                    dashboard.dashboardExpedientes.UniversoExpedientes = 23;
                }
                if (municipio == "TEPEACA" & tipoExpediente == "obrapublica" & ejercicio == "2019")
                {
                    dashboard.dashboardExpedientes.UniversoExpedientes = 85;
                }
                

                foreach (var expediente in Expedientes)
                {

                    if (expediente.estatus == "EN REVISION")
                    {
                        ListaExpedientesEnRevision ListaExpedientesEnRevision = new ListaExpedientesEnRevision();

                        ListaExpedientesEnRevision.NombreExpediente = expediente.NombreExpediente;
                        ListaExpedientesEnRevision.avanceDocumental = expediente.AvanceDocumental;
                        ListaExpedientesEnRevision.idExpediente = expediente.idExpediente;

                        dashboard.dashboardExpedientes.ListaExpedientesEnRevision.Add(ListaExpedientesEnRevision);
                    }
                    else if (expediente.estatus == "REVISADO CON OBSERVACIONES")
                    {
                        ListaExpedientesEnRevision ListaExpedientesEnRevision = new ListaExpedientesEnRevision();

                        ListaExpedientesEnRevision.NombreExpediente = expediente.NombreExpediente;
                        ListaExpedientesEnRevision.avanceDocumental = expediente.AvanceDocumental;
                        ListaExpedientesEnRevision.idExpediente = expediente.idExpediente;

                        dashboard.dashboardExpedientes.ListaExpedientesEnRevision.Add(ListaExpedientesEnRevision);
                    }


                    porcentaje = porcentaje + (expediente.Porcentaje/100);
                }

                dashboard.dashboardExpedientes.Porcentaje = porcentaje == 0 ? 0 : Math.Round(((porcentaje * 100) / Expedientes.Count),2);

                    
                dashboard.success = true;
                dashboard.messages.Add("Respuesta con exito");

                return Ok(dashboard);
            }
            catch (Exception ex)
            {

                dashboard.success = false;
                dashboard.messages.Add(ex.ToString());
                return Ok(dashboard);
            }

        }

        [Authorize(Roles = "Auditor, Coordinador, Ejecutivo")]
        [HttpGet]
        [Route("api/dashboard/procedimiento/{tipoExpediente}/{ejercicio}/{idAuditor}")]
        public async System.Threading.Tasks.Task<IHttpActionResult> ShowDashboardProcedimiento(string tipoExpediente, string ejercicio, string idAuditor)
        {
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

            if (role == "Auditor")
            {
                idAuditor = idUsername;
            }


            string constr = ConfigurationManager.AppSettings["connectionString"];
            ResponseGraficas dashboard = new ResponseGraficas();
           
            
            Graficas graficas = new Graficas();

            try
            {              
                if (tipoExpediente == "adquisiciones")
                {
                    graficas = await (new GetExpedientesData()).GetInformationFromProcedimientoAsync("adquisiciones", municipio, idAuditor, ejercicio);
                }
                else
                {
                    graficas = await (new GetExpedientesData()).GetInformationFromProcedimientoAsync("obrapublica", municipio, idAuditor, ejercicio);
                }

                dashboard.graficas = graficas;
                dashboard.success = true;
                dashboard.messages.Add("Respuesta con exito");

                return Ok(dashboard);
            }
            catch (Exception ex)
            {

                dashboard.success = false;
                dashboard.messages.Add(ex.ToString());
                return Ok(dashboard);
            }

        }
        
    }
}
