using credentialsPBR.Models.Expedientes.Adquisiciones;
using credentialsPBR.Models.Expedientes.ObraPublica;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace credentialsPBR.Models.Expedientes.Utilerias
{
    public class GetExpedientesInformacion
    {
        public async System.Threading.Tasks.Task<ExpedienteInformacion> GetInformationFromExpedientsAsync(string tipo, string idExpediente)
        {
            ExpedienteInformacion expedienteInformacion = new ExpedienteInformacion();
            string constr = ConfigurationManager.AppSettings["connectionString"];
            MongoClient Client = new MongoClient(constr);
            if (tipo == "adquisiciones")
            {
                try
                {


                    var DB = Client.GetDatabase("PRB");
                    var collection = DB.GetCollection<AdquisicionesV1>("Adquisiciones");
                    var filter = Builders<AdquisicionesV1>.Filter.Eq(x => x.Id, idExpediente);

                    var result = await collection.Find(filter).FirstOrDefaultAsync();

                    if (result != null)
                    {
                        expedienteInformacion.ejercicio = result.ejercicio;
                        expedienteInformacion.estado = result.estado;
                        expedienteInformacion.idExpediente = result.numeroAdjudicacion;
                        expedienteInformacion.municipio = result.municipio;
                        expedienteInformacion.tipoExpediente = result.expediente;
                        expedienteInformacion.expediente = result.Id;
                    }

                    return expedienteInformacion;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
            else
            {
                try
                {

                    var DB = Client.GetDatabase("PRB");
                    var collection = DB.GetCollection<ObraPublicaV1>("ObraPublica");
                    var filter = Builders<ObraPublicaV1>.Filter.Eq(x => x.Id, idExpediente);

                    var result = await collection.Find(filter).FirstOrDefaultAsync();

                    if (result != null)
                    {
                        //expedienteInformacion.expediente = result.numero
                        expedienteInformacion.ejercicio = result.ejercicio;
                        expedienteInformacion.estado = result.estado;
                        expedienteInformacion.idExpediente = result.numeroProcedimiento;
                        expedienteInformacion.municipio = result.municipio;
                        expedienteInformacion.tipoExpediente = result.tipoExpediente;
                        expedienteInformacion.expediente = result.Id;
                    }

                    return expedienteInformacion;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }

        }
        public async System.Threading.Tasks.Task<ExpedienteInformacion> GetInformationFromExpedientsXIDAsync(string tipo, string idExpediente)
        {
            ExpedienteInformacion expedienteInformacion = new ExpedienteInformacion();
            string constr = ConfigurationManager.AppSettings["connectionString"];
            MongoClient Client = new MongoClient(constr);
            if (tipo == "adquisiciones")
            {
                try
                {


                    var DB = Client.GetDatabase("PRB");
                    var collection = DB.GetCollection<AdquisicionesV1>("Adquisiciones");
                    var filter = Builders<AdquisicionesV1>.Filter.Eq(x => x.Id, idExpediente);

                    var result = await collection.Find(filter).FirstOrDefaultAsync();

                    if (result != null)
                    {
                        expedienteInformacion.ejercicio = result.ejercicio;
                        expedienteInformacion.estado = result.estado;
                        expedienteInformacion.idExpediente = result.numeroAdjudicacion;
                        expedienteInformacion.municipio = result.municipio;
                        expedienteInformacion.tipoExpediente = result.expediente;
                        expedienteInformacion.expediente = result.Id;
                    }

                    return expedienteInformacion;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
            else
            {
                try
                {

                    var DB = Client.GetDatabase("PRB");
                    var collection = DB.GetCollection<ObraPublicaV1>("ObraPublica");
                    var filter = Builders<ObraPublicaV1>.Filter.Eq(x => x.Id, idExpediente);

                    var result = await collection.Find(filter).FirstOrDefaultAsync();

                    if (result != null)
                    {
                        expedienteInformacion.ejercicio = result.ejercicio;
                        expedienteInformacion.estado = result.estado;
                        expedienteInformacion.idExpediente = result.numeroProcedimiento;
                        expedienteInformacion.municipio = result.municipio;
                        expedienteInformacion.tipoExpediente = result.tipoExpediente;
                        expedienteInformacion.expediente = result.Id;
                    }

                    return expedienteInformacion;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }

        }
    }
}