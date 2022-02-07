using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace credentialsPBR.Models.Expedientes.Utilerias
{
    public class GetInformationFromComplementarioExpedientes
    {
        public async System.Threading.Tasks.Task<InfoComplementariaExpedientes> InformacionComplementariaExpedientesPublicaAsync(string idExpediente, string tipoExpediente)
        {

            InfoComplementariaExpedientes infoComplementariaExpedientes = new InfoComplementariaExpedientes();
            string constr = ConfigurationManager.AppSettings["connectionString"];
            try
            {
                MongoClient Client = new MongoClient(constr);

                var DB = Client.GetDatabase("PRB");
                var collectionComplemento = DB.GetCollection<ComplementoExpediente>("ComplementoExpedientes");

                var filter = Builders<ComplementoExpediente>.Filter.Eq(x=>x.idExpediente, idExpediente) & Builders<ComplementoExpediente>.Filter.Eq(x => x.tipoExpediente, tipoExpediente);

                var result = await collectionComplemento.Find(filter).FirstOrDefaultAsync();

                if (result != null)
                {
                    if (result.Location.Coordinates != null & result.Location.Coordinates.Count > 0)
                    {
                        infoComplementariaExpedientes.latitud = result.Location.Coordinates[1].ToString() == null ? "" : result.Location.Coordinates[1].ToString();
                        infoComplementariaExpedientes.longitud = result.Location.Coordinates[0].ToString() == null ? "" : result.Location.Coordinates[0].ToString();
                        
                    }
                    else
                    {
                        infoComplementariaExpedientes.latitud = null;
                        infoComplementariaExpedientes.longitud = null;
                    }
                    infoComplementariaExpedientes.estatusExpediente = result.estatus == null ? "CARGADO" : result.estatus;

                }
                else
                {
                    infoComplementariaExpedientes = null;
                }


                return infoComplementariaExpedientes;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}