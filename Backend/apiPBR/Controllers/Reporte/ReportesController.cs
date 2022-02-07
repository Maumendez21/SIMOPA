using apiPBR.Models.Response.Expedientes.Adquisiciones;
using credentialsPBR.Models.Expedientes.Adquisiciones;
using credentialsPBR.Models.Expedientes.ObraPublica;
using credentialsPBR.Models.Expedientes.Utilerias;
using credentialsPBR.Models.Users;
using ICSharpCode.SharpZipLib.Zip;
using MongoDB.Driver;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Hosting;
using System.Web.Http;

namespace apiPBR.Controllers.Reporte
{
    public class ReportesController : ApiController
    {

        private string path = HostingEnvironment.MapPath("/TempFiles/");

        /// BackLog 703 - Task 974
        /// <summary>
        /// Cedula de Recomendaciones y Observaciones
        /// </summary>
        /// <param name="tipoexpediente"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("api/reporte/{tipoexpediente}/cedula/observaciones/recomendaciones/{id}")]
        public async System.Threading.Tasks.Task<HttpResponseMessage> DescargaReporteCedulaObservacionesRecomendaciones(string tipoexpediente, string id)
        {
            //5e9f5aa3d066036b046ac966 adjquisiciones
            //5e6b246c29e31517f4d48397 obra
            MemoryStream memoryStream = new MemoryStream();

            WebClient client = new WebClient();
            try
            {
                //memoryStream = new MemoryStream(client.DownloadData("C:/Simopa/Templete/TemplateCedulaRecomendacionObservacion.xlsx"));
                memoryStream = new MemoryStream(client.DownloadData("https://apipbrdevelolop.azurewebsites.net/templates/reportes/TemplateCedulaRecomendacionObservacion.xlsx"));

            }
            finally
            {
                client.Dispose();
            }

            string nombreObra = string.Empty;
            string noObra = string.Empty;
            string noContrato = string.Empty;
            string montoContrato = string.Empty;
            string proveedor = string.Empty;
            string fechaVisita = string.Empty;
            string idExpediente = string.Empty;
            string tipoAdjudicacion = string.Empty;
            string situacionActual = string.Empty;
            string problematica = string.Empty;

            string entidad = string.Empty;
            string noAdjudicacion = string.Empty;
            string observacionesGral = string.Empty;
            List<DocumentoAdquisicionesV1> listDoc = new List<DocumentoAdquisicionesV1>();
            List<Observaciones> listObs = new List<Observaciones>();
            List<DocumentosObrasV1> listObra = new List<DocumentosObrasV1>();

            string constr = ConfigurationManager.AppSettings["connectionString"];
            var Client = new MongoClient(constr);
            var DB = Client.GetDatabase("PRB");

            bool obra = false;

            string municipio = string.Empty;
            string encabezado = string.Empty;
            string ejercicio = string.Empty;

            if (tipoexpediente.Equals("A"))
            {
                try
                {
                    var collAdqui = DB.GetCollection<AdquisicionesV1>("Adquisiciones");
                    var filterAdqui = Builders<AdquisicionesV1>.Filter.Eq(x => x.Id, id);
                    var adquisiciones = await collAdqui.Find(filterAdqui).FirstOrDefaultAsync();

                    if (adquisiciones != null)
                    {
                        encabezado = "NUMERO DE ADJUDICACIÓN";
                        ejercicio = adquisiciones.ejercicio;
                        municipio = adquisiciones.municipio;
                        nombreObra = adquisiciones.objetoContrato;
                        tipoAdjudicacion = adquisiciones.tipoAdjudicacion;
                        entidad = adquisiciones.entidad;
                        noContrato = adquisiciones.numeroContrato;
                        noAdjudicacion = adquisiciones.numeroAdjudicacion;
                        proveedor = adquisiciones.proveedorAdjudicado;
                        montoContrato = adquisiciones.montoAdjudicado;
                        observacionesGral = adquisiciones.observacionesGenerales;
                        listDoc = adquisiciones.documentos;
                    }
                    else
                    {
                        obra = true;
                    }
                }
                catch (Exception ex)
                {
                    obra = true;
                    //response.success = false;
                    //response.messages.Add(ex.ToString());
                    //return Ok(ex.ToString());
                }
            }
            else if (tipoexpediente.Equals("O"))
            {
                try
                {

                    var collObra = DB.GetCollection<ObraPublicaV1>("ObraPublica");
                    var filterObra = Builders<ObraPublicaV1>.Filter.Eq(x => x.Id, id);
                    var obras = await collObra.Find(filterObra).FirstOrDefaultAsync();

                    if (obras != null)
                    {
                        encabezado = "NUMÉRO DE OBRA";
                        ejercicio = obras.ejercicio;
                        municipio = obras.municipio;
                        nombreObra = obras.nombreObra;
                        tipoAdjudicacion = obras.tipoAdjudicacion;
                        entidad = string.Empty;
                        noContrato = obras.numeroContrato;
                        noAdjudicacion = obras.numeroProcedimiento;
                        proveedor = obras.proveedor;
                        montoContrato = obras.montoContrato;
                        observacionesGral = obras.ObservacionesGenerales;
                        listObra = obras.documentos;
                        obra = true;
                    }
                }
                catch (Exception ex)
                {
                    //response.success = false;
                    //response.messages.Add(ex.ToString());
                    //return Ok(ex.ToString());
                }
            }

            //Imagen           
            int Height = 0;
            int Width = 0;
            int rowIndex = 0;
            int colIndex = 0;
            string nameImage = string.Empty;

            WebClient clientImage = new WebClient();
            MemoryStream memoryImage = new MemoryStream();
            try
            {
                if (municipio.Equals("TEPEACA"))
                {
                    //memoryImage = new MemoryStream(clientImage.DownloadData("C:/Simopa/Templete/Logo/LogoTepeaca.png"));
                    memoryImage = new MemoryStream(client.DownloadData("https://apipbrdevelolop.azurewebsites.net/templates/logo/LogoTepeaca.png"));
                    nameImage = "Tepeaca";
                    Height = 230;
                    Width = 340;
                    rowIndex = 8;
                    colIndex = 1;

                }
                else if (municipio.Equals("SAN ANDRES CHOLULA"))
                {
                    //memoryImage = new MemoryStream(clientImage.DownloadData("C:/Simopa/Templete/Logo/LogoDespacho.png"));
                    memoryImage = new MemoryStream(client.DownloadData("https://apipbrdevelolop.azurewebsites.net/templates/logo/LogoDespacho.png"));
                    nameImage = "ESACH";
                    Height = 230;
                    Width = 340;
                    rowIndex = 8;
                    colIndex = 1;
                }
                else
                {
                    //memoryImage = new MemoryStream(clientImage.DownloadData("C:/Simopa/Templete/Logo/LogoDespacho.png"));
                    memoryImage = new MemoryStream(client.DownloadData("https://apipbrdevelolop.azurewebsites.net/templates/logo/LogoDespacho.png"));
                }
            }
            catch(Exception ex)
            {
                clientImage.Dispose();
            }

            MemoryStream ms = new MemoryStream();
            //Envia datos a Excel
            using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {

                ExcelWorkbook excelWorkBook = excelPackage.Workbook;
                ExcelWorksheet excelWorksheet = excelWorkBook.Worksheets[1];

                //Imagen
                Image img = Image.FromStream(memoryImage);
                ExcelPicture pic = excelWorksheet.Drawings.AddPicture("imageVista-" + nameImage, img);
                pic.SetPosition(rowIndex, 0, colIndex + ((1 - 1) * 4), 0);
                pic.SetSize(Width, Height);

                //Datos
                //Ejercicio
                excelWorksheet.Cells[8, 2].Value = "EJERCICIO " + ejercicio;
                //Encabezado
                excelWorksheet.Cells[11, 6].Value = encabezado;

                excelWorksheet.Cells[10, 4].Value = tipoAdjudicacion;
                excelWorksheet.Cells[10, 7].Value = nombreObra;

                excelWorksheet.Cells[12, 4].Value = entidad;
                excelWorksheet.Cells[12, 6].Value = noAdjudicacion;
                excelWorksheet.Cells[12, 7].Value = noContrato;
                excelWorksheet.Cells[12, 8].Value = proveedor;
                excelWorksheet.Cells[12, 9].Value = montoContrato;
                excelWorksheet.Cells[17, 2].Value = observacionesGral;

                int countSi = 0;
                int countNo = 0;
                int countNA = 0;
                int countDoc = 0;
                int countAten = 0;
                int countNoAten = 0;
                int fila = 20;
                int columna = 2;

                if(!obra)
                {
                    foreach (var doc in listDoc)
                    {
                        ////Observaciones
                        var collObs = DB.GetCollection<Observaciones>("Observaciones");
                        var filterObs = Builders<Observaciones>.Filter.Eq(x => x.idExpediente, id)
                            & Builders<Observaciones>.Filter.Eq(x => x.Clave, doc.clave);
                        var observaciones = await collObs.Find(filterObs).Sort("{clave: 1}").ToListAsync();
                        if (observaciones != null)
                        {
                            foreach (Observaciones dat in observaciones)
                            {
                                listObs.Add(dat);
                            }
                        }

                        if (string.IsNullOrEmpty(doc.estatus))
                        {
                            doc.estatus = "NO";
                        }

                        if (doc.estatus.Equals("SI"))
                        {
                            countSi++;
                        }
                        else if (doc.estatus.Equals("NO"))
                        {
                            countNo++;
                        }
                        else if (doc.estatus.Equals("N/A"))
                        {
                            countNA++;
                        }
                        else if (doc.estatus.Equals("DOC.ERRONEO"))
                        {
                            countDoc++;
                        }

                        if (listObs.Count > 0)
                        {
                            int count = 1;
                            for (int i = 0; i < listObs.Count; i++)
                            {
                                excelWorksheet.Cells[fila, columna].Value = doc.clave.ToString(); columna++;
                                excelWorksheet.Cells[fila, columna].Value = doc.documento; columna++;
                                excelWorksheet.Cells[fila, columna].Value = doc.estatus; columna++;

                                //OBservaciones
                                excelWorksheet.Cells[fila, columna].Value = count; columna++;
                                excelWorksheet.Cells[fila, columna].Value = listObs[i].Observacion; columna++;
                                excelWorksheet.Cells[fila, columna].Value = listObs[i].Recomendacion; columna++;

                                //Contador de atendidos y no atendidos
                                string estatus = string.Empty;
                                estatus = Convert.ToInt32(listObs[i].Estatus) == 1 ? "Atendido" : "No atendido";
                                if (Convert.ToInt32(listObs[i].Estatus) == 1) { countAten++; } else { countNoAten++; }

                                excelWorksheet.Cells[fila, columna].Value = estatus; columna++;

                                fila++;
                                columna = 2;
                                count++;
                            }
                        }
                        else
                        {
                            excelWorksheet.Cells[fila, columna].Value = doc.clave.ToString(); columna++;
                            excelWorksheet.Cells[fila, columna].Value = doc.documento; columna++;
                            excelWorksheet.Cells[fila, columna].Value = doc.estatus; columna++;

                            fila++;
                            columna = 2;
                        }
                        listObs = new List<Observaciones>();
                    }
                }
                else if (obra)
                {
                    foreach (var doc in listObra)
                    {
                        ////Observaciones
                        var collObs = DB.GetCollection<Observaciones>("Observaciones");
                        var filterObs = Builders<Observaciones>.Filter.Eq(x => x.idExpediente, id)
                            & Builders<Observaciones>.Filter.Eq(x => x.Clave, doc.clave);
                        var observaciones = await collObs.Find(filterObs).Sort("{clave: 1}").ToListAsync();
                        if (observaciones != null)
                        {
                            foreach (Observaciones dat in observaciones)
                            {
                                listObs.Add(dat);
                            }
                        }

                        if (string.IsNullOrEmpty(doc.integracion))
                        {
                            doc.integracion = "NO";
                        }

                        if (doc.integracion.Equals("SI"))
                        {
                            countSi++;
                        }
                        else if (doc.integracion.Equals("NO"))
                        {
                            countNo++;
                        }
                        else if (doc.integracion.Equals("N/A"))
                        {
                            countNA++;
                        }
                        else if (doc.integracion.Equals("DOC.ERRONEO"))
                        {
                            countDoc++;
                        }

                        if (listObs.Count > 0)
                        {
                            int count = 1;
                            for (int i = 0; i < listObs.Count; i++)
                            {
                                excelWorksheet.Cells[fila, columna].Value = doc.clave.ToString(); columna++;
                                excelWorksheet.Cells[fila, columna].Value = doc.documento; columna++;
                                excelWorksheet.Cells[fila, columna].Value = doc.integracion; columna++;

                                //OBservaciones
                                excelWorksheet.Cells[fila, columna].Value = count; columna++;
                                excelWorksheet.Cells[fila, columna].Value = listObs[i].Observacion; columna++;
                                excelWorksheet.Cells[fila, columna].Value = listObs[i].Recomendacion; columna++;

                                //Contador de atendidos y no atendidos
                                string estatus = string.Empty;
                                estatus = Convert.ToInt32(listObs[i].Estatus) == 1 ? "Atendido" : "No atendido";
                                if (Convert.ToInt32(listObs[i].Estatus) == 1) { countAten++; } else { countNoAten++; }

                                excelWorksheet.Cells[fila, columna].Value = estatus; columna++;

                                fila++;
                                columna = 2;
                                count++;
                            }
                        }
                        else
                        {
                            excelWorksheet.Cells[fila, columna].Value = doc.clave.ToString(); columna++;
                            excelWorksheet.Cells[fila, columna].Value = doc.documento; columna++;
                            excelWorksheet.Cells[fila, columna].Value = doc.integracion; columna++;

                            fila++;
                            columna = 2;
                        }
                        listObs = new List<Observaciones>();
                    }
                }

                excelWorksheet.Cells[15, 2].Value = countSi.ToString();
                excelWorksheet.Cells[15, 3].Value = countNo.ToString();
                excelWorksheet.Cells[15, 4].Value = countNA.ToString();
                excelWorksheet.Cells[15, 5].Value = countDoc.ToString();
                excelWorksheet.Cells[15, 6].Value = Convert.ToString((countSi + countNo + countNA + countDoc));

                excelWorksheet.Cells[15, 7].Value = countAten.ToString();
                excelWorksheet.Cells[15, 8].Value = countNoAten.ToString();
                excelWorksheet.Cells[15, 9].Value = Convert.ToString((countAten + countNoAten));

                excelPackage.SaveAs(ms);
            }

            ms.Position = 0;

            Spire.Xls.Workbook workbook = new Spire.Xls.Workbook();
            workbook.LoadFromStream(ms);

            var reporte = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(ms.ToArray())
            };
            reporte.Content.Headers.ContentDisposition =
                new ContentDispositionHeaderValue("attachment")
                {
                    FileName = "ReporteCedulaObserRec.xlsx"
                };
            reporte.Content.Headers.ContentType =
                new MediaTypeHeaderValue("application/octet-stream");

            return reporte;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("api/reporte/obras/{tipo}/{municipio}")]
        public async System.Threading.Tasks.Task<HttpResponseMessage> DescargaReporte(string tipo,string municipio)
        {
            //5e9f5aa3d066036b046ac966 adjquisiciones
            //5e6b246c29e31517f4d48397 obra
            MemoryStream memoryStream = new MemoryStream();

            WebClient client = new WebClient();
            try
            {
                if(tipo.Equals("A"))
                {
                    //memoryStream = new MemoryStream(client.DownloadData("C:/Simopa/Templete/TemplateReporteAdquisiciones.xlsx"));
                    memoryStream = new MemoryStream(client.DownloadData("https://apipbrdevelolop.azurewebsites.net/templates/reportes/TemplateReporteAdquisiciones.xlsx"));
                }
                else if (tipo.Equals("O"))
                {
                    //memoryStream = new MemoryStream(client.DownloadData("C:/Simopa/Templete/TemplateReporteObra.xlsx"));
                    memoryStream = new MemoryStream(client.DownloadData("https://apipbrdevelolop.azurewebsites.net/templates/reportes/TemplateReporteObra.xlsx"));
                }
            }
            finally
            {
                client.Dispose();
            }

            string id = string.Empty;
            string constr = ConfigurationManager.AppSettings["connectionString"];
            var Client = new MongoClient(constr);
            var DB = Client.GetDatabase("PRB");

            //bool obra = false;

            List<AdquisicionesV1> listAdquisiciones = new List<AdquisicionesV1>();
            List<ObraPublicaV1> listObras = new List<ObraPublicaV1>();

            if(tipo.Equals("A"))
            {
                try
                {
                    var collAdqui = DB.GetCollection<AdquisicionesV1>("Adquisiciones");
                    var filterAdqui = Builders<AdquisicionesV1>.Filter.Eq(x => x.municipio, municipio);
                    var adquisiciones = await collAdqui.Find(filterAdqui).Sort("{ejercicio:-1}").ToListAsync();

                    if (adquisiciones.Count > 0)
                    {
                        listAdquisiciones = adquisiciones;
                    }
                    else
                    {
                        listAdquisiciones = new List<AdquisicionesV1>();
                    }
                }
                catch (Exception ex)
                {
                    //response.success = false;
                    //response.messages.Add(ex.ToString());
                    //return Ok(ex.ToString());
                }
            }
            else if (tipo.Equals("O"))
            {
                try
                {

                    var collObra = DB.GetCollection<ObraPublicaV1>("ObraPublica");
                    var filterObra = Builders<ObraPublicaV1>.Filter.Eq(x => x.municipio, municipio);
                    var obras = await collObra.Find(filterObra).Sort("{ejercicio:-1}").ToListAsync();

                    if (obras.Count > 0)
                    {
                        listObras = obras;
                    }
                    else
                    {
                        listObras = new List<ObraPublicaV1>();
                    }
                }
                catch (Exception ex)
                {
                    //response.success = false;
                    //response.messages.Add(ex.ToString());
                    //return Ok(ex.ToString());
                }
            }

            MemoryStream ms = new MemoryStream();
            //Envia datos a Excel
            using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {

                ExcelWorkbook excelWorkBook = excelPackage.Workbook;
                ExcelWorksheet excelWorksheet = excelWorkBook.Worksheets[1];

                int rows = 2;
                int columns = 1;

                foreach (AdquisicionesV1 data in listAdquisiciones)
                {
                    id = data.Id;
                    excelWorksheet.Cells[rows, columns].Value = data.numeroAdjudicacion; columns++;
                    excelWorksheet.Cells[rows, columns].Value = data.objetoContrato; columns++;
                    excelWorksheet.Cells[rows, columns].Value = data.tipoAdjudicacion; columns++;
                    excelWorksheet.Cells[rows, columns].Value = data.ejercicio; columns++;
                    //string numeroAdjudicaicon=  data.ejercicio

                    var collection = DB.GetCollection<VisitaObraImagenes>("VisitaObraImagen");
                    var filter = Builders<VisitaObraImagenes>.Filter.Eq(x => x.ExpedienteId, id);
                    var images = await collection.Find(filter).ToListAsync();

                    if (images.Count > 0)
                    {
                        if (tipo.Equals("A"))
                        {
                            excelWorksheet.Cells[rows, columns].Value = "SI"; columns++;
                        }
                        else if (tipo.Equals("O"))
                        {
                            excelWorksheet.Cells[rows, columns].Value = "SI"; columns++;
                            excelWorksheet.Cells[rows, columns].Value = "SI"; columns++;
                        }
                    }
                    else
                    {
                        if (tipo.Equals("A"))
                        {
                            excelWorksheet.Cells[rows, columns].Value = "NO"; columns++;
                        }
                        else if (tipo.Equals("O"))
                        {
                            excelWorksheet.Cells[rows, columns].Value = "NO"; columns++;
                            excelWorksheet.Cells[rows, columns].Value = "NO"; columns++;
                        }
                    }

                    id = string.Empty;
                    rows++;
                    columns = 1;
                }

                foreach (ObraPublicaV1 data in listObras)
                {
                    id = data.Id;

                    //excelWorksheet.Cells[rows, columns].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    //excelWorksheet.Cells[rows, columns].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                    excelWorksheet.Cells[rows, columns].Value = data.numeroProcedimiento; columns++;
                    excelWorksheet.Cells[rows, columns].Value = data.nombreObra; columns++;
                    excelWorksheet.Cells[rows, columns].Value = data.tipoAdjudicacion; columns++;
                    excelWorksheet.Cells[rows, columns].Value = data.ejercicio; columns++;

                    var collection = DB.GetCollection<VisitaObraImagenes>("VisitaObraImagen");
                    var filter = Builders<VisitaObraImagenes>.Filter.Eq(x => x.ExpedienteId, id);
                    var images = await collection.Find(filter).ToListAsync();

                    if (images.Count > 0)
                    {
                        excelWorksheet.Cells[rows, columns].Value = "SI"; columns++;
                        excelWorksheet.Cells[rows, columns].Value = "SI"; columns++;
                    }
                    else
                    {
                        excelWorksheet.Cells[rows, columns].Value = "NO"; columns++;
                        excelWorksheet.Cells[rows, columns].Value = "NO"; columns++;
                    }

                    id = string.Empty;
                    rows++;
                    columns = 1;
                }

                excelPackage.SaveAs(ms);
            }

            ms.Position = 0;

            Spire.Xls.Workbook workbook = new Spire.Xls.Workbook();
            workbook.LoadFromStream(ms);

            var reporte = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(ms.ToArray())
            };
            reporte.Content.Headers.ContentDisposition =
                new ContentDispositionHeaderValue("attachment")
                {
                    FileName = "Reporte.xlsx"
                };
            reporte.Content.Headers.ContentType =
                new MediaTypeHeaderValue("application/octet-stream");

            return reporte;
        }

        /// <summary>
        /// Metodo que ordena la informacion por clave Adquisiciones
        /// </summary>
        /// <param name="list"></param>
        /// <param name="sortBy"></param>
        /// <param name="sortDirection"></param>
        private void Sort(ref List<DocumentoAdquisicionesV1> list, string sortBy, string sortDirection)
        {
            //Example data:
            //sortBy = "FirstName"
            //sortDirection = "ASC" or "DESC"

            if (sortBy == "clave")
            {
                list = list.OrderBy(x => x.clave).ToList<DocumentoAdquisicionesV1>();
            }
        }

        /// <summary>
        /// Metodo que ordena la informacion por clave Obra Publica
        /// </summary>
        /// <param name="list"></param>
        /// <param name="sortBy"></param>
        /// <param name="sortDirection"></param>
        private void Sort(ref List<DocumentosObrasV1> list, string sortBy, string sortDirection)
        {
            //Example data:
            //sortBy = "FirstName"
            //sortDirection = "ASC" or "DESC"

            if (sortBy == "clave")
            {
                list = list.OrderBy(x => x.clave).ToList<DocumentosObrasV1>();
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("api/reporte/adquisiciones/sumatoria/invitacion/tres/{municipio}/{ejercicio}/{tipoadjudicacion}")]
        public async System.Threading.Tasks.Task<HttpResponseMessage> DescargaReporteSumarioAdquInvTres(string municipio, string ejercicio, string tipoadjudicacion)
        {
            //5e9f5aa3d066036b046ac966 adjquisiciones
            //5e6b246c29e31517f4d48397 obra
            MemoryStream memoryStream = new MemoryStream();

            WebClient client = new WebClient();
            try
            {
                //memoryStream = new MemoryStream(client.DownloadData("C:/Simopa/Templete/TemplateSumariaAdquisicionesInvTres.xlsx"));

                if (municipio.Equals("TEPEACA"))
                {
                    memoryStream = new MemoryStream(client.DownloadData("https://apipbrdevelolop.azurewebsites.net/templates/reportes/TemplateSumariaAdquisicionesInvTres.xlsx"));
                }
                else if (municipio.Equals("SAN ANDRES CHOLULA"))
                {
                    
                    memoryStream = new MemoryStream(client.DownloadData("https://apipbrdevelolop.azurewebsites.net/templates/reportes/TemplateSumariaAdquisicionesInvA3Sach.xlsx"));
                }
                    
            }
            finally
            {
                client.Dispose();
            }

            string id = string.Empty;
            string constr = ConfigurationManager.AppSettings["connectionString"];
            var Client = new MongoClient(constr);
            var DB = Client.GetDatabase("PRB");

            List<AdquisicionesV1> listAdquisiciones = new List<AdquisicionesV1>();
            List<ObraPublicaV1> listObras = new List<ObraPublicaV1>();
           
            try
            {
                var collAdqui = DB.GetCollection<AdquisicionesV1>("Adquisiciones");
                var filterAdqui = Builders<AdquisicionesV1>.Filter.Eq(x => x.municipio, municipio)
                    & Builders<AdquisicionesV1>.Filter.Eq(x => x.ejercicio, ejercicio)
                    & Builders<AdquisicionesV1>.Filter.Eq(x => x.tipoAdjudicacion, tipoadjudicacion);
                var adquisiciones = await collAdqui.Find(filterAdqui).Sort("{ejercicio:-1}").ToListAsync();

                if (adquisiciones.Count > 0)
                {
                    listAdquisiciones = adquisiciones;
                }
                else
                {
                    listAdquisiciones = new List<AdquisicionesV1>();
                }
            }
            catch (Exception ex)
            {
                //response.success = false;
                //response.messages.Add(ex.ToString());
                //return Ok(ex.ToString());
            }

            //Imagen           
            int Height = 0;
            int Width = 0;
            int rowIndex = 0;
            int colIndex = 0;

            WebClient clientImage = new WebClient();
            MemoryStream memoryImage = new MemoryStream();
            string nameImage = string.Empty;
            try
            {
                if (municipio.Equals("TEPEACA"))
                {
                    //memoryImage = new MemoryStream(clientImage.DownloadData("C:/Simopa/Templete/Logo/LogoTepeaca.png"));
                    memoryImage = new MemoryStream(client.DownloadData("https://apipbrdevelolop.azurewebsites.net/templates/logo/LogoTepeaca.png"));
                    nameImage = "Tepeaca";
                    Height = 160;
                    Width = 150;
                    rowIndex = 1;
                    colIndex = 1;
                }
                else if (municipio.Equals("SAN ANDRES CHOLULA"))
                {
                    //memoryImage = new MemoryStream(clientImage.DownloadData("D:/Plantillas/SACH/LogoDespacho.png"));
                    memoryImage = new MemoryStream(client.DownloadData("https://apipbrdevelolop.azurewebsites.net/templates/logo/LogoDespacho.png"));
                    nameImage = "ESACH";
                    Height = 120;
                    Width = 150;
                    rowIndex = 3;
                    colIndex = 1;
                }
                else
                {
                    //memoryImage = new MemoryStream(clientImage.DownloadData("C:/Simopa/Templete/Logo/LogoDespacho.png"));
                    memoryImage = new MemoryStream(client.DownloadData("https://apipbrdevelolop.azurewebsites.net/templates/logo/LogoDespacho.png"));
                    nameImage = "Any";
                }
            }
            catch (Exception ex)
            {
                clientImage.Dispose();
            }

            MemoryStream ms = new MemoryStream();
            //Envia datos a Excel
            using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {

                ExcelWorkbook excelWorkBook = excelPackage.Workbook;
                ExcelWorksheet excelWorksheet = excelWorkBook.Worksheets[1];
                excelWorksheet.Workbook.CalcMode = ExcelCalcMode.Manual;

                //Imagen
                Image img = Image.FromStream(memoryImage);
                ExcelPicture pic = excelWorksheet.Drawings.AddPicture("imageVista-" + nameImage, img);
                pic.SetPosition(rowIndex, 0, colIndex + ((1 - 1) * 4), 0);
                pic.SetSize(Width, Height);

                int rows = 0;
                int columns = 0;
                //int count = 1;
                DateTime fecha = DateTime.Now;

                if (municipio.Equals("TEPEACA"))
                {
                    excelWorksheet.Cells[5, 4].Value = "CÉDULA ANALÍTICA DE REVISIÓN DE PROCEDIMIENTO DE INVITACIÓN A CUANDO MENOS TRES PERSONAS " + ejercicio;
                    excelWorksheet.Cells[7, 4].Value = "LIMT- LEY DE INGRESOS DEL MUNICIPIO DE TEPEACA PARA EL " + ejercicio;
                    rows = 9;
                    columns = 7;
                }
                else if (municipio.Equals("SAN ANDRES CHOLULA"))
                {
                    excelWorksheet.Cells[5, 4].Value = "CÉDULA ANALÍTICA DE REVISIÓN DE PROCEDIMIENTO DE INVITACIÓN A CUANDO MENOS TRES PERSONAS " + ejercicio;
                    excelWorksheet.Cells[7, 4].Value = "LIMSACH- LEY DE INGRESOS DEL MUNICIPIO DE SAN ANDRES CHOLULA PARA EL " + ejercicio;
                    rows = 9;
                    columns = 7;
                }

                int SI = 0;
                int NO = 0;
                int NA = 0;
                int count = 0;

                int gralSI = 0;
                int gralNO = 0;
                int gralNA = 0;

                DocumentosAdquisicionesSanAndresCholula documentos = new DocumentosAdquisicionesSanAndresCholula();
                List<DocumentoAdquisicionesV1> listDoc = new List<DocumentoAdquisicionesV1>();


                #region TEPEACA

                if (municipio.Equals("TEPEACA"))
                {
                    foreach (AdquisicionesV1 data in listAdquisiciones)
                    {

                        listDoc = data.documentos;
                        Sort(ref listDoc, "clave", "ASC");

                        excelWorksheet.Cells[rows, columns].Value = data.numeroAdjudicacion;

                        rows++;
                        rows++;
                        rows++;

                        foreach (var documents in listDoc)
                        {

                            string val = documents.estatus;

                            if (val == null)
                            {
                                NO++;
                            }
                            else if (val.Equals("NO"))
                            {
                                NO++;
                            }
                            else if (val.Equals("SI"))
                            {
                                SI++;
                            }
                            else if (val.Equals("N/A"))
                            {
                                NA++;
                            }

                            if (documents.grupo.Equals("DOCUMENTACIÓN ADMINISTRATIVA"))
                            {
                                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                rows++;
                                count++;
                                if (count == 5) { rows++; count = 0; }
                            }
                            else if (documents.grupo.Equals("PROCEDIMIENTO DE ADJUDICACIÓN"))
                            {
                                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                rows++;
                                count++;
                                if (count == 13) { rows++; count = 0; }
                            }
                            else if (documents.grupo.Equals("REQUISITOS DEL CONTRATO"))
                            {
                                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                rows++;
                                count++;
                                if (count == 16) { rows++; count = 0; }
                            }
                            else if (documents.grupo.Equals("GARANTÍAS"))
                            {
                                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                rows++;
                                count++;
                                if (count == 3) { rows++; count = 0; }
                            }
                            else if (documents.grupo.Equals("ENTREGABLES"))
                            {
                                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                rows++;
                                count++;
                                if (count == 8) { rows++; count = 0; }
                            }
                        }

                        gralSI = gralSI + SI;
                        gralNO = gralNO + NO;
                        gralNA = gralNA + NA;

                        //SI
                        excelWorksheet.Cells[61, columns].Value = SI; rows++;
                        //NO
                        excelWorksheet.Cells[62, columns].Value = NO; rows++;
                        //NA
                        excelWorksheet.Cells[63, columns].Value = NA; rows++;
                        //Sumatoria
                        excelWorksheet.Cells[64, columns].Value = (SI + NO + NA);

                        rows = 9;
                        columns++;
                        SI = 0;
                        NO = 0;
                        NA = 0;
                        count = 0;
                    }

                    columns++;
                    //excelWorksheet.Cells[8, columns].Value = "SI"; columns++;
                    //excelWorksheet.Cells[8, columns].Value = "NO"; columns++;
                    //excelWorksheet.Cells[8, columns].Value = "N/A"; columns++;
                    rows++;
                    rows++;
                    rows++;

                    columns = 56;

                    #region SI, NO, N/A
                    /*
                    foreach (AdquisicionesV1 data in listAdquisiciones)
                    {
                        foreach (var documents in data.documentos)
                        {
                            if (documents.grupo.Equals("DOCUMENTACIÓN ADMINISTRATIVA"))
                            {
                                columns = 56;
                                columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BA" + rows + ",\"SI\")"; columns++;
                                //excelWorksheet.Cells[rows, columns].Calculate();  
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BA" + rows + ",\"NO\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BA" + rows + ",\"N/A\")"; columns++;

                                rows++;
                                count++;
                                if (count == 5) { rows++; count = 0; }
                            }
                            else if (documents.grupo.Equals("PROCEDIMIENTO DE ADJUDICACIÓN"))
                            {
                                columns = 56;
                                columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                                rows++;
                                count++;
                                if (count == 13) { rows++; count = 0; }
                            }
                            else if (documents.grupo.Equals("REQUISITOS DEL CONTRATO"))
                            {
                                columns = 56;
                                columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                                rows++;
                                count++;
                                if (count == 16) { rows++; count = 0; }
                            }
                            else if (documents.grupo.Equals("GARANTÍAS"))
                            {
                                columns = 56;
                                columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                                rows++;
                                count++;
                                if (count == 3) { rows++; count = 0; }
                            }
                            else if (documents.grupo.Equals("ENTREGABLES"))
                            {
                                columns = 56;
                                columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                                rows++;
                                count++;
                                if (count == 8) { rows++; count = 0; }
                            }
                        }
                        break;
                    }
                    */
                    #endregion

                    //SI
                    excelWorksheet.Cells[61, 6].Value = gralSI;
                    //NO
                    excelWorksheet.Cells[62, 6].Value = gralNO;
                    //NA
                    excelWorksheet.Cells[63, 6].Value = gralNA;
                    //Suma Gral
                    excelWorksheet.Cells[64, 6].Value = (gralSI + gralNO + gralNA);
                    //Total Expedientes
                    excelWorksheet.Cells[65, 6].Value = listAdquisiciones.Count;

                    SI = 0;
                    NO = 0;
                    NA = 0;
                    count = 0;
                }
                #endregion
                #region SAN ANDRES CHOLULA
                else if (municipio.Equals("SAN ANDRES CHOLULA"))
                {
                    foreach (AdquisicionesV1 data in listAdquisiciones)
                    {

                        listDoc = data.documentos;
                        Sort(ref listDoc, "clave", "ASC");

                        excelWorksheet.Cells[rows, columns].Value = data.numeroAdjudicacion;

                        rows++;
                        rows++;
                        rows++;

                        foreach (var documents in listDoc)
                        {

                            string val = documents.estatus;

                            if (val == null)
                            {
                                NO++;
                            }
                            else if (val.Equals("NO"))
                            {
                                NO++;
                            }
                            else if (val.Equals("SI"))
                            {
                                SI++;
                            }
                            else if (val.Equals("N/A"))
                            {
                                NA++;
                            }

                            if (documents.grupo.Equals("DOCUMENTACIÓN ADMINISTRATIVA"))
                            {
                                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                rows++;
                                count++;
                                if (count == 5) { rows++; count = 0; }
                            }
                            else if (documents.grupo.Equals("PROCEDIMIENTO DE ADJUDICACIÓN"))
                            {
                                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                rows++;
                                count++;
                                if (count == 13) { rows++; count = 0; }
                            }
                            else if (documents.grupo.Equals("REQUISITOS DEL CONTRATO"))
                            {
                                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                rows++;
                                count++;
                                if (count == 16) { rows++; count = 0; }
                            }
                            else if (documents.grupo.Equals("GARANTÍAS"))
                            {
                                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                rows++;
                                count++;
                                if (count == 3) { rows++; count = 0; }
                            }
                            else if (documents.grupo.Equals("ENTREGABLES"))
                            {
                                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                rows++;
                                count++;
                                if (count == 8) { rows++; count = 0; }
                            }
                        }

                        gralSI = gralSI + SI;
                        gralNO = gralNO + NO;
                        gralNA = gralNA + NA;

                        //SI
                        excelWorksheet.Cells[61, columns].Value = SI; rows++;
                        //NO
                        excelWorksheet.Cells[62, columns].Value = NO; rows++;
                        //NA
                        excelWorksheet.Cells[63, columns].Value = NA; rows++;
                        //Sumatoria
                        excelWorksheet.Cells[64, columns].Value = (SI + NO + NA);

                        rows = 9;
                        columns++;
                        SI = 0;
                        NO = 0;
                        NA = 0;
                        count = 0;
                    }

                    columns++;
                    //excelWorksheet.Cells[8, columns].Value = "SI"; columns++;
                    //excelWorksheet.Cells[8, columns].Value = "NO"; columns++;
                    //excelWorksheet.Cells[8, columns].Value = "N/A"; columns++;
                    rows++;
                    rows++;
                    rows++;

                    columns = 56;

                    #region SI, NO, N/A
                    /*
                    foreach (AdquisicionesV1 data in listAdquisiciones)
                    {
                        foreach (var documents in data.documentos)
                        {
                            if (documents.grupo.Equals("DOCUMENTACIÓN ADMINISTRATIVA"))
                            {
                                columns = 56;
                                columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BA" + rows + ",\"SI\")"; columns++;
                                //excelWorksheet.Cells[rows, columns].Calculate();  
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BA" + rows + ",\"NO\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BA" + rows + ",\"N/A\")"; columns++;

                                rows++;
                                count++;
                                if (count == 5) { rows++; count = 0; }
                            }
                            else if (documents.grupo.Equals("PROCEDIMIENTO DE ADJUDICACIÓN"))
                            {
                                columns = 56;
                                columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                                rows++;
                                count++;
                                if (count == 13) { rows++; count = 0; }
                            }
                            else if (documents.grupo.Equals("REQUISITOS DEL CONTRATO"))
                            {
                                columns = 56;
                                columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                                rows++;
                                count++;
                                if (count == 16) { rows++; count = 0; }
                            }
                            else if (documents.grupo.Equals("GARANTÍAS"))
                            {
                                columns = 56;
                                columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                                rows++;
                                count++;
                                if (count == 3) { rows++; count = 0; }
                            }
                            else if (documents.grupo.Equals("ENTREGABLES"))
                            {
                                columns = 56;
                                columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                                rows++;
                                count++;
                                if (count == 8) { rows++; count = 0; }
                            }
                        }
                        break;
                    }
                    */
                    #endregion

                    //SI
                    excelWorksheet.Cells[61, 6].Value = gralSI;
                    //NO
                    excelWorksheet.Cells[62, 6].Value = gralNO;
                    //NA
                    excelWorksheet.Cells[63, 6].Value = gralNA;
                    //Suma Gral
                    excelWorksheet.Cells[64, 6].Value = (gralSI + gralNO + gralNA);
                    //Total Expedientes
                    excelWorksheet.Cells[65, 6].Value = listAdquisiciones.Count;

                    SI = 0;
                    NO = 0;
                    NA = 0;
                    count = 0;
                }
                #endregion

                #region Anterior

                //foreach (AdquisicionesV1 data in listAdquisiciones)
                //{

                //    listDoc = data.documentos;
                //    Sort(ref listDoc, "clave", "ASC");

                //    excelWorksheet.Cells[rows, columns].Value = data.numeroAdjudicacion;

                //    rows++;
                //    rows++;
                //    rows++;

                //    foreach (var documents in listDoc)
                //    {

                //        string val = documents.estatus;

                //        if (val == null)
                //        {
                //            NO++;
                //        }
                //        else if (val.Equals("NO"))
                //        {
                //            NO++;
                //        }
                //        else if (val.Equals("SI"))
                //        {
                //            SI++;
                //        }
                //        else if (val.Equals("N/A"))
                //        {
                //            NA++;
                //        }

                //        if (documents.grupo.Equals("DOCUMENTACIÓN ADMINISTRATIVA"))
                //        {
                //            excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus)? "NO": documents.estatus;
                //            rows++;
                //            count++;
                //            if (count == 5) { rows++; count = 0; }
                //        }
                //        else if (documents.grupo.Equals("PROCEDIMIENTO DE ADJUDICACIÓN"))
                //        {
                //            excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                //            rows++;
                //            count++;
                //            if (count == 13) { rows++; count = 0; }
                //        }
                //        else if (documents.grupo.Equals("REQUISITOS DEL CONTRATO"))
                //        {
                //            excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                //            rows++;
                //            count++;
                //            if (count == 16) { rows++; count = 0; }
                //            }
                //        else if (documents.grupo.Equals("GARANTÍAS"))
                //        {
                //            excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                //            rows++;
                //            count++;
                //            if (count == 3) { rows++; count = 0; }
                //        }
                //        else if (documents.grupo.Equals("ENTREGABLES"))
                //        {
                //            excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                //            rows++;
                //            count++;
                //            if (count == 8) { rows++; count = 0; }
                //        }
                //    }

                //    gralSI = gralSI + SI;
                //    gralNO = gralNO + NO;
                //    gralNA = gralNA + NA;

                //    //SI
                //    excelWorksheet.Cells[61, columns].Value = SI; rows++;
                //    //NO
                //    excelWorksheet.Cells[62, columns].Value = NO; rows++;
                //    //NA
                //    excelWorksheet.Cells[63, columns].Value = NA; rows++;
                //    //Sumatoria
                //    excelWorksheet.Cells[64, columns].Value = (SI + NO + NA);

                //    rows = 9;
                //    columns++;
                //    SI = 0;
                //    NO = 0;
                //    NA = 0;
                //    count = 0;
                //}

                //columns++;
                ////excelWorksheet.Cells[8, columns].Value = "SI"; columns++;
                ////excelWorksheet.Cells[8, columns].Value = "NO"; columns++;
                ////excelWorksheet.Cells[8, columns].Value = "N/A"; columns++;
                //rows++;
                //rows++;
                //rows++;

                //columns = 56;

                //#region SI, NO, N/A
                ///*
                //foreach (AdquisicionesV1 data in listAdquisiciones)
                //{
                //    foreach (var documents in data.documentos)
                //    {
                //        if (documents.grupo.Equals("DOCUMENTACIÓN ADMINISTRATIVA"))
                //        {
                //            columns = 56;
                //            columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BA" + rows + ",\"SI\")"; columns++;
                //            //excelWorksheet.Cells[rows, columns].Calculate();  
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BA" + rows + ",\"NO\")"; columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BA" + rows + ",\"N/A\")"; columns++;

                //            rows++;
                //            count++;
                //            if (count == 5) { rows++; count = 0; }
                //        }
                //        else if (documents.grupo.Equals("PROCEDIMIENTO DE ADJUDICACIÓN"))
                //        {
                //            columns = 56;
                //            columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                //            rows++;
                //            count++;
                //            if (count == 13) { rows++; count = 0; }
                //        }
                //        else if (documents.grupo.Equals("REQUISITOS DEL CONTRATO"))
                //        {
                //            columns = 56;
                //            columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                //            rows++;
                //            count++;
                //            if (count == 16) { rows++; count = 0; }
                //        }
                //        else if (documents.grupo.Equals("GARANTÍAS"))
                //        {
                //            columns = 56;
                //            columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                //            rows++;
                //            count++;
                //            if (count == 3) { rows++; count = 0; }
                //        }
                //        else if (documents.grupo.Equals("ENTREGABLES"))
                //        {
                //            columns = 56;
                //            columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                //            rows++;
                //            count++;
                //            if (count == 8) { rows++; count = 0; }
                //        }
                //    }
                //    break;
                //}
                //*/
                //#endregion

                ////SI
                //excelWorksheet.Cells[61, 6].Value = gralSI;
                ////NO
                //excelWorksheet.Cells[62, 6].Value = gralNO;
                ////NA
                //excelWorksheet.Cells[63, 6].Value = gralNA;
                ////Suma Gral
                //excelWorksheet.Cells[64, 6].Value = (gralSI + gralNO + gralNA);
                ////Total Expedientes
                //excelWorksheet.Cells[65, 6].Value = listAdquisiciones.Count;

                //SI = 0;
                //NO = 0;
                //NA = 0;
                //count = 0;

                #endregion

                excelPackage.SaveAs(ms);
            }

            ms.Position = 0;

            Spire.Xls.Workbook workbook = new Spire.Xls.Workbook();
            workbook.LoadFromStream(ms);

            var reporte = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(ms.ToArray())
            };
            reporte.Content.Headers.ContentDisposition =
                new ContentDispositionHeaderValue("attachment")
                {
                    FileName = "ReporteAdquisicionesInvTres.xlsx"
                };
            reporte.Content.Headers.ContentType =
                new MediaTypeHeaderValue("application/octet-stream");

            return reporte;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("api/reporte/adquisiciones/sumatoria/consurso/invitacion/{municipio}/{ejercicio}/{tipoadjudicacion}")]
        public async System.Threading.Tasks.Task<HttpResponseMessage> DescargaReporteSumarioAdquConcurso(string municipio, string ejercicio, string tipoadjudicacion)
        {
            //5e9f5aa3d066036b046ac966 adjquisiciones
            //5e6b246c29e31517f4d48397 obra
            MemoryStream memoryStream = new MemoryStream();

            //if (municipio.Equals("SAN ANDRES CHOLULA"))
            //{
            //    MemoryStream mss = new MemoryStream();
            //    var reportes = new HttpResponseMessage(HttpStatusCode.OK)
            //    {
            //        Content = new ByteArrayContent(mss.ToArray())
            //    };
            //    reportes.Content.Headers.ContentDisposition =
            //        new ContentDispositionHeaderValue("attachment")
            //        {
            //            FileName = "ReporteAdquisicionesConsursoInv.xlsx"
            //        };
            //    reportes.Content.Headers.ContentType =
            //        new MediaTypeHeaderValue("application/octet-stream");

            //    return reportes;
            //}

            WebClient client = new WebClient();
            try
            {
                //memoryStream = new MemoryStream(client.DownloadData("C:/Simopa/Templete/TemplateSumariaAdquisicionesConcursoInv.xlsx"));
                if (municipio.Equals("TEPEACA"))
                {
                    memoryStream = new MemoryStream(client.DownloadData("https://apipbrdevelolop.azurewebsites.net/templates/reportes/TemplateSumariaAdquisicionesConcursoInv.xlsx"));
                }
                else if (municipio.Equals("SAN ANDRES CHOLULA"))
                {

                    memoryStream = new MemoryStream(client.DownloadData("https://apipbrdevelolop.azurewebsites.net/templates/reportes/TemplateSumariaAdquisicionesConsursoInvSach.xlsx"));
                }

            }
            finally
            {
                client.Dispose();
            }

            string id = string.Empty;
            string constr = ConfigurationManager.AppSettings["connectionString"];
            var Client = new MongoClient(constr);
            var DB = Client.GetDatabase("PRB");

            List<AdquisicionesV1> listAdquisiciones = new List<AdquisicionesV1>();
            List<ObraPublicaV1> listObras = new List<ObraPublicaV1>();

            try
            {
                var collAdqui = DB.GetCollection<AdquisicionesV1>("Adquisiciones");
                var filterAdqui = Builders<AdquisicionesV1>.Filter.Eq(x => x.municipio, municipio)
                    & Builders<AdquisicionesV1>.Filter.Eq(x => x.ejercicio, ejercicio)
                    & Builders<AdquisicionesV1>.Filter.Eq(x => x.tipoAdjudicacion, tipoadjudicacion);
                var adquisiciones = await collAdqui.Find(filterAdqui).Sort("{ejercicio:-1}").ToListAsync();

                if (adquisiciones.Count > 0)
                {
                    listAdquisiciones = adquisiciones;
                }
                else
                {
                    listAdquisiciones = new List<AdquisicionesV1>();
                }
            }
            catch (Exception ex)
            {
                //response.success = false;
                //response.messages.Add(ex.ToString());
                //return Ok(ex.ToString());
            }

            //Imagen           
            int Height = 0;
            int Width = 0;
            int rowIndex = 0;
            int colIndex = 0;
            string nameImage = string.Empty;

            WebClient clientImage = new WebClient();
            MemoryStream memoryImage = new MemoryStream();
            try
            {
                if (municipio.Equals("TEPEACA"))
                {
                    //memoryImage = new MemoryStream(clientImage.DownloadData("C:/Simopa/Templete/Logo/LogoTepeaca.png"));
                    memoryImage = new MemoryStream(client.DownloadData("https://apipbrdevelolop.azurewebsites.net/templates/logo/LogoTepeaca.png"));
                    nameImage = "Tepeaca";
                    Height = 160;
                    Width = 150;
                    rowIndex = 1;
                    colIndex = 1;
                }
                else if (municipio.Equals("SAN ANDRES CHOLULA"))
                {
                    //memoryImage = new MemoryStream(clientImage.DownloadData("C:/Simopa/Templete/Logo/LogoDespacho.png"));
                    memoryImage = new MemoryStream(client.DownloadData("https://apipbrdevelolop.azurewebsites.net/templates/logo/LogoDespacho.png"));
                    nameImage = "ESACH";
                    Height = 100;
                    Width = 150;
                    rowIndex = 4;
                    colIndex = 1;
                }
                else
                {
                    //memoryImage = new MemoryStream(clientImage.DownloadData("C:/Simopa/Templete/Logo/LogoDespacho.png"));
                    memoryImage = new MemoryStream(client.DownloadData("https://apipbrdevelolop.azurewebsites.net/templates/logo/LogoDespacho.png"));
                }
            }
            catch (Exception ex)
            {
                clientImage.Dispose();
            }

            DocumentosAdquisicionesSanAndresCholula documentos = new DocumentosAdquisicionesSanAndresCholula();
            List<DocumentoAdquisicionesV1> listDoc = new List<DocumentoAdquisicionesV1>();

            MemoryStream ms = new MemoryStream();
            //Envia datos a Excel
            using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {

                ExcelWorkbook excelWorkBook = excelPackage.Workbook;
                ExcelWorksheet excelWorksheet = excelWorkBook.Worksheets[1];
                excelWorksheet.Workbook.CalcMode = ExcelCalcMode.Manual;

                //Imagen
                Image img = Image.FromStream(memoryImage);
                ExcelPicture pic = excelWorksheet.Drawings.AddPicture("imageVista-" + nameImage, img);
                pic.SetPosition(rowIndex, 0, colIndex + ((1 - 1) * 4), 0);
                pic.SetSize(Width, Height);

                
                int rows = 0;
                int columns = 0;

                DateTime fecha = DateTime.Now;

                if (municipio.Equals("TEPEACA"))
                {
                    excelWorksheet.Cells[5, 4].Value = "CÉDULA ANALÍTICA DE REVISIÓN DE PROCEDIMIENTO DE CONCURSO POR INVITACIÓN " + ejercicio;
                    excelWorksheet.Cells[7, 4].Value = "LIMT- LEY DE INGRESOS DEL MUNICIPIO DE TEPEACA PARA EL " + ejercicio;
                    rows = 9;
                    columns = 8;
                }
                else if (municipio.Equals("SAN ANDRES CHOLULA"))
                {
                    excelWorksheet.Cells[5, 4].Value = "CÉDULA ANALÍTICA DE REVISIÓN DE PROCEDIMIENTO DE ADJUDICACIÓN DIRECTA " + ejercicio;
                    excelWorksheet.Cells[7, 4].Value = "LIMSACH- LEY DE INGRESOS DEL MUNICIPIO DE SAN ANDRES CHOLULA PARA EL " + ejercicio;
                    rows = 9;
                    columns = 8;
                }

                int SI = 0;
                int NO = 0;
                int NA = 0;
                int count = 0;

                int gralSI = 0;
                int gralNO = 0;
                int gralNA = 0;

                // Excel 43
                // Caso 41  18 no existe, 42 18 si existe 

                #region TEPEACA
                if (municipio.Equals("TEPEACA"))
                {
                    foreach (AdquisicionesV1 data in listAdquisiciones)
                    {
                        excelWorksheet.Cells[rows, columns].Value = data.numeroAdjudicacion;

                        rows++;
                        rows++;
                        rows++;

                        listDoc = data.documentos;
                        Sort(ref listDoc, "clave", "ASC");

                        foreach (var documents in listDoc)
                        {

                            string val = documents.estatus;

                            if (val == null)
                            {
                                NO++;
                            }
                            else if (val.Equals("NO"))
                            {
                                NO++;
                            }
                            else if (val.Equals("SI"))
                            {
                                SI++;
                            }
                            else if (val.Equals("N/A"))
                            {
                                NA++;
                            }

                            if (documents.grupo.Equals("DOCUMENTACIÓN ADMINISTRATIVA"))
                            {
                                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                rows++;
                                count++;
                                if (count == 5) { rows++; count = 0; }
                            }
                            else if (documents.grupo.Equals("PROCEDIMIENTO DE ADJUDICACIÓN"))
                            {
                                if (data.documentos.Count == 41)
                                {
                                    if (documents.clave == 17)
                                    {
                                        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                        rows++;
                                        rows++;
                                        count++;
                                    }
                                    else
                                    {
                                        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                        rows++;
                                        count++;
                                    }

                                    if (count == 20) { rows++; count = 0; }
                                }
                                else
                                {
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                    rows++;
                                    count++;
                                    if (count == 21) { rows++; count = 0; }
                                }
                            }
                            else if (documents.grupo.Equals("REQUISITOS DEL CONTRATO"))
                            {
                                if (data.documentos.Count == 41)
                                {
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                    rows++;
                                    count++;
                                    if (count == 5) { rows++; rows++; count = 0; }
                                }
                                else if (data.documentos.Count == 42)
                                {
                                    string x = documents.documento;
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                    rows++;
                                    count++;
                                    if (count == 5) { rows++; rows++; count = 0; }
                                }
                                else //if (data.documentos.Count == 43)
                                {
                                    string x = documents.documento;
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                    rows++;
                                    count++;
                                    if (count == 6) { rows++; count = 0; }
                                }
                            }
                            else if (documents.grupo.Equals("GARANTÍAS"))
                            {
                                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                rows++;
                                count++;
                                if (count == 3) { rows++; count = 0; }
                            }
                            else if (documents.grupo.Equals("PROCEDIMIENTO DE PAGO"))
                            {
                                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                rows++;
                                count++;
                                if (count == 8) { rows++; count = 0; }
                            }
                        }

                        gralSI = gralSI + SI;
                        gralNO = gralNO + NO;
                        gralNA = gralNA + NA;

                        //SI
                        excelWorksheet.Cells[59, columns].Value = SI; rows++;
                        //NO
                        excelWorksheet.Cells[60, columns].Value = NO; rows++;
                        //NA
                        excelWorksheet.Cells[61, columns].Value = NA; rows++;
                        //Sumatoria
                        excelWorksheet.Cells[62, columns].Value = (SI + NO + NA);

                        rows = 9;
                        columns++;
                        SI = 0;
                        NO = 0;
                        NA = 0;
                        count = 0;
                    }

                    columns++;
                    //excelWorksheet.Cells[8, columns].Value = "SI"; columns++;
                    //excelWorksheet.Cells[8, columns].Value = "NO"; columns++;
                    //excelWorksheet.Cells[8, columns].Value = "N/A"; columns++;
                    rows++;
                    rows++;
                    rows++;

                    columns = 56;

                    #region SI, NO, N/A
                    /*
                    foreach (AdquisicionesV1 data in listAdquisiciones)
                    {
                        foreach (var documents in data.documentos)
                        {
                            if (documents.grupo.Equals("DOCUMENTACIÓN ADMINISTRATIVA"))
                            {
                                columns = 56;
                                columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BA" + rows + ",\"SI\")"; columns++;
                                //excelWorksheet.Cells[rows, columns].Calculate();  
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BA" + rows + ",\"NO\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BA" + rows + ",\"N/A\")"; columns++;

                                rows++;
                                count++;
                                if (count == 5) { rows++; count = 0; }
                            }
                            else if (documents.grupo.Equals("PROCEDIMIENTO DE ADJUDICACIÓN"))
                            {
                                columns = 56;
                                columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                                rows++;
                                count++;
                                if (count == 13) { rows++; count = 0; }
                            }
                            else if (documents.grupo.Equals("REQUISITOS DEL CONTRATO"))
                            {
                                columns = 56;
                                columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                                rows++;
                                count++;
                                if (count == 16) { rows++; count = 0; }
                            }
                            else if (documents.grupo.Equals("GARANTÍAS"))
                            {
                                columns = 56;
                                columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                                rows++;
                                count++;
                                if (count == 3) { rows++; count = 0; }
                            }
                            else if (documents.grupo.Equals("ENTREGABLES"))
                            {
                                columns = 56;
                                columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                                rows++;
                                count++;
                                if (count == 8) { rows++; count = 0; }
                            }
                        }
                        break;
                    }
                    */
                    #endregion

                    //SI
                    excelWorksheet.Cells[59, 7].Value = gralSI;
                    //NO
                    excelWorksheet.Cells[60, 7].Value = gralNO;
                    //NA
                    excelWorksheet.Cells[61, 7].Value = gralNA;
                    //Suma Gral
                    excelWorksheet.Cells[62, 7].Value = (gralSI + gralNO + gralNA);
                    //Total Expedientes
                    excelWorksheet.Cells[63, 7].Value = listAdquisiciones.Count;

                    SI = 0;
                    NO = 0;
                    NA = 0;
                    count = 0;
                }
                #endregion
                #region SAN ANDRES CHOLULA
                else if (municipio.Equals("SAN ANDRES CHOLULA"))
                {
                    foreach (AdquisicionesV1 data in listAdquisiciones)
                    {
                        excelWorksheet.Cells[rows, columns].Value = data.numeroAdjudicacion;

                        rows++;
                        rows++;
                        rows++;

                        listDoc = data.documentos;
                        Sort(ref listDoc, "clave", "ASC");

                        foreach (var documents in listDoc)
                        {

                            string val = documents.estatus;

                            if (val == null)
                            {
                                NO++;
                            }
                            else if (val.Equals("NO"))
                            {
                                NO++;
                            }
                            else if (val.Equals("SI"))
                            {
                                SI++;
                            }
                            else if (val.Equals("N/A"))
                            {
                                NA++;
                            }

                            if (documents.grupo.Equals("DOCUMENTACIÓN ADMINISTRATIVA"))
                            {
                                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                rows++;
                                count++;
                                if (count == 5) { rows++; count = 0; }
                            }
                            else if (documents.grupo.Equals("PROCEDIMIENTO DE ADJUDICACIÓN"))
                            {
                                if (data.documentos.Count == 41)
                                {
                                    if (documents.clave == 17)
                                    {
                                        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                        rows++;
                                        rows++;
                                        count++;
                                    }
                                    else
                                    {
                                        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                        rows++;
                                        count++;
                                    }

                                    if (count == 20) { rows++; count = 0; }
                                }
                                else
                                {
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                    rows++;
                                    count++;
                                    if (count == 21) { rows++; count = 0; }
                                }
                            }
                            else if (documents.grupo.Equals("REQUISITOS DEL CONTRATO"))
                            {
                                if (data.documentos.Count == 41)
                                {
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                    rows++;
                                    count++;
                                    if (count == 5) { rows++; rows++; count = 0; }
                                }
                                else if (data.documentos.Count == 42)
                                {
                                    string x = documents.documento;
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                    rows++;
                                    count++;
                                    if (count == 5) { rows++; rows++; count = 0; }
                                }
                                else //if (data.documentos.Count == 43)
                                {
                                    string x = documents.documento;
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                    rows++;
                                    count++;
                                    if (count == 6) { rows++; count = 0; }
                                }
                            }
                            else if (documents.grupo.Equals("GARANTÍAS"))
                            {
                                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                rows++;
                                count++;
                                if (count == 3) { rows++; count = 0; }
                            }
                            else if (documents.grupo.Equals("PROCEDIMIENTO DE PAGO"))
                            {
                                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                rows++;
                                count++;
                                if (count == 8) { rows++; count = 0; }
                            }
                        }

                        gralSI = gralSI + SI;
                        gralNO = gralNO + NO;
                        gralNA = gralNA + NA;

                        //SI
                        excelWorksheet.Cells[59, columns].Value = SI; rows++;
                        //NO
                        excelWorksheet.Cells[60, columns].Value = NO; rows++;
                        //NA
                        excelWorksheet.Cells[61, columns].Value = NA; rows++;
                        //Sumatoria
                        excelWorksheet.Cells[62, columns].Value = (SI + NO + NA);

                        rows = 9;
                        columns++;
                        SI = 0;
                        NO = 0;
                        NA = 0;
                        count = 0;
                    }

                    columns++;
                    //excelWorksheet.Cells[8, columns].Value = "SI"; columns++;
                    //excelWorksheet.Cells[8, columns].Value = "NO"; columns++;
                    //excelWorksheet.Cells[8, columns].Value = "N/A"; columns++;
                    rows++;
                    rows++;
                    rows++;

                    columns = 56;

                    #region SI, NO, N/A
                    /*
                    foreach (AdquisicionesV1 data in listAdquisiciones)
                    {
                        foreach (var documents in data.documentos)
                        {
                            if (documents.grupo.Equals("DOCUMENTACIÓN ADMINISTRATIVA"))
                            {
                                columns = 56;
                                columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BA" + rows + ",\"SI\")"; columns++;
                                //excelWorksheet.Cells[rows, columns].Calculate();  
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BA" + rows + ",\"NO\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BA" + rows + ",\"N/A\")"; columns++;

                                rows++;
                                count++;
                                if (count == 5) { rows++; count = 0; }
                            }
                            else if (documents.grupo.Equals("PROCEDIMIENTO DE ADJUDICACIÓN"))
                            {
                                columns = 56;
                                columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                                rows++;
                                count++;
                                if (count == 13) { rows++; count = 0; }
                            }
                            else if (documents.grupo.Equals("REQUISITOS DEL CONTRATO"))
                            {
                                columns = 56;
                                columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                                rows++;
                                count++;
                                if (count == 16) { rows++; count = 0; }
                            }
                            else if (documents.grupo.Equals("GARANTÍAS"))
                            {
                                columns = 56;
                                columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                                rows++;
                                count++;
                                if (count == 3) { rows++; count = 0; }
                            }
                            else if (documents.grupo.Equals("ENTREGABLES"))
                            {
                                columns = 56;
                                columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                                rows++;
                                count++;
                                if (count == 8) { rows++; count = 0; }
                            }
                        }
                        break;
                    }
                    */
                    #endregion

                    //SI
                    excelWorksheet.Cells[59, 7].Value = gralSI;
                    //NO
                    excelWorksheet.Cells[60, 7].Value = gralNO;
                    //NA
                    excelWorksheet.Cells[61, 7].Value = gralNA;
                    //Suma Gral
                    excelWorksheet.Cells[62, 7].Value = (gralSI + gralNO + gralNA);
                    //Total Expedientes
                    excelWorksheet.Cells[63, 7].Value = listAdquisiciones.Count;

                    SI = 0;
                    NO = 0;
                    NA = 0;
                    count = 0;
                }
                #endregion

                #region Codigo Anterior
                //foreach (AdquisicionesV1 data in listAdquisiciones)
                //{
                //    excelWorksheet.Cells[rows, columns].Value = data.numeroAdjudicacion;

                //    rows++;
                //    rows++;
                //    rows++;

                //    listDoc = data.documentos;
                //    Sort(ref listDoc, "clave", "ASC");

                //    foreach (var documents in listDoc)
                //    {

                //        string val = documents.estatus;

                //        if (val == null)
                //        {
                //            NO++;
                //        }
                //        else if (val.Equals("NO"))
                //        {
                //            NO++;
                //        }
                //        else if (val.Equals("SI"))
                //        {
                //            SI++;
                //        }
                //        else if (val.Equals("N/A"))
                //        {
                //            NA++;
                //        }

                //        if (documents.grupo.Equals("DOCUMENTACIÓN ADMINISTRATIVA"))
                //        {
                //            excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                //            rows++;
                //            count++;
                //            if (count == 5) { rows++; count = 0; }
                //        }
                //        else if (documents.grupo.Equals("PROCEDIMIENTO DE ADJUDICACIÓN"))
                //        {
                //            if(data.documentos.Count == 41)
                //            {
                //                if (documents.clave == 17)
                //                {
                //                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                //                    rows++;
                //                    rows++;
                //                    count++;
                //                }
                //                else
                //                {
                //                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                //                    rows++;
                //                    count++;
                //                }

                //                if (count == 20) { rows++; count = 0; }
                //            }
                //            else
                //            {                                
                //                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                //                rows++;
                //                count++;
                //                if (count == 21) { rows++; count = 0; }
                //            }
                //        }
                //        else if (documents.grupo.Equals("REQUISITOS DEL CONTRATO"))
                //        {
                //            if (data.documentos.Count == 41)
                //            {
                //                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                //                rows++;
                //                count++;
                //                if (count == 5) { rows++; rows++; count = 0; }
                //            }
                //            else if (data.documentos.Count == 42)
                //            {
                //                string x = documents.documento;
                //                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                //                rows++;
                //                count++;
                //                if (count == 5) { rows++; rows++; count = 0; }
                //            }
                //            else //if (data.documentos.Count == 43)
                //            {
                //                string x = documents.documento;
                //                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                //                rows++;
                //                count++;
                //                if (count == 6) { rows++; count = 0; }
                //            }
                //        }
                //        else if (documents.grupo.Equals("GARANTÍAS"))
                //        {
                //            excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                //            rows++;
                //            count++;
                //            if (count == 3) { rows++; count = 0; }
                //        }
                //        else if (documents.grupo.Equals("PROCEDIMIENTO DE PAGO"))
                //        {
                //            excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                //            rows++;
                //            count++;
                //            if (count == 8) { rows++; count = 0; }
                //        }
                //    }

                //    gralSI = gralSI + SI;
                //    gralNO = gralNO + NO;
                //    gralNA = gralNA + NA;

                //    //SI
                //    excelWorksheet.Cells[59, columns].Value = SI; rows++;
                //    //NO
                //    excelWorksheet.Cells[60, columns].Value = NO; rows++;
                //    //NA
                //    excelWorksheet.Cells[61, columns].Value = NA; rows++;
                //    //Sumatoria
                //    excelWorksheet.Cells[62, columns].Value = (SI + NO + NA);

                //    rows = 9;
                //    columns++;
                //    SI = 0;
                //    NO = 0;
                //    NA = 0;
                //    count = 0;
                //}

                //columns++;
                ////excelWorksheet.Cells[8, columns].Value = "SI"; columns++;
                ////excelWorksheet.Cells[8, columns].Value = "NO"; columns++;
                ////excelWorksheet.Cells[8, columns].Value = "N/A"; columns++;
                //rows++;
                //rows++;
                //rows++;

                //columns = 56;

                //#region SI, NO, N/A
                ///*
                //foreach (AdquisicionesV1 data in listAdquisiciones)
                //{
                //    foreach (var documents in data.documentos)
                //    {
                //        if (documents.grupo.Equals("DOCUMENTACIÓN ADMINISTRATIVA"))
                //        {
                //            columns = 56;
                //            columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BA" + rows + ",\"SI\")"; columns++;
                //            //excelWorksheet.Cells[rows, columns].Calculate();  
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BA" + rows + ",\"NO\")"; columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BA" + rows + ",\"N/A\")"; columns++;

                //            rows++;
                //            count++;
                //            if (count == 5) { rows++; count = 0; }
                //        }
                //        else if (documents.grupo.Equals("PROCEDIMIENTO DE ADJUDICACIÓN"))
                //        {
                //            columns = 56;
                //            columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                //            rows++;
                //            count++;
                //            if (count == 13) { rows++; count = 0; }
                //        }
                //        else if (documents.grupo.Equals("REQUISITOS DEL CONTRATO"))
                //        {
                //            columns = 56;
                //            columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                //            rows++;
                //            count++;
                //            if (count == 16) { rows++; count = 0; }
                //        }
                //        else if (documents.grupo.Equals("GARANTÍAS"))
                //        {
                //            columns = 56;
                //            columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                //            rows++;
                //            count++;
                //            if (count == 3) { rows++; count = 0; }
                //        }
                //        else if (documents.grupo.Equals("ENTREGABLES"))
                //        {
                //            columns = 56;
                //            columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                //            rows++;
                //            count++;
                //            if (count == 8) { rows++; count = 0; }
                //        }
                //    }
                //    break;
                //}
                //*/
                //#endregion

                ////SI
                //excelWorksheet.Cells[59, 7].Value = gralSI;
                ////NO
                //excelWorksheet.Cells[60, 7].Value = gralNO;
                ////NA
                //excelWorksheet.Cells[61, 7].Value = gralNA;
                ////Suma Gral
                //excelWorksheet.Cells[62, 7].Value = (gralSI + gralNO + gralNA);
                ////Total Expedientes
                //excelWorksheet.Cells[63, 7].Value = listAdquisiciones.Count;

                //SI = 0;
                //NO = 0;
                //NA = 0;
                //count = 0;
                #endregion


                excelPackage.SaveAs(ms);
            }

            ms.Position = 0;

            Spire.Xls.Workbook workbook = new Spire.Xls.Workbook();
            workbook.LoadFromStream(ms);

            var reporte = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(ms.ToArray())
            };
            reporte.Content.Headers.ContentDisposition =
                new ContentDispositionHeaderValue("attachment")
                {
                    FileName = "ReporteAdquisicionesConsursoInv.xlsx"
                };
            reporte.Content.Headers.ContentType =
                new MediaTypeHeaderValue("application/octet-stream");

            return reporte;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("api/reporte/adquisiciones/sumatoria/adjudicacion/directa/{municipio}/{ejercicio}/{tipoadjudicacion}")]
        public async System.Threading.Tasks.Task<HttpResponseMessage> DescargaReporteSumarioAdquAdjDirecta(string municipio, string ejercicio, string tipoadjudicacion)
        {
            //5e9f5aa3d066036b046ac966 adjquisiciones
            //5e6b246c29e31517f4d48397 obra
            MemoryStream memoryStream = new MemoryStream();

            WebClient client = new WebClient();
            try
            {
                //memoryStream = new MemoryStream(client.DownloadData("C:/Simopa/Templete/TemplateSumariaAdquisicionesAdjDirecta.xlsx"));
                if (municipio.Equals("TEPEACA"))
                {
                    memoryStream = new MemoryStream(client.DownloadData("https://apipbrdevelolop.azurewebsites.net/templates/reportes/TemplateSumariaAdquisicionesAdjDirecta.xlsx"));
                }
                else if (municipio.Equals("SAN ANDRES CHOLULA"))
                {
                    memoryStream = new MemoryStream(client.DownloadData("https://apipbrdevelolop.azurewebsites.net/templates/reportes/TemplateSumariaAdquisicionesAdjDirectaSach.xlsx"));
                }
            }
            finally
            {
                client.Dispose();
            }

            string id = string.Empty;
            string constr = ConfigurationManager.AppSettings["connectionString"];
            var Client = new MongoClient(constr);
            var DB = Client.GetDatabase("PRB");

            List<AdquisicionesV1> listAdquisiciones = new List<AdquisicionesV1>();
            List<ObraPublicaV1> listObras = new List<ObraPublicaV1>();

            try
            {
                var collAdqui = DB.GetCollection<AdquisicionesV1>("Adquisiciones");
                var filterAdqui = Builders<AdquisicionesV1>.Filter.Eq(x => x.municipio, municipio)
                    & Builders<AdquisicionesV1>.Filter.Eq(x => x.ejercicio, ejercicio)
                    & Builders<AdquisicionesV1>.Filter.Eq(x => x.tipoAdjudicacion, tipoadjudicacion);
                    //& Builders<AdquisicionesV1>.Filter.Eq(x => x.numeroAdjudicacion, "CMA-AD.EA.SACH_414_2019");
                //& Builders<AdquisicionesV1>.Filter.Eq(x => x.numeroAdjudicacion, "16419040");
                var adquisiciones = await collAdqui.Find(filterAdqui).Sort("{ejercicio:-1}").ToListAsync();

                if (adquisiciones.Count > 0)
                {
                    listAdquisiciones = adquisiciones;
                }
                else
                {
                    listAdquisiciones = new List<AdquisicionesV1>();
                }
            }
            catch (Exception ex)
            {
                //response.success = false;
                //response.messages.Add(ex.ToString());
                //return Ok(ex.ToString());
            }

            //Imagen           
            int Height = 0;
            int Width = 0;
            int rowIndex = 0;
            int colIndex = 0;
            string nameImage = string.Empty;
            WebClient clientImage = new WebClient();
            MemoryStream memoryImage = new MemoryStream();
            try
            {
                if (municipio.Equals("TEPEACA"))
                {
                    //memoryImage = new MemoryStream(clientImage.DownloadData("C:/Simopa/Templete/Logo/LogoTepeaca.png"));
                    memoryImage = new MemoryStream(client.DownloadData("https://apipbrdevelolop.azurewebsites.net/templates/logo/LogoTepeaca.png"));
                    nameImage = "Tepeaca";
                    Height = 160;
                    Width = 150;
                    rowIndex = 1;
                    colIndex = 1;
                }
                else if (municipio.Equals("SAN ANDRES CHOLULA"))
                {
                    //memoryImage = new MemoryStream(clientImage.DownloadData("C:/Simopa/Templete/Logo/LogoDespacho.png"));
                    memoryImage = new MemoryStream(client.DownloadData("https://apipbrdevelolop.azurewebsites.net/templates/logo/LogoDespacho.png"));
                    nameImage = "ESACH"; 
                    Height = 100;
                    Width = 150;
                    rowIndex = 4;
                    colIndex = 1;
                }
                else
                {
                    //memoryImage = new MemoryStream(clientImage.DownloadData("C:/Simopa/Templete/Logo/LogoDespacho.png"));
                    memoryImage = new MemoryStream(client.DownloadData("https://apipbrdevelolop.azurewebsites.net/templates/logo/LogoDespacho.png"));
                }
            }
            catch (Exception ex)
            {
                clientImage.Dispose();
            }



            MemoryStream ms = new MemoryStream();
            //Envia datos a Excel
            using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {

                ExcelWorkbook excelWorkBook = excelPackage.Workbook;
                ExcelWorksheet excelWorksheet = excelWorkBook.Worksheets[1];
                excelWorksheet.Workbook.CalcMode = ExcelCalcMode.Manual;

                //Imagen
                Image img = Image.FromStream(memoryImage);
                ExcelPicture pic = excelWorksheet.Drawings.AddPicture("imageVista-" + nameImage, img);
                pic.SetPosition(rowIndex, 0, colIndex + ((1 - 1) * 4), 0);
                pic.SetSize(Width, Height);

                int rows = 0;
                int columns = 0;
                //int count = 1;
                DateTime fecha = DateTime.Now;

                if (municipio.Equals("TEPEACA"))
                {
                    excelWorksheet.Cells[5, 4].Value = "CÉDULA ANALÍTICA DE REVISIÓN DE PROCEDIMIENTO DE ADJUDICACIÓN DIRECTA " + ejercicio;
                    excelWorksheet.Cells[7, 4].Value = "LIMT- LEY DE INGRESOS DEL MUNICIPIO DE TEPEACA PARA EL  " + ejercicio;
                    rows = 9;
                    columns = 8;
                }
                else if (municipio.Equals("SAN ANDRES CHOLULA"))
                {
                    excelWorksheet.Cells[5, 4].Value = "CÉDULA ANALÍTICA DE REVISIÓN DE PROCEDIMIENTO DE ADJUDICACIÓN DIRECTA " + ejercicio;
                    excelWorksheet.Cells[7, 4].Value = "LIMSACH- LEY DE INGRESOS DEL MUNICIPIO DE SAN ANDRES CHOLULA PARA EL " + ejercicio;
                    rows = 9;
                    columns = 8;
                }                

                int SI = 0;
                int NO = 0;
                int NA = 0;
                int count = 0;

                int gralSI = 0;
                int gralNO = 0;
                int gralNA = 0;

                DocumentosAdquisicionesSanAndresCholula documentos = new DocumentosAdquisicionesSanAndresCholula();
                List<DocumentoAdquisicionesV1> listDoc = new List<DocumentoAdquisicionesV1>();

                #region TEPEACA
                if (municipio.Equals("TEPEACA"))
                {
                    foreach (AdquisicionesV1 data in listAdquisiciones)
                    {
                        excelWorksheet.Cells[rows, columns].Value = data.numeroAdjudicacion;

                        rows++;
                        rows++;
                        rows++;

                        listDoc = data.documentos;
                        Sort(ref listDoc, "clave", "ASC");

                        //41 version 1
                        //Version Excel 38

                        foreach (var documents in listDoc)
                        {
                            string val = documents.estatus;

                            if (val == null)
                            {
                                NO++;
                            }
                            else if (val.Equals("NO"))
                            {
                                NO++;
                            }
                            else if (val.Equals("SI"))
                            {
                                SI++;
                            }
                            else if (val.Equals("N/A"))
                            {
                                NA++;
                            }

                            string doc = documents.documento;
                            string doc1 = string.Empty;

                            if (listDoc.Count == 37)
                            {
                                if (documents.clave >= 1 & documents.clave <= 4)
                                {
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                    rows++;
                                    count++;
                                }
                                else if (documents.clave == 5)
                                {
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                    rows++; rows++;
                                    count++;
                                }
                                else if (documents.clave >= 6 & documents.clave <= 15)
                                {
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                    rows++;
                                    count++;
                                }
                                else if (documents.clave == 16)
                                {
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                    rows++; rows++;
                                    count++;
                                }
                                else if (documents.clave >= 17 & documents.clave <= 26)
                                {
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                    rows++;
                                    count++;
                                }
                                else if (documents.clave == 27)
                                {
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                    rows++; rows++;
                                    count++;
                                }
                                else if (documents.clave == 28)
                                {
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                    rows++;
                                    count++;
                                }
                                else if (documents.clave == 29)
                                {
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                    rows++; rows++;
                                    count++;
                                }
                                else if (documents.clave >= 30)
                                {
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                    rows++;
                                    count++;
                                }
                            }
                            else if (listDoc.Count == 38)
                            {
                                if (documents.clave >= 1 & documents.clave <= 4)
                                {
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                    rows++;
                                    count++;
                                }
                                else if (documents.clave == 5)
                                {
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                    rows++; rows++;
                                    count++;
                                }
                                else if (documents.clave >= 6 & documents.clave <= 27)
                                {
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                    rows++;
                                    count++;
                                }
                                else if (documents.clave == 28)
                                {
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                    rows++; rows++;
                                    count++;
                                }
                                else if (documents.clave == 29)
                                {
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                    rows++;
                                    count++;
                                }
                                else if (documents.clave == 30)
                                {
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                    rows++; rows++;
                                    count++;
                                }
                                else if (documents.clave >= 31)
                                {
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                    rows++;
                                    count++;
                                }
                            }
                            /*
                            else if (listDoc.Count == 41)
                            {
                                if (documents.grupo.Equals("DOCUMENTACIÓN ADMINISTRATIVA"))
                                {
                                    if (documents.clave != 1)
                                    {
                                        if (documents.clave == 2)
                                        {
                                            excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                            rows++;
                                            rows++;
                                            count++;
                                        }
                                        else
                                        {
                                            excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                            rows++;
                                            count++;
                                            if (count == 4) { rows++; count = 0; }
                                        }
                                    }
                                }
                                if (documents.clave == 6)
                                {
                                    rows++; rows++; rows++;
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                    rows++; rows++; rows++; rows++;
                                    count++;
                                }
                                else if (documents.clave >= 7 & documents.clave <= 14)
                                {
                                    string vacio = string.Empty;
                                }
                                else if (documents.clave == 15)
                                {
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                    rows++;
                                    count++;
                                }
                                else if (documents.clave == 16)
                                {
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                    rows++;
                                    count++;
                                }
                                else if (documents.clave == 17)
                                {
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                    rows++; rows++; rows++; rows++;
                                    count++;
                                }
                                else if (documents.clave == 19)
                                {
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                    rows++;
                                    count++;
                                }
                                else if (documents.clave == 20)
                                {
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                    rows++;
                                    count++;
                                }
                                else if (documents.clave == 21)
                                {
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                    rows++; rows++;
                                    count++;
                                }
                                else if (documents.clave == 22)
                                {
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                    rows++; rows++; rows++; rows++;
                                    count++;
                                }
                                else if (documents.clave == 25)
                                {
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                    rows++;
                                    count++;
                                }
                                else if (documents.clave == 26)
                                {
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                    rows = 0;
                                    count++;
                                }
                                else if (documents.clave == 27)
                                {
                                    excelWorksheet.Cells[21, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                }
                                else if (documents.clave == 28)
                                {
                                    excelWorksheet.Cells[22, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                }
                                else if (documents.clave == 29)
                                {
                                    excelWorksheet.Cells[23, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                }
                                else if (documents.clave == 30)
                                {
                                    excelWorksheet.Cells[35, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                }
                                else if (documents.clave == 31)
                                {
                                    excelWorksheet.Cells[36, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                }
                                if (documents.clave == 33)
                                {
                                    excelWorksheet.Cells[41, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                }
                                if (documents.clave == 34)
                                {
                                    excelWorksheet.Cells[42, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                    rows = 44;
                                    count = 0;
                                }
                                else if (documents.grupo.Equals("PROCEDIMIENTO DE PAGO"))
                                {
                                    if (documents.clave >= 35)
                                    {
                                        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                        rows++;
                                        count++;
                                        if (count == 8) { rows++; count = 0; }
                                    }
                                }
                            }
                            */
                        }



                        //db.getCollection('Fundamentos').find({ municipio: "TEPEACA",procedimiento: "INVITACIÓN A CUANDO MENOS 3 PERSONAS"}).sort({ clave: 1})
                        //foreach (var documents in data.documentos)
                        //{
                        //    //excelWorksheet.Cells[rows, columns].Value = (new Common.Common()).FundamentosAdquisiciones(municipio, data.tipoAdjudicacion, documents.documento); columns++;
                        //    //excelWorksheet.Cells[rows, columns].Value = documents.documento; columns++;

                        //    string val = documents.estatus;

                        //    if (val == null)
                        //    {
                        //        NO++;
                        //    }
                        //    else if (val.Equals("NO"))
                        //    {
                        //        NO++;
                        //    }
                        //    else if (val.Equals("SI"))
                        //    {
                        //        SI++;
                        //    }
                        //    else if (val.Equals("N/A"))
                        //    {
                        //        NA++;
                        //    }

                        //    if (documents.grupo.Equals("DOCUMENTACIÓN ADMINISTRATIVA"))
                        //    {
                        //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                        //        rows++;
                        //        count++;
                        //        if (count == 5) { rows++; count = 0; }
                        //    }
                        //    else if (documents.grupo.Equals("PROCEDIMIENTO DE ADJUDICACIÓN"))
                        //    {
                        //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                        //        rows++;
                        //        count++;
                        //        if (count == 20) { rows++; rows++; rows++; count = 0; } //4
                        //    }
                        //    else if (documents.grupo.Equals("REQUISITOS DEL CONTRATO"))
                        //    {
                        //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                        //        rows++;
                        //        count++;
                        //        if (count == 5) { rows++; count = 0; } //5  //18
                        //    }
                        //    else if (documents.grupo.Equals("GARANTÍAS"))
                        //    {
                        //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                        //        rows++;
                        //        count++;
                        //        if (count == 9) { rows++; count = 0; }//4
                        //    }
                        //    else if (documents.grupo.Equals("ENTREGABLES"))
                        //    {
                        //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                        //        rows++;
                        //        count++;
                        //        if (count == 8) { rows++; count = 0; }
                        //    }
                        //}


                        gralSI = gralSI + SI;
                        gralNO = gralNO + NO;
                        gralNA = gralNA + NA;

                        //SI
                        excelWorksheet.Cells[53, columns].Value = SI; rows++;
                        //NO
                        excelWorksheet.Cells[54, columns].Value = NO; rows++;
                        //NA
                        excelWorksheet.Cells[55, columns].Value = NA; rows++;
                        //Sumatoria
                        excelWorksheet.Cells[56, columns].Value = (SI + NO + NA);

                        rows = 9;
                        columns++;
                        SI = 0;
                        NO = 0;
                        NA = 0;
                        count = 0;
                    }

                    columns++;
                    //excelWorksheet.Cells[8, columns].Value = "SI"; columns++;
                    //excelWorksheet.Cells[8, columns].Value = "NO"; columns++;
                    //excelWorksheet.Cells[8, columns].Value = "N/A"; columns++;
                    rows++;
                    rows++;
                    rows++;

                    columns = 56;

                    #region SI, NO, N/A
                    /*
                    foreach (AdquisicionesV1 data in listAdquisiciones)
                    {
                        foreach (var documents in data.documentos)
                        {
                            if (documents.grupo.Equals("DOCUMENTACIÓN ADMINISTRATIVA"))
                            {
                                columns = 56;
                                columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BA" + rows + ",\"SI\")"; columns++;
                                //excelWorksheet.Cells[rows, columns].Calculate();  
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BA" + rows + ",\"NO\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BA" + rows + ",\"N/A\")"; columns++;

                                rows++;
                                count++;
                                if (count == 5) { rows++; count = 0; }
                            }
                            else if (documents.grupo.Equals("PROCEDIMIENTO DE ADJUDICACIÓN"))
                            {
                                columns = 56;
                                columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                                rows++;
                                count++;
                                if (count == 13) { rows++; count = 0; }
                            }
                            else if (documents.grupo.Equals("REQUISITOS DEL CONTRATO"))
                            {
                                columns = 56;
                                columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                                rows++;
                                count++;
                                if (count == 16) { rows++; count = 0; }
                            }
                            else if (documents.grupo.Equals("GARANTÍAS"))
                            {
                                columns = 56;
                                columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                                rows++;
                                count++;
                                if (count == 3) { rows++; count = 0; }
                            }
                            else if (documents.grupo.Equals("ENTREGABLES"))
                            {
                                columns = 56;
                                columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                                rows++;
                                count++;
                                if (count == 8) { rows++; count = 0; }
                            }
                        }
                        break;
                    }
                    */
                    #endregion

                    //SI
                    excelWorksheet.Cells[53, 7].Value = gralSI;
                    //NO
                    excelWorksheet.Cells[54, 7].Value = gralNO;
                    //NA
                    excelWorksheet.Cells[55, 7].Value = gralNA;
                    //Suma Gral
                    excelWorksheet.Cells[56, 7].Value = (gralSI + gralNO + gralNA);
                    //Total Expedientes
                    excelWorksheet.Cells[57, 7].Value = listAdquisiciones.Count;

                    SI = 0;
                    NO = 0;
                    NA = 0;
                    count = 0;
                }
                #endregion
                #region SAN ANDRES CHOLULA
                else if (municipio.Equals("SAN ANDRES CHOLULA"))
                {
                    foreach (AdquisicionesV1 data in listAdquisiciones)
                    {
                        excelWorksheet.Cells[rows, columns].Value = data.numeroAdjudicacion;

                        rows++;
                        rows++;
                        rows++;

                        listDoc = data.documentos;
                        Sort(ref listDoc, "clave", "ASC");

                        //41 version 1
                        //Version Excel 38

                        foreach (var documents in listDoc)
                        {
                            string val = documents.estatus;

                            if (val == null)
                            {
                                NO++;
                            }
                            else if (val.Equals("NO"))
                            {
                                NO++;
                            }
                            else if (val.Equals("SI"))
                            {
                                SI++;
                            }
                            else if (val.Equals("N/A"))
                            {
                                NA++;
                            }

                            string doc = documents.documento;
                            string doc1 = string.Empty;

                            if (listDoc.Count == 37)
                            {
                                if (documents.clave >= 1 & documents.clave <= 4)
                                {
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                    rows++;
                                    count++;
                                }
                                else if (documents.clave == 5)
                                {
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                    rows++; rows++;
                                    count++;
                                }
                                else if (documents.clave >= 6 & documents.clave <= 15)
                                {
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                    rows++;
                                    count++;
                                }
                                else if (documents.clave == 16)
                                {
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                    rows++; rows++;
                                    count++;
                                }
                                else if (documents.clave >= 17 & documents.clave <= 26)
                                {
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                    rows++;
                                    count++;
                                }
                                else if (documents.clave == 27)
                                {
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                    rows++; rows++;
                                    count++;
                                }
                                else if (documents.clave == 28)
                                {
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                    rows++;
                                    count++;
                                }
                                else if (documents.clave == 29)
                                {
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                    rows++; rows++;
                                    count++;
                                }
                                else if (documents.clave >= 30)
                                {
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                    rows++;
                                    count++;
                                }
                            }
                            else if(listDoc.Count == 38)
                            {
                                if (documents.clave >= 1 & documents.clave <= 4)
                                {
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                    rows++;
                                    count++;
                                }
                                else if (documents.clave == 5)
                                {
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                    rows++; rows++;
                                    count++;
                                }
                                else if (documents.clave >= 6 & documents.clave <= 27)
                                {
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                    rows++;
                                    count++;
                                }
                                else if (documents.clave == 28)
                                {
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                    rows++; rows++;
                                    count++;
                                }
                                else if (documents.clave == 29)
                                {
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                    rows++;
                                    count++;
                                }
                                else if (documents.clave == 30)
                                {
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                    rows++; rows++;
                                    count++;
                                }
                                else if (documents.clave >= 31)
                                {
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                    rows++;
                                    count++;
                                }
                                //else if (documents.clave == 29)
                                //{
                                //    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                //    rows++; rows++;
                                //    count++;
                                //}
                                //else if (documents.clave >= 30)
                                //{
                                //    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                //    rows++;
                                //    count++;
                                //}
                            }
                           /*
                            else if (listDoc.Count == 41)
                            {
                                if (documents.grupo.Equals("DOCUMENTACIÓN ADMINISTRATIVA"))
                                {
                                    if (documents.clave != 1)
                                    {
                                        if (documents.clave == 2)
                                        {
                                            excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                            rows++;
                                            rows++;
                                            count++;
                                        }
                                        else
                                        {
                                            excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                            rows++;
                                            count++;
                                            if (count == 4) { rows++; count = 0; }
                                        }
                                    }
                                }
                                if (documents.clave == 6)
                                {
                                    rows++; rows++; rows++;
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                    rows++; rows++; rows++; rows++;
                                    count++;
                                }
                                else if (documents.clave >= 7 & documents.clave <= 14)
                                {
                                    string vacio = string.Empty;
                                }
                                else if (documents.clave == 15)
                                {
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                    rows++;
                                    count++;
                                }
                                else if (documents.clave == 16)
                                {
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                    rows++;
                                    count++;
                                }
                                else if (documents.clave == 17)
                                {
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                    rows++; rows++; rows++; rows++;
                                    count++;
                                }
                                else if (documents.clave == 19)
                                {
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                    rows++;
                                    count++;
                                }
                                else if (documents.clave == 20)
                                {
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                    rows++;
                                    count++;
                                }
                                else if (documents.clave == 21)
                                {
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                    rows++; rows++;
                                    count++;
                                }
                                else if (documents.clave == 22)
                                {
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                    rows++; rows++; rows++; rows++;
                                    count++;
                                }
                                else if (documents.clave == 25)
                                {
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                    rows++;
                                    count++;
                                }
                                else if (documents.clave == 26)
                                {
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                    rows = 0;
                                    count++;
                                }
                                else if (documents.clave == 27)
                                {
                                    excelWorksheet.Cells[21, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                }
                                else if (documents.clave == 28)
                                {
                                    excelWorksheet.Cells[22, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                }
                                else if (documents.clave == 29)
                                {
                                    excelWorksheet.Cells[23, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                }
                                else if (documents.clave == 30)
                                {
                                    excelWorksheet.Cells[35, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                }
                                else if (documents.clave == 31)
                                {
                                    excelWorksheet.Cells[36, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                }
                                if (documents.clave == 33)
                                {
                                    excelWorksheet.Cells[41, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                }
                                if (documents.clave == 34)
                                {
                                    excelWorksheet.Cells[42, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                    rows = 44;
                                    count = 0;
                                }
                                else if (documents.grupo.Equals("PROCEDIMIENTO DE PAGO"))
                                {
                                    if (documents.clave >= 35)
                                    {
                                        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                                        rows++;
                                        count++;
                                        if (count == 8) { rows++; count = 0; }
                                    }
                                }
                            }
                            */
                        }

                        /*

                        //db.getCollection('Fundamentos').find({ municipio: "TEPEACA",procedimiento: "INVITACIÓN A CUANDO MENOS 3 PERSONAS"}).sort({ clave: 1})
                        //foreach (var documents in data.documentos)
                        //{
                        //    //excelWorksheet.Cells[rows, columns].Value = (new Common.Common()).FundamentosAdquisiciones(municipio, data.tipoAdjudicacion, documents.documento); columns++;
                        //    //excelWorksheet.Cells[rows, columns].Value = documents.documento; columns++;

                        //    string val = documents.estatus;

                        //    if (val == null)
                        //    {
                        //        NO++;
                        //    }
                        //    else if (val.Equals("NO"))
                        //    {
                        //        NO++;
                        //    }
                        //    else if (val.Equals("SI"))
                        //    {
                        //        SI++;
                        //    }
                        //    else if (val.Equals("N/A"))
                        //    {
                        //        NA++;
                        //    }

                        //    if (documents.grupo.Equals("DOCUMENTACIÓN ADMINISTRATIVA"))
                        //    {
                        //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                        //        rows++;
                        //        count++;
                        //        if (count == 5) { rows++; count = 0; }
                        //    }
                        //    else if (documents.grupo.Equals("PROCEDIMIENTO DE ADJUDICACIÓN"))
                        //    {
                        //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                        //        rows++;
                        //        count++;
                        //        if (count == 20) { rows++; rows++; rows++; count = 0; } //4
                        //    }
                        //    else if (documents.grupo.Equals("REQUISITOS DEL CONTRATO"))
                        //    {
                        //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                        //        rows++;
                        //        count++;
                        //        if (count == 5) { rows++; count = 0; } //5  //18
                        //    }
                        //    else if (documents.grupo.Equals("GARANTÍAS"))
                        //    {
                        //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                        //        rows++;
                        //        count++;
                        //        if (count == 9) { rows++; count = 0; }//4
                        //    }
                        //    else if (documents.grupo.Equals("ENTREGABLES"))
                        //    {
                        //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                        //        rows++;
                        //        count++;
                        //        if (count == 8) { rows++; count = 0; }
                        //    }
                        //}
                        */


                        gralSI = gralSI + SI;
                        gralNO = gralNO + NO;
                        gralNA = gralNA + NA;

                        //SI
                        excelWorksheet.Cells[53, columns].Value = SI; rows++;
                        //NO
                        excelWorksheet.Cells[54, columns].Value = NO; rows++;
                        //NA
                        excelWorksheet.Cells[55, columns].Value = NA; rows++;
                        //Sumatoria
                        excelWorksheet.Cells[56, columns].Value = (SI + NO + NA);

                        rows = 9;
                        columns++;
                        SI = 0;
                        NO = 0;
                        NA = 0;
                        count = 0;
                    }

                    columns++;
                    //excelWorksheet.Cells[8, columns].Value = "SI"; columns++;
                    //excelWorksheet.Cells[8, columns].Value = "NO"; columns++;
                    //excelWorksheet.Cells[8, columns].Value = "N/A"; columns++;
                    rows++;
                    rows++;
                    rows++;

                    columns = 56;

                    #region SI, NO, N/A
                    /*
                    foreach (AdquisicionesV1 data in listAdquisiciones)
                    {
                        foreach (var documents in data.documentos)
                        {
                            if (documents.grupo.Equals("DOCUMENTACIÓN ADMINISTRATIVA"))
                            {
                                columns = 56;
                                columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BA" + rows + ",\"SI\")"; columns++;
                                //excelWorksheet.Cells[rows, columns].Calculate();  
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BA" + rows + ",\"NO\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BA" + rows + ",\"N/A\")"; columns++;

                                rows++;
                                count++;
                                if (count == 5) { rows++; count = 0; }
                            }
                            else if (documents.grupo.Equals("PROCEDIMIENTO DE ADJUDICACIÓN"))
                            {
                                columns = 56;
                                columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                                rows++;
                                count++;
                                if (count == 13) { rows++; count = 0; }
                            }
                            else if (documents.grupo.Equals("REQUISITOS DEL CONTRATO"))
                            {
                                columns = 56;
                                columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                                rows++;
                                count++;
                                if (count == 16) { rows++; count = 0; }
                            }
                            else if (documents.grupo.Equals("GARANTÍAS"))
                            {
                                columns = 56;
                                columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                                rows++;
                                count++;
                                if (count == 3) { rows++; count = 0; }
                            }
                            else if (documents.grupo.Equals("ENTREGABLES"))
                            {
                                columns = 56;
                                columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                                rows++;
                                count++;
                                if (count == 8) { rows++; count = 0; }
                            }
                        }
                        break;
                    }
                    */
                    #endregion

                    //SI
                    excelWorksheet.Cells[53, 7].Value = gralSI;
                    //NO
                    excelWorksheet.Cells[54, 7].Value = gralNO;
                    //NA
                    excelWorksheet.Cells[55, 7].Value = gralNA;
                    //Suma Gral
                    excelWorksheet.Cells[56, 7].Value = (gralSI + gralNO + gralNA);
                    //Total Expedientes
                    excelWorksheet.Cells[57, 7].Value = listAdquisiciones.Count;

                    SI = 0;
                    NO = 0;
                    NA = 0;
                    count = 0;
                }
                #endregion

                #region Anterior
                //foreach (AdquisicionesV1 data in listAdquisiciones)
                //{
                //    excelWorksheet.Cells[rows, columns].Value = data.numeroAdjudicacion;

                //    rows++;
                //    rows++;
                //    rows++;

                //    listDoc = data.documentos;
                //    Sort(ref listDoc, "clave", "ASC");

                //    //41 version 1
                //    //Version Excel 38

                //    foreach (var documents in listDoc)
                //    {
                //        string val = documents.estatus;

                //        if (val == null)
                //        {
                //            NO++;
                //        }
                //        else if (val.Equals("NO"))
                //        {
                //            NO++;
                //        }
                //        else if (val.Equals("SI"))
                //        {
                //            SI++;
                //        }
                //        else if (val.Equals("N/A"))
                //        {
                //            NA++;
                //        }

                //        string doc = documents.documento;
                //        string doc1 = string.Empty;

                //        if (listDoc.Count == 37)
                //        {
                //            if (documents.clave >= 1 & documents.clave <= 4)
                //            {
                //                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                //                rows++;
                //                count++;
                //            }
                //            else if (documents.clave == 5)
                //            {
                //                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                //                rows++; rows++;
                //                count++;
                //            }
                //            else if (documents.clave >= 6 & documents.clave <= 15)
                //            {
                //                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                //                rows++;
                //                count++;
                //            }
                //            else if (documents.clave == 16)
                //            {
                //                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                //                rows++; rows++;
                //                count++;
                //            }
                //            else if (documents.clave >= 17 & documents.clave <= 26)
                //            {
                //                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                //                rows++;
                //                count++;
                //            }
                //            else if (documents.clave == 27)
                //            {
                //                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                //                rows++; rows++;
                //                count++;
                //            }
                //            else if (documents.clave == 28)
                //            {
                //                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                //                rows++;
                //                count++;
                //            }
                //            else if (documents.clave == 29)
                //            {
                //                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                //                rows++; rows++;
                //                count++;
                //            }
                //            else if (documents.clave >= 30)
                //            {
                //                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                //                rows++;
                //                count++;
                //            }
                //        }
                //        else if (listDoc.Count == 41)
                //        {
                //            if (documents.grupo.Equals("DOCUMENTACIÓN ADMINISTRATIVA"))
                //            {
                //                if (documents.clave != 1)
                //                {
                //                    if (documents.clave == 2)
                //                    {
                //                        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                //                        rows++;
                //                        rows++;
                //                        count++;
                //                    }
                //                    else
                //                    {
                //                        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                //                        rows++;
                //                        count++;
                //                        if (count == 4) { rows++; count = 0; }
                //                    }
                //                }
                //            }
                //            if (documents.clave == 6)
                //            {
                //                rows++; rows++; rows++;
                //                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                //                rows++; rows++; rows++; rows++;
                //                count++;
                //            }
                //            else if (documents.clave >= 7 & documents.clave <= 14)
                //            {
                //                string vacio = string.Empty;
                //            }
                //            else if (documents.clave == 15)
                //            {
                //                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                //                rows++;
                //                count++;
                //            }
                //            else if (documents.clave == 16)
                //            {
                //                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                //                rows++;
                //                count++;
                //            }
                //            else if (documents.clave == 17)
                //            {
                //                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                //                rows++; rows++; rows++; rows++;
                //                count++;
                //            }
                //            else if (documents.clave == 19)
                //            {
                //                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                //                rows++;
                //                count++;
                //            }
                //            else if (documents.clave == 20)
                //            {
                //                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                //                rows++;
                //                count++;
                //            }
                //            else if (documents.clave == 21)
                //            {
                //                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                //                rows++; rows++;
                //                count++;
                //            }
                //            else if (documents.clave == 22)
                //            {
                //                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                //                rows++; rows++; rows++; rows++;
                //                count++;
                //            }
                //            else if (documents.clave == 25)
                //            {
                //                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                //                rows++;
                //                count++;
                //            }
                //            else if (documents.clave == 26)
                //            {
                //                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                //                rows = 0;
                //                count++;
                //            }
                //            else if (documents.clave == 27)
                //            {
                //                excelWorksheet.Cells[21, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                //            }
                //            else if (documents.clave == 28)
                //            {
                //                excelWorksheet.Cells[22, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                //            }
                //            else if (documents.clave == 29)
                //            {
                //                excelWorksheet.Cells[23, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                //            }
                //            else if (documents.clave == 30)
                //            {
                //                excelWorksheet.Cells[35, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                //            }
                //            else if (documents.clave == 31)
                //            {
                //                excelWorksheet.Cells[36, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                //            }
                //            if (documents.clave == 33)
                //            {
                //                excelWorksheet.Cells[41, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                //            }
                //            if (documents.clave == 34)
                //            {
                //                excelWorksheet.Cells[42, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                //                rows = 44;
                //                count = 0;
                //            }
                //            else if (documents.grupo.Equals("PROCEDIMIENTO DE PAGO"))
                //            {
                //                if (documents.clave >= 35)
                //                {
                //                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                //                    rows++;
                //                    count++;
                //                    if (count == 8) { rows++; count = 0; }
                //                }
                //            }
                //        }
                //    }



                //    //db.getCollection('Fundamentos').find({ municipio: "TEPEACA",procedimiento: "INVITACIÓN A CUANDO MENOS 3 PERSONAS"}).sort({ clave: 1})
                //    //foreach (var documents in data.documentos)
                //    //{
                //    //    //excelWorksheet.Cells[rows, columns].Value = (new Common.Common()).FundamentosAdquisiciones(municipio, data.tipoAdjudicacion, documents.documento); columns++;
                //    //    //excelWorksheet.Cells[rows, columns].Value = documents.documento; columns++;

                //    //    string val = documents.estatus;

                //    //    if (val == null)
                //    //    {
                //    //        NO++;
                //    //    }
                //    //    else if (val.Equals("NO"))
                //    //    {
                //    //        NO++;
                //    //    }
                //    //    else if (val.Equals("SI"))
                //    //    {
                //    //        SI++;
                //    //    }
                //    //    else if (val.Equals("N/A"))
                //    //    {
                //    //        NA++;
                //    //    }

                //    //    if (documents.grupo.Equals("DOCUMENTACIÓN ADMINISTRATIVA"))
                //    //    {
                //    //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                //    //        rows++;
                //    //        count++;
                //    //        if (count == 5) { rows++; count = 0; }
                //    //    }
                //    //    else if (documents.grupo.Equals("PROCEDIMIENTO DE ADJUDICACIÓN"))
                //    //    {
                //    //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                //    //        rows++;
                //    //        count++;
                //    //        if (count == 20) { rows++; rows++; rows++; count = 0; } //4
                //    //    }
                //    //    else if (documents.grupo.Equals("REQUISITOS DEL CONTRATO"))
                //    //    {
                //    //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                //    //        rows++;
                //    //        count++;
                //    //        if (count == 5) { rows++; count = 0; } //5  //18
                //    //    }
                //    //    else if (documents.grupo.Equals("GARANTÍAS"))
                //    //    {
                //    //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                //    //        rows++;
                //    //        count++;
                //    //        if (count == 9) { rows++; count = 0; }//4
                //    //    }
                //    //    else if (documents.grupo.Equals("ENTREGABLES"))
                //    //    {
                //    //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documents.estatus) ? "NO" : documents.estatus;
                //    //        rows++;
                //    //        count++;
                //    //        if (count == 8) { rows++; count = 0; }
                //    //    }
                //    //}


                //    gralSI = gralSI + SI;
                //    gralNO = gralNO + NO;
                //    gralNA = gralNA + NA;

                //    //SI
                //    excelWorksheet.Cells[53, columns].Value = SI; rows++;
                //    //NO
                //    excelWorksheet.Cells[54, columns].Value = NO; rows++;
                //    //NA
                //    excelWorksheet.Cells[55, columns].Value = NA; rows++;
                //    //Sumatoria
                //    excelWorksheet.Cells[56, columns].Value = (SI + NO + NA);

                //    rows = 9;
                //    columns++;
                //    SI = 0;
                //    NO = 0;
                //    NA = 0;
                //    count = 0;
                //}

                //columns++;
                ////excelWorksheet.Cells[8, columns].Value = "SI"; columns++;
                ////excelWorksheet.Cells[8, columns].Value = "NO"; columns++;
                ////excelWorksheet.Cells[8, columns].Value = "N/A"; columns++;
                //rows++;
                //rows++;
                //rows++;

                //columns = 56;

                //#region SI, NO, N/A
                ///*
                //foreach (AdquisicionesV1 data in listAdquisiciones)
                //{
                //    foreach (var documents in data.documentos)
                //    {
                //        if (documents.grupo.Equals("DOCUMENTACIÓN ADMINISTRATIVA"))
                //        {
                //            columns = 56;
                //            columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BA" + rows + ",\"SI\")"; columns++;
                //            //excelWorksheet.Cells[rows, columns].Calculate();  
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BA" + rows + ",\"NO\")"; columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BA" + rows + ",\"N/A\")"; columns++;
                            
                //            rows++;
                //            count++;
                //            if (count == 5) { rows++; count = 0; }
                //        }
                //        else if (documents.grupo.Equals("PROCEDIMIENTO DE ADJUDICACIÓN"))
                //        {
                //            columns = 56;
                //            columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                //            rows++;
                //            count++;
                //            if (count == 13) { rows++; count = 0; }
                //        }
                //        else if (documents.grupo.Equals("REQUISITOS DEL CONTRATO"))
                //        {
                //            columns = 56;
                //            columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                //            rows++;
                //            count++;
                //            if (count == 16) { rows++; count = 0; }
                //        }
                //        else if (documents.grupo.Equals("GARANTÍAS"))
                //        {
                //            columns = 56;
                //            columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                //            rows++;
                //            count++;
                //            if (count == 3) { rows++; count = 0; }
                //        }
                //        else if (documents.grupo.Equals("ENTREGABLES"))
                //        {
                //            columns = 56;
                //            columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                //            rows++;
                //            count++;
                //            if (count == 8) { rows++; count = 0; }
                //        }
                //    }
                //    break;
                //}
                //*/
                //#endregion

                ////SI
                //excelWorksheet.Cells[53, 7].Value = gralSI;
                ////NO
                //excelWorksheet.Cells[54, 7].Value = gralNO;
                ////NA
                //excelWorksheet.Cells[55, 7].Value = gralNA;
                ////Suma Gral
                //excelWorksheet.Cells[56, 7].Value = (gralSI + gralNO + gralNA);
                ////Total Expedientes
                //excelWorksheet.Cells[57, 7].Value = listAdquisiciones.Count;

                //SI = 0;
                //NO = 0;
                //NA = 0;
                //count = 0;
                #endregion

                excelPackage.SaveAs(ms);
            }

            ms.Position = 0;

            Spire.Xls.Workbook workbook = new Spire.Xls.Workbook();
            workbook.LoadFromStream(ms);

            var reporte = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(ms.ToArray())
            };
            reporte.Content.Headers.ContentDisposition =
                new ContentDispositionHeaderValue("attachment")
                {
                    FileName = "ReporteAdquisicionesAdjDirecta.xlsx"
                };
            reporte.Content.Headers.ContentType =
                new MediaTypeHeaderValue("application/octet-stream");

            return reporte;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("api/reporte/adquisiciones/sumatoria/33/{municipio}/{ejercicio}/{tipoadjudicacion}")]
        public async System.Threading.Tasks.Task<HttpResponseMessage> DescargaReporteSumario33(string municipio, string ejercicio, string tipoadjudicacion)
        {
            MemoryStream memoryStream = new MemoryStream();

            WebClient client = new WebClient();
            try
            {
                //memoryStream = new MemoryStream(client.DownloadData("C:/Simopa/Templete/TemplateSumariaAdquisiciones33.xlsx"));
                
                if (municipio.Equals("TEPEACA"))
                {
                    memoryStream = new MemoryStream(client.DownloadData("https://apipbrdevelolop.azurewebsites.net/templates/reportes/TemplateSumariaAdquisiciones33.xlsx"));
                }
                else if (municipio.Equals("SAN ANDRES CHOLULA"))
                {
                    memoryStream = new MemoryStream(client.DownloadData("https://apipbrdevelolop.azurewebsites.net/templates/reportes/TemplateSumariaAdquisiciones33Sach.xlsx"));
                }

            }
            finally
            {
                client.Dispose();
            }

            string id = string.Empty;
            string constr = ConfigurationManager.AppSettings["connectionString"];
            var Client = new MongoClient(constr);
            var DB = Client.GetDatabase("PRB");

            List<AdquisicionesV1> listAdquisiciones = new List<AdquisicionesV1>();
            List<ObraPublicaV1> listObras = new List<ObraPublicaV1>();

            try
            {
                var collAdqui = DB.GetCollection<AdquisicionesV1>("Adquisiciones");
                var filterAdqui = Builders<AdquisicionesV1>.Filter.Eq(x => x.municipio, municipio)
                    & Builders<AdquisicionesV1>.Filter.Eq(x => x.ejercicio, ejercicio)
                    & Builders<AdquisicionesV1>.Filter.Eq(x => x.tipoAdjudicacion, tipoadjudicacion);
                var adquisiciones = await collAdqui.Find(filterAdqui).Sort("{ejercicio:-1}").ToListAsync();

                if (adquisiciones.Count > 0)
                {
                    listAdquisiciones = adquisiciones;
                }
                else
                {
                    listAdquisiciones = new List<AdquisicionesV1>();
                }
            }
            catch (Exception ex)
            {
                //response.success = false;
                //response.messages.Add(ex.ToString());
                //return Ok(ex.ToString());
            }

            //Imagen           
            int Height = 0;
            int Width = 0;
            int rowIndex = 0;
            int colIndex = 0;
            string nameImage = string.Empty;
            WebClient clientImage = new WebClient();
            MemoryStream memoryImage = new MemoryStream();
            try
            {
                if (municipio.Equals("TEPEACA"))
                {
                    //memoryImage = new MemoryStream(clientImage.DownloadData("C:/Simopa/Templete/Logo/LogoTepeaca.png"));
                    memoryImage = new MemoryStream(client.DownloadData("https://apipbrdevelolop.azurewebsites.net/templates/logo/LogoTepeaca.png"));
                    nameImage = "Tepeaca";
                    Height = 160;
                    Width = 150;
                    rowIndex = 1;
                    colIndex = 1;
                }
                else if (municipio.Equals("SAN ANDRES CHOLULA"))
                {
                    //memoryImage = new MemoryStream(clientImage.DownloadData("C:/Simopa/Templete/Logo/LogoDespacho.png"));
                    memoryImage = new MemoryStream(client.DownloadData("https://apipbrdevelolop.azurewebsites.net/templates/logo/LogoDespacho.png"));
                    nameImage = "ESACH";
                    Height = 120;
                    Width = 170;
                    rowIndex = 4;
                    colIndex = 1;
                }
                else
                {
                    //memoryImage = new MemoryStream(clientImage.DownloadData("C:/Simopa/Templete/Logo/LogoDespacho.png"));
                    memoryImage = new MemoryStream(client.DownloadData("https://apipbrdevelolop.azurewebsites.net/templates/logo/LogoDespacho.png"));
                }
            }
            catch (Exception ex)
            {
                clientImage.Dispose();
            }

            DocumentosAdquisicionesSanAndresCholula documentos = new DocumentosAdquisicionesSanAndresCholula();
            List<DocumentoAdquisicionesV1> listDoc = new List<DocumentoAdquisicionesV1>();

            MemoryStream ms = new MemoryStream();
            //Envia datos a Excel
            using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {

                ExcelWorkbook excelWorkBook = excelPackage.Workbook;
                ExcelWorksheet excelWorksheet = excelWorkBook.Worksheets[1];
                excelWorksheet.Workbook.CalcMode = ExcelCalcMode.Manual;

                //Imagen
                Image img = Image.FromStream(memoryImage);
                ExcelPicture pic = excelWorksheet.Drawings.AddPicture("imageVista-" + nameImage, img);
                pic.SetPosition(rowIndex, 0, colIndex + ((1 - 1) * 4), 0);
                pic.SetSize(Width, Height);

                int rows = 9;
                int columns = 7;
                //int count = 1;
                DateTime fecha = DateTime.Now;

                
                if (municipio.Equals("TEPEACA"))
                {
                    excelWorksheet.Cells[5, 4].Value = "CÉDULA ANALITICA DE REVISIÓN DE LAS COMPRAS URGENTES Y ESPECIALES -33 MIL DEL EJERCICIO DE " + ejercicio + ".";
                    excelWorksheet.Cells[7, 4].Value = "LIMT - LEY DE INGRESOS DEL MUNICIPIO DE TEPEACA PARA EL " + ejercicio;
                }
                else if (municipio.Equals("SAN ANDRES CHOLULA"))
                {
                    excelWorksheet.Cells[5, 4].Value = "CÉDULA ANALITICA DE REVISIÓN DE LAS COMPRAS URGENTES Y ESPECIALES -33 MIL DEL EJERCICIO DE " + ejercicio + ".";
                    excelWorksheet.Cells[7, 4].Value = "LIMSACH- LEY DE INGRESOS DEL MUNICIPIO DE SAN ANDRES CHOLULA PARA EL " + ejercicio;
                }


                int SI = 0;
                int NO = 0;
                int NA = 0;
                int count = 0;

                int gralSI = 0;
                int gralNO = 0;
                int gralNA = 0;

                #region TEPEACA
                if (municipio.Equals("TEPEACA"))
                {
                    foreach (AdquisicionesV1 data in listAdquisiciones)
                    {
                        excelWorksheet.Cells[rows, columns].Value = data.numeroAdjudicacion;

                        rows++;
                        rows++;
                        rows++;

                        listDoc = data.documentos;
                        Sort(ref listDoc, "clave", "ASC");

                        //8 primera version

                        foreach (var documento in listDoc)
                        {
                            string val = documento.estatus;

                            if (val == null)
                            {
                                NO++;
                            }
                            else if (val.Equals("NO"))
                            {
                                NO++;
                            }
                            else if (val.Equals("SI"))
                            {
                                SI++;
                            }
                            else if (val.Equals("N/A"))
                            {
                                NA++;
                            }

                            if (documento.grupo.Equals("DOCUMENTACIÓN ADMINISTRATIVA"))
                            {
                                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.estatus) ? "NO" : documento.estatus;
                                rows++;
                                count++;
                                if (count == 3) { rows++; count = 0; }
                            }
                            else if (documento.grupo.Equals("ENTREGABLES"))
                            {
                                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.estatus) ? "NO" : documento.estatus;
                                rows++;
                                count++;
                                if (count == 5) { rows++; count = 0; }
                            }
                        }

                        gralSI = gralSI + SI;
                        gralNO = gralNO + NO;
                        gralNA = gralNA + NA;

                        //SI
                        excelWorksheet.Cells[21, columns].Value = SI; rows++;
                        //NO
                        excelWorksheet.Cells[22, columns].Value = NO; rows++;
                        //NA
                        excelWorksheet.Cells[23, columns].Value = NA; rows++;
                        //Sumatoria
                        excelWorksheet.Cells[24, columns].Value = (SI + NO + NA);

                        rows = 9;
                        columns++;
                        SI = 0;
                        NO = 0;
                        NA = 0;
                        count = 0;
                    }

                    columns++;
                    //excelWorksheet.Cells[8, columns].Value = "SI"; columns++;
                    //excelWorksheet.Cells[8, columns].Value = "NO"; columns++;
                    //excelWorksheet.Cells[8, columns].Value = "N/A"; columns++;
                    rows++;
                    rows++;
                    rows++;

                    columns = 56;

                    #region SI, NO, N/A
                    /*
                    foreach (AdquisicionesV1 data in listAdquisiciones)
                    {
                        foreach (var documents in data.documentos)
                        {
                            if (documents.grupo.Equals("DOCUMENTACIÓN ADMINISTRATIVA"))
                            {
                                columns = 56;
                                columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BA" + rows + ",\"SI\")"; columns++;
                                //excelWorksheet.Cells[rows, columns].Calculate();  
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BA" + rows + ",\"NO\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BA" + rows + ",\"N/A\")"; columns++;

                                rows++;
                                count++;
                                if (count == 5) { rows++; count = 0; }
                            }
                            else if (documents.grupo.Equals("PROCEDIMIENTO DE ADJUDICACIÓN"))
                            {
                                columns = 56;
                                columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                                rows++;
                                count++;
                                if (count == 13) { rows++; count = 0; }
                            }
                            else if (documents.grupo.Equals("REQUISITOS DEL CONTRATO"))
                            {
                                columns = 56;
                                columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                                rows++;
                                count++;
                                if (count == 16) { rows++; count = 0; }
                            }
                            else if (documents.grupo.Equals("GARANTÍAS"))
                            {
                                columns = 56;
                                columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                                rows++;
                                count++;
                                if (count == 3) { rows++; count = 0; }
                            }
                            else if (documents.grupo.Equals("ENTREGABLES"))
                            {
                                columns = 56;
                                columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                                rows++;
                                count++;
                                if (count == 8) { rows++; count = 0; }
                            }
                        }
                        break;
                    }
                    */
                    #endregion

                    //SI
                    excelWorksheet.Cells[21, 6].Value = gralSI;
                    //NO
                    excelWorksheet.Cells[22, 6].Value = gralNO;
                    //NA
                    excelWorksheet.Cells[23, 6].Value = gralNA;
                    //Suma Gral
                    excelWorksheet.Cells[24, 6].Value = (gralSI + gralNO + gralNA);
                    //Total Expedientes
                    excelWorksheet.Cells[25, 6].Value = listAdquisiciones.Count;

                    SI = 0;
                    NO = 0;
                    NA = 0;
                    count = 0;
                }
                #endregion
                #region SAN ANDRES CHOLULA
                else if (municipio.Equals("SAN ANDRES CHOLULA"))
                {
                    foreach (AdquisicionesV1 data in listAdquisiciones)
                    {
                        excelWorksheet.Cells[rows, columns].Value = data.numeroAdjudicacion;

                        rows++;
                        rows++;
                        rows++;

                        listDoc = data.documentos;
                        Sort(ref listDoc, "clave", "ASC");

                        //8 primera version

                        foreach (var documento in listDoc)
                        {
                            string val = documento.estatus;

                            if (val == null)
                            {
                                NO++;
                            }
                            else if (val.Equals("NO"))
                            {
                                NO++;
                            }
                            else if (val.Equals("SI"))
                            {
                                SI++;
                            }
                            else if (val.Equals("N/A"))
                            {
                                NA++;
                            }

                            if (documento.grupo.Equals("DOCUMENTACIÓN ADMINISTRATIVA"))
                            {
                                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.estatus) ? "NO" : documento.estatus;
                                rows++;
                                count++;
                                if (count == 3) { rows++; count = 0; }
                            }
                            else if (documento.grupo.Equals("ENTREGABLES"))
                            {
                                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.estatus) ? "NO" : documento.estatus;
                                rows++;
                                count++;
                                if (count == 5) { rows++; count = 0; }
                            }
                        }

                        gralSI = gralSI + SI;
                        gralNO = gralNO + NO;
                        gralNA = gralNA + NA;

                        //SI
                        excelWorksheet.Cells[21, columns].Value = SI; rows++;
                        //NO
                        excelWorksheet.Cells[22, columns].Value = NO; rows++;
                        //NA
                        excelWorksheet.Cells[23, columns].Value = NA; rows++;
                        //Sumatoria
                        excelWorksheet.Cells[24, columns].Value = (SI + NO + NA);

                        rows = 9;
                        columns++;
                        SI = 0;
                        NO = 0;
                        NA = 0;
                        count = 0;
                    }

                    columns++;
                    //excelWorksheet.Cells[8, columns].Value = "SI"; columns++;
                    //excelWorksheet.Cells[8, columns].Value = "NO"; columns++;
                    //excelWorksheet.Cells[8, columns].Value = "N/A"; columns++;
                    rows++;
                    rows++;
                    rows++;

                    columns = 56;

                    #region SI, NO, N/A
                    /*
                    foreach (AdquisicionesV1 data in listAdquisiciones)
                    {
                        foreach (var documents in data.documentos)
                        {
                            if (documents.grupo.Equals("DOCUMENTACIÓN ADMINISTRATIVA"))
                            {
                                columns = 56;
                                columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BA" + rows + ",\"SI\")"; columns++;
                                //excelWorksheet.Cells[rows, columns].Calculate();  
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BA" + rows + ",\"NO\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BA" + rows + ",\"N/A\")"; columns++;

                                rows++;
                                count++;
                                if (count == 5) { rows++; count = 0; }
                            }
                            else if (documents.grupo.Equals("PROCEDIMIENTO DE ADJUDICACIÓN"))
                            {
                                columns = 56;
                                columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                                rows++;
                                count++;
                                if (count == 13) { rows++; count = 0; }
                            }
                            else if (documents.grupo.Equals("REQUISITOS DEL CONTRATO"))
                            {
                                columns = 56;
                                columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                                rows++;
                                count++;
                                if (count == 16) { rows++; count = 0; }
                            }
                            else if (documents.grupo.Equals("GARANTÍAS"))
                            {
                                columns = 56;
                                columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                                rows++;
                                count++;
                                if (count == 3) { rows++; count = 0; }
                            }
                            else if (documents.grupo.Equals("ENTREGABLES"))
                            {
                                columns = 56;
                                columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                                rows++;
                                count++;
                                if (count == 8) { rows++; count = 0; }
                            }
                        }
                        break;
                    }
                    */
                    #endregion

                    //SI
                    excelWorksheet.Cells[21, 6].Value = gralSI;
                    //NO
                    excelWorksheet.Cells[22, 6].Value = gralNO;
                    //NA
                    excelWorksheet.Cells[23, 6].Value = gralNA;
                    //Suma Gral
                    excelWorksheet.Cells[24, 6].Value = (gralSI + gralNO + gralNA);
                    //Total Expedientes
                    excelWorksheet.Cells[25, 6].Value = listAdquisiciones.Count;

                    SI = 0;
                    NO = 0;
                    NA = 0;
                    count = 0;
                }
                #endregion

                #region Anterior

                //foreach (AdquisicionesV1 data in listAdquisiciones)
                //{
                //    excelWorksheet.Cells[rows, columns].Value = data.numeroAdjudicacion;

                //    rows++;
                //    rows++;
                //    rows++;

                //    listDoc = data.documentos;
                //    Sort(ref listDoc, "clave", "ASC");

                //    //8 primera version

                //    foreach(var documento in listDoc)
                //    {
                //        string val = documento.estatus;

                //        if (val == null)
                //        {
                //            NO++;
                //        }
                //        else if (val.Equals("NO"))
                //        {
                //            NO++;
                //        }
                //        else if (val.Equals("SI"))
                //        {
                //            SI++;
                //        }
                //        else if (val.Equals("N/A"))
                //        {
                //            NA++;
                //        }

                //        if (documento.grupo.Equals("DOCUMENTACIÓN ADMINISTRATIVA"))
                //        {
                //            excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.estatus) ? "NO" : documento.estatus;
                //            rows++;
                //            count++;
                //            if (count == 3) { rows++; count = 0; }
                //        }
                //        else if (documento.grupo.Equals("ENTREGABLES"))
                //        {
                //            excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.estatus) ? "NO" : documento.estatus;
                //            rows++;
                //            count++;
                //            if (count == 5) { rows++; count = 0; }
                //        }
                //    }

                //    gralSI = gralSI + SI;
                //    gralNO = gralNO + NO;
                //    gralNA = gralNA + NA;

                //    //SI
                //    excelWorksheet.Cells[21, columns].Value = SI; rows++;
                //    //NO
                //    excelWorksheet.Cells[22, columns].Value = NO; rows++;
                //    //NA
                //    excelWorksheet.Cells[23, columns].Value = NA; rows++;
                //    //Sumatoria
                //    excelWorksheet.Cells[24, columns].Value = (SI + NO + NA);

                //    rows = 9;
                //    columns++;
                //    SI = 0;
                //    NO = 0;
                //    NA = 0;
                //    count = 0;
                //}

                //columns++;
                ////excelWorksheet.Cells[8, columns].Value = "SI"; columns++;
                ////excelWorksheet.Cells[8, columns].Value = "NO"; columns++;
                ////excelWorksheet.Cells[8, columns].Value = "N/A"; columns++;
                //rows++;
                //rows++;
                //rows++;

                //columns = 56;

                //#region SI, NO, N/A
                ///*
                //foreach (AdquisicionesV1 data in listAdquisiciones)
                //{
                //    foreach (var documents in data.documentos)
                //    {
                //        if (documents.grupo.Equals("DOCUMENTACIÓN ADMINISTRATIVA"))
                //        {
                //            columns = 56;
                //            columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BA" + rows + ",\"SI\")"; columns++;
                //            //excelWorksheet.Cells[rows, columns].Calculate();  
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BA" + rows + ",\"NO\")"; columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BA" + rows + ",\"N/A\")"; columns++;

                //            rows++;
                //            count++;
                //            if (count == 5) { rows++; count = 0; }
                //        }
                //        else if (documents.grupo.Equals("PROCEDIMIENTO DE ADJUDICACIÓN"))
                //        {
                //            columns = 56;
                //            columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                //            rows++;
                //            count++;
                //            if (count == 13) { rows++; count = 0; }
                //        }
                //        else if (documents.grupo.Equals("REQUISITOS DEL CONTRATO"))
                //        {
                //            columns = 56;
                //            columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                //            rows++;
                //            count++;
                //            if (count == 16) { rows++; count = 0; }
                //        }
                //        else if (documents.grupo.Equals("GARANTÍAS"))
                //        {
                //            columns = 56;
                //            columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                //            rows++;
                //            count++;
                //            if (count == 3) { rows++; count = 0; }
                //        }
                //        else if (documents.grupo.Equals("ENTREGABLES"))
                //        {
                //            columns = 56;
                //            columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                //            rows++;
                //            count++;
                //            if (count == 8) { rows++; count = 0; }
                //        }
                //    }
                //    break;
                //}
                //*/
                //#endregion

                ////SI
                //excelWorksheet.Cells[21, 6].Value = gralSI;
                ////NO
                //excelWorksheet.Cells[22, 6].Value = gralNO;
                ////NA
                //excelWorksheet.Cells[23, 6].Value = gralNA;
                ////Suma Gral
                //excelWorksheet.Cells[24, 6].Value = (gralSI + gralNO + gralNA);
                ////Total Expedientes
                //excelWorksheet.Cells[25, 6].Value = listAdquisiciones.Count;

                //SI = 0;
                //NO = 0;
                //NA = 0;
                //count = 0;
                #endregion

                excelPackage.SaveAs(ms);
            }

            ms.Position = 0;

            Spire.Xls.Workbook workbook = new Spire.Xls.Workbook();
            workbook.LoadFromStream(ms);

            var reporte = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(ms.ToArray())
            };
            reporte.Content.Headers.ContentDisposition =
                new ContentDispositionHeaderValue("attachment")
                {
                    FileName = "ReporteAdquisicionesComprasUrgEsp33.xlsx"
                };
            reporte.Content.Headers.ContentType =
                new MediaTypeHeaderValue("application/octet-stream");

            return reporte;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("api/reporte/adquisiciones/sumatoria/licitacion/publica/{municipio}/{ejercicio}/{tipoadjudicacion}")]
        public async System.Threading.Tasks.Task<HttpResponseMessage> DescargaReporteSumarioLicitacionPublica(string municipio, string ejercicio, string tipoadjudicacion)
        {
            //5e9f5aa3d066036b046ac966 adjquisiciones
            //5e6b246c29e31517f4d48397 obra
            MemoryStream memoryStream = new MemoryStream();

            WebClient client = new WebClient();
            try
            {
                //memoryStream = new MemoryStream(client.DownloadData("C:/Simopa/Templete/TemplateSumariaAdquisicionesLicitacionPub.xlsx"));
                
                if (municipio.Equals("TEPEACA"))
                {
                    memoryStream = new MemoryStream(client.DownloadData("https://apipbrdevelolop.azurewebsites.net/templates/reportes/TemplateSumariaAdquisicionesLicitacionPub.xlsx"));
                }
                else if (municipio.Equals("SAN ANDRES CHOLULA"))
                {
                    memoryStream = new MemoryStream(client.DownloadData("https://apipbrdevelolop.azurewebsites.net/templates/reportes/TemplateSumariaAdquisicionesLicPublicaSach.xlsx"));
                }

            }
            finally
            {
                client.Dispose();
            }

            string id = string.Empty;
            string constr = ConfigurationManager.AppSettings["connectionString"];
            var Client = new MongoClient(constr);
            var DB = Client.GetDatabase("PRB");

            List<AdquisicionesV1> listAdquisiciones = new List<AdquisicionesV1>();
            List<ObraPublicaV1> listObras = new List<ObraPublicaV1>();

            try
            {
                var collAdqui = DB.GetCollection<AdquisicionesV1>("Adquisiciones");
                var filterAdqui = Builders<AdquisicionesV1>.Filter.Eq(x => x.municipio, municipio)
                    & Builders<AdquisicionesV1>.Filter.Eq(x => x.ejercicio, ejercicio)
                    & Builders<AdquisicionesV1>.Filter.Eq(x => x.tipoAdjudicacion, tipoadjudicacion);
                var adquisiciones = await collAdqui.Find(filterAdqui).Sort("{ejercicio:-1}").ToListAsync();

                if (adquisiciones.Count > 0)
                {
                    listAdquisiciones = adquisiciones;
                }
                else
                {
                    listAdquisiciones = new List<AdquisicionesV1>();
                }
            }
            catch (Exception ex)
            {
                //response.success = false;
                //response.messages.Add(ex.ToString());
                //return Ok(ex.ToString());
            }

            //Imagen           
            int Height = 0;
            int Width = 0;
            int rowIndex = 0;
            int colIndex = 0;
            string nameImage = string.Empty;
            WebClient clientImage = new WebClient();
            MemoryStream memoryImage = new MemoryStream();
            try
            {
                if (municipio.Equals("TEPEACA"))
                {
                    //memoryImage = new MemoryStream(clientImage.DownloadData("C:/Simopa/Templete/Logo/LogoTepeaca.png"));
                    memoryImage = new MemoryStream(client.DownloadData("https://apipbrdevelolop.azurewebsites.net/templates/logo/LogoTepeaca.png"));
                    nameImage = "Tepeaca";
                    Height = 160;
                    Width = 150;
                    rowIndex = 1;
                    colIndex = 1;
                }
                else if (municipio.Equals("SAN ANDRES CHOLULA"))
                {
                    //memoryImage = new MemoryStream(clientImage.DownloadData("C:/Simopa/Templete/Logo/LogoDespacho.png"));
                    memoryImage = new MemoryStream(client.DownloadData("https://apipbrdevelolop.azurewebsites.net/templates/logo/LogoDespacho.png"));
                    nameImage = "ESACH";
                    Height = 120;
                    Width = 150;
                    rowIndex = 4;
                    colIndex = 1;
                }
                else
                {
                    //memoryImage = new MemoryStream(clientImage.DownloadData("C:/Simopa/Templete/Logo/LogoDespacho.png"));
                    memoryImage = new MemoryStream(client.DownloadData("https://apipbrdevelolop.azurewebsites.net/templates/logo/LogoDespacho.png"));
                }
            }
            catch (Exception ex)
            {
                clientImage.Dispose();
            }

            DocumentosAdquisicionesSanAndresCholula documentos = new DocumentosAdquisicionesSanAndresCholula();
            List<DocumentoAdquisicionesV1> listDoc = new List<DocumentoAdquisicionesV1>();

            MemoryStream ms = new MemoryStream();
            //Envia datos a Excel
            using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {
                //ExcelWorkbook workBook = excelPackage.Workbook;
                //var excelWorksheet = workBook.Worksheets.First();
                //excelWorksheet.Workbook.CalcMode = ExcelCalcMode.Automatic;

                ExcelWorkbook excelWorkBook = excelPackage.Workbook;
                ExcelWorksheet excelWorksheet = excelWorkBook.Worksheets[1];
                excelWorksheet.Workbook.CalcMode = ExcelCalcMode.Manual;

                //Imagen
                Image img = Image.FromStream(memoryImage);
                ExcelPicture pic = excelWorksheet.Drawings.AddPicture("imageVista-" + nameImage, img);
                pic.SetPosition(rowIndex, 0, colIndex + ((1 - 1) * 4), 0);
                pic.SetSize(Width, Height);

                int rows = 0;
                int columns = 0;
                //int count = 1;
                DateTime fecha = DateTime.Now;

                if (municipio.Equals("TEPEACA"))
                {
                    excelWorksheet.Cells[5, 4].Value = "CÉDULA ANALITICA DE REVISIÓN DE PROCEDIMIENTO DE LICITACIÓN PÚBLICA " + ejercicio;
                    excelWorksheet.Cells[7, 4].Value = "LIMT- LEY DE INGRESOS DEL MUNICIPIO DE TEPEACA PARA EL " + ejercicio;
                    rows = 9;
                    columns = 7;
                }
                else if (municipio.Equals("SAN ANDRES CHOLULA"))
                {
                    excelWorksheet.Cells[5, 4].Value = "CÉDULA ANALITICA DE REVISIÓN DE PROCEDIMIENTO DE LICITACIÓN PÚBLICA " + ejercicio;
                    excelWorksheet.Cells[7, 4].Value = "LIMSACH- LEY DE INGRESOS DEL MUNICIPIO DE SAN ANDRES CHOLULA PARA EL " + ejercicio;
                    rows = 9;
                    columns = 7;
                }

                int SI = 0;
                int NO = 0;
                int NA = 0;
                int count = 0;

                int gralSI = 0;
                int gralNO = 0;
                int gralNA = 0;

                #region TEPEACA
                if (municipio.Equals("TEPEACA"))
                {
                    foreach (AdquisicionesV1 data in listAdquisiciones)
                    {
                        excelWorksheet.Cells[rows, columns].Value = data.numeroAdjudicacion;

                        rows++;
                        rows++;
                        rows++;

                        listDoc = data.documentos;
                        Sort(ref listDoc, "clave", "ASC");

                        //36 documentos en Excel
                        //31 documentos antigua version

                        foreach (var documento in listDoc)
                        {
                            string val = documento.estatus;

                            if (val == null)
                            {
                                NO++;
                            }
                            else if (val.Equals("NO"))
                            {
                                NO++;
                            }
                            else if (val.Equals("SI"))
                            {
                                SI++;
                            }
                            else if (val.Equals("N/A"))
                            {
                                NA++;
                            }

                            if (documento.grupo.Equals("DOCUMENTACIÓN ADMINISTRATIVA"))
                            {
                                if (listDoc.Count == 31)
                                {
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.estatus) ? "NO" : documento.estatus;
                                    rows++;
                                    count++;
                                    if (count == 5) { rows++; rows++; count = 0; }//6
                                }
                                else //caso 36 documentos
                                {
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.estatus) ? "NO" : documento.estatus;
                                    rows++;
                                    count++;
                                    if (count == 6) { rows++; count = 0; }
                                }

                            }
                            else if (documento.grupo.Equals("PROCEDIMIENTO DE ADJUDICACIÓN"))
                            {
                                if (listDoc.Count == 31)
                                {
                                    if (documento.clave == 10)
                                    {
                                        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.estatus) ? "NO" : documento.estatus;
                                        rows++; rows++; count++;
                                    }
                                    else
                                    {
                                        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.estatus) ? "NO" : documento.estatus;
                                        rows++;
                                        count++;
                                    }

                                    if (count == 18) { rows++; count = 0; }//11
                                }
                                else //caso 36 documentos
                                {
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.estatus) ? "NO" : documento.estatus;
                                    rows++;
                                    count++;

                                    if (count == 19) { rows++; count = 0; }
                                }
                            }
                            else if (documento.grupo.Equals("REQUISITOS DEL CONTRATO"))
                            {
                                if (listDoc.Count == 31)
                                {
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.estatus) ? "NO" : documento.estatus;
                                    rows++;
                                    count++;
                                    if (count == 3) { rows++; rows++; count = 0; } //29
                                }
                                else //caso 36 documentos
                                {
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.estatus) ? "NO" : documento.estatus;
                                    rows++;
                                    count++;
                                    if (count == 4) { rows++; count = 0; }
                                }
                            }
                            else if (documento.grupo.Equals("GARANTÍAS"))
                            {
                                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.estatus) ? "NO" : documento.estatus;
                                rows++;
                                count++;
                                if (count == 2) { rows++; count = 0; } //2
                            }
                            else if (documento.grupo.Equals("ENTREGABLES"))
                            {
                                if (listDoc.Count == 31)
                                {
                                    if (documento.clave == 33)
                                    {
                                        rows++;
                                        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.estatus) ? "NO" : documento.estatus;
                                        rows++; rows++; count++;
                                    }
                                    else
                                    {
                                        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.estatus) ? "NO" : documento.estatus;
                                        rows++;
                                        count++;
                                    }
                                    if (count == 3) { rows++; count = 0; }//32,34
                                }
                                else  //caso 36 documentos
                                {
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.estatus) ? "NO" : documento.estatus;
                                    rows++;
                                    count++;
                                    if (count == 5) { rows++; count = 0; }
                                }
                            }
                        }

                        gralSI = gralSI + SI;
                        gralNO = gralNO + NO;
                        gralNA = gralNA + NA;

                        //SI
                        excelWorksheet.Cells[52, columns].Value = SI; rows++;
                        //NO
                        excelWorksheet.Cells[53, columns].Value = NO; rows++;
                        //NA
                        excelWorksheet.Cells[54, columns].Value = NA; rows++;
                        //Sumatoria
                        excelWorksheet.Cells[55, columns].Value = (SI + NO + NA);

                        rows = 9;
                        columns++;
                        SI = 0;
                        NO = 0;
                        NA = 0;
                        count = 0;
                    }

                    columns++;
                    int col = columns;

                    rows++;
                    rows++;
                    rows++;

                    columns = 56;

                    #region SI, NO, N/A
                    /*
                    foreach (AdquisicionesV1 data in listAdquisiciones)
                    {
                        foreach (var documents in data.documentos)
                        {
                            if (documents.grupo.Equals("DOCUMENTACIÓN ADMINISTRATIVA"))
                            {
                                columns = 56;
                                columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BA" + rows + ",\"SI\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BA" + rows + ",\"NO\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BA" + rows + ",\"N/A\")"; columns++;

                                rows++;
                                count++;
                                if (count == 5) { rows++; count = 0; }
                            }
                            else if (documents.grupo.Equals("PROCEDIMIENTO DE ADJUDICACIÓN"))
                            {
                                columns = 56;
                                columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                                rows++;
                                count++;
                                if (count == 13) { rows++; count = 0; }
                            }
                            else if (documents.grupo.Equals("REQUISITOS DEL CONTRATO"))
                            {
                                columns = 56;
                                columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                                rows++;
                                count++;
                                if (count == 16) { rows++; count = 0; }
                            }
                            else if (documents.grupo.Equals("GARANTÍAS"))
                            {
                                columns = 56;
                                columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                                rows++;
                                count++;
                                if (count == 3) { rows++; count = 0; }
                            }
                            else if (documents.grupo.Equals("ENTREGABLES"))
                            {
                                columns = 56;
                                columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                                rows++;
                                count++;
                                if (count == 8) { rows++; count = 0; }
                            }
                        }
                        break;
                    }
                    */
                    #endregion

                    //SI
                    excelWorksheet.Cells[52, 6].Value = gralSI;
                    //NO
                    excelWorksheet.Cells[53, 6].Value = gralNO;
                    //NA
                    excelWorksheet.Cells[54, 6].Value = gralNA;
                    //Suma Gral
                    excelWorksheet.Cells[55, 6].Value = (gralSI + gralNO + gralNA);
                    //Total Expedientes
                    excelWorksheet.Cells[56, 6].Value = listAdquisiciones.Count;

                    SI = 0;
                    NO = 0;
                    NA = 0;
                    count = 0;
                }
                #endregion
                #region SAN ANDRES CHOLULA
                else if (municipio.Equals("SAN ANDRES CHOLULA"))
                {
                    foreach (AdquisicionesV1 data in listAdquisiciones)
                    {
                        excelWorksheet.Cells[rows, columns].Value = data.numeroAdjudicacion;

                        rows++;
                        rows++;
                        rows++;

                        listDoc = data.documentos;
                        Sort(ref listDoc, "clave", "ASC");

                        //36 documentos en Excel
                        //31 documentos antigua version

                        foreach (var documento in listDoc)
                        {
                            string val = documento.estatus;

                            if (val == null)
                            {
                                NO++;
                            }
                            else if (val.Equals("NO"))
                            {
                                NO++;
                            }
                            else if (val.Equals("SI"))
                            {
                                SI++;
                            }
                            else if (val.Equals("N/A"))
                            {
                                NA++;
                            }

                            if (documento.grupo.Equals("DOCUMENTACIÓN ADMINISTRATIVA"))
                            {
                                if (listDoc.Count == 31)
                                {
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.estatus) ? "NO" : documento.estatus;
                                    rows++;
                                    count++;
                                    if (count == 5) { rows++; rows++; count = 0; }//6
                                }
                                else //caso 36 documentos
                                {
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.estatus) ? "NO" : documento.estatus;
                                    rows++;
                                    count++;
                                    if (count == 6) { rows++; count = 0; }
                                }

                            }
                            else if (documento.grupo.Equals("PROCEDIMIENTO DE ADJUDICACIÓN"))
                            {
                                if (listDoc.Count == 31)
                                {
                                    if (documento.clave == 10)
                                    {
                                        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.estatus) ? "NO" : documento.estatus;
                                        rows++; rows++; count++;
                                    }
                                    else
                                    {
                                        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.estatus) ? "NO" : documento.estatus;
                                        rows++;
                                        count++;
                                    }

                                    if (count == 18) { rows++; count = 0; }//11
                                }
                                else //caso 36 documentos
                                {
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.estatus) ? "NO" : documento.estatus;
                                    rows++;
                                    count++;

                                    if (count == 19) { rows++; count = 0; }
                                }
                            }
                            else if (documento.grupo.Equals("REQUISITOS DEL CONTRATO"))
                            {
                                if (listDoc.Count == 31)
                                {
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.estatus) ? "NO" : documento.estatus;
                                    rows++;
                                    count++;
                                    if (count == 3) { rows++; rows++; count = 0; } //29
                                }
                                else //caso 36 documentos
                                {
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.estatus) ? "NO" : documento.estatus;
                                    rows++;
                                    count++;
                                    if (count == 4) { rows++; count = 0; }
                                }
                            }
                            else if (documento.grupo.Equals("GARANTÍAS"))
                            {
                                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.estatus) ? "NO" : documento.estatus;
                                rows++;
                                count++;
                                if (count == 2) { rows++; count = 0; } //2
                            }
                            else if (documento.grupo.Equals("ENTREGABLES"))
                            {
                                if (listDoc.Count == 31)
                                {
                                    if (documento.clave == 33)
                                    {
                                        rows++;
                                        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.estatus) ? "NO" : documento.estatus;
                                        rows++; rows++; count++;
                                    }
                                    else
                                    {
                                        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.estatus) ? "NO" : documento.estatus;
                                        rows++;
                                        count++;
                                    }
                                    if (count == 3) { rows++; count = 0; }//32,34
                                }
                                else  //caso 36 documentos
                                {
                                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.estatus) ? "NO" : documento.estatus;
                                    rows++;
                                    count++;
                                    if (count == 5) { rows++; count = 0; }
                                }
                            }
                        }

                        gralSI = gralSI + SI;
                        gralNO = gralNO + NO;
                        gralNA = gralNA + NA;

                        //SI
                        excelWorksheet.Cells[52, columns].Value = SI; rows++;
                        //NO
                        excelWorksheet.Cells[53, columns].Value = NO; rows++;
                        //NA
                        excelWorksheet.Cells[54, columns].Value = NA; rows++;
                        //Sumatoria
                        excelWorksheet.Cells[55, columns].Value = (SI + NO + NA);

                        rows = 9;
                        columns++;
                        SI = 0;
                        NO = 0;
                        NA = 0;
                        count = 0;
                    }

                    columns++;
                    int col = columns;

                    rows++;
                    rows++;
                    rows++;

                    columns = 56;

                    #region SI, NO, N/A
                    /*
                    foreach (AdquisicionesV1 data in listAdquisiciones)
                    {
                        foreach (var documents in data.documentos)
                        {
                            if (documents.grupo.Equals("DOCUMENTACIÓN ADMINISTRATIVA"))
                            {
                                columns = 56;
                                columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BA" + rows + ",\"SI\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BA" + rows + ",\"NO\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BA" + rows + ",\"N/A\")"; columns++;

                                rows++;
                                count++;
                                if (count == 5) { rows++; count = 0; }
                            }
                            else if (documents.grupo.Equals("PROCEDIMIENTO DE ADJUDICACIÓN"))
                            {
                                columns = 56;
                                columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                                rows++;
                                count++;
                                if (count == 13) { rows++; count = 0; }
                            }
                            else if (documents.grupo.Equals("REQUISITOS DEL CONTRATO"))
                            {
                                columns = 56;
                                columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                                rows++;
                                count++;
                                if (count == 16) { rows++; count = 0; }
                            }
                            else if (documents.grupo.Equals("GARANTÍAS"))
                            {
                                columns = 56;
                                columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                                rows++;
                                count++;
                                if (count == 3) { rows++; count = 0; }
                            }
                            else if (documents.grupo.Equals("ENTREGABLES"))
                            {
                                columns = 56;
                                columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                                rows++;
                                count++;
                                if (count == 8) { rows++; count = 0; }
                            }
                        }
                        break;
                    }
                    */
                    #endregion

                    //SI
                    excelWorksheet.Cells[52, 6].Value = gralSI;
                    //NO
                    excelWorksheet.Cells[53, 6].Value = gralNO;
                    //NA
                    excelWorksheet.Cells[54, 6].Value = gralNA;
                    //Suma Gral
                    excelWorksheet.Cells[55, 6].Value = (gralSI + gralNO + gralNA);
                    //Total Expedientes
                    excelWorksheet.Cells[56, 6].Value = listAdquisiciones.Count;

                    SI = 0;
                    NO = 0;
                    NA = 0;
                    count = 0;
                }
                #endregion

                #region Anterior
                //foreach (AdquisicionesV1 data in listAdquisiciones)
                //{
                //    excelWorksheet.Cells[rows, columns].Value = data.numeroAdjudicacion;

                //    rows++;
                //    rows++;
                //    rows++;

                //    listDoc = data.documentos;
                //    Sort(ref listDoc, "clave", "ASC");

                //    //36 documentos en Excel
                //    //31 documentos antigua version

                //    foreach(var documento in listDoc)
                //    {
                //        string val = documento.estatus;

                //        if (val == null)
                //        {
                //            NO++;
                //        }
                //        else if (val.Equals("NO"))
                //        {
                //            NO++;
                //        }
                //        else if (val.Equals("SI"))
                //        {
                //            SI++;
                //        }
                //        else if (val.Equals("N/A"))
                //        {
                //            NA++;
                //        }

                //        if (documento.grupo.Equals("DOCUMENTACIÓN ADMINISTRATIVA"))
                //        {
                //            if (listDoc.Count == 31)
                //            {
                //                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.estatus) ? "NO" : documento.estatus;
                //                rows++;
                //                count++;
                //                if (count == 5) { rows++; rows++; count = 0; }//6
                //            }
                //            else //caso 36 documentos
                //            {
                //                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.estatus) ? "NO" : documento.estatus;
                //                rows++;
                //                count++;
                //                if (count == 6) { rows++; count = 0; }
                //            }

                //        }
                //        else if (documento.grupo.Equals("PROCEDIMIENTO DE ADJUDICACIÓN"))
                //        {
                //            if (listDoc.Count == 31)
                //            {
                //                if (documento.clave == 10)
                //                {
                //                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.estatus) ? "NO" : documento.estatus;
                //                    rows++; rows++; count++;
                //                }
                //                else
                //                {
                //                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.estatus) ? "NO" : documento.estatus;
                //                    rows++;
                //                    count++;
                //                }

                //                if (count == 18) { rows++; count = 0; }//11
                //            }
                //            else //caso 36 documentos
                //            {                               
                //                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.estatus) ? "NO" : documento.estatus;
                //                rows++;
                //                count++;

                //                if (count == 19) { rows++; count = 0; }
                //            }                             
                //        }
                //        else if (documento.grupo.Equals("REQUISITOS DEL CONTRATO"))
                //        {
                //            if (listDoc.Count == 31)
                //            {
                //                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.estatus) ? "NO" : documento.estatus;
                //                rows++;
                //                count++;                                
                //                if (count == 3) { rows++; rows++; count = 0; } //29
                //            }
                //            else //caso 36 documentos
                //            {
                //                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.estatus) ? "NO" : documento.estatus;
                //                rows++;
                //                count++;
                //                if (count == 4) { rows++; count = 0; } 
                //            }
                //        }
                //        else if (documento.grupo.Equals("GARANTÍAS"))
                //        {
                //            excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.estatus) ? "NO" : documento.estatus;
                //            rows++;
                //            count++;
                //            if (count == 2) { rows++; count = 0; } //2
                //        }
                //        else if (documento.grupo.Equals("ENTREGABLES"))
                //        {
                //            if (listDoc.Count == 31)
                //            {
                //                if (documento.clave == 33)
                //                {
                //                    rows++;
                //                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.estatus) ? "NO" : documento.estatus;
                //                    rows++; rows++; count++;
                //                }
                //                else
                //                {
                //                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.estatus) ? "NO" : documento.estatus;
                //                    rows++;
                //                    count++;
                //                }
                //                if (count == 3) { rows++; count = 0; }//32,34
                //            }
                //            else  //caso 36 documentos
                //            {
                //                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.estatus) ? "NO" : documento.estatus;
                //                rows++;
                //                count++;
                //                if (count == 5) { rows++; count = 0; }
                //            }                                
                //        }
                //    }

                //    gralSI = gralSI + SI;
                //    gralNO = gralNO + NO;
                //    gralNA = gralNA + NA;

                //    //SI
                //    excelWorksheet.Cells[52, columns].Value = SI; rows++;
                //    //NO
                //    excelWorksheet.Cells[53, columns].Value = NO; rows++;
                //    //NA
                //    excelWorksheet.Cells[54, columns].Value = NA; rows++;
                //    //Sumatoria
                //    excelWorksheet.Cells[55, columns].Value = (SI + NO + NA);

                //    rows = 9;
                //    columns++;
                //    SI = 0;
                //    NO = 0;
                //    NA = 0;
                //    count = 0;
                //}

                //columns++;
                //int col = columns;

                //rows++;
                //rows++;
                //rows++;

                //columns = 56;

                //#region SI, NO, N/A
                ///*
                //foreach (AdquisicionesV1 data in listAdquisiciones)
                //{
                //    foreach (var documents in data.documentos)
                //    {
                //        if (documents.grupo.Equals("DOCUMENTACIÓN ADMINISTRATIVA"))
                //        {
                //            columns = 56;
                //            columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BA" + rows + ",\"SI\")"; columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BA" + rows + ",\"NO\")"; columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BA" + rows + ",\"N/A\")"; columns++;

                //            rows++;
                //            count++;
                //            if (count == 5) { rows++; count = 0; }
                //        }
                //        else if (documents.grupo.Equals("PROCEDIMIENTO DE ADJUDICACIÓN"))
                //        {
                //            columns = 56;
                //            columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                //            rows++;
                //            count++;
                //            if (count == 13) { rows++; count = 0; }
                //        }
                //        else if (documents.grupo.Equals("REQUISITOS DEL CONTRATO"))
                //        {
                //            columns = 56;
                //            columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                //            rows++;
                //            count++;
                //            if (count == 16) { rows++; count = 0; }
                //        }
                //        else if (documents.grupo.Equals("GARANTÍAS"))
                //        {
                //            columns = 56;
                //            columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                //            rows++;
                //            count++;
                //            if (count == 3) { rows++; count = 0; }
                //        }
                //        else if (documents.grupo.Equals("ENTREGABLES"))
                //        {
                //            columns = 56;
                //            columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                //            rows++;
                //            count++;
                //            if (count == 8) { rows++; count = 0; }
                //        }
                //    }
                //    break;
                //}
                //*/
                //#endregion

                ////SI
                //excelWorksheet.Cells[52, 6].Value = gralSI;
                ////NO
                //excelWorksheet.Cells[53, 6].Value = gralNO;
                ////NA
                //excelWorksheet.Cells[54, 6].Value = gralNA;
                ////Suma Gral
                //excelWorksheet.Cells[55, 6].Value = (gralSI + gralNO + gralNA);
                ////Total Expedientes
                //excelWorksheet.Cells[56, 6].Value = listAdquisiciones.Count;

                //SI = 0;
                //NO = 0;
                //NA = 0;
                //count = 0;
                #endregion

                excelPackage.SaveAs(ms);
            }

            ms.Position = 0;

            Spire.Xls.Workbook workbook = new Spire.Xls.Workbook();
            workbook.LoadFromStream(ms);

            var reporte = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(ms.ToArray())
            };
            reporte.Content.Headers.ContentDisposition =
                new ContentDispositionHeaderValue("attachment")
                {
                    FileName = "ReporteAdquisicionesLicitacionPub.xlsx"
                };
            reporte.Content.Headers.ContentType =
                new MediaTypeHeaderValue("application/octet-stream");

            return reporte;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("api/reporte/adquisiciones/sumatoria/obra/publica/{municipio}/{ejercicio}/{tipoadjudicacion}")]
        public async System.Threading.Tasks.Task<HttpResponseMessage> DescargaReporteObra(string municipio, string ejercicio, string tipoadjudicacion)
        {
            //5e9f5aa3d066036b046ac966 adjquisiciones
            //5e6b246c29e31517f4d48397 obra
            MemoryStream memoryStream = new MemoryStream();

            WebClient client = new WebClient();
            try
            {
                //memoryStream = new MemoryStream(client.DownloadData("C:/Simopa/Templete/TemplateSumariaObraPublicaGral.xlsx"));
                
                if (municipio.Equals("TEPEACA"))
                {
                    memoryStream = new MemoryStream(client.DownloadData("https://apipbrdevelolop.azurewebsites.net/templates/reportes/TemplateSumariaObraPublicaGral.xlsx"));
                }
                else if (municipio.Equals("SAN ANDRES CHOLULA"))
                {
                    memoryStream = new MemoryStream(client.DownloadData("https://apipbrdevelolop.azurewebsites.net/templates/reportes/TemplateSumariaObraPublicaSach.xlsx"));
                }

            }
            finally
            {
                client.Dispose();
            }

            string id = string.Empty;
            string constr = ConfigurationManager.AppSettings["connectionString"];
            var Client = new MongoClient(constr);
            var DB = Client.GetDatabase("PRB");

            List<ObraPublicaV1> listObras = new List<ObraPublicaV1>();

            try
            {
                var collAdqui = DB.GetCollection<ObraPublicaV1>("ObraPublica");
                var filterAdqui = Builders<ObraPublicaV1>.Filter.Eq(x => x.municipio, municipio)
                    & Builders<ObraPublicaV1>.Filter.Eq(x => x.ejercicio, ejercicio);
                    //& Builders<ObraPublicaV1>.Filter.Eq(x => x.tipoAdjudicacion, tipoadjudicacion);
                    //& Builders<ObraPublicaV1>.Filter.Eq(x => x.numeroObra, "2019002");
                var obras = await collAdqui.Find(filterAdqui).Sort("{ejercicio:-1}").ToListAsync();

                if (obras.Count > 0)
                {
                    listObras = obras;
                }
                else
                {
                    listObras = new List<ObraPublicaV1>();
                }
            }
            catch (Exception ex)
            {
                //response.success = false;
                //response.messages.Add(ex.ToString());
                //return Ok(ex.ToString());
            }

            //Imagen           
            int Height = 0;
            int Width = 0;
            int rowIndex = 0;
            int colIndex = 0;
            string nameImage = string.Empty;
            WebClient clientImage = new WebClient();
            MemoryStream memoryImage = new MemoryStream();
            try
            {
                if (municipio.Equals("TEPEACA"))
                {
                    //memoryImage = new MemoryStream(clientImage.DownloadData("C:/Simopa/Templete/Logo/LogoTepeaca.png"));
                    memoryImage = new MemoryStream(client.DownloadData("https://apipbrdevelolop.azurewebsites.net/templates/logo/LogoTepeaca.png"));
                    nameImage = "Tepeaca";
                    Height = 160;
                    Width = 150;
                    rowIndex = 1;
                    colIndex = 1;
                }
                else if (municipio.Equals("SAN ANDRES CHOLULA"))
                {
                    //memoryImage = new MemoryStream(clientImage.DownloadData("C:/Simopa/Templete/Logo/LogoDespacho.png"));
                    memoryImage = new MemoryStream(client.DownloadData("https://apipbrdevelolop.azurewebsites.net/templates/logo/LogoDespacho.png"));
                    nameImage = "ESACH";
                    Height = 130;
                    Width = 180;
                    rowIndex = 4;
                    colIndex = 1;
                }
                else
                {
                    //memoryImage = new MemoryStream(clientImage.DownloadData("C:/Simopa/Templete/Logo/LogoDespacho.png"));
                    memoryImage = new MemoryStream(client.DownloadData("https://apipbrdevelolop.azurewebsites.net/templates/logo/LogoDespacho.png"));
                }
            }
            catch (Exception ex)
            {
                clientImage.Dispose();
            }

            DocumentosAdquisicionesSanAndresCholula documentos = new DocumentosAdquisicionesSanAndresCholula();
            List<DocumentosObrasV1> listDoc = new List<DocumentosObrasV1>();

            MemoryStream ms = new MemoryStream();
            //Envia datos a Excel
            using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {
                //ExcelWorkbook workBook = excelPackage.Workbook;
                //var excelWorksheet = workBook.Worksheets.First();
                //excelWorksheet.Workbook.CalcMode = ExcelCalcMode.Automatic;

                ExcelWorkbook excelWorkBook = excelPackage.Workbook;
                ExcelWorksheet excelWorksheet = excelWorkBook.Worksheets[1];
                excelWorksheet.Workbook.CalcMode = ExcelCalcMode.Manual;

                //Imagen
                Image img = Image.FromStream(memoryImage);
                ExcelPicture pic = excelWorksheet.Drawings.AddPicture("imageVista-" + nameImage, img);
                pic.SetPosition(rowIndex, 0, colIndex + ((1 - 1) * 4), 0);
                pic.SetSize(Width, Height);

                int rows = 0;
                int columns = 0;
                //int count = 1;
                DateTime fecha = DateTime.Now;

                if (municipio.Equals("TEPEACA"))
                {
                    excelWorksheet.Cells[5, 4].Value = "CÉDULA ANALITICA DE REVISIÓN DE OBRAS PÚBLICAS " + ejercicio;
                    excelWorksheet.Cells[2, 12].Value = "TIPO ADJUDICACIÓN " + tipoadjudicacion;
                    rows = 8;
                    columns = 7;
                }
                else if (municipio.Equals("SAN ANDRES CHOLULA"))
                {
                    excelWorksheet.Cells[5, 4].Value = "CÉDULA ANALITICA DE REVISIÓN DE OBRAS PÚBLICAS " + ejercicio;
                    excelWorksheet.Cells[2, 12].Value = "TIPO ADJUDICACIÓN " + tipoadjudicacion;
                    rows = 8;
                    columns = 7;
                }

               

                int SI = 0;
                int NO = 0;
                int NA = 0;
                int count = 0;

                int gralSI = 0;
                int gralNO = 0;
                int gralNA = 0;

                #region TEPEACA
                if (municipio.Equals("TEPEACA"))
                {
                    foreach (ObraPublicaV1 data in listObras)
                    {
                        listDoc = data.documentos;

                        string numero = data.numeroObra;
                        int counts = listDoc.Count;

                        if (listDoc.Count == 98)
                        {
                            excelWorksheet.Cells[rows, columns].Value = data.numeroObra;

                            rows++;
                            rows++;
                            rows++;

                            if (listDoc.Count != 196)
                            {
                                Sort(ref listDoc, "clave", "ASC");
                            }

                            // documentos en Excel
                            //96 documentos antigua version
                            //98 documentos antigua version

                            foreach (var documento in listDoc)
                            {
                                string val = documento.integracion;

                                if (val == null)
                                {
                                    NO++;
                                }
                                else if (val.Equals("NO"))
                                {
                                    NO++;
                                }
                                else if (val.Equals("SI"))
                                {
                                    SI++;
                                }
                                else if (val.Equals("N/A"))
                                {
                                    NA++;
                                }

                                //if (listDoc.Count == 95)
                                //{
                                //    if (documento.clave >= 1 & documento.clave <= 10)
                                //    {
                                //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                                //        rows++;
                                //        count++;
                                //        if (count == 10) { rows++; count = 0; }
                                //    }
                                //    else if (documento.clave >= 11 & documento.clave <= 19)
                                //    {
                                //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                                //        rows++;
                                //        count++;
                                //        if (count == 9) { rows++; count = 0; }
                                //    }
                                //    else if (documento.clave >= 20 & documento.clave <= 25)
                                //    {
                                //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                                //        rows++;
                                //        count++;
                                //        if (count == 6) { rows++; rows++; count = 0; }
                                //    }
                                //    else if (documento.clave >= 27 & documento.clave <= 32)
                                //    {
                                //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                                //        rows++;
                                //        count++;
                                //        if (count == 6) { rows++; rows++; count = 0; }
                                //    }
                                //    else if (documento.clave >= 33 & documento.clave <= 41)
                                //    {
                                //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                                //        rows++;
                                //        count++;
                                //        if (count == 9) { rows++; count = 0; }
                                //    }
                                //    else if (documento.clave >= 42 & documento.clave <= 43)
                                //    {
                                //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                                //        rows++;
                                //        count++;
                                //        if (count == 2) { rows++; count = 0; }
                                //    }
                                //    else if (documento.clave == 44)
                                //    {
                                //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                                //        rows++;
                                //        count++;
                                //        if (count == 1) { rows++; rows++; count = 0; }
                                //    }
                                //    else if (documento.clave >= 45 & documento.clave <= 57)
                                //    {
                                //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                                //        rows++;
                                //        count++;
                                //        if (count == 13) { rows++; count = 0; }
                                //    }
                                //    else if (documento.clave >= 58 & documento.clave <= 62)
                                //    {
                                //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                                //        rows++;
                                //        count++;
                                //        if (count == 5) { rows++; count = 0; }
                                //    }
                                //    else if (documento.clave >= 63 & documento.clave <= 72)
                                //    {
                                //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                                //        rows++;
                                //        count++;
                                //        if (count == 10) { rows++; count = 0; }
                                //    }
                                //    else if (documento.clave >= 73 & documento.clave <= 75)
                                //    {
                                //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                                //        rows++;
                                //        count++;
                                //        if (count == 3) { rows++; count = 0; }
                                //    }
                                //    else if (documento.clave >= 76 & documento.clave <= 77)
                                //    {
                                //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                                //        rows++;
                                //        count++;
                                //        if (count == 2) { rows++; rows++; count = 0; }
                                //    }
                                //    else if (documento.clave >= 80 & documento.clave <= 84)
                                //    {
                                //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                                //        rows++;
                                //        count++;
                                //        if (count == 5) { rows++; count = 0; }
                                //    }
                                //    else if (documento.clave >= 85 & documento.clave <= 90)
                                //    {
                                //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                                //        rows++;
                                //        count++;
                                //        if (count == 6) { rows++; count = 0; }
                                //    }
                                //    else if (documento.clave >= 91 & documento.clave <= 96)
                                //    {
                                //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                                //        rows++;
                                //        count++;
                                //        if (count == 6) { rows++; count = 0; }
                                //    }
                                //    else if (documento.clave >= 97 & documento.clave <= 98)
                                //    {
                                //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                                //        rows++;
                                //        count++;
                                //        if (count == 2) { rows++; count = 0; }
                                //    }
                                //}
                                //else if (listDoc.Count == 96 | listDoc.Count == 97)
                                //{
                                //    if (documento.clave >= 1 & documento.clave <= 10)
                                //    {
                                //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                                //        rows++;
                                //        count++;
                                //        if (count == 10) { rows++; count = 0; }
                                //    }
                                //    else if (documento.clave >= 11 & documento.clave <= 19)
                                //    {
                                //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                                //        rows++;
                                //        count++;
                                //        if (count == 9) { rows++; count = 0; }
                                //    }
                                //    else if (documento.clave >= 20 & documento.clave <= 25)
                                //    {
                                //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                                //        rows++;
                                //        count++;
                                //        if (count == 6) { rows++; rows++; count = 0; }
                                //    }
                                //    else if (documento.clave >= 27 & documento.clave <= 32)
                                //    {
                                //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                                //        rows++;
                                //        count++;
                                //        if (count == 6) { rows++; rows++; count = 0; }
                                //    }
                                //    else if (documento.clave >= 33 & documento.clave <= 40)
                                //    {
                                //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                                //        rows++;
                                //        count++;

                                //        if (listDoc.Count == 96)
                                //        {
                                //            //if (count == 8) { rows++; rows++; count = 0; }
                                //            if (count == 8) { count = 0; }
                                //        }
                                //        else if (listDoc.Count == 97)
                                //        {
                                //            if (count == 9) { rows++; count = 0; }
                                //        }                                    
                                //    }
                                //    else if (documento.clave == 41)
                                //    {
                                //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                                //        rows++;
                                //        count++;
                                //        if (count == 1) { rows++; count = 0; }
                                //    }
                                //    else if (documento.clave >= 42 & documento.clave <= 43)
                                //    {
                                //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                                //        rows++;
                                //        count++;
                                //        if (count == 2) { rows++; count = 0; }
                                //    }
                                //    else if (documento.clave == 44)
                                //    {
                                //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                                //        rows++;
                                //        count++;
                                //        if (count == 1) { rows++; rows++; count = 0; }
                                //    }
                                //    else if (documento.clave >= 45 & documento.clave <= 57)
                                //    {
                                //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                                //        rows++;
                                //        count++;
                                //        if (count == 13) { rows++; count = 0; }
                                //    }
                                //    else if (documento.clave >= 58 & documento.clave <= 62)
                                //    {
                                //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                                //        rows++;
                                //        count++;
                                //        if (count == 5) { rows++; count = 0; }
                                //    }
                                //    else if (documento.clave >= 63 & documento.clave <= 70)
                                //    {
                                //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                                //        rows++;
                                //        count++;
                                //        if (listDoc.Count == 96)
                                //        {
                                //            if (count == 8) { count = 0; }
                                //        }                                    
                                //    }
                                //    else if (documento.clave == 71)
                                //    {
                                //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                                //        rows++;
                                //        count++;

                                //        if (listDoc.Count == 96)
                                //        {
                                //            if (count == 1) { count = 0; }
                                //        }
                                //        else if (listDoc.Count == 97)
                                //        {
                                //            count = 0; 
                                //        }
                                //    }
                                //    else if (documento.clave == 72)
                                //    {
                                //        excelWorksheet.Cells[93, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                                //        rows = 93;
                                //        rows++;
                                //        count++;
                                //        if (count == 1) { rows++; count = 0; }
                                //    }
                                //    else if (documento.clave >= 73 & documento.clave <= 75)
                                //    {
                                //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                                //        rows++;
                                //        count++;
                                //        if (count == 3) { rows++; count = 0; }
                                //    }
                                //    else if (documento.clave >= 76 & documento.clave <= 84)
                                //    {
                                //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                                //        rows++;
                                //        count++;
                                //        if (count == 9) { rows++; count = 0; }
                                //    }
                                //    else if (documento.clave >= 85 & documento.clave <= 90)
                                //    {
                                //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                                //        rows++;
                                //        count++;
                                //        if (count == 6) { rows++; count = 0; }
                                //    }
                                //    else if (documento.clave >= 91 & documento.clave <= 96)
                                //    {
                                //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                                //        rows++;
                                //        count++;
                                //        if (count == 6) { rows++; count = 0; }
                                //    }
                                //    else if (documento.clave >= 97 & documento.clave <= 98)
                                //    {
                                //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                                //        rows++;
                                //        count++;
                                //        //if (count == 2) { rows++; count = 0; }
                                //    }
                                //}

                                if (listDoc.Count == 98 | listDoc.Count == 196)
                                {
                                    if (documento.clave == 62)
                                    {
                                        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                                        rows++;
                                        count++;
                                        if (count == 5) { rows++; count = 0; }
                                    }
                                    else if (documento.grupo.Equals("PLANEACIÓN,  PROGRAMACIÓN  Y  PRESUPUESTACIÓN"))
                                    {
                                        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                                        rows++;
                                        count++;
                                        if (count == 10) { rows++; count = 0; }
                                    }
                                    else if (documento.grupo.Equals("ESTUDIOS  PREVIOS"))
                                    {
                                        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                                        rows++;
                                        count++;
                                        if (count == 9) { rows++; count = 0; }
                                    }
                                    else if (documento.grupo.Equals("PROYECTO  EJECUTIVO"))
                                    {
                                        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                                        rows++;
                                        count++;
                                        if (count == 7) { rows++; count = 0; }
                                    }
                                    else if (documento.grupo.Equals("GENERALIDADES"))
                                    {
                                        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                                        rows++;
                                        count++;
                                        if (count == 6) { rows++; rows++; count = 0; }
                                    }
                                    else if (documento.grupo.Equals("PROCEDIMIENTO DE LICITACIÓN"))
                                    {
                                        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                                        rows++;
                                        count++;
                                        if (count == 9) { rows++; count = 0; }
                                    }
                                    else if (documento.grupo.Equals("FALLO PARA LA LICITACIÓN"))
                                    {
                                        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                                        rows++;
                                        count++;
                                        if (count == 2) { rows++; count = 0; }
                                    }
                                    else if (documento.grupo.Equals("EXCEPCIONES PARA LA LICITACIÓN"))
                                    {
                                        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                                        rows++;
                                        count++;
                                        if (count == 1) { rows++; rows++; count = 0; }
                                    }
                                    else if (documento.grupo.Equals("CONTRATACIÓN"))
                                    {
                                        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                                        rows++;
                                        count++;
                                        if (count == 13) { rows++; count = 0; }
                                    }
                                    else if (documento.grupo.Equals("EJECUCIÓN"))
                                    {
                                        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                                        rows++;
                                        count++;
                                        if (count == 5) { rows++; count = 0; }
                                    }
                                    else if (documento.grupo.Equals("ESTIMACIONES DE OBRA DEBIDAMENTE REQUISITADAS"))
                                    {
                                        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                                        rows++;
                                        count++;
                                        if (count == 10) { rows++; count = 0; }
                                    }
                                    else if (documento.grupo.Equals("AJUSTES DE COSTOS"))
                                    {
                                        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                                        rows++;
                                        count++;
                                        if (count == 3) { rows++; count = 0; }
                                    }
                                    else if (documento.grupo.Equals("CONVENIOS"))
                                    {
                                        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                                        rows++;
                                        count++;
                                        if (count == 9) { rows++; count = 0; }
                                    }
                                    else if (documento.grupo.Equals("SUSPENSIÓN, TERMINACIÓN ANTICIPADA O RESCISIÓN"))
                                    {
                                        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                                        rows++;
                                        count++;
                                        if (count == 6) { rows++; count = 0; }
                                    }
                                    else if (documento.grupo.Equals("TERMINACIÓN DE LOS TRABAJOS"))
                                    {
                                        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                                        rows++;
                                        count++;
                                        if (count == 6) { rows++; count = 0; }
                                    }
                                    else if (documento.grupo.Equals("OTROS"))
                                    {
                                        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                                        rows++;
                                        count++;
                                        if (count == 2) { rows++; count = 0; break; }
                                    }
                                }

                            }

                            gralSI = gralSI + SI;
                            gralNO = gralNO + NO;
                            gralNA = gralNA + NA;

                            //SI
                            excelWorksheet.Cells[125, columns].Value = SI; rows++;
                            //NO
                            excelWorksheet.Cells[126, columns].Value = NO; rows++;
                            //NA
                            excelWorksheet.Cells[127, columns].Value = NA; rows++;
                            //Sumatoria
                            excelWorksheet.Cells[128, columns].Value = (SI + NO + NA);

                            rows = 8;
                            columns++;
                            SI = 0;
                            NO = 0;
                            NA = 0;
                            count = 0;
                        }
                    }

                    columns++;
                    int col = columns;

                    rows++;
                    rows++;
                    rows++;

                    columns = 56;

                    #region SI, NO, N/A
                    /*
                    foreach (AdquisicionesV1 data in listAdquisiciones)
                    {
                        foreach (var documents in data.documentos)
                        {
                            if (documents.grupo.Equals("DOCUMENTACIÓN ADMINISTRATIVA"))
                            {
                                columns = 56;
                                columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BA" + rows + ",\"SI\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BA" + rows + ",\"NO\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BA" + rows + ",\"N/A\")"; columns++;

                                rows++;
                                count++;
                                if (count == 5) { rows++; count = 0; }
                            }
                            else if (documents.grupo.Equals("PROCEDIMIENTO DE ADJUDICACIÓN"))
                            {
                                columns = 56;
                                columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                                rows++;
                                count++;
                                if (count == 13) { rows++; count = 0; }
                            }
                            else if (documents.grupo.Equals("REQUISITOS DEL CONTRATO"))
                            {
                                columns = 56;
                                columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                                rows++;
                                count++;
                                if (count == 16) { rows++; count = 0; }
                            }
                            else if (documents.grupo.Equals("GARANTÍAS"))
                            {
                                columns = 56;
                                columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                                rows++;
                                count++;
                                if (count == 3) { rows++; count = 0; }
                            }
                            else if (documents.grupo.Equals("ENTREGABLES"))
                            {
                                columns = 56;
                                columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                                rows++;
                                count++;
                                if (count == 8) { rows++; count = 0; }
                            }
                        }
                        break;
                    }
                    */
                    #endregion

                    //SI
                    excelWorksheet.Cells[125, 6].Value = gralSI;
                    //NO
                    excelWorksheet.Cells[126, 6].Value = gralNO;
                    //NA
                    excelWorksheet.Cells[127, 6].Value = gralNA;
                    //Suma Gral
                    excelWorksheet.Cells[128, 6].Value = (gralSI + gralNO + gralNA);
                    //Total Expedientes
                    excelWorksheet.Cells[129, 6].Value = listObras.Count;

                    SI = 0;
                    NO = 0;
                    NA = 0;
                    count = 0;
                }
                #endregion
                #region SAN ANDRES CHOLULA
                else if (municipio.Equals("SAN ANDRES CHOLULA"))
                {
                    foreach (ObraPublicaV1 data in listObras)
                    {
                        listDoc = data.documentos;

                        string numero = data.numeroObra;
                        int counts = listDoc.Count;

                        if (listDoc.Count == 98)
                        {
                            excelWorksheet.Cells[rows, columns].Value = data.numeroObra;

                            rows++;
                            rows++;
                            rows++;

                            if (listDoc.Count != 196)
                            {
                                Sort(ref listDoc, "clave", "ASC");
                            }

                            // documentos en Excel
                            //96 documentos antigua version
                            //98 documentos antigua version

                            foreach (var documento in listDoc)
                            {
                                string val = documento.integracion;

                                if (val == null)
                                {
                                    NO++;
                                }
                                else if (val.Equals("NO"))
                                {
                                    NO++;
                                }
                                else if (val.Equals("SI"))
                                {
                                    SI++;
                                }
                                else if (val.Equals("N/A"))
                                {
                                    NA++;
                                }



                                if (listDoc.Count == 98 | listDoc.Count == 196)
                                {
                                    if (documento.clave == 62)
                                    {
                                        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                                        rows++;
                                        count++;
                                        if (count == 5) { rows++; count = 0; }
                                    }
                                    else if (documento.grupo.Equals("PLANEACIÓN,  PROGRAMACIÓN  Y  PRESUPUESTACIÓN"))
                                    {
                                        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                                        rows++;
                                        count++;
                                        if (count == 10) { rows++; count = 0; }
                                    }
                                    else if (documento.grupo.Equals("ESTUDIOS  PREVIOS"))
                                    {
                                        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                                        rows++;
                                        count++;
                                        if (count == 9) { rows++; count = 0; }
                                    }
                                    else if (documento.grupo.Equals("PROYECTO  EJECUTIVO"))
                                    {
                                        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                                        rows++;
                                        count++;
                                        if (count == 7) { rows++; count = 0; }
                                    }
                                    else if (documento.grupo.Equals("GENERALIDADES"))
                                    {
                                        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                                        rows++;
                                        count++;
                                        if (count == 6) { rows++; rows++; count = 0; }
                                    }
                                    else if (documento.grupo.Equals("PROCEDIMIENTO DE LICITACIÓN"))
                                    {
                                        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                                        rows++;
                                        count++;
                                        if (count == 9) { rows++; count = 0; }
                                    }
                                    else if (documento.grupo.Equals("FALLO PARA LA LICITACIÓN"))
                                    {
                                        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                                        rows++;
                                        count++;
                                        if (count == 2) { rows++; count = 0; }
                                    }
                                    else if (documento.grupo.Equals("EXCEPCIONES PARA LA LICITACIÓN"))
                                    {
                                        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                                        rows++;
                                        count++;
                                        if (count == 1) { rows++; rows++; count = 0; }
                                    }
                                    else if (documento.grupo.Equals("CONTRATACIÓN"))
                                    {
                                        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                                        rows++;
                                        count++;
                                        if (count == 13) { rows++; count = 0; }
                                    }
                                    else if (documento.grupo.Equals("EJECUCIÓN"))
                                    {
                                        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                                        rows++;
                                        count++;
                                        if (count == 5) { rows++; count = 0; }
                                    }
                                    else if (documento.grupo.Equals("ESTIMACIONES DE OBRA DEBIDAMENTE REQUISITADAS"))
                                    {
                                        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                                        rows++;
                                        count++;
                                        if (count == 10) { rows++; count = 0; }
                                    }
                                    else if (documento.grupo.Equals("AJUSTES DE COSTOS"))
                                    {
                                        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                                        rows++;
                                        count++;
                                        if (count == 3) { rows++; count = 0; }
                                    }
                                    else if (documento.grupo.Equals("CONVENIOS"))
                                    {
                                        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                                        rows++;
                                        count++;
                                        if (count == 9) { rows++; count = 0; }
                                    }
                                    else if (documento.grupo.Equals("SUSPENSIÓN, TERMINACIÓN ANTICIPADA O RESCISIÓN"))
                                    {
                                        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                                        rows++;
                                        count++;
                                        if (count == 6) { rows++; count = 0; }
                                    }
                                    else if (documento.grupo.Equals("TERMINACIÓN DE LOS TRABAJOS"))
                                    {
                                        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                                        rows++;
                                        count++;
                                        if (count == 6) { rows++; count = 0; }
                                    }
                                    else if (documento.grupo.Equals("OTROS"))
                                    {
                                        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                                        rows++;
                                        count++;
                                        if (count == 2) { rows++; count = 0; break; }
                                    }
                                }

                            }

                            gralSI = gralSI + SI;
                            gralNO = gralNO + NO;
                            gralNA = gralNA + NA;

                            //SI
                            excelWorksheet.Cells[125, columns].Value = SI; rows++;
                            //NO
                            excelWorksheet.Cells[126, columns].Value = NO; rows++;
                            //NA
                            excelWorksheet.Cells[127, columns].Value = NA; rows++;
                            //Sumatoria
                            excelWorksheet.Cells[128, columns].Value = (SI + NO + NA);

                            rows = 8;
                            columns++;
                            SI = 0;
                            NO = 0;
                            NA = 0;
                            count = 0;
                        }
                    }

                    columns++;
                    int col = columns;

                    rows++;
                    rows++;
                    rows++;

                    columns = 56;

                    #region SI, NO, N/A
                    /*
                    foreach (AdquisicionesV1 data in listAdquisiciones)
                    {
                        foreach (var documents in data.documentos)
                        {
                            if (documents.grupo.Equals("DOCUMENTACIÓN ADMINISTRATIVA"))
                            {
                                columns = 56;
                                columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BA" + rows + ",\"SI\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BA" + rows + ",\"NO\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BA" + rows + ",\"N/A\")"; columns++;

                                rows++;
                                count++;
                                if (count == 5) { rows++; count = 0; }
                            }
                            else if (documents.grupo.Equals("PROCEDIMIENTO DE ADJUDICACIÓN"))
                            {
                                columns = 56;
                                columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                                rows++;
                                count++;
                                if (count == 13) { rows++; count = 0; }
                            }
                            else if (documents.grupo.Equals("REQUISITOS DEL CONTRATO"))
                            {
                                columns = 56;
                                columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                                rows++;
                                count++;
                                if (count == 16) { rows++; count = 0; }
                            }
                            else if (documents.grupo.Equals("GARANTÍAS"))
                            {
                                columns = 56;
                                columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                                rows++;
                                count++;
                                if (count == 3) { rows++; count = 0; }
                            }
                            else if (documents.grupo.Equals("ENTREGABLES"))
                            {
                                columns = 56;
                                columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                                excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                                rows++;
                                count++;
                                if (count == 8) { rows++; count = 0; }
                            }
                        }
                        break;
                    }
                    */
                    #endregion

                    //SI
                    excelWorksheet.Cells[125, 6].Value = gralSI;
                    //NO
                    excelWorksheet.Cells[126, 6].Value = gralNO;
                    //NA
                    excelWorksheet.Cells[127, 6].Value = gralNA;
                    //Suma Gral
                    excelWorksheet.Cells[128, 6].Value = (gralSI + gralNO + gralNA);
                    //Total Expedientes
                    excelWorksheet.Cells[129, 6].Value = listObras.Count;

                    SI = 0;
                    NO = 0;
                    NA = 0;
                    count = 0;
                }
                #endregion

                #region Anterior
                
                //foreach (ObraPublicaV1 data in listObras)
                //{
                //    listDoc = data.documentos;

                //    string numero = data.numeroObra;
                //    int counts = listDoc.Count;

                //    if (listDoc.Count == 98)
                //    {
                //        excelWorksheet.Cells[rows, columns].Value = data.numeroObra;

                //        rows++;
                //        rows++;
                //        rows++;

                //        if (listDoc.Count != 196)
                //        {
                //            Sort(ref listDoc, "clave", "ASC");
                //        }

                //        // documentos en Excel
                //        //96 documentos antigua version
                //        //98 documentos antigua version

                //        foreach (var documento in listDoc)
                //        {
                //            string val = documento.integracion;

                //            if (val == null)
                //            {
                //                NO++;
                //            }
                //            else if (val.Equals("NO"))
                //            {
                //                NO++;
                //            }
                //            else if (val.Equals("SI"))
                //            {
                //                SI++;
                //            }
                //            else if (val.Equals("N/A"))
                //            {
                //                NA++;
                //            }

                //            //if (listDoc.Count == 95)
                //            //{
                //            //    if (documento.clave >= 1 & documento.clave <= 10)
                //            //    {
                //            //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                //            //        rows++;
                //            //        count++;
                //            //        if (count == 10) { rows++; count = 0; }
                //            //    }
                //            //    else if (documento.clave >= 11 & documento.clave <= 19)
                //            //    {
                //            //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                //            //        rows++;
                //            //        count++;
                //            //        if (count == 9) { rows++; count = 0; }
                //            //    }
                //            //    else if (documento.clave >= 20 & documento.clave <= 25)
                //            //    {
                //            //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                //            //        rows++;
                //            //        count++;
                //            //        if (count == 6) { rows++; rows++; count = 0; }
                //            //    }
                //            //    else if (documento.clave >= 27 & documento.clave <= 32)
                //            //    {
                //            //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                //            //        rows++;
                //            //        count++;
                //            //        if (count == 6) { rows++; rows++; count = 0; }
                //            //    }
                //            //    else if (documento.clave >= 33 & documento.clave <= 41)
                //            //    {
                //            //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                //            //        rows++;
                //            //        count++;
                //            //        if (count == 9) { rows++; count = 0; }
                //            //    }
                //            //    else if (documento.clave >= 42 & documento.clave <= 43)
                //            //    {
                //            //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                //            //        rows++;
                //            //        count++;
                //            //        if (count == 2) { rows++; count = 0; }
                //            //    }
                //            //    else if (documento.clave == 44)
                //            //    {
                //            //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                //            //        rows++;
                //            //        count++;
                //            //        if (count == 1) { rows++; rows++; count = 0; }
                //            //    }
                //            //    else if (documento.clave >= 45 & documento.clave <= 57)
                //            //    {
                //            //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                //            //        rows++;
                //            //        count++;
                //            //        if (count == 13) { rows++; count = 0; }
                //            //    }
                //            //    else if (documento.clave >= 58 & documento.clave <= 62)
                //            //    {
                //            //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                //            //        rows++;
                //            //        count++;
                //            //        if (count == 5) { rows++; count = 0; }
                //            //    }
                //            //    else if (documento.clave >= 63 & documento.clave <= 72)
                //            //    {
                //            //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                //            //        rows++;
                //            //        count++;
                //            //        if (count == 10) { rows++; count = 0; }
                //            //    }
                //            //    else if (documento.clave >= 73 & documento.clave <= 75)
                //            //    {
                //            //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                //            //        rows++;
                //            //        count++;
                //            //        if (count == 3) { rows++; count = 0; }
                //            //    }
                //            //    else if (documento.clave >= 76 & documento.clave <= 77)
                //            //    {
                //            //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                //            //        rows++;
                //            //        count++;
                //            //        if (count == 2) { rows++; rows++; count = 0; }
                //            //    }
                //            //    else if (documento.clave >= 80 & documento.clave <= 84)
                //            //    {
                //            //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                //            //        rows++;
                //            //        count++;
                //            //        if (count == 5) { rows++; count = 0; }
                //            //    }
                //            //    else if (documento.clave >= 85 & documento.clave <= 90)
                //            //    {
                //            //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                //            //        rows++;
                //            //        count++;
                //            //        if (count == 6) { rows++; count = 0; }
                //            //    }
                //            //    else if (documento.clave >= 91 & documento.clave <= 96)
                //            //    {
                //            //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                //            //        rows++;
                //            //        count++;
                //            //        if (count == 6) { rows++; count = 0; }
                //            //    }
                //            //    else if (documento.clave >= 97 & documento.clave <= 98)
                //            //    {
                //            //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                //            //        rows++;
                //            //        count++;
                //            //        if (count == 2) { rows++; count = 0; }
                //            //    }
                //            //}
                //            //else if (listDoc.Count == 96 | listDoc.Count == 97)
                //            //{
                //            //    if (documento.clave >= 1 & documento.clave <= 10)
                //            //    {
                //            //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                //            //        rows++;
                //            //        count++;
                //            //        if (count == 10) { rows++; count = 0; }
                //            //    }
                //            //    else if (documento.clave >= 11 & documento.clave <= 19)
                //            //    {
                //            //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                //            //        rows++;
                //            //        count++;
                //            //        if (count == 9) { rows++; count = 0; }
                //            //    }
                //            //    else if (documento.clave >= 20 & documento.clave <= 25)
                //            //    {
                //            //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                //            //        rows++;
                //            //        count++;
                //            //        if (count == 6) { rows++; rows++; count = 0; }
                //            //    }
                //            //    else if (documento.clave >= 27 & documento.clave <= 32)
                //            //    {
                //            //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                //            //        rows++;
                //            //        count++;
                //            //        if (count == 6) { rows++; rows++; count = 0; }
                //            //    }
                //            //    else if (documento.clave >= 33 & documento.clave <= 40)
                //            //    {
                //            //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                //            //        rows++;
                //            //        count++;

                //            //        if (listDoc.Count == 96)
                //            //        {
                //            //            //if (count == 8) { rows++; rows++; count = 0; }
                //            //            if (count == 8) { count = 0; }
                //            //        }
                //            //        else if (listDoc.Count == 97)
                //            //        {
                //            //            if (count == 9) { rows++; count = 0; }
                //            //        }                                    
                //            //    }
                //            //    else if (documento.clave == 41)
                //            //    {
                //            //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                //            //        rows++;
                //            //        count++;
                //            //        if (count == 1) { rows++; count = 0; }
                //            //    }
                //            //    else if (documento.clave >= 42 & documento.clave <= 43)
                //            //    {
                //            //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                //            //        rows++;
                //            //        count++;
                //            //        if (count == 2) { rows++; count = 0; }
                //            //    }
                //            //    else if (documento.clave == 44)
                //            //    {
                //            //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                //            //        rows++;
                //            //        count++;
                //            //        if (count == 1) { rows++; rows++; count = 0; }
                //            //    }
                //            //    else if (documento.clave >= 45 & documento.clave <= 57)
                //            //    {
                //            //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                //            //        rows++;
                //            //        count++;
                //            //        if (count == 13) { rows++; count = 0; }
                //            //    }
                //            //    else if (documento.clave >= 58 & documento.clave <= 62)
                //            //    {
                //            //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                //            //        rows++;
                //            //        count++;
                //            //        if (count == 5) { rows++; count = 0; }
                //            //    }
                //            //    else if (documento.clave >= 63 & documento.clave <= 70)
                //            //    {
                //            //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                //            //        rows++;
                //            //        count++;
                //            //        if (listDoc.Count == 96)
                //            //        {
                //            //            if (count == 8) { count = 0; }
                //            //        }                                    
                //            //    }
                //            //    else if (documento.clave == 71)
                //            //    {
                //            //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                //            //        rows++;
                //            //        count++;

                //            //        if (listDoc.Count == 96)
                //            //        {
                //            //            if (count == 1) { count = 0; }
                //            //        }
                //            //        else if (listDoc.Count == 97)
                //            //        {
                //            //            count = 0; 
                //            //        }
                //            //    }
                //            //    else if (documento.clave == 72)
                //            //    {
                //            //        excelWorksheet.Cells[93, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                //            //        rows = 93;
                //            //        rows++;
                //            //        count++;
                //            //        if (count == 1) { rows++; count = 0; }
                //            //    }
                //            //    else if (documento.clave >= 73 & documento.clave <= 75)
                //            //    {
                //            //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                //            //        rows++;
                //            //        count++;
                //            //        if (count == 3) { rows++; count = 0; }
                //            //    }
                //            //    else if (documento.clave >= 76 & documento.clave <= 84)
                //            //    {
                //            //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                //            //        rows++;
                //            //        count++;
                //            //        if (count == 9) { rows++; count = 0; }
                //            //    }
                //            //    else if (documento.clave >= 85 & documento.clave <= 90)
                //            //    {
                //            //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                //            //        rows++;
                //            //        count++;
                //            //        if (count == 6) { rows++; count = 0; }
                //            //    }
                //            //    else if (documento.clave >= 91 & documento.clave <= 96)
                //            //    {
                //            //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                //            //        rows++;
                //            //        count++;
                //            //        if (count == 6) { rows++; count = 0; }
                //            //    }
                //            //    else if (documento.clave >= 97 & documento.clave <= 98)
                //            //    {
                //            //        excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                //            //        rows++;
                //            //        count++;
                //            //        //if (count == 2) { rows++; count = 0; }
                //            //    }
                //            //}

                //            if (listDoc.Count == 98 | listDoc.Count == 196)
                //            {
                //                if (documento.clave == 62)
                //                {
                //                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                //                    rows++;
                //                    count++;
                //                    if (count == 5) { rows++; count = 0; }
                //                }
                //                else if (documento.grupo.Equals("PLANEACIÓN,  PROGRAMACIÓN  Y  PRESUPUESTACIÓN"))
                //                {
                //                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                //                    rows++;
                //                    count++;
                //                    if (count == 10) { rows++; count = 0; }
                //                }
                //                else if (documento.grupo.Equals("ESTUDIOS  PREVIOS"))
                //                {
                //                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                //                    rows++;
                //                    count++;
                //                    if (count == 9) { rows++; count = 0; }
                //                }
                //                else if (documento.grupo.Equals("PROYECTO  EJECUTIVO"))
                //                {
                //                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                //                    rows++;
                //                    count++;
                //                    if (count == 7) { rows++; count = 0; }
                //                }
                //                else if (documento.grupo.Equals("GENERALIDADES"))
                //                {
                //                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                //                    rows++;
                //                    count++;
                //                    if (count == 6) { rows++; rows++; count = 0; }
                //                }
                //                else if (documento.grupo.Equals("PROCEDIMIENTO DE LICITACIÓN"))
                //                {
                //                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                //                    rows++;
                //                    count++;
                //                    if (count == 9) { rows++; count = 0; }
                //                }
                //                else if (documento.grupo.Equals("FALLO PARA LA LICITACIÓN"))
                //                {
                //                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                //                    rows++;
                //                    count++;
                //                    if (count == 2) { rows++; count = 0; }
                //                }
                //                else if (documento.grupo.Equals("EXCEPCIONES PARA LA LICITACIÓN"))
                //                {
                //                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                //                    rows++;
                //                    count++;
                //                    if (count == 1) { rows++; rows++; count = 0; }
                //                }
                //                else if (documento.grupo.Equals("CONTRATACIÓN"))
                //                {
                //                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                //                    rows++;
                //                    count++;
                //                    if (count == 13) { rows++; count = 0; }
                //                }
                //                else if (documento.grupo.Equals("EJECUCIÓN"))
                //                {
                //                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                //                    rows++;
                //                    count++;
                //                    if (count == 5) { rows++; count = 0; }
                //                }
                //                else if (documento.grupo.Equals("ESTIMACIONES DE OBRA DEBIDAMENTE REQUISITADAS"))
                //                {
                //                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                //                    rows++;
                //                    count++;
                //                    if (count == 10) { rows++; count = 0; }
                //                }
                //                else if (documento.grupo.Equals("AJUSTES DE COSTOS"))
                //                {
                //                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                //                    rows++;
                //                    count++;
                //                    if (count == 3) { rows++; count = 0; }
                //                }
                //                else if (documento.grupo.Equals("CONVENIOS"))
                //                {
                //                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                //                    rows++;
                //                    count++;
                //                    if (count == 9) { rows++; count = 0; }
                //                }
                //                else if (documento.grupo.Equals("SUSPENSIÓN, TERMINACIÓN ANTICIPADA O RESCISIÓN"))
                //                {
                //                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                //                    rows++;
                //                    count++;
                //                    if (count == 6) { rows++; count = 0; }
                //                }
                //                else if (documento.grupo.Equals("TERMINACIÓN DE LOS TRABAJOS"))
                //                {
                //                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                //                    rows++;
                //                    count++;
                //                    if (count == 6) { rows++; count = 0; }
                //                }
                //                else if (documento.grupo.Equals("OTROS"))
                //                {
                //                    excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(documento.integracion) ? "NO" : documento.integracion;
                //                    rows++;
                //                    count++;
                //                    if (count == 2) { rows++; count = 0; break; }
                //                }
                //            }

                //        }

                //        gralSI = gralSI + SI;
                //        gralNO = gralNO + NO;
                //        gralNA = gralNA + NA;

                //        //SI
                //        excelWorksheet.Cells[125, columns].Value = SI; rows++;
                //        //NO
                //        excelWorksheet.Cells[126, columns].Value = NO; rows++;
                //        //NA
                //        excelWorksheet.Cells[127, columns].Value = NA; rows++;
                //        //Sumatoria
                //        excelWorksheet.Cells[128, columns].Value = (SI + NO + NA);

                //        rows = 8;
                //        columns++;
                //        SI = 0;
                //        NO = 0;
                //        NA = 0;
                //        count = 0;
                //    }
                //}

                //columns++;
                //int col = columns;

                //rows++;
                //rows++;
                //rows++;

                //columns = 56;

                //#region SI, NO, N/A
                ///*
                //foreach (AdquisicionesV1 data in listAdquisiciones)
                //{
                //    foreach (var documents in data.documentos)
                //    {
                //        if (documents.grupo.Equals("DOCUMENTACIÓN ADMINISTRATIVA"))
                //        {
                //            columns = 56;
                //            columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BA" + rows + ",\"SI\")"; columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BA" + rows + ",\"NO\")"; columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BA" + rows + ",\"N/A\")"; columns++;
                            
                //            rows++;
                //            count++;
                //            if (count == 5) { rows++; count = 0; }
                //        }
                //        else if (documents.grupo.Equals("PROCEDIMIENTO DE ADJUDICACIÓN"))
                //        {
                //            columns = 56;
                //            columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                //            rows++;
                //            count++;
                //            if (count == 13) { rows++; count = 0; }
                //        }
                //        else if (documents.grupo.Equals("REQUISITOS DEL CONTRATO"))
                //        {
                //            columns = 56;
                //            columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                //            rows++;
                //            count++;
                //            if (count == 16) { rows++; count = 0; }
                //        }
                //        else if (documents.grupo.Equals("GARANTÍAS"))
                //        {
                //            columns = 56;
                //            columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                //            rows++;
                //            count++;
                //            if (count == 3) { rows++; count = 0; }
                //        }
                //        else if (documents.grupo.Equals("ENTREGABLES"))
                //        {
                //            columns = 56;
                //            columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"SI\")"; columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"NO\")"; columns++;
                //            excelWorksheet.Cells[rows, columns].Formula = "=CONTAR.SI(G" + rows + ":BC" + rows + ",\"N/A\")"; columns++;
                //            rows++;
                //            count++;
                //            if (count == 8) { rows++; count = 0; }
                //        }
                //    }
                //    break;
                //}
                //*/
                //#endregion

                ////SI
                //excelWorksheet.Cells[125, 6].Value = gralSI;
                ////NO
                //excelWorksheet.Cells[126, 6].Value = gralNO;
                ////NA
                //excelWorksheet.Cells[127, 6].Value = gralNA;
                ////Suma Gral
                //excelWorksheet.Cells[128, 6].Value = (gralSI + gralNO + gralNA);
                ////Total Expedientes
                //excelWorksheet.Cells[129, 6].Value = listObras.Count;

                //SI = 0;
                //NO = 0;
                //NA = 0;
                //count = 0;

                #endregion

                excelPackage.SaveAs(ms);
            }

            ms.Position = 0;

            Spire.Xls.Workbook workbook = new Spire.Xls.Workbook();
            workbook.LoadFromStream(ms);

            var reporte = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(ms.ToArray())
            };
            reporte.Content.Headers.ContentDisposition =
                new ContentDispositionHeaderValue("attachment")
                {
                    FileName = "ReporteObraPublica.xlsx"
                };
            reporte.Content.Headers.ContentType =
                new MediaTypeHeaderValue("application/octet-stream");

            return reporte;
        }

        /// BackLog 838 Task - 975
        /// <summary>
        /// Metodo que genera los reportes Generales del Municipio seleccionado
        /// </summary>
        /// <param name="municipio"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("api/report/obra/general/adquisiciones/obra/zip/{municipio}")]
        public async System.Threading.Tasks.Task<HttpResponseMessage> ReporteGlobalZip(string municipio)
        {

            var fileName = string.Empty;
            var tempOutput = string.Empty;
            var nameZip = path + "ReporteGlobal.zip";

            #region 

            List<string> filesCsv = new List<string>();

            if (municipio.Equals("TEPEACA"))
            {
                fileName = "Reporte_Adquisiciones_TEPEACA.xlsx";
            }
            else if (municipio.Equals("SAN ANDRES CHOLULA"))
            {
                fileName = "Reporte_Adquisiciones_ESACH.xlsx";
            }
            tempOutput = path + fileName;

            ReporteGlobalAdquisiciones(municipio, tempOutput);
            filesCsv.Add(tempOutput);

            if (municipio.Equals("TEPEACA"))
            {
                fileName = "Reporte_Obra_TEPEACA.xlsx";
            }
            else if (municipio.Equals("SAN ANDRES CHOLULA"))
            {
                fileName = "Reporte_Obra_ESACH.xlsx";
            }

            tempOutput = path + fileName;

            ReporteGlobalObra(municipio, tempOutput);
            filesCsv.Add(tempOutput);

            //Zipeando los archivos excel
            using (ZipOutputStream zipStream = new ZipOutputStream(File.Create(nameZip)))
            {
                zipStream.SetLevel(9);
                byte[] buffer = new byte[4096];

                for (int i = 0; i < filesCsv.Count; i++)
                {
                    ZipEntry entry = new ZipEntry(Path.GetFileName(filesCsv[i]));
                    entry.DateTime = DateTime.Now;
                    entry.IsUnicodeText = true;
                    zipStream.PutNextEntry(entry);

                    using (FileStream fileStream = File.OpenRead(filesCsv[i]))
                    {
                        int sourceBytes;
                        do
                        {
                            sourceBytes = fileStream.Read(buffer, 0, buffer.Length);
                            zipStream.Write(buffer, 0, sourceBytes);
                        } while (sourceBytes > 0);
                    }
                }
                zipStream.Finish();
                zipStream.Flush();
                zipStream.Close();
            }

            //Archivo zip a MemoryStream
            MemoryStream memoryZip = new MemoryStream();
            using (FileStream file = new FileStream(nameZip, FileMode.Open, FileAccess.Read))
            {
                byte[] bytes = new byte[file.Length];
                file.Read(bytes, 0, (int)file.Length);
                memoryZip.Write(bytes, 0, (int)file.Length);
                filesCsv.Add(nameZip);
            }

            //Eliminando Archivos de Excel Generados
            if (File.Exists(tempOutput))
            {
                foreach (var archivos in filesCsv)
                {
                    File.Delete(archivos);
                }
            }

            var reporte = new HttpResponseMessage(HttpStatusCode.OK);
            reporte.Content = new ByteArrayContent(memoryZip.ToArray());
            reporte.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            reporte.Content.Headers.ContentDisposition.FileName = "ReporteGlobal.zip";
            reporte.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            return reporte;

            #endregion

        }

        private string nombreAuditor(string idAuditor)
        {

            string constr = ConfigurationManager.AppSettings["connectionString"];
            var Client = new MongoClient(constr);
            var DB = Client.GetDatabase("PRB");
           
            var collectionUser = DB.GetCollection<UsersPRB>("UsersPBR");
            var filter = Builders<UsersPRB>.Filter.Eq(x => x.Id, idAuditor);

            var result = collectionUser.Find(filter).FirstOrDefault();

            if (result != null)
            {
                return result.name;
            }

            return "";
        }

        private void ReporteGlobalAdquisiciones(string municipio, string path)
        {
            string constr = ConfigurationManager.AppSettings["connectionString"];
            var Client = new MongoClient(constr);
            var DB = Client.GetDatabase("PRB");
            var collection = DB.GetCollection<AdquisicionesV1>("Adquisiciones");

            var filter = Builders<AdquisicionesV1>.Filter.Eq(x => x.municipio, municipio);
            var result = collection.Find(filter).ToList();

            MemoryStream memoryStream = new MemoryStream();
            WebClient client = new WebClient();

            try
            {
                //memoryStream = new MemoryStream(client.DownloadData("C:/Simopa/Templete/TemplateItinerarios.xlsx"));
                memoryStream = new MemoryStream(client.DownloadData("https://apipbrdevelolop.azurewebsites.net/templates/reportes/TempPlantillaGeneral.xlsx"));
            }
            finally
            {
                client.Dispose();
            }

            int rows = 1;
            int columns = 1;
            MemoryStream ms = new MemoryStream();

            using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {
                ExcelWorkbook excelWorkBook = excelPackage.Workbook;
                ExcelWorksheet excelWorksheet = excelWorkBook.Worksheets[1];

                excelWorksheet.Cells[rows, columns].Value = "*"; columns++;
                excelWorksheet.Cells[rows, columns].Value = "No Procedimiento"; columns++;
                excelWorksheet.Cells[rows, columns].Value = "No Contrato"; columns++;
                excelWorksheet.Cells[rows, columns].Value = "Ejercicio"; columns++;
                excelWorksheet.Cells[rows, columns].Value = "Proveedor"; columns++;
                excelWorksheet.Cells[rows, columns].Value = "Monto Contrato"; columns++;
                excelWorksheet.Cells[rows, columns].Value = "Monto Ejercido"; columns++;
                excelWorksheet.Cells[rows, columns].Value = "Concepto"; columns++;
                excelWorksheet.Cells[rows, columns].Value = "Tipo de Procedimiento"; columns++;
                excelWorksheet.Cells[rows, columns].Value = "Avance Documental"; columns++;
                excelWorksheet.Cells[rows, columns].Value = "Auditor"; columns++;
                excelWorksheet.Cells[rows, columns].Value = "Contrato"; columns++;
                excelWorksheet.Cells[rows, columns].Value = "Observaciones - Contrato"; columns++;
                excelWorksheet.Cells[rows, columns].Value = "Recomendaciones - Contrato"; columns++;
                excelWorksheet.Cells[rows, columns].Value = "Contrato Abierto"; columns++;
                excelWorksheet.Cells[rows, columns].Value = "Observaciones - Contrato Abierto"; columns++;
                excelWorksheet.Cells[rows, columns].Value = "Recomendaciones-Contrato Abierto"; columns++;
                excelWorksheet.Cells[rows, columns].Value = "Convenio Modificado"; columns++;
                excelWorksheet.Cells[rows, columns].Value = "Observaciones-Convenio Modificado"; columns++;
                excelWorksheet.Cells[rows, columns].Value = "Recomendaciones-Convenio Modificado"; columns++;
                columns = 1;
                rows++;

                int contador = 1;
                
                if (result.Count > 0)
                {
                    foreach (var r in result)
                    {
                       

                        string Auditor = nombreAuditor(r.auditor);
                        int si = r.documentos.Where(x => x.estatus == "SI").Count();
                        int no = r.documentos.Where(x => x.estatus == "NO").Count();
                        int AvanceDoc = (si * 100) / (si + no);

                        string contrato = string.Empty;
                        string observacionesContrato = string.Empty;
                        string recomendacionesContrato = string.Empty;

                        string contratoAbierto = string.Empty;
                        string observcionesContratoAbierto = string.Empty;
                        string recomendacionesContratoAbierto = string.Empty;

                        string convenioModificado = string.Empty;
                        string observacionesConvenioModificado = string.Empty;
                        string recomendacionesConvenioModificado = string.Empty;

                        switch (r.tipoAdjudicacion)
                        {
                            case "INVITACIÓN A CUANDO MENOS TRES PERSONAS":
                                contrato = r.documentos.Where(x => x.clave == 19).FirstOrDefault().estatus;
                                observacionesContrato = r.documentos.Where(x => x.clave == 19).FirstOrDefault().comentario;
                                recomendacionesContrato = r.documentos.Where(x => x.clave == 19).FirstOrDefault().recomendacion;

                                contratoAbierto = r.documentos.Where(x => x.clave == 20).FirstOrDefault().estatus;
                                observcionesContratoAbierto = r.documentos.Where(x => x.clave == 20).FirstOrDefault().comentario; ;
                                recomendacionesContratoAbierto = r.documentos.Where(x => x.clave == 20).FirstOrDefault().recomendacion; ;

                                convenioModificado = r.documentos.Where(x => x.clave == 21).FirstOrDefault().estatus;
                                observacionesConvenioModificado = r.documentos.Where(x => x.clave == 21).FirstOrDefault().comentario;
                                recomendacionesConvenioModificado = r.documentos.Where(x => x.clave == 21).FirstOrDefault().recomendacion;

                                break;
                            case "CONCURSO POR INVITACIÓN":
                                contrato = r.documentos.Where(x => x.clave == 27).FirstOrDefault().estatus;
                                observacionesContrato = r.documentos.Where(x => x.clave == 27).FirstOrDefault().comentario;
                                recomendacionesContrato = r.documentos.Where(x => x.clave == 27).FirstOrDefault().recomendacion;

                                contratoAbierto = r.documentos.Where(x => x.clave == 28).FirstOrDefault().estatus;
                                observcionesContratoAbierto = r.documentos.Where(x => x.clave == 28).FirstOrDefault().comentario; ;
                                recomendacionesContratoAbierto = r.documentos.Where(x => x.clave == 28).FirstOrDefault().recomendacion; ;

                                convenioModificado = r.documentos.Where(x => x.clave == 29).FirstOrDefault().estatus;
                                observacionesConvenioModificado = r.documentos.Where(x => x.clave == 29).FirstOrDefault().comentario;
                                recomendacionesConvenioModificado = r.documentos.Where(x => x.clave == 29).FirstOrDefault().recomendacion;

                                break;
                            case "ADJUDICACIÓN DIRECTA":

                                contrato = r.documentos.Where(x => x.clave == 10).FirstOrDefault().estatus;
                                observacionesContrato = r.documentos.Where(x => x.clave == 10).FirstOrDefault().comentario;
                                recomendacionesContrato = r.documentos.Where(x => x.clave == 10).FirstOrDefault().recomendacion;


                                contratoAbierto = r.documentos.Where(x => x.clave == 11).FirstOrDefault().estatus;
                                observcionesContratoAbierto = r.documentos.Where(x => x.clave == 11).FirstOrDefault().comentario; ;
                                recomendacionesContratoAbierto = r.documentos.Where(x => x.clave == 11).FirstOrDefault().recomendacion; ;

                                convenioModificado = r.documentos.Where(x => x.clave == 12).FirstOrDefault().estatus;
                                observacionesConvenioModificado = r.documentos.Where(x => x.clave == 12).FirstOrDefault().comentario;
                                recomendacionesConvenioModificado = r.documentos.Where(x => x.clave == 12).FirstOrDefault().recomendacion;

                                break;
                            default:
                                break;
                        }

                        excelWorksheet.Cells[rows, columns].Value = contador; columns++;
                        excelWorksheet.Cells[rows, columns].Value = r.numeroAdjudicacion; columns++;
                        excelWorksheet.Cells[rows, columns].Value = r.numeroContrato; columns++;
                        excelWorksheet.Cells[rows, columns].Value = r.ejercicio; columns++;
                        excelWorksheet.Cells[rows, columns].Value = r.proveedorAdjudicado; columns++;
                        excelWorksheet.Cells[rows, columns].Value = r.montoAdjudicado; columns++;
                        excelWorksheet.Cells[rows, columns].Value = r.montoAdjudicado; columns++;
                        excelWorksheet.Cells[rows, columns].Value = r.objetoContrato; columns++;
                        excelWorksheet.Cells[rows, columns].Value = r.tipoAdjudicacion; columns++;
                        excelWorksheet.Cells[rows, columns].Value = AvanceDoc; columns++;
                        excelWorksheet.Cells[rows, columns].Value = Auditor; columns++;
                        excelWorksheet.Cells[rows, columns].Value = contrato; columns++;
                        excelWorksheet.Cells[rows, columns].Value = observacionesContrato; columns++;
                        excelWorksheet.Cells[rows, columns].Value = recomendacionesContrato; columns++;
                        excelWorksheet.Cells[rows, columns].Value = contratoAbierto; columns++;
                        excelWorksheet.Cells[rows, columns].Value = observcionesContratoAbierto; columns++;
                        excelWorksheet.Cells[rows, columns].Value = recomendacionesContratoAbierto; columns++;
                        excelWorksheet.Cells[rows, columns].Value = convenioModificado; columns++;
                        excelWorksheet.Cells[rows, columns].Value = observacionesConvenioModificado; columns++;
                        excelWorksheet.Cells[rows, columns].Value = recomendacionesConvenioModificado; columns++;

                        columns = 1;
                        rows++;
                        contador++;
                    }

                    excelPackage.SaveAs(ms);

                    FileInfo fi = new FileInfo(path);
                    excelPackage.SaveAs(fi);
                }
                ms.Position = 0;
            }
        }

        private void ReporteGlobalObra(string municipio, string path)
        {
            string constr = ConfigurationManager.AppSettings["connectionString"];
            var Client = new MongoClient(constr);
            var DB = Client.GetDatabase("PRB");
            var collection = DB.GetCollection<ObraPublicaV1>("ObraPublica");

            var filter = Builders<ObraPublicaV1>.Filter.Eq(x => x.municipio, municipio);

            var result = collection.Find(filter).ToList();

            MemoryStream memoryStream = new MemoryStream();
            WebClient client = new WebClient();

            try
            {
                //memoryStream = new MemoryStream(client.DownloadData("C:/Simopa/Templete/TemplateItinerarios.xlsx"));
                memoryStream = new MemoryStream(client.DownloadData("https://apipbrdevelolop.azurewebsites.net/templates/reportes/TempPlantillaGeneral.xlsx"));
            }
            finally
            {
                client.Dispose();
            }

            int rows = 1;
            int columns = 1;
            MemoryStream ms = new MemoryStream();
            

            using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {
                ExcelWorkbook excelWorkBook = excelPackage.Workbook;
                ExcelWorksheet excelWorksheet = excelWorkBook.Worksheets[1];

                excelWorksheet.Cells[rows, columns].Value = "*"; columns++;
                excelWorksheet.Cells[rows, columns].Value = "No Procedimiento"; columns++;
                excelWorksheet.Cells[rows, columns].Value = "No Obra"; columns++;
                excelWorksheet.Cells[rows, columns].Value = "Ejercicio"; columns++;
                excelWorksheet.Cells[rows, columns].Value = "Proveedor"; columns++;
                excelWorksheet.Cells[rows, columns].Value = "Monto Contrato"; columns++;
                excelWorksheet.Cells[rows, columns].Value = "Monto Ejercido"; columns++;
                excelWorksheet.Cells[rows, columns].Value = "Concepto"; columns++;
                excelWorksheet.Cells[rows, columns].Value = "Tipo de Procedimiento"; columns++;
                excelWorksheet.Cells[rows, columns].Value = "Avance Documental"; columns++;
                excelWorksheet.Cells[rows, columns].Value = "Auditor"; columns++;
                excelWorksheet.Cells[rows, columns].Value = "Contrato"; columns++;
                excelWorksheet.Cells[rows, columns].Value = "Observaciones-Contrato"; columns++;
                excelWorksheet.Cells[rows, columns].Value = "Recomendaciones-Contrato"; columns++;

                columns = 1;
                rows++;
                int contador = 1;

                if (result != null)
                {
                    foreach (var r in result)
                    {
                        string Auditor = nombreAuditor(r.auditor);
                        int si = r.documentos.Where(x => x.integracion == "SI").Count();
                        int no = r.documentos.Where(x => x.integracion == "NO").Count();
                        int AvanceDoc = (si * 100) / (si + no);

                        string contrato = string.Empty;
                        string observacionesContrato = string.Empty;
                        string recomendacionesContrato = string.Empty;

                        string contratoAbierto = string.Empty;
                        string observcionesContratoAbierto = string.Empty;
                        string recomendacionesContratoAbierto = string.Empty;

                        string convenioModificado = string.Empty;
                        string observacionesConvenioModificado = string.Empty;
                        string recomendacionesConvenioModificado = string.Empty;

                        contrato = r.documentos.Where(x => x.clave == 53).FirstOrDefault().integracion;
                        observacionesContrato = r.documentos.Where(x => x.clave == 53).FirstOrDefault().observaciones;
                        recomendacionesContrato = r.documentos.Where(x => x.clave == 53).FirstOrDefault().recomendacion;

                        excelWorksheet.Cells[rows, columns].Value = contador; columns++;
                        excelWorksheet.Cells[rows, columns].Value = r.numeroProcedimiento; columns++;
                        excelWorksheet.Cells[rows, columns].Value = r.numeroObra; columns++;
                        excelWorksheet.Cells[rows, columns].Value = r.ejercicio; columns++;
                        excelWorksheet.Cells[rows, columns].Value = r.proveedor; columns++;
                        excelWorksheet.Cells[rows, columns].Value = r.montoContrato; columns++;
                        excelWorksheet.Cells[rows, columns].Value = r.montoEjercido; columns++;
                        excelWorksheet.Cells[rows, columns].Value = r.nombreObra; columns++;
                        excelWorksheet.Cells[rows, columns].Value = r.tipoAdjudicacion; columns++;
                        excelWorksheet.Cells[rows, columns].Value = AvanceDoc; columns++;
                        excelWorksheet.Cells[rows, columns].Value = Auditor; columns++;
                        excelWorksheet.Cells[rows, columns].Value = contrato; columns++;
                        excelWorksheet.Cells[rows, columns].Value = observacionesContrato; columns++;

                        columns = 1;
                        rows++;

                        contador++;
                    }
                    excelPackage.SaveAs(ms);

                    FileInfo fi = new FileInfo(path);
                    excelPackage.SaveAs(fi);
                }
                ms.Position = 0;
            }
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("api/report/expediente/{municipio}/{obra}/{tipo}/{id}")]
        public async System.Threading.Tasks.Task<HttpResponseMessage> ReporteExpediente(string municipio,string obra,string tipo, string id)
        {

            var fileName = string.Empty;
            var tempOutput = string.Empty;

            List<string> filesCsv = new List<string>();

            if (municipio.Equals("TEPEACA"))
            {
                if (obra.Equals("OBRA"))
                {
                    fileName = "TepTemplateObra.xlsx";
                }
                else
                {
                    if (tipo.Equals("ADJUDICACIÓN DIRECTA"))
                    {
                        fileName = "TepTemplateAdjDirecta.xlsx";
                    }
                    else if (tipo.Equals("CONCURSO POR INVITACIÓN"))
                    {
                        fileName = "TepTemplateConInvitacion.xlsx";
                    }
                    else if (tipo.Equals("INVITACIÓN A CUANDO MENOS TRES PERSONAS"))
                    {
                        fileName = "TepTemplateInvTres.xlsx";
                    }
                    else if (tipo.Equals("LICITACIÓN PÚBLICA"))
                    {
                        fileName = "TepTemplateLicPublica.xlsx";
                    }
                    else if (tipo.Equals("REVISIÓN DE LAS COMPRAS URGENTES Y ESPECIALES -33 MIL"))
                    {
                        fileName = "TepTemplateUrgEspecial33.xlsx";
                    }
                }              
            }
            else if (municipio.Equals("SAN ANDRES CHOLULA"))
            {
                if (obra.Equals("OBRA"))
                {
                    fileName = "SachTemplateObra.xlsx";
                }
                else
                {
                    if (tipo.Equals("ADJUDICACIÓN DIRECTA"))
                    {
                        fileName = "SachTemplateAdjDirecta.xlsx";
                    }
                    else if (tipo.Equals("CONCURSO POR INVITACIÓN"))
                    {
                        fileName = "SachTemplateConInvitacion.xlsx";
                    }
                    else if (tipo.Equals("INVITACIÓN A CUANDO MENOS TRES PERSONAS"))
                    {
                        fileName = "SachTemplateInvTres.xlsx";
                    }
                    else if (tipo.Equals("LICITACIÓN PÚBLICA"))
                    {
                        fileName = "SachTemplateLicPublica.xlsx";
                    }
                    else if (tipo.Equals("REVISIÓN DE LAS COMPRAS URGENTES Y ESPECIALES -33 MIL"))
                    {
                        fileName = "SachTemplateUrgEspecial33.xlsx";
                    }
                    else if (tipo.Equals("OBRA"))
                    {
                        fileName = "SachTemplateObra.xlsx";
                    }
                }
                    
            }
            
            tempOutput = path + fileName;

            if (municipio.Equals("TEPEACA"))
            {                
                if (obra.Equals("OBRA"))
                {
                    ReporteObraTepeaca(id, path, fileName);
                }
                else //if (obra.Equals("Adquisiciones"))
                {
                    ReporteAdquisicionesTepeaca(tipo, id, path, fileName);
                }
            }
            else if (municipio.Equals("SAN ANDRES CHOLULA"))
            {
               
                if (obra.Equals("OBRA"))
                {
                    ReporteObraSach(id, path, fileName);
                }
                else//if (obra.Equals("Adquisiciones"))
                {
                    ReporteAdquisicionesSach(tipo, id, path, fileName);
                }
            }

            filesCsv.Add(tempOutput);
            
            #region Archivos en Memmoria

            //Archivo zip a MemoryStream
            MemoryStream memoryZip = new MemoryStream();
            using (FileStream file = new FileStream(tempOutput, FileMode.Open, FileAccess.Read))
            {
                byte[] bytes = new byte[file.Length];
                file.Read(bytes, 0, (int)file.Length);
                memoryZip.Write(bytes, 0, (int)file.Length);
            }

            //Eliminando Archivos de Excel Generados
            if (File.Exists(tempOutput))
            {
                foreach (var archivos in filesCsv)
                {
                    File.Delete(archivos);
                }
            }

            #endregion

            var reporte = new HttpResponseMessage(HttpStatusCode.OK);
            reporte.Content = new ByteArrayContent(memoryZip.ToArray());
            reporte.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            reporte.Content.Headers.ContentDisposition.FileName = "ReporteExpediente_" + id +".xlsx";
            reporte.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            return reporte;
        }

        #region Generacion de Expediente Tepeaca

        private void ReporteAdquisicionesTepeaca(string tipo, string id, string path, string name)
        {
            string constr = ConfigurationManager.AppSettings["connectionString"];
            var Client = new MongoClient(constr);
            var DB = Client.GetDatabase("PRB");
            var collection = DB.GetCollection<AdquisicionesV1>("Adquisiciones");
            var filter = Builders<AdquisicionesV1>.Filter.Eq(x => x.Id, id);
            var result = collection.Find(filter).FirstOrDefault();

            MemoryStream memoryStream = new MemoryStream();
            WebClient client = new WebClient();

            try
            {
                //memoryStream = new MemoryStream(client.DownloadData("C:/Simopa/Templete/TemplateItinerarios.xlsx"));
                memoryStream = new MemoryStream(client.DownloadData("https://apipbrdevelolop.azurewebsites.net/templates/reportes/" + name));
            }
            finally
            {
                client.Dispose();
            }

            int rows = 1;
            int columns = 1;
            MemoryStream ms = new MemoryStream();

            using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {
                ExcelWorkbook excelWorkBook = excelPackage.Workbook;
                ExcelWorksheet excelWorksheet = excelWorkBook.Worksheets[1];

                if (result != null)
                {
                    //Encabezado
                    excelWorksheet.Cells[3, 3].Value = result.numeroAdjudicacion;
                    excelWorksheet.Cells[3, 6].Value = result.numeroContrato; 
                    excelWorksheet.Cells[3, 9].Value = result.ejercicio;
                    excelWorksheet.Cells[3, 10].Value = result.entidad;

                    excelWorksheet.Cells[5, 3].Value = result.objetoContrato; 
                    excelWorksheet.Cells[5, 6].Value = result.proveedorAdjudicado; 
                    excelWorksheet.Cells[5, 15].Value = result.estado;

                    excelWorksheet.Cells[6, 4].Value = result.montoAdjudicado;
                    excelWorksheet.Cells[6, 6].Value = result.origenRecurso;
                    //excelWorksheet.Cells[6, 8].Value = result.  //Partida Presupuestal
                    excelWorksheet.Cells[6, 11].Value = string.IsNullOrEmpty(result.fechaRevision) ? "" : result.fechaRevision;
                    excelWorksheet.Cells[6, 15].Value = result.municipio;

                    #region Observaciones Generales

                    if (tipo.Equals("ADJUDICACIÓN DIRECTA"))
                    {
                        excelWorksheet.Cells[52, 3].Value = string.IsNullOrEmpty(result.observacionesGenerales) ? "" : result.observacionesGenerales;
                    }
                    else if (tipo.Equals("CONCURSO POR INVITACIÓN"))
                    {
                        excelWorksheet.Cells[58, 3].Value = string.IsNullOrEmpty(result.observacionesGenerales) ? "" : result.observacionesGenerales;
                    }
                    else if (tipo.Equals("INVITACIÓN A CUANDO MENOS TRES PERSONAS"))
                    {
                        excelWorksheet.Cells[60, 3].Value = string.IsNullOrEmpty(result.observacionesGenerales) ? "" : result.observacionesGenerales;
                    }
                    else if (tipo.Equals("LICITACIÓN PÚBLICA"))
                    {
                        excelWorksheet.Cells[51, 3].Value = string.IsNullOrEmpty(result.observacionesGenerales) ? "" : result.observacionesGenerales;
                    }
                    else if (tipo.Equals("REVISIÓN DE LAS COMPRAS URGENTES Y ESPECIALES -33 MIL"))
                    {
                        excelWorksheet.Cells[20, 3].Value = string.IsNullOrEmpty(result.observacionesGenerales) ? "" : result.observacionesGenerales;
                    }
                    //else if (tipo.Equals("OBRA"))
                    //{
                    //    excelWorksheet.Cells[52, 3].Value = string.IsNullOrEmpty(result.observacionesGenerales) ? "" : result.observacionesGenerales;
                    //}
                    #endregion

                    //Documentos
                    rows = 10;
                    columns = 9;
                    List<DocumentoAdquisicionesV1> listDoc = new List<DocumentoAdquisicionesV1>();
                    listDoc = result.documentos;
                    Sort(ref listDoc, "clave", "ASC");

                    foreach (var document in listDoc)
                    {

                        if (tipo.Equals("ADJUDICACIÓN DIRECTA"))
                        {
                            #region Documentos
                            if (document.clave >= 1 & document.clave <= 5)
                            {
                                excelWorksheet.Cells[rows, columns].Value = document.estatus; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = getObservaciones(result.Id, document.clave); columns++; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia; columns++;
                                //excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.grupo) ? "" : document.grupo; columns++;

                                if (document.clave == 5)
                                {
                                    rows++; rows++;
                                }
                                else
                                {
                                    rows++;
                                }
                                columns = 9;
                            }
                            else if (document.clave >= 6 & document.clave <= 28)
                            {
                                excelWorksheet.Cells[rows, columns].Value = document.estatus; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = getObservaciones(result.Id, document.clave); columns++; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia; columns++;
                                //excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.grupo) ? "" : document.grupo; columns++;
                                if (document.clave == 28)
                                {
                                    rows++; rows++;
                                }
                                else
                                {
                                    rows++;
                                }
                                columns = 9;
                            }
                            else if (document.clave >= 29 & document.clave <= 30)
                            {
                                excelWorksheet.Cells[rows, columns].Value = document.estatus; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = getObservaciones(result.Id, document.clave); columns++; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia; columns++;
                                //excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.grupo) ? "" : document.grupo; columns++;
                                if (document.clave == 30)
                                {
                                    rows++; rows++;
                                }
                                else
                                {
                                    rows++;
                                }
                                columns = 9;
                            }
                            else if (document.clave >= 31 & document.clave <= 38)
                            {
                                excelWorksheet.Cells[rows, columns].Value = document.estatus; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = getObservaciones(result.Id, document.clave); columns++; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia; columns++;
                                //excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.grupo) ? "" : document.grupo; columns++;
                                if (document.clave == 38)
                                {
                                    rows++; rows++;
                                }
                                else
                                {
                                    rows++;
                                }
                                columns = 9;
                            }
                            #endregion
                        }
                        else if (tipo.Equals("CONCURSO POR INVITACIÓN"))
                        {
                            #region Documentos
                            if (document.clave >= 1 & document.clave <= 5)
                            {
                                excelWorksheet.Cells[rows, columns].Value = document.estatus; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = getObservaciones(result.Id, document.clave); columns++; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia; columns++;
                                //excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.grupo) ? "" : document.grupo; columns++;

                                if (document.clave == 5)
                                {
                                    rows++; rows++;
                                }
                                else
                                {
                                    rows++;
                                }
                                columns = 9;
                            }
                            else if (document.clave >= 6 & document.clave <= 26)
                            {
                                excelWorksheet.Cells[rows, columns].Value = document.estatus; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = getObservaciones(result.Id, document.clave); columns++; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia; columns++;
                                //excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.grupo) ? "" : document.grupo; columns++;
                                if (document.clave == 26)
                                {
                                    rows++; rows++;
                                }
                                else
                                {
                                    rows++;
                                }
                                columns = 9;
                            }
                            else if (document.clave >= 27 & document.clave <= 32)
                            {
                                excelWorksheet.Cells[rows, columns].Value = document.estatus; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = getObservaciones(result.Id, document.clave); columns++; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia; columns++;
                                //excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.grupo) ? "" : document.grupo; columns++;
                                if (document.clave == 32)
                                {
                                    rows++; rows++;
                                }
                                else
                                {
                                    rows++;
                                }
                                columns = 9;
                            }
                            else if (document.clave >= 33 & document.clave <= 35)
                            {
                                excelWorksheet.Cells[rows, columns].Value = document.estatus; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = getObservaciones(result.Id, document.clave); columns++; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia; columns++;
                                //excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.grupo) ? "" : document.grupo; columns++;
                                if (document.clave == 35)
                                {
                                    rows++; rows++;
                                }
                                else
                                {
                                    rows++;
                                }
                                columns = 9;
                            }
                            else if (document.clave >= 36 & document.clave <= 43)
                            {
                                excelWorksheet.Cells[rows, columns].Value = document.estatus; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = getObservaciones(result.Id, document.clave); columns++; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia; columns++;
                                //excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.grupo) ? "" : document.grupo; columns++;
                                if (document.clave == 35)
                                {
                                    rows++; rows++;
                                }
                                else
                                {
                                    rows++;
                                }
                                columns = 9;
                            }
                            #endregion
                        }
                        else if (tipo.Equals("INVITACIÓN A CUANDO MENOS TRES PERSONAS"))
                        {
                            #region Documentos
                            if (document.clave >= 1 & document.clave <= 5)
                            {
                                excelWorksheet.Cells[rows, columns].Value = document.estatus; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = getObservaciones(result.Id, document.clave); columns++; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia; columns++;
                                //excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.grupo) ? "" : document.grupo; columns++;

                                if (document.clave == 5)
                                {
                                    rows++; rows++;
                                }
                                else
                                {
                                    rows++;
                                }
                                columns = 9;
                            }
                            else if (document.clave >= 6 & document.clave <= 18)
                            {
                                excelWorksheet.Cells[rows, columns].Value = document.estatus; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = getObservaciones(result.Id, document.clave); columns++; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia; columns++;
                                //excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.grupo) ? "" : document.grupo; columns++;
                                if (document.clave == 18)
                                {
                                    rows++; rows++;
                                }
                                else
                                {
                                    rows++;
                                }
                                columns = 9;
                            }
                            else if (document.clave >= 19 & document.clave <= 34)
                            {
                                excelWorksheet.Cells[rows, columns].Value = document.estatus; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = getObservaciones(result.Id, document.clave); columns++; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia; columns++;
                                //excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.grupo) ? "" : document.grupo; columns++;
                                if (document.clave == 34)
                                {
                                    rows++; rows++;
                                }
                                else
                                {
                                    rows++;
                                }
                                columns = 9;
                            }
                            else if (document.clave >= 35 & document.clave <= 37)
                            {
                                excelWorksheet.Cells[rows, columns].Value = document.estatus; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = getObservaciones(result.Id, document.clave); columns++; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia; columns++;
                                //excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.grupo) ? "" : document.grupo; columns++;
                                if (document.clave == 37)
                                {
                                    rows++; rows++;
                                }
                                else
                                {
                                    rows++;
                                }
                                columns = 9;
                            }
                            else if (document.clave >= 38 & document.clave <= 45)
                            {
                                excelWorksheet.Cells[rows, columns].Value = document.estatus; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = getObservaciones(result.Id, document.clave); columns++; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia; columns++;
                                //excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.grupo) ? "" : document.grupo; columns++;
                                if (document.clave == 45)
                                {
                                    rows++; rows++;
                                }
                                else
                                {
                                    rows++;
                                }
                                columns = 9;
                            }
                            #endregion
                        }
                        else if (tipo.Equals("LICITACIÓN PÚBLICA"))
                        {
                            #region Documentos
                            if (document.clave >= 1 & document.clave <= 6)
                            {
                                excelWorksheet.Cells[rows, columns].Value = document.estatus; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = getObservaciones(result.Id, document.clave); columns++; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia; columns++;
                                //excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.grupo) ? "" : document.grupo; columns++;

                                if (document.clave == 6)
                                {
                                    rows++; rows++;
                                }
                                else
                                {
                                    rows++;
                                }
                                columns = 9;
                            }
                            else if (document.clave >= 7 & document.clave <= 25)
                            {
                                excelWorksheet.Cells[rows, columns].Value = document.estatus; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = getObservaciones(result.Id, document.clave); columns++; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia; columns++;
                                //excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.grupo) ? "" : document.grupo; columns++;
                                if (document.clave == 25)
                                {
                                    rows++; rows++;
                                }
                                else
                                {
                                    rows++;
                                }
                                columns = 9;
                            }
                            else if (document.clave >= 26 & document.clave <= 29)
                            {
                                excelWorksheet.Cells[rows, columns].Value = document.estatus; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = getObservaciones(result.Id, document.clave); columns++; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia; columns++;
                                //excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.grupo) ? "" : document.grupo; columns++;
                                if (document.clave == 29)
                                {
                                    rows++; rows++;
                                }
                                else
                                {
                                    rows++;
                                }
                                columns = 9;
                            }
                            else if (document.clave >= 30 & document.clave <= 31)
                            {
                                excelWorksheet.Cells[rows, columns].Value = document.estatus; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = getObservaciones(result.Id, document.clave); columns++; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia; columns++;
                                //excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.grupo) ? "" : document.grupo; columns++;
                                if (document.clave == 31)
                                {
                                    rows++; rows++;
                                }
                                else
                                {
                                    rows++;
                                }
                                columns = 9;
                            }
                            else if (document.clave >= 32 & document.clave <= 36)
                            {
                                excelWorksheet.Cells[rows, columns].Value = document.estatus; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = getObservaciones(result.Id, document.clave); columns++; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia; columns++;
                                //excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.grupo) ? "" : document.grupo; columns++;
                                if (document.clave == 36)
                                {
                                    rows++; rows++;
                                }
                                else
                                {
                                    rows++;
                                }
                                columns = 9;
                            }
                            #endregion
                        }
                        else if (tipo.Equals("REVISIÓN DE LAS COMPRAS URGENTES Y ESPECIALES -33 MIL"))
                        {
                            #region Documentos
                            if (document.clave >= 1 & document.clave <= 3)
                            {
                                excelWorksheet.Cells[rows, columns].Value = document.estatus; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = getObservaciones(result.Id, document.clave); columns++; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia; columns++;
                                //excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.grupo) ? "" : document.grupo; columns++;

                                if (document.clave == 3)
                                {
                                    rows++; rows++;
                                }
                                else
                                {
                                    rows++;
                                }
                                columns = 9;
                            }

                            if (document.clave >= 4 & document.clave <= 8)
                            {
                                excelWorksheet.Cells[rows, columns].Value = document.estatus; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = getObservaciones(result.Id, document.clave); columns++; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia; columns++;
                                //excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.grupo) ? "" : document.grupo; columns++;
                                if (document.clave == 8)
                                {
                                    rows++; rows++;
                                }
                                else
                                {
                                    rows++;
                                }
                                columns = 9;
                            }
                            #endregion
                        }
                        else if (tipo.Equals("OBRA"))
                        {
                        }

                        
                    }

                    excelPackage.SaveAs(ms);

                    FileInfo fi = new FileInfo(path + name);
                    excelPackage.SaveAs(fi);
                }
                ms.Position = 0;
            }
        }

        private string getObservaciones(string idExpediente, int clave)
        {
            StringBuilder observaciones = new StringBuilder();
            string constr = ConfigurationManager.AppSettings["connectionString"];
            var Client = new MongoClient(constr);
            var DB = Client.GetDatabase("PRB");

            var collectionUser = DB.GetCollection<Observaciones>("Observaciones");
            var filter = Builders<Observaciones>.Filter.Eq(x => x.idExpediente, idExpediente)
                & Builders<Observaciones>.Filter.Eq(x => x.Clave, clave);

            var result = collectionUser.Find(filter).ToList();

            if (result.Count > 0)
            {
                foreach(var data in result)
                {
                    observaciones.Append(data.Observacion).Append("\n");
                }
            }

            return observaciones.ToString();
        }

        private void ReporteObraTepeaca(string id, string path, string name)
        {
            string constr = ConfigurationManager.AppSettings["connectionString"];
            var Client = new MongoClient(constr);
            var DB = Client.GetDatabase("PRB");
            var collection = DB.GetCollection<ObraPublicaV1>("ObraPublica");

            var filter = Builders<ObraPublicaV1>.Filter.Eq(x => x.Id, id);
            var result = collection.Find(filter).FirstOrDefault();

            MemoryStream memoryStream = new MemoryStream();
            WebClient client = new WebClient();

            try
            {
                //memoryStream = new MemoryStream(client.DownloadData("C:/Simopa/Templete/TemplateItinerarios.xlsx"));
                memoryStream = new MemoryStream(client.DownloadData("https://apipbrdevelolop.azurewebsites.net/templates/reportes/" +  name));
            }
            finally
            {
                client.Dispose();
            }

            int rows = 1;
            int columns = 1;
            MemoryStream ms = new MemoryStream();

            using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {
                ExcelWorkbook excelWorkBook = excelPackage.Workbook;
                ExcelWorksheet excelWorksheet = excelWorkBook.Worksheets[1];

                if (result != null)
                {
                    excelWorksheet.Cells[3, 11].Value = result.municipio;
                    //fecha

                    excelWorksheet.Cells[4, 11].Value = result.programa;
                    excelWorksheet.Cells[4, 11].Value = result.ejercicio;

                    excelWorksheet.Cells[5, 11].Value = result.ejecutor;
                    excelWorksheet.Cells[5, 62].Value = result.estado;
                    excelWorksheet.Cells[6, 62].Value = result.municipio;


                    excelWorksheet.Cells[7, 10].Value = result.fechaRevision;
                    excelWorksheet.Cells[8, 10].Value = result.localidad;

                    excelWorksheet.Cells[9, 10].Value = result.nombreObra;
                    excelWorksheet.Cells[10, 10].Value = result.proyecto;
                    excelWorksheet.Cells[10, 59].Value = result.numeroObra;
                    
                    excelWorksheet.Cells[11, 10].Value = result.programa;
                    //excelWorksheet.Cells[11, 10].Value = result.;  //Normativa
                   
                    //Modalidad
                    if (result.modalidad.Equals("CONTRATO"))
                    {
                        excelWorksheet.Cells[15, 27].Value = "X";
                    }
                    else if (result.modalidad.Equals("MIXTO"))
                    {
                        excelWorksheet.Cells[15, 39].Value = "X";
                    }
                    else 
                    {
                        excelWorksheet.Cells[15, 15].Value = "X";
                    }

                    //Tipo de Adjudicacion
                    if (result.tipoAdjudicacion.Equals("ADJUDICACIÓN DIRECTA"))
                    {
                        excelWorksheet.Cells[16, 15].Value = "X";
                    }
                    else if (result.tipoAdjudicacion.Equals("LICITACIÓN PÚBLICA"))
                    {
                        excelWorksheet.Cells[16, 47].Value = "X";
                    }
                    else if (result.tipoAdjudicacion.Equals("INVITACIÓN RESTRINGIDA A 5 PROVEEDORES"))
                    {
                        excelWorksheet.Cells[16, 39].Value = "X";
                    }
                    else if (result.tipoAdjudicacion.Equals("INVITACIÓN RESTRINGIDA A 3 PROVEEDORES"))
                    {
                        excelWorksheet.Cells[16, 27].Value = "X";
                    }

                    excelWorksheet.Cells[16, 54].Value = result.numeroProcedimiento;

                    //excelWorksheet.Cells[17, 54].Value = result.numeroProcedimiento;
                    //Tipo de Contrato
                    if (result.tipoContrato.Equals("DE OBRA"))
                    {
                        excelWorksheet.Cells[17, 15].Value = "X";
                    }
                    else if (result.tipoContrato.Equals("OTRO"))
                    {
                        excelWorksheet.Cells[17, 47].Value = "X";
                    }
                    else if (result.tipoContrato.Contains("RENTA"))
                    {
                        excelWorksheet.Cells[17, 39].Value = "X";
                    }
                    else 
                    {
                        excelWorksheet.Cells[17, 27].Value = "X";
                    }

                    excelWorksheet.Cells[17, 54].Value = result.fechaProcedimiento;

                    excelWorksheet.Cells[18, 10].Value = result.proveedor;
                    excelWorksheet.Cells[19, 10].Value = result.numeroContrato;

                    //Coordenadas
                    excelWorksheet.Cells[21, 56].Value = result.Location.Coordinates[1];//Latitud
                    excelWorksheet.Cells[21, 60].Value = result.Location.Coordinates[0];//Longitud

                    excelWorksheet.Cells[22, 10].Value = result.montoAsignado;
                    excelWorksheet.Cells[22, 31].Value = result.montoContrato;
                    excelWorksheet.Cells[22, 54].Value = result.montoEjercido;


                    List<DocumentosObrasV1> listDoc = new List<DocumentosObrasV1>();
                    listDoc = result.documentos;
                    Sort(ref listDoc, "clave", "ASC");

                    rows = 36;
                    columns = 38;

                    foreach (var document in listDoc)
                    {
                        #region Documentos
                        if (document.clave >= 1 & document.clave <= 10)
                        {
                            
                            excelWorksheet.Cells[rows, columns].Value = document.integracion; 
                            columns = 57;
                            excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia;
                            if (document.clave == 3)
                            {
                                rows++; rows++; rows++; 
                            }
                            else if (document.clave == 4)
                            {
                                rows++; rows++; rows++;
                            }
                            else if (document.clave == 10)
                            {
                                rows++; rows++; rows++; rows++;
                            }
                            else
                            {
                                rows++;
                            }
                            columns = 38;

                        }
                        else if (document.clave >= 11 & document.clave <= 19)
                        {
                            excelWorksheet.Cells[rows, columns].Value = document.integracion;
                            columns = 57;
                            excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia;
                            if (document.clave == 19)
                            {
                                rows++; rows++;
                            }
                            else
                            {
                                rows++;
                            }
                            columns = 38;
                        }
                        else if (document.clave >= 20 & document.clave <= 26)
                        {
                            excelWorksheet.Cells[rows, columns].Value = document.integracion;
                            columns = 57;
                            excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia;
                            if (document.clave == 26)
                            {
                                rows++; rows++; rows++; rows++;
                            }
                            else
                            {
                                rows++;
                            }
                            columns = 38;
                        }
                        else if (document.clave >= 27 & document.clave <= 32)
                        {
                            excelWorksheet.Cells[rows, columns].Value = document.integracion;
                            columns = 57;
                            excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia;
                            if (document.clave == 32)
                            {
                                rows++; rows++; rows++; rows++;
                            }
                            else
                            {
                                rows++;
                            }
                            columns = 38;
                        }
                        else if (document.clave >= 33 & document.clave <= 41)
                        {
                            excelWorksheet.Cells[rows, columns].Value = document.integracion;
                            columns = 57;
                            excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia;
                            if (document.clave == 41)
                            {
                                rows++; rows++;
                            }
                            else
                            {
                                rows++;
                            }
                            columns = 38;
                        }
                        else if (document.clave >= 42 & document.clave <= 43)
                        {
                            excelWorksheet.Cells[rows, columns].Value = document.integracion;
                            columns = 57;
                            excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia;
                            if (document.clave == 43)
                            {
                                rows++; rows++;
                            }
                            else
                            {
                                rows++;
                            }
                            columns = 38;
                        }
                        else if (document.clave == 44)
                        {
                            excelWorksheet.Cells[rows, columns].Value = document.integracion;
                            columns = 57;
                            excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia;
                            if (document.clave == 44)
                            {
                                rows++; rows++; rows++; rows++; rows++; 
                            }
                            else
                            {
                                rows++;
                            }
                            columns = 38;
                        }
                        else if (document.clave >= 45 & document.clave <= 57)
                        {
                            excelWorksheet.Cells[rows, columns].Value = document.integracion;
                            columns = 57;
                            excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia;
                            if (document.clave == 53)
                            {
                                rows++; rows++; rows++; rows++;
                            }
                            else if (document.clave == 56)
                            {
                                rows++; rows++;
                            }
                            else if (document.clave == 57)
                            {
                                rows++; rows++; rows++;
                            }
                            else
                            {
                                rows++;
                            }
                            columns = 38;
                        }
                        else if (document.clave >= 58 & document.clave <= 62)
                        {
                            excelWorksheet.Cells[rows, columns].Value = document.integracion;
                            columns = 57;
                            excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia;
                            if (document.clave == 61)
                            {
                                rows++; rows++;
                            }
                            else if (document.clave == 62)
                            {
                                rows++; rows++;
                            }
                            else
                            {
                                rows++;
                            }
                            columns = 38;
                        }
                        else if (document.clave >= 63 & document.clave <= 72)
                        {
                            excelWorksheet.Cells[rows, columns].Value = document.integracion;
                            columns = 57;
                            excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia;
                            if (document.clave == 72)
                            {
                                rows++; rows++;
                            }
                            else
                            {
                                rows++;
                            }
                            columns = 38;
                        }
                        else if (document.clave >= 73 & document.clave <= 75)
                        {
                            excelWorksheet.Cells[rows, columns].Value = document.integracion;
                            columns = 57;
                            excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia;
                            if (document.clave == 75)
                            {
                                rows++; rows++;
                            }
                            else
                            {
                                rows++;
                            }
                            columns = 38;
                        }
                        else if (document.clave >= 76 & document.clave <= 84)
                        {
                            excelWorksheet.Cells[rows, columns].Value = document.integracion;
                            columns = 57;
                            excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia;
                            if (document.clave == 76)
                            {
                                rows++; rows++; rows++;
                            }
                            else if (document.clave == 84)
                            {
                                rows++; rows++;
                            }
                            else
                            {
                                rows++;
                            }
                            columns = 38;
                        }
                        else if (document.clave >= 85 & document.clave <= 90)
                        {
                            excelWorksheet.Cells[rows, columns].Value = document.integracion;
                            columns = 57;
                            excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia;
                            if (document.clave == 90)
                            {
                                rows++; rows++;
                            }
                            else
                            {
                                rows++;
                            }
                            columns = 38;
                        }
                        else if (document.clave >= 91 & document.clave <= 96)
                        {
                            excelWorksheet.Cells[rows, columns].Value = document.integracion;
                            columns = 57;
                            excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia;
                            if (document.clave == 96)
                            {
                                rows++; rows++;
                            }
                            else
                            {
                                rows++;
                            }
                            columns = 38;
                        }
                        else if (document.clave >= 97 & document.clave <= 98)
                        {
                            excelWorksheet.Cells[rows, columns].Value = document.integracion;
                            columns = 57;
                            excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia;
                            if (document.clave == 98)
                            {
                                rows++; rows++;
                            }
                            else
                            {
                                rows++;
                            }
                            columns = 38;
                        }

                        #endregion

                    }

                    excelPackage.SaveAs(ms);

                    FileInfo fi = new FileInfo(path+name);
                    excelPackage.SaveAs(fi);
                }
                ms.Position = 0;
            }
        }

        #endregion

        #region Generacion de Expediente San Andres Cholula

        private void ReporteAdquisicionesSach(string tipo, string id, string path, string name)
        {
            string constr = ConfigurationManager.AppSettings["connectionString"];
            var Client = new MongoClient(constr);
            var DB = Client.GetDatabase("PRB");
            var collection = DB.GetCollection<AdquisicionesV1>("Adquisiciones");
            var filter = Builders<AdquisicionesV1>.Filter.Eq(x => x.Id, id);
            var result = collection.Find(filter).FirstOrDefault();

            MemoryStream memoryStream = new MemoryStream();
            WebClient client = new WebClient();

            try
            {
                //memoryStream = new MemoryStream(client.DownloadData("C:/Simopa/Templete/TemplateItinerarios.xlsx"));
                memoryStream = new MemoryStream(client.DownloadData("https://apipbrdevelolop.azurewebsites.net/templates/reportes/" + name));
            }
            finally
            {
                client.Dispose();
            }

            int rows = 1;
            int columns = 1;
            MemoryStream ms = new MemoryStream();

            using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {
                ExcelWorkbook excelWorkBook = excelPackage.Workbook;
                ExcelWorksheet excelWorksheet = excelWorkBook.Worksheets[1];

                if (result != null)
                {
                    //Encabezado
                    excelWorksheet.Cells[3, 3].Value = result.numeroAdjudicacion;
                    excelWorksheet.Cells[3, 6].Value = result.numeroContrato;
                    excelWorksheet.Cells[3, 9].Value = result.ejercicio;
                    excelWorksheet.Cells[3, 10].Value = result.entidad;

                    excelWorksheet.Cells[5, 3].Value = result.objetoContrato;
                    excelWorksheet.Cells[5, 6].Value = result.proveedorAdjudicado;
                    excelWorksheet.Cells[5, 15].Value = result.estado;

                    excelWorksheet.Cells[6, 4].Value = result.montoAdjudicado;
                    excelWorksheet.Cells[6, 6].Value = result.origenRecurso;
                    //excelWorksheet.Cells[6, 8].Value = result.  //Partida Presupuestal
                    excelWorksheet.Cells[6, 11].Value = string.IsNullOrEmpty(result.fechaRevision) ? "" : result.fechaRevision;
                    excelWorksheet.Cells[6, 15].Value = result.municipio;

                    #region Observaciones Generales

                    if (tipo.Equals("ADJUDICACIÓN DIRECTA"))
                    {
                        excelWorksheet.Cells[52, 3].Value = string.IsNullOrEmpty(result.observacionesGenerales) ? "" : result.observacionesGenerales;
                    }
                    else if (tipo.Equals("CONCURSO POR INVITACIÓN"))
                    {
                        excelWorksheet.Cells[58, 3].Value = string.IsNullOrEmpty(result.observacionesGenerales) ? "" : result.observacionesGenerales;
                    }
                    else if (tipo.Equals("INVITACIÓN A CUANDO MENOS TRES PERSONAS"))
                    {
                        excelWorksheet.Cells[60, 3].Value = string.IsNullOrEmpty(result.observacionesGenerales) ? "" : result.observacionesGenerales;
                    }
                    else if (tipo.Equals("LICITACIÓN PÚBLICA"))
                    {
                        excelWorksheet.Cells[51, 3].Value = string.IsNullOrEmpty(result.observacionesGenerales) ? "" : result.observacionesGenerales;
                    }
                    else if (tipo.Equals("REVISIÓN DE LAS COMPRAS URGENTES Y ESPECIALES -33 MIL"))
                    {
                        excelWorksheet.Cells[20, 3].Value = string.IsNullOrEmpty(result.observacionesGenerales) ? "" : result.observacionesGenerales;
                    }
                    //else if (tipo.Equals("OBRA"))
                    //{
                    //    excelWorksheet.Cells[52, 3].Value = string.IsNullOrEmpty(result.observacionesGenerales) ? "" : result.observacionesGenerales;
                    //}
                    #endregion

                    //Documentos
                    rows = 10;
                    columns = 9;
                    List<DocumentoAdquisicionesV1> listDoc = new List<DocumentoAdquisicionesV1>();
                    listDoc = result.documentos;
                    Sort(ref listDoc, "clave", "ASC");

                    foreach (var document in listDoc)
                    {

                        if (tipo.Equals("ADJUDICACIÓN DIRECTA"))
                        {
                            #region Documentos
                            if (document.clave >= 1 & document.clave <= 5)
                            {
                                excelWorksheet.Cells[rows, columns].Value = document.estatus; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = getObservaciones(result.Id, document.clave); columns++; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia; columns++;
                                //excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.grupo) ? "" : document.grupo; columns++;

                                if (document.clave == 5)
                                {
                                    rows++; rows++;
                                }
                                else
                                {
                                    rows++;
                                }
                                columns = 9;
                            }
                            else if (document.clave >= 6 & document.clave <= 28)
                            {
                                excelWorksheet.Cells[rows, columns].Value = document.estatus; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = getObservaciones(result.Id, document.clave); columns++; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia; columns++;
                                //excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.grupo) ? "" : document.grupo; columns++;
                                if (document.clave == 28)
                                {
                                    rows++; rows++;
                                }
                                else
                                {
                                    rows++;
                                }
                                columns = 9;
                            }
                            else if (document.clave >= 29 & document.clave <= 30)
                            {
                                excelWorksheet.Cells[rows, columns].Value = document.estatus; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = getObservaciones(result.Id, document.clave); columns++; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia; columns++;
                                //excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.grupo) ? "" : document.grupo; columns++;
                                if (document.clave == 30)
                                {
                                    rows++; rows++;
                                }
                                else
                                {
                                    rows++;
                                }
                                columns = 9;
                            }
                            else if (document.clave >= 31 & document.clave <= 38)
                            {
                                excelWorksheet.Cells[rows, columns].Value = document.estatus; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = getObservaciones(result.Id, document.clave); columns++; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia; columns++;
                                //excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.grupo) ? "" : document.grupo; columns++;
                                if (document.clave == 38)
                                {
                                    rows++; rows++;
                                }
                                else
                                {
                                    rows++;
                                }
                                columns = 9;
                            }
                            #endregion
                        }
                        else if (tipo.Equals("CONCURSO POR INVITACIÓN"))
                        {
                            #region Documentos
                            if (document.clave >= 1 & document.clave <= 5)
                            {
                                excelWorksheet.Cells[rows, columns].Value = document.estatus; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = getObservaciones(result.Id, document.clave); columns++; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia; columns++;
                                //excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.grupo) ? "" : document.grupo; columns++;

                                if (document.clave == 5)
                                {
                                    rows++; rows++;
                                }
                                else
                                {
                                    rows++;
                                }
                                columns = 9;
                            }
                            else if (document.clave >= 6 & document.clave <= 26)
                            {
                                excelWorksheet.Cells[rows, columns].Value = document.estatus; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = getObservaciones(result.Id, document.clave); columns++; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia; columns++;
                                //excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.grupo) ? "" : document.grupo; columns++;
                                if (document.clave == 26)
                                {
                                    rows++; rows++;
                                }
                                else
                                {
                                    rows++;
                                }
                                columns = 9;
                            }
                            else if (document.clave >= 27 & document.clave <= 32)
                            {
                                excelWorksheet.Cells[rows, columns].Value = document.estatus; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = getObservaciones(result.Id, document.clave); columns++; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia; columns++;
                                //excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.grupo) ? "" : document.grupo; columns++;
                                if (document.clave == 32)
                                {
                                    rows++; rows++;
                                }
                                else
                                {
                                    rows++;
                                }
                                columns = 9;
                            }
                            else if (document.clave >= 33 & document.clave <= 35)
                            {
                                excelWorksheet.Cells[rows, columns].Value = document.estatus; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = getObservaciones(result.Id, document.clave); columns++; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia; columns++;
                                //excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.grupo) ? "" : document.grupo; columns++;
                                if (document.clave == 35)
                                {
                                    rows++; rows++;
                                }
                                else
                                {
                                    rows++;
                                }
                                columns = 9;
                            }
                            else if (document.clave >= 36 & document.clave <= 43)
                            {
                                excelWorksheet.Cells[rows, columns].Value = document.estatus; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = getObservaciones(result.Id, document.clave); columns++; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia; columns++;
                                //excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.grupo) ? "" : document.grupo; columns++;
                                if (document.clave == 35)
                                {
                                    rows++; rows++;
                                }
                                else
                                {
                                    rows++;
                                }
                                columns = 9;
                            }
                            #endregion
                        }
                        else if (tipo.Equals("INVITACIÓN A CUANDO MENOS TRES PERSONAS"))
                        {
                            #region Documentos
                            if (document.clave >= 1 & document.clave <= 5)
                            {
                                excelWorksheet.Cells[rows, columns].Value = document.estatus; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = getObservaciones(result.Id, document.clave); columns++; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia; columns++;
                                //excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.grupo) ? "" : document.grupo; columns++;

                                if (document.clave == 5)
                                {
                                    rows++; rows++;
                                }
                                else
                                {
                                    rows++;
                                }
                                columns = 9;
                            }
                            else if (document.clave >= 6 & document.clave <= 18)
                            {
                                excelWorksheet.Cells[rows, columns].Value = document.estatus; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = getObservaciones(result.Id, document.clave); columns++; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia; columns++;
                                //excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.grupo) ? "" : document.grupo; columns++;
                                if (document.clave == 18)
                                {
                                    rows++; rows++;
                                }
                                else
                                {
                                    rows++;
                                }
                                columns = 9;
                            }
                            else if (document.clave >= 19 & document.clave <= 34)
                            {
                                excelWorksheet.Cells[rows, columns].Value = document.estatus; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = getObservaciones(result.Id, document.clave); columns++; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia; columns++;
                                //excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.grupo) ? "" : document.grupo; columns++;
                                if (document.clave == 34)
                                {
                                    rows++; rows++;
                                }
                                else
                                {
                                    rows++;
                                }
                                columns = 9;
                            }
                            else if (document.clave >= 35 & document.clave <= 37)
                            {
                                excelWorksheet.Cells[rows, columns].Value = document.estatus; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = getObservaciones(result.Id, document.clave); columns++; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia; columns++;
                                //excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.grupo) ? "" : document.grupo; columns++;
                                if (document.clave == 37)
                                {
                                    rows++; rows++;
                                }
                                else
                                {
                                    rows++;
                                }
                                columns = 9;
                            }
                            else if (document.clave >= 38 & document.clave <= 45)
                            {
                                excelWorksheet.Cells[rows, columns].Value = document.estatus; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = getObservaciones(result.Id, document.clave); columns++; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia; columns++;
                                //excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.grupo) ? "" : document.grupo; columns++;
                                if (document.clave == 45)
                                {
                                    rows++; rows++;
                                }
                                else
                                {
                                    rows++;
                                }
                                columns = 9;
                            }
                            #endregion
                        }
                        else if (tipo.Equals("LICITACIÓN PÚBLICA"))
                        {
                            #region Documentos
                            if (document.clave >= 1 & document.clave <= 6)
                            {
                                excelWorksheet.Cells[rows, columns].Value = document.estatus; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = getObservaciones(result.Id, document.clave); columns++; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia; columns++;
                                //excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.grupo) ? "" : document.grupo; columns++;

                                if (document.clave == 6)
                                {
                                    rows++; rows++;
                                }
                                else
                                {
                                    rows++;
                                }
                                columns = 9;
                            }
                            else if (document.clave >= 7 & document.clave <= 25)
                            {
                                excelWorksheet.Cells[rows, columns].Value = document.estatus; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = getObservaciones(result.Id, document.clave); columns++; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia; columns++;
                                //excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.grupo) ? "" : document.grupo; columns++;
                                if (document.clave == 25)
                                {
                                    rows++; rows++;
                                }
                                else
                                {
                                    rows++;
                                }
                                columns = 9;
                            }
                            else if (document.clave >= 26 & document.clave <= 29)
                            {
                                excelWorksheet.Cells[rows, columns].Value = document.estatus; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = getObservaciones(result.Id, document.clave); columns++; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia; columns++;
                                //excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.grupo) ? "" : document.grupo; columns++;
                                if (document.clave == 29)
                                {
                                    rows++; rows++;
                                }
                                else
                                {
                                    rows++;
                                }
                                columns = 9;
                            }
                            else if (document.clave >= 30 & document.clave <= 31)
                            {
                                excelWorksheet.Cells[rows, columns].Value = document.estatus; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = getObservaciones(result.Id, document.clave); columns++; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia; columns++;
                                //excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.grupo) ? "" : document.grupo; columns++;
                                if (document.clave == 31)
                                {
                                    rows++; rows++;
                                }
                                else
                                {
                                    rows++;
                                }
                                columns = 9;
                            }
                            else if (document.clave >= 32 & document.clave <= 36)
                            {
                                excelWorksheet.Cells[rows, columns].Value = document.estatus; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = getObservaciones(result.Id, document.clave); columns++; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia; columns++;
                                //excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.grupo) ? "" : document.grupo; columns++;
                                if (document.clave == 36)
                                {
                                    rows++; rows++;
                                }
                                else
                                {
                                    rows++;
                                }
                                columns = 9;
                            }
                            #endregion
                        }
                        else if (tipo.Equals("REVISIÓN DE LAS COMPRAS URGENTES Y ESPECIALES -33 MIL"))
                        {
                            #region Documentos
                            if (document.clave >= 1 & document.clave <= 3)
                            {
                                excelWorksheet.Cells[rows, columns].Value = document.estatus; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = getObservaciones(result.Id, document.clave); columns++; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia; columns++;
                                //excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.grupo) ? "" : document.grupo; columns++;

                                if (document.clave == 3)
                                {
                                    rows++; rows++;
                                }
                                else
                                {
                                    rows++;
                                }
                                columns = 9;
                            }

                            if (document.clave >= 4 & document.clave <= 8)
                            {
                                excelWorksheet.Cells[rows, columns].Value = document.estatus; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = getObservaciones(result.Id, document.clave); columns++; columns++; columns++;
                                excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia; columns++;
                                //excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.grupo) ? "" : document.grupo; columns++;
                                if (document.clave == 8)
                                {
                                    rows++; rows++;
                                }
                                else
                                {
                                    rows++;
                                }
                                columns = 9;
                            }
                            #endregion
                        }
                        else if (tipo.Equals("OBRA"))
                        {
                        }


                    }

                    excelPackage.SaveAs(ms);

                    FileInfo fi = new FileInfo(path + name);
                    excelPackage.SaveAs(fi);
                }
                ms.Position = 0;
            }
        }

        private void ReporteObraSach(string id, string path, string name)
        {
            string constr = ConfigurationManager.AppSettings["connectionString"];
            var Client = new MongoClient(constr);
            var DB = Client.GetDatabase("PRB");
            var collection = DB.GetCollection<ObraPublicaV1>("ObraPublica");

            var filter = Builders<ObraPublicaV1>.Filter.Eq(x => x.Id, id);
            var result = collection.Find(filter).FirstOrDefault();

            MemoryStream memoryStream = new MemoryStream();
            WebClient client = new WebClient();

            try
            {
                //memoryStream = new MemoryStream(client.DownloadData("C:/Simopa/Templete/TemplateItinerarios.xlsx"));
                memoryStream = new MemoryStream(client.DownloadData("https://apipbrdevelolop.azurewebsites.net/templates/reportes/" + name));
            }
            finally
            {
                client.Dispose();
            }

            int rows = 1;
            int columns = 1;
            MemoryStream ms = new MemoryStream();

            using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {
                ExcelWorkbook excelWorkBook = excelPackage.Workbook;
                ExcelWorksheet excelWorksheet = excelWorkBook.Worksheets[1];

                if (result != null)
                {
                    excelWorksheet.Cells[3, 11].Value = result.municipio;
                    //fecha

                    excelWorksheet.Cells[4, 11].Value = result.programa;
                    excelWorksheet.Cells[4, 11].Value = result.ejercicio;

                    excelWorksheet.Cells[5, 11].Value = result.ejecutor;
                    excelWorksheet.Cells[5, 62].Value = result.estado;
                    excelWorksheet.Cells[6, 62].Value = result.municipio;


                    excelWorksheet.Cells[7, 10].Value = result.fechaRevision;
                    excelWorksheet.Cells[8, 10].Value = result.localidad;

                    excelWorksheet.Cells[9, 10].Value = result.nombreObra;
                    excelWorksheet.Cells[10, 10].Value = result.proyecto;
                    excelWorksheet.Cells[10, 59].Value = result.numeroObra;

                    excelWorksheet.Cells[11, 10].Value = result.programa;
                    //excelWorksheet.Cells[11, 10].Value = result.;  //Normativa

                    //Modalidad
                    if (result.modalidad.Equals("CONTRATO"))
                    {
                        excelWorksheet.Cells[15, 27].Value = "X";
                    }
                    else if (result.modalidad.Equals("MIXTO"))
                    {
                        excelWorksheet.Cells[15, 39].Value = "X";
                    }
                    else
                    {
                        excelWorksheet.Cells[15, 15].Value = "X";
                    }

                    //Tipo de Adjudicacion
                    if (result.tipoAdjudicacion.Equals("ADJUDICACIÓN DIRECTA"))
                    {
                        excelWorksheet.Cells[16, 15].Value = "X";
                    }
                    else if (result.tipoAdjudicacion.Equals("LICITACIÓN PÚBLICA"))
                    {
                        excelWorksheet.Cells[16, 47].Value = "X";
                    }
                    else if (result.tipoAdjudicacion.Equals("INVITACIÓN RESTRINGIDA A 5 PROVEEDORES"))
                    {
                        excelWorksheet.Cells[16, 39].Value = "X";
                    }
                    else if (result.tipoAdjudicacion.Equals("INVITACIÓN RESTRINGIDA A 3 PROVEEDORES"))
                    {
                        excelWorksheet.Cells[16, 27].Value = "X";
                    }

                    excelWorksheet.Cells[16, 54].Value = result.numeroProcedimiento;

                    //excelWorksheet.Cells[17, 54].Value = result.numeroProcedimiento;
                    //Tipo de Contrato
                    if (result.tipoContrato.Equals("DE OBRA"))
                    {
                        excelWorksheet.Cells[17, 15].Value = "X";
                    }
                    else if (result.tipoContrato.Equals("OTRO"))
                    {
                        excelWorksheet.Cells[17, 47].Value = "X";
                    }
                    else if (result.tipoContrato.Contains("RENTA"))
                    {
                        excelWorksheet.Cells[17, 39].Value = "X";
                    }
                    else
                    {
                        excelWorksheet.Cells[17, 27].Value = "X";
                    }

                    excelWorksheet.Cells[17, 54].Value = result.fechaProcedimiento;

                    excelWorksheet.Cells[18, 10].Value = result.proveedor;
                    excelWorksheet.Cells[19, 10].Value = result.numeroContrato;

                    //Coordenadas
                    if(result.Location == null)
                    {
                        excelWorksheet.Cells[21, 56].Value = "";
                        excelWorksheet.Cells[21, 60].Value = "";
                    }
                    else
                    {
                        excelWorksheet.Cells[21, 56].Value = result.Location.Coordinates[1];//Latitud
                        excelWorksheet.Cells[21, 60].Value = result.Location.Coordinates[0];//Longitud
                    }                    

                    excelWorksheet.Cells[22, 10].Value = result.montoAsignado;
                    excelWorksheet.Cells[22, 31].Value = result.montoContrato;
                    excelWorksheet.Cells[22, 54].Value = result.montoEjercido;


                    List<DocumentosObrasV1> listDoc = new List<DocumentosObrasV1>();
                    listDoc = result.documentos;
                    Sort(ref listDoc, "clave", "ASC");

                    rows = 36;
                    columns = 38;

                    foreach (var document in listDoc)
                    {
                        #region Documentos
                        if (document.clave >= 1 & document.clave <= 10)
                        {

                            excelWorksheet.Cells[rows, columns].Value = document.integracion;
                            columns = 57;
                            excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia;
                            if (document.clave == 3)
                            {
                                rows++; rows++; rows++;
                            }
                            else if (document.clave == 4)
                            {
                                rows++; rows++; rows++;
                            }
                            else if (document.clave == 10)
                            {
                                rows++; rows++; rows++; rows++;
                            }
                            else
                            {
                                rows++;
                            }
                            columns = 38;

                        }
                        else if (document.clave >= 11 & document.clave <= 19)
                        {
                            excelWorksheet.Cells[rows, columns].Value = document.integracion;
                            columns = 57;
                            excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia;
                            if (document.clave == 19)
                            {
                                rows++; rows++;
                            }
                            else
                            {
                                rows++;
                            }
                            columns = 38;
                        }
                        else if (document.clave >= 20 & document.clave <= 26)
                        {
                            excelWorksheet.Cells[rows, columns].Value = document.integracion;
                            columns = 57;
                            excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia;
                            if (document.clave == 26)
                            {
                                rows++; rows++; rows++; rows++;
                            }
                            else
                            {
                                rows++;
                            }
                            columns = 38;
                        }
                        else if (document.clave >= 27 & document.clave <= 32)
                        {
                            excelWorksheet.Cells[rows, columns].Value = document.integracion;
                            columns = 57;
                            excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia;
                            if (document.clave == 32)
                            {
                                rows++; rows++; rows++; rows++;
                            }
                            else
                            {
                                rows++;
                            }
                            columns = 38;
                        }
                        else if (document.clave >= 33 & document.clave <= 41)
                        {
                            excelWorksheet.Cells[rows, columns].Value = document.integracion;
                            columns = 57;
                            excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia;
                            if (document.clave == 41)
                            {
                                rows++; rows++;
                            }
                            else
                            {
                                rows++;
                            }
                            columns = 38;
                        }
                        else if (document.clave >= 42 & document.clave <= 43)
                        {
                            excelWorksheet.Cells[rows, columns].Value = document.integracion;
                            columns = 57;
                            excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia;
                            if (document.clave == 43)
                            {
                                rows++; rows++;
                            }
                            else
                            {
                                rows++;
                            }
                            columns = 38;
                        }
                        else if (document.clave == 44)
                        {
                            excelWorksheet.Cells[rows, columns].Value = document.integracion;
                            columns = 57;
                            excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia;
                            if (document.clave == 44)
                            {
                                rows++; rows++; rows++; rows++; rows++;
                            }
                            else
                            {
                                rows++;
                            }
                            columns = 38;
                        }
                        else if (document.clave >= 45 & document.clave <= 57)
                        {
                            excelWorksheet.Cells[rows, columns].Value = document.integracion;
                            columns = 57;
                            excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia;
                            if (document.clave == 53)
                            {
                                rows++; rows++; rows++; rows++;
                            }
                            else if (document.clave == 56)
                            {
                                rows++; rows++;
                            }
                            else if (document.clave == 57)
                            {
                                rows++; rows++; rows++;
                            }
                            else
                            {
                                rows++;
                            }
                            columns = 38;
                        }
                        else if (document.clave >= 58 & document.clave <= 62)
                        {
                            excelWorksheet.Cells[rows, columns].Value = document.integracion;
                            columns = 57;
                            excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia;
                            if (document.clave == 61)
                            {
                                rows++; rows++;
                            }
                            else if (document.clave == 62)
                            {
                                rows++; rows++;
                            }
                            else
                            {
                                rows++;
                            }
                            columns = 38;
                        }
                        else if (document.clave >= 63 & document.clave <= 72)
                        {
                            excelWorksheet.Cells[rows, columns].Value = document.integracion;
                            columns = 57;
                            excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia;
                            if (document.clave == 72)
                            {
                                rows++; rows++;
                            }
                            else
                            {
                                rows++;
                            }
                            columns = 38;
                        }
                        else if (document.clave >= 73 & document.clave <= 75)
                        {
                            excelWorksheet.Cells[rows, columns].Value = document.integracion;
                            columns = 57;
                            excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia;
                            if (document.clave == 75)
                            {
                                rows++; rows++;
                            }
                            else
                            {
                                rows++;
                            }
                            columns = 38;
                        }
                        else if (document.clave >= 76 & document.clave <= 84)
                        {
                            excelWorksheet.Cells[rows, columns].Value = document.integracion;
                            columns = 57;
                            excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia;
                            if (document.clave == 76)
                            {
                                rows++; rows++; rows++;
                            }
                            else if (document.clave == 84)
                            {
                                rows++; rows++;
                            }
                            else
                            {
                                rows++;
                            }
                            columns = 38;
                        }
                        else if (document.clave >= 85 & document.clave <= 90)
                        {
                            excelWorksheet.Cells[rows, columns].Value = document.integracion;
                            columns = 57;
                            excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia;
                            if (document.clave == 90)
                            {
                                rows++; rows++;
                            }
                            else
                            {
                                rows++;
                            }
                            columns = 38;
                        }
                        else if (document.clave >= 91 & document.clave <= 96)
                        {
                            excelWorksheet.Cells[rows, columns].Value = document.integracion;
                            columns = 57;
                            excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia;
                            if (document.clave == 96)
                            {
                                rows++; rows++;
                            }
                            else
                            {
                                rows++;
                            }
                            columns = 38;
                        }
                        else if (document.clave >= 97 & document.clave <= 98)
                        {
                            excelWorksheet.Cells[rows, columns].Value = document.integracion;
                            columns = 57;
                            excelWorksheet.Cells[rows, columns].Value = string.IsNullOrEmpty(document.referencia) ? "" : document.referencia;
                            if (document.clave == 98)
                            {
                                rows++; rows++;
                            }
                            else
                            {
                                rows++;
                            }
                            columns = 38;
                        }

                        #endregion

                    }

                    excelPackage.SaveAs(ms);

                    FileInfo fi = new FileInfo(path + name);
                    excelPackage.SaveAs(fi);
                }
                ms.Position = 0;
            }
        }

        #endregion

    }
}