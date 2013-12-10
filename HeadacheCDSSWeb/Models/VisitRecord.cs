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
    public partial class VisitRecord
    {
        public VisitRecord()
        {
            this.SecondaryHeadacheSymptom = new HashSet<SecondaryHeadacheSymptom>();
            this.MedicationAdvice = new HashSet<MedicationAdvice>();
        }
    
        public int Id { get; set; }
        public string OutpatientID { get; set; }
        public string ChiefComplaint { get; set; }
        public System.DateTime VisitDate { get; set; }
        public string CDSSDiagnosis1 { get; set; }
        public string CDSSDiagnosis2 { get; set; }
        public string CDSSDiagnosis3 { get; set; }
        public string DiagnosisResult1 { get; set; }
        public string DiagnosisResult2 { get; set; }
        public string DiagnosisResult3 { get; set; }
        public string Prescription { get; set; }
        public string PreviousDiagnosis { get; set; }
        public string PrescriptionNote { get; set; }
        public string PatBasicInforId { get; set; }
    
        public virtual ICollection<SecondaryHeadacheSymptom> SecondaryHeadacheSymptom { get; set; }
        public virtual PrimaryHeadacheOverView PrimaryHeadachaOverView { get; set; }
        public virtual GADQuestionaire GADQuestionaire { get; set; }
        public virtual PHQuestionaire PHQuestionaire { get; set; }
        public virtual SleepStatus SleepStatus { get; set; }
        public virtual DisabilityEvaluation DisabilityEvaluation { get; set; }
        public virtual PatBasicInfor PatBasicInfor { get; set; }
        public virtual ICollection<MedicationAdvice> MedicationAdvice { get; set; }
    }
    
}