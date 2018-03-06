using Client.Logging;
using Client.Models;
using Client.Styles;
using Client.ViewModels;
using Client.XML;
using DTO;
using Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client.Common
{
    public static class ReactionViewBuilder
    {
        private static readonly string PLUS_ICON = "/Images/equationPlus.png";
        private static readonly string ARROW_ICON = "/Images/equationNext.png";

        public static ReactionViewVM GetReactionView(ReactionVM reaction, List<ReactionParticipantVM> ReactionParticipants)
        {

            try
            {
                ReactionViewVM reactionView = new ReactionViewVM();
                if (reaction != null && ReactionParticipants != null && ReactionParticipants.Count > 0)
                {
                    reactionView.Name = reaction.Name;
                    reactionView.IsReviewCompleted = reaction.IsReviewCompleted;
                    reactionView.ReviewEnable = U.RoleId == 2 ? true : false;
                    reactionView.ReactionId = reaction.Id;

                    #region Reactants
                    var reactants = ReactionParticipants.Where(rp => rp.ReactionVM == reaction
                               && rp.ParticipantType == ParticipantType.Reactant).ToList();
                    foreach (var reactant in reactants)
                    {
                        var view = GetParticipantView(reactant);
                        view.NextImagePath = PLUS_ICON;
                        reactionView.ReactantsViews.Add(view);
                    }
                    var lastReactant = reactionView.ReactantsViews.LastOrDefault();
                    if (lastReactant != null)
                        lastReactant.NextImagePath = ARROW_ICON;
                    #endregion

                    foreach (var view in reactionView.ReactantsViews)
                        reactionView.EquationViews.Add(view);

                    #region Products
                    var products = ReactionParticipants.Where(rp => rp.ReactionVM == reaction
                               && rp.ParticipantType == ParticipantType.Product).ToList();
                    foreach (var product in products)
                    {
                        var view = GetParticipantView(product);
                        view.NextImagePath = PLUS_ICON;
                        reactionView.ProductViews.Add(view);
                    }
                    var lastProduct = reactionView.ProductViews.LastOrDefault();
                    if (lastProduct != null)
                        lastProduct.NextImagePath = null;
                    #endregion
                    foreach (var view in reactionView.ProductViews)
                        reactionView.EquationViews.Add(view);

                    foreach (var stage in reaction.Stages)
                    {
                        var stageView = new StageViewVM() { StageVM = stage };

                        var agentViews = (from rp in ReactionParticipants
                                          where rp.ReactionVM == reaction && rp.StageVM != null && rp.StageVM.Id == stage.Id && rp.ParticipantType == ParticipantType.Agent
                                          select GetParticipantView(rp));
                        foreach (var agentView in agentViews)
                            stageView.AgentsViews.Add(agentView);
                        stageView.ShowAgents = stageView.AgentsViews.Any() ? Visibility.Visible : Visibility.Collapsed;

                        var reactantViews = (from rp in ReactionParticipants
                                            where rp.ReactionVM == reaction && rp.StageVM != null && rp.StageVM.Id == stage.Id && rp.ParticipantType == ParticipantType.Reactant
                                            select GetParticipantView(rp));
                        foreach (var reactantView in reactantViews)
                            stageView.ReactantsViews.Add(reactantView);
                        stageView.ShowReactants = stageView.ReactantsViews.Any() ? Visibility.Visible : Visibility.Collapsed;

                        var solventViews = (from rp in ReactionParticipants
                                            where rp.ReactionVM == reaction && rp.StageVM != null && rp.StageVM.Id == stage.Id && rp.ParticipantType == ParticipantType.Solvent
                                            select GetParticipantView(rp));
                        foreach (var solventView in solventViews)
                            stageView.SolventsViews.Add(solventView);
                        stageView.ShowSolvents = stageView.SolventsViews.Any() ? Visibility.Visible : Visibility.Collapsed;

                        var catalystViews = (from rp in ReactionParticipants
                                             where rp.ReactionVM == reaction && rp.StageVM != null && rp.StageVM.Id == stage.Id && rp.ParticipantType == ParticipantType.Catalyst
                                             select GetParticipantView(rp));
                        foreach (var catalystView in catalystViews)
                            stageView.CatalystsViews.Add(catalystView);
                        stageView.ShowCatalysts = stageView.CatalystsViews.Any() ? Visibility.Visible : Visibility.Collapsed;

                        stageView.Temperature = (stage.Conditions != null && stage.Conditions.Select(temp => temp.Temperature).Any()) ? string.Join(",", stage.Conditions.Select(temp => temp.Temperature).ToList()) : string.Empty;
                        stageView.Time = (stage.Conditions != null && stage.Conditions.Select(temp => temp.Time).Any()) ? string.Join(",", stage.Conditions.Select(temp => temp.Time).ToList()) : string.Empty;
                        stageView.Pressure = (stage.Conditions != null && stage.Conditions.Select(pres => pres.Pressure).Any()) ? string.Join(",", stage.Conditions.Select(pres => pres.Pressure).ToList()) : string.Empty;
                        stageView.Ph = (stage.Conditions != null && stage.Conditions.Select(ph => ph.PH).Any()) ? string.Join(",", stage.Conditions.Select(ph => ph.PH).ToList()) : string.Empty;
                        stageView.ConditionsVisibility = stageView.GetConditionsVisibility();
                        //stageView.Temperature = string.Join(",", stage.Conditions.Where(temp => !string.IsNullOrEmpty(temp.Temperature)).Select(temp => temp.Temperature).ToList());
                        //stageView.Time = string.Join(",", stage.Conditions.Where(temp => !string.IsNullOrEmpty(temp.Time)).Select(temp => temp.Time).ToList());
                        //stageView.Pressure = string.Join(",", stage.Conditions.Where(pres => !string.IsNullOrEmpty(pres.Pressure)).Select(pres => pres.Pressure).ToList());
                        //stageView.Ph = string.Join(",", stage.Conditions.Where(ph => !string.IsNullOrEmpty(ph.PH)).Select(ph => ph.PH).ToList());

                        var rsns = reaction.TanVM.Rsns.Where(rsn => rsn.Stage != null && rsn.Stage.Id == stage.Id).ToList();
                        if (stage.DisplayOrder == 1)
                            rsns.InsertRange(0, reaction.TanVM.Rsns.Where(rsn => rsn.Reaction.Id == reaction.Id && rsn.Stage == null).ToList());
                        stageView.FreeText = rsns.Select(rsn => rsn.FreeTextWithRxn).Count() > 0 ? string.Join(",", rsns.Where(rsn => !string.IsNullOrEmpty(rsn.FreeTextWithRxn)).Select(rsn => rsn.FreeTextWithRxn)) : string.Empty;
                        stageView.CVT = string.Join(",", rsns.Select(rsn => rsn.Stage == null ? !string.IsNullOrEmpty(rsn.CvtText) ? $"{rsn.CvtText} (Reaction)" : string.Empty : rsn.CvtText).Where(s => !string.IsNullOrEmpty(s)));

                        reactionView.Stages.Add(stageView);
                    }
                    reactionView.YieldProducts = new ObservableCollection<string>(products.Select(p => (p.Num + ((!string.IsNullOrEmpty(p.Yield) && p.Yield != "0") ? " (" + p.Yield + ")" : string.Empty))));
                }
                return reactionView;
            }
            catch (Exception ex)
            {
                Log.This(ex);
                throw;
            }
        }

        private static ReactionParticipantViewVM GetParticipantView(ReactionParticipantVM participant)
        {

            try
            {
                var mainViewModel = (App.Current.MainWindow as MainWindow).DataContext as MainVM;
                var participantView = new ReactionParticipantViewVM();
                participantView.ShortName = participant.Num.ToString();
                participantView.TooltipText = participant.Reg + " - " + participant.Name;
                participantView.Name = participant.Name + "(" + participant.Num + ")";
                participantView.Formula = participant.Formula;
                participantView.Num = participant.Num;
                participantView.StageVM = participant.StageVM;
                participantView.Reg = participant.Reg;
                participantView.NextImagePath = PLUS_ICON;

                #region Color
                if (participant.ParticipantType == ParticipantType.Product)
                {
                    participantView.BorderBrush = StyleConstants.ProductBrush;
                    participantView.BgColor = StyleConstants.ProductColor.Name;
                }
                else if (participant.ParticipantType == ParticipantType.Reactant)
                {
                    participantView.BorderBrush = StyleConstants.ReactantBrush;
                    participantView.BgColor = StyleConstants.ReactantColor.Name;
                }
                else if (participant.ParticipantType == ParticipantType.Agent)
                    participantView.BorderBrush = StyleConstants.AgentBrush;
                else if (participant.ParticipantType == ParticipantType.Catalyst)
                    participantView.BorderBrush = StyleConstants.CatalystBrush;
                else if (participant.ParticipantType == ParticipantType.Solvent)
                    participantView.BorderBrush = StyleConstants.SolventBrush;
                #endregion

                #region Chemical Name
                TanChemicalVM chemicalName = null;
                if (participant.ChemicalType == ChemicalType.S8500 || participant.ChemicalType == ChemicalType.S9000)
                {
                    chemicalName = S.Find(participant.Reg);
                }
                else if (participant.ChemicalType == ChemicalType.NUM || participant.ChemicalType == ChemicalType.S8000)
                {
                    var tanChemical = (from p in mainViewModel.TanVM.TanChemicals where p.NUM == participant.Num select p).FirstOrDefault();
                    if (tanChemical != null)
                    {
                        chemicalName = new Models.TanChemicalVM
                        {
                            RegNumber = tanChemical.RegNumber,
                            NUM = tanChemical.NUM,
                            ChemicalType = tanChemical.ChemicalType,
                            Name = tanChemical.Name,
                            ImagePath = tanChemical.ImagePath,
                            MolString = tanChemical.MolString,
                            AllImagePaths = tanChemical.Substancepaths.Select(s => s.ImagePath).Distinct().ToList(),
                            StereoChemisrty = tanChemical.ABSSterio
                        };
                    }
                }
                participantView.ChemicalName = chemicalName;
                #endregion
                return participantView;
            }
            catch (Exception ex)
            {
                Log.This(ex);
                throw;
            }
        }
    }
}
