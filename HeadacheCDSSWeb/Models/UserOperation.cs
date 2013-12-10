using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity.Validation;
using MongoDB.Bson;
using MongoDB.Driver;
namespace HeadacheCDSSWeb.Models
{
    public class UserOperation
    {
        
        public bool ValidateUser(String User, String Password)
        {
            try
            {
                string connectionString = "mongodb://localhost:27017";
                MongoClient client = new MongoClient(connectionString);
                MongoServer server = client.GetServer();
                MongoDatabase db = server.GetDatabase("hdb");
            
                using (server.RequestStart(db))
                {
                    //db.DropCollection("Users");
                    //db.DropCollection("Pats");
                 var Usercollection = db.GetCollection<mUser>("Users");
                    //mUser u = new mUser();
                    //u.UserName = "123";
                    //u.PassWord = "123";
                    //Usercollection.Insert(u);
                  
                    QueryDocument query = new QueryDocument();
                   BsonDocument b = new BsonDocument();
                   b.Add("UserName", User);
                   b.Add("PassWord", Password);
                   query.Add(b);
                    mUser user = Usercollection.FindOne(query);
                    if (user == null)
                    {
                        return false;
                    }
                    return true;
                }
            }
            catch (System.Exception ex)
            {
                return false;
            }

        }

    }
}