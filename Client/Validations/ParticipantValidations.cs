using Client.Common;
using Client.Logging;
using Client.Models;
using Client.Util;
using Client.ViewModels;
using DTO;
using Excelra.Utils.Library;
using ProductTracking.Models.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client.Validations
{
    public static class ParticipantValidations
    {
        public static ReactionParticipantVM AlreadyContains(TanChemical selectedChemical, ViewAnalogousVM analogousreactionVM, ReactionVM SelectedReaction, ParticipantType participantType, [Optional] List<ReactionParticipantVM> ReactionParticipant, [Optional] List<RsnVM> Rsn, [Optional] bool BulkValidation)
        {
            try
            {
                var ReactionParticipants = ReactionParticipant == null ? analogousreactionVM.ReactionParticipants.OfReaction(SelectedReaction.Id) : ReactionParticipant;
                var Rsns = Rsn == null ? analogousreactionVM.Rsns.OfReaction(SelectedReaction.Id,true).ToList() : Rsn;
                var chemicalName = new Models.TanChemicalVM
                {
                    Id = selectedChemical.Id,
                    Name = selectedChemical.Name,
                    NUM = selectedChemical.NUM,
                    RegNumber = selectedChemical.RegNumber
                };
                var groupedValidations = S.GetGroupedCVTs();
                if (ReactionParticipants.Count() > 0)
                {
                    if (participantType == ParticipantType.Product)
                    {
                        var existingProducts = ReactionParticipants.OfReaction(SelectedReaction.Id).OfNumOrReg(selectedChemical.NUM, selectedChemical.RegNumber);
                        if (existingProducts != null && existingProducts.Any())
                            return existingProducts.FirstOrDefault();
                    }

                    else if (SelectedReaction.SelectedStage != null)
                    {
                        #region Reactant Validations
                        if (participantType == ParticipantType.Reactant)
                        {
                            var stageReactants = ReactionParticipants.OfReactionAndStage(SelectedReaction.Id, SelectedReaction.SelectedStage.Id).OfNumOrReg(selectedChemical.NUM, selectedChemical.RegNumber).OfExceptTypes(ParticipantType.Solvent);
                            if (stageReactants != null && stageReactants.OfType(ParticipantType.Reactant).Any())
                                return stageReactants.FirstOrDefault();
                            List<int> StageNumbers = new List<int>();
                            if (SelectedReaction.Stages.Count > 1)
                                StageNumbers = PressureValidations.GetStageDisplayOrdersFromFreetexts(Rsns, SelectedReaction, S.DUAL_ROLE_STRING);
                            else if (Rsns.Where(rsn => rsn.Reaction.Id == SelectedReaction.Id && rsn.Stage == null && rsn.CvtText.SafeEqualsLower(S.DUAL_ROLE_STRING)).Any())
                                StageNumbers.Add(1);
                            if (stageReactants != null && stageReactants.OfExceptTypes(ParticipantType.Reactant).Any())
                            {
                                if (!StageNumbers.Contains(SelectedReaction.SelectedStage.DisplayOrder))
                                    return stageReactants.OfExceptTypes(ParticipantType.Reactant).FirstOrDefault();
                                else
                                {
                                    var rsnVM = Rsns.Where(rsn => rsn.Reaction.Id == SelectedReaction.Id && 
                                                                  rsn.SelectedChemical == null &&
                                                                  (rsn.FreeText.SafeContainsLower(S.DUAL_ROLE_STRING) || rsn.CvtText.SafeEqualsLower(S.DUAL_ROLE_STRING))).FirstOrDefault();
                                    if (rsnVM != null)
                                        rsnVM.SelectedChemical = chemicalName;
                                }
                            }

                            var Products = ReactionParticipants.OfReactionOfType(SelectedReaction.Id, ParticipantType.Product).OfNum(selectedChemical.NUM);
                            if (Products != null && Products.Any())
                                return Products.FirstOrDefault();

                            var AmdProducts = ReactionParticipants.OfReactionOfType(SelectedReaction.Id, ParticipantType.Product).Where(ro => ro.Reg == selectedChemical.RegNumber && ro.Num != selectedChemical.NUM);
                            if (AmdProducts != null && AmdProducts.Any())
                            {
                                string msg = $"Selected chemical with same Reg and different num already involved in {AmdProducts.FirstOrDefault().ReactionVM.DisplayName} as a Product. Do you want to still add as Reactant?";
                                DialogResult dialogResult = System.Windows.Forms.MessageBox.Show(msg, "Reactions", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                if (dialogResult == DialogResult.No)
                                    return AmdProducts.FirstOrDefault();
                            }

                            var otherstageReactants = ReactionParticipants.OfReactionAndExceptStage(SelectedReaction.Id, SelectedReaction.SelectedStage.Id)
                                                                          .OfNumOrReg(selectedChemical.NUM, selectedChemical.RegNumber)
                                                                          .OfExceptTypes(ParticipantType.Solvent, ParticipantType.Product)
                                                                          .GroupBy(p => p.ParticipantType)
                                                                          .ToDictionary(p => p.Key, p => p.ToList());
                            var duplicate = ValidateWithCVT1(otherstageReactants, Rsns, SelectedReaction, chemicalName, ParticipantType.Reactant);
                            if (duplicate != null)
                                return duplicate;
                        }
                        #endregion

                        #region Solvent Validation
                        if (participantType == ParticipantType.Solvent)
                        {
                            var InStageSolvents = ReactionParticipants.OfReactionAndStage(SelectedReaction.Id, SelectedReaction.SelectedStage.Id).OfExceptTypes(ParticipantType.Reactant).OfNumOrReg(selectedChemical.NUM, selectedChemical.RegNumber);
                            if (InStageSolvents != null && InStageSolvents.OfType(ParticipantType.Solvent).Any())
                                return InStageSolvents.FirstOrDefault();
                            List<int> StageNumbers = new List<int>();
                            if (SelectedReaction.Stages.Count > 1)
                                StageNumbers = PressureValidations.GetStageDisplayOrdersFromFreetexts(Rsns, SelectedReaction, S.DUAL_ROLE_STRING);
                            else if (Rsns.Where(rsn => rsn.Reaction.Id == SelectedReaction.Id && rsn.Stage == null && rsn.CvtText.SafeEqualsLower(S.DUAL_ROLE_STRING)).Any())
                                StageNumbers.Add(1);
                            if (InStageSolvents != null && InStageSolvents.OfExceptTypes(ParticipantType.Solvent).Any())
                            {
                                if (!StageNumbers.Contains(SelectedReaction.SelectedStage.DisplayOrder))
                                    return InStageSolvents.OfExceptTypes(ParticipantType.Solvent).FirstOrDefault();
                                else
                                {
                                    var rsnVM = Rsns.Where(rsn => rsn.Reaction.Id == SelectedReaction.Id &&
                                                                  rsn.SelectedChemical == null &&
                                                                  (rsn.FreeText.SafeContainsLower(S.DUAL_ROLE_STRING) || rsn.CvtText.SafeEqualsLower(S.DUAL_ROLE_STRING))).FirstOrDefault();
                                    if (rsnVM != null)
                                        rsnVM.SelectedChemical = chemicalName;
                                }
                            }

                            var Products = ReactionParticipants.OfReactionOfType(SelectedReaction.Id, ParticipantType.Product).OfNumOrReg(selectedChemical.NUM, selectedChemical.RegNumber);
                            if (Products != null && Products.Any())
                                return Products.FirstOrDefault();
                            var otherthanProductsAndSolvents = ReactionParticipants.OfReactionAndExceptStage(SelectedReaction.Id,SelectedReaction.SelectedStage.Id)
                                                                                   .OfNumOrReg(selectedChemical.NUM, selectedChemical.RegNumber)
                                                                                   .OfExceptTypes(ParticipantType.Solvent, ParticipantType.Reactant, ParticipantType.Product)
                                                                                   .GroupBy(p => p.ParticipantType)
                                                                                   .ToDictionary(p => p.Key, p => p.ToList());
                            var duplicate = ValidateWithCVT1(otherthanProductsAndSolvents, Rsns, SelectedReaction, chemicalName, ParticipantType.Solvent);
                            if (duplicate != null)
                                return duplicate;
                        }
                        #endregion

                        #region Catalyst Validations
                        if (participantType == ParticipantType.Catalyst)
                        {
                            var InStageCatalysts = ReactionParticipants.OfReactionAndStage(SelectedReaction.Id, SelectedReaction.SelectedStage.Id).OfNumOrReg(selectedChemical.NUM, selectedChemical.RegNumber);
                            if (InStageCatalysts != null && InStageCatalysts.OfType(ParticipantType.Catalyst).Any())
                                return InStageCatalysts.FirstOrDefault();
                            List<int> StageNumbers = new List<int>();
                            if (SelectedReaction.Stages.Count > 1)
                                StageNumbers = PressureValidations.GetStageDisplayOrdersFromFreetexts(Rsns, SelectedReaction, S.DUAL_ROLE_STRING);
                            else if (Rsns.Where(rsn => rsn.Reaction.Id == SelectedReaction.Id && rsn.Stage == null && rsn.CvtText.SafeEqualsLower(S.DUAL_ROLE_STRING)).Any())
                                StageNumbers.Add(1);
                            if (InStageCatalysts != null && InStageCatalysts.OfExceptTypes(ParticipantType.Catalyst).Any())
                            {
                                if (!StageNumbers.Contains(SelectedReaction.SelectedStage.DisplayOrder))
                                    return InStageCatalysts.OfExceptTypes(ParticipantType.Catalyst).FirstOrDefault();
                                else
                                {
                                    var rsnVM = Rsns.Where(rsn => rsn.Reaction.Id == SelectedReaction.Id &&
                                                                  rsn.SelectedChemical == null &&
                                                                  (rsn.FreeText.SafeContainsLower(S.DUAL_ROLE_STRING) || rsn.CvtText.SafeEqualsLower(S.DUAL_ROLE_STRING))).FirstOrDefault();
                                    if (rsnVM != null)
                                        rsnVM.SelectedChemical = chemicalName;
                                }
                            }
                            var Products = ReactionParticipants.OfReactionOfType(SelectedReaction.Id, ParticipantType.Product).OfNumOrReg(selectedChemical.NUM, selectedChemical.RegNumber);
                            if (Products != null && Products.Any())
                                return Products.FirstOrDefault();

                            var otherThanCatalysts = ReactionParticipants.OfReactionAndExceptStage(SelectedReaction.Id, SelectedReaction.SelectedStage.Id)
                                                                         .OfNumOrReg(selectedChemical.NUM, selectedChemical.RegNumber)
                                                                         .OfExceptTypes(ParticipantType.Catalyst, ParticipantType.Product)
                                                                         .GroupBy(p => p.ParticipantType)
                                                                         .ToDictionary(p => p.Key, p => p.ToList());
                            var duplicate = ValidateWithCVT1(otherThanCatalysts, Rsns, SelectedReaction, chemicalName, ParticipantType.Catalyst);
                            if (duplicate != null)
                                return duplicate;
                        }
                        #endregion
                        #region Agent Validations
                        if (participantType == ParticipantType.Agent)
                        {
                            var InStageAgents = ReactionParticipants.OfReactionAndStage(SelectedReaction.Id, SelectedReaction.SelectedStage.Id).OfNumOrReg(selectedChemical.NUM, selectedChemical.RegNumber);
                            if (InStageAgents != null && InStageAgents.OfType(ParticipantType.Agent).Any())
                                return InStageAgents.FirstOrDefault();
                            List<int> StageNumbers = new List<int>();
                            if (SelectedReaction.Stages.Count > 1)
                                StageNumbers = PressureValidations.GetStageDisplayOrdersFromFreetexts(Rsns, SelectedReaction, S.DUAL_ROLE_STRING);
                            else if (Rsns.Where(rsn => rsn.Reaction.Id == SelectedReaction.Id && rsn.Stage == null && rsn.CvtText.SafeEqualsLower(S.DUAL_ROLE_STRING)).Any())
                                StageNumbers.Add(1);
                            if (InStageAgents != null && InStageAgents.OfExceptTypes(ParticipantType.Agent).Any())
                            {
                                if (!StageNumbers.Contains(SelectedReaction.SelectedStage.DisplayOrder))
                                    return InStageAgents.OfExceptTypes(ParticipantType.Agent).FirstOrDefault();
                                else
                                {
                                    var rsnVM = Rsns.Where(rsn => rsn.Reaction.Id == SelectedReaction.Id &&
                                                                  rsn.SelectedChemical == null &&
                                                                  (rsn.FreeText.SafeContainsLower(S.DUAL_ROLE_STRING) || rsn.CvtText.SafeEqualsLower(S.DUAL_ROLE_STRING))).FirstOrDefault();
                                    if (rsnVM != null)
                                        rsnVM.SelectedChemical = chemicalName;
                                }
                            }

                            var Products = ReactionParticipants.OfReactionOfType(SelectedReaction.Id, ParticipantType.Product).OfNumOrReg(selectedChemical.NUM, selectedChemical.RegNumber);
                            if (Products != null && Products.Any())
                                return Products.FirstOrDefault();

                            var otherStageAgents = ReactionParticipants.OfReactionAndExceptStage(SelectedReaction.Id, SelectedReaction.SelectedStage.Id)
                                                                       .OfNumOrReg(selectedChemical.NUM, selectedChemical.RegNumber)
                                                                       .OfExceptTypes(ParticipantType.Agent, ParticipantType.Product)
                                                                       .GroupBy(p => p.ParticipantType)
                                                                       .ToDictionary(p => p.Key, p => p.ToList());

                            var duplicate = ValidateWithCVT1(otherStageAgents, Rsns, SelectedReaction, chemicalName, ParticipantType.Agent);
                            if (duplicate != null)
                                return duplicate;
                        }
                        #endregion
                    }
                    else
                        return null;
                }
                return null;
            }
            catch (Exception ex)
            {
                Log.This(ex);
                return null;
            }
        }

        public static ReactionParticipantVM ValidateWithCVT1(Dictionary<ParticipantType, List<ReactionParticipantVM>> otherstageReactants, List<RsnVM> Rsns, ReactionVM SelectedReaction, TanChemicalVM chemicalName, ParticipantType participanttype)
        {
            var groupedValidations = S.GetGroupedCVTs();
            if (otherstageReactants != null && otherstageReactants.Any())
            {
                foreach (var type in otherstageReactants.Keys)
                {
                    if (groupedValidations.ContainsKey(participanttype) && groupedValidations[participanttype].Select(s => s.ExistingType).Contains(type))
                    {
                        var cvt = groupedValidations[participanttype].Where(c => c.ExistingType == type).FirstOrDefault();
                        List<int> StageNumbers = PressureValidations.GetStageDisplayOrdersFromFreetexts(Rsns.ToList(), SelectedReaction, cvt.AssociatedFreeText);
                        if (StageNumbers.Contains(SelectedReaction.SelectedStage.DisplayOrder))
                        {
                            var rsnVM = Rsns.Where(rsn => rsn.Reaction.Id == SelectedReaction.Id && rsn.Stage != null && StageNumbers.Contains(rsn.Stage.DisplayOrder) && rsn.SelectedChemical == null &&
                            rsn.FreeText.SafeContainsLower(cvt.AssociatedFreeText)).FirstOrDefault();
                            if (rsnVM != null)
                                rsnVM.SelectedChemical = chemicalName;
                            return null;
                        }
                        else
                            return otherstageReactants[type].FirstOrDefault();
                    }
                    else
                        return otherstageReactants.Select(s => s.Value).FirstOrDefault().FirstOrDefault();
                }
            }
            return null;
        }
    }
}
