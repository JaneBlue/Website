//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace HeadacheCDSSWeb.Models
{
    public partial class DisabilityEvaluation
    {
        public int Id { get; set; }
        public string HeadacheDays { get; set; }
        public string OutOfWorkDays { get; set; }
        public string AffectWorkDays { get; set; }
        public string OutOfHouseWorkDays { get; set; }
        public string AffectHouseWorkDays { get; set; }
        public string MissActivityDays { get; set; }
    
        public virtual VisitRecord VisitRecord { get; set; }
    }
    
}