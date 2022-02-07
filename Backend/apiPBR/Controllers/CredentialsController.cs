using apiPBR.Models.Response.Credentials;
using credentialsPBR.Models.Credentials;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Security.Claims;
using System.Web.Http;
using apiPBR.Models.Request.Credentials;
using credentialsPBR.Models.Users;
using System.Configuration;
using MongoDB.Driver;
using apiPBR.Controllers.Settings;

namespace apiPBR.Controllers
{
    public class CredentialsController : ApiController
    {
        [AllowAnonymous]
        [HttpPost]
        [Route("api/access/login")]
        public async System.Threading.Tasks.Task<LoginCredentialsResponse> AccessLogin([FromBody] LoginCredentialsRequest LoginCredentials)
        {
            LoginCredentialsResponse loginCredetialsResponse = new LoginCredentialsResponse();

            try
            {
                Token token = new Token();
                

                string constr = ConfigurationManager.AppSettings["connectionString"];
                var Client = new MongoClient(constr);

                var DB = Client.GetDatabase("PRB");
                var collection = DB.GetCollection<UsersPRB>("UsersPBR");

                var filterAcive = Builders<UsersPRB>.Filter.Eq(x => x.username, LoginCredentials.Username)
                    & Builders<UsersPRB>.Filter.Eq(x => x.password, LoginCredentials.Password)
                    & Builders<UsersPRB>.Filter.Eq(x => x.active, true);
                var result = await collection.Find(filterAcive).FirstOrDefaultAsync();

                bool verificadoActivo = false;
                bool verificadoAuthen = false;

                if (result != null)
                {
                    verificadoActivo = true;
                }

                var filter = Builders<UsersPRB>.Filter.Eq(x => x.username, LoginCredentials.Username)
                   & Builders<UsersPRB>.Filter.Eq(x => x.password, LoginCredentials.Password);
                var resultAuthen = await collection.Find(filter).FirstOrDefaultAsync();


                if (resultAuthen != null)
                {
                    verificadoAuthen = true;
                }


                if (verificadoActivo == false & verificadoAuthen == true)
                {
                    loginCredetialsResponse.name = "";
                    loginCredetialsResponse.success = false;
                    loginCredetialsResponse.messages.Add("El Usuario se encuentra desactivado, verificarlo con su Administrador");
                    loginCredetialsResponse.refresh_token = "";
                }
                else if (verificadoActivo == true & verificadoAuthen == true)
                {
                    var _token = await token.GetToken(LoginCredentials.Username, LoginCredentials.Password);

                    if (_token.access_token == null)
                    {
                        loginCredetialsResponse.name = "";
                        loginCredetialsResponse.success = false;
                        loginCredetialsResponse.messages.Add("Usuario o Contaseña incorrecta, favor de validar");
                        loginCredetialsResponse.refresh_token = "";
                    }
                    else
                    {
                        loginCredetialsResponse.success = true;
                        loginCredetialsResponse.messages.Add("Acceso correcto");
                        loginCredetialsResponse.access_token = _token.access_token;
                        loginCredetialsResponse.refresh_token = _token.refresh_token;
                        loginCredetialsResponse.expires_in = _token.expires_in;
                        loginCredetialsResponse.listAcces = (new AccesoController()).ListAccess(result.Id);
                    }
                }
                else if (verificadoActivo == true & verificadoAuthen == false)
                {
                    var _token = await token.GetToken(LoginCredentials.Username, LoginCredentials.Password);

                    if (_token.access_token == null)
                    {
                        loginCredetialsResponse.name = "";
                        loginCredetialsResponse.success = false;
                        loginCredetialsResponse.messages.Add("Usuario o Contaseña incorrecta, favor de validar");
                        loginCredetialsResponse.refresh_token = "";
                    }
                    else
                    {
                        loginCredetialsResponse.success = true;
                        loginCredetialsResponse.messages.Add("Acceso correcto");
                        loginCredetialsResponse.access_token = _token.access_token;
                        loginCredetialsResponse.refresh_token = _token.refresh_token;
                        loginCredetialsResponse.expires_in = _token.expires_in;
                        loginCredetialsResponse.listAcces = (new AccesoController()).ListAccess(result.Id);
                    }
                }
                else if (verificadoActivo == false & verificadoAuthen == false)
                {
                    loginCredetialsResponse.name = "";
                    loginCredetialsResponse.success = false;
                    loginCredetialsResponse.messages.Add("Usuario o Contaseña incorrecta, favor de validar");
                    loginCredetialsResponse.refresh_token = "";
                }
            }
            catch (Exception ex)
            {
                loginCredetialsResponse.success = false;
                loginCredetialsResponse.messages.Add(ex.ToString());

                return loginCredetialsResponse;
            }

            return loginCredetialsResponse;
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("api/access/refreshtoken")]
        public async System.Threading.Tasks.Task<IHttpActionResult> GetNewToken(string token)
        {
            RefreshToken tokenRefresh = new RefreshToken();

            var _token = await tokenRefresh.GetNewToken(token);

            return Ok(_token);
        }

        [Authorize]
        [HttpGet]
        [Route("api/data/authenticate")]
        public IHttpActionResult GetForAuthenticate()
        {
            var identity = (ClaimsIdentity)User.Identity;
            return Ok("Hello " + identity.Name);
        }

        [Authorize(Roles = "auditor")]
        [HttpGet]
        [Route("api/data/authorize")]
        public IHttpActionResult GetForAdmin()
        {
            var identity = (ClaimsIdentity)User.Identity;
            var roles = identity.Claims
                        .Where(c => c.Type == ClaimTypes.Role)
                        .Select(c => c.Value);
            return Ok("Hello " + identity.Name + " Role: " + string.Join(",", roles.ToList()));
        }
    }
}