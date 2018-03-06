using Client.Logging;
using Client.ViewModels;
using DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Excelra.Utils.Library;
using Entities.DTO.Static;
using Client.Util;

namespace Client.Common
{
    public static class ThermalValidations
    {
        public static List<ValidationError> TermalValidations(IEnumerable<ReactionParticipantVM> participants, List<string> SolventReg, List<SolventBoilingPointDTO> solventBoilingPoints, TanVM tanVM, ReactionVM reaction, StageVM stage, List<RsnVM> freeTexts)
        {
            ExtentionMethods common = new ExtentionMethods();

            var errors = new List<ValidationError>();
            try
            {
                var solvents = participants.OfStageOfType(stage.Id, ParticipantType.Solvent).Where(s => SolventReg.Contains(s.Reg)).ToList();
                bool unUsedCVT = false;
                #region 12-4
                var CVTList = tanVM.Rsns.OfReactionAndStage(reaction.Id, stage.Id).Where(c => !string.IsNullOrEmpty(c.CvtText)).Select(cvt => cvt.CvtText);
                List<int> StageThermalNumbers = PressureValidations.GetStageDisplayOrdersFromFreetexts(freeTexts, reaction, S.THERMAL_STRING, new List<string> { S.HYDRO_THERMAL_STRING, S.SOLVO_THERMAL_STRING });
                List<int> MicrowaveStageNumbers = PressureValidations.GetStageDisplayOrdersFromFreetexts(freeTexts, reaction, S.MICROWAVE_STRING);
                if (CVTList.Contains(S.MIC_IRR) || MicrowaveStageNumbers.Contains(stage.DisplayOrder))
                    if (CVTList.Contains(S.THERMAL_STRING) || StageThermalNumbers.Contains(stage.DisplayOrder))
                        errors.Add(VF.OfRSN(reaction, "Microwave irradiation has no Thermal RSN even though it exceeds boiling point of solvent", stage));
                #endregion
                if ((tanVM.Rsns.OfReaction(reaction.Id).Where(cvt=> (cvt.CvtText.SafeEqualsLower(S.THERMAL_STRING) || (cvt.FreeText.IncludeExclude(S.THERMAL_STRING, new List<string> { S.HYDRO_THERMAL_STRING, S.SOLVO_THERMAL_STRING })))).Any()
                    || StageThermalNumbers.Contains(stage.DisplayOrder)) && (stage.Conditions == null || !stage.Conditions.Any() || stage.Conditions.Where(con => !string.IsNullOrEmpty(con.Temperature)).Count() == 0 || solvents == null || solvents.Count == 0 || solvents.Count > 1))
                    errors.Add(VF.OfRSN(reaction,"RSN CVT/FreeText 'thermal' Present. It allowed only when Solvent Boiling point exceeds.", stage));

                if (solvents.Any() && solvents.Count == 1 && stage.Conditions != null)
                {
                    var TemperatureList = stage.Conditions.Where(con => !string.IsNullOrEmpty(con.Temperature) && con.TEMP_TYPE != TemperatureEnum.RefluxtoRoomTemp.ToString() && con.TEMP_TYPE != TemperatureEnum.RoomTemptoReflux.ToString());
                    foreach (var condition in TemperatureList)
                    {
                        try
                        {
                            string TempUnit = string.Empty;
                            var sol = (from p in solventBoilingPoints where solvents.Select(r => r.Reg).Contains(p.RegNo) select p).FirstOrDefault();
                            common.GetTemperatureAndUnitsFromString(condition.Temperature, out TempUnit, condition);
                            float i = 0, j = 0, k = 0;
                            var temperatureValue = Regex.Replace(condition.Temperature, "[^0-9.\\-\\]]", "");
                            if (temperatureValue.Contains("-") || temperatureValue.Contains("]"))
                            {
                                string[] splitterRange = { "-", "]" };
                                string[] strValsRange = temperatureValue.Split(splitterRange, StringSplitOptions.RemoveEmptyEntries);
                                if (temperatureValue.Contains($"-{strValsRange[0]}"))
                                    strValsRange[0] = $"-{strValsRange[0]}";
                                if (strValsRange.Count() > 1 && (temperatureValue.Contains($"--{strValsRange[1]}") || temperatureValue.Contains($"]-{strValsRange[1]}")))
                                    strValsRange[1] = $"-{strValsRange[1]}";
                                if (strValsRange.Count() > 1)
                                {
                                    if (float.TryParse(common.GetTemperatureAndUnitsFromString(strValsRange[0], out TempUnit), out j) && float.TryParse(common.GetTemperatureAndUnitsFromString(strValsRange[1], out TempUnit), out k))
                                    {
                                        if (j > k)
                                            temperatureValue = j.ToString();
                                        else
                                            temperatureValue = k.ToString();
                                    }
                                }
                                else
                                    temperatureValue = common.GetTemperatureAndUnitsFromString(strValsRange[0], out TempUnit);
                            }
                            if (float.TryParse(temperatureValue, out i) && ((TempUnit == S.FH_UNIT && i > sol.fahrenheitBoilingPoint) || (TempUnit == S.K_UNIT && i > sol.KelvinBoilingPoint) || (TempUnit == S.RANKINE_UNIT && i > sol.RankineBoilingPoint) || (TempUnit == S.CENTI_UNIT && i > sol.DegreesBoilingPoint)))
                            {
                                List<int> StagemicrowaveNumbers = PressureValidations.GetStageDisplayOrdersFromFreetexts(freeTexts, reaction, S.MICROWAVE_STRING);
                                if (freeTexts.Where(f => f.Stage != null && f.Stage.Id == stage.Id && (f.FreeText.IncludeExclude(S.THERMAL_STRING, new List<string> { S.HYDRO_THERMAL_STRING, S.SOLVO_THERMAL_STRING }) || f.CvtText.IncludeExclude(S.THERMAL_STRING, new List<string> { S.SOLVO_THERMAL_STRING, S.HYDRO_THERMAL_STRING }))).Count() == 0 && !StageThermalNumbers.Contains(stage.DisplayOrder) && !StagemicrowaveNumbers.Contains(stage.DisplayOrder) &&
                                    tanVM.Rsns.Where(r => r.Reaction.Id == reaction.Id && r.Stage == null && r.CvtText.SafeEqualsLower(S.MIC_IRR)).Count() == 0 && tanVM.Rsns.Where(r => r.Reaction.Id == reaction.Id && r.Stage == null && (r.CvtText.SafeContainsLower(S.THERMAL_STRING) || r.FreeText.IncludeExclude(S.THERMAL_STRING, new List<string> { S.HYDRO_THERMAL_STRING, S.SOLVO_THERMAL_STRING }))).Count() == 0)
                                    errors.Add(VF.OfRSN(reaction, 
                                                        $"Solvent {sol.Name} Boiling Point is  {(TempUnit == S.K_UNIT ? sol.KelvinBoilingPoint : TempUnit == S.RANKINE_UNIT ? sol.RankineBoilingPoint : TempUnit == S.FH_UNIT ? sol.fahrenheitBoilingPoint : sol.DegreesBoilingPoint)} {TempUnit} Exceeded. RSN CVT/Free should be 'thermal'",
                                                        stage));

                            }
                            if ((float.TryParse(temperatureValue, out i) && ((TempUnit == S.FH_UNIT && i <= sol.fahrenheitBoilingPoint) || (TempUnit == S.K_UNIT && i <= sol.KelvinBoilingPoint) || (TempUnit == S.RANKINE_UNIT && i <= sol.RankineBoilingPoint) || (TempUnit == S.CENTI_UNIT && i <= sol.DegreesBoilingPoint))) || (condition.TEMP_TYPE == TemperatureEnum.Cool.ToString() || condition.TEMP_TYPE == TemperatureEnum.Roomtemp.ToString() || condition.TEMP_TYPE == TemperatureEnum.RefluxBoiled.ToString() || condition.TEMP_TYPE == TemperatureEnum.Heated.ToString() || condition.TEMP_TYPE == TemperatureEnum.LessthanRoomTemp.ToString() || condition.TEMP_TYPE == TemperatureEnum.RoomTempgraterthan.ToString() || condition.TEMP_TYPE == TemperatureEnum.Cool.ToString()))
                            {
                                if ((freeTexts.Where(f => f.Stage != null && f.Stage.Id == stage.Id && (f.FreeText.IncludeExclude(S.THERMAL_STRING, new List<string> { S.HYDRO_THERMAL_STRING, S.SOLVO_THERMAL_STRING }) || f.CvtText.SafeContainsLower(S.THERMAL_STRING))).Count() > 0 || StageThermalNumbers.Contains(stage.DisplayOrder) ||
                                     tanVM.Rsns.Where(r => r.Reaction.Id == reaction.Id && r.Stage == null && (r.CvtText.SafeContainsLower(S.THERMAL_STRING) || r.FreeText.IncludeExclude(S.THERMAL_STRING, new List<string> { S.HYDRO_THERMAL_STRING, S.SOLVO_THERMAL_STRING }))).Count() > 0) && !unUsedCVT)
                                    unUsedCVT = false;
                            }
                            else
                                unUsedCVT = true;
                        }
                        catch (Exception ex)
                        {
                            Log.This(ex);
                        }
                    }
                    if ((StageThermalNumbers.Contains(stage.DisplayOrder) || tanVM.Rsns.Where(cvt => cvt.Reaction.Id == reaction.Id && cvt.Stage == null && (cvt.CvtText.SafeEqualsLower(S.THERMAL_STRING) || cvt.FreeText.IncludeExclude(S.THERMAL_STRING, new List<string> { S.HYDRO_THERMAL_STRING, S.SOLVO_THERMAL_STRING }))).Count() > 0) && !unUsedCVT)
                        errors.Add(VF.OfRSN(reaction,"RSN CVT/FreeText 'thermal' Present. It allowed only when Solvent Boiling point exceeds.", stage));
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
            return errors;
        }
    }
}
