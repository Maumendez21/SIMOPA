using apiPBR.Models.Response.Expedientes.ObraPublica;
using credentialsPBR.Models.Expedientes.ObraPublica;
using credentialsPBR.Models.Expedientes.Utilerias;
using credentialsPBR.Models.Users;
using MongoDB.Driver;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace apiPBR.Controllers.Auditor.ObraPublica
{
    public class CardObraPublicaV1Controller : ApiController
    {
        GetUserInfo getUserInfo = new GetUserInfo();

        [Authorize(Roles = "Auditor, Coordinador, Ejecutivo")]
        [HttpGet]
        [Route("api/expedients/obrapublica/cards/{ejercicio}")]
        public async System.Threading.Tasks.Task<IHttpActionResult> ShowCardsAsync(string ejercicio,[FromUri] string sortCard)
        {
            var idUsername = string.Empty;
            var role = string.Empty;
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
            }

            headerObraPublicaV1 listHeaderObraPublicaV1 = new headerObraPublicaV1
            {
                headerObraPublicaV1s = new List<headerCardObraPublicaV1>()
            };

            string constr = ConfigurationManager.AppSettings["connectionString"];

            try
            {
                MongoClient Client = new MongoClient(constr);

                UsersPRB userPBR = new UsersPRB();
                userPBR = await getUserInfo.GetInfoUserPBRAsync(idUsername, Client);

                var DB = Client.GetDatabase("PRB");
                var collection = DB.GetCollection<ObraPublicaV1>("ObraPublica");


                var filter = Builders<ObraPublicaV1>.Filter.Eq("municipio", userPBR.municipio) &
                    Builders<ObraPublicaV1>.Filter.Eq(x => x.ejercicio, ejercicio); ;

                if (role == "Auditor")
                {
                    filter = Builders<ObraPublicaV1>.Filter.Eq("auditor", userPBR.Id.ToString()) &
                    Builders<ObraPublicaV1>.Filter.Eq(x=>x.ejercicio, ejercicio);
                }

                var result = await collection.Find(filter).ToListAsync();

                List<headerCardObraPublicaV1> l_headerObraPublicaV1 = new List<headerCardObraPublicaV1>();
                foreach (var r in result)
                {
                    headerCardObraPublicaV1 headerObraPublicaV1 = new headerCardObraPublicaV1();

                    headerObraPublicaV1.Id = r.Id;
                    headerObraPublicaV1.procedimiento = r.procedimiento;
                    headerObraPublicaV1.programa = r.programa;
                    headerObraPublicaV1.numeroContrato = r.numeroContrato;
                    headerObraPublicaV1.ejecutor = r.ejecutor;
                    headerObraPublicaV1.tipoAdjudicacion = r.tipoAdjudicacion;
                    headerObraPublicaV1.fechaRevision = r.fechaRevision;
                    headerObraPublicaV1.localidad = r.localidad;
                    headerObraPublicaV1.nombreObra = r.nombreObra;
                    headerObraPublicaV1.numeroObra = r.numeroObra == null ? r.numeroObra = "-" : r.numeroObra;
                    headerObraPublicaV1.modalidad = r.modalidad;
                    headerObraPublicaV1.tipoContrato = r.tipoContrato;
                    headerObraPublicaV1.tipoProveedor = r.tipoProveedor;
                    headerObraPublicaV1.proveedor = r.proveedor;
                    headerObraPublicaV1.numeroProcedimiento = r.numeroProcedimiento;
                    headerObraPublicaV1.fechaProcedimiento = r.fechaProcedimiento;
                    //headerObraPublicaV1.montoAsignado = r.montoAsignado;
                    //headerObraPublicaV1.montoContrato =  r.montoContrato;
                    //headerObraPublicaV1.montoEjercido = r.montoEjercido;
                    headerObraPublicaV1.ejecucionInicio = r.ejecucionInicio;
                    headerObraPublicaV1.ejecucionTermino = r.ejecucionTermino;
                    headerObraPublicaV1.ejecucionPeriodo = r.ejecucionPeriodo;
                    headerObraPublicaV1.estatusExpediente = r.estatusExpediente;

                    headerObraPublicaV1.montoContrato = r.montoContrato == null ? r.montoContrato : String.Format("{0:n}", float.Parse(r.montoContrato));
                    headerObraPublicaV1.montoAsignado = r.montoAsignado == null ? r.montoAsignado : String.Format("{0:n}", float.Parse(r.montoAsignado));
                    headerObraPublicaV1.montoEjercido = r.montoEjercido == null ? r.montoEjercido : String.Format("{0:n}", float.Parse(r.montoEjercido));

                    if (r.Location != null)
                    {
                        if(r.Location.Coordinates != null)
                        {
                            if (r.Location.Coordinates.Count > 1)
                            {
                                headerObraPublicaV1.latitud = r.Location.Coordinates[1].ToString();
                                headerObraPublicaV1.longitud = r.Location.Coordinates[0].ToString();
                            }
                        }
                        else
                        {
                            headerObraPublicaV1.latitud = null;
                            headerObraPublicaV1.longitud = null;
                        }
                    }
                    else
                    {
                        headerObraPublicaV1.latitud = null;
                        headerObraPublicaV1.longitud = null;
                    }


                    int valorSI = r.documentos.Where(x => x.integracion == "SI").Count();
                    int valorNO = r.documentos.Where(x => x.integracion == "NO").Count();

                    int total = valorNO + valorSI;
                    headerObraPublicaV1.porcentajeAvance = (valorSI * 100) / total;

                    l_headerObraPublicaV1.Add(headerObraPublicaV1);

                }

                if (sortCard == "desc")
                {
                    l_headerObraPublicaV1.Sort((x, y) => x.numeroObra.CompareTo(y.numeroObra));
                }
                else
                {
                    l_headerObraPublicaV1.Sort((x, y) => y.numeroObra.CompareTo(x.numeroObra));
                }
                

                listHeaderObraPublicaV1.success = true;
                listHeaderObraPublicaV1.messages.Add("Respuesta exitosa");
                listHeaderObraPublicaV1.headerObraPublicaV1s = l_headerObraPublicaV1;





                return Ok(listHeaderObraPublicaV1);
            }
            catch (Exception ex)
            {
                listHeaderObraPublicaV1.success = false;
                listHeaderObraPublicaV1.messages.Add(ex.ToString());

                return Ok(listHeaderObraPublicaV1);
            }

        }
    }


}
