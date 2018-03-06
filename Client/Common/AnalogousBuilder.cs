using Client.Logging;
using Client.Models;
using Client.ViewModels;
using Client.Views;
using DTO;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Windows.Data;
using Telerik.Windows.Controls;
using Client.Util;

namespace Client.Common
{
    public static class AnalogousBuilder
    {

        private static String AG = "AG";
        private static String RXNID = "RXNID";
        private static String STAGEID = "STAGEID";
        private static String SNO = "SNO";
        private static String RXN = "RXN";
        private static String REACTIONCVT = "ReactionCVT";
        private static String REACTIONFREETEXT = "ReactionFreeText";
        private static String STAGE = "STAGE";
        private static String PRODUCT = "PRODUCT";
        private static String YIELD = "YIELD";
        private static String TEMPERATURE = "TEMP";
        private static String PRESSURE = "PRESSURE";
        private static String TIME = "TIME";
        private static String PH = "PH";
        private static String STAGECVT = "StageCVT";
        private static String STAGEFREETEXT = "StageFreeText";
        private static List<String> ParticipantHeaders = new List<String> {
            ParticipantType.Reactant.ToString(),
            ParticipantType.Solvent.ToString(),
            ParticipantType.Agent.ToString(),
            ParticipantType.Catalyst.ToString()
        };

        static private GridViewColumnGroup productGroup = new Telerik.Windows.Controls.GridViewColumnGroup { Name = PRODUCT };
        static private GridViewColumnGroup yieldGroup = new Telerik.Windows.Controls.GridViewColumnGroup { Name = YIELD };
        static List<TanChemicalVM> ChemicalsList = new List<TanChemicalVM>();
        public static ViewAnalogousVM GenerateNewAnalogousReactions(
            ReactionVM SelectedReaction,
            bool IsReactantsSelected,
            bool IsSolventsSelected,
            bool IsAgentsSelected,
            bool IsCatalystSelected,
            bool IspHSelected,
            bool IsTempSelected,
            bool IsTimeSelected,
            bool IsPressureSelected,
            bool IsRSNSelected,
            int ReactionsCountToCopy, int IndexToAdd
            )
        {
            ViewAnalogousVM vm = new ViewAnalogousVM();
            var mainViewModel = ((MainWindow)(App.Current.MainWindow)).DataContext as MainVM;
            mainViewModel.ProgressText = "Creating Analogous Reacions . .";
            try
            {
                vm.SelectedMasterReaction = SelectedReaction;
                List<ReactionParticipantVM> allParticipants = new List<ReactionParticipantVM>();
                List<RsnVM> tanRsns = new List<RsnVM>();
                List<ReactionVM> tanReactions = new List<ReactionVM>();
                var tanParticipants = new List<ReactionParticipantVM>();
                if (IsReactantsSelected)
                {
                    var reactants = mainViewModel.TanVM.ReactionParticipants.OfType(ParticipantType.Reactant);
                    allParticipants.AddRange(reactants);
                }
                if (IsSolventsSelected)
                {
                    
                    var solvents = mainViewModel.TanVM.ReactionParticipants.OfType(ParticipantType.Solvent);
                    allParticipants.AddRange(solvents);
                }

                if (IsAgentsSelected)
                {
                    var agents = mainViewModel.TanVM.ReactionParticipants.OfType(ParticipantType.Agent);
                    allParticipants.AddRange(agents);
                }

                if (IsCatalystSelected)
                {
                    var catalysts = mainViewModel.TanVM.ReactionParticipants.OfType(ParticipantType.Catalyst);
                    allParticipants.AddRange(catalysts);
                }
                for (int i = 0; i < ReactionsCountToCopy; i++)
                {
                    var reaction = new ReactionVM
                    {
                        DisplayOrder = ++IndexToAdd,
                        Id = Guid.NewGuid(),
                        TanVM = SelectedReaction.TanVM,
                        AnalogousVMId = SelectedReaction.Id
                    };
                    #region Stages
                    var stages = new List<StageVM>();
                    foreach (var masterStage in SelectedReaction.Stages)
                    {
                        bool ValidStage = true;
                        var analogousStage = new StageVM { Id = Guid.NewGuid(), ReactionVm = reaction };
                        var Conditions = new List<StageConditionVM>();
                        if (masterStage.Conditions != null)
                        {
                            foreach (var selectedCondition in masterStage.Conditions)
                            {
                                var condition = new StageConditionVM
                                {
                                    DisplayOrder = selectedCondition.DisplayOrder,
                                    Id = Guid.NewGuid(),
                                    PH = IspHSelected ? selectedCondition.PH : "",
                                    Pressure = IsPressureSelected ? selectedCondition.Pressure : "",
                                    StageId = analogousStage.Id,
                                    Temperature = IsTempSelected ? selectedCondition.Temperature : "",
                                    Time = IsTimeSelected ? selectedCondition.Time : "",
                                    TIME_TYPE = IsTimeSelected ? selectedCondition.TIME_TYPE : "",
                                    PRESSURE_TYPE = IsPressureSelected ? selectedCondition.PRESSURE_TYPE : "",
                                    TEMP_TYPE = IsTempSelected ? selectedCondition.TEMP_TYPE : "",
                                    PH_TYPE = IspHSelected ? selectedCondition.PH_TYPE : "",
                                };
                                if ((IspHSelected && !string.IsNullOrEmpty(selectedCondition.PH)) || (IsPressureSelected && !string.IsNullOrEmpty(selectedCondition.Pressure)) ||
                                    (IsTempSelected && !string.IsNullOrEmpty(selectedCondition.Temperature)) || (IsTimeSelected && !string.IsNullOrEmpty(selectedCondition.Time)))
                                    Conditions.Add(condition);
                            }
                            analogousStage.SetConditions(Conditions);
                        }
                        var stageParticipants = (from sp in allParticipants where sp.StageVM.Id == masterStage.Id select sp).ToList();
                        foreach (var stageParticipant in stageParticipants)
                        {
                            var newParticipant = new ReactionParticipantVM
                            {
                                ChemicalType = stageParticipant.ChemicalType,
                                DisplayOrder = stageParticipant.DisplayOrder,
                                Name = stageParticipant.Name,
                                Num = stageParticipant.Num,
                                ParticipantType = stageParticipant.ParticipantType,
                                ReactionVM = reaction,
                                Reg = stageParticipant.Reg,
                                StageVM = analogousStage,
                                TanChemicalId = stageParticipant.TanChemicalId,
                                Yield = stageParticipant.Yield,
                                Id = Guid.NewGuid()
                            };
                            tanParticipants.Add(newParticipant);
                        }
                        if (IsRSNSelected)
                        {
                            var stagersnList = (from rsn in mainViewModel.TanVM.Rsns where rsn.Reaction.Id == SelectedReaction.Id && rsn.Stage != null && rsn.Stage.Id == masterStage.Id select rsn).ToList();
                            foreach (var rsn in stagersnList)
                            {
                                var newRsn = new RsnVM
                                {
                                    CvtText = rsn.CvtText,
                                    Reaction = reaction,
                                    IsRXN = rsn.IsRXN,
                                    Stage = analogousStage,
                                    FreeText = rsn.FreeText,
                                    Id = Guid.NewGuid(),
                                    IsIgnorableInDelivery = rsn.IsIgnorableInDelivery,
                                    DisplayOrder = rsn.DisplayOrder,
                                    ReactionParticipantId = rsn.ReactionParticipantId,
                                    SelectedChemical = rsn.SelectedChemical                                   
                                };
                                tanRsns.Add(newRsn);
                            }
                        }
                        if ((analogousStage.Conditions == null || !analogousStage.Conditions.Any()) &&
                            (tanParticipants == null || !tanParticipants.Where(tp => tp.StageVM != null && tp.StageVM.Id == analogousStage.Id).Any()) &&
                            (tanRsns == null || !tanRsns.Where(tp => tp.Stage != null && tp.Stage.Id == analogousStage.Id).Any()) && !mainViewModel.TanVM.ReactionParticipants.OfReactionAndStage(SelectedReaction.Id,masterStage.Id).OfType(ParticipantType.Reactant).Any())
                            ValidStage = false;
                        if (ValidStage)
                            stages.Add(analogousStage);
                    }
                    #endregion
                    reaction.SetStages(stages, 0, false, true);
                    tanReactions.Add(reaction);
                    var reactionParticipants = (from sp in allParticipants where sp.ReactionVM.Id == SelectedReaction.Id && sp.StageVM == null select sp).ToList();
                    foreach (var participant in reactionParticipants)
                    {
                        var newParticipant = new ReactionParticipantVM
                        {
                            ChemicalType = participant.ChemicalType,
                            DisplayOrder = participant.DisplayOrder,
                            Name = participant.Name,
                            Num = participant.Num,
                            ParticipantType = participant.ParticipantType,
                            ReactionVM = reaction,
                            Reg = participant.Reg,
                            StageVM = null,
                            TanChemicalId = participant.TanChemicalId,
                            Yield = participant.Yield,
                            Id = Guid.NewGuid()
                        };
                        tanParticipants.Add(newParticipant);
                    }

                    if (IsRSNSelected)
                    {
                        var reationrsnList = (from rsn in mainViewModel.TanVM.Rsns where rsn.Reaction.Id == SelectedReaction.Id && rsn.Stage == null select rsn).ToList();
                        foreach (var rsn in reationrsnList)
                        {
                            var newRsn = new RsnVM
                            {
                                CvtText = rsn.CvtText,
                                Reaction = reaction,
                                FreeText = rsn.FreeText,
                                IsRXN = rsn.IsRXN,
                                Stage = null,
                                Id = Guid.NewGuid(),
                                SelectedChemical = rsn.SelectedChemical,
                                ReactionParticipantId = rsn.ReactionParticipantId,
                                DisplayOrder = rsn.DisplayOrder,
                                IsIgnorableInDelivery = rsn.IsIgnorableInDelivery
                            };
                            tanRsns.Add(newRsn);
                        }
                    }
                }
                foreach (var participant in tanParticipants)
                {
                    vm.AllParticipants.Add(participant);
                    vm.ReactionParticipants.Add(participant);
                }
                vm.Rsns.Clear();
                foreach (var rsn in tanRsns)
                    vm.Rsns.Add(rsn);
                foreach (var reaction in tanReactions)
                    vm.AnalogousReactions.Add(reaction);
            }
            catch (Exception ex)
            {
                AppErrorBox.ShowErrorMessage("Can't Create Analogous Reactions", ex.Message);
            }
            return vm;
        }
        public static void BuildAnalogousGrid(ViewAnalogousVM anagousReactionVM, RadGridView ConsolidatedGrid, bool NewAnalogous)
        {
            try
            {
                ConsolidatedGrid.ItemsSource = null;
                ConsolidatedGrid.Columns.Clear();
                ConsolidatedGrid.ColumnGroups.Clear();
                ConsolidatedGrid.Items.Clear();

                var mainViewModel = ((MainWindow)(App.Current.MainWindow)).DataContext as MainVM;
                if (anagousReactionVM != null)
                {
                    #region Add Columns
                    
                    var maxProducts = mainViewModel.TanVM.ReactionParticipants.OfReactionOfType(anagousReactionVM.SelectedMasterReaction.Id, ParticipantType.Product).Select(p => p.DisplayOrder).Distinct().OrderByDescending(o => o).FirstOrDefault();
                    var maxStage = anagousReactionVM.AnalogousReactions.Any() ? anagousReactionVM.AnalogousReactions[0]?.Stages.Count : 0;

                    GridViewDataColumn ReactionIdColumn = new Telerik.Windows.Controls.GridViewDataColumn();
                    ReactionIdColumn.Header = RXNID;
                    ReactionIdColumn.DataMemberBinding = new Binding(RXNID);
                    ReactionIdColumn.IsVisible = false;
                    ConsolidatedGrid.Columns.Add(ReactionIdColumn);

                    var snoNumberColumn = new Telerik.Windows.Controls.GridViewDataColumn();
                    snoNumberColumn.Header = SNO;
                    snoNumberColumn.DataMemberBinding = new Binding(SNO);
                    snoNumberColumn.IsReadOnly = true;
                    snoNumberColumn.Width = 40;
                    ConsolidatedGrid.Columns.Add(snoNumberColumn);

                    var rxnColumn = new Telerik.Windows.Controls.GridViewDataColumn();
                    rxnColumn.Header = RXN;
                    rxnColumn.DataMemberBinding = new Binding(RXN);
                    rxnColumn.IsReadOnly = true;
                    rxnColumn.Width = 100;
                    ConsolidatedGrid.Columns.Add(rxnColumn);

                    var cvtColumn = new Telerik.Windows.Controls.GridViewDataColumn();
                    cvtColumn.Header = REACTIONCVT;
                    cvtColumn.DataMemberBinding = new Binding(REACTIONCVT);
                    cvtColumn.IsReadOnly = true;
                    cvtColumn.Width = 100;
                    ConsolidatedGrid.Columns.Add(cvtColumn);

                    var FreeTextColumn = new Telerik.Windows.Controls.GridViewDataColumn();
                    FreeTextColumn.Header = REACTIONFREETEXT;
                    FreeTextColumn.DataMemberBinding = new Binding(REACTIONFREETEXT);
                    FreeTextColumn.IsReadOnly = true;
                    FreeTextColumn.Width = 100;
                    ConsolidatedGrid.Columns.Add(FreeTextColumn);

                    //for (int productIndex = 1; productIndex <= maxProducts; productIndex++)
                    //{
                    GridViewDataColumn ProductIdColumn = new Telerik.Windows.Controls.GridViewDataColumn();
                    ProductIdColumn.Header = PRODUCT + "ID";
                    ProductIdColumn.DataMemberBinding = new Binding(PRODUCT + "ID");
                    ProductIdColumn.IsVisible = false;
                    ConsolidatedGrid.Columns.Add(ProductIdColumn);

                    var ProductColumn = new Telerik.Windows.Controls.GridViewDataColumn();
                    ProductColumn.Header = PRODUCT;
                    ProductColumn.DataMemberBinding = new Binding(PRODUCT);
                    ProductColumn.ColumnGroupName = PRODUCT;
                    ProductColumn.Width = 100;
                    ConsolidatedGrid.Columns.Add(ProductColumn);
                    //}

                    //for (int productIndex = 1; productIndex <= maxProducts; productIndex++)
                    //{
                    var YieldColumn = new Telerik.Windows.Controls.GridViewDataColumn();
                    YieldColumn.Header = YIELD;
                    YieldColumn.DataMemberBinding = new Binding(YIELD);
                    YieldColumn.ColumnGroupName = YIELD;
                    YieldColumn.Width = 45;
                    ConsolidatedGrid.Columns.Add(YieldColumn);
                    //}

                    for (int stageIndex = 1; stageIndex <= maxStage; stageIndex++)
                    {
                        var stageGroup = new Telerik.Windows.Controls.GridViewColumnGroup { Name = STAGE + stageIndex };
                        ConsolidatedGrid.ColumnGroups.Add(stageGroup);

                        foreach (var participantName in ParticipantHeaders)
                        {
                            GridViewDataColumn IdColumn = new Telerik.Windows.Controls.GridViewDataColumn();
                            IdColumn.Header = participantName + stageIndex + "ID";
                            IdColumn.DataMemberBinding = new Binding(participantName + stageIndex + "ID");
                            IdColumn.ColumnGroupName = stageGroup.Name;
                            IdColumn.IsVisible = false;
                            ConsolidatedGrid.Columns.Add(IdColumn);
                            var participantColumn = new Telerik.Windows.Controls.GridViewDataColumn();
                            participantColumn.Header = participantName;
                            participantColumn.ColumnGroupName = stageGroup.Name;
                            participantColumn.DataMemberBinding = new Binding(participantName + stageIndex);
                            participantColumn.Width = 70;
                            ConsolidatedGrid.Columns.Add(participantColumn);
                        }

                        var Temperature = new Telerik.Windows.Controls.GridViewDataColumn();
                        Temperature.Header = TEMPERATURE;
                        Temperature.ColumnGroupName = stageGroup.Name;
                        Temperature.DataMemberBinding = new Binding(TEMPERATURE + stageIndex);
                        Temperature.Width = 70;
                        Temperature.IsReadOnly = true;
                        ConsolidatedGrid.Columns.Add(Temperature);

                        var Pressure = new Telerik.Windows.Controls.GridViewDataColumn();
                        Pressure.Header = PRESSURE;
                        Pressure.ColumnGroupName = stageGroup.Name;
                        Pressure.DataMemberBinding = new Binding(PRESSURE + stageIndex);
                        Pressure.Width = 70;
                        Pressure.IsReadOnly = true;
                        ConsolidatedGrid.Columns.Add(Pressure);

                        var Time = new Telerik.Windows.Controls.GridViewDataColumn();
                        Time.Header = TIME;
                        Time.ColumnGroupName = stageGroup.Name;
                        Time.DataMemberBinding = new Binding(TIME + stageIndex);
                        Time.Width = 70;
                        Time.IsReadOnly = true;
                        ConsolidatedGrid.Columns.Add(Time);

                        var pH = new Telerik.Windows.Controls.GridViewDataColumn();
                        pH.Header = PH;
                        pH.ColumnGroupName = stageGroup.Name;
                        pH.DataMemberBinding = new Binding(PH + stageIndex);
                        pH.Width = 70;
                        pH.IsReadOnly = true;
                        ConsolidatedGrid.Columns.Add(pH);

                        var StageCVT = new Telerik.Windows.Controls.GridViewDataColumn();
                        StageCVT.Header = STAGECVT + stageIndex;
                        StageCVT.ColumnGroupName = stageGroup.Name;
                        StageCVT.DataMemberBinding = new Binding(STAGECVT + stageIndex);
                        StageCVT.Width = 70;
                        StageCVT.IsReadOnly = true;
                        ConsolidatedGrid.Columns.Add(StageCVT);

                        var StageFreeText = new Telerik.Windows.Controls.GridViewDataColumn();
                        StageFreeText.Header = STAGEFREETEXT;
                        StageFreeText.ColumnGroupName = stageGroup.Name;
                        StageFreeText.DataMemberBinding = new Binding(STAGEFREETEXT + stageIndex);
                        StageFreeText.Width = 70;
                        StageFreeText.IsReadOnly = true;
                        ConsolidatedGrid.Columns.Add(StageFreeText);
                    }
                    ConsolidatedGrid.FrozenColumnCount = 9;
                    foreach (Telerik.Windows.Controls.GridViewDataColumn c in ConsolidatedGrid.Columns)
                        c.IsFilterable = false;
                    #endregion

                    foreach (var c in ConsolidatedGrid.Columns)
                    {
                        var dc = (GridViewDataColumn)c;
                    }

                    int rowCount = 1;
                    List<IDictionary<String, Object>> rows = new List<IDictionary<String, Object>>();

                    foreach (var reaction in anagousReactionVM.AnalogousReactions)
                    {
                        var row = (IDictionary<String, Object>)new ExpandoObject();
                        row[AG] = 1;
                        row[SNO] = rowCount++;
                        row[RXN] = reaction.Name;
                        row[RXNID] = reaction.Id;

                        if (NewAnalogous)
                        {
                            var reactionRSN = anagousReactionVM.Rsns.Where(p => p.Reaction.Id == reaction.Id && p.Stage == null).OrderBy(p => p.DisplayOrder);
                            row[PRODUCT] = string.Empty;
                            row[PRODUCT + "ID"] = string.Empty;
                            row[YIELD] = string.Empty;
                            row[REACTIONFREETEXT] = !string.IsNullOrEmpty(string.Join(",", reactionRSN.Select(rsn => rsn.FreeText).ToList())) ? string.Join(",", reactionRSN.Select(rsn => rsn.FreeText).ToList()) : string.Empty;
                            row[REACTIONCVT] = !string.IsNullOrEmpty(string.Join(",", reactionRSN.Select(rsn => rsn.CvtText).ToList())) ? string.Join(",", reactionRSN.Select(rsn => rsn.CvtText).ToList()) : string.Empty;
                        }
                        else
                        {
                            var products = mainViewModel.TanVM.ReactionParticipants.OfReactionOfType(reaction.Id, ParticipantType.Product).OrderBy(p => p.DisplayOrder);

                            var reactionRSN = mainViewModel.TanVM.Rsns.Where(p => p.Reaction.Id == reaction.Id && p.Stage == null).OrderBy(p => p.DisplayOrder);
                            row[PRODUCT] = string.Join(",", products.Select(n => n.Num).ToList());
                            row[PRODUCT + "ID"] = string.Join(",", products.Select(n => n.Id).ToList());
                            row[YIELD] = string.Join(",", products.Select(n => n.Yield).ToList());
                            row[REACTIONFREETEXT] = !string.IsNullOrEmpty(string.Join(",", reactionRSN.Select(rsn => rsn.FreeText).ToList())) ? string.Join(",", reactionRSN.Select(rsn => rsn.FreeText).ToList()) : string.Empty;
                            row[REACTIONCVT] = !string.IsNullOrEmpty(string.Join(",", reactionRSN.Select(rsn => rsn.CvtText).ToList())) ? string.Join(",", reactionRSN.Select(rsn => rsn.CvtText).ToList()) : string.Empty;
                        }

                        if (reaction.Stages != null && reaction.Stages.Any())
                        {
                            int stageCount = 1;
                            foreach (var stage in reaction.Stages)
                            {
                                row.Add(STAGEID + stageCount, stage.Id);
                                var stageRsn = NewAnalogous ? anagousReactionVM.Rsns.Where(p => p.Reaction.Id == reaction.Id && p.Stage != null && p.Stage.Id == stage.Id).OrderBy(p => p.DisplayOrder) :
                                    mainViewModel.TanVM.Rsns.Where(p => p.Reaction.Id == reaction.Id && p.Stage != null && p.Stage.Id == stage.Id).OrderBy(p => p.DisplayOrder);

                                foreach (var participantType in ParticipantHeaders)
                                {
                                    if (participantType.Equals(ParticipantType.Reactant.ToString()) && !NewAnalogous)
                                    {
                                        var reactants = mainViewModel.TanVM.ReactionParticipants.OfReactionAndStage(reaction.Id, stage.Id).Where(p => p.ParticipantType.ToString() == participantType).OrderBy(p => p.DisplayOrder);
                                        var csvNames = String.Join(",", reactants.Select(r => r.Num));
                                        row.Add(participantType.ToString() + stageCount, csvNames);
                                        row.Add(participantType.ToString() + stageCount + "ID", String.Join(",", reactants.Select(r => r.Id)));
                                    }
                                    else
                                    {
                                        var otherParticipants = anagousReactionVM.ReactionParticipants.OfReactionAndStage(reaction.Id, stage.Id).Where(p => p.ParticipantType.ToString() == participantType).OrderBy(p => p.DisplayOrder);
                                        if (otherParticipants.Count() > 0)
                                        {
                                            var csvNames = String.Join(",", otherParticipants.Select(r => r.Num));
                                            row.Add(participantType.ToString() + stageCount, csvNames);
                                            row.Add(participantType.ToString() + stageCount + "ID", String.Join(",", otherParticipants.Select(r => r.Id)));
                                        }
                                        else
                                        {
                                            row.Add(participantType.ToString() + stageCount, string.Empty);
                                            row.Add(participantType.ToString() + stageCount + "ID", string.Empty);
                                        }
                                    }
                                }
                                row.Add(TIME + stageCount, string.Join(",", stage.Conditions.Select(t => t.Time).ToList()));
                                row.Add(TEMPERATURE + stageCount, string.Join(",", stage.Conditions.Select(t => t.Temperature).ToList()));
                                row.Add(PRESSURE + stageCount, string.Join(",", stage.Conditions.Select(t => t.Pressure).ToList()));
                                row.Add(PH + stageCount, string.Join(",", stage.Conditions.Select(t => t.PH).ToList()));
                                row.Add(STAGECVT + stageCount, string.Join(",", stageRsn.Select(t => t.CvtText).ToList()));
                                row.Add(STAGEFREETEXT + stageCount, string.Join(",", stageRsn.Select(t => t.FreeText).ToList()));
                                stageCount++;
                            }
                        }
                        rows.Add(row);
                    }
                    ConsolidatedGrid.ItemsSource = rows;
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
                throw;
            }
        }
    }
}