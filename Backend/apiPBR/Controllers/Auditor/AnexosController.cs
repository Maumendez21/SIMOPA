using apiPBR.Models.Response;
using apiPBR.Models.Response.Expedientes;
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
    public class AnexosController : ApiController
    {
        [Authorize(Roles = "Auditor, Coordinador, Ejecutivo")]
        [HttpPost]
        [Route("api/expedients/{tipoExpediente}/anexo")]
        public async System.Threading.Tasks.Task<IHttpActionResult> UploadEvidenciaFotograficaAdquisiciones(string tipoExpediente)
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

            GenericClass genericClass = new GenericClass();
            UploadFileToAzure uploadFileToAzure = new UploadFileToAzure();
            string idExpediente = string.Empty;
            string titulo = string.Empty;
            string descripcion = string.Empty;
            string path = string.Empty;
            string filenameUpload = string.Empty;
            GetExpedientesInformacion getExpedientesInformacion = new GetExpedientesInformacion();
            ExpedienteInformacion expedienteInformacion = new ExpedienteInformacion();
            var evidenciaKeys = HttpContext.Current.Request.Form.AllKeys;

            foreach (string e in evidenciaKeys)
            {
                if (e == "idExpediente")
                {
                    idExpediente = HttpContext.Current.Request.Form[e].ToString();
                }
                if (e == "descripcion")
                {
                    descripcion = HttpContext.Current.Request.Form[e].ToString();
                }
                if (e == "titulo")
                {
                    titulo = HttpContext.Current.Request.Form[e].ToString();
                }
            }

            expedienteInformacion = await getExpedientesInformacion.GetInformationFromExpedientsAsync(tipoExpediente, idExpediente);



            var file = HttpContext.Current.Request.Files.Count > 0 ?
        HttpContext.Current.Request.Files[0] : null;

            if (file != null && file.ContentLength > 0)
            {

                string[] extencion = file.FileName.Split('.');
                filenameUpload = "anexo_" + DateTime.Now.ToString("yyyy'_'MM'_'dd'T'HH':'mm':'ss") + "." + extencion[1];

                path = expedienteInformacion.estado + "/" +
                expedienteInformacion.municipio + "/" +
                expedienteInformacion.ejercicio + "/" +
                expedienteInformacion.tipoExpediente + "/" +
                expedienteInformacion.idExpediente + "/" +
                "anexo" + "/" +
                filenameUpload;


                byte[] fileData = null;
                using (var binaryReader = new BinaryReader(file.InputStream))
                {
                    fileData = binaryReader.ReadBytes(file.ContentLength);
                }

                MemoryStream ms = new MemoryStream(fileData);


                if (uploadFileToAzure.UploadFileAzure(path, ms))
                {
                    genericClass.success = true;
                    genericClass.messages.Add("Anexo Subido correctamente");


                    SaveRecordEvidenciaFotograficaMongo saveRecordEvidenciaFotograficaMongo = new SaveRecordEvidenciaFotograficaMongo();

                    if (saveRecordEvidenciaFotograficaMongo.SaveRecordAnexoAsync(expedienteInformacion, path, filenameUpload, descripcion, titulo, idUsername))
                    {
                        genericClass.messages.Add("Registro guardado correctamente");
                    }
                    else
                    {
                        genericClass.success = false;
                        genericClass.messages.Add("Error al guardar el registro en base de datos");
                    }

                }
                return Ok(genericClass);
            }

            genericClass.success = false;
            genericClass.messages.Add("Error al subir imagen");

            return Ok(genericClass);
        }

        [Authorize(Roles = "Auditor, Coordinador, Ejecutivo")]
        [HttpGet]
        [Route("api/expedients/anexo/{idExpediente}")]
        public IHttpActionResult EvidenciaFotograficaDescargaImgAsync(string idExpediente)
        {
            string constr = ConfigurationManager.AppSettings["connectionString"];
            ResponseEvidenciaFotografica lrequestAnexo = new ResponseEvidenciaFotografica
            {
                listResponseEvidenciaFotografica = new List<REvidenciaFotografica>()
            };

            try
            {
                var Client = new MongoClient(constr);

                var DB = Client.GetDatabase("PRB");
                var collection = DB.GetCollection<EvidenciaFotografica>("Anexo");

                var filter = Builders<EvidenciaFotografica>.Filter.Eq(x => x.idExpediente, idExpediente);

                var result = collection.Find(filter).ToList();

                if (result != null)
                {
                    foreach (var r in result)
                    {
                        REvidenciaFotografica responseAnexo = new REvidenciaFotografica();

                        CloudStorageAccount account = new CloudStorageAccount(new StorageCredentials("storagepbr", "mWminVy4acvZseDV7bt4ZVKA2w8ZA4nRr1LJ00aA0WFSW92P6/Hl4/DLZSOWNoRD+X5LkfSCttIEUYO1N1dpaA=="), true);
                        var blobClient = account.CreateCloudBlobClient();
                        var container = blobClient.GetContainerReference("bancodocumentos");
                        var blob = container.GetBlockBlobReference(r.path);
                        var sasToken = blob.GetSharedAccessSignature(new SharedAccessBlobPolicy()
                        {
                            Permissions = SharedAccessBlobPermissions.Read,
                            SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(100),//assuming the blob can be downloaded in 10 miinutes
                        }, new SharedAccessBlobHeaders()
                        {
                            ContentDisposition = "attachment; filename=" + r.nombre
                        });
                        var blobUrl = string.Format("{0}{1}", blob.Uri, sasToken);


                        responseAnexo.id = r.Id;
                        responseAnexo.description = r.descripcion;
                        responseAnexo.url = blobUrl;
                        responseAnexo.title = r.titulo;
                        responseAnexo.name = r.nombre;

                        lrequestAnexo.listResponseEvidenciaFotografica.Add(responseAnexo);

                    }

                    lrequestAnexo.success = true;
                    lrequestAnexo.messages.Add("Listado de imagenes generado correctamente");

                    return Ok(lrequestAnexo);
                }

                lrequestAnexo.success = false;
                lrequestAnexo.messages.Add("No se encontró registro");

                return Ok(lrequestAnexo);

            }
            catch (Exception ex)
            {
                lrequestAnexo.success = false;
                lrequestAnexo.messages.Add("Error al generar listado de imagenes");

                return Ok(lrequestAnexo);
            }

        }
        [Authorize(Roles = "Coordinador, Ejecutivo")]
        [HttpDelete]
        [Route("api/expedients/anexo/{id}")]
        public IHttpActionResult AnexoEliminaImgAsync(string id)
        {
            string constr = ConfigurationManager.AppSettings["connectionString"];
            ResponseEvidenciaFotografica lrequestAnexo = new ResponseEvidenciaFotografica
            {
                listResponseEvidenciaFotografica = new List<REvidenciaFotografica>()
            };

            try
            {
                var Client = new MongoClient(constr);

                var DB = Client.GetDatabase("PRB");
                var collection = DB.GetCollection<EvidenciaFotografica>("Anexo");

                var filter = Builders<EvidenciaFotografica>.Filter.Eq(x => x.Id, id);

                var result = collection.Find(filter).FirstOrDefault();

                if (result != null)
                {

                    REvidenciaFotografica responseAnexo = new REvidenciaFotografica();

                    CloudStorageAccount account = new CloudStorageAccount(new StorageCredentials("storagepbr", "mWminVy4acvZseDV7bt4ZVKA2w8ZA4nRr1LJ00aA0WFSW92P6/Hl4/DLZSOWNoRD+X5LkfSCttIEUYO1N1dpaA=="), true);
                    var blobClient = account.CreateCloudBlobClient();
                    var container = blobClient.GetContainerReference("bancodocumentos");
                    var blob = container.GetBlockBlobReference(result.path);

                    blob.DeleteIfExists();

                    //blockBlob.DeleteIfExists();

                    responseAnexo.description = result.descripcion;
                    responseAnexo.url = "";
                    responseAnexo.title = result.titulo;
                    responseAnexo.name = result.nombre;


                    //Elimina registro de base de datos
                    collection.DeleteOne(filter);


                    lrequestAnexo.listResponseEvidenciaFotografica.Add(responseAnexo);


                    lrequestAnexo.success = true;
                    lrequestAnexo.messages.Add("Anexo eliminado correctamente");

                    return Ok(lrequestAnexo);
                }

                lrequestAnexo.success = false;
                lrequestAnexo.messages.Add("No se encontró registro");

                return Ok(lrequestAnexo);

            }
            catch (Exception ex)
            {
                lrequestAnexo.success = false;
                lrequestAnexo.messages.Add("Error al eliminar recurso");

                return Ok(lrequestAnexo);
            }

        }
    }
}
