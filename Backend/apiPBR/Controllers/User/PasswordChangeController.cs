
using apiPBR.Models.Request.Credentials;
using apiPBR.Models.Response.Credentials;
using apiPBR.Models.Response.UserPBR;
using credentialsPBR.Models.Users;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Management;

namespace apiPBR.Controllers.User
{
    public class PasswordChangeController : ApiController
    {

        private int longitudPassword = 8;

        [Authorize(Roles = "Auditor, Coordinador, Ejecutivo")]
        [HttpPost]
        [Route("api/password/change")]
        public async System.Threading.Tasks.Task<PasswordChangeResponse> AccessLogin([FromBody] PasswordChangeRequest requestPassword)
        {
            PasswordChangeResponse response = new PasswordChangeResponse();
            var userName = string.Empty;
            var identity = (ClaimsIdentity)User.Identity;
            IEnumerable<Claim> claims = identity.Claims;
            Common.Common common = new Common.Common();

            foreach (var c in claims)
            {
                if (c.Type == "username")
                {
                    userName = c.Value;
                    break;
                }               
            }


            if(!requestPassword.NewPassword.Equals(""))
            {
                if (!common.IsValidCaracterEspecial(requestPassword.NewPassword))
                {
                    response.success = false;
                    response.messages.Add("La nueva contraseña no cu");
                    return response;
                }

                if (common.hasSpecialChar(requestPassword.NewPassword))
                {
                    response.success = false;
                    response.messages.Add("La nueva contraseña no debe tener caracteres especiales");
                    return response;
                }

                if (!common.IsValidLength(requestPassword.NewPassword, longitudPassword))
                {
                    response.success = false;
                    response.messages.Add("La nueva contraseña excede los 8 caracteres permitidos");
                    return response;
                }

               

            }




            try
            {
                string constr = ConfigurationManager.AppSettings["connectionString"];
                var Client = new MongoClient(constr);

                var DB = Client.GetDatabase("PRB");
                var collection = DB.GetCollection<UsersPRB>("UsersPBR");

                var filter = Builders<UsersPRB>.Filter.Eq(x => x.username, userName)
                    & Builders<UsersPRB>.Filter.Eq(x => x.password, requestPassword.Password);
                var result = await collection.Find(filter).FirstOrDefaultAsync();

                if(result != null)
                {
                    var filterUpdate = Builders<UsersPRB>.Filter.Eq(x => x.Id, result.Id);
                    result.password = requestPassword.NewPassword;
                    await collection.ReplaceOneAsync(filter, result);
                    response.success = true;
                }
                else
                {
                    response.success = false;
                    response.messages.Add("El Password no es el correcto");
                    return response;
                }

            }
            catch (Exception ex)
            {
                response.success = false;
                response.messages.Add(ex.ToString());

                return response;
            }

            return response;
        }


        [Authorize(Roles = "Auditor, Coordinador, Ejecutivo")]
        [HttpPost]
        [Route("api/password/change")]
        public async System.Threading.Tasks.Task<PasswordChangeResponse> ChangeAccessLogin([FromBody] PasswordChangeRequest requestPassword)
        {
            PasswordChangeResponse response = new PasswordChangeResponse();
            var userName = string.Empty;
            var identity = (ClaimsIdentity)User.Identity;
            IEnumerable<Claim> claims = identity.Claims;
            Common.Common common = new Common.Common();

            foreach (var c in claims)
            {
                if (c.Type == "username")
                {
                    userName = c.Value;
                    break;
                }
            }


            if (!requestPassword.NewPassword.Equals(""))
            {
                if (!common.IsValidCaracterEspecial(requestPassword.NewPassword))
                {
                    response.success = false;
                    response.messages.Add("La nueva contraseña no cumple con los parametros requeridos");
                    return response;
                }

                if (common.hasSpecialChar(requestPassword.NewPassword))
                {
                    response.success = false;
                    response.messages.Add("La nueva contraseña no debe tener caracteres especiales");
                    return response;
                }

                if (!common.IsValidLength(requestPassword.NewPassword, longitudPassword))
                {
                    response.success = false;
                    response.messages.Add("La nueva contraseña excede los 8 caracteres permitidos");
                    return response;
                }



            }




            try
            {
                string constr = ConfigurationManager.AppSettings["connectionString"];
                var Client = new MongoClient(constr);

                var DB = Client.GetDatabase("PRB");
                var collection = DB.GetCollection<UsersPRB>("UsersPBR");

                var filter = Builders<UsersPRB>.Filter.Eq(x => x.username, userName)
                    & Builders<UsersPRB>.Filter.Eq(x => x.password, requestPassword.Password);
                var result = await collection.Find(filter).FirstOrDefaultAsync();

                if (result != null)
                {
                    var filterUpdate = Builders<UsersPRB>.Filter.Eq(x => x.Id, result.Id);
                    result.password = requestPassword.NewPassword;
                    await collection.ReplaceOneAsync(filter, result);
                    response.success = true;
                }
                else
                {
                    response.success = false;
                    response.messages.Add("El Password no es el correcto");
                    return response;
                }

            }
            catch (Exception ex)
            {
                response.success = false;
                response.messages.Add(ex.ToString());

                return response;
            }

            return response;
        }

        [Authorize(Roles = "Auditor, Coordinador, Ejecutivo")]
        [HttpPut]
        [Route("api/configuration/password/change/password")]
        public async System.Threading.Tasks.Task<PasswordChangeResponse> PasswordChange([FromBody] UserPasswordChangeRequest requestPassword)
        {
            PasswordChangeResponse response = new PasswordChangeResponse();
            var userName = string.Empty;
            var identity = (ClaimsIdentity)User.Identity;
            IEnumerable<Claim> claims = identity.Claims;
            Common.Common common = new Common.Common();

            foreach (var c in claims)
            {
                if (c.Type == "username")
                {
                    userName = c.Value;
                    break;
                }
            }

            if (!requestPassword.Password.Equals(""))
            {
                if (!common.ValidatePassword(requestPassword.Password, out string ErrorMessage))
                {
                    response.success = false;
                    response.messages.Add(ErrorMessage);
                    return response;
                }
            }
            else
            {
                response.success = false;
                response.messages.Add("No ha ingresado la contraseña");
                return response;
            }

            if (requestPassword.UserName.Equals(""))
            {
                response.success = false;
                response.messages.Add("No ha ingresado el Usuario");
                return response;
            }

            try
            {
                string constr = ConfigurationManager.AppSettings["connectionString"];
                var Client = new MongoClient(constr);

                var DB = Client.GetDatabase("PRB");
                var collection = DB.GetCollection<UsersPRB>("UsersPBR");

                var filter = Builders<UsersPRB>.Filter.Eq(x => x.username, requestPassword.UserName);
                var result = await collection.Find(filter).FirstOrDefaultAsync();

                if (result != null)
                {
                    var filterUpdate = Builders<UsersPRB>.Filter.Eq(x => x.Id, result.Id);
                    result.password = requestPassword.Password;
                    await collection.ReplaceOneAsync(filter, result);
                    response.success = true;
                }
                else
                {
                    response.success = false;
                    response.messages.Add("El usuario no se encontro, intento nuevamente");
                    return response;
                }

            }
            catch (Exception ex)
            {
                response.success = false;
                response.messages.Add(ex.ToString());

                return response;
            }

            return response;
        }

        [Authorize(Roles = "Auditor, Coordinador, Ejecutivo")]
        [HttpGet]
        [Route("api/configuration/list/users")]
        public async System.Threading.Tasks.Task<UsersResponse> ListUsers()
        {
            UsersResponse response = new UsersResponse();
            var userName = string.Empty;
            var municipio = string.Empty;
            var identity = (ClaimsIdentity)User.Identity;
            IEnumerable<Claim> claims = identity.Claims;
            Common.Common common = new Common.Common();

            foreach (var c in claims)
            {
                if (c.Type == "username")
                {
                    userName = c.Value;
                    break;
                }
            }

            foreach (var c in claims)
            {
                if (c.Type == "municipio")
                {
                    municipio = c.Value;
                    break;
                }
            }

            try
            {
                string constr = ConfigurationManager.AppSettings["connectionString"];
                var Client = new MongoClient(constr);

                var DB = Client.GetDatabase("PRB");
                var collection = DB.GetCollection<UsersPRB>("UsersPBR");
                //.Find(new BsonDocument()).ToListAsync();

                var filter = Builders<UsersPRB>.Filter.Eq(x => x.municipio, municipio);
                var result = await collection.Find(filter).ToListAsync();
                List<UserActivation> ListUsers = new List<UserActivation>();
                UserActivation user = new UserActivation();

                if (result != null)
                {
                    foreach(var data in result.ToList())
                    {
                        user = new UserActivation();
                        user.Id = data.Id;
                        user.UserName = data.username;
                        user.Name = data.name;
                        user.Active = data.active;

                        ListUsers.Add(user);
                    }
                    response.success = true;
                    response.ListUsers = ListUsers;
                }
                else
                {
                    response.success = false;
                    response.messages.Add("No se encontraron resultados, intentalo mas tarde");
                    return response;
                }

            }
            catch (Exception ex)
            {
                response.success = false;
                response.messages.Add(ex.ToString());

                return response;
            }

            return response;
        }

        [Authorize(Roles = "Auditor, Coordinador, Ejecutivo")]
        [HttpPut]
        [Route("api/configuration/change/status")]
        public async System.Threading.Tasks.Task<UsersActiveResponse> ActiveUser([FromBody] UserActiveRequest requestUser)
        {
            UsersActiveResponse response = new UsersActiveResponse();
            var userName = string.Empty;
            var identity = (ClaimsIdentity)User.Identity;
            IEnumerable<Claim> claims = identity.Claims;
            Common.Common common = new Common.Common();

            foreach (var c in claims)
            {
                if (c.Type == "username")
                {
                    userName = c.Value;
                    break;
                }
            }

            if (requestUser.Id.Equals(""))
            {
                response.success = false;
                response.messages.Add("No ha ingresado el Id del usuario");
                return response;
            }

            try
            {
                string constr = ConfigurationManager.AppSettings["connectionString"];
                var Client = new MongoClient(constr);

                var DB = Client.GetDatabase("PRB");
                var collection = DB.GetCollection<UsersPRB>("UsersPBR");

                var filter = Builders<UsersPRB>.Filter.Eq(x => x.Id, requestUser.Id);
                var result = await collection.Find(filter).FirstOrDefaultAsync();

                if (result != null)
                {
                    var filterUpdate = Builders<UsersPRB>.Filter.Eq(x => x.Id, result.Id);
                    result.active = requestUser.Active;
                    await collection.ReplaceOneAsync(filter, result);
                    response.success = true;
                }
                else
                {
                    response.success = false;
                    response.messages.Add("El usuario no se encontro, intento nuevamente");
                    return response;
                }

            }
            catch (Exception ex)
            {
                response.success = false;
                response.messages.Add(ex.ToString());

                return response;
            }

            return response;
        }

    }
}
