using credentialsPBR.Models.Expedientes.ObraPublica;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace credentialsPBR.Models.Expedientes.Utilerias
{
    public class SaveRecordEvidenciaFotograficaMongo
    {
        public bool SaveRecordAsync(ExpedienteInformacion expedienteInformacion, string path, string name, string descripcion, string titulo, string idUser)
        {
            EvidenciaFotografica evidenciaFotografica = new EvidenciaFotografica();
            string constr = ConfigurationManager.AppSettings["connectionString"];
            MongoClient Client = new MongoClient(constr);
                try
                {

                evidenciaFotografica.descripcion = descripcion;
                evidenciaFotografica.ejercicio = expedienteInformacion.ejercicio;
                evidenciaFotografica.fecha = DateTime.Now;
                evidenciaFotografica.idExpediente = expedienteInformacion.expediente;
                evidenciaFotografica.idUser = idUser;
                evidenciaFotografica.municipio = expedienteInformacion.municipio;
                evidenciaFotografica.nombre = name;
                evidenciaFotografica.path = path;
                evidenciaFotografica.titulo = titulo;
                
                    var DB = Client.GetDatabase("PRB");
                    var collection = DB.GetCollection<EvidenciaFotografica>("EvidenciaFotografica");

                    collection.InsertOne(evidenciaFotografica);
    

                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }

        }

        public bool SaveRecordAnexoAsync(ExpedienteInformacion expedienteInformacion, string path, string name, string descripcion, string titulo, string idUser)
        {
            EvidenciaFotografica evidenciaFotografica = new EvidenciaFotografica();
            string constr = ConfigurationManager.AppSettings["connectionString"];
            MongoClient Client = new MongoClient(constr);
            try
            {

                evidenciaFotografica.descripcion = descripcion;
                evidenciaFotografica.ejercicio = expedienteInformacion.ejercicio;
                evidenciaFotografica.fecha = DateTime.Now;
                evidenciaFotografica.idExpediente = expedienteInformacion.expediente;
                evidenciaFotografica.idUser = idUser;
                evidenciaFotografica.municipio = expedienteInformacion.municipio;
                evidenciaFotografica.nombre = name;
                evidenciaFotografica.path = path;
                evidenciaFotografica.titulo = titulo;

                var DB = Client.GetDatabase("PRB");
                var collection = DB.GetCollection<EvidenciaFotografica>("Anexo");

                collection.InsertOne(evidenciaFotografica);


                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public bool SaveRecordVisitaObraAsync(ExpedienteInformacion expedienteInformacion, string idVisita, string path, string name, string descripcion, string titulo)
        {
            VisitaObraImagenes evidenciaFotografica = new VisitaObraImagenes();
            string constr = ConfigurationManager.AppSettings["connectionString"];
            MongoClient Client = new MongoClient(constr);
            try
            {
                evidenciaFotografica.VisitaId = idVisita;
                evidenciaFotografica.descripcion = descripcion;
                evidenciaFotografica.fecha = DateTime.Now;
                evidenciaFotografica.ExpedienteId = expedienteInformacion.expediente;
                evidenciaFotografica.nombre = name;
                evidenciaFotografica.path = path;
                evidenciaFotografica.titulo = titulo;

                var DB = Client.GetDatabase("PRB");
                var collection = DB.GetCollection<VisitaObraImagenes>("VisitaObraImagen");

                collection.InsertOne(evidenciaFotografica);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
    }
}