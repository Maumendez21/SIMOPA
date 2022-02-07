using apiPBR.Models.Response.Expedientes.Adquisiciones;
using credentialsPBR.Models.Expedientes.Adquisiciones;
using credentialsPBR.Models.Expedientes.Utilerias;
using credentialsPBR.Models.Users;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace apiPBR.Controllers.Auditor
{
    public class DocumentsAdquisicionesController : ApiController
    {
        [Authorize(Roles = "Auditor, Coordinador, Ejecutivo")]
        [HttpGet]
        [Route("api/expedients/adquisiciones/documents")]
        public async System.Threading.Tasks.Task<IHttpActionResult> ShowDocumentsAsync(string id)
        {
            DocumentosSanAndresCholula DocumentosSanAndresCholula = new DocumentosSanAndresCholula
            {
                documentos = new List<DocumentosAdquisicionesSanAndresCholula>()
            };

            string constr = ConfigurationManager.AppSettings["connectionString"];

            try
            {
                var Client = new MongoClient(constr);

                var DB = Client.GetDatabase("PRB");
                var collection = DB.GetCollection<AdquisicionesV1>("Adquisiciones");

                var filter = Builders<AdquisicionesV1>.Filter.Eq(x=>x.Id, id);

                var result = await collection.Find(filter).FirstOrDefaultAsync();

                if (result != null)
                {
                    List<DocumentosAdquisicionesSanAndresCholula> l_documentosSanAndresCholulas = new List<DocumentosAdquisicionesSanAndresCholula>();

                    DocumentosSanAndresCholula.montoAdjudicado = result.montoAdjudicado == null ? result.montoAdjudicado : String.Format("{0:n}", float.Parse(result.montoAdjudicado));
                    DocumentosSanAndresCholula.montoAdjudicado = DocumentosSanAndresCholula.montoAdjudicado.Replace(",", "");
                    DocumentosSanAndresCholula.numeroAdjudicacion = result.numeroAdjudicacion;
                    DocumentosSanAndresCholula.numeroContrato = result.numeroContrato;
                    DocumentosSanAndresCholula.objetoContrato = result.objetoContrato;
                    DocumentosSanAndresCholula.tipoAdjudicacion = result.tipoAdjudicacion;
                    DocumentosSanAndresCholula.entidad = result.entidad;
                    DocumentosSanAndresCholula.origenRecurso = result.origenRecurso;
                    DocumentosSanAndresCholula.proveedorAdjudicado = result.proveedorAdjudicado;
                    DocumentosSanAndresCholula.status = 0;
                    DocumentosSanAndresCholula.ObservacionesGenerales = result.observacionesGenerales;
                    DocumentosSanAndresCholula.estatusExpediente = result.estatusExpediente;
                   
                    //GetInformationFromComplementarioExpedientes getInfoFromExpedientesComplementarios = new GetInformationFromComplementarioExpedientes();

                    //var info = await getInfoFromExpedientesComplementarios.InformacionComplementariaExpedientesPublicaAsync(id, "obrapublica");


                    foreach (var r in result.documentos)
                    {
                        DocumentosAdquisicionesSanAndresCholula documentos = new DocumentosAdquisicionesSanAndresCholula();

                        documentos.clave = r.clave;
                        documentos.comentario = r.comentario;
                        documentos.recomendacion = r.recomendacion;

                        documentos.estatus = (string.IsNullOrEmpty(r.estatus) ? "" : r.estatus);
                        if (documentos.estatus.Equals("SI"))
                        {
                            documentos.downloadLinkDocument = "https://apipbrdevelolop.azurewebsites.net/api/expedients/adownload?id=" + id + "&clave=" + r.clave;
                        }
                        else if (documentos.estatus.Equals("POR REVISAR"))
                        {
                            documentos.downloadLinkDocument = "https://apipbrdevelolop.azurewebsites.net/api/expedients/adownload?id=" + id + "&clave=" + r.clave;
                        }
                        else
                        {
                            documentos.downloadLinkDocument = "";
                        }

                        documentos.documento = r.documento;
                        documentos.grupo = r.grupo;
                        //documentos.fundamento = (new Common.Common()).Fundamentos(DocumentosSanAndresCholula.tipoAdjudicacion, r.documento);
                        documentos.fundamento = (new Common.Common()).FundamentosAdquisiciones(result.municipio, DocumentosSanAndresCholula.tipoAdjudicacion, r.documento.Trim());

                        l_documentosSanAndresCholulas.Add(documentos);
                    }

                    List<DocumentosAdquisicionesSanAndresCholula> listDoc = new List<DocumentosAdquisicionesSanAndresCholula>();
                    listDoc = l_documentosSanAndresCholulas;

                    Sort(ref listDoc, "clave", "");

                    DocumentosSanAndresCholula.documentos = listDoc;


                    //Nombre del Auditor
                    if (!result.auditor.Equals(""))
                    {
                        var con = Client.GetDatabase("PRB");
                        var coll = con.GetCollection<UsersPRB>("UsersPBR");

                        var filterUsr = Builders<UsersPRB>.Filter.Eq(x => x.Id, result.auditor.Trim());
                        var resultUsr = await coll.Find(filterUsr).FirstOrDefaultAsync();

                        if (resultUsr != null)
                        {
                            DocumentosSanAndresCholula.NombreAuditor = resultUsr.name;
                        }
                    }


                    DocumentosSanAndresCholula.success = true;
                    DocumentosSanAndresCholula.messages.Add("Respuesta exitosa");
                    //DocumentosSanAndresCholula.documentos = l_documentosSanAndresCholulas;

                    return Ok(DocumentosSanAndresCholula);
                }
                else
                {
                    DocumentosSanAndresCholula.success = false;
                    DocumentosSanAndresCholula.messages.Add("No se encontró el registro");

                    return Ok(DocumentosSanAndresCholula);
                }

                
            }
            catch (Exception ex)
            {
                DocumentosSanAndresCholula.success = false;
                DocumentosSanAndresCholula.messages.Add(ex.ToString());

                return Ok(DocumentosSanAndresCholula);
            }

        }

        private void Sort(ref List<DocumentosAdquisicionesSanAndresCholula> list, string sortBy, string sortDirection)
        {
            if (sortBy == "clave")
            {
                list = list.OrderBy(x => x.clave).ToList<DocumentosAdquisicionesSanAndresCholula>();
            }
        }

    }
}
