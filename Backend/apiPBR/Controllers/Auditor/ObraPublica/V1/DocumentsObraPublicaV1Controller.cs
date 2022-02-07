using apiPBR.Models.Response.Expedientes.ObraPublica;
using credentialsPBR.Models.Expedientes.ObraPublica;
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

namespace apiPBR.Controllers.Auditor.ObraPublica.V1
{
    public class DocumentsObraPublicaV1Controller : ApiController
    {
        [Authorize(Roles = "Auditor, Coordinador, Ejecutivo")]
        [HttpGet]
        [Route("api/expedients/obrapublica/documents")]
        public async System.Threading.Tasks.Task<IHttpActionResult> ShowDocumentsAsync(string id)
        {
            DocumentosDetalleObraPublicaV1 listDocumentsObraPublicaV1 = new DocumentosDetalleObraPublicaV1
            {
                documentosObraPublicaV1 = new List<DocumentosObraPublicaV1>()
            };

            string constr = ConfigurationManager.AppSettings["connectionString"];

            try
            {
                var Client = new MongoClient(constr);

                var DB = Client.GetDatabase("PRB");
                var collection = DB.GetCollection<ObraPublicaV1>("ObraPublica");

                var filter = Builders<ObraPublicaV1>.Filter.Eq(x => x.Id, id);

                var result = await collection.Find(filter).FirstOrDefaultAsync();

                if (result != null)
                {
                    

                    listDocumentsObraPublicaV1.Id = result.Id;
                    listDocumentsObraPublicaV1.procedimiento = result.procedimiento;
                    listDocumentsObraPublicaV1.programa = result.programa;
                    listDocumentsObraPublicaV1.numeroContrato = result.numeroContrato;
                    listDocumentsObraPublicaV1.ejecutor = result.ejecutor;
                    listDocumentsObraPublicaV1.tipoAdjudicacion = result.tipoAdjudicacion;
                    listDocumentsObraPublicaV1.fechaRevision = result.fechaRevision;
                    listDocumentsObraPublicaV1.localidad = result.localidad;
                    listDocumentsObraPublicaV1.nombreObra = result.nombreObra;
                    listDocumentsObraPublicaV1.modalidad = result.modalidad;
                    listDocumentsObraPublicaV1.tipoContrato = result.tipoContrato;
                    listDocumentsObraPublicaV1.tipoProveedor = result.tipoProveedor;
                    listDocumentsObraPublicaV1.proveedor = result.proveedor;
                    listDocumentsObraPublicaV1.numeroProcedimiento = result.numeroProcedimiento;
                    listDocumentsObraPublicaV1.fechaProcedimiento = result.fechaProcedimiento;
                    listDocumentsObraPublicaV1.montoAsignado = result.montoAsignado;
                    listDocumentsObraPublicaV1.montoContrato = result.montoContrato == null ? result.montoContrato : String.Format("{0:n}", float.Parse(result.montoContrato));
                    listDocumentsObraPublicaV1.montoEjercido = result.montoEjercido;
                    listDocumentsObraPublicaV1.ejecucionInicio = result.ejecucionInicio;
                    listDocumentsObraPublicaV1.ejecucionTermino = result.ejecucionTermino;
                    listDocumentsObraPublicaV1.ejecucionPeriodo = result.ejecucionPeriodo;
                    listDocumentsObraPublicaV1.estatusExpediente = result.estatusExpediente;
                    listDocumentsObraPublicaV1.numeroObra = result.numeroObra;
                    listDocumentsObraPublicaV1.responsable = result.responsable;
                    listDocumentsObraPublicaV1.proyecto = result.proyecto;
                    listDocumentsObraPublicaV1.fechaContrato = result.fechaContrato;
                    listDocumentsObraPublicaV1.observacionesGenerales = result.ObservacionesGenerales;

                  

                    //GetInformationFromComplementarioExpedientes getInfoFromExpedientesComplementarios = new GetInformationFromComplementarioExpedientes();

                    //var info = await getInfoFromExpedientesComplementarios.InformacionComplementariaExpedientesPublicaAsync(result.Id, "obrapublica");

                    if (result.Location != null)
                    {
                        if (result.Location.Coordinates != null)
                        {
                            if (result.Location.Coordinates.Count > 1)
                            {
                                listDocumentsObraPublicaV1.latitud = result.Location.Coordinates[1].ToString();
                                listDocumentsObraPublicaV1.longitud = result.Location.Coordinates[0].ToString();
                            }
                        }
                        else
                        {
                            listDocumentsObraPublicaV1.latitud = null;
                            listDocumentsObraPublicaV1.longitud = null;
                        }
                    }
                    else
                    {
                        listDocumentsObraPublicaV1.latitud = null;
                        listDocumentsObraPublicaV1.longitud = null;
                    }

                    //GetInformationFromComplementarioExpedientes getInfoFromExpedientesComplementarios = new GetInformationFromComplementarioExpedientes();

                    //var info = await getInfoFromExpedientesComplementarios.InformacionComplementariaExpedientesPublicaAsync(id, "obrapublica");


                    List<DocumentosObraPublicaV1> lDocumentosObraV1 = new List<DocumentosObraPublicaV1>();

                    foreach (var r in result.documentos)
                    {
                        DocumentosObraPublicaV1 documentos = new DocumentosObraPublicaV1();

                        documentos.clave = r.clave;
                        documentos.comentario = r.observaciones;
                        
                        //documentos.estatus = r.integracion;
                        documentos.estatus = (string.IsNullOrEmpty(r.integracion) ? "" : r.integracion);
                        if (documentos.estatus == "SI" || documentos.estatus == "DOC. ERRONEO")
                        {
                            documentos.downloadLinkDocument = "https://apipbrdevelolop.azurewebsites.net/api/expedients/obrapublica/adownload?id=" + id + "&clave=" + r.clave;
                        }
                        else if (documentos.estatus == "POR REVISAR")
                        {
                            documentos.downloadLinkDocument = "https://apipbrdevelolop.azurewebsites.net/api/expedients/obrapublica/adownload?id=" + id + "&clave=" + r.clave;
                        }
                        else
                        {
                            documentos.downloadLinkDocument = "";
                        }

                        documentos.documento = r.documento;
                        documentos.grupo = r.grupo;
                        documentos.recomendacion = r.recomendacion;
                        documentos.fundamento = (new Common.Common()).FundamentosObra(r.documento);
                        //documentos.fundamento = (new Common.Common()).FundamentosAdquisiciones(result.municipio, DocumentosSanAndresCholula.tipoAdjudicacion, r.documento);

                        lDocumentosObraV1.Add(documentos);
                    }

                    List<DocumentosObraPublicaV1> listDoc = new List<DocumentosObraPublicaV1>();
                    listDoc = lDocumentosObraV1;

                    Sort(ref listDoc, "clave", "");

                    listDocumentsObraPublicaV1.documentosObraPublicaV1 = listDoc;

                    //Nombre del Auditor
                    if (!result.auditor.Equals(""))
                    {
                        var coll = DB.GetCollection<UsersPRB>("UsersPBR");

                        var filterUsr = Builders<UsersPRB>.Filter.Eq(x => x.Id, result.auditor);
                        var resultUsr = await coll.Find(filterUsr).FirstOrDefaultAsync();

                        if (resultUsr != null)
                        {
                            listDocumentsObraPublicaV1.NombreAuditor = resultUsr.name;
                        }
                    }

                    listDocumentsObraPublicaV1.success = true;
                    listDocumentsObraPublicaV1.messages.Add("Respuesta exitosa");
                    //listDocumentsObraPublicaV1.documentosObraPublicaV1 = lDocumentosObraV1;



                    return Ok(listDocumentsObraPublicaV1);
                }
                else
                {
                    listDocumentsObraPublicaV1.success = false;
                    listDocumentsObraPublicaV1.messages.Add("No se encontró el registro");

                    return Ok(listDocumentsObraPublicaV1);
                }


            }
            catch (Exception ex)
            {
                listDocumentsObraPublicaV1.success = false;
                listDocumentsObraPublicaV1.messages.Add(ex.ToString());

                return Ok(listDocumentsObraPublicaV1);
            }

        }

        private void Sort(ref List<DocumentosObraPublicaV1> list, string sortBy, string sortDirection)
        {
            if (sortBy == "clave")
            {
                list = list.OrderBy(x => x.clave).ToList<DocumentosObraPublicaV1>();
            }
        }

    }
}
