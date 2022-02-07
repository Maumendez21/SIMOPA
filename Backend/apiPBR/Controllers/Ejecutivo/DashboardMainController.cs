using apiPBR.Models.Response.Dashboard;
using credentialsPBR.Models.Dashboard;
using credentialsPBR.Models.Expedientes.Adquisiciones;
using credentialsPBR.Models.Expedientes.ObraPublica;
using credentialsPBR.Models.Expedientes.Utilerias;
using credentialsPBR.Models.Users;
using MongoDB.Bson;
using MongoDB.Driver;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Information;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace apiPBR.Controllers.Ejecutivo
{
    public class DashboardMainController : ApiController
    {
        GetUserInfo getUserInfo = new GetUserInfo();

        [Authorize(Roles = "Ejecutivo")]
        [HttpGet]
        [Route("api/dashboard/principal/{ejercicio}")]
        public async System.Threading.Tasks.Task<IHttpActionResult> ShowDashboardMain(string ejercicio)
        {
            var idUsername = string.Empty;
            var role = string.Empty;
            string municipio = string.Empty;
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
            ResponseDashboardMain principal = new ResponseDashboardMain
            {
                Principal = new Principal()
            };

            try
            {
                GetDatosGeneralMunicipio getDatosGeneralMunicipio = new GetDatosGeneralMunicipio();

                GetExpedientesData getExpedientesData = new GetExpedientesData();
                var ExpedientesAdquisiciones = new List<ExpedienteData>();
                var ExpedientesObras = new List<ExpedienteData>();


                ExpedientesAdquisiciones = await getExpedientesData.GetInformationFromExpedientsAsync("adquisiciones", municipio, "auditorestodosdq37iq84burwwjdbsd3", ejercicio);
                ExpedientesObras = await getExpedientesData.GetInformationFromExpedientsAsync("obrapublica", municipio, "auditorestodosdq37iq84burwwjdbsd3", ejercicio);
                //-----------------
                double porcentajeAdj = 0;
                double porcentajeObra = 0;

                foreach (var expediente in ExpedientesAdquisiciones)
                {
                    porcentajeAdj = porcentajeAdj + (expediente.Porcentaje / 100);
                }

                principal.Principal.PorcentajeAdj = porcentajeAdj == 0 ? 0 : Math.Round(((porcentajeAdj * 100) / ExpedientesAdquisiciones.Count), 2);

                foreach (var expediente in ExpedientesObras)
                {
                    porcentajeObra = porcentajeObra + (expediente.Porcentaje / 100);
                }

                principal.Principal.PorcentajeObra = porcentajeObra == 0 ? 0 : Math.Round(((porcentajeObra * 100) / ExpedientesObras.Count), 2);
                //------------------
                double MontoContratadoAdquisiciones = 0;
                foreach(var eA in ExpedientesAdquisiciones)
                {
                    MontoContratadoAdquisiciones += eA.MontoContratado;
                }
                
                double MontoContratadoObras = 0;
                double MontoPagadoObras = 0;
                double MontoAsignado = 0;
                foreach (var eA in ExpedientesObras)
                {
                    MontoContratadoObras += eA.MontoContratado;
                    MontoPagadoObras += eA.MontoPagado;
                    MontoAsignado = +eA.MontoAsignado;
                }

                var GeneralMunicipios = await getDatosGeneralMunicipio.DatosGeneralMunicipioAsync(municipio, ejercicio);

                //principal.Principal.AdquisicionesConProblemas = ExpedientesAdquisiciones.Where(x => x.estatus == "REVISADO CON OBSERVACIONES").Count();
                //principal.Principal.AdquisicionesContratadas = ExpedientesAdquisiciones.Count();
                //principal.Principal.AdquisicionesPresupuestadas = ExpedientesAdquisiciones.Count();


                //Tarjeta header principal
                principal.Principal.PresupuestoAutorizado = GeneralMunicipios.presupuestoAutorizadoAdquisiciones + GeneralMunicipios.presupuestoAutorizadoObras;
                principal.Principal.PresupuestoAutorizado_Texto = principal.Principal.PresupuestoAutorizado.ToString("C", CultureInfo.CurrentCulture);
                principal.Principal.PresupuestoComprometido = MontoContratadoAdquisiciones + MontoContratadoObras;
                principal.Principal.PresupuestoComprometido_Texto = principal.Principal.PresupuestoComprometido.ToString("C", CultureInfo.CurrentCulture);
                principal.Principal.PorcentajeContratadoPresupuestoAutorizado = (int)(((MontoContratadoAdquisiciones + MontoContratadoObras) * 100) / (GeneralMunicipios.presupuestoAutorizadoAdquisiciones + GeneralMunicipios.presupuestoAutorizadoObras));


                principal.Principal.PorcentajePresupuestoAutorizadoObraPublica = (int)(((MontoContratadoObras) * 100) / GeneralMunicipios.presupuestoAutorizadoObras);
                principal.Principal.PorcentajePresupuestoAutorizadoAdquisiciones = (int)(((MontoContratadoAdquisiciones) * 100) / GeneralMunicipios.presupuestoAutorizadoAdquisiciones);

                

                /*
                if (ejercicio == "2018")
                {
                    //principal.Principal.PorcentajeContratadoPresupuestoAutorizado = 3;
                    principal.Principal.PorcentajePresupuestoAutorizadoObraPublica = 85;
                    principal.Principal.PorcentajePresupuestoAutorizadoAdquisiciones = 80;
                }
                if (ejercicio == "2019")
                {
                    //principal.Principal.PorcentajeContratadoPresupuestoAutorizado = 11;
                    principal.Principal.PorcentajePresupuestoAutorizadoObraPublica = 70; //(int)(((MontoContratadoObras) * 100) / GeneralMunicipios.presupuestoAutorizadoObras);
                    principal.Principal.PorcentajePresupuestoAutorizadoAdquisiciones = 65;  //(int)(((MontoContratadoAdquisiciones) * 100) / GeneralMunicipios.presupuestoAutorizadoAdquisiciones);
                }
                if (ejercicio == "2020")
                {
                    //principal.Principal.PorcentajeContratadoPresupuestoAutorizado = 0;  // (int)(((MontoContratadoAdquisiciones + MontoContratadoObras) * 100) / (GeneralMunicipios.presupuestoAutorizadoAdquisiciones + GeneralMunicipios.presupuestoAutorizadoObras));
                    principal.Principal.PorcentajePresupuestoAutorizadoObraPublica = 0; //(int)(((MontoContratadoObras) * 100) / GeneralMunicipios.presupuestoAutorizadoObras);
                    principal.Principal.PorcentajePresupuestoAutorizadoAdquisiciones = 0;  //(int)(((MontoContratadoAdquisiciones) * 100) / GeneralMunicipios.presupuestoAutorizadoAdquisiciones);
                }*/

                principal.Principal.PresupuestoAutorizadoAdquisiciones = GeneralMunicipios.presupuestoAutorizadoAdquisiciones;
                principal.Principal.PresupuestoAutorizadoAdquisiciones_Texto = GeneralMunicipios.presupuestoAutorizadoAdquisiciones.ToString("C", CultureInfo.CurrentCulture);
                principal.Principal.PresupuestoAutorizadoObraPublica = GeneralMunicipios.presupuestoAutorizadoObras;
                principal.Principal.PresupuestoAutorizadoObraPublica_Texto = GeneralMunicipios.presupuestoAutorizadoObras.ToString("C", CultureInfo.CurrentCulture);


                //Grafica de Adquisiciones
                principal.Principal.MontoAdquisicionesContratadas = MontoContratadoAdquisiciones;  //***
                principal.Principal.MontoAdquisicionesContratadasComplemento = GeneralMunicipios.presupuestoAutorizadoAdquisiciones - MontoContratadoAdquisiciones;
                
                principal.Principal.MontoAdquisicionesPagadas = MontoContratadoAdquisiciones;
                principal.Principal.MontoAdquisicionesPagadasComplemento = GeneralMunicipios.presupuestoAutorizadoAdquisiciones - MontoContratadoAdquisiciones;

                //Grafica de Obras
                principal.Principal.MontoObrasContratadas = MontoContratadoObras;
                principal.Principal.MontoObrasContratadasComplemento = GeneralMunicipios.presupuestoAutorizadoObras - MontoContratadoObras;

                principal.Principal.MontoObrasPagadas = MontoPagadoObras;
                principal.Principal.MontoObrasPagadasComplemento = GeneralMunicipios.presupuestoAutorizadoObras - MontoPagadoObras;

                //principal.Principal.ObrasConProblemas = ExpedientesObras.Where(x => x.estatus == "REVISADO CON OBSERVACIONES").Count();
                //principal.Principal.ObrasContratadas = ExpedientesObras.Count();
                //principal.Principal.ObrasPresupuestadas = ExpedientesObras.Count();
                //principal.Principal.PresupuestoAutorizadoObras = GeneralMunicipios.presupuestoAutorizadoObras;


                principal.Principal.montoEjercidoCabezara = String.Format("{0,12:C2}", Math.Round((MontoContratadoAdquisiciones + MontoPagadoObras) ,2));
                principal.Principal.montoContratodoCabezera = String.Format("{0,12:C2}", Math.Round((MontoContratadoAdquisiciones + MontoContratadoObras), 2));

                principal.Principal.montoContratoAdquisiciones = String.Format("{0,12:C2}", Math.Round(MontoContratadoAdquisiciones , 2));
                principal.Principal.montoContratoObraPublica = String.Format("{0,12:C2}", Math.Round(MontoContratadoObras, 2));
                principal.Principal.montoAsignado = String.Format("{0,12:C2}", Math.Round(MontoAsignado, 2));                
                principal.Principal.montoEjercido = String.Format("{0,12:C2}", Math.Round(MontoPagadoObras, 2));

                principal.Principal.PorcentajeAdquisiciones = Math.Round((MontoContratadoAdquisiciones / MontoContratadoAdquisiciones) * 100,2);
                principal.Principal.PorcentajeObraPublica = Math.Round((MontoPagadoObras / MontoContratadoObras) * 100,2);

                principal.success = true;
                principal.messages.Add("Respuesta con exito");

                return Ok(principal);
            }
            catch (Exception ex)
            {

                principal.success = false;
                principal.messages.Add(ex.ToString());
                return Ok(principal);
            }

        }

        [Authorize(Roles = "Ejecutivo")]
        [HttpGet]
        [Route("api/dashboard/global")]
        public async System.Threading.Tasks.Task<IHttpActionResult> ShowDashboardGlobal()
        {
            var idUsername = string.Empty;
            var role = string.Empty;
            var identity = (ClaimsIdentity)User.Identity;
            IEnumerable<Claim> claims = identity.Claims;

            var roleClaimType = identity.RoleClaimType;
            var roles = claims.Where(c => c.Type == ClaimTypes.Role).ToList();
            string municipio = string.Empty;

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
            var Client = new MongoClient(constr);
            var DB = Client.GetDatabase("PRB");

            ResponseInformationGlobal estadisticas = new ResponseInformationGlobal();


            try
            {
                #region Informacion Global

                string ejercicio = string.Empty;
                int expedientes = 0;
                decimal montoAdjudicado = 0;
                List<InformationGlobal> listGlobalAdqu = new List<InformationGlobal>();
                InformationGlobal global = new InformationGlobal();
                decimal montoGlobal = 0;
                int expedientesGlobal = 0;

                try
                {
                    var collAdqui = DB.GetCollection<AdquisicionesV1>("Adquisiciones");
                    var filterAdqui = Builders<AdquisicionesV1>.Filter.Eq(x => x.municipio, municipio);
                    var adquisiciones = await collAdqui.Find(filterAdqui).Sort("{ejercicio:1}").ToListAsync();

                    if (adquisiciones.Count > 0)
                    {
                        foreach(var data in adquisiciones)
                        {
                            if(string.IsNullOrEmpty(ejercicio))
                            {
                                ejercicio = data.ejercicio;
                                expedientes++;
                                montoAdjudicado = montoAdjudicado + Convert.ToDecimal(data.montoAdjudicado);
                            }
                            else
                            {
                                if(ejercicio.Equals(data.ejercicio))
                                {
                                    expedientes++;
                                    montoAdjudicado = montoAdjudicado + Convert.ToDecimal(data.montoAdjudicado);
                                }
                                else
                                {
                                    global = new InformationGlobal();
                                    global.ejercicio = ejercicio;
                                    global.expedientes = expedientes.ToString();
                                    global.monto = montoAdjudicado.ToString("###,##0.##");
                                    listGlobalAdqu.Add(global);

                                    montoGlobal = montoGlobal + montoAdjudicado;
                                    expedientesGlobal = expedientesGlobal + expedientes;
                                    expedientes = 0;
                                    montoAdjudicado = 0;
                                    ejercicio = data.ejercicio;
                                    expedientes++;
                                    montoAdjudicado = montoAdjudicado + Convert.ToDecimal(data.montoAdjudicado);
                                }
                            }
                        }
                        montoGlobal = montoGlobal + montoAdjudicado;
                        expedientesGlobal = expedientesGlobal + expedientes;

                        global = new InformationGlobal();
                        global.ejercicio = ejercicio;
                        global.expedientes = expedientes.ToString();
                        global.monto = montoAdjudicado.ToString("###,##0.##");
                        listGlobalAdqu.Add(global);

                        montoGlobal = montoGlobal + montoAdjudicado;
                        global = new InformationGlobal();
                        global.ejercicio = "Totales";
                        global.expedientes = expedientesGlobal.ToString();
                        global.monto = montoGlobal.ToString("###,##0.##");
                        listGlobalAdqu.Add(global);


                    }                    
                }
                catch (Exception ex)
                {
                    //response.success = false;
                    //response.messages.Add(ex.ToString());
                    //return Ok(ex.ToString());
                }

                estadisticas.InformationGlobalAdquisiciones = listGlobalAdqu;

                ejercicio = string.Empty;
                expedientes = 0;
                montoAdjudicado = 0;

                decimal montoContrato = 0;
                decimal montoAsignado = 0;                
                decimal montoEjercido = 0;

                montoGlobal = 0;
                decimal montoGlobalEjercido = 0;
                expedientesGlobal = 0;

                List<InformationGlobal> listGlobalObra = new List<InformationGlobal>();
                List<InformationGlobal> listGlobalObraEjercido = new List<InformationGlobal>();
                global = new InformationGlobal();

                try
                {
                    var collObra = DB.GetCollection<ObraPublicaV1>("ObraPublica");
                    var filterObra = Builders<ObraPublicaV1>.Filter.Eq(x => x.municipio, municipio);
                    var obras = await collObra.Find(filterObra).Sort("{ejercicio:1}").ToListAsync();

                    if (obras.Count > 0)
                    {
                        foreach (var data in obras)
                        {

                            if (string.IsNullOrEmpty(ejercicio))
                            {
                                ejercicio = data.ejercicio;
                                expedientes++;
                                montoContrato = montoContrato + Convert.ToDecimal(data.montoContrato);
                                montoEjercido = montoEjercido + Convert.ToDecimal(data.montoEjercido);
                            }
                            else
                            {
                                if (ejercicio.Equals(data.ejercicio))
                                {
                                    expedientes++;
                                    montoContrato = montoContrato + Convert.ToDecimal(data.montoContrato);
                                    montoEjercido = montoEjercido + Convert.ToDecimal(data.montoEjercido);
                                }
                                else
                                {                                    
                                    global = new InformationGlobal();
                                    global.ejercicio = ejercicio;
                                    global.expedientes = expedientes.ToString();
                                    global.monto = montoContrato.ToString("###,##0.##");
                                    listGlobalObra.Add(global);

                                    global = new InformationGlobal();
                                    global.ejercicio = ejercicio;
                                    global.expedientes = expedientes.ToString();
                                    global.monto = montoEjercido.ToString("###,##0.##");
                                    listGlobalObraEjercido.Add(global);

                                    montoGlobal = montoGlobal + montoContrato;
                                    expedientesGlobal = expedientesGlobal + expedientes;
                                    expedientes = 0;
                                    montoContrato = 0;
                                    ejercicio = data.ejercicio;
                                    expedientes++;
                                    montoContrato = montoContrato + Convert.ToDecimal(data.montoContrato);

                                    //montoGlobalEjercido
                                    montoGlobalEjercido = montoGlobalEjercido + montoEjercido;
                                    //expedientesGlobal = expedientesGlobal + expedientes;
                                    //expedientes = 0;
                                    montoEjercido = 0;
                                    //ejercicio = data.ejercicio;
                                    //expedientes++;
                                    montoEjercido = montoEjercido + Convert.ToDecimal(data.montoEjercido);

                                }
                            }
                        }

                        montoGlobal = montoGlobal + montoContrato;
                        expedientesGlobal = expedientesGlobal + expedientes;

                        global = new InformationGlobal();
                        global.ejercicio = ejercicio;
                        global.expedientes = expedientes.ToString();
                        global.monto = montoContrato.ToString("###,##0.##");
                        listGlobalObra.Add(global);

                        global = new InformationGlobal();
                        global.ejercicio = ejercicio;
                        global.expedientes = expedientes.ToString();
                        global.monto = montoEjercido.ToString("###,##0.##");
                        listGlobalObraEjercido.Add(global);

                        montoGlobalEjercido = montoGlobalEjercido + montoEjercido;

                        global = new InformationGlobal();
                        global.ejercicio = "Totales";
                        global.expedientes = expedientesGlobal.ToString();
                        global.monto = montoGlobal.ToString("###,##0.##");
                        listGlobalObra.Add(global);


                        global = new InformationGlobal();
                        global.ejercicio = "Totales";
                        global.expedientes = expedientesGlobal.ToString();
                        global.monto = montoGlobalEjercido.ToString("###,##0.##");
                        listGlobalObraEjercido.Add(global);


                    }
                }
                catch (Exception ex)
                {
                    //response.success = false;
                    //response.messages.Add(ex.ToString());
                    //return Ok(ex.ToString());
                }

                estadisticas.InformationGlobalObras = listGlobalObra;
                estadisticas.InformationGlobalObrasEjercido = listGlobalObraEjercido;

                #endregion

                #region Cake
                List<InformationCake> cake = new List<InformationCake>();
                int lastPosition = listGlobalAdqu.Count;
                int total = Convert.ToInt32(listGlobalAdqu[lastPosition -1].expedientes);
                Dictionary<string, string> anio = new Dictionary<string, string>();
                InformationCake information = new InformationCake();
                information.Anios = new List<string>();
                information.Expedientes = new List<string>();

                foreach (var data in listGlobalAdqu)
                {
                    if(data.ejercicio.Equals("Totales"))
                    {
                        break;
                    }
                    else
                    {
                        information.Anios.Add(data.ejercicio);
                        double porcentaje = ((Convert.ToInt32(data.expedientes) * 100.00) / total);
                        information.Expedientes.Add(Math.Round(porcentaje,2).ToString());
                    }
                }

                cake.Add(information);
                estadisticas.InformationCakeAdjudicaciones = cake;

                cake = new List<InformationCake>();
                lastPosition = listGlobalObra.Count;
                total = Convert.ToInt32(listGlobalObra[lastPosition - 1].expedientes);
                information = new InformationCake();
                information.Anios = new List<string>();
                information.Expedientes = new List<string>();

                foreach (var data in listGlobalObra)
                {
                    if (data.ejercicio.Equals("Totales"))
                    {
                        break;
                    }
                    else
                    {
                        information.Anios.Add(data.ejercicio);
                        double porcentaje = ((Convert.ToInt32(data.expedientes) * 100.00) / total);
                        information.Expedientes.Add(Math.Round(porcentaje, 2).ToString());
                    }
                }

                cake.Add(information);
                estadisticas.InformationCakeObras = cake;
                #endregion

                #region Line
                List<InformationCake> line = new List<InformationCake>();
                lastPosition = listGlobalAdqu.Count;
                total = Convert.ToInt32(listGlobalAdqu[lastPosition - 1].expedientes);
                
                information = new InformationCake();
                information.Anios = new List<string>();
                information.Expedientes = new List<string>();

                foreach (var data in listGlobalAdqu)
                {
                    if (data.ejercicio.Equals("Totales"))
                    {
                        break;
                    }
                    else
                    {
                        information.Anios.Add(data.ejercicio);
                        information.Expedientes.Add(data.expedientes);
                    }
                }

                line.Add(information);
                estadisticas.InformationAdjudicaciones = line;

                line = new List<InformationCake>();
                lastPosition = listGlobalObra.Count;
                total = Convert.ToInt32(listGlobalObra[lastPosition - 1].expedientes);
                information = new InformationCake();
                information.Anios = new List<string>();
                information.Expedientes = new List<string>();

                foreach (var data in listGlobalObra)
                {
                    if (data.ejercicio.Equals("Totales"))
                    {
                        break;
                    }
                    else
                    {
                        information.Anios.Add(data.ejercicio);
                        information.Expedientes.Add(data.expedientes);
                    }
                }

                line.Add(information);
                estadisticas.InformationObras = line;

                #endregion

                estadisticas.success = true;
                estadisticas.messages.Add("Respuesta con exito");

                return Ok(estadisticas);
            }
            catch (Exception ex)
            {
                estadisticas.success = false;
                estadisticas.messages.Add(ex.ToString());
                return Ok(estadisticas);
            }
        }
    }
}
