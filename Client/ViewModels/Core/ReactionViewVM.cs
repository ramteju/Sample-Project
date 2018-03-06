using Client.Common;
using Client.Logging;
using Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Forms;

namespace Client.ViewModels
{
    public class ReactionViewVM : ViewModelBase
    {
        private static readonly string SELECT_RXN = "Select Reaction . .";

        private string name;
        private Visibility enableEdit;
        private ObservableCollection<String> yieldProducts;
        private ObservableCollection<ReactionParticipantViewVM> productViews, reactantsViews;
        private ObservableCollection<StageViewVM> stages;
        private bool isReviewCompleted;
        private bool reviewEnable;
        private Guid reactionId;
        private string reviewOrQCCompleteText;

        public string Name { get { return name; } set { SetProperty(ref name, value); } }
        public Visibility EnableEdit { get { return enableEdit; } set { SetProperty(ref enableEdit, value); } }
        public Guid ReactionId { get { return reactionId; } set { SetProperty(ref reactionId, value); } }
        public bool ReviewEnable { get { return reviewEnable; } set { SetProperty(ref reviewEnable, value); } }
        public bool IsReviewCompleted
        {
            get
            { return isReviewCompleted; }
            set
            {
                SetProperty(ref isReviewCompleted, value);
                ReactionView_ReviewCompleted();
            }
        }
        private void ReactionView_ReviewCompleted()
        {
            try
            {
                if (U.RoleId == 2 || U.RoleId == 3)
                {
                    var main = ((App.Current.MainWindow as MainWindow).DataContext as MainVM);
                    foreach (var reaction in main.TanVM.Reactions)
                    {
                        if (reaction.Id == ReactionId)
                        {
                            reaction.IsReviewCompleted = IsReviewCompleted;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }
        public ObservableCollection<String> YieldProducts { get { return yieldProducts; } set { SetProperty(ref yieldProducts, value); } }
        public ObservableCollection<ReactionParticipantViewVM> ProductViews { get { return productViews; } set { SetProperty(ref productViews, value); } }
        public ObservableCollection<ReactionParticipantViewVM> ReactantsViews { get { return reactantsViews; } set { SetProperty(ref reactantsViews, value); } }
        public ObservableCollection<ReactionParticipantViewVM> EquationViews { get; set; }
        public ObservableCollection<StageViewVM> Stages { get { return stages; } set { SetProperty(ref stages, value); } }
        public string ReviewOrQCCompleteText
        {
            get
            {
                if (U.RoleId == 2)
                {
                    return "ReviewCompleted";
                }
                else if (U.RoleId == 3)
                {
                    return "QCCompleted";
                }
                return "Review/QC Status";
            }
        }


        public ReactionViewVM()
        {
            ProductViews = new ObservableCollection<ReactionParticipantViewVM>();
            ReactantsViews = new ObservableCollection<ReactionParticipantViewVM>();
            YieldProducts = new ObservableCollection<string>();
            Stages = new ObservableCollection<StageViewVM>();
            EquationViews = new ObservableCollection<ReactionParticipantViewVM>();
            Name = SELECT_RXN;
        }
    }
}
