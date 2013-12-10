using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
namespace HeadacheCDSSWeb.Models
{
    public class mUser
    {     

        public ObjectId Id { get; set; }
        public string UserName { get; set; }
        public string PassWord { get; set; }
        public string Hospital { get; set; }
        public string ChiefDoctor { get; set; }
        public string Region { get; set; }
        public string Authority { get; set; }
        
    }
}