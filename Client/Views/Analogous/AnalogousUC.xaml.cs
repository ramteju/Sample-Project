using Client.Common;
using Client.Logging;
using Client.Models;
using Client.ViewModels;
using Client.XML;
using DTO;
using Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace Client.Views
{
    /// <summary>
    /// Interaction logic for AnalogousUC.xaml
    /// </summary>
    public partial class AnalogousUC : UserControl
    {
        public AnalogousUC()
        {
            InitializeComponent();
            (App.Current.MainWindow as MainWindow).MasterReactionChangedInAnalogousTab += AnalogousUC_MasterReactionChangedInAnalogousTab;
        }

        private static String PRODUCT = "PRODUCT";
        private static String YIELD = "YIELD";
        private static List<String> ParticipantHeaders = new List<String> {
            ParticipantType.Reactant.ToString(),
            ParticipantType.Solvent.ToString(),
            ParticipantType.Agent.ToString(),
            ParticipantType.Catalyst.ToString()
        };
        public MainVM mainViewModel = ((MainWindow)(App.Current.MainWindow)).DataContext as MainVM;
        public ViewAnalogousVM reactionVM;

        private GridViewColumnGroup productGroup = new Telerik.Windows.Controls.GridViewColumnGroup { Name = PRODUCT };
        private GridViewColumnGroup yieldGroup = new Telerik.Windows.Controls.GridViewColumnGroup { Name = YIELD };
        Dictionary<string, TanChemicalVM> ChemicalDict = S.ChemicalDict;
        List<TanChemicalVM> ChemicalsList = new List<TanChemicalVM>();
        void ClearAll()
        {
            
        }
        private void AnalogousUC_MasterReactionChangedInAnalogousTab(object sender, ReactionVM e)
        {
            try
            {
                ClearAll();
                ShowExistingAnalogousReactions(e);
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }
        void ShowExistingAnalogousReactions(ReactionVM masterReaction)
        {
            try
            {
                var viewModel = this.DataContext as ViewAnalogousVM;
                if (viewModel != null)
                {
                    ViewAnalogousVM analogousReactionVM = viewModel.CollectExistingAnalogousReactions();
                    AnalogousBuilder.BuildAnalogousGrid(analogousReactionVM, ConsolidatedGrid, false);
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
                var viewModel = this.DataContext as ViewAnalogousVM;
                IDictionary<String, Object> row = ConsolidatedGrid.SelectedItem as IDictionary<String, Object>;
                var selectedAnalagousReaction = (from r in viewModel.AnalogousReactions where r.Id == new Guid(row["RXNID"].ToString()) select r).FirstOrDefault();
                if (selectedAnalagousReaction != null)
                    viewModel.UpdateAnalogousReactionPreview(selectedAnalagousReaction);
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }
    }
}
