using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;
using System.Data.Entity.Validation;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
namespace HeadacheCDSSWeb.Models
{
     
    public class VisitDataOperation
    {
        HeadacheModelContainer context = new HeadacheModelContainer();
        static public MongoServer ConnectDB()
        {
            string connectionString = "mongodb://localhost:27017";
            MongoClient client = new MongoClient(connectionString);
            MongoServer server = client.GetServer();
           
            return server;
        }
        public List<mPatInfo> GetPat(List<string> Condition)
        {
            List<mPatInfo> Pats = new List<mPatInfo>();
            List<mPatInfo> SelectedPats = new List<mPatInfo>();
            List<mPatInfo> Unormalpat = new List<mPatInfo>();
            try
            {

               MongoServer server= ConnectDB();
               MongoDatabase db = server.GetDatabase("hdb");
                using (server.RequestStart(db))
                {
                    //对象打点添加 
                    //QueryDocument query = new QueryDocument("Name",Condition.Last());
                    //var Usercollection = db.GetCollection<mUser>("Users");
                    //mUser User= Usercollection.FindOne(query);
                    BsonDocument b = new BsonDocument();
                    int conditionflag = 0;
                    if (Condition[0] != "")
                    {
                        b.Add("Name", Condition[0]);
                        conditionflag++;
                    }
                    if (Condition[1] != "")
                    {
                        b.Add("Sex", Condition[1]);
                        conditionflag++;
                    }


                    var PCollection = db.GetCollection<mPatInfo>("Pats");
                    if (!(Condition[2] != "" && Condition[3] != ""))
                    {
                        if (conditionflag != 0)
                        {
                            QueryDocument query = new QueryDocument();
                            query.Add(b);
                            foreach (mPatInfo p in PCollection.FindAs<mPatInfo>(query))
                            {
                               
                                var u = db.FetchDBRef(p.Doctor);
                                if (u["UserName"].ToString() == Condition[4])
                                {
                                    Pats.Add(p);
                                }
                            }

                            foreach (mPatInfo p in Pats)
                            {
                                if (p.refs.Count != 0)
                                {
                                    var visit = db.FetchDBRef(p.refs.Last());
                                    bool m = (Condition[2] == "") ? true : (visit["VisitDate"].ToString() == Condition[2]);
                                    bool n = (Condition[3] == "") ? true : (visit["DiagnosisResult1"].ToString() == Condition[3]);
                                    if (!(m && n))
                                    {
                                        Pats.Remove(p);
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (mPatInfo p in PCollection.FindAllAs<mPatInfo>())
                            {
                                if (p.refs.Count != 0)
                                {
                                    var visit = db.FetchDBRef(p.refs.Last());
                                    bool m = (Condition[2] == "") ? true : (visit["VisitDate"].ToString() == Condition[2]);
                                    bool n = (Condition[3] == "") ? true : (visit["DiagnosisResult1"].ToString() == Condition[3]);
                                    if (m && n)
                                    {
                                        Pats.Add(p);
                                    }
                                }
                            }
                        }
                    }






                    if (Pats != null)
                    {

                        foreach (mPatInfo pt in Pats)
                        {
                            if (pt.refs.Count != 0)
                            {
                                SelectedPats.Add(pt);
                            }
                            else
                            {
                                Unormalpat.Add(pt);
                            }
                        }

                        //if (string.IsNullOrEmpty(Condition[2]))
                        //{
                        //    InsertSort(SelectedPats);
                           
                        //}
                        SelectedPats.AddRange(Unormalpat);
                    }
                }
            }
            catch (Exception e)
            {
                string error = e.Message;

            }
            return SelectedPats;
        }
         public string InsertPat(mPatInfo pat, string User)
         {
             MongoServer server = ConnectDB();
             var db = server.GetDatabase("hdb");
             string pid = "";
            try
            {
                using (server.RequestStart(db))
                {
                    
                    var Usercollection = db.GetCollection<mUser>("Users");

                    var query = new QueryDocument("UserName", User);
                    //mUser u = new mUser();
                    //u.UserName = "123";
                    //u.PassWord = "123";
                    mUser entity = Usercollection.FindOne(query);
                    MongoDBRef r = new MongoDBRef("Users", entity.Id);
                  
                    var PatCollection = db.GetCollection<mPatInfo>("Pats");
                    pat.Doctor = r;
                    PatCollection.Insert(pat);
                    pid = pat.PatId.ToString();
                    //collection.Remove(query);
                    
                  


                }
                return pid;
            }
            catch (System.Exception ex)
            {
                return pid;
            }
            
            
         }
        //public static void InsertSort(List<mPatInfo> data)
        //{
        //    var count = data.Count;
        //    for (int i = 1; i < count; i++)
        //    {
        //        var t = data[i].VisitRecord.Last().VisitDate;
        //        var d = data[i];
        //        var j = i;
        //        while (j > 0 && data[j - 1].VisitRecord.Last().VisitDate < t)
        //        {
        //            data[j] = data[j - 1];
        //            --j;
        //        }
        //        data[j] = d;
        //    }
        //}
        public bool SaveRecord(string PatID, mVisitData VData)
        {
            try
            {
                MongoServer server = ConnectDB();
                var db = server.GetDatabase("hdb");
                mVisitData vdata = DataPreprocess(VData);
               
                
                using (server.RequestStart(db))
                {
                    var collection = db.GetCollection<mPatInfo>("Pats");
                    ObjectId id = new ObjectId(PatID);
                    // Create a document with the ID we want to find
                    var query = new QueryDocument { { "_id", id } };
                    mPatInfo entity = collection.FindOne(query);
                    var Visitcollection = db.GetCollection<mVisitRecord>("VisitRecords");
                    
                    mVisitRecord vr = vdata.Visitrecord;
                    vr.VisitDate = DateTime.Now.Date;
                    Visitcollection.Insert(vr);
                    MongoDBRef r = new MongoDBRef("VisitRecords", vr.VisitrecordId);
                    entity.refs.Add(r);

                    mRelatedInfo mr = vdata.RelateInfo;
                    entity.relatedInfo = mr;
                    collection.Save(entity);
                }         
                
               
                return true;
            }
            catch (Exception e)
            {
                return false;
            }

        }
         public ReportData ViewDetail(string PatID, string RecordID)
         {
             ReportData rdata = new ReportData();
             MongoServer server = ConnectDB();
             var db = server.GetDatabase("hdb");
             try
             {
                 using (server.RequestStart(db))
                 {
                     var PatCollection = db.GetCollection<mPatInfo>("Pats");
                     ObjectId id = new ObjectId(PatID);

                     // Create a document with the ID we want to find
                     var query = new QueryDocument { { "_id", id } };

                     mPatInfo entity = PatCollection.FindOne(query);
                     rdata.Name = entity.Name;
                     rdata.Sex = entity.Sex;
                     rdata.Phone = entity.Phone;
                     rdata.Address = entity.Address;
                     rdata.VisitDate = DateTime.Now.Date;
                    rdata.headacheoverview.OnsetDate = DateTime.Now.Date;
                     if(entity.relatedInfo!=null)
                     {
                         rdata.patlifestyle = entity.relatedInfo.patlifestyle;
                         rdata.Hfamilymember = entity.relatedInfo.Hfamilymember;
                         rdata.Ofamilydisease = entity.relatedInfo.Ofamilydisease;
                         rdata.previousdrug = entity.relatedInfo.previousdrug;
                         rdata.previousexam = entity.relatedInfo.previousexam;
                     }
                     
             
                     if(RecordID!="")
                     {
                         var Visitrecord = db.FetchDBRef(entity.refs.Last());
                         mVisitRecord r = BsonSerializer.Deserialize<mVisitRecord>(Visitrecord);
                         ObjectMapper.CopyValueProperties(r, rdata);
                     }
                 }
                 return rdata;
             }
             catch(Exception e)
             {
                 return null;
             }
       
         }
        //public bool UpdateRecord(string PatID, string VisitID, VisitData VData)
        //{
        //    try
        //    {
        //        VisitData vdata = DataPreprocess(VData);
        //        PatBasicInfor pt = context.PatBasicInforSet.Find(PatID);
        //        pt.HeadacheFamilyMember = vdata.HFamilyMember;//个人信息相关保存
        //        pt.OtherFamilyDisease = vdata.OFamilyDisease;
        //        ObjectMapper.CopyProperties(vdata.lifestyle, pt.Lifestyle);
        //        pt.PreviousDrug = vdata.PDrug;
        //        pt.PreviousExam = vdata.PExam;
        //        if (vdata.Similarfamily == "有")
        //        {
        //            pt.SimilarFamily = true;
        //        }
        //        else
        //        {
        //            pt.SimilarFamily = false;
        //        }
        //        if (vdata.visitrecord != null)
        //        {
        //            IEnumerable<VisitRecord> record = from p in context.VisitRecordSet.ToList()
        //                                              where (p.PatBasicInfor.Id == PatID) && (p.Id == int.Parse(VisitID))
        //                                              select p;
        //            VisitRecord vr = record.First();

        //            ObjectMapper.CopyProperties(vdata.visitrecord, vr);
        //            ObjectMapper.CopyProperties(vdata.PHeadacheOverview, vr.PrimaryHeadachaOverView);
                   
        //            ObjectMapper.CopyProperties(vdata.GADquestionaire, vr.GADQuestionaire);
        //            ObjectMapper.CopyProperties(vdata.PHquestionaire, vr.PHQuestionaire);
        //            ObjectMapper.CopyProperties(vdata.Disabilityevaluation, vr.DisabilityEvaluation);
        //            ObjectMapper.CopyProperties(vdata.Sleepstatus, vr.SleepStatus);
        //            vr.PrimaryHeadachaOverView.VisitRecord = vr;
        //            vr.GADQuestionaire.VisitRecord=vr;
        //            vr.PHQuestionaire.VisitRecord=vr;
        //            vr.DisabilityEvaluation.VisitRecord=vr;
        //            vr.SleepStatus.VisitRecord=vr;
                   
                   
        //            vr.PatBasicInforId = PatID;
        //            vr.VisitDate = DateTime.Now.Date;
        //            context.Entry(vr).State = System.Data.EntityState.Modified;
        //        }
        //        context.SaveChanges();
        //        return true;
        //    }
        //    catch (Exception e)
        //    {
        //        return false;
        //    }

        //}
         public bool UpdateRecord(string PatID, string VisitID, mVisitData VData)
         {
             try{

                 MongoServer server = ConnectDB();
                var db = server.GetDatabase("hdb");
                mVisitData vdata = DataPreprocess(VData);
               
                
                using (server.RequestStart(db))
                {
                    var collection = db.GetCollection<mPatInfo>("Pats");
                    ObjectId id = new ObjectId(PatID);
                    // Create a document with the ID we want to find
                    var query = new QueryDocument { { "_id", id } };
                    mPatInfo entity = collection.FindOne(query);
                    var Visitcollection = db.GetCollection<mVisitRecord>("VisitRecords");
                   
                    mVisitRecord vr = vdata.Visitrecord;
                    vr.VisitDate = DateTime.Now.Date;
                    vr.VisitrecordId = new ObjectId(VisitID);
                    Visitcollection.Save(vr);
                    mRelatedInfo mr = vdata.RelateInfo;
                    entity.relatedInfo = mr;
                    collection.Save(entity);
                }         
                
               
                return true;
            }
    
            catch (Exception e)
            {
                return false;
            }
         }
         public List<mVisitRecord> GetVistRecord(string PatID)
         {
             List<mVisitRecord> vr = new List<mVisitRecord>();
                try{
                       MongoServer server = ConnectDB();
                       MongoDatabase db = server.GetDatabase("hdb");
                       using (server.RequestStart(db))
                       {
                           var PCollection = db.GetCollection<mPatInfo>("Pats");
                           ObjectId pid = new ObjectId(PatID);
                           var query = Query.EQ("_id",pid);
                           //var query = Query.EQ("Sex","男");
                           mPatInfo p = PCollection.FindOneAs<mPatInfo>(query);
                           if(p!=null)
                           {
                               foreach (MongoDBRef r in p.refs)
                               {
                                   mVisitRecord v = db.FetchDBRefAs<mVisitRecord>(r);
                                   if(v!=null)
                                   {
                                       vr.Add(v);
                                   }
                                   else{
                                   p.refs.Remove(r);
                                   PCollection.Save(p);
                                   }
                               }
                               return vr;
                           }
                           
                        }
                       return null;
                }
             catch(Exception e)
                {
                    return null;
                }
                
         }
        public bool DeleteRecord(string PatID, string RecordID)
        {
            try
            {
                MongoServer server = ConnectDB();
                var db = server.GetDatabase("hdb");
                using (server.RequestStart(db))
                {
                    var PatCollection = db.GetCollection<mPatInfo>("Pats");
                    ObjectId id = new ObjectId(PatID);

                    // Create a document with the ID we want to find
                    var query = new QueryDocument { { "_id", id } };
                    mPatInfo p = PatCollection.FindOne(query);
                    var VisitCollection = db.GetCollection<VisitRecord>("VisitRecords");
                    var qVisit = new QueryDocument { { "_id", new ObjectId(RecordID) } };
                    VisitCollection.Remove(qVisit);

                    if (p != null)
                    {
                        foreach (MongoDBRef r in p.refs)
                        {

                            if (r.Id.ToString() == RecordID)
                            {
                                p.refs.Remove(r);
                            }
                        }
                        PatCollection.Save(p);
                    }
                    return true;
                }
            }
            catch (System.Exception e)
            {
                return false;

            }
        }
        //public ReportData ViewDetail(string PatID, string RecordID)
        //{
        //    ReportData rdata = new ReportData();
        //    PatBasicInfor pt = context.PatBasicInforSet.Find(PatID);
        //    rdata.Name = pt.Name;
        //    rdata.Age = pt.Age;
        //    rdata.Sex = pt.Sex;
        //    rdata.Address = pt.Address;
        //    rdata.Education = pt.Education;
        //    rdata.Job = pt.Job;
        //    rdata.Phone = pt.Phone;
        //    rdata.Weight = pt.Weight;
        //    rdata.Height = pt.Height;
        //    if (pt.SimilarFamily != null)
        //    {
        //        if (pt.SimilarFamily == true)
        //        {
        //            rdata.SimilarFamily = true;
        //        }
        //        else
        //        {
        //            rdata.SimilarFamily = false;
        //        }

        //    }

        //    if (pt.Lifestyle != null)
        //    {
        //        rdata.patlifestyle.SmokeState = pt.Lifestyle.SmokeState;
        //        rdata.patlifestyle.SmokeYear = pt.Lifestyle.SmokeYear;
                
        //        rdata.patlifestyle.DrinkState = pt.Lifestyle.DrinkState;
               
        //        rdata.patlifestyle.DrinkYear = pt.Lifestyle.DrinkYear;
        //        rdata.patlifestyle.TeaPerDay = pt.Lifestyle.TeaPerDay;
        //        rdata.patlifestyle.CoffePerDay = pt.Lifestyle.CoffePerDay;
        //        rdata.patlifestyle.ExerciseOften = pt.Lifestyle.ExerciseOften;
               
        //    }

        //    //if (pt.SimilarFamily!=null)
        //    //{
        //    //    rdata.SimilarFamily = pt.SimilarFamily;
        //    //}
        //    foreach (HeadacheFamilyMember hfmember in pt.HeadacheFamilyMember)
        //    {
        //        rdata.Hfamilymember.Add(hfmember.Person);
        //    }
        //    foreach (OtherFamilyDisease ofdisease in pt.OtherFamilyDisease)
        //    {
        //        rdata.Ofamilydisease.Add(ofdisease.DiseaseName);
        //    }
        //    foreach (PreviousDrug pdrug in pt.PreviousDrug)
        //    {
        //        PDrug pd = new PDrug();
        //        ObjectMapper.CopyProperties(pdrug, pd);
        //        rdata.previousdrug.Add(pd);
        //    }
        //    foreach (PreviousExam pexam in pt.PreviousExam)
        //    {
        //        Exam exam = new Exam();
        //        ObjectMapper.CopyProperties(pexam, exam);
        //        rdata.previousexam.Add(exam);
        //    }
        //    if (RecordID != "")
        //    {

        //        var record = from p in context.VisitRecordSet.ToList()
        //                     where (p.PatBasicInfor.Id == PatID) && (p.Id == int.Parse(RecordID))
        //                     select p;
        //        VisitRecord vr = record.First();
        //        if (vr != null)
        //        {

        //            rdata.VisitDate = vr.VisitDate;
        //            rdata.CDSSDiagnosis1 = vr.CDSSDiagnosis1;
        //            rdata.CDSSDiagnosis2 = vr.CDSSDiagnosis2;
        //            rdata.CDSSDiagnosis3 = vr.CDSSDiagnosis3;
        //            rdata.DiagnosisResult1 = vr.DiagnosisResult1;
        //            rdata.DiagnosisResult2 = vr.DiagnosisResult2;
        //            rdata.DiagnosisResult3 = vr.DiagnosisResult3;
        //            rdata.Prescription = vr.Prescription;
        //            rdata.ChiefComplaint = vr.ChiefComplaint;
        //            rdata.PreviousDiagnosis = vr.PreviousDiagnosis;
        //            rdata.PrescriptionNote = vr.PrescriptionNote;
        //            foreach (SecondaryHeadacheSymptom ss in vr.SecondaryHeadacheSymptom)
        //            {
        //                rdata.secondaryheadachesymptom.Add(ss.Symptom);
        //            }
        //            foreach (MedicationAdvice madvice in vr.MedicationAdvice)
        //            {
        //                HMedicine hmedicine = new HMedicine();
        //                hmedicine.DrugApplication = madvice.DrugApplication;
        //                hmedicine.DrugCategory = madvice.DrugCategory;
        //                hmedicine.DrugName = madvice.DrugName;
        //                hmedicine.DrugDose = madvice.DrugDose;
        //                hmedicine.DrugDoseUnit = madvice.DrugDoseUnit;
        //                rdata.medicationadvice.Add(hmedicine);
        //            }

        //            if (vr.PrimaryHeadachaOverView != null)
        //            {
        //                rdata.headacheoverview.HeadacheType = vr.PrimaryHeadachaOverView.HeadacheType;
        //                rdata.headacheoverview.HeadacheDegree = vr.PrimaryHeadachaOverView.HeadacheDegree;
        //                rdata.headacheoverview.HeadcheTime = vr.PrimaryHeadachaOverView.HeadcheTime;
        //                rdata.headacheoverview.HeacheTimeUnit = vr.PrimaryHeadachaOverView.HeacheTimeUnit;
                       
        //                rdata.headacheoverview.FrequencyPerMonth = vr.PrimaryHeadachaOverView.FrequencyPerMonth;
        //                rdata.headacheoverview.OnsetFixedPeriod = vr.PrimaryHeadachaOverView.OnsetFixedPeriod;

        //                rdata.headacheoverview.OnsetDate = vr.PrimaryHeadachaOverView.OnsetDate;
        //                rdata.headacheoverview.OnsetAmount = vr.PrimaryHeadachaOverView.OnsetAmount;
        //                rdata.headacheoverview.DailyAggravation = vr.PrimaryHeadachaOverView.DailyAggravation;
        //                rdata.headacheoverview.FirstOnsetContinue = vr.PrimaryHeadachaOverView.FirstOnsetContinue;

        //                foreach (HeadachePlace hp in vr.PrimaryHeadachaOverView.HeadachePlace)
        //                {
        //                    string strPlace = hp.Position + hp.SpecificPlace;
        //                    rdata.headacheplace.Add(strPlace);
        //                }
        //                foreach (HeadacheProdrome hprodrome in vr.PrimaryHeadachaOverView.HeadacheProdrome)
        //                {
        //                    rdata.headacheprodrome.Add(hprodrome.Prodrome);
        //                }
        //                foreach (MitigatingFactors mfactors in vr.PrimaryHeadachaOverView.MitigatingFactors)
        //                {
        //                    rdata.mitigatingfactors.Add(mfactors.FactorName);
        //                }
        //                foreach (HeadacheAccompany haccompay in vr.PrimaryHeadachaOverView.HeadacheAccompany)
        //                {
        //                    rdata.headacheaccompany.Add(haccompay.Symptom);
        //                }
        //                foreach (PrecipitatingFactor pfactor in vr.PrimaryHeadachaOverView.PrecipitatingFactor)
        //                {
        //                    Factor f = new Factor();
        //                    f.FName = pfactor.FactorName;
        //                    f.FDetail = pfactor.FactorDetail;
        //                    rdata.precipitatingfactor.Add(f);
        //                }
        //                foreach (PremonitorySymptom psymptom in vr.PrimaryHeadachaOverView.PremonitorySymptom)
        //                {   
        //                    rdata.premonitorsymptom.Add(psymptom.Symptom);
        //                }
        //            }

        //            //add 2013/7/23
        //            if (vr.GADQuestionaire != null)
        //            {
        //                ObjectMapper.CopyProperties(vr.GADQuestionaire, rdata.gdaquestionaire);
        //            }
        //            if (vr.PHQuestionaire != null)
        //            {
        //                ObjectMapper.CopyProperties(vr.PHQuestionaire, rdata.phquestionaire);
        //            }
        //            if (vr.SleepStatus != null)
        //            {
        //                ObjectMapper.CopyProperties(vr.SleepStatus, rdata.sleepconditon);
        //            }
        //            if (vr.DisabilityEvaluation != null)
        //            {
        //                ObjectMapper.CopyProperties(vr.DisabilityEvaluation, rdata.disablityevaluation);
        //            }
        //        }

        //    }
        //    else
        //    {
        //        rdata.VisitDate = DateTime.Now.Date;
        //        rdata.headacheoverview.OnsetDate = DateTime.Now.Date;
        //    }
        //    return rdata;
        //}

        public mVisitData DataPreprocess(mVisitData VData)
        {
            try
            {
                int num1 = VData.RelateInfo.Hfamilymember.Count - 1;
                //对于空字符串进行处理
                for (int i = num1; i >= 0; i--)
                {
                    if (VData.RelateInfo.Hfamilymember[i] == "")
                    {
                        VData.RelateInfo.Hfamilymember.RemoveAt(i);
                    }
                }
                int num2 = VData.RelateInfo.Ofamilydisease.Count - 1;
                for (int j = num2; j >= 0; j--)
                {
                    if (VData.RelateInfo.Ofamilydisease[j] == "")
                    {
                        VData.RelateInfo.Ofamilydisease.RemoveAt(j);
                    }
                }
                int num3 = VData.RelateInfo.previousdrug.Count - 1;
                for (int m = num3; m >= 0; m--)
                {
                    if (VData.RelateInfo.previousdrug[m].DrugCategory == "")
                    {
                        VData.RelateInfo.previousdrug.RemoveAt(m);
                    }
                }
                int num4 = VData.RelateInfo.previousexam.Count - 1;
                for (int n = num4; n >= 0; n--)
                {
                    if (VData.RelateInfo.previousexam[n].ExamName == "")
                    {
                        VData.RelateInfo.previousexam.RemoveAt(n);
                    }
                }
                int num5 = VData.Visitrecord.medicationadvice.Count - 1;
                for (int n = num5; n >= 0; n--)
                {
                    if (VData.Visitrecord.medicationadvice[n].DrugName == "")
                    {
                        VData.Visitrecord.medicationadvice.RemoveAt(n);
                    }
                }

                int count1 = VData.Visitrecord.headacheaccompany.Count - 1;
                for (int n = count1; n >= 0; n--)
                {
                   string ha = VData.Visitrecord.headacheaccompany.ElementAt(n);
                    if (ha == "")
                    {
                        VData.Visitrecord.headacheaccompany.Remove(ha);
                    }
                }
                int count2 = VData.Visitrecord.headacheprodrome.Count - 1;
                for (int n = count2; n >= 0; n--)
                {
                    
                    if (VData.Visitrecord.headacheprodrome.ElementAt(n) == "")
                    {
                        VData.Visitrecord.headacheprodrome.RemoveAt(n);
                    }
                }
                int count3 = VData.Visitrecord.headacheplace.Count - 1;
                for (int n = count3; n >= 0; n--)
                {
                    if (VData.Visitrecord.headacheplace.ElementAt(n) == "")
                    {
                        VData.Visitrecord.headacheplace.RemoveAt(n);
                    }
                }


                int count4 = VData.Visitrecord.mitigatingfactors.Count - 1;
                for (int n = count4; n >= 0; n--)
                {
                    if (VData.Visitrecord.mitigatingfactors.ElementAt(n) == "")
                    {
                        VData.Visitrecord.mitigatingfactors.RemoveAt(n);
                    }
                }
                int count5 = VData.Visitrecord.precipitatingfactor.Count - 1;
                for (int n = count5; n >= 0; n--)
                {
                    Factor ha = VData.Visitrecord.precipitatingfactor.ElementAt(n);
                    if (ha.FName == "")
                    {
                        VData.Visitrecord.precipitatingfactor.Remove(ha);
                    }
                }
                int count6 = VData.Visitrecord.secondaryheadachesymptom.Count - 1;
                for (int n = count6; n >= 0; n--)
                {
                    if (VData.Visitrecord.secondaryheadachesymptom.ElementAt(n) == "")
                    {
                        VData.Visitrecord.secondaryheadachesymptom.RemoveAt(n);
                    }
                }

                int count7 = VData.Visitrecord.premonitorsymptom.Count - 1;
                for (int n = count7; n >= 0; n--)
                {
                    if (VData.Visitrecord.premonitorsymptom.ElementAt(n) == "")
                    {
                        VData.Visitrecord.premonitorsymptom.RemoveAt(n);
                    }
                }

                return VData;
            }
            catch (Exception e)
            {
                return null;
            }
        }
       public List<int> GetDiaryNumericData(string PatID ,DateTime StartDate,DateTime EndDate,string Query)
        {
            PatBasicInfor pt = context.PatBasicInforSet.Find(PatID);
            List<HeadacheDiary> HDiary = new List<HeadacheDiary>();
            List<int> NumericData =new List<int>();
            foreach (HeadacheDiary vr in pt.HeadacheDiary)
            {
                TimeSpan ts1=vr.RecordDate-StartDate;
                TimeSpan ts2=EndDate-vr.RecordDate;
                if (ts1.Days>=0&&ts2.Days>=0)
                {
                     HDiary.Add(vr);
                }
               
            }
            if (Query=="头痛程度")
            {
                foreach (HeadacheDiary d in HDiary)
                {
                    NumericData.Add(d.HeadacheDegree);
                }
            }
            else if(Query=="头痛时长"){
                foreach (HeadacheDiary d in HDiary)
                {
                    NumericData.Add(d.HeadacheTime);
                }
            }
            return NumericData;
        }

        public List<DiaryDataPoint> GetDiaryQualitativeData(string PatID, DateTime StartDate, DateTime EndDate, string Query)
        {
            List<DiaryDataPoint> Result = new List<DiaryDataPoint>();
            PatBasicInfor pt = context.PatBasicInforSet.Find(PatID);
            List<HeadacheDiary> HDiary = new List<HeadacheDiary>();
            List<string> Hdata = new List<string>();
            foreach (HeadacheDiary vr in pt.HeadacheDiary)
            {
                TimeSpan ts1 = vr.RecordDate - StartDate;
                TimeSpan ts2 = EndDate - vr.RecordDate;
                if (ts1.Days >= 0 && ts2.Days >= 0)
                {
                    HDiary.Add(vr);
                }

            }
            if (Query == "头痛性质")
            {
                foreach (HeadacheDiary hd in HDiary)
                {
                    Hdata.Add(hd.HeadacheType);
                }
            }
            if (Query == "头痛部位")
            {
                foreach (HeadacheDiary hd in HDiary)
                {
                    foreach (HDheadacheplace ha in hd.HDheadacheplace)
                    {
                        Hdata.Add(ha.postion+ha.detailposition);
                    }
                }
            }
            if (Query == "伴随症状")
            {
                foreach (HeadacheDiary hd in HDiary)
                {
                   foreach(HDAcompanion ha in hd.HDAcompanion){
                       Hdata.Add(ha.symptom);
                   }
                }
            }
            if (Query == "头痛先兆")
            {
                foreach (HeadacheDiary hd in HDiary)
                {
                    foreach (HDHeadacheProdrome ha in hd.HDHeadacheProdrome)
                    {
                        Hdata.Add(ha.symptom);
                    }
                }
            }
            if (Query == "诱发因素")
            {
                foreach (HeadacheDiary hd in HDiary)
                {
                    foreach (HDPrecipitatingFactor ha in hd.HDPrecipitatingFactor)
                    {
                        Hdata.Add(ha.factor);
                    }
                }
            }
            if (Query == "缓解因素")
            {
                foreach (HeadacheDiary hd in HDiary)
                {
                    foreach (HDMitigatingFactors ha in hd.HDMitigatingFactors)
                    {
                        Hdata.Add(ha.factor);
                    }
                }
            }
            Result = Count(Hdata);
            return Result;
        }

        public List<DiaryDataPoint> Count(List<string> HData)
        {
            List<DiaryDataPoint> Result = new List<DiaryDataPoint>();
            List<string> kinds =new List<string>();
            List<int> kindscount = new List<int>();
            for (int i = 0; i < HData.Count;i++ )
            {
                if (i == 0)
                {
                    kinds.Add(HData[0]);
                    kindscount.Add(1);
                }
                else
                {
                    bool flag=false;
                    for (int j = 0; j < kinds.Count; j++)
                    {
                        if (HData[i]==kinds[j])
                        {
                            kindscount[j]++;
                            flag=true;
                            break;
                            
                        }
                        
                    }
                    if(!flag)
                    {
                        kinds.Add(HData[i]);
                        kindscount.Add(1);
                    }
                }

            }
            for (int n = 0; n < kinds.Count; n++)
            {
                DiaryDataPoint dp = new DiaryDataPoint();
                dp.data = kindscount[n].ToString();
                dp.kind = kinds[n];
                Result.Add(dp);
            }
                return Result;
        }
   
    }
}