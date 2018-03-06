using Client.Common;
using Client.Logging;
using Client.Models;
using Client.Util;
using Client.Validations;
using Client.ViewModels;
using Client.XML;
using DTO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Telerik.Windows.Controls;

namespace Client.Views.Analogous
{
    /// <summary>
    /// Interaction logic for EditAnalogous.xaml
    /// </summary>
    public partial class EditAnalogous : Window
    {

        private static EditAnalogous thisInstance;

        private static String RXNID = "RXNID";
        private static String STAGEID = "STAGEID";
        private static String PRODUCT = "PRODUCT";
        private static String YIELD = "YIELD";
        private static List<String> ParticipantHeaders = new List<String> {
            ParticipantType.Reactant.ToString(),
            ParticipantType.Solvent.ToString(),
            ParticipantType.Agent.ToString(),
            ParticipantType.Catalyst.ToString()
        };

        private GridViewColumnGroup productGroup = new Telerik.Windows.Controls.GridViewColumnGroup { Name = PRODUCT };
        private GridViewColumnGroup yieldGroup = new Telerik.Windows.Controls.GridViewColumnGroup { Name = YIELD };
        Dictionary<string, TanChemicalVM> ChemicalDict = S.ChemicalDict;
        List<TanChemicalVM> ChemicalsList = new List<TanChemicalVM>();
        ReactionVM reactionVM;
        ViewAnalogousVM analogousReactionVM;
        private int IndextoAdd;

        public EditAnalogous()
        {
            InitializeComponent();
        }

        public static void Show(ReactionVM reactionVM)
        {
            if (thisInstance == null)
            {
                thisInstance = new EditAnalogous();
            }
            thisInstance.Solvents.IsChecked = true;
            thisInstance.Agents.IsChecked = true;
            thisInstance.Catalyst.IsChecked = true;
            thisInstance.pH.IsChecked = true;
            thisInstance.Temp.IsChecked = true;
            thisInstance.Time.IsChecked = true;
            thisInstance.Pressure.IsChecked = true;
            thisInstance.RSNs.IsChecked = true;
            thisInstance.reactionVM = reactionVM;
            thisInstance.LoadReaction(reactionVM);
            (thisInstance.DataContext as EditAnalogousVM).AnalogousReactionPreview = new ReactionViewVM();
            thisInstance.IndextoAdd = 0;
            if (!thisInstance.IsVisible)
                thisInstance.Show();
            thisInstance.Activate();
        }

        private void LoadReaction(ReactionVM reactionVM)
        {
            ConsolidatedGrid.ItemsSource = null;
            ConsolidatedGrid.Columns.Clear();
            ConsolidatedGrid.ColumnGroups.Clear();
            ConsolidatedGrid.Items.Clear();
            MasterReaction.Content = reactionVM.Name;
        }

        private void ApplyReactions_Click(object sender, RoutedEventArgs e)
        {
            if ((thisInstance.DataContext as EditAnalogousVM) != null)
            {

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                });
                var mainVM = (App.Current.MainWindow as MainWindow).DataContext as MainVM;
                if (mainVM != null && mainVM.TanVM != null && mainVM.TanVM.Reactions != null)
                    IndextoAdd = mainVM.TanVM.Reactions.Count() >= 1 ? mainVM.TanVM.Reactions.ToList().FindIndex(x => x.Id == reactionVM.Id) : 0;
                if (Append.IsChecked.HasValue && Append.IsChecked.Value)
                    IndextoAdd = mainVM.TanVM.Reactions.Count;
                else if (After.IsChecked.HasValue && After.IsChecked.Value)
                    IndextoAdd = IndextoAdd + 1;
                bool IsSolventsChecked = Solvents.IsChecked.Value;
                bool IsCatalystChecked = Catalyst.IsChecked.Value;
                bool IsAgentChecked = Agents.IsChecked.Value;
                bool IspHChecked = pH.IsChecked.Value;
                bool IsTempChecked = Temp.IsChecked.Value;
                bool IsTimeChecked = Time.IsChecked.Value;
                bool IsPressureChecked = Pressure.IsChecked.Value;
                bool IsRsnsChecked = RSNs.IsChecked.Value;
                var start = DateTime.Now;
                analogousReactionVM = AnalogousBuilder.GenerateNewAnalogousReactions(reactionVM,
                                                                                    false,
                                                                                    IsSolventsChecked,
                                                                                    IsAgentChecked,
                                                                                    IsCatalystChecked,
                                                                                    IspHChecked,
                                                                                    IsTempChecked,
                                                                                    IsTimeChecked,
                                                                                    IsPressureChecked,
                                                                                    IsRsnsChecked,
                                                                                    (int)NewRecords.Value,
                                                                                    IndextoAdd
                                                                                    );
                AnalogousBuilder.BuildAnalogousGrid(analogousReactionVM, ConsolidatedGrid, true);
                Debug.WriteLine($"Generated Analogous Reactions in {(DateTime.Now - start).TotalSeconds} seconds.");
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Mouse.OverrideCursor = null;
                });
            }
        }

        private void SaveReactions_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                });
                var mainViewModel = ((MainWindow)(App.Current.MainWindow)).DataContext as MainVM;
                if (ConsolidatedGrid.ItemsSource != null)
                {
                    List<IDictionary<String, Object>> rows = (List<IDictionary<String, Object>>)ConsolidatedGrid.ItemsSource;
                    var chemicalNames = ChemicalDict.Select(item => item.Value).ToList();
                    var tanChemicals = mainViewModel.TanVM.TanChemicals;
                    if (analogousReactionVM != null)
                    {
                        int j = 0;
                        analogousReactionVM.ReactionParticipants.Clear();
                        var existingParticipants = mainViewModel.TanVM.ReactionParticipants.Where(rp => rp.ReactionVM.Id == reactionVM.Id);
                        foreach (var row in rows)
                        {
                            j++;
                            var reaction = analogousReactionVM.AnalogousReactions.Where(id => id.Id == new Guid(row[RXNID].ToString())).FirstOrDefault();

                            #region Products
                            if (row.ContainsKey(PRODUCT) && row[PRODUCT] != null && row[PRODUCT].ToString() != "")
                            {
                                var productNums = row[PRODUCT].ToString().Split(',').ToList();
                                int index = 0;
                                foreach (var item in productNums)
                                {
                                    int num;
                                    if (!int.TryParse(item, out num))
                                    {
                                        AppInfoBox.ShowInfoMessage($"Invalid Num in Row {j} for Product.");
                                        SetCursor(false); return;
                                    }
                                    var product = (from c in tanChemicals
                                                   where c.NUM == Convert.ToInt32(item)
                                                   select c).FirstOrDefault();
                                    if (product == null)
                                    {
                                        var chemical = (from c in chemicalNames where c.NUM == Convert.ToInt32(item) select c).FirstOrDefault();
                                        if (chemical != null)
                                        {
                                            var numchemical = (from c in tanChemicals where c.ChemicalType == ChemicalType.NUM && c.RegNumber == chemical.RegNumber select c).FirstOrDefault();
                                            if (numchemical != null)
                                                product = numchemical;
                                            else
                                            {
                                                product = new ProductTracking.Models.Core.TanChemical
                                                {
                                                    ChemicalType = chemical.ChemicalType,
                                                    CompoundNo = chemical.CompoundNo,
                                                    GenericName = chemical.GenericName,
                                                    ImagePath = chemical.ImagePath,
                                                    Name = chemical.Name,
                                                    NUM = chemical.NUM,
                                                    RegNumber = chemical.RegNumber,
                                                    Id = Guid.NewGuid()
                                                };
                                                tanChemicals.Add(product);
                                            }
                                        }
                                        else
                                        {
                                            AppInfoBox.ShowInfoMessage($"Num {item} not found in Chemicals. Please enter proper Num in Row {j} for Product.");
                                            SetCursor(false); return;
                                        }
                                    }
                                    if (product != null)
                                    {
                                        var existingParticipant = ParticipantValidations.AlreadyContains(product, analogousReactionVM, reaction, ParticipantType.Product);
                                        if (existingParticipant != null)
                                        {
                                            if (existingParticipant.ChemicalType == ChemicalType.S8000 && existingParticipant.Name != product.Name && existingParticipant.Num != product.NUM)
                                                existingParticipant = null;
                                        }
                                        if (existingParticipant == null)
                                        {
                                            string yield = string.Empty;
                                            if (row[YIELD].ToString().Split(',').Length > index && !string.IsNullOrEmpty(row[YIELD].ToString().Split(',')[index]))
                                            {
                                                if (!Regex.IsMatch(row[YIELD].ToString().Split(',')[index], S.YIELD_REG_EXP) || row[YIELD].ToString().Split(',')[index].StartsWith("0"))
                                                {
                                                    AppInfoBox.ShowInfoMessage($"Invalid Yield Value {row[YIELD].ToString().Split(',')[index]} in Row {j}");
                                                    SetCursor(false); return;
                                                }
                                            }
                                            var reactionParticipant = new ReactionParticipantVM
                                            {
                                                ChemicalType = product.ChemicalType,
                                                Id = Guid.NewGuid(),
                                                Name = !string.IsNullOrEmpty(existingParticipants.Where(rp => rp.Num == product.NUM && rp.Reg == product.RegNumber).Select(rp => rp.Name).FirstOrDefault()) ? existingParticipants.Where(rp => rp.Num == product.NUM && rp.Reg == product.RegNumber).Select(rp => rp.Name).FirstOrDefault() : product.Name,
                                                Num = product.NUM,
                                                ParticipantType = ParticipantType.Product,
                                                ReactionVM = reaction,
                                                StageVM = null,
                                                Reg = product.RegNumber,
                                                TanChemicalId = product.Id,
                                                DisplayOrder = index,
                                                KeyProduct = index == 0 ? true : false,
                                                Yield = (row.ContainsKey(YIELD) && row[YIELD] != null && !string.IsNullOrEmpty(row[YIELD].ToString())) ? row[YIELD].ToString().Split(',').Length > index ? row[YIELD].ToString().Split(',')[index] : string.Empty : string.Empty,
                                                IsIgnorable = true,
                                                Formula = product.Formula
                                            };
                                            analogousReactionVM.ReactionParticipants.Add(reactionParticipant);
                                            var rsnVM = analogousReactionVM.Rsns.Where(rsn => rsn.Reaction.Id == reaction.Id && rsn.Stage != null && rsn.IsIgnorableInDelivery && rsn.SelectedChemical != null && rsn.SelectedChemical.Id == product.Id).FirstOrDefault();
                                            if (rsnVM != null)
                                            {
                                                rsnVM.SelectedChemical = null;
                                                if (rsnVM.ReactionParticipantId == null)
                                                    rsnVM.ReactionParticipantId = new List<Guid>();
                                                rsnVM.ReactionParticipantId.Add(reactionParticipant.Id);
                                            }
                                        }
                                        else
                                        {
                                            existingParticipant.DisplayDuplicateString(product.RegNumber);
                                            var ParticipantsToRemove = analogousReactionVM.ReactionParticipants.Where(rp => rp.IsIgnorable).ToList();
                                            foreach (var participant in ParticipantsToRemove)
                                                analogousReactionVM.ReactionParticipants.Remove(participant);
                                            //SaveReactions.IsEnabled = true;
                                            SetCursor(false); return;
                                        }
                                    }
                                    index++;
                                }
                            }

                            else
                            {
                                System.Windows.Forms.MessageBox.Show($"There is no Product in the Row {j}. Atleast one Product is required to Save Reaction.", "Analogous Reactions", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                //SaveReactions.IsEnabled = true;
                                SetCursor(false); return;
                            }
                            #endregion

                            for (int i = 1; i <= analogousReactionVM.SelectedMasterReaction.Stages.Count; i++)
                            {
                                List<string> reactantNums = new List<string>();
                                #region Reactants
                                if (row.ContainsKey(ParticipantType.Reactant.ToString() + i) && row[ParticipantType.Reactant.ToString() + i] != null && !string.IsNullOrEmpty(row[ParticipantType.Reactant.ToString() + i].ToString()))
                                {
                                    reactantNums = row[ParticipantType.Reactant.ToString() + i].ToString().Split(',').ToList();
                                    foreach (var item in reactantNums)
                                    {
                                        int num;
                                        if (!int.TryParse(item, out num))
                                        {
                                            AppInfoBox.ShowInfoMessage($"Invalid Num in Row {j} for Reactant.");
                                            SetCursor(false); return;
                                        }
                                        var reactant = (from c in tanChemicals
                                                        where c.NUM == Convert.ToInt32(item)
                                                        select c).FirstOrDefault();

                                        if (reactant == null)
                                        {
                                            var chemical = (from c in chemicalNames where c.NUM == Convert.ToInt32(item) select c).FirstOrDefault();
                                            if (chemical != null)
                                            {
                                                var numchemical = (from c in tanChemicals where c.ChemicalType == ChemicalType.NUM && c.RegNumber == chemical.RegNumber select c).FirstOrDefault();
                                                if (numchemical != null)
                                                    reactant = numchemical;
                                                else
                                                {
                                                    reactant = new ProductTracking.Models.Core.TanChemical
                                                    {
                                                        ChemicalType = chemical.ChemicalType,
                                                        CompoundNo = chemical.CompoundNo,
                                                        GenericName = chemical.GenericName,
                                                        ImagePath = chemical.ImagePath,
                                                        Name = chemical.Name,
                                                        NUM = chemical.NUM,
                                                        RegNumber = chemical.RegNumber,
                                                        Id = Guid.NewGuid()
                                                    };
                                                    tanChemicals.Add(reactant);
                                                }
                                            }
                                            else
                                            {
                                                AppInfoBox.ShowInfoMessage($"Num {item} not found in Chemicals. Please enter proper Num in Row {j} for Product.");
                                                SetCursor(false); return;
                                            }
                                        }

                                        var stages = (from r in analogousReactionVM.AnalogousReactions where r.Id == reaction.Id select r).FirstOrDefault();

                                        if (reactant != null)
                                        {
                                            reaction.SelectedStage = reaction.Stages.Where(s => s.Id == new Guid(row[STAGEID + i].ToString())).FirstOrDefault();
                                            var existingParticipant = ParticipantValidations.AlreadyContains(reactant, analogousReactionVM, reaction, ParticipantType.Reactant);
                                            if (existingParticipant != null)
                                            {
                                                if (existingParticipant.ChemicalType == ChemicalType.S8000 && existingParticipant.Name != reactant.Name && existingParticipant.Num != reactant.NUM)
                                                    existingParticipant = null;
                                            }
                                            if (existingParticipant == null)
                                            {
                                                var reactionParticipant = new ReactionParticipantVM
                                                {

                                                    ChemicalType = reactant.ChemicalType,
                                                    Id = Guid.NewGuid(),
                                                    Name = !string.IsNullOrEmpty(existingParticipants.Where(rp => rp.Num == reactant.NUM && rp.Reg == reactant.RegNumber).Select(rp => rp.Name).FirstOrDefault()) ? existingParticipants.Where(rp => rp.Num == reactant.NUM && rp.Reg == reactant.RegNumber).Select(rp => rp.Name).FirstOrDefault() : reactant.Name,
                                                    Num = reactant.NUM,
                                                    ParticipantType = ParticipantType.Reactant,
                                                    ReactionVM = analogousReactionVM.AnalogousReactions.Where(id => id.Id == new Guid(row[RXNID].ToString())).FirstOrDefault(),
                                                    StageVM = reaction.Stages.Where(s => s.Id == new Guid(row[STAGEID + i].ToString())).FirstOrDefault(),
                                                    Reg = reactant.RegNumber,
                                                    TanChemicalId = reactant.Id,
                                                    DisplayOrder = i,
                                                    Yield = string.Empty,
                                                    IsIgnorable = true,
                                                    Formula = reactant.Formula
                                                };
                                                analogousReactionVM.ReactionParticipants.Add(reactionParticipant);
                                                var rsnVM = analogousReactionVM.Rsns.Where(rsn => rsn.Reaction.Id == reaction.Id && rsn.Stage != null && rsn.IsIgnorableInDelivery && rsn.SelectedChemical != null && rsn.SelectedChemical.Id == reactant.Id).FirstOrDefault();
                                                if (rsnVM != null)
                                                {
                                                    rsnVM.SelectedChemical = null;
                                                    if (rsnVM.ReactionParticipantId == null)
                                                        rsnVM.ReactionParticipantId = new List<Guid>();
                                                    rsnVM.ReactionParticipantId.Add(reactionParticipant.Id);
                                                }
                                            }

                                            else
                                            {
                                                existingParticipant.DisplayDuplicateString(reactant.RegNumber);
                                                var ParticipantsToRemove = analogousReactionVM.ReactionParticipants.Where(rp => rp.IsIgnorable).ToList();
                                                foreach (var participant in ParticipantsToRemove)
                                                    analogousReactionVM.ReactionParticipants.Remove(participant);
                                                //SaveReactions.IsEnabled = true;
                                                SetCursor(false); return;
                                            }
                                        }
                                    }
                                }
                                #endregion
                                #region Solvents
                                if (row.ContainsKey(ParticipantType.Solvent.ToString() + i) && row[ParticipantType.Solvent.ToString() + i] != null && !string.IsNullOrEmpty(row[ParticipantType.Solvent.ToString() + i].ToString()))
                                {
                                    reactantNums = row[ParticipantType.Solvent.ToString() + i].ToString().Split(',').ToList();
                                    foreach (var item in reactantNums)
                                    {
                                        int num;
                                        if (!int.TryParse(item, out num))
                                        {
                                            AppInfoBox.ShowInfoMessage($"Invalid Num in Row {j} for Solvent.");
                                            SetCursor(false); return;
                                        }
                                        var solvent = (from c in tanChemicals
                                                       where c.NUM == Convert.ToInt32(item)
                                                       select c).FirstOrDefault();

                                        if (solvent == null)
                                        {
                                            var chemical = (from c in chemicalNames where c.NUM == Convert.ToInt32(item) select c).FirstOrDefault();
                                            if (chemical != null)
                                            {
                                                var numchemical = (from c in tanChemicals where c.ChemicalType == ChemicalType.NUM && c.RegNumber == chemical.RegNumber select c).FirstOrDefault();
                                                if (numchemical != null)
                                                    solvent = numchemical;
                                                else
                                                {
                                                    solvent = new ProductTracking.Models.Core.TanChemical
                                                    {
                                                        ChemicalType = chemical.ChemicalType,
                                                        CompoundNo = chemical.CompoundNo,
                                                        GenericName = chemical.GenericName,
                                                        ImagePath = chemical.ImagePath,
                                                        Name = chemical.Name,
                                                        NUM = chemical.NUM,
                                                        RegNumber = chemical.RegNumber,
                                                        Id = Guid.NewGuid()
                                                    };
                                                    tanChemicals.Add(solvent);
                                                }
                                            }
                                            else
                                            {
                                                AppInfoBox.ShowInfoMessage($"Num {item} not found in Chemicals. Please enter proper Num in Row {j} for Solvent.");
                                                SetCursor(false); return;
                                            }
                                        }

                                        var stages = (from r in analogousReactionVM.AnalogousReactions where r.Id == reaction.Id select r).FirstOrDefault();

                                        if (solvent != null)
                                        {
                                            reaction.SelectedStage = reaction.Stages.Where(s => s.Id == new Guid(row[STAGEID + i].ToString())).FirstOrDefault();
                                            var existingParticipant = ParticipantValidations.AlreadyContains(solvent, analogousReactionVM, reaction, ParticipantType.Solvent);
                                            if (existingParticipant != null)
                                            {
                                                if (existingParticipant.ChemicalType == ChemicalType.S8000 && existingParticipant.Name != solvent.Name && existingParticipant.Num != solvent.NUM)
                                                    existingParticipant = null;
                                            }
                                            if (existingParticipant == null)
                                            {
                                                var reactionParticipant = new ReactionParticipantVM
                                                {

                                                    ChemicalType = solvent.ChemicalType,
                                                    Id = Guid.NewGuid(),
                                                    Name = !string.IsNullOrEmpty(existingParticipants.Where(rp => rp.Num == solvent.NUM && rp.Reg == solvent.RegNumber).Select(rp => rp.Name).FirstOrDefault()) ? existingParticipants.Where(rp => rp.Num == solvent.NUM && rp.Reg == solvent.RegNumber).Select(rp => rp.Name).FirstOrDefault() : solvent.Name,
                                                    Num = solvent.NUM,
                                                    ParticipantType = ParticipantType.Solvent,
                                                    ReactionVM = analogousReactionVM.AnalogousReactions.Where(id => id.Id == new Guid(row[RXNID].ToString())).FirstOrDefault(),
                                                    StageVM = reaction.Stages.Where(s => s.Id == new Guid(row[STAGEID + i].ToString())).FirstOrDefault(),
                                                    Reg = solvent.RegNumber,
                                                    TanChemicalId = solvent.Id,
                                                    DisplayOrder = i,
                                                    Yield = string.Empty,
                                                    IsIgnorable = true
                                                };
                                                analogousReactionVM.ReactionParticipants.Add(reactionParticipant);
                                                var rsnVM = analogousReactionVM.Rsns.Where(rsn => rsn.Reaction.Id == reaction.Id && rsn.Stage != null && rsn.IsIgnorableInDelivery && rsn.SelectedChemical != null && rsn.SelectedChemical.Id == solvent.Id).FirstOrDefault();
                                                if (rsnVM != null)
                                                {
                                                    rsnVM.SelectedChemical = null;
                                                    if (rsnVM.ReactionParticipantId == null)
                                                        rsnVM.ReactionParticipantId = new List<Guid>();
                                                    rsnVM.ReactionParticipantId.Add(reactionParticipant.Id);
                                                }
                                            }
                                            else
                                            {
                                                existingParticipant.DisplayDuplicateString(solvent.RegNumber);
                                                var ParticipantsToRemove = analogousReactionVM.ReactionParticipants.Where(rp => rp.IsIgnorable).ToList();
                                                foreach (var participant in ParticipantsToRemove)
                                                    analogousReactionVM.ReactionParticipants.Remove(participant);
                                                //SaveReactions.IsEnabled = true;
                                                SetCursor(false); return;
                                            }
                                        }
                                    }
                                }
                                #endregion
                                #region Agents
                                if (row.ContainsKey(ParticipantType.Agent.ToString() + i) && row[ParticipantType.Agent.ToString() + i] != null && !string.IsNullOrEmpty(row[ParticipantType.Agent.ToString() + i].ToString()))
                                {
                                    reactantNums = row[ParticipantType.Agent.ToString() + i].ToString().Split(',').ToList();
                                    foreach (var item in reactantNums)
                                    {
                                        int num;
                                        if (!int.TryParse(item, out num))
                                        {
                                            AppInfoBox.ShowInfoMessage($"Invalid Num in Row {j} for Agent.");
                                            SetCursor(false); return;
                                        }
                                        var agent = (from c in tanChemicals
                                                     where c.NUM == Convert.ToInt32(item)
                                                     select c).FirstOrDefault();

                                        if (agent == null)
                                        {
                                            var chemical = (from c in chemicalNames where c.NUM == Convert.ToInt32(item) select c).FirstOrDefault();
                                            if (chemical != null)
                                            {
                                                var numchemical = (from c in tanChemicals where c.ChemicalType == ChemicalType.NUM && c.RegNumber == chemical.RegNumber select c).FirstOrDefault();
                                                if (numchemical != null)
                                                    agent = numchemical;
                                                else
                                                {
                                                    agent = new ProductTracking.Models.Core.TanChemical
                                                    {
                                                        ChemicalType = chemical.ChemicalType,
                                                        CompoundNo = chemical.CompoundNo,
                                                        GenericName = chemical.GenericName,
                                                        ImagePath = chemical.ImagePath,
                                                        Name = chemical.Name,
                                                        NUM = chemical.NUM,
                                                        RegNumber = chemical.RegNumber,
                                                        Id = Guid.NewGuid()
                                                    };
                                                    tanChemicals.Add(agent);
                                                }
                                            }
                                            else
                                            {
                                                AppInfoBox.ShowInfoMessage($"Num {item} not found in Chemicals. Please enter proper Num in Row {j} for Agent.");
                                                SetCursor(false); return;
                                            }
                                        }

                                        var stages = (from r in analogousReactionVM.AnalogousReactions where r.Id == reaction.Id select r).FirstOrDefault();

                                        if (agent != null)
                                        {
                                            reaction.SelectedStage = reaction.Stages.Where(s => s.Id == new Guid(row[STAGEID + i].ToString())).FirstOrDefault();
                                            var existingParticipant = ParticipantValidations.AlreadyContains(agent, analogousReactionVM, reaction, ParticipantType.Agent);
                                            if (existingParticipant != null)
                                            {
                                                if (existingParticipant.ChemicalType == ChemicalType.S8000 && existingParticipant.Name != agent.Name && existingParticipant.Num != agent.NUM)
                                                    existingParticipant = null;
                                            }
                                            if (existingParticipant == null)
                                            {
                                                var reactionParticipant = new ReactionParticipantVM
                                                {

                                                    ChemicalType = agent.ChemicalType,
                                                    Id = Guid.NewGuid(),
                                                    Name = !string.IsNullOrEmpty(existingParticipants.Where(rp => rp.Num == agent.NUM && rp.Reg == agent.RegNumber).Select(rp => rp.Name).FirstOrDefault()) ? existingParticipants.Where(rp => rp.Num == agent.NUM && rp.Reg == agent.RegNumber).Select(rp => rp.Name).FirstOrDefault() : agent.Name,
                                                    Num = agent.NUM,
                                                    ParticipantType = ParticipantType.Agent,
                                                    ReactionVM = analogousReactionVM.AnalogousReactions.Where(id => id.Id == new Guid(row[RXNID].ToString())).FirstOrDefault(),
                                                    StageVM = reaction.Stages.Where(s => s.Id == new Guid(row[STAGEID + i].ToString())).FirstOrDefault(),
                                                    Reg = agent.RegNumber,
                                                    TanChemicalId = agent.Id,
                                                    DisplayOrder = i,
                                                    Yield = string.Empty,
                                                    IsIgnorable = true
                                                };
                                                analogousReactionVM.ReactionParticipants.Add(reactionParticipant);
                                                var rsnVM = analogousReactionVM.Rsns.Where(rsn => rsn.Reaction.Id == reaction.Id && rsn.Stage != null && rsn.IsIgnorableInDelivery && rsn.SelectedChemical != null && rsn.SelectedChemical.Id == agent.Id).FirstOrDefault();
                                                if (rsnVM != null)
                                                {
                                                    rsnVM.SelectedChemical = null;
                                                    if (rsnVM.ReactionParticipantId == null)
                                                        rsnVM.ReactionParticipantId = new List<Guid>();
                                                    rsnVM.ReactionParticipantId.Add(reactionParticipant.Id);
                                                }
                                            }
                                            else
                                            {
                                                existingParticipant.DisplayDuplicateString(agent.RegNumber);
                                                var ParticipantsToRemove = analogousReactionVM.ReactionParticipants.Where(rp => rp.IsIgnorable).ToList();
                                                foreach (var participant in ParticipantsToRemove)
                                                    analogousReactionVM.ReactionParticipants.Remove(participant);
                                                SetCursor(false); return;
                                            }
                                        }
                                    }
                                }
                                #endregion
                                #region Catalyst
                                if (row.ContainsKey(ParticipantType.Catalyst.ToString() + i) && row[ParticipantType.Catalyst.ToString() + i] != null && !string.IsNullOrEmpty(row[ParticipantType.Catalyst.ToString() + i].ToString()))
                                {
                                    reactantNums = row[ParticipantType.Catalyst.ToString() + i].ToString().Split(',').ToList();
                                    foreach (var item in reactantNums)
                                    {
                                        int num;
                                        if (!int.TryParse(item, out num))
                                        {
                                            AppInfoBox.ShowInfoMessage($"Invalid Num in Row {j} for Catalyst.");
                                            SetCursor(false); return;
                                        }
                                        var catalyst = (from c in tanChemicals
                                                        where c.NUM == Convert.ToInt32(item)
                                                        select c).FirstOrDefault();

                                        if (catalyst == null)
                                        {
                                            var chemical = (from c in chemicalNames where c.NUM == Convert.ToInt32(item) select c).FirstOrDefault();
                                            if (chemical != null)
                                            {
                                                var numchemical = (from c in tanChemicals where c.ChemicalType == ChemicalType.NUM && c.RegNumber == chemical.RegNumber select c).FirstOrDefault();
                                                if (numchemical != null)
                                                    catalyst = numchemical;
                                                else
                                                {
                                                    catalyst = new ProductTracking.Models.Core.TanChemical
                                                    {
                                                        ChemicalType = chemical.ChemicalType,
                                                        CompoundNo = chemical.CompoundNo,
                                                        GenericName = chemical.GenericName,
                                                        ImagePath = chemical.ImagePath,
                                                        Name = chemical.Name,
                                                        NUM = chemical.NUM,
                                                        RegNumber = chemical.RegNumber,
                                                        Id = Guid.NewGuid()
                                                    };
                                                    tanChemicals.Add(catalyst);
                                                }
                                            }
                                            else
                                            {
                                                AppInfoBox.ShowInfoMessage($"Num {item} not found in Chemicals. Please enter proper Num in Row {j} for Catalyst.");
                                                SetCursor(false); return;
                                            }
                                        }

                                        var stages = (from r in analogousReactionVM.AnalogousReactions where r.Id == reaction.Id select r).FirstOrDefault();

                                        if (catalyst != null)
                                        {
                                            reaction.SelectedStage = reaction.Stages.Where(s => s.Id == new Guid(row[STAGEID + i].ToString())).FirstOrDefault();
                                            var existingParticipant = ParticipantValidations.AlreadyContains(catalyst, analogousReactionVM, reaction, ParticipantType.Catalyst);
                                            if (existingParticipant != null)
                                            {
                                                if (existingParticipant.ChemicalType == ChemicalType.S8000 && existingParticipant.Name != catalyst.Name && existingParticipant.Num != catalyst.NUM)
                                                    existingParticipant = null;
                                            }
                                            if (existingParticipant == null)
                                            {
                                                var reactionParticipant = new ReactionParticipantVM
                                                {

                                                    ChemicalType = catalyst.ChemicalType,
                                                    Id = Guid.NewGuid(),
                                                    Name = !string.IsNullOrEmpty(existingParticipants.Where(rp => rp.Num == catalyst.NUM && rp.Reg == catalyst.RegNumber).Select(rp => rp.Name).FirstOrDefault()) ? existingParticipants.Where(rp => rp.Num == catalyst.NUM && rp.Reg == catalyst.RegNumber).Select(rp => rp.Name).FirstOrDefault() : catalyst.Name,
                                                    Num = catalyst.NUM,
                                                    ParticipantType = ParticipantType.Catalyst,
                                                    ReactionVM = analogousReactionVM.AnalogousReactions.Where(ar => ar.Id == new Guid(row[RXNID].ToString())).FirstOrDefault(),
                                                    StageVM = reaction.Stages.Where(s => s.Id == new Guid(row[STAGEID + i].ToString())).FirstOrDefault(),
                                                    Reg = catalyst.RegNumber,
                                                    TanChemicalId = catalyst.Id,
                                                    DisplayOrder = i,
                                                    Yield = string.Empty,
                                                    IsIgnorable = true
                                                };
                                                analogousReactionVM.ReactionParticipants.Add(reactionParticipant);
                                                var rsnVM = analogousReactionVM.Rsns.Where(rsn => rsn.Reaction.Id == reaction.Id && rsn.Stage != null && rsn.IsIgnorableInDelivery && rsn.SelectedChemical != null && rsn.SelectedChemical.Id == catalyst.Id).FirstOrDefault();
                                                if (rsnVM != null)
                                                {
                                                    rsnVM.SelectedChemical = null;
                                                    if (rsnVM.ReactionParticipantId == null)
                                                        rsnVM.ReactionParticipantId = new List<Guid>();
                                                    rsnVM.ReactionParticipantId.Add(reactionParticipant.Id);
                                                }
                                            }
                                            else
                                            {
                                                existingParticipant.DisplayDuplicateString(catalyst.RegNumber);
                                                var ParticipantsToRemove = analogousReactionVM.ReactionParticipants.Where(rp => rp.IsIgnorable).ToList();
                                                foreach (var participant in ParticipantsToRemove)
                                                    analogousReactionVM.ReactionParticipants.Remove(participant);
                                                SetCursor(false); return;
                                            }
                                        }
                                    }
                                }
                                #endregion
                            }
                        }
                    }
                    StringBuilder ReactantMised = new StringBuilder();
                    foreach (var reaction in analogousReactionVM.AnalogousReactions)
                    {
                        var reactionParticipants = analogousReactionVM.ReactionParticipants.Where(rp => rp.ReactionVM.Id == reaction.Id);
                        foreach (var stage in reaction.Stages)
                        {
                            var stageParticipants = reactionParticipants.OfStage(stage.Id);
                            if (!stageParticipants.Any())
                                ReactantMised.AppendLine($"Atleast one participant is required to save stage in Reaction {reaction.Name} {stage.Name}.");
                        }
                        var reactants = analogousReactionVM.ReactionParticipants.Where(rp => rp.ReactionVM.Id == reaction.Id && rp.ParticipantType == ParticipantType.Reactant);
                        if (!reactants.Any())
                            ReactantMised.Append($"Reactant Missed in Reaction {reaction.Name}");
                    }
                    if (!string.IsNullOrEmpty(ReactantMised.ToString()))
                    {
                        AppInfoBox.ShowInfoMessage(ReactantMised.ToString());
                        SetCursor(false);
                        return;
                    }

                    //mainViewModel.TanVM.EnablePreview = true;

                    mainViewModel.TanVM.SelectedReaction = null;
                    mainViewModel.TanVM.EnablePreview = false;
                    mainViewModel.TanVM.ReactionParticipants.AddRange(analogousReactionVM.ReactionParticipants);
                    mainViewModel.TanVM.Rsns.AddRange(analogousReactionVM.Rsns);
                    mainViewModel.TanVM.SetReactions(analogousReactionVM.AnalogousReactions.ToList(), IndextoAdd);
                    //mainViewModel.TanVM.Reactions.AddRange(analogousReactionVM.AnalogousReactions);
                    mainViewModel.TanVM.EnablePreview = true;
                    mainViewModel.AnalogourVisibility = Visibility.Hidden;
                    mainViewModel.PreviewTabIndex = 0;
                    ConsolidatedGrid.ItemsSource = null;
                    mainViewModel.TanVM.PerformAutoSave("Created Analogous Reactions");
                    Hide();
                }
                else
                    AppInfoBox.ShowInfoMessage("Please Create Analogous Reactions First");
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
            SetCursor(false);
        }
        public void SetCursor(bool waitCursor)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Mouse.OverrideCursor = waitCursor ? Cursors.Wait : null;
            });
        }

        private void ConsolidatedGrid_CellEditEnded(object sender, Telerik.Windows.Controls.GridViewCellEditEndedEventArgs e)
        {
            try
            {
                var mainViewModel = ((MainWindow)(App.Current.MainWindow)).DataContext as MainVM;
                IDictionary<String, Object> row = ConsolidatedGrid.SelectedItem as IDictionary<String, Object>;
                char columnNum = e.Cell.Column.ColumnGroupName.Last();
                string ColumnType = e.Cell.Column.Header.ToString();
                string IdsToRemove = string.Empty;
                if (e.OldData != null && e.OldData.ToString() != string.Empty)
                    IdsToRemove = ColumnType.Contains(PRODUCT) ? row[ColumnType + "ID"].ToString() : row[ColumnType + columnNum + "ID"].ToString();

                if (!string.IsNullOrEmpty(IdsToRemove))
                {
                    foreach (var idToRemove in IdsToRemove.Split(','))
                    {
                        var participantToRemove = analogousReactionVM.AllParticipants.Where(rp => rp.Id == new Guid(idToRemove)).FirstOrDefault();
                        if (participantToRemove != null)
                            analogousReactionVM.AllParticipants.Remove(participantToRemove);
                        analogousReactionVM.ReactionParticipants.Remove(analogousReactionVM.ReactionParticipants.Where(rp => rp.Id == new Guid(idToRemove)).FirstOrDefault());
                    }
                }
                var editingReaction = (from r in analogousReactionVM.AnalogousReactions where r.Id == new Guid(row[RXNID].ToString()) select r).FirstOrDefault();
                if (e.NewData != null && e.NewData.ToString() != "")
                {
                    var list = new List<TanChemicalVM>();
                    list = ChemicalDict.Select(item => item.Value).ToList();
                    var existingNums = analogousReactionVM.AllParticipants.Where(p => p.ReactionVM.Id == editingReaction.Id).Select(p => p.Num);
                    if (ColumnType.Contains(PRODUCT) || ColumnType.Contains(ParticipantType.Reactant.ToString()) || ColumnType.Contains(ParticipantType.Agent.ToString()) || ColumnType.Contains(ParticipantType.Solvent.ToString()) || ColumnType.Contains(ParticipantType.Catalyst.ToString()))
                    {
                        var participantIdStrings = e.NewData.ToString().Split(',');
                        row[ColumnType + "ID"] = string.Empty;
                        string guid = string.Empty;
                        foreach (var participantIdString in participantIdStrings)
                        {
                            int num;
                            if (int.TryParse(participantIdString, out num))

                            {
                                var tanChemicalProduct = (from c in mainViewModel.TanVM.TanChemicals
                                                          where c.NUM == num
                                                          select c).FirstOrDefault();
                                if (tanChemicalProduct == null)
                                {
                                    var chemical = (from c in list where c.NUM == Convert.ToInt32(participantIdString) select c).FirstOrDefault();
                                    if (chemical != null)
                                    {
                                        tanChemicalProduct = new ProductTracking.Models.Core.TanChemical
                                        {
                                            ChemicalType = chemical.ChemicalType,
                                            CompoundNo = chemical.CompoundNo,
                                            GenericName = chemical.GenericName,
                                            ImagePath = chemical.ImagePath,
                                            Name = chemical.Name,
                                            NUM = chemical.NUM,
                                            RegNumber = chemical.RegNumber,
                                            Id = chemical.Id
                                        };
                                    }
                                }
                                if (tanChemicalProduct != null)
                                {
                                    Guid newParticipantId = Guid.NewGuid();
                                    guid = !string.IsNullOrEmpty(guid) ? guid + "," + newParticipantId.ToString() : newParticipantId.ToString();
                                    analogousReactionVM.AllParticipants.Add(new ReactionParticipantVM
                                    {
                                        ChemicalType = tanChemicalProduct.ChemicalType,
                                        Id = newParticipantId,
                                        Name = tanChemicalProduct.Name,
                                        Num = tanChemicalProduct.NUM,
                                        ParticipantType = ColumnType.Contains(PRODUCT) ? ParticipantType.Product : ColumnType.Contains(ParticipantType.Reactant.ToString()) ? ParticipantType.Reactant : ColumnType.Contains(ParticipantType.Agent.ToString()) ? ParticipantType.Agent : ColumnType.Contains(ParticipantType.Solvent.ToString()) ? ParticipantType.Solvent : ParticipantType.Catalyst,
                                        ReactionVM = editingReaction,
                                        StageVM = ColumnType.Contains(PRODUCT) ? null : editingReaction.Stages.Where(s => s.Id == new Guid(row[STAGEID + columnNum].ToString())).FirstOrDefault(),
                                        Reg = tanChemicalProduct.RegNumber,
                                        TanChemicalId = tanChemicalProduct.Id
                                    });
                                }
                                else
                                {
                                    e.Handled = false;
                                    AppInfoBox.ShowInfoMessage("Num " + participantIdString + " Not Found. Please enter proper num");
                                }
                            }
                            else
                            {
                                e.Handled = false;
                                AppInfoBox.ShowInfoMessage("Num " + participantIdString + " Not Valid. Please enter proper num");
                            }
                        }
                        if (ColumnType.Contains(PRODUCT))
                        {
                            row[ColumnType + "ID"] = guid;
                        }
                        else
                            row[ColumnType + columnNum + "ID"] = guid;
                        guid = string.Empty;
                    }
                }
                UpdateAnalogousReactionPreview(editingReaction);
            }
            catch (Exception ex)
            {
                Log.This(ex);
                AppInfoBox.ShowInfoMessage("Error in Adding Data. Please inform it Team to Fix.");
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void ConsolidatedGrid_Deleted(object sender, GridViewDeletedEventArgs e)
        {
            try
            {
                List<IDictionary<String, Object>> rows = (List<IDictionary<String, Object>>)ConsolidatedGrid.ItemsSource;
                var reactions = analogousReactionVM.AnalogousReactions;
                List<ReactionVM> reactionsToMaintain = new List<ReactionVM>();
                if (rows != null && rows.Any() && reactions != null && reactions.Any())
                {
                    foreach (var row in rows)
                    {
                        if (!string.IsNullOrEmpty(row[RXNID].ToString()))
                        {
                            reactionsToMaintain.Add(analogousReactionVM.AnalogousReactions.Where(rxn => rxn.Id == new Guid(row[RXNID].ToString())).FirstOrDefault());
                        }
                    }
                    var listToRemove = analogousReactionVM.AnalogousReactions.Except(reactionsToMaintain);
                    analogousReactionVM.AnalogousReactions.Remove(listToRemove.FirstOrDefault());
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private void ConsolidatedGrid_SelectionChanged(object sender, SelectionChangeEventArgs e)
        {
            try
            {
                if (ConsolidatedGrid.SelectedItem != null)
                {
                    IDictionary<String, Object> row = ConsolidatedGrid.SelectedItem as IDictionary<String, Object>;
                    if (!string.IsNullOrEmpty(row[RXNID].ToString()))
                    {
                        var selectedAnalagousReaction = (from r in analogousReactionVM.AnalogousReactions where r.Id == new Guid(row[RXNID].ToString()) select r).FirstOrDefault();
                        if (selectedAnalagousReaction != null)
                            UpdateAnalogousReactionPreview(selectedAnalagousReaction);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        public void UpdateAnalogousReactionPreview(ReactionVM SelectedReaction)
        {
            if ((thisInstance.DataContext as EditAnalogousVM).AnalogousReactionPreview != null)
            {
                if (SelectedReaction != null)
                    (thisInstance.DataContext as EditAnalogousVM).AnalogousReactionPreview = ReactionViewBuilder.GetReactionView(SelectedReaction, analogousReactionVM.AllParticipants.ToList());
                else
                    (thisInstance.DataContext as EditAnalogousVM).AnalogousReactionPreview.Name = "Select Reaction To Preview . .";
            }
        }
    }
}
