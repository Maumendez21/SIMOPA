using apiPBR.Models.Response.Expedientes;
using credentialsPBR.Models.Expedientes.ObraPublica;
using Microsoft.Ajax.Utilities;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using MongoDB.Driver;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace apiPBR.Controllers.Auditor.ObraPublica.V1
{
    public class VisitaObraReporteController : ApiController
    {
        //[Authorize(Roles = "Auditor, Coordinador, Ejecutivo")]
        [AllowAnonymous]
        [HttpGet]
        [Route("api/expedients/obrapublica/reporte/visitaobra/{id}")]
        public async System.Threading.Tasks.Task<HttpResponseMessage> DescargaReporteVisitaObra(string id)
        {
            //5ea07ca3d066066da4885e82
            try
            {
                MemoryStream memoryStream = new MemoryStream();

                WebClient client = new WebClient();
                try
                {
                    memoryStream = new MemoryStream(client.DownloadData("https://apipbrdevelolop.azurewebsites.net/templates/reportes/TemplateVisitaObra.xlsx"));
                    //memoryStream = new MemoryStream(client.DownloadData("C:/Simopa/Templete/TemplateVisitaObra.xlsx"));
                }
                finally
                {
                    client.Dispose();
                }

                string idVisita = string.Empty;
                string nombreObra = string.Empty;
                string noObra = string.Empty;
                string noContrato = string.Empty;
                string montoContrato = string.Empty;
                string proveedor = string.Empty;
                string fechaVisita = string.Empty;
                string idExpediente = string.Empty;
                string tipoAdjudicacion = string.Empty;
                int documentacion = 0;
                int fisico = 0;
                int financiero = 0;
                string situacionActual = string.Empty;
                string problematica = string.Empty;
                int si = 0;
                int no = 0;
                int docError = 0;
                List<PartidasPrincipalesObra> partidas = new List<PartidasPrincipalesObra>();
                bool licenciaConstruccion = false;
                bool licenciaFactibilidad = false;

                string constr = ConfigurationManager.AppSettings["connectionString"];
                var Client = new MongoClient(constr);
                var DB = Client.GetDatabase("PRB");

                try
                {
                    var collection = DB.GetCollection<VisitaObraHeader>("VisitaObra");

                    var filter = Builders<VisitaObraHeader>.Filter.Eq(x => x.Id, id);

                    var r = await collection.Find(filter).FirstOrDefaultAsync();

                    if (r != null)
                    {
                        idVisita = r.Id;
                        idExpediente = r.ExpedienteId;
                        financiero = r.AvaceFinanciero;
                        fisico = r.AvanceFisico;
                        fechaVisita = r.FechaVisita.ToString("dddd, dd MMMM yyyy HH:mm:ss", new CultureInfo("es-MX"));
                        fechaVisita = (CultureInfo.InvariantCulture.TextInfo.ToTitleCase(fechaVisita));
                        situacionActual = r.SitutacionActual;
                        problematica = r.Problematica;
                    }
                }
                catch (Exception ex)
                {
                    //response.success = false;
                    //response.messages.Add(ex.ToString());

                    //return Ok(ex.ToString());
                }
                try
                {
                    var collection = DB.GetCollection<ObraPublicaV1>("ObraPublica");
                    var filter = Builders<ObraPublicaV1>.Filter.Eq(x => x.Id, idExpediente);
                    var result = await collection.Find(filter).FirstOrDefaultAsync();

                    if (result != null)
                    {
                        noObra = result.numeroObra;
                        noContrato = result.numeroContrato;
                        tipoAdjudicacion = result.tipoAdjudicacion;
                        nombreObra = result.nombreObra;
                        proveedor = result.proveedor;
                        montoContrato = result.montoContrato;
                        si = result.documentos.Where(x => x.integracion == "SI").Count();
                        no = result.documentos.Where(x => x.integracion == "NO").Count();
                        docError = result.documentos.Where(x => x.integracion == "DOC.ERROR").Count();

                        documentacion = (si * 100) / (si + no + docError);

                        foreach (var d in result.documentos)
                        {
                            if (d.clave == 15)
                                licenciaFactibilidad = d.integracion == "SI" ? true : false;

                            if (d.clave == 16)
                                licenciaConstruccion = d.integracion == "SI" ? true : false;
                        }
                    }
                    else
                    {
                        //response.success = false;
                        //response.messages.Add("No se encontró el registro");
                        //return Ok();
                    }
                }
                catch (Exception ex)
                {
                    //response.success = false;
                    //response.messages.Add(ex.ToString());
                    //return Ok(ex.ToString());
                }

                try
                {
                    var collection = DB.GetCollection<PartidasPrincipalesObra>("PartidasPrincipalesObra");
                    var filter = Builders<PartidasPrincipalesObra>.Filter.Eq(x => x.ExpedienteId, idExpediente);
                    var result = await collection.Find(filter).ToListAsync();

                    if (result != null)
                    {
                        foreach (var r in result)
                        {
                            PartidasPrincipalesObra partida = new PartidasPrincipalesObra();
                            partida.Nombre = r.Nombre;
                            partida.Estatus = r.Estatus;

                            partidas.Add(partida);
                        }
                    }
                    else
                    {
                        //response.success = false;
                        //response.messages.Add("No se encontró el registro");
                        //return Ok();
                    }
                }
                catch (Exception ex)
                {
                    //response.success = false;
                    //response.messages.Add(ex.ToString());

                    //return Ok(ex.ToString());
                }
                //response.success = true;
                //response.messages.Add("Consulta Exitosa");

                MemoryStream ms = new MemoryStream();
                MemoryStream pdf = new MemoryStream();

                using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
                {

                    ExcelWorkbook excelWorkBook = excelPackage.Workbook;
                    ExcelWorksheet excelWorksheet = excelWorkBook.Worksheets[1];

                    excelWorksheet.Cells[1, 8].Value = nombreObra;
                    excelWorksheet.Cells[4, 1].Value = "Fecha Visita: " + fechaVisita;
                    //Color
                    //excelWorksheet.Cells[1, 46].Value = Color.Red;
                    excelWorksheet.Cells[1, 46].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    if (problematica.Equals(""))
                    {
                        excelWorksheet.Cells[1, 46].Style.Fill.BackgroundColor.SetColor(Color.Green);
                    }
                    else
                    {
                        excelWorksheet.Cells[1, 46].Style.Fill.BackgroundColor.SetColor(Color.Red);
                    }

                    excelWorksheet.Cells[7, 8].Value = noObra;
                    excelWorksheet.Cells[8, 8].Value = noContrato;
                    excelWorksheet.Cells[9, 8].Value = montoContrato;

                    // Fecha de inicio y fin
                    excelWorksheet.Cells[10, 8].Value = "";
                    excelWorksheet.Cells[11, 8].Value = "";

                    //proveedor
                    excelWorksheet.Cells[17, 23].Value = proveedor;

                    //Grafica
                    excelWorksheet.Cells[15, 2].Value = documentacion;
                    excelWorksheet.Cells[16, 2].Value = financiero;
                    excelWorksheet.Cells[17, 2].Value = fisico;

                    //Documentos
                    excelWorksheet.Cells[14, 46].Value = (si + no + docError).ToString();
                    excelWorksheet.Cells[15, 46].Value = si.ToString();
                    excelWorksheet.Cells[16, 46].Value = no.ToString();
                    excelWorksheet.Cells[17, 46].Value = docError.ToString();

                    //Situacion y problematica
                    excelWorksheet.Cells[43, 1].Value = situacionActual;
                    excelWorksheet.Cells[43, 26].Value = problematica;

                    //Partidas principales
                    int contadorPartida = 0;
                    foreach (var p in partidas)
                    {
                        if (p.Estatus)
                        {
                            excelWorksheet.Cells[7 + contadorPartida, 23].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            excelWorksheet.Cells[7 + contadorPartida, 23].Style.Fill.BackgroundColor.SetColor(Color.Green);
                        }
                        else
                        {
                            excelWorksheet.Cells[7 + contadorPartida, 23].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            excelWorksheet.Cells[7 + contadorPartida, 23].Style.Fill.BackgroundColor.SetColor(Color.Red);
                        }
                        excelWorksheet.Cells[7 + contadorPartida, 24].Value = p.Nombre;
                        contadorPartida++;
                    }

                    //Licencias
                    if (licenciaConstruccion)
                    {
                        excelWorksheet.Cells[10, 48].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        excelWorksheet.Cells[10, 48].Style.Fill.BackgroundColor.SetColor(Color.Green);
                    }
                    else
                    {
                        excelWorksheet.Cells[10, 48].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        excelWorksheet.Cells[10, 48].Style.Fill.BackgroundColor.SetColor(Color.Red);
                    }
                    if (licenciaFactibilidad)
                    {
                        excelWorksheet.Cells[8, 48].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        excelWorksheet.Cells[8, 48].Style.Fill.BackgroundColor.SetColor(Color.Green);
                    }
                    else
                    {
                        excelWorksheet.Cells[8, 48].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        excelWorksheet.Cells[8, 48].Style.Fill.BackgroundColor.SetColor(Color.Red);
                    }

                    //Loop para carga de imagenes
                    try
                    {
                        var collection = DB.GetCollection<VisitaObraImagenes>("VisitaObraImagen");
                        var filter = Builders<VisitaObraImagenes>.Filter.Eq(x => x.VisitaId, id);
                        var r = await collection.Find(filter).ToListAsync();

                        if (r != null)
                        {
                            int contador = 1;
                            int col = 0;
                            foreach (var image in r)
                            {
                                if (contador == 3)
                                    break;

                                REvidenciaFotografica responseEvidenciaFotografica = new REvidenciaFotografica();

                                CloudStorageAccount account = new CloudStorageAccount(new StorageCredentials("storagepbr", "mWminVy4acvZseDV7bt4ZVKA2w8ZA4nRr1LJ00aA0WFSW92P6/Hl4/DLZSOWNoRD+X5LkfSCttIEUYO1N1dpaA=="), true);
                                var blobClient = account.CreateCloudBlobClient();
                                var container = blobClient.GetContainerReference("bancodocumentos");
                                var blob = container.GetBlockBlobReference(image.path);
                                var sasToken = blob.GetSharedAccessSignature(new SharedAccessBlobPolicy()
                                {
                                    Permissions = SharedAccessBlobPermissions.Read,
                                    SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(100),//assuming the blob can be downloaded in 10 miinutes
                                }, new SharedAccessBlobHeaders()
                                {
                                    ContentDisposition = "attachment; filename=" + image.nombre
                                });
                                var blobUrl = string.Format("{0}{1}", blob.Uri, sasToken);

                                responseEvidenciaFotografica.url = blobUrl;
                                responseEvidenciaFotografica.title = image.titulo;
                                responseEvidenciaFotografica.name = image.nombre;

                                //int PixelTop = 88;
                                //int PixelLeft = 129;
                                int rowIndex = 21;
                                int colIndex = col;
                                int Height = 280;
                                int Width = 280;

                                MemoryStream memoryImage = new MemoryStream();
                                try
                                {
                                    memoryImage = new MemoryStream(client.DownloadData(blobUrl));
                                }
                                finally
                                {
                                    client.Dispose();
                                }

                                //Original
                                //Image img = Image.FromStream(memoryImage);
                                //ExcelPicture pic = excelWorksheet.Drawings.AddPicture("imageVista-"+contador, img);
                                //pic.SetPosition(rowIndex, 0, colIndex + ((contador - 1) * 4), 0);
                                //pic.SetSize(Width, Height);

                                Image img = Image.FromStream(memoryImage);
                                ExcelPicture pic = excelWorksheet.Drawings.AddPicture("imageVista-" + contador, img);  //23-2           23-23
                                if (contador == 2)
                                {
                                    rowIndex = 21;
                                    colIndex = 20;
                                }
                                pic.SetPosition(rowIndex, 0, colIndex + ((contador - 1) * 4), 0);
                                pic.SetSize(Width, Height);

                                contador++;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //response.success = false;
                        //response.messages.Add(ex.ToString());
                        //return Ok(ex.ToString());
                    }
                    excelPackage.SaveAs(ms);
                }


                ms.Position = 0;

                Spire.Xls.Workbook workbook = new Spire.Xls.Workbook();
                //workbook.LoadFromFile(result.numeroAdjudicacion + ".xlsx");
                //workbook.SaveToFile(result.numeroAdjudicacion + ".pdf", Spire.Xls.FileFormat.PDF);

                workbook.LoadFromStream(ms);
                workbook.SaveToStream(pdf, Spire.Xls.FileFormat.PDF);

                var reporte = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(pdf.ToArray())
                };
                reporte.Content.Headers.ContentDisposition =
                    new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                    {
                        FileName = "ReporteVisitaObra22.pdf"
                    };
                reporte.Content.Headers.ContentType =
                    new MediaTypeHeaderValue("application/octet-stream");

                return reporte;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
