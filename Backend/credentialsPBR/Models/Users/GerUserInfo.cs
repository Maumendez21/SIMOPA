using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace credentialsPBR.Models.Users
{
    public class GetUserInfo
    {
        public async System.Threading.Tasks.Task<UsersPRB> GetInfoUserPBRAsync(string id, MongoClient Client)
        {
            var DB = Client.GetDatabase("PRB");
            var collection = DB.GetCollection<UsersPRB>("UsersPBR");

            var filter = Builders<UsersPRB>.Filter.Eq(x=>x.Id, id);

            var result = await collection.Find(filter).FirstOrDefaultAsync();

            if (result != null)
                return result;

            return null;
        }
    }
}