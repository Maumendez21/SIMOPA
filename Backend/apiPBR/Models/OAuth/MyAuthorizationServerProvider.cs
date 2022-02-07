

using credentialsPBR.Models.Users;
using Microsoft.Owin.Security.OAuth;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace apiPBR.Models.OAuth
{
    public class MyAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated(); // 
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            //CredentialsDAO credentialsDAO = new CredentialsDAO();
            //AuthenticationRequest authenticationRequestDAO = new AuthenticationRequest();

            //authenticationRequestDAO.Login = context.UserName;
            //authenticationRequestDAO.Password = context.Password;
            string constr = ConfigurationManager.AppSettings["connectionString"];
            try
            {
                var Client = new MongoClient(constr);

                var DB = Client.GetDatabase("PRB");
                var collection = DB.GetCollection<UsersPRB>("UsersPBR");

                var filter = Builders<UsersPRB>.Filter.Eq("username", context.UserName) & Builders<UsersPRB>.Filter.Eq("password", context.Password);

                var result = await collection.Find(filter).FirstOrDefaultAsync();

                if (result != null)
                {
                    var identity = new ClaimsIdentity(context.Options.AuthenticationType);

                    identity.AddClaim(new Claim(ClaimTypes.Role, result.role));
                    identity.AddClaim(new Claim("username", context.UserName));
                    identity.AddClaim(new Claim("id", result.Id));
                    identity.AddClaim(new Claim("municipio", result.municipio));
                    identity.AddClaim(new Claim("name",result.name));
                    //identity.AddClaim(new Claim(ClaimTypes.Name, "Sourav Mondal"));
                    context.Validated(identity);

                    return;
                }
                else
                {
                    context.SetError("invalid_grant", "Usuario o contraseña invalida, favor de verificar");
                    return;
                }
            }
            catch (Exception ex)
            {
                //response.Success = false;
                //response.Messages.Add(ex.Message);
                //return response;
                context.SetError(ex.Message);
            }
        }
    }
}