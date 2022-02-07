using apiPBR.Models;
using apiPBR.Models.Response;
using apiPBR.Models.Response.Expedientes.Adquisiciones;
using credentialsPBR.Models.Expedientes.Adquisiciones;
using credentialsPBR.Models.Expedientes.Utilerias;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

namespace apiPBR.Controllers.Auditor
{
    public class DocumentDonwloadController : ApiController
    {

        // Descarga PDF
        [AllowAnonymous]
        [HttpGet]
        [Route("api/expedients/adownload")]
        public async System.Threading.Tasks.Task<IHttpActionResult> DocumentsBajaAsync(string id, string clave)
        {
            GenericClass genericClass = new GenericClass();

            string constr = ConfigurationManager.AppSettings["connectionString"];

            try
            {
                var Client = new MongoClient(constr);

                var DB = Client.GetDatabase("PRB");
                var collection = DB.GetCollection<AdquisicionesV1>("Adquisiciones");

                var filter = Builders<AdquisicionesV1>.Filter.Eq(x => x.Id, id);

                var result = await collection.Find(filter).FirstOrDefaultAsync();


                CloudStorageAccount account = new CloudStorageAccount(new StorageCredentials("storagepbr", "mWminVy4acvZseDV7bt4ZVKA2w8ZA4nRr1LJ00aA0WFSW92P6/Hl4/DLZSOWNoRD+X5LkfSCttIEUYO1N1dpaA=="), true);
                var blobClient = account.CreateCloudBlobClient();
                var container = blobClient.GetContainerReference("bancodocumentos");

                var blob = container.GetBlockBlobReference(result.estado+"/"+result.municipio+"/"+result.ejercicio+"/"+result.expediente+"/"+ result.numeroAdjudicacion.TrimEnd('.') +"/"+ clave+".pdf");
                var sasToken = blob.GetSharedAccessSignature(new SharedAccessBlobPolicy()
                {
                    Permissions = SharedAccessBlobPermissions.Read,
                    SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(10),//assuming the blob can be downloaded in 10 miinutes
                }, new SharedAccessBlobHeaders()
                {
                    ContentDisposition = "attachment; filename=" + clave + ".pdf"
                });
                var blobUrl = string.Format("{0}{1}", blob.Uri, sasToken);
                return Redirect(blobUrl);

            }
            catch (Exception ex)
            {
                genericClass.success = false;
                genericClass.messages.Add(ex.ToString());

                return Ok(genericClass);
            }
        }


        // Elimina PDF
        //[AllowAnonymous]
        [Authorize(Roles = "Coordinador, Ejecutivo")]
        //[Authorize(Roles = "Magica")]
        [HttpDelete]
        [Route("api/expedients/adquisiciones/document")]
        public async System.Threading.Tasks.Task<IHttpActionResult> DocumentsDeletesync(string id, string clave)
        {
            GenericClass genericClass = new GenericClass();
            string constr = ConfigurationManager.AppSettings["connectionString"];
            AdquisicionesV1 adquisicionesV1 = new AdquisicionesV1
            {
                documentos = new List<DocumentoAdquisicionesV1>()
            };
            
            try
            {
                var Client = new MongoClient(constr);

                var DB = Client.GetDatabase("PRB");
                var collection = DB.GetCollection<AdquisicionesV1>("Adquisiciones");

                var filter = Builders<AdquisicionesV1>.Filter.Eq(x => x.Id, id);

                var result = await collection.Find(filter).FirstOrDefaultAsync();


                if (result != null)
                {
                    adquisicionesV1 = result;
                    UploadFileToAzure uploadFileToAzure = new UploadFileToAzure();
                    uploadFileToAzure.DeleteFileAzure(result.estado + "/" + result.municipio + "/" + result.ejercicio + "/" + result.expediente + "/" + result.numeroAdjudicacion + "/" + clave + ".pdf");

                    int index = adquisicionesV1.documentos.FindIndex(x=>x.clave == Convert.ToInt32(clave));
                    adquisicionesV1.documentos[index].estatus = "NO";


                    await collection.ReplaceOneAsync(filter,adquisicionesV1);

                    genericClass.success = true;
                    genericClass.messages.Add("Tarea realizada con exito");

                    return Ok(genericClass);

                }
                genericClass.success = false;
                genericClass.messages.Add("No se encontró el registro del expediente");

                return Ok(genericClass);

            }
            catch (Exception ex)
            {
                genericClass.success = false;
                genericClass.messages.Add(ex.ToString());

                return Ok(genericClass);
            }
        }

        // Sube PDF
        //[AllowAnonymous]
        [Authorize(Roles = "Auditor, Coordinador, Ejecutivo")]
        [HttpPost]
        [Route("api/expedients/adquisiciones/document")]
        public async System.Threading.Tasks.Task<IHttpActionResult> DocumentsUploadsync()
        {
            GenericClass genericClass = new GenericClass();

            var role = string.Empty;
            var identity = (ClaimsIdentity)User.Identity;
            IEnumerable<Claim> claims = identity.Claims;
            var roleClaimType = identity.RoleClaimType;
            var roles = claims.Where(c => c.Type == ClaimTypes.Role).ToList();
            string idUser = string.Empty;

            foreach (var r in roles)
            {
                role = r.Value;
            }

            AdquisicionesV1 adquisicionesV1 = new AdquisicionesV1
            {
                documentos = new List<DocumentoAdquisicionesV1>()
            };

            string constr = ConfigurationManager.AppSettings["connectionString"];
            string idExpediente = string.Empty;
            string clave = string.Empty;            
            string path = string.Empty;
            string filenameUpload = string.Empty;
            
            var evidenciaKeys = HttpContext.Current.Request.Form.AllKeys;

            foreach (string e in evidenciaKeys)
            {
                if (e == "idExpediente")
                {
                    idExpediente = HttpContext.Current.Request.Form[e].ToString();
                }
                if (e == "clave")
                {
                    clave = HttpContext.Current.Request.Form[e].ToString();
                }
            }

            foreach (var c in claims)
            {
                if (c.Type == "id")
                {
                    idUser = c.Value;
                    break;
                }
            }

            var file = HttpContext.Current.Request.Files.Count > 0 ?
        HttpContext.Current.Request.Files[0] : null;

            
            try
            {
                var Client = new MongoClient(constr);

                var DB = Client.GetDatabase("PRB");
                var collection = DB.GetCollection<AdquisicionesV1>("Adquisiciones");

                var filter = Builders<AdquisicionesV1>.Filter.Eq(x => x.Id, idExpediente);

                var result = await collection.Find(filter).FirstOrDefaultAsync();


                if (result != null)
                {
                    adquisicionesV1 = result;

                    //UploadFileToAzure uploadFileToAzure = new UploadFileToAzure();
                    //uploadFileToAzure.UploadFileAzure(result.estado + "/" + result.municipio + "/" + result.ejercicio + "/" + result.expediente + "/" + result.numeroAdjudicacion + "/" + clave + ".pdf");

                    if (file != null && file.ContentLength > 0)
                    {
                        filenameUpload = clave + ".pdf";

                        path = result.estado + "/" +
                        result.municipio + "/" +
                        result.ejercicio + "/" +
                        result.expediente + "/" +
                        result.numeroAdjudicacion + "/" +
                        filenameUpload;

                        byte[] fileData = null;
                        using (var binaryReader = new BinaryReader(file.InputStream))
                        {
                            fileData = binaryReader.ReadBytes(file.ContentLength);
                        }

                        MemoryStream ms = new MemoryStream(fileData);
                        UploadFileToAzure uploadFileToAzure = new UploadFileToAzure();
                        if (uploadFileToAzure.UploadFileAzure(path, ms))
                        {
                            genericClass.success = true;
                            genericClass.messages.Add("Archivo subido correctamente");
                        }                        
                    }

                    int index = adquisicionesV1.documentos.FindIndex(x => x.clave == Convert.ToInt32(clave));

                    if (role.Equals("Coordinador"))
                    {   
                        if(adquisicionesV1.auditor.Equals(idUser))
                        {
                            adquisicionesV1.documentos[index].estatus = "SI";
                        }
                        else
                        {
                            adquisicionesV1.documentos[index].estatus = "POR REVISAR";
                        }
                    }
                    else
                    {
                        adquisicionesV1.documentos[index].estatus = "SI";
                    }                    

                    await collection.ReplaceOneAsync(filter, adquisicionesV1);

                    genericClass.success = true;
                    genericClass.messages.Add("Base de datos actualizada con exito");

                    return Ok(genericClass);

                }
                genericClass.success = false;
                genericClass.messages.Add("No se encontró el registro del expediente");

                return Ok(genericClass);

            }
            catch (Exception ex)
            {
                genericClass.success = false;
                genericClass.messages.Add(ex.ToString());

                return Ok(genericClass);
            }
        }

        //Cedula
        [AllowAnonymous]
        [HttpGet]
        [Route("api/expedients/cedula/adownload")]
        public async System.Threading.Tasks.Task<IHttpActionResult> DocumentExcelCedulaAsync(string id)
        {
            GenericClass genericClass = new GenericClass();

            string constr = ConfigurationManager.AppSettings["connectionString"];

            try
            {
                var Client = new MongoClient(constr);

                var DB = Client.GetDatabase("PRB");
                var collection = DB.GetCollection<AdquisicionesV1>("Adquisiciones");

                var filter = Builders<AdquisicionesV1>.Filter.Eq(x => x.Id, id);

                var result = await collection.Find(filter).FirstOrDefaultAsync();


                CloudStorageAccount account = new CloudStorageAccount(new StorageCredentials("storagepbr", "mWminVy4acvZseDV7bt4ZVKA2w8ZA4nRr1LJ00aA0WFSW92P6/Hl4/DLZSOWNoRD+X5LkfSCttIEUYO1N1dpaA=="), true);
                var blobClient = account.CreateCloudBlobClient();
                var container = blobClient.GetContainerReference("bancodocumentos");
                var blob = container.GetBlockBlobReference(result.estado + "/" + result.municipio + "/" + result.ejercicio + "/" + result.expediente + "/" + result.numeroAdjudicacion + "/" + result.numeroAdjudicacion + ".xlsx");
                var sasToken = blob.GetSharedAccessSignature(new SharedAccessBlobPolicy()
                {
                    Permissions = SharedAccessBlobPermissions.Read,
                    SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(10),//assuming the blob can be downloaded in 10 miinutes
                }, new SharedAccessBlobHeaders()
                {
                    ContentDisposition = "attachment; filename=" + result.numeroAdjudicacion + ".xlsx"
                });
                var blobUrl = string.Format("{0}{1}", blob.Uri, sasToken);
                return Redirect(blobUrl);

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
