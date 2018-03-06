using Client.Command;
using Client.Common;
using Client.Converters;
using Client.Models;
using Client.Styles;
using Client.Util;
using Client.Views;
using Client.XML;
using DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Telerik.Windows.Controls;

namespace Client.ViewModels
{
    public class ViewAnalogousVM : ViewModelBase
    {
        private static readonly string PLUS_ICON = ConfigurationManager.AppSettings["PLUS_ICON"];
        private static readonly string ARROW_ICON = ConfigurationManager.AppSettings["ARROW_ICON"];

        public event EventHandler<ReactionVM> MasterReactionChanged = delegate { };

        public ViewAnalogousVM()
        {
            AnalogousReactions = new ObservableCollection<ViewModels.ReactionVM>();
            MasterReactions = new ObservableCollection<ViewModels.ReactionVM>();
            mainViewModel = ((MainWindow)(App.Current.MainWindow)).DataContext as MainVM;
            ReactionParticipants = new ObservableCollection<ViewModels.ReactionParticipantVM>();
            Rsns = new ObservableCollection<ViewModels.RsnVM>();
            AllParticipants = new ObservableCollection<ViewModels.ReactionParticipantVM>();
            MasterReactionPreview = new ViewModels.ReactionViewVM();
            UpdateReactionPreview(SelectedMasterReaction);
            AnalogousReactionPreview = new ViewModels.ReactionViewVM();
        }

        private ReactionVM selectedMasterReaction;
        private ObservableCollection<ReactionVM> copiedReactions;
        private ObservableCollection<ReactionVM> masterReactions;
        private ObservableCollection<ReactionParticipantVM> reactionparticipants;
        private ObservableCollection<ReactionParticipantVM> allparticipants;
        private ObservableCollection<RsnVM> rsns;
        private ReactionViewVM masterReactionPreview;
        private ReactionViewVM analogousReactionPreview;
        private int analogoursIndex;
        public ReactionVM SelectedMasterReaction
        {
            get { return selectedMasterReaction; }
            set
            {
                SetProperty(ref selectedMasterReaction, value);
                RefreshView();
            }
        }
        public void UpdateReactionPreview(ReactionVM SelectedReaction)
        {
            var mainVM = (App.Current.MainWindow as MainWindow).DataContext as MainVM;
            #region Curator View
            if (MasterReactionPreview != null)
            {
                if (SelectedReaction != null)
                    MasterReactionPreview = ReactionViewBuilder.GetReactionView(SelectedReaction, mainVM.TanVM.ReactionParticipants.ToList());
                else
                    MasterReactionPreview.Name = "Select Reaction To Preview . .";
            }
            #endregion
        }
        public void UpdateAnalogousReactionPreview(ReactionVM SelectedReaction)
        {
            if (AnalogousReactionPreview != null)
            {
                if (SelectedReaction != null)
                    AnalogousReactionPreview = ReactionViewBuilder.GetReactionView(SelectedReaction, AllParticipants.ToList());
                else
                    AnalogousReactionPreview.Name = "Select Reaction To Preview . .";
            }
        }
        public MainVM mainViewModel;
        public ObservableCollection<ReactionVM> AnalogousReactions { get { return copiedReactions; } set { SetProperty(ref copiedReactions, value); } }
        public ObservableCollection<ReactionVM> MasterReactions { get { return masterReactions; } set { SetProperty(ref masterReactions, value); } }
        public ObservableCollection<ReactionParticipantVM> ReactionParticipants { get { return reactionparticipants; } set { SetProperty(ref reactionparticipants, value); } }
        public ObservableCollection<ReactionParticipantVM> AllParticipants { get { return allparticipants; } set { SetProperty(ref allparticipants, value); } }
        public ObservableCollection<RsnVM> Rsns { get { return rsns; } set { SetProperty(ref rsns, value); } }
        public ReactionViewVM MasterReactionPreview { get { return masterReactionPreview; } set { SetProperty(ref masterReactionPreview, value); } }
        public ReactionViewVM AnalogousReactionPreview { get { return analogousReactionPreview; } set { SetProperty(ref analogousReactionPreview, value); } }
        public ViewAnalogousVM CollectExistingAnalogousReactions()
        {
            var mainViewModel = ((MainWindow)(App.Current.MainWindow)).DataContext as MainVM;
            List<RsnVM> tanRsns = new List<RsnVM>();
            List<ReactionVM> tanReactions = new List<ReactionVM>();
            var tanParticipants = new List<ReactionParticipantVM>();
            AnalogousReactions.Clear();
            AllParticipants.Clear();
            ReactionParticipants.Clear();
            try
            {
                var analogousReactions = mainViewModel.TanVM.Reactions.Where(r => r.AnalogousVMId == SelectedMasterReaction.Id);
                foreach (var existingAnalogousReaction in analogousReactions)
                {
                    AnalogousReactions.Add(existingAnalogousReaction);
                    AllParticipants.AddRange(mainViewModel.TanVM.ReactionParticipants.Where(rp => rp.ReactionVM.Id == existingAnalogousReaction.Id));
                    ReactionParticipants.AddRange(mainViewModel.TanVM.ReactionParticipants.Where(rp => rp.ReactionVM.Id == existingAnalogousReaction.Id));
                    foreach (var rsn in (mainViewModel.TanVM.Rsns.Where(rsn => rsn.Reaction.Id == existingAnalogousReaction.Id)))
                        Rsns.Add(rsn);

                }
            }
            catch (Exception ex)
            {
                AppErrorBox.ShowErrorMessage("Error while collecting analogous reactions", ex.Message);
            }
            return this;
        }
        public void RefreshView()
        {
            AnalogousReactions.Clear();
            AnalogousReactionPreview = new ReactionViewVM();
            MasterReactionPreview = new ReactionViewVM();
            Rsns = new ObservableCollection<ViewModels.RsnVM>(mainViewModel.TanVM.Rsns.Where(p => p.Reaction.Id == SelectedMasterReaction.Id).ToList());
            var participants = mainViewModel.TanVM.ReactionParticipants.Where(rp => rp.ReactionVM.Id == SelectedMasterReaction.Id).Select(rp => rp);
            AllParticipants = new ObservableCollection<ViewModels.ReactionParticipantVM>(participants);
            UpdateReactionPreview(SelectedMasterReaction);
            UpdateAnalogousReactionPreview(null);
            MasterReactionChanged.Invoke(this, SelectedMasterReaction);
        }

    }
}
