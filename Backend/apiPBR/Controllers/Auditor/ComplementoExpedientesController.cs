using apiPBR.Models.Request.Auditors;
using apiPBR.Models.Response;
using apiPBR.Models.Response.Expedientes.ObraPublica;
using credentialsPBR.Models.Expedientes.Adquisiciones;
using credentialsPBR.Models.Expedientes.ObraPublica;
using credentialsPBR.Models.Expedientes.Utilerias;
using credentialsPBR.Models.Users;
using MongoDB.Driver;
using MongoDB.Driver.GeoJsonObjectModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace apiPBR.Controllers.Auditor
{
    public class ComplementoExpedientesController : ApiController
    {
        [Authorize(Roles = "Auditor, Coordinador, Ejecutivo")]
        [HttpPost]
        [Route("api/expedients/complementaria/{tipoExpediente}/{id}")]
        public async System.Threading.Tasks.Task<IHttpActionResult> AgregaInformacionComplementariaExpediente([FromBody]InformacionComplementariaExpediente informacionComplementariaExpediente, string tipoExpediente, string id)
        {
            GenericClass genericClass = new GenericClass();

            string constr = ConfigurationManager.AppSettings["connectionString"];
            
            try
            {
                var Client = new MongoClient(constr);

                var DB = Client.GetDatabase("PRB");

                              
                     var collectionComplemento = DB.GetCollection<ComplementoExpediente>("ComplementoExpedientes");

                    var filterComplemento = Builders<ComplementoExpediente>.Filter.Eq(x => x.idExpediente, id) & Builders<ComplementoExpediente>.Filter.Eq(x => x.tipoExpediente, tipoExpediente);

                    var resultComplemento = await collectionComplemento.Find(filterComplemento).FirstOrDefaultAsync();

                    if (resultComplemento != null)
                    {
                        ComplementoExpediente complementoExpediente = new ComplementoExpediente
                        {
                            Location = new Coordenate
                            {
                                Coordinates = new List<double>()
                            }
                        };

                        complementoExpediente.Id = resultComplemento.Id;

                        if (informacionComplementariaExpediente.latitud != null & informacionComplementariaExpediente.longitud != null)
                        {
                            complementoExpediente.Location.Type = "Point";
                            complementoExpediente.Location.Coordinates.Add(Convert.ToDouble(informacionComplementariaExpediente.longitud));
                            complementoExpediente.Location.Coordinates.Add(Convert.ToDouble(informacionComplementariaExpediente.latitud));
                        }
                        if (informacionComplementariaExpediente.estatusExpediente != null)
                        {
                            complementoExpediente.estatus = informacionComplementariaExpediente.estatusExpediente;
                        }
                        else
                        {
                            if (resultComplemento.estatus != null)
                            {
                                complementoExpediente.estatus = resultComplemento.estatus;
                            }
                            else
                            {
                                complementoExpediente.estatus = "CARGANDO";
                            }
                        }

                        if (informacionComplementariaExpediente.avanceDocumental != null)
                        {
                            complementoExpediente.porcentajeAvance = Convert.ToInt32(informacionComplementariaExpediente.avanceDocumental);
                        }
                        

                    complementoExpediente.idExpediente = id;
                        complementoExpediente.tipoExpediente = tipoExpediente;

                        await collectionComplemento.ReplaceOneAsync(filterComplemento, complementoExpediente);

                        genericClass.success = true;
                        genericClass.messages.Add("Registro generado con existo");

                        return Ok(genericClass);
                    }
                    else
                    {
                        ComplementoExpediente complementoExpediente = new ComplementoExpediente
                        {
                            Location = new Coordenate
                            {
                                Coordinates = new List<double>()
                            }
                        };

                        
                        if (informacionComplementariaExpediente.latitud != null & informacionComplementariaExpediente.longitud != null)
                        {
                            complementoExpediente.Location.Type = "Point";
                            complementoExpediente.Location.Coordinates.Add(Convert.ToDouble(informacionComplementariaExpediente.longitud));
                            complementoExpediente.Location.Coordinates.Add(Convert.ToDouble(informacionComplementariaExpediente.latitud));
                        }
                        else
                        {
                            complementoExpediente.Location = null;
                        }
                        if (informacionComplementariaExpediente.estatusExpediente != null)
                        {
                            complementoExpediente.estatus = informacionComplementariaExpediente.estatusExpediente;
                        }
                        else
                        {
                            complementoExpediente.estatus = "CARGADO";
                        }
                        if (informacionComplementariaExpediente.avanceDocumental != null)
                        {
                            complementoExpediente.porcentajeAvance = Convert.ToInt32(informacionComplementariaExpediente.avanceDocumental);
                        }
                        else
                        {
                            complementoExpediente.porcentajeAvance = 0;
                        }
                        complementoExpediente.tipoExpediente = tipoExpediente;
                        complementoExpediente.idExpediente = id;

                        await collectionComplemento.InsertOneAsync(complementoExpediente);

                        genericClass.success = true;
                        genericClass.messages.Add("Registro generado con existo");

                        return Ok(genericClass);

                    }

            }
            catch (Exception ex)
            {
                genericClass.success = false;
                genericClass.messages.Add(ex.ToString());

                return Ok(genericClass);
            }

        }


        GetUserInfo getUserInfo = new GetUserInfo();

        [Authorize(Roles = "Auditor, Coordinador, Ejecutivo")]
        [HttpGet]
        [Route("api/expedients/complementaria/coordenadas/obrapublica/{ejercicio}")]
        public async System.Threading.Tasks.Task<IHttpActionResult> CoordenadasGPSObraPublicaAsync(string ejercicio)
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

            CoordenadasObraPublica coordenadasObraPublica = new CoordenadasObraPublica
            {
                listaCoordenadasObraOublicas = new List<CoordenadaObraOublica>()
            };

            string constr = ConfigurationManager.AppSettings["connectionString"];
            try
            {
                MongoClient Client = new MongoClient(constr);

                UsersPRB userPBR = new UsersPRB();
                userPBR = await getUserInfo.GetInfoUserPBRAsync(idUsername, Client);

                var DB = Client.GetDatabase("PRB");
                var collection = DB.GetCollection<ObraPublicaV1>("ObraPublica");
                var collectionComplemento = DB.GetCollection<ComplementoExpediente>("ComplementoExpedientes");

                var filter = Builders<ObraPublicaV1>.Filter.Eq("municipio", userPBR.municipio) &
                    Builders<ObraPublicaV1>.Filter.Eq(x => x.ejercicio, ejercicio); ;

                if (role == "Auditor")
                {
                    filter = Builders<ObraPublicaV1>.Filter.Eq("auditor", userPBR.Id.ToString()) &
                    Builders<ObraPublicaV1>.Filter.Eq(x => x.ejercicio, ejercicio);
                }

                var result = await collection.Find(filter).ToListAsync();

                List<CoordenadaObraOublica> l_coordenadaObraOublica = new List<CoordenadaObraOublica>();
                foreach (var r in result)
                {
                    
                    var filterComplemento = Builders<ComplementoExpediente>.Filter.Eq(x => x.Id, r.Id);

                    var resultComplemento = await  collectionComplemento.Find(filterComplemento).FirstOrDefaultAsync();

                    if (resultComplemento != null)
                    {
                        CoordenadaObraOublica coordenadaObraOublica = new CoordenadaObraOublica();
                        
                        coordenadaObraOublica.id = resultComplemento.Id;
                        coordenadaObraOublica.latitud = resultComplemento.Location.Coordinates[1].ToString();
                        coordenadaObraOublica.longitud = resultComplemento.Location.Coordinates[0].ToString();

                        l_coordenadaObraOublica.Add(coordenadaObraOublica);
                    }
                   
                }

                coordenadasObraPublica.success = true;
                coordenadasObraPublica.messages.Add("Respuesta exitosa");
                coordenadasObraPublica.listaCoordenadasObraOublicas = l_coordenadaObraOublica;


                return Ok(coordenadasObraPublica);
            }
            catch (Exception ex)
            {
                coordenadasObraPublica.success = false;
                coordenadasObraPublica.messages.Add(ex.ToString());

                return Ok(coordenadasObraPublica);
            }
        }


    }
}
