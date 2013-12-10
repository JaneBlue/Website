using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Bson.Serialization.Attributes;
namespace HeadacheCDSSWeb.Models
{
    public class mPatInfo
    {
        public mPatInfo()
         {
             
             this.refs = new List<MongoDBRef>();
             
         }
        [BsonId]
        public ObjectId PatId { get; set; }
        public string Name { get; set; }
        public string Sex { get; set; }
        public string Age { get; set; }
        public string Education { get; set; }
        public string Job { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Identity { get; set; }
        public int DoctorAccountId { get; set; }
        public string Weight { get; set; }
        public string Height { get; set; }
        public mRelatedInfo relatedInfo{get;set;}
        public List<MongoDBRef> refs { get; set; }
        public  MongoDBRef Doctor{get;set;}
    }
    public class mRelatedInfo
    {
         public mRelatedInfo()
         {
             this.Hfamilymember = new List<string>();
             this.Ofamilydisease = new List<string>();
             this.previousdrug = new List<PDrug>();
             this.previousexam = new List<Exam>();
         }

        public bool SimilarFamily { get; set; }
        public List<string> Hfamilymember;
        public List<string> Ofamilydisease;
        public List<PDrug> previousdrug { get; set; }
        public List<Exam> previousexam { get; set; }
        public lifestyle patlifestyle { get; set; }
    }
    //public class lifestyle
    //{
    //    public Nullable<bool> SmokeState { get; set; }
    //    public string SmokeYear { get; set; }
    //    public Nullable<bool> DrinkState { get; set; }
    //    public string DrinkYear { get; set; }
    //    public string TeaPerDay { get; set; }
    //    public string CoffePerDay { get; set; }
    //    public Nullable<bool> ExerciseOften { get; set; }

    //}
    //public class Exam
    //{
    //    public string ExamName { get; set; }
    //    public string Result { get; set; }
    //    public string Date { get; set; }
    //}
}