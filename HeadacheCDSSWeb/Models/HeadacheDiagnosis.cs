using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace HeadacheCDSSWeb.Models
{
    public class HeadacheDiagnosis
    {
        VisitDataOperation vdataoperation = new VisitDataOperation();
        public string GetDiagnosis(mVisitData vData)
        {
            string result = "";
            mVisitData vd = vdataoperation.DataPreprocess(vData);
            if (vd.Visitrecord.secondaryheadachesymptom.Count != 0)
            {
                //继发性头痛
                string[] Disease1 = { "蛛网膜下腔出血", "脑出血", "瘤卒中", "脑外伤", "颅内占位病变" };
                string[] Disease2 = { "颅内占位病变", "硬膜下血肿", "过度使用药物" };
                string[] Disease3 = { "颅内感染", "系统性感染", "结缔组织疾病", "血管炎" };
                string[] Disease4 = { "颅内占位", "动静脉畸形", "结缔组织疾病", "颅内感染", "脑卒中" };
                string[] Disease5 = { "颅内占位病变", "假性脑瘤综合怔", "颅内感染" };
                string[] Disease6 = { "蛛网膜下腔出血", "颅内占位病变" };
                string[] Disease7 = { "皮层静脉/静脉窦血栓形成", "垂体卒中" };
                string[] Disease8 = { "转移癌", "机会性感染" };
                string[] Disease9 = { "颅内占位病变", "颞动脉炎" };
                List<string[]> StrDisease = new List<string[]>();
                StrDisease.Add(Disease1);
                StrDisease.Add(Disease2);
                StrDisease.Add(Disease3);
                StrDisease.Add(Disease4);
                StrDisease.Add(Disease5);
                StrDisease.Add(Disease6);
                StrDisease.Add(Disease7);
                StrDisease.Add(Disease8);
                StrDisease.Add(Disease9);
                List<string> advice = new List<string>();
                List<string> disease = new List<string>();
                string conclusion = "需要做以下检查：";
                for (int n = 0; n < vd.Visitrecord.secondaryheadachesymptom.Count; n++)
                {
                    int num = n + 1;
                    string number = num.ToString();
                    adddisease(disease, StrDisease[n]);
                    advice.Add(number);
                }
                if (advice.Count != 0)
                {
                    conclusion = conclusion + "神经影像学检查";
                    if (advice.Contains("1") || advice.Contains("3") || advice.Contains("4"))
                    {
                        conclusion = conclusion + "、腰穿";
                    }
                    else
                    {
                        if (advice.Contains("5") || advice.Contains("6") || advice.Contains("8"))
                        {
                            conclusion = conclusion + "、腰穿";
                        }
                    }
                }
                string last = "可能患有以下疾病：";
                foreach (string d in disease)
                {
                    last = last + d + "、";
                }
                string DiseaseLast = last.Substring(0, last.Length - 1);
                result = DiseaseLast + "," + conclusion;

            }
            else
            {   //原发性头痛
                localhost.InputData InputDataValue = new localhost.InputData();
                List<string> errorinfor = new List<string>();
                if (vd.Visitrecord.headacheoverview.OnsetDate != null)
                {
                    DateTime Startdate = vd.Visitrecord.headacheoverview.OnsetDate;
                    string sd = Startdate.Date.ToShortDateString();
                    int month = (DateTime.Now.Date.Year - Startdate.Year) * 12 + (DateTime.Now.Date.Month - Startdate.Month);
                    InputDataValue.m_nHeadache_Duration = month;
                    if (sd == "1753/1/1")
                    {
                        errorinfor.Add("最近起病年月");
                    }
                }

                if (vd.Visitrecord.headacheoverview.FirstOnsetContinue != "")
                {
                    if (vd.Visitrecord.headacheoverview.FirstOnsetContinue == "是")
                    {
                        InputDataValue.m_bDaily_Headache = true;
                    }
                    else if (vd.Visitrecord.headacheoverview.FirstOnsetContinue == "否")
                    {
                        InputDataValue.m_bDaily_Headache = false;
                    }
                }
                else
                {
                    errorinfor.Add("是否每日头痛");
                }
                if (vd.Visitrecord.headacheoverview.OnsetFixedPeriod == "")
                {
                    errorinfor.Add("头痛是否固定时间发生");
                }
                else
                {
                    if (vd.Visitrecord.headacheoverview.OnsetFixedPeriod == "否")
                    {
                        InputDataValue.m_bPeriodism = false;
                    }
                    else
                    {
                        InputDataValue.m_bPeriodism = true;
                    }
                }
                if (vd.Visitrecord.headacheoverview.HeadacheDegree != "")
                {
                    int number = int.Parse(vd.Visitrecord.headacheoverview.HeadacheDegree);
                    if (number <= 4)
                    {
                        InputDataValue.m_nHeadacheDegree = localhost.HeadacheDegree.Mild;
                    }
                    if (number > 4 && number < 8)
                    {
                        InputDataValue.m_nHeadacheDegree = localhost.HeadacheDegree.Middle;
                    }
                    if (number >= 8)
                    {
                        InputDataValue.m_nHeadacheDegree = localhost.HeadacheDegree.Heavy;
                    }
                }
                else
                {
                    errorinfor.Add("头痛程度");
                }
                if (vd.Visitrecord.headacheoverview.DailyAggravation != "")
                {
                    if (vd.Visitrecord.headacheoverview.DailyAggravation.Contains("无"))

                    {
                        InputDataValue.m_bWorsen_By_Physicial_Activity = false;
                    }
                    else
                    {
                        InputDataValue.m_bWorsen_By_Physicial_Activity = true;
                    }
                }
                else
                {
                    errorinfor.Add("头痛是否因为日常生活加重");
                }
                if (vd.Visitrecord.headacheoverview.HeadacheType != "")
                {

                    if (vd.Visitrecord.headacheoverview.HeadacheType == "搏动性痛" || vd.Visitrecord.headacheoverview.HeadacheType == "胀痛")
                    {
                        InputDataValue.m_nHeadahceProperty = localhost.HeadacheProperty.Pulse_Pain;
                    }
                    if (vd.Visitrecord.headacheoverview.HeadacheType.Contains("压迫")|| vd.Visitrecord.headacheoverview.HeadacheType == "紧箍性痛")
                    {
                        InputDataValue.m_nHeadahceProperty = localhost.HeadacheProperty.Pressure_Pain;
                    }
                    if (vd.Visitrecord.headacheoverview.HeadacheType.Contains("过电样"))
                    {
                        InputDataValue.m_nHeadahceProperty = localhost.HeadacheProperty.Electric_Shock_Like_Pain;
                    }
                    if (vd.Visitrecord.headacheoverview.HeadacheType == "其它" || vd.Visitrecord.headacheoverview.HeadacheType.Contains("炸裂样"))
                    {
                        InputDataValue.m_nHeadahceProperty = localhost.HeadacheProperty.Other;
                    }
                }
                else
                {
                    errorinfor.Add("头痛性质");
                }
                if (vd.Visitrecord.headacheoverview.HeadcheTime != "")
                {
                    InputDataValue.m_nHeadache_Duration_PerTime = int.Parse(vd.Visitrecord.headacheoverview.HeadcheTime);
                }
                else
                {
                    errorinfor.Add("每次头痛持续时间");
                }
                if (vd.Visitrecord.headacheoverview.HeacheTimeUnit != "")
                {
                    InputDataValue.m_nHeadache_Duration_PerTime_Unit = vd.Visitrecord.headacheoverview.HeacheTimeUnit;
                }
                else
                {
                    errorinfor.Add("每次头痛持续时间单位");
                }
                if (vd.Visitrecord.headacheoverview.OnsetAmount != "")
                {
                    string total = vd.Visitrecord.headacheoverview.OnsetAmount;
                    if (total.Contains("5"))
                    {
                        InputDataValue.m_nHeadache_TotalNumber = 3;
                    }
                    if (total.Contains("9"))
                    {
                        InputDataValue.m_nHeadache_TotalNumber = 7;
                    }
                    if (total.Contains("10") || total.Contains("持续头痛"))
                    {
                        InputDataValue.m_nHeadache_TotalNumber = 12;
                    }
                }
                else
                {
                    errorinfor.Add("头痛发作总次数");
                }
                if (vd.Visitrecord.headacheoverview.FrequencyPerMonth != "")
                {
                    string frequency = vd.Visitrecord.headacheoverview.FrequencyPerMonth;
                    if (frequency == "<1")
                    {
                        InputDataValue.m_nHeadache_Monthly_Duration = 0;
                    }
                    if (frequency == "1-15")
                    {
                        InputDataValue.m_nHeadache_Monthly_Duration = 8;
                    }
                    if (frequency == ">15")
                    {
                        InputDataValue.m_nHeadache_Monthly_Duration = 16;
                    }
                }
                else
                {
                    errorinfor.Add("头痛每月发生天数");
                }
                foreach (string hp in vd.Visitrecord.headacheplace)
                {
                    if (hp.Contains("双侧") || hp.Contains ("全头痛"))
                    {
                        InputDataValue.m_HeadacheLocation = localhost.HeadacheLocation.Bi_Pain;
                        break;
                    }
                    if (hp.Contains("左侧") || hp.Contains("右侧"))
                    {
                        InputDataValue.m_HeadacheLocation = localhost.HeadacheLocation.Uni_Pain;
                        break;
                    }
                }
                if (vd.Visitrecord.headacheplace.Count == 0)
                {
                    errorinfor.Add("头痛部位");
                }
                List<localhost.HeadacheAssociatedSymptoms> HeadacheAssociatedSymptonList = new List<localhost.HeadacheAssociatedSymptoms>();
                foreach (string ha in vd.Visitrecord.headacheaccompany)
                {

                    if (ha == "恶心")
                    {
                        HeadacheAssociatedSymptonList.Add(localhost.HeadacheAssociatedSymptoms.Nausea);
                    }
                    if (ha == "呕吐")
                    {
                        HeadacheAssociatedSymptonList.Add(localhost.HeadacheAssociatedSymptoms.Vomit);
                    }
                    if (ha== "怕吵")
                    {
                        HeadacheAssociatedSymptonList.Add(localhost.HeadacheAssociatedSymptoms.Fair_Of_Sound);
                    }
                    if (ha== "畏光")
                    {
                        HeadacheAssociatedSymptonList.Add(localhost.HeadacheAssociatedSymptoms.Fire_Of_Light);
                    }
                    if (ha == "同侧鼻塞/流涕")
                    {
                        HeadacheAssociatedSymptonList.Add(localhost.HeadacheAssociatedSymptoms.Blocked_or_Watery_Nose);
                    }
                    if (ha == "同侧结膜充血/流泪")
                    {
                        HeadacheAssociatedSymptonList.Add(localhost.HeadacheAssociatedSymptoms.Conjunctival_congestion_or_Tears);
                    }
                    if (ha == "感觉不安或躁动")
                    {
                        HeadacheAssociatedSymptonList.Add(localhost.HeadacheAssociatedSymptoms.Dysphoria);
                    }
                    if (ha == "同侧前额/面部出汗")
                    {
                        HeadacheAssociatedSymptonList.Add(localhost.HeadacheAssociatedSymptoms.Frontal_facial_Sweating);
                    }
                    if (ha == "同侧眼睑水肿")
                    {
                        HeadacheAssociatedSymptonList.Add(localhost.HeadacheAssociatedSymptoms.Ipsilateral_Heyelids_Swelling);
                    }
                    if (!HeadacheAssociatedSymptonList.Contains(localhost.HeadacheAssociatedSymptoms.Miosis_or_Blepharoptosis))
                    {

                      if (ha== "同侧眼睑下垂" || ha == "同侧瞳孔缩小")
                      {
                        HeadacheAssociatedSymptonList.Add(localhost.HeadacheAssociatedSymptoms.Miosis_or_Blepharoptosis);
                      }
                    }
                }

                List<localhost.HeadacheAura> HeadacheAuraList = new List<localhost.HeadacheAura>();
                foreach (string headacheP in vd.Visitrecord.headacheprodrome)
                {
                    if (headacheP.Contains("单侧视觉"))
                    {
                        HeadacheAuraList.Add(localhost.HeadacheAura.Hemi_Visual_Aura);
                    }
                    if (headacheP.Contains("双侧视觉"))
                    {
                        HeadacheAuraList.Add(localhost.HeadacheAura.Bilateral_Visual_Aura);
                    }
                    if (headacheP.Contains("感觉"))
                    {
                        HeadacheAuraList.Add(localhost.HeadacheAura.Feeling_Aura);
                    }
                    if (headacheP == "语言障碍")
                    {
                        HeadacheAuraList.Add(localhost.HeadacheAura.Allolalia);
                    }
                    if (headacheP == "运动障碍")
                    {
                        HeadacheAuraList.Add(localhost.HeadacheAura.Dyscinesia);
                    }
                }
                InputDataValue.m_HeadacheAuraList = HeadacheAuraList.ToArray();
                foreach (PDrug pdrug in vd.RelateInfo.previousdrug)
                {
                    if (pdrug.DrugApplication=="止痛"&&pdrug.DayAmoutnPerM != ""&& pdrug.MonthTotalAmount != "")
                    {
                        int day = int.Parse(pdrug.DayAmoutnPerM);
                        int month = int.Parse(pdrug.MonthTotalAmount);
                        if (pdrug.DrugCategory == "曲普坦")
                        {

                            if (day > 10 && month >= 3)
                            {
                                InputDataValue.m_nTriptan_Drugin_Monthly = day;
                                InputDataValue.m_nTriptan_Total_Drugin_Duration = month;

                            }
                        }
                        if (pdrug.DrugCategory != "曲普坦")
                        {

                            if (day > 15 && month > 3)
                            {
                                InputDataValue.m_nNon_Triptan_Drugin_Monthly = day;
                                InputDataValue.m_nNon_Triptan_Total_Drugin_Duration = month;

                            }
                        }
                    }
                }
                if (errorinfor.Count == 0)
                {

                    InputDataValue.m_HeadacheAssociatedSymptonList = HeadacheAssociatedSymptonList.ToArray();
                    localhost.InferenceService test = new localhost.InferenceService();
                    string strReslut = null;
                    test.DoInference(InputDataValue, ref strReslut);
                    result = strReslut;
                }
                else
                {
                    for (int i = 0; i < errorinfor.Count; i++)
                    {
                        string err = errorinfor[i];
                        if (i != errorinfor.Count - 1)
                        {
                            err = err + ",";
                        }

                        result = result + err;
                    }
                    result = "注意：" + result + "为必填项，请填写";
                }
            }

            return result;

        }
        private void adddisease(List<string> disease, string[] newdisease)
        {
            foreach (string d in newdisease)
            {
                if (!disease.Contains(d))
                {
                    disease.Add(d);
                }
            }

        }
    }
}