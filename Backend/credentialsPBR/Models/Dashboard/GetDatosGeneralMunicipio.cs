using credentialsPBR.Models.Expedientes.Adquisiciones;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace credentialsPBR.Models.Dashboard
{
    public class GetDatosGeneralMunicipio
    {
        public async System.Threading.Tasks.Task<GeneralMunicipios> DatosGeneralMunicipioAsync(string municipio, string ejercicio)
        {

            string constr = ConfigurationManager.AppSettings["connectionString"];
            MongoClient Client = new MongoClient(constr);

            GeneralMunicipios generalMunicipios = new GeneralMunicipios();

            try
            {

                var DB = Client.GetDatabase("PRB");
                var collection = DB.GetCollection<GeneralMunicipios>("GeneralMunicipio");

                var filter = Builders<GeneralMunicipios>.Filter.Eq(x => x.municipio, municipio) & Builders<GeneralMunicipios>.Filter.Eq(x => x.ejercicio, ejercicio);
                
                var result = await collection.Find(filter).FirstOrDefaultAsync();

                if (result != null)
                {

                    generalMunicipios.Id = result.Id;
                    generalMunicipios.municipio = result.municipio;
                    generalMunicipios.ejercicio = result.ejercicio;
                    generalMunicipios.presupuestoAutorizadoAdquisiciones = result.presupuestoAutorizadoAdquisiciones;
                    generalMunicipios.presupuestoAutorizadoObras = result.presupuestoAutorizadoObras;

                    
                }

                return generalMunicipios;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
    }
}