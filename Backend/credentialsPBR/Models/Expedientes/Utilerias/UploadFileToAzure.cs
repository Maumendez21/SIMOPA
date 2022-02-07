using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.File;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace credentialsPBR.Models.Expedientes.Utilerias
{
    public class UploadFileToAzure
    {
        private CloudFileDirectory directorio;
        /*public UploadFileToAzure()
        {
            try
            {
                string keys = @"DefaultEndpointsProtocol=https;AccountName=storagepbr;AccountKey=mWminVy4acvZseDV7bt4ZVKA2w8ZA4nRr1LJ00aA0WFSW92P6/Hl4/DLZSOWNoRD+X5LkfSCttIEUYO1N1dpaA==;EndpointSuffix=core.windows.net";
                CloudStorageAccount cuenta = CloudStorageAccount.Parse(keys);
                CloudFileClient cliente = cuenta.CreateCloudFileClient();
                CloudFileShare recurso = cliente.GetShareReference("bancodocumentos");
                directorio = recurso.GetRootDirectoryReference();
            }
            catch (Exception ex)
            {
            }
        }*/

        public bool UploadFileAzure(string nObjeto, Stream fileStream)
        {
            try
            {
                // Retrieve storage account from connection string.
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=storagepbr;AccountKey=mWminVy4acvZseDV7bt4ZVKA2w8ZA4nRr1LJ00aA0WFSW92P6/Hl4/DLZSOWNoRD+X5LkfSCttIEUYO1N1dpaA==;EndpointSuffix=core.windows.net");

                // Create the blob client.
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                // Retrieve reference to a previously created container.
                CloudBlobContainer container = blobClient.GetContainerReference("bancodocumentos");

                //CloudBlobDirectory directory = container.createdo("tepeaca");


                // Retrieve reference to a blob named "myblob".
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(nObjeto);

                // Create or overwrite the "myblob" blob with contents from a local file.  
                //using (var fileStream = System.IO.File.OpenRead(fullname))
                //{
                    blockBlob.UploadFromStream(fileStream);
                
                //}

                //File.Delete(nombre);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool DeleteFileAzure(string nObjeto)
        {
            try
            {
                // Retrieve storage account from connection string.
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=storagepbr;AccountKey=mWminVy4acvZseDV7bt4ZVKA2w8ZA4nRr1LJ00aA0WFSW92P6/Hl4/DLZSOWNoRD+X5LkfSCttIEUYO1N1dpaA==;EndpointSuffix=core.windows.net");

                // Create the blob client.
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                // Retrieve reference to a previously created container.
                CloudBlobContainer container = blobClient.GetContainerReference("bancodocumentos");

                //CloudBlobDirectory directory = container.createdo("tepeaca");


                // Retrieve reference to a blob named "myblob".
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(nObjeto);

                // Create or overwrite the "myblob" blob with contents from a local file.  
                //using (var fileStream = System.IO.File.OpenRead(fullname))
                //{
                blockBlob.DeleteIfExists();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}