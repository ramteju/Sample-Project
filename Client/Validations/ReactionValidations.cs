using Client.Logging;
using Client.ViewModels;
using Client.Views;
using DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Client.Properties;
using Excelra.Utils.Library;
using Entities.DTO.Static;
using Client.Util;
using Client.Validations;
using System.Diagnostics;
using System.Windows.Input;
using System.Collections.ObjectModel;

namespace Client.Common
{
    public static class ReactionValidations
    {
        public static List<ValidationError> IsValidReaction(ReactionVM reaction,IEnumerable<ReactionParticipantVM> participants, Collection<RsnVM> ReactionRsns, Collection<RsnVM> ReactionRsnsWithoutStages)
        {
            try
            {
                var start = DateTime.Now;
                //Debug.WriteLine($"Validating Reaction {reaction.Name} started at {start}");
                List<SolventBoilingPointDTO> solventBoilingPoints = S.SolventBoilingPoints;
                List<string> SolventReg = solventBoilingPoints.Select(s => s.RegNo).ToList();
                List<ValidationError> errors = new List<ValidationError>();
                var duplicateReactions = new List<ReactionVM>();
                Regex yieldReg = new Regex(@"^[,0-9\s]+$");
                Regex carbonReg = new Regex(S.CORBON_REG_EXP);
                var mainViewModel = ((MainWindow)(App.Current.MainWindow)).DataContext as MainVM;
                var tanVM = mainViewModel.TanVM;
                ExtentionMethods common = new Common.ExtentionMethods();
                if (participants ==null && ReactionRsns == null && ReactionRsnsWithoutStages == null)
                {
                    participants = tanVM.ReactionParticipants.OfReaction(reaction.Id);
                    ReactionRsns = tanVM.Rsns.OfReaction(reaction.Id, true);
                    ReactionRsnsWithoutStages = tanVM.Rsns.OfReaction(reaction.Id);
                }
                var CVTs = ReactionRsns.Where(cvt => !string.IsNullOrEmpty(cvt.CvtText)).Select(cvt => cvt.CvtText.ToLower()).ToList();// (from cvt in tanVM.Rsns where cvt.Reaction.Id == reaction.Id && !string.IsNullOrEmpty(cvt.CvtText) select cvt.CvtText.ToLower()).ToList();
                var freeTexts = ReactionRsns.Where(cvt => !string.IsNullOrEmpty(cvt.FreeText)).Select(cvt => cvt).ToList();//(from freetext in tanVM.Rsns where freetext.Reaction.Id == reaction.Id && freetext.FreeText != null select freetext).ToList();
                var Above9999AndBelow1Nums = participants.Where(num => num.Num > 9999 || num.Num < 1);
                if (Above9999AndBelow1Nums.Any())
                {
                    var Above9999AndBelow1Participant = Above9999AndBelow1Nums.FirstOrDefault();
                    errors.Add(new ValidationError { ReactionVM = reaction, Message = $"Participant {Above9999AndBelow1Participant.Reg} having Num {Above9999AndBelow1Participant.Num}. Num range must be in 1-9999." });
                }
                if (participants.OfType(ParticipantType.Product).Where(rp => !string.IsNullOrEmpty(rp.Formula)).Any() && !participants.OfType(ParticipantType.Product).Where(rp => !string.IsNullOrEmpty(rp.Formula) && carbonReg.IsMatch(rp.Formula)).Any())
                {
                    if (mainViewModel.ValidationVM.ValidationWarnings != null)
                        mainViewModel.ValidationVM.ValidationWarnings.Add(new ValidationError { ReactionVM = reaction, Message = $"Atleast one product must contain Carbon", Category = ValidationError.RXN });
                }

                if (participants.OfType(ParticipantType.Reactant).Where(rp => !string.IsNullOrEmpty(rp.Formula)).Any() && !participants.OfType(ParticipantType.Reactant).Where(rp => !string.IsNullOrEmpty(rp.Formula) && carbonReg.IsMatch(rp.Formula)).Any())
                {
                    if (mainViewModel.ValidationVM.ValidationWarnings != null)
                        mainViewModel.ValidationVM.ValidationWarnings.Add(new ValidationError { ReactionVM = reaction, Message = $"Atleast one Reactant must contain Carbon", Category = ValidationError.RXN });
                }

                if (!string.IsNullOrEmpty(reaction.KeyProductSeq) && reaction.KeyProductSeq.Split('-').Count() > 1)
                {
                    var Sequence = reaction.KeyProductSeq.Split('-')[1];
                    int KeyProductSeq;
                    if (int.TryParse(Sequence, out KeyProductSeq) && KeyProductSeq > 9999)
                        errors.Add(VF.OfReaction(reaction, "Reaction having Sequence more than 9999. Sequence must be in 1-9999 range"));
                }

                string rsd = GetRSDString(participants, reaction);
                if (rsd.Length > 500)
                    errors.Add(VF.OfReaction(reaction, "RSD Length Exceeds 500 Chanracters"));

                var unUsedrsns = (from rsn in ReactionRsns
                                  where !rsn.CvtText.SafeContainsLower(S.CAT_PRE_USED)
                                        && !rsn.CvtText.SafeContainsLower(S.NO_EXP_DET_CHRO)
                                        && rsn.IsIgnorableInDelivery && (rsn.ReactionParticipantId == null || rsn.ReactionParticipantId.Count == 0)
                                  select rsn)
                                  .ToList();
                if (unUsedrsns != null && unUsedrsns.Count > 0)
                    errors.Add(VF.OfRSN(reaction, $"There are unused Special RSN in the Reaction, {string.Join(",", unUsedrsns.Select(rsn => rsn.FreeText).ToList())}"));

                var FreeTextStageNumbers = PressureValidations.GetStageDisplayOrdersFromFreetexts(ReactionRsns.ToList(), reaction, "", null, true);
                var StageDisplayOrders = reaction.Stages.Select(s => s.DisplayOrder).ToList();
                if (FreeTextStageNumbers.Except(StageDisplayOrders).Count() > 0)
                    errors.Add(VF.OfRSN(reaction, "Invalid satge information in RSN Freetext"));
                #region 12-4
                if (ReactionRsnsWithoutStages.OfCVTEquals(S.MIC_IRR).Any())
                {
                    if (ReactionRsns.OfCVTEquals(S.THERMAL).Any())
                        errors.Add(VF.OfReaction(reaction, "Microwave irradiation has no Thermal RSN even though it exceeds boiling point of solvent"));
                }
                #endregion

                #region 13-45
                if ((CVTs.Contains(S.FAILED_RXN) || freeTexts.Where(f => f.FreeText.SafeContainsLower(S.FAILED_RXN)).Any())
                    && participants.OfType(ParticipantType.Product).Where(p => !string.IsNullOrEmpty(p.Yield) && p.Yield != "0").Any())
                    errors.Add(VF.OfReaction(reaction, "Yeild Not allowed for failed reactions"));
                #endregion

                #region 4-5
                if (!participants.Where(p => !string.IsNullOrEmpty(p.Reg)).Any())
                    errors.Add(VF.OfReaction(reaction, "Must Contain atleast One CAS Registered ChemicalName."));
                #endregion

                if (ReactionRsnsWithoutStages.Where(freetext => string.IsNullOrEmpty(freetext.CvtText)).Count() > 1)
                    errors.Add(VF.OfReaction(reaction, "In Reaction level only one free text Is accepted"));

                if (!participants.OfType(ParticipantType.Product).Any())
                    errors.Add(VF.OfReaction(reaction, "At Least one product is required."));

                if (ReactionRsnsWithoutStages.OfCVTEquals(S.CAT_PRE_USED).Any() && !ReactionRsnsWithoutStages.OfFreetextContains(S.CAT_PRE_AND_USED).Any())
                    errors.Add(VF.OfRSN(reaction, $"'{S.CAT_PRE_AND_USED}' freetext must present when CVT-1 {S.CAT_PRE_USED} present"));
                #region DuplicateReaction Cross validation
                bool duplicatePresent = false;
                if (ReactionRsnsWithoutStages.OfCVTEquals(S.CAT_PRE_USED).Any())
                {
                    var productsAndReactants = participants.OfType(ParticipantType.Product, ParticipantType.Reactant).Select(num => num.Num);
                    var otherReactionParticipants = participants.OfType(ParticipantType.Product, ParticipantType.Reactant).GroupBy(rp => rp.ReactionVM.Id).ToDictionary(t => t.Key, t => t.ToList());
                    foreach (var item in otherReactionParticipants)
                    {
                        if (item.Value.Select(s => s.Num).All(productsAndReactants.Contains) && productsAndReactants.All(item.Value.Select(s => s.Num).Contains))
                        {
                            duplicatePresent = true;
                            break;
                        }
                    }
                    if (!duplicatePresent)
                        errors.Add(VF.OfReaction(reaction, $"Invalid CVT-1 term {S.CAT_PRE_USED}"));
                }
                #endregion

                #region 4-2,4-4
                var InValidyields = participants.OfType(ParticipantType.Product)
                    .Where(p => !string.IsNullOrEmpty(p.Yield) && !yieldReg.IsMatch(p.Yield));
                if (InValidyields.Any())
                    errors.Add(VF.OfReaction(reaction, "InValid Yield values : " + string.Join(",", InValidyields)));

                var yields = participants.OfType(ParticipantType.Product).Where(p => !string.IsNullOrEmpty(p.Yield)).Select(p => p.Yield);
                foreach (var yield in yields)
                {
                    float doubleYield = 0;
                    string[] allYield = yield.Split(',');
                    foreach (var y in allYield)
                    {
                        if (!float.TryParse(y, out doubleYield) || doubleYield > 100)
                            errors.Add(VF.OfReaction(reaction, "In Valid Yield value : " + y));
                    }
                }
                float value = 0;
                var list = yields.Where(y => y.Contains(",") && float.TryParse(y.Split(',')[1], out value)).Select(l => l.Split(',')[1]).ToList();

                if (list.Count > 1 && Math.Abs(Convert.ToDouble(list[0]) - Convert.ToDouble(list[1])) < 10 && (CVTs.Contains(S.STRERIOSELECTIVE) || CVTs.Contains(S.REGIOSELECTICE)))
                    errors.Add(VF.OfReaction(reaction, "InValid RSN term stereoselective/regioselective"));

                if (list.Count != 0 && list.Sum(x => Convert.ToDouble(x)) != 100)
                    errors.Add(VF.OfReaction(reaction, "Total Yield Must be 100"));

                var commaYields = participants.OfType(ParticipantType.Product)
                                              .Where(rp => rp.Yield.SafeContainsLower(","))
                                              .Select(rp => rp.Yield);
                var onlyFirstValues = commaYields.Select(st => st.Split(',')[0]).GroupBy(s => s).Select(d => d.Key).Count();
                if (onlyFirstValues > 1)
                    errors.Add(VF.OfReaction(reaction, "Left value must be same in comma(,) seperated yields"));
                #endregion

                #region 4-3
                if (!participants.OfType(ParticipantType.Reactant).Any())
                    errors.Add(VF.OfReaction(reaction, "Reactant Is Missing."));
                #endregion

                #region 12-28
                var duplicates = ReactionRsns.Where(x => !string.IsNullOrEmpty(x.CvtText))
                                             .Select(c => c.CvtText)
                                             .GroupBy(x => x)
                                             .Where(g => g.Count() > 1)
                                             .Select(g => g.Key);
                if (duplicates.Any())
                    errors.Add(VF.OfRSN(reaction, string.Join(",", duplicates) + " are repeated. Stage level CVT duplicates are not allowed."));
                #endregion

                #region 12-25
                if (participants.OfReg("5137553").Any())
                    errors.Add(VF.OfRSN(reaction, "Aliquat 336 not allowed in RSD. Please romove 5137553 from RSD"));
                #endregion


                if (participants.OfReg("69849452").Any())
                    errors.Add(VF.OfRSN(reaction, "Davis reagent (REGNUM: 69849452) cannot be used in RSD, please capture in RSN"));

                #region Enzymic Validation
                if (ReactionRsns.OfCVTEquals(S.ENZYMIC_CVT).Any() && !ReactionRsns.OfCVTEquals(S.BIOTRANSFORMATION_CVT).Any())
                    errors.Add(VF.OfRSN(reaction, $"{S.ENZYMIC_CVT} CVT Present. But Respective {S.BIOTRANSFORMATION_CVT} Missed. Please Add {S.BIOTRANSFORMATION_CVT} to RSNs"));
                #endregion

                foreach (var stage in reaction.Stages)
                {
                    #region Enzymic Validation
                    List<int> enzymicStageNumbers = PressureValidations.GetStageDisplayOrdersFromFreetexts(ReactionRsns.OfReactionAndStage(reaction.Id, stage.Id).ToList(), reaction, S.ENZYMIC_FREETEXT);
                    List<int> BioTransformationStageNumbers = PressureValidations.GetStageDisplayOrdersFromFreetexts(ReactionRsns.OfReactionAndStage(reaction.Id, stage.Id).ToList(), reaction, S.BIOTRANSFORMATION_FREETEXT);
                    if (enzymicStageNumbers.Any() && (enzymicStageNumbers.Except(BioTransformationStageNumbers).Any() || BioTransformationStageNumbers.Except(enzymicStageNumbers).Any()))
                        errors.Add(VF.OfRSN(reaction, $"{S.ENZYMIC_CVT} CVT stage information and {S.BIOTRANSFORMATION_CVT}'s stage information not matched.", stage));
                    #endregion

                    #region 12-7
                    var NoSolventDisplayOrders = PressureValidations.GetStageDisplayOrdersFromFreetexts(freeTexts, reaction, S.NO_SOL);
                    var NoSolventReactionLevel = ReactionRsnsWithoutStages.OfFreetextContains(S.NO_SOL);
                    if ((NoSolventDisplayOrders.Contains(stage.DisplayOrder) || NoSolventReactionLevel.Any()) && participants.OfStageOfType(stage.Id, ParticipantType.Solvent).Any())
                        errors.Add(new ValidationError { ReactionVM = reaction, StageVM = stage, Message = "In Valid CVT 'No Solvent'. It allowed when solvents are empty.", Category = ValidationError.RSN });
                    #endregion

                    #region 6-2
                    var stageParticipants = participants.OfStage(stage.Id);// (from p in participants where p.StageVM != null && p.StageVM.Id == stage.Id select p).ToList();
                    if (!stageParticipants.Any())
                        errors.Add(new ValidationError { ReactionVM = reaction, StageVM = stage, Message = "Atleast one participant is required to save stage.", Category = ValidationError.RSN });
                    #endregion

                    #region 8-2
                    var HCLGASStageDisplayOrders = PressureValidations.GetStageDisplayOrdersFromFreetexts(ReactionRsns.ToList(), reaction, S.HCL_GAS_USED);
                    if (participants.OfStage(stage.Id).Where(p => S.InargonicAcidsList.Contains(p.Reg) && p.Name.SafeContainsLower("(gas)")).Any()
                        && (!tanVM.Rsns.OfReaction(reaction.Id).OfFreetextContains(S.HCL_GAS_USED).Any() && !HCLGASStageDisplayOrders.Contains(stage.DisplayOrder)))
                        errors.Add(new ValidationError { ReactionVM = reaction, StageVM = stage, Message = "HCL Gas is Used in RSD. Please add 'hcl gas used' Freetext in RSN", Category = ValidationError.RSN });

                    if (!participants.OfStage(stage.Id).Where(p => S.InargonicAcidsList.Contains(p.Reg) && p.Name.SafeContainsLower("(gas)")).Any() &&
                        (HCLGASStageDisplayOrders.Contains(stage.DisplayOrder) || (ReactionRsnsWithoutStages.OfFreetextContains(S.HCL_GAS_USED).Any())))
                        errors.Add(new ValidationError { ReactionVM = reaction, StageVM = stage, Message = "'hcl gas used' RSN Present. But HCL(Gas) missed in Participants.", Category = ValidationError.RSN });

                    if (participants.OfStage(stage.Id).Where(p => !p.Name.SafeContainsLower("(solid)") && !p.Name.SafeContainsLower("(conc.)") && !p.Name.SafeContainsLower("(gas)") && S.InargonicAcidsList.Contains(p.Reg)).Any() &&
                                     !participants.OfStageOfType(stage.Id, ParticipantType.Solvent).Any())
                        errors.Add(new ValidationError { ReactionVM = reaction, StageVM = stage, Message = "Solvent Should be there for inorganic acids and bases.", Category = ValidationError.RSN });
                    #endregion

                    #region 8-3
                    List<int> StagebufferNumbers = PressureValidations.GetStageDisplayOrdersFromFreetexts(freeTexts, reaction, S.BUFFER);
                    var BufferReactionLevel = ReactionRsnsWithoutStages.Where(rsn => rsn.CvtText.SafeContainsLower(S.BUFFER) || rsn.FreeText.SafeContainsLower(S.BUFFER));
                    if ((StagebufferNumbers.Contains(stage.DisplayOrder) || BufferReactionLevel.Any()) && !participants.OfStageOfType(stage.Id, ParticipantType.Solvent).OfReg("7732185").Any())
                        errors.Add(new ValidationError { ReactionVM = reaction, StageVM = stage, Message = "If RSN CVT/Freetext contains Buffer Solution then solvent must be Water", Category = ValidationError.RSN });

                    #endregion

                    #region 12-3

                    errors.AddRange(ThermalValidations.TermalValidations(participants, SolventReg, solventBoilingPoints, tanVM, reaction, stage, freeTexts));

                    #endregion

                    #region 12-21
                    var highPressureErrors = PressureValidations.ValidatePressure(true, reaction, stage, ReactionRsns.ToList());
                    if (highPressureErrors != null && highPressureErrors.Count > 0)
                        errors.AddRange(highPressureErrors);
                    #endregion

                    #region 12-22

                    var lowPressureErrors = PressureValidations.ValidatePressure(false, reaction, stage, ReactionRsns.ToList());
                    if (lowPressureErrors != null && lowPressureErrors.Count > 0)
                        errors.AddRange(lowPressureErrors);
                    #endregion

                    #region 12-17
                    var NitricStageDisplayOrders = PressureValidations.GetStageDisplayOrdersFromFreetexts(freeTexts, reaction, S.FUMING_NIT_USED);
                    var stagefumingnitricacid = participants.OfStage(stage.Id).OfReg("8007587");// (from n in participants where n.StageVM != null && n.StageVM.Id == stage.Id && n.Reg == "8007587" select n);
                    if (stagefumingnitricacid.Any() && !NitricStageDisplayOrders.Contains(stage.DisplayOrder) && (freeTexts == null || freeTexts.Where(f => f.Stage == null && f.FreeText.SafeContainsLower(S.FUMING_NIT_USED)).ToList().Count == 0))
                        errors.Add(new ValidationError { ReactionVM = reaction, StageVM = stage, Message = "Fuming nitric acid (REGNUM: 8007587) used. FreeText should be 'fuming nitric acid used'", Category = ValidationError.RSN });
                    if (!stagefumingnitricacid.Any() && (NitricStageDisplayOrders.Contains(stage.DisplayOrder) || freeTexts.Where(f => f.Stage == null && f.FreeText.SafeContainsLower(S.FUMING_NIT_USED)).ToList().Count > 0))
                        errors.Add(new ValidationError { ReactionVM = reaction, StageVM = stage, Message = "Fuming nitric acid used in free text, but Fuming nitric acid (REGNUM: 8007587) missed in RSD", Category = ValidationError.RSN });
                    #endregion

                    #region 12-18
                    var SulfuricStageDisplayOrders = PressureValidations.GetStageDisplayOrdersFromFreetexts(freeTexts, reaction, S.FUMING_SULF_USED);
                    var OlemStageDisplayOrders = PressureValidations.GetStageDisplayOrdersFromFreetexts(freeTexts, reaction, S.OLEM_USED);
                    var stagefumingsulfuricacid = participants.OfStage(stage.Id).OfReg("8014957");// (from n in participants where n.StageVM != null && n.StageVM.Id == stage.Id && n.Reg == "8014957" select n).ToList();
                    if (stagefumingsulfuricacid.Any() && !SulfuricStageDisplayOrders.Contains(stage.DisplayOrder) && (freeTexts == null || freeTexts.Where(f => f.Stage == null && f.FreeText.SafeContainsLower(S.FUMING_SULF_USED)).ToList().Count == 0))
                        if (!OlemStageDisplayOrders.Contains(stage.DisplayOrder) && (freeTexts == null || freeTexts.Where(f => f.Stage == null && f.FreeText.SafeContainsLower(S.OLEM_USED)).ToList().Count == 0))
                            errors.Add(VF.OfStage(reaction, "Fuming sulfuric acid or oleum (REGNUM:8014957) used. FreeText should be 'fuming sulfuric acid used or oleum used based on author language'", stage));
                    if (!stagefumingsulfuricacid.Any() && ((SulfuricStageDisplayOrders.Contains(stage.DisplayOrder) || OlemStageDisplayOrders.Contains(stage.DisplayOrder) || (freeTexts.Where(f => f.Stage == null && (f.FreeText.SafeContainsLower(S.FUMING_SULF_USED) || f.FreeText.SafeContainsLower(S.OLEM_USED))).ToList().Count > 0))))
                        errors.Add(VF.OfStage(reaction, "Fuming sulfuric acid used or oleum used in free text, but Fuming sulfuric acid or oleum (REGNUM:8014957) missed in RSD", stage));
                    #endregion

                    #region stage Level FreeText Validations
                    var rsns = freeTexts.Where(ft => ft.Stage != null && ft.Stage.Id == stage.Id).ToList();
                    foreach (var rsn in rsns)
                    {
                        var listofStageDisplayOrders = PressureValidations.GetStageDisplayOrdersFromFreetexts(new List<RsnVM> { rsn }, reaction, "");
                        if (listofStageDisplayOrders != null && listofStageDisplayOrders.Count > 0)
                        {
                            if (listofStageDisplayOrders.Count > 1 && !listofStageDisplayOrders.Contains(stage.DisplayOrder))
                                errors.Add(new ValidationError { ReactionVM = reaction, StageVM = stage, Category = ValidationError.RSN, Message = "Freetext Contains invalid display order" });
                            if (listofStageDisplayOrders.Count == 1 && listofStageDisplayOrders.FirstOrDefault() != stage.DisplayOrder)
                                errors.Add(new ValidationError { ReactionVM = reaction, StageVM = stage, Category = ValidationError.RSN, Message = "Freetext Contains invalid display order" });
                        }
                    }

                    #endregion


                    #region 12-6
                    var stageReactants = participants.OfType(ParticipantType.Reactant, ParticipantType.Agent).OfStage(stage.Id).OfReg("50000").OfName(S.PARA_FORM);
                    var paraFormaldehideStageDisplayOrders = PressureValidations.GetStageDisplayOrdersFromFreetexts(freeTexts, reaction, S.PARA_FORM_USED);
                    if (stageReactants.Any() && !ReactionRsnsWithoutStages.OfFreetextContains(S.PARA_FORM_USED).Any() && !paraFormaldehideStageDisplayOrders.Contains(stage.DisplayOrder))
                        errors.Add(new ValidationError { ReactionVM = reaction, StageVM = stage, Message = $"{S.PARAFORMALDEHIDE_USED_CVT} Then Freetext must Contains paraformaldehyde used", Category = ValidationError.RSN });
                    else if ((stageReactants == null || !stageReactants.Any()) && (ReactionRsnsWithoutStages.OfFreetextContains(S.PARA_FORM_USED).Any() || paraFormaldehideStageDisplayOrders.Contains(stage.DisplayOrder)))
                        errors.Add(VF.OfRSN(reaction, $"{S.PARA_FORM_USED} present in RSN. Respective stage must contains paraformaldehyde(50000) as Reactant/Reagent.", stage));
                    #endregion

                    #region 12-10
                    var BasicHydrolysysStageDisplayOrders = PressureValidations.GetStageDisplayOrdersFromFreetexts(freeTexts, reaction, S.UNSPECIFIEDREAGENT_BASIC_HYDROLYSIS);
                    var BASIDIFICATIONStageDisplayOrders = PressureValidations.GetStageDisplayOrdersFromFreetexts(freeTexts, reaction, S.UNSPECIFIEDREAGENT_BASIDIFICATION);
                    var ReactionLevelInfo = ReactionRsnsWithoutStages.Where(rsn => rsn.FreeText.SafeContainsLower(S.UNSPECIFIEDREAGENT_BASIC_HYDROLYSIS) || rsn.FreeText.SafeContainsLower(S.UNSPECIFIEDREAGENT_BASIDIFICATION));
                    var BasicHydrolysys = participants.OfStage(stage.Id).OfReg("14280309");// (from c in participants where c.StageVM != null && c.StageVM.Id == stage.Id && c.Reg == "14280309" select c).ToList();
                    if (BasicHydrolysys.Any() && (freeTexts == null || (!BasicHydrolysysStageDisplayOrders.Contains(stage.DisplayOrder) && !BASIDIFICATIONStageDisplayOrders.Contains(stage.DisplayOrder) && !ReactionLevelInfo.Any())))
                        errors.Add(VF.OfRSN(reaction, $"14280309 used. Then Freetext must Contains {S.UNSPECIFIEDREAGENT_BASIDIFICATION}/{S.UNSPECIFIEDREAGENT_BASIC_HYDROLYSIS}", stage));
                    if ((BasicHydrolysys == null || !BasicHydrolysys.Any()) && (BasicHydrolysysStageDisplayOrders.Contains(stage.DisplayOrder) || BASIDIFICATIONStageDisplayOrders.Contains(stage.DisplayOrder) || ReactionLevelInfo.Any()))
                        errors.Add(VF.OfRSN(reaction, $"Freetext Contains {S.UNSPECIFIEDREAGENT_BASIDIFICATION}/{S.UNSPECIFIEDREAGENT_BASIC_HYDROLYSIS} but 14280309 missed in RSD.", stage));
                    #endregion

                    #region 12-9
                    var AcidicHydrolysysStageDisplayOrders = PressureValidations.GetStageDisplayOrdersFromFreetexts(freeTexts, reaction, S.UNSPECIFIEDREAGENT_ACIDIC_HYDROLYSIS);
                    var AcidificationStageDisplayOrders = PressureValidations.GetStageDisplayOrdersFromFreetexts(freeTexts, reaction, S.UNSPECIFIEDREAGENT_ACIDIFICATION);
                    var AcidificationReactionLevelInfo = ReactionRsnsWithoutStages.Where(rsn => rsn.FreeText.SafeContainsLower(S.UNSPECIFIEDREAGENT_ACIDIC_HYDROLYSIS) || rsn.FreeText.SafeContainsLower(S.UNSPECIFIEDREAGENT_ACIDIFICATION));
                    var AcidicHydrolysys = participants.OfStage(stage.Id).OfReg("12408025");// (from c in participants where c.StageVM != null && c.StageVM.Id == stage.Id && c.Reg == "14280309" select c).ToList();
                    if (AcidicHydrolysys.Any() && (freeTexts == null || (!AcidicHydrolysysStageDisplayOrders.Contains(stage.DisplayOrder) && !AcidificationStageDisplayOrders.Contains(stage.DisplayOrder) && !AcidificationReactionLevelInfo.Any())))
                        errors.Add(VF.OfRSN(reaction, $"12408025 used. Then Freetext must Contains {S.UNSPECIFIEDREAGENT_ACIDIFICATION}/{S.UNSPECIFIEDREAGENT_ACIDIC_HYDROLYSIS}", stage));
                    if ((AcidicHydrolysys == null || !AcidicHydrolysys.Any()) && (AcidicHydrolysysStageDisplayOrders.Contains(stage.DisplayOrder) || AcidificationStageDisplayOrders.Contains(stage.DisplayOrder) || AcidificationReactionLevelInfo.Any()))
                        errors.Add(VF.OfRSN(reaction, $"Freetext Contains {S.UNSPECIFIEDREAGENT_ACIDIFICATION}/{S.UNSPECIFIEDREAGENT_ACIDIC_HYDROLYSIS} but 12408025 missed in RSD.", stage));
                    #endregion

                    #region 12-5

                    var SoliSupportCatalystStageDisplayOrders = PressureValidations.GetStageDisplayOrdersFromFreetexts(freeTexts, reaction, S.SOLID_SUPPORT_CATALYST_CVT);
                    var SoliSupportCatalystReactionLevelInfo = ReactionRsns.Where(rsn => rsn.CvtText.SafeContainsLower(S.SOLID_SUPPORT_CATALYST_CVT));
                    var solidcatalysts = participants.OfStageOfType(stage.Id, ParticipantType.Catalyst).OfReg("12135227").Where(c => c.Name.SafeEqualsLower(S.SOLID_SUPPORT_CATALYST_NAME1) || c.Name.SafeEqualsLower(S.SOLID_SUPPORT_CATALYST_NAME2));// (from c in participants where c.StageVM != null && c.StageVM.Id == stage.Id && c.Reg == "14280309" select c).ToList();
                    if (solidcatalysts.Any() && (CVTs == null || (!SoliSupportCatalystStageDisplayOrders.Contains(stage.DisplayOrder) && !SoliSupportCatalystReactionLevelInfo.Any())))
                        errors.Add(new ValidationError { ReactionVM = reaction, StageVM = stage, Message = "9655-12135227 catalyst used. But Solid-supported catalyst RSN Missed.", Category = ValidationError.RSN });
                    //if ((SoliSupportCatalystStageDisplayOrders.Contains(stage.DisplayOrder) || SoliSupportCatalystReactionLevelInfo.Any()) && !solidcatalysts.Any())
                    //    errors.Add(new ValidationError { ReactionVM = reaction, StageVM = stage, Message = $"{S.SOLID_SUPPORT_CATALYST_CVT} RSN used. But 9655-12135227 catalyst Missed in RSD.", Category = ValidationError.RSN });
                    #endregion

                    #region 14-3
                    if (freeTexts != null && (PressureValidations.GetStageDisplayOrdersFromFreetexts(freeTexts, reaction, S.KARS_CAT_USED).Contains(stage.DisplayOrder) || ReactionRsnsWithoutStages.OfFreetextContains(S.KARS_CAT_USED).Any()))
                        if (!participants.OfStageOfType(stage.Id, ParticipantType.Catalyst).OfReg("7440064").Any())
                            errors.Add(new ValidationError { ReactionVM = reaction, StageVM = stage, Message = "Karstedt's catalyst used RSN Used. But 7440064 catalyst Missed.", Category = ValidationError.RSN });
                    #endregion

                    #region 14-4
                    if (freeTexts != null && (PressureValidations.GetStageDisplayOrdersFromFreetexts(freeTexts, reaction, S.LIN_CAT_USED).Contains(stage.DisplayOrder) || ReactionRsnsWithoutStages.OfFreetextContains(S.LIN_CAT_USED).Any()))
                        if (!participants.OfStageOfType(stage.Id, ParticipantType.Catalyst).OfReg("7440053").Any())
                            errors.Add(new ValidationError { ReactionVM = reaction, StageVM = stage, Message = "Lindlar's catalyst used RSN Used. But 7440053 catalyst Missed.", Category = ValidationError.RSN });
                    #endregion

                    #region 14-5
                    var DescribedMedium = participants.OfStageOfType(stage.Id, ParticipantType.Solvent).OfReg("7732185");
                    if ((DescribedMedium == null || !DescribedMedium.Any()) && (ReactionRsns.OfReactionAndStage(reaction.Id, stage.Id).OfFreetextContains(S.DES_MED).Any() || ReactionRsns.OfFreetextContains(S.DES_MED).Any()))
                        errors.Add(new ValidationError { ReactionVM = reaction, StageVM = stage, Message = "described medium Freetext Used. Then solvent must be water.", Category = ValidationError.RSN });
                    #endregion

                    #region 12-8
                    var nickelcatalyst = participants.OfStageOfType(stage.Id, ParticipantType.Catalyst).OfReg("7440020").OfName(S.RN);
                    var RaneyNickelStageDisplayorders = PressureValidations.GetStageDisplayOrdersFromFreetexts(freeTexts, reaction, S.RN);
                    if (nickelcatalyst.Any() && (freeTexts == null || (!RaneyNickelStageDisplayorders.Contains(stage.DisplayOrder) && !ReactionRsnsWithoutStages.OfFreetextContains(S.RN).Any())))
                        errors.Add(new ValidationError { ReactionVM = reaction, StageVM = stage, Message = "9444-7440020 catalyst used. But Raney nickel used RSN Missed.", Category = ValidationError.RSN });

                    if ((nickelcatalyst == null || !nickelcatalyst.Any()) && (RaneyNickelStageDisplayorders.Contains(stage.DisplayOrder) || ReactionRsnsWithoutStages.OfFreetextContains(S.RN).Any()))
                        errors.Add(new ValidationError { ReactionVM = reaction, StageVM = stage, Message = "Raney nickel used RSN  used. But 9444-7440020 catalyst Missed.", Category = ValidationError.RSN });
                    #endregion

                    #region 10-2
                    var ThionylAgent = participants.OfStageOfType(stage.Id, ParticipantType.Agent).OfReg("7719097");
                    var DMFSolvent = participants.OfStageOfType(stage.Id, ParticipantType.Solvent).OfReg("68122");
                    if (ThionylAgent.Any() && DMFSolvent.Any())
                        errors.Add(new ValidationError { ReactionVM = reaction, StageVM = stage, Message = "7719097(Thionyl Chloride) Agent used. Then catalyst should be 68122(DMF)", Category = ValidationError.RSN });

                    var OxalylAgent = participants.OfStageOfType(stage.Id, ParticipantType.Agent).OfReg("79378");
                    if (OxalylAgent.Any() && DMFSolvent.Any())
                        errors.Add(new ValidationError { ReactionVM = reaction, StageVM = stage, Message = "79378(Oxalyl Chloride) Agent used. Then catalyst should be 68122(DMF)", Category = ValidationError.RSN });
                    #endregion

                    #region 12-14
                    var cvtwithoutFreeText = ReactionRsns.OfReactionAndStage(reaction.Id, stage.Id).Where(cvt => !string.IsNullOrEmpty(cvt.CvtText) && string.IsNullOrEmpty(cvt.FreeText));
                    if (cvtwithoutFreeText.Any())
                        errors.Add(new ValidationError { ReactionVM = reaction, StageVM = stage, Category = ValidationError.RSN, Message = "Freetext is mandatory when cvt present at stage level." });
                    #endregion

                    #region 12-2
                    var OtherthanproductReactants = participants.OfType(ParticipantType.Agent, ParticipantType.Catalyst, ParticipantType.Solvent);
                    if (freeTexts != null && (freeTexts.Where(f => f.FreeText.SafeContainsLower(S.NO_EXP_DET)).Any() || CVTs.Contains(S.NO_EXP_DET)) && (OtherthanproductReactants.Any() || stage.Conditions.Any()))
                        errors.Add(new ValidationError { ReactionVM = reaction, Message = "FreeText No experimental detail Present. It Allowed only when Reactants and Products Present.", Category = ValidationError.RSN });
                    if (!OtherthanproductReactants.Any() && !reaction.Stages.Where(sc => sc.Conditions.Any()).Any() && (freeTexts == null || !(freeTexts.Where(f => f.FreeText.SafeContainsLower(S.NO_EXP_DET)).Any())) && !CVTs.Contains(S.NO_EXP_DET_CHRO))
                        errors.Add(new ValidationError { ReactionVM = reaction, Message = "FreeText No experimental detail Missed. No experimental detail should pressent when only Reactants and Products Present.", Category = ValidationError.RSN });

                    if (CVTs.Contains(S.NO_EXP_DET_CHRO) && freeTexts.Where(cvt => !cvt.CvtText.SafeEqualsLower(S.NO_EXP_DET_CHRO) && !string.IsNullOrEmpty(cvt.FreeText)).Count() == 0)
                        errors.Add(new ValidationError { ReactionVM = reaction, Message = "RSN 'no experimental details-chromatography' present. Freetext is mandatory", Category = ValidationError.RSN });
                    #endregion


                }
                #region 12-12
                if ((from f in freeTexts where !string.IsNullOrEmpty(f.FreeText) && S.FreeTextListToRestrict.Where(item => f.FreeText.SafeContainsLower(item)).Count() > 0 select f).Any())
                    errors.Add(new ValidationError { ReactionVM = reaction, Message = "Freetext contains special characters", Category = ValidationError.RSN });
                #endregion
                #region 12-13
                var freetextandcvt = (from f in freeTexts where S.CommentDictionary.CVT.Select(c => c.CVTS).Contains(f.FreeText) select f);
                if (freetextandcvt.Any())
                    errors.Add(new ValidationError { ReactionVM = reaction, Message = "Freetext contains CVT terms : " + string.Join(",", freetextandcvt.Select(c => c.FreeText)), Category = ValidationError.RSN });
                #endregion

                #region 12-26
                if (reaction.Stages.Count() == 1 && ReactionRsns.OfReactionOnlyStages(reaction.Id).Any())
                    errors.Add(new ValidationError { ReactionVM = reaction, Message = "StageLevel Comment not allowed for Single stage reaction.", Category = ValidationError.RSN });
                #endregion

                #region 12-27
                if ((string.Join("", CVTs).Length + string.Join("", freeTexts.Select(f => f.FreeText).ToList()).Length) > 500)
                    errors.Add(new ValidationError { ReactionVM = reaction, Message = "CVT+Free Text length exceeds MAX length(500char)", Category = ValidationError.RSN });
                #endregion
                //var end = DateTime.Now;
                //Debug.WriteLine($"Validating Reaction {reaction.Name} end at {end} in {(end - start).TotalSeconds} Seconds");
                return errors;
            }
            catch (Exception ex)
            {

                Log.This(ex);
                throw;
            }
        }
        public static bool AllreactionCurationCompleted(TanVM tanVM, bool CurationCompleted, out List<string> inCompletedReactions)
        {
            try
            {
                inCompletedReactions = (from r in tanVM.Reactions where (CurationCompleted ? r.IsCurationCompleted == false : r.IsReviewCompleted == false) select "Reaction " + r.KeyProductSeq).ToList();
                if (inCompletedReactions.Any())
                    return false;
                return true;
            }
            catch (Exception ex)
            {
                Log.This(ex);
                throw;
            }
        }
        public static List<ValidationError> ValidateTan(TanVM tanVM)
        {
            List<string> SolventReg = S.SolventBoilingPoints.Select(s => s.RegNo).ToList();
            List<ValidationError> tanErrors = new List<ValidationError>();
            var duplicateReactions = new List<ReactionVM>();
            var mainViewModel = ((MainWindow)(App.Current.MainWindow)).DataContext as MainVM;
            ExtentionMethods common = new Common.ExtentionMethods();
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                Mouse.OverrideCursor = Cursors.Wait;
            });
            try
            {
                var start = DateTime.Now;
                #region 4-7
                if (mainViewModel != null && mainViewModel.TanVM != null)
                {
                    var formaldihydes = mainViewModel.TanVM.ReactionParticipants.OfReg("30525894");
                    if (formaldihydes.Any())
                    {
                        foreach (var formaldihyde in formaldihydes)
                            formaldihyde.Reg = "50000";
                        mainViewModel.TanVM.PerformAutoSave("Paraformaldihide Reg number updated with 50000");
                    }
                    #endregion
                    var groupedParticipants = tanVM.ReactionParticipants.GroupBy(rp => rp.ReactionVM.Id).ToDictionary(rp => rp.Key, rp => rp.ToList());
                    var groupedRsns = tanVM.Rsns.GroupBy(rp => rp.Reaction.Id).ToDictionary(rp => rp.Key, rp => rp.ToList());
                    foreach (var reaction in tanVM.Reactions)
                    {
                        //if (U.RoleId == 3)
                        //{

                            var reactionErrors = IsValidReaction(reaction,groupedParticipants.ContainsKey(reaction.Id) ? groupedParticipants[reaction.Id] : new List<ReactionParticipantVM>(), groupedRsns.ContainsKey(reaction.Id) ? groupedRsns[reaction.Id].ToCollection() : new Collection<RsnVM>(), groupedRsns.ContainsKey(reaction.Id) ? groupedRsns[reaction.Id].Where(r => r.Stage == null).ToCollection() : new Collection<RsnVM>());
                            tanErrors.AddRange(reactionErrors);
                        //}
                        //if ((U.RoleId == 1 && !reaction.IsCurationCompleted) || (U.RoleId == 2 && !reaction.IsReviewCompleted))
                        //    tanErrors.Add(new ValidationError { ReactionVM = reaction, Category = ValidationError.RXN, Message = U.RoleId == 1 ? "Curation not Completed" : "Review not Completed" });
                    }
                    Debug.WriteLine($"Curation Completed validations Done in {(DateTime.Now - start).TotalSeconds} seconds");
                    foreach (var reaction in tanVM.Reactions)
                    {
                        bool keySequence = reaction.KeyProductSeq.SafeContainsLower("-") ? Convert.ToInt32(reaction.KeyProductSeq.Split('-')[1]) > 1 : false;
                        int keyProductNum = tanVM.ReactionParticipants.Where(rp => rp.KeyProduct && rp.ReactionVM.Id == reaction.Id).Select(rp => rp.Num).FirstOrDefault();
                        start = DateTime.Now;
                        if (keySequence)
                        {
                            var reactions = tanVM.ReactionParticipants.Where(rp => rp.Num == keyProductNum && rp.KeyProduct).Select(rp => rp.ReactionVM);
                            foreach (var rxn in reactions)
                                if (!tanVM.Rsns.Where(rsn => rsn.Reaction.Id == rxn.Id && rsn.Stage == null && rsn.FreeText.SafeEqualsLower(S.ALTERNATIVE_PREPARATION_SHOWN)).Any()
                                    && !mainViewModel.ValidationVM.ValidationWarnings.Where(vw => vw.ReactionVM.Id == rxn.Id && vw.Category == ValidationError.RXN && vw.Message.SafeContainsLower(S.ALTERNATIVE_PREPARATION_SHOWN)).Any())
                                    mainViewModel.ValidationVM.ValidationWarnings.Add(new ValidationError { ReactionVM = rxn, Message = $"{S.ALTERNATIVE_PREPARATION_SHOWN} should be there in Rsns when Alternative Reaction prepared.", Category = ValidationError.RXN });
                        }
                        if (tanVM.Rsns.Where(rsn => rsn.Reaction.Id == reaction.Id && rsn.Stage == null && rsn.FreeText.SafeEqualsLower(S.ALTERNATIVE_PREPARATION_SHOWN)).Any()
                            && !tanVM.ReactionParticipants.Where(rp => rp.Num == keyProductNum && rp.KeyProduct && rp.ReactionVM.Id != reaction.Id).Any())
                            tanErrors.Add(new ValidationError { ReactionVM = reaction, Message = $"{S.ALTERNATIVE_PREPARATION_SHOWN} Freetext present. It allowed when alternative reaction prepared.", Category = ValidationError.RXN });
                    }
                    Debug.WriteLine($"KeySequence validation Done in {(DateTime.Now - start).TotalSeconds} seconds");
                    start = DateTime.Now;
                    foreach (var reaction in tanVM.Reactions)
                    {
                        if (groupedParticipants.ContainsKey(reaction.Id))
                        {
                            var products = groupedParticipants[reaction.Id].OfType(ParticipantType.Product).Select(rp => rp.Num);
                            var Reactants = groupedParticipants[reaction.Id].OfType(ParticipantType.Reactant).Select(rp => rp.Num);
                            foreach (var otherReaction in tanVM.Reactions)
                            {
                                if (otherReaction.Id != reaction.Id)
                                {
                                    if (groupedParticipants.ContainsKey(reaction.Id))
                                    {
                                        var Otherproducts = groupedParticipants[otherReaction.Id].OfType(ParticipantType.Product).Select(rp => rp.Num);
                                        var OtherReactants = groupedParticipants[otherReaction.Id].OfType(ParticipantType.Reactant).Select(rp => rp.Num);
                                        if (products.All(Otherproducts.Contains) && Otherproducts.All(products.Contains) && Reactants.All(OtherReactants.Contains) && OtherReactants.All(Reactants.Contains)
                                        && !tanVM.Rsns.Where(rsn => (rsn.Reaction.Id == otherReaction.Id || rsn.Reaction.Id == reaction.Id) && rsn.CvtText.SafeContainsLower(S.CAT_PRE_USED)).Any())
                                            duplicateReactions.Add(otherReaction);
                                    }
                                }
                            }
                        }
                        //var products = tanVM.ReactionParticipants.OfReactionOfType(reaction.Id, ParticipantType.Product).Select(p => p.Num);
                        //var Reactants = tanVM.ReactionParticipants.OfReactionOfType(reaction.Id, ParticipantType.Reactant).Select(p => p.Num);

                        //foreach (var otherReaction in tanVM.Reactions)
                        //{
                        //    if (otherReaction.Id != reaction.Id)
                        //    {
                        //        var otherReactionParticipants = tanVM.ReactionParticipants.OfReaction(otherReaction.Id);
                        //        var otherReactionProducts = otherReactionParticipants.OfType(ParticipantType.Product).Select(p => p.Num);
                        //        var OtherReactionReactants = otherReactionParticipants.OfType(ParticipantType.Reactant).Select(p => p.Num);
                        //        if (products.All(otherReactionProducts.Contains) && otherReactionProducts.All(products.Contains) && Reactants.All(OtherReactionReactants.Contains) && OtherReactionReactants.All(Reactants.Contains)
                        //            && !tanVM.Rsns.Where(rsn => (rsn.Reaction.Id == otherReaction.Id || rsn.Reaction.Id == reaction.Id) && rsn.CvtText.SafeContainsLower(S.CAT_PRE_USED)).Any())
                        //            duplicateReactions.Add(otherReaction);
                        //    }
                        //}
                    }
                    Debug.WriteLine($"Duplicate Reaction validation Done in {(DateTime.Now - start).TotalSeconds} seconds");

                    if (duplicateReactions.Count > 0)
                        foreach (var rxn in duplicateReactions)
                            tanErrors.Add(new ValidationError { ReactionVM = rxn, Message = "Duplicate Reaction", Category = ValidationError.RXN });
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                Mouse.OverrideCursor = null;
            });
            return tanErrors;
        }
        public static string GetRSDString(IEnumerable<ReactionParticipantVM> participants, ReactionVM reaction)
        {
            try
            {
                StringBuilder rsd = new StringBuilder();
                var products = participants.OfReactionOfType(reaction.Id, ParticipantType.Product).Select(product => product.Num + ((!string.IsNullOrEmpty(product.Yield) && product.Yield != "0") ? "(" + product.Yield + ")" : string.Empty));
                if (products != null && products.Any())
                    rsd.Append("P=" + string.Join(",", products));
                foreach (var stage in reaction.Stages)
                {
                    string ReactantString = string.Join(",", participants.OfStageOfType(stage.Id, ParticipantType.Reactant).Select(p => p.Num));
                    string solventString = string.Join(",", participants.OfStageOfType(stage.Id, ParticipantType.Solvent).Select(p => p.Num));
                    string AgentString = string.Join(",", participants.OfStageOfType(stage.Id, ParticipantType.Agent).Select(p => p.Num));
                    string CatalystString = string.Join(",", participants.OfStageOfType(stage.Id, ParticipantType.Catalyst).Select(p => p.Num));
                    rsd.Append(StringOfType(ReactantString, 'R') + StringOfType(AgentString, 'A') + StringOfType(solventString, 'S') + StringOfType(CatalystString, 'C') + ";");
                }
                return rsd.ToString().TrimEnd(';');
            }
            catch (Exception ex)
            {
                Log.This(ex);
                throw;
            }
        }
        private static string StringOfType(string text, char c)
        {
            if (!string.IsNullOrEmpty(text))
                return $"{c}={text}";
            return string.Empty;
        }
        public static void AddJonesReAgentRSN(TanVM tanVM)
        {
            var freeTexts = tanVM.Rsns.ToList();
            var ReactionWiseParticipants = tanVM.ReactionParticipants.GroupBy(rp => rp.ReactionVM.Id).ToDictionary(rp => rp.Key, rp => rp.ToList());
            foreach (var reaction in tanVM.Reactions)
            {
                if (ReactionWiseParticipants.ContainsKey(reaction.Id))
                {
                    var StageWiseParticipants = ReactionWiseParticipants[reaction.Id].Where(rp => rp.StageVM != null).GroupBy(rp => rp.StageVM.Id).ToDictionary(rp => rp.Key, rp => rp.ToList());
                    foreach (var stage in reaction.Stages)
                    {
                        if (StageWiseParticipants.ContainsKey(stage.Id))
                        {
                            var JonesReagents_CRO3 = StageWiseParticipants[stage.Id].OfType(ParticipantType.Agent).OfReg("1333820");
                            var JonesReagents_H2SO4 = StageWiseParticipants[stage.Id].OfType(ParticipantType.Agent).OfReg("7664939");
                            if ((JonesReagents_CRO3.Any() && JonesReagents_H2SO4.Any()))
                            {
                                if (!tanVM.Rsns.OfReaction(reaction.Id).OfFreetextContains(S.JONES_TEXT).Any() && !tanVM.Rsns.OfReactionAndStage(reaction.Id, stage.Id).OfFreetextContains(S.JONES_TEXT).Any())
                                    tanVM.Rsns.Add(new RsnVM
                                    {
                                        Id = Guid.NewGuid(),
                                        FreeText = (reaction.Stages.Count > 1 ? $"{S.JONES_TEXT} (stage {stage.DisplayOrder})" : S.JONES_TEXT),
                                        Reaction = reaction,
                                        Stage = (reaction.Stages.Count > 1 ? stage : null),
                                        IsRXN = (reaction.Stages.Count > 1 ? false : true)
                                    });
                                tanVM.ReactionParticipants.Remove(tanVM.ReactionParticipants.Where(rp => rp.Id == JonesReagents_CRO3.FirstOrDefault().Id).FirstOrDefault());
                                tanVM.ReactionParticipants.Remove(tanVM.ReactionParticipants.Where(rp => rp.Id == JonesReagents_H2SO4.FirstOrDefault().Id).FirstOrDefault());
                                AppInfoBox.ShowInfoMessage($"{S.JONES_TEXT} Added and CRO3,H2SO4 were removed from RSD");
                            }
                        }
                    }
                }
            }
        }

    }
}
