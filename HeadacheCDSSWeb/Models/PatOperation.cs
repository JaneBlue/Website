using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity.Validation;
using System.Diagnostics;
namespace HeadacheCDSSWeb.Models
{
    public class PatOperation
    {
       HeadacheModelContainer context = new HeadacheModelContainer();
       
        public bool InsertPat(PatBasicInfor pat, string PID, string User)
        {

            try
            {
                pat.Id = System.Guid.NewGuid().ToString().Replace("-", "");
                Lifestyle lstyle = new Lifestyle();
                lstyle.PatBasicInfor=pat;
                pat.Lifestyle = lstyle;
                var users = from s in context.DoctorAccountSet.ToList() select s;
                DoctorAccount user = users.Where(s => s.UserName == User).FirstOrDefault();
                user.PatBasicInfor.Add(pat);

                context.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}