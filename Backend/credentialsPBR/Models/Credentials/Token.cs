using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace credentialsPBR.Models.Credentials
{
    public class Token
    {
        public async System.Threading.Tasks.Task<TokenCredentials> GetToken(string Username, string Password)
        {
            HttpClient httpClient = new HttpClient();

            //var responseMesssage = await httpClient.PostAsync("https://localhost:44374/token", new FormUrlEncodedContent(
            var responseMesssage = await httpClient.PostAsync("https://apipbrdevelolop.azurewebsites.net/token", new FormUrlEncodedContent(
            new[]
                {
                        new KeyValuePair<string, string>("grant_type", "password"),
                        new KeyValuePair<string, string>("username", Username),
                        new KeyValuePair<string, string>("password", Password),
                }
                ));

            var tokenModel = await responseMesssage.Content.ReadAsAsync<TokenCredentials>();

            return tokenModel;
        }
    }
}