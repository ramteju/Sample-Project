using Client.Logging;
using Client.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Excelra.Utils.Library;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Client.Validations;

namespace Client.Common
{
    static class PressureValidations
    {
        public static List<ValidationError> ValidatePressure(bool highPressure, ReactionVM reaction, StageVM stage, List<RsnVM> Rsns)
        {
            var errors = new List<ValidationError>();
            ExtentionMethods common = new Common.ExtentionMethods();
            List<string> pressureTypes = new List<string> { PressureEnum.Pressure.ToString(), PressureEnum.Directional.ToString(), PressureEnum.Range.ToString() };
            var StagePressureConditions = stage.Conditions != null ? stage.Conditions.Where(p => pressureTypes.Contains(p.PRESSURE_TYPE)).ToList() : null;
            var HighPressureCVTs = Rsns.Where(f => f.Reaction.Id == reaction.Id && f.Stage == null && (f.CvtText.SafeContainsLower(S.HIGH_PRESSURE_CVT_TERM) || f.FreeText.SafeContainsLower(S.HIGH_PRESSURE_FREETEXT_TERM)));
            var lowPressureCVTs = Rsns.Where(f => f.Stage == null && (f.CvtText.SafeContainsLower(S.LOW_PRESSURE_CVT_TERM) || f.FreeText.SafeContainsLower(S.LOW_PRESSURE_FREETEXT_TERM)));
            var stageHighPressureDisplayOrders = GetStageDisplayOrdersFromFreetexts(Rsns.ToList(), reaction, S.HIGH_PRESSURE_FREETEXT_TERM);
            var stageLowPressureDisplayOrders = GetStageDisplayOrdersFromFreetexts(Rsns.ToList(), reaction, S.LOW_PRESSURE_FREETEXT_TERM);
            #region 12-21
            if (highPressure)
            {
                if ((HighPressureCVTs.Any() || stageHighPressureDisplayOrders.Contains(stage.DisplayOrder)) && (!StagePressureConditions.Any() || StagePressureConditions.Where(con => !string.IsNullOrEmpty(con.Pressure)).Count() == 0))
                    errors.Add(VF.OfRSN(reaction, "RSN Conatins 'High Pressure' in CVT/FreeText. High Pressure is Allowed only when Pressure Exceeds Normal Pressure", stage));
                errors.AddRange(HighAndLowPresssureValidations(StagePressureConditions, highPressure, reaction, stage, stageHighPressureDisplayOrders, HighPressureCVTs.ToList()));
            }
            #endregion

            #region 12-22
            else
            {
                if ((lowPressureCVTs.Any() || stageLowPressureDisplayOrders.Contains(stage.DisplayOrder)) && (!StagePressureConditions.Any() || StagePressureConditions.Where(con => !string.IsNullOrEmpty(con.Pressure)).Count() == 0))
                    errors.Add(VF.OfRSN(reaction, "RSN Conatins 'Low Pressure' in CVT/FreeText. Low Pressure is Allowed only when Pressure below Normal Pressure", stage));
                errors.AddRange(HighAndLowPresssureValidations(StagePressureConditions, highPressure, reaction, stage, stageLowPressureDisplayOrders, lowPressureCVTs.ToList()));
            }
            #endregion
            //if (highPressure)
            //{
            //    #region 12-21
            //    if (stageHighPressureDisplayOrders.Contains(stage.DisplayOrder) && (!StagePressureConditions.Any() || StagePressureConditions.Where(con => !string.IsNullOrEmpty(con.Pressure)).Count() == 0))
            //        errors.Add(VF.OfRSN(reaction, "RSN Conatins 'High Pressure' in CVT/FreeText. High Pressure is Allowed only when Pressure Exceeds Noraml Pressure", stage));
            //    errors.AddRange(HighAndLowPresssureValidations(StagePressureConditions, highPressure, reaction, stage, stageHighPressureDisplayOrders,HighPressureCVTs.ToList()));
            //    #endregion
            //}

            //else
            //{
            //    #region 12-22

            //    if (stageLowPressureDisplayOrders.Contains(stage.DisplayOrder) && (!StagePressureConditions.Any() || StagePressureConditions.Where(con => !string.IsNullOrEmpty(con.Pressure)).Count() == 0))
            //        errors.Add(VF.OfRSN(reaction, "RSN Conatins 'Low Pressure' in CVT/FreeText. Low Pressure is Allowed only when Pressure below Noraml Pressure", stage));
            //    errors.AddRange(HighAndLowPresssureValidations(StagePressureConditions, highPressure, reaction, stage, stageLowPressureDisplayOrders,lowPressureCVTs.ToList()));
            //    #endregion
            //}
            return errors;
        }
        public static List<int> GetStageDisplayOrdersFromFreetexts(List<RsnVM> Rsns, ReactionVM SelectedReaction, string SearchString, List<string> excludestrings = null, bool AllowEmpty = false, bool IncludeCVTVerification = false, string cvtTerm = "")
        {

            try
            {
                List<int> StageNumbers = new List<int>();
                var freetextDisplayOrders = Rsns.Where(f => f.Reaction.Id == SelectedReaction.Id
                                                            && f.FreeText.Split(new String[] { "), " }, StringSplitOptions.RemoveEmptyEntries).Where(s => s.IncludeExclude(SearchString, excludestrings, AllowEmpty)).Any()
                                                            && (f.FreeText.Split(new String[] { "), " }, StringSplitOptions.RemoveEmptyEntries).Where(s => s.IncludeExclude(SearchString, excludestrings, AllowEmpty)).First().TrimEnd(')') + ")").Contains("(stage"))
                                                 .Select(f => RV.GetStageInfoWithOutFreeText(f.FreeText.Split(new String[] { "), " }, StringSplitOptions.RemoveEmptyEntries).Where(s => s.IncludeExclude(SearchString, excludestrings, AllowEmpty)).First().TrimEnd(')') + ")"))
                                                 .ToList();
                foreach (var item in freetextDisplayOrders)
                {
                    string outMsg = string.Empty;
                    RV.GetStageNumbersFromFreeText(item,ref StageNumbers,ref outMsg);
                    #region Commented RSNFreetexts
                    //if (Regex.IsMatch(item, S.STAGEINFO_RANGE))
                    //{
                    //    var itemlist = Regex.Replace(item, @"[a-z\(\)\s]", "").Split('-').Select(c => Convert.ToInt32(c)).ToList();
                    //    if (itemlist.Count > 1)
                    //        for (int r = itemlist[0]; r <= itemlist[1]; r++)
                    //            StageNumbers.Add(r);
                    //}
                    //else if (Regex.IsMatch(item, S.STAGEINFO_AND))
                    //{
                    //    var itemlist = item.Replace("(stages ", "").Replace(" and ", "-").Replace(")", "").Split('-').Select(c => Convert.ToInt32(c)).ToList();
                    //    StageNumbers.AddRange(itemlist);
                    //}
                    //else if (Regex.IsMatch(item, S.STAGEINFO_SINGLE_STAGE))
                    //    StageNumbers.Add(Convert.ToInt32(item.Replace("(stage ", "").Replace(")", "")));
                    //else if (Regex.IsMatch(item, S.STAGEINFO_MULTIPLE_COMMA))
                    //{
                    //    var itemlist = item.Replace("(stages ", "").Replace(",", "-").Replace(")", "").Split('-').Select(c => Convert.ToInt32(c)).ToList();
                    //    StageNumbers.AddRange(itemlist);
                    //}
                    //else if (Regex.IsMatch(item, S.STAGEINFO_MULTIPLE_COMMA_AND))
                    //{
                    //    var itemlist = item.Replace("(stages ", "").Replace(",", "-").Replace(" and ", "-").Replace(")", "").Split('-').Select(c => Convert.ToInt32(c)).ToList();
                    //    StageNumbers.AddRange(itemlist);
                    //}
                    //else if (Regex.IsMatch(item, S.STAGEINFO_RANGE_AND_RANGE))
                    //{
                    //    var itemlist = item.Replace("(stages ", "").Replace(")", "").Split(new string[] { " and " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    //    foreach (string range in itemlist)
                    //    {
                    //        var splittedList = range.Split('-').Select(c => Convert.ToInt32(c)).ToList();
                    //        if (splittedList.Count > 1)
                    //            for (int r = splittedList[0]; r <= splittedList[1]; r++)
                    //                StageNumbers.Add(r);
                    //    }
                    //}
                    //else if (Regex.IsMatch(item, S.STAGEINFO_RANGE_COMMA_AND))
                    //{
                    //    var itemlist = item.Replace("(stages ", "").Replace(")", "").Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    //    var splittedListWithRange = itemlist[0].Split('-').Select(c => Convert.ToInt32(c)).ToList();
                    //    if (splittedListWithRange.Count > 1)
                    //        for (int r = splittedListWithRange[0]; r <= splittedListWithRange[1]; r++)
                    //            StageNumbers.Add(r);
                    //    var splittedList = itemlist[1].Replace(" and ", "-").Split('-').Select(c => Convert.ToInt32(c)).ToList();
                    //    StageNumbers.AddRange(splittedList);
                    //} 
                    #endregion
                }
                return StageNumbers.OrderBy(c => c).Distinct().ToList();
            }
            catch (Exception ex)
            {
                Log.This(ex);
                throw;
            }
        }


        public static List<ValidationError> HighAndLowPresssureValidations(List<StageConditionVM> StagePressureConditions, bool highPressure, ReactionVM reaction, StageVM stage, List<int> stageHighPressureDisplayOrders, List<RsnVM> ReactionLevelRsns=null)
        {
            var errors = new List<ValidationError>();
            bool unUsedCVT = false;
            if (StagePressureConditions.Any())
            {
                foreach (var pres in StagePressureConditions)
                {
                    string pressureUnit = string.Empty;
                    ExtentionMethods common = new Common.ExtentionMethods();
                    var normalvalue = common.GetPressureAndUnitsFromString(pres.Pressure, out pressureUnit);
                    float i = 0, j = 0, k = 0;
                    string value = Regex.Replace(pres.Pressure, "[^0-9.\\-\\]]", "");


                    if (value.Contains("-") || value.Contains("]"))
                    {
                        string[] splitterRange = { "-", "]" };
                        string[] strValsRange = pres.Pressure.Split(splitterRange, StringSplitOptions.RemoveEmptyEntries);
                        if (pres.Pressure.Contains($"-{strValsRange[0]}"))
                            strValsRange[0] = $"-{strValsRange[0]}";
                        if (strValsRange.Count() > 1 && (pres.Pressure.Contains($"--{strValsRange[1]}") || pres.Pressure.Contains($"]-{strValsRange[1]}")))
                            strValsRange[1] = $"-{strValsRange[1]}";
                        if (strValsRange.Count() > 1)
                        {
                            if (float.TryParse(common.GetPressureAndUnitsFromString(strValsRange[0], out pressureUnit), out j) && float.TryParse(common.GetPressureAndUnitsFromString(strValsRange[1], out pressureUnit), out k))
                                value = highPressure ? Math.Max(j, k).ToString() : Math.Min(j, k).ToString();
                        }
                        else
                            value = normalvalue;
                    }

                    if (float.TryParse(value, out i) && pressureUnit == "kp" && (highPressure ? i >= S.KP_HIGHVALUE : i < S.K_LOWVALUE) &&
                        (!stageHighPressureDisplayOrders.Contains(stage.DisplayOrder) && (ReactionLevelRsns == null || !ReactionLevelRsns.Any())))
                        errors.Add(VF.OfRSN(reaction, $"If pressure { (highPressure ? S.EXCEEDS_KP_HIGH : S.BELOW_KP_VALUE)} kPa then Free text must contains {(highPressure ? "'High Pressure'" : "'Low Pressure'")}", stage));
                    else if (float.TryParse(value, out i) && pressureUnit == "s" && (highPressure ? i >= S.S_HIGHVALUE : i < S.S_LOWVALUE) &&
                        !stageHighPressureDisplayOrders.Contains(stage.DisplayOrder) && (ReactionLevelRsns == null || !ReactionLevelRsns.Any()))
                        errors.Add(VF.OfRSN(reaction, $"If pressure { (highPressure ? S.EXCEEDS_S_HIGH : S.BELOW_S_VALUE)} Psi then Free text must contains {(highPressure ? "'High Pressure'" : "'Low Pressure'")}", stage));
                    else if (float.TryParse(value, out i) && (highPressure ? i >= S.A_B_HIGHVALUE : i < S.A_B_LOWVALUE) && (pressureUnit == "a" || pressureUnit == "b") &&
                        !stageHighPressureDisplayOrders.Contains(stage.DisplayOrder) && (ReactionLevelRsns == null || !ReactionLevelRsns.Any()))
                        errors.Add(VF.OfRSN(reaction, $"If pressure { (highPressure ? S.EXCEEDS_A_B_HIGH : S.BELOW_A_B_VALUE)} atm/Bars then Free text must contains {(highPressure ? "'High Pressure'" : "'Low Pressure'")}", stage));
                    if (stageHighPressureDisplayOrders.Contains(stage.DisplayOrder) || (ReactionLevelRsns != null && ReactionLevelRsns.Any()))
                    {
                        if (float.TryParse(value, out i) && ((pressureUnit == "kp" && (highPressure ? i < S.KP_HIGHVALUE : i >= S.K_LOWVALUE)) || (pressureUnit == "s" && (highPressure ? i < S.S_HIGHVALUE : i >= S.S_LOWVALUE) || ((highPressure ? i < S.A_B_HIGHVALUE : i >= S.A_B_LOWVALUE) && (pressureUnit == "a" || pressureUnit == "b")))) && !unUsedCVT)
                            unUsedCVT = false;
                        else
                            unUsedCVT = true;
                    }
                }
            }
            if ((stageHighPressureDisplayOrders.Contains(stage.DisplayOrder) || (ReactionLevelRsns != null && ReactionLevelRsns.Any())) && !unUsedCVT)
                errors.Add(VF.OfRSN(reaction, $"RSN Contains { (highPressure ? "High" : "Low")} pressure in CVT/Freetext. {(highPressure ? "'High Pressure'" : "'Low Pressure'")} is Allowed only when certain Pressure exists.", stage));
            return errors;
        }
    }
}
