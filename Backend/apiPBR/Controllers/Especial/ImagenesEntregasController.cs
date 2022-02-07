using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace apiPBR.Controllers.Especial
{
    public class ImagenesEntregasController : ApiController
    {
       /* [HttpPost("{id}")]
        public async Task<IActionResult> CargaImagen([FromRoute]string id, IFormFile file)
        {
            Console.WriteLine(id);

            ResponseEntregas response = new ResponseEntregas();
            try
            {
                if (file.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        var name = file.FileName;
                        var type = file.ContentType;
                        var dis = file.ContentDisposition;
                        var t = file.GetType();
                        file.CopyTo(ms);
                        ms.Position = 0;

                        string[] extencion = file.FileName.Split('.');
                        string filenameUpload = "foto_" + DateTime.Now.ToString("yyyy'_'MM'_'dd'T'HH':'mm':'ss") + "." + extencion[1];

                        Uploading uploading = new Uploading();



                        uploading.subirimagenAsync("pipas/" + id + "/" + filenameUpload, ms, extencion[1]);


                        response.success = true;
                        response.message = "OK";


                        var Client = new MongoClient("mongodb+srv://Gosword:PBRMAGICa2020@pbr-yy2eo.mongodb.net/test?retryWrites=true");

                        var DB = Client.GetDatabase("PRB");
                        var collection = DB.GetCollection<Imagenes>("ImagenesPIPA");

                        Imagenes imagenes = new Imagenes();

                        imagenes.Nombre = filenameUpload;
                        imagenes.Ruta = "pipas/" + id + "/" + filenameUpload;
                        imagenes.idEntrega = id;

                        await collection.InsertOneAsync(imagenes);

                    }

                    return Ok(response);
                }
                else
                {
                    response.success = false;
                    response.message = "Archivo malo";

                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                response.success = false;
                response.message = ex.ToString();

                return Ok(response);
            }
        }*/
    }
}
