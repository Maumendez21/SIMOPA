using apiPBR.Models.Response;
using apiPBR.Models.Response.Expedientes;
using credentialsPBR.Models.Expedientes.ObraPublica;
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

namespace apiPBR.Controllers.Auditor.ObraPublica.V1
{
    public class ImagenesVisitaObraController : ApiController
    {
        [Authorize(Roles = "Auditor, Coordinador, Ejecutivo")]
        [HttpPost]
        [Route("api/expedients/obrapublica/visitaobra/imagenes/evidenciafotografica")]
        public async System.Threading.Tasks.Task<IHttpActionResult> UploadEvidenciaFotografica()
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
            string idVisita = string.Empty;
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
                if (e == "idVisita")
                {
                    idVisita = HttpContext.Current.Request.Form[e].ToString();
                }
            }

            if (idVisita.Length < 1)
            {
                genericClass.success = false;
                genericClass.messages.Add("Es necesario introducir el idVisita");

                return Ok(genericClass);
            }

            expedienteInformacion = await getExpedientesInformacion.GetInformationFromExpedientsXIDAsync("obrapublica", idExpediente);

            var file = HttpContext.Current.Request.Files.Count > 0 ?
        HttpContext.Current.Request.Files[0] : null;

            if (file != null && file.ContentLength > 0)
            {
                string[] extencion = file.FileName.Split('.');
                filenameUpload = "foto_" + DateTime.Now.ToString("yyyy'_'MM'_'dd'T'HH':'mm':'ss") + "." + extencion[1];

                path = expedienteInformacion.estado + "/" +
                expedienteInformacion.municipio + "/" +
                expedienteInformacion.ejercicio + "/" +
                expedienteInformacion.tipoExpediente + "/" +
                expedienteInformacion.idExpediente + "/" +
                "visita_obra" + "/" +
                idVisita + "/" +
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
                    genericClass.messages.Add("Imagen subida correctamente");


                    SaveRecordEvidenciaFotograficaMongo saveRecordEvidenciaFotograficaMongo = new SaveRecordEvidenciaFotograficaMongo();

                    if (saveRecordEvidenciaFotograficaMongo.SaveRecordVisitaObraAsync(expedienteInformacion,idVisita, path, filenameUpload, descripcion, titulo))
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
        [Route("api/expedients/obrapublica/visitaobra/evidenciafotografica/mosaico")]
        public IHttpActionResult EvidenciaFotograficaDescargaImgAsync(string idVisita)
        {
            string constr = ConfigurationManager.AppSettings["connectionString"];
            ResponseEvidenciaFotografica lrequestEvidenciaFotograficas = new ResponseEvidenciaFotografica
            {
                listResponseEvidenciaFotografica = new List<REvidenciaFotografica>()
            };

            try
            {
                var Client = new MongoClient(constr);

                var DB = Client.GetDatabase("PRB");
                var collection = DB.GetCollection<VisitaObraImagenes>("VisitaObraImagen");

                var filter = Builders<VisitaObraImagenes>.Filter.Eq(x => x.VisitaId, idVisita);

                var result = collection.Find(filter).ToList();

                if (result != null)
                {
                    foreach (var r in result)
                    {
                        REvidenciaFotografica responseEvidenciaFotografica = new REvidenciaFotografica();

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

                        responseEvidenciaFotografica.description = r.descripcion;
                        responseEvidenciaFotografica.url = blobUrl;
                        responseEvidenciaFotografica.title = r.titulo;
                        responseEvidenciaFotografica.name = r.nombre;

                        lrequestEvidenciaFotograficas.listResponseEvidenciaFotografica.Add(responseEvidenciaFotografica);

                    }

                    lrequestEvidenciaFotograficas.success = true;
                    lrequestEvidenciaFotograficas.messages.Add("Listado de imagenes generado correctamente");

                    return Ok(lrequestEvidenciaFotograficas);
                }
                
                lrequestEvidenciaFotograficas.success = false;
                lrequestEvidenciaFotograficas.messages.Add("No se encontró registro");

                return Ok(lrequestEvidenciaFotograficas);

            }
            catch (Exception ex)
            {
                lrequestEvidenciaFotograficas.success = false;
                lrequestEvidenciaFotograficas.messages.Add("Error al generar listado de imagenes");

                return Ok(lrequestEvidenciaFotograficas);
            }

        }
    }
}
