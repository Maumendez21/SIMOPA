using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Security.Claims;
using System.Web.Http;
using System.Configuration;
using MongoDB.Driver;
using credentialsPBR.Models.Expedientes.Adquisiciones;
using apiPBR.Models.Response.Expedientes.Adquisiciones;
using credentialsPBR.Models.Users;
using System.Security.Claims;
using Microsoft.Owin.Security.OAuth;
using credentialsPBR.Models.Expedientes.Utilerias;

namespace apiPBR.Controllers.Auditor
{
    public class CardAdquisicionesController : ApiController
    {

        GetUserInfo getUserInfo = new GetUserInfo();

        [Authorize(Roles = "Auditor, Coordinador, Ejecutivo")]
        [HttpGet]
        [Route("api/expedients/adquisiciones/cards/{ejercicio}")]
        public async System.Threading.Tasks.Task<IHttpActionResult> ShowCardsAsync(string ejercicio, [FromUri] string sortCard)
        {
            var idUsername = string.Empty;
            var role = string.Empty;
            var identity = (ClaimsIdentity)User.Identity;
            IEnumerable<Claim> claims = identity.Claims;

            var roleClaimType = identity.RoleClaimType;
            var roles = claims.Where(c => c.Type == ClaimTypes.Role).ToList();

            foreach(var r in roles)
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

            HeadersSanAndresCholulaLista listHeaderSanAndresCholula = new HeadersSanAndresCholulaLista
            {
                ListaHeadersSanAndresCholula = new List<headerSanAndresCholula>()
            };

            string constr = ConfigurationManager.AppSettings["connectionString"];
            
            try
            {
                MongoClient Client = new MongoClient(constr);

                UsersPRB userPBR = new UsersPRB();
                userPBR = await getUserInfo.GetInfoUserPBRAsync(idUsername, Client);

                var DB = Client.GetDatabase("PRB");
                var collection = DB.GetCollection<AdquisicionesV1>("Adquisiciones");


                var filter = Builders<AdquisicionesV1>.Filter.Eq("municipio", userPBR.municipio) &
                    Builders<AdquisicionesV1>.Filter.Eq(x => x.ejercicio, ejercicio);

                if (role == "Auditor")
                {
                    filter = Builders<AdquisicionesV1>.Filter.Eq(x=>x.auditor, userPBR.Id.ToString()) &
                    Builders<AdquisicionesV1>.Filter.Eq(x=>x.ejercicio, ejercicio);
                }
                
                var result = await collection.Find(filter).ToListAsync();

                List<headerSanAndresCholula> l_headerSanAndresCholulas = new List<headerSanAndresCholula>();
                foreach (var r in result)
                {
                    headerSanAndresCholula headerSanAndresCholula = new headerSanAndresCholula();

                    headerSanAndresCholula.Id = r.Id;
                    headerSanAndresCholula.montoAdjudicado = r.montoAdjudicado == null ? r.montoAdjudicado : String.Format("{0:n}", float.Parse(r.montoAdjudicado));
                    headerSanAndresCholula.numeroAdjudicacion = r.numeroAdjudicacion;
                    headerSanAndresCholula.numeroContrato = r.numeroContrato;
                    headerSanAndresCholula.objetoContrato = r.objetoContrato;
                    headerSanAndresCholula.tipoAdjudicacion = r.tipoAdjudicacion;
                    headerSanAndresCholula.ObservacionesGenerales = r.observacionesGenerales;
                    headerSanAndresCholula.estatusExpediente = r.estatusExpediente;
                    headerSanAndresCholula.proveedor = r.proveedorAdjudicado;

                    //GetInformationFromComplementarioExpedientes getInfoFromExpedientesComplementarios = new GetInformationFromComplementarioExpedientes();

                    //var info = await getInfoFromExpedientesComplementarios.InformacionComplementariaExpedientesPublicaAsync(r.Id, "adquisiciones");



                    int valorSI = r.documentos.Where(x => x.estatus == "SI").Count();
                    int valorNO = r.documentos.Where(x => x.estatus == "NO").Count();

                    int total = valorNO + valorSI;
                    
                    if (total == 0)
                    {
                        headerSanAndresCholula.porcentajeAvance = 0;
                    }
                    else
                    {
                        headerSanAndresCholula.porcentajeAvance = (valorSI * 100) / total;
                    }
                        


                    l_headerSanAndresCholulas.Add(headerSanAndresCholula);

                }

                if (sortCard == "desc")
                {
                    l_headerSanAndresCholulas.Sort((x, y) => x.numeroAdjudicacion.CompareTo(y.numeroAdjudicacion));
                }
                else
                {
                    l_headerSanAndresCholulas.Sort((x, y) => y.numeroAdjudicacion.CompareTo(x.numeroAdjudicacion));
                }


                listHeaderSanAndresCholula.success = true;
                listHeaderSanAndresCholula.messages.Add("Respuesta exitosa");
                listHeaderSanAndresCholula.ListaHeadersSanAndresCholula = l_headerSanAndresCholulas;



                return Ok(listHeaderSanAndresCholula);
            }
            catch(Exception ex)
            {
                listHeaderSanAndresCholula.success = false;
                listHeaderSanAndresCholula.messages.Add(ex.ToString());

                return Ok(listHeaderSanAndresCholula);
            }
            
        }
    }
}