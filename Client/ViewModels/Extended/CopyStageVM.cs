using Client.Command;
using Client.Converters;
using DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telerik.Windows.Controls;
using Client.Views;

namespace Client.ViewModels
{
    public class CopyStageVM : ViewModelBase
    {
        public CopyStageVM()
        {
            CopyStages = new Command.DelegateCommand(this.CopySelectedStages);
            Stages = new ObservableCollection<StageVM>();
            SelectedStages = new List<ViewModels.StageVM>();
            var mainViewModel = ((MainWindow)(App.Current.MainWindow)).DataContext as MainVM;
            Stages = mainViewModel.TanVM.SelectedReaction.Stages;
            IsAgentsSelected = true;
            IsCatalystSelected = true;
            IsConditionsSelected = true;
            IsRSNSelected = true;
            IsSolventsSelected = true;
            Visible = System.Windows.Visibility.Visible;
            StagesCountToCopy = 1;
        }

        private void CopySelectedStages(object obj)
        {
            if (SelectedStages.Count > 0)
            {
                var mainViewModel = ((MainWindow)(App.Current.MainWindow)).DataContext as MainVM;
                if (SelectedStageOption != CopyStageOptions.APPEND && (mainViewModel.TanVM.SelectedReaction == null || mainViewModel.TanVM.SelectedReaction.SelectedStage == null))
                {
                    AppInfoBox.ShowInfoMessage("Please select stage to Add stages before/after in curation window.");
                    return;
                }
                List<ReactionParticipantVM> allParticipants = new List<ReactionParticipantVM>();
                List<StageVM> reactionStages = new List<StageVM>();
                var tanParticipants = new List<ReactionParticipantVM>();
                if (IsReactantsSelected)
                {
                    var reactants = (from p in mainViewModel.TanVM.ReactionParticipants where p.ReactionVM.Id == mainViewModel.TanVM.SelectedReaction.Id && p.ParticipantType == ParticipantType.Reactant select p).ToList();
                    allParticipants.AddRange(reactants);
                }
                if (IsSolventsSelected || IsAgentsSelected || IsCatalystSelected)
                {
                    if (IsSolventsSelected)
                    {
                        var solvents = (from p in mainViewModel.TanVM.ReactionParticipants where p.ReactionVM.Id == mainViewModel.TanVM.SelectedReaction.Id && p.ParticipantType == ParticipantType.Solvent select p).ToList();
                        allParticipants.AddRange(solvents);
                    }

                    if (IsAgentsSelected)
                    {
                        var agents = (from p in mainViewModel.TanVM.ReactionParticipants where p.ReactionVM.Id == mainViewModel.TanVM.SelectedReaction.Id && p.ParticipantType == ParticipantType.Agent select p).ToList();
                        allParticipants.AddRange(agents);
                    }

                    if (IsCatalystSelected)
                    {
                        var catalysts = (from p in mainViewModel.TanVM.ReactionParticipants where p.ReactionVM.Id == mainViewModel.TanVM.SelectedReaction.Id && p.ParticipantType == ParticipantType.Catalyst select p).ToList();
                        allParticipants.AddRange(catalysts);
                    }
                }
                else
                {
                    AppInfoBox.ShowInfoMessage("Please select Atleast one type of Participants to Copy.");
                    return;
                }

                var selectedreaction = mainViewModel.TanVM.SelectedReaction;
                List<RsnVM> tanRsns = new List<RsnVM>();
                var stages = new List<StageVM>();
                for (int i = 0; i < StagesCountToCopy; i++)
                {
                    foreach (var selectedstage in SelectedStages)
                    {
                        var newStageToAdd = new StageVM
                        {
                            Id = Guid.NewGuid(),
                            ReactionVm = selectedreaction
                        };
                        if (IsConditionsSelected)
                        {
                            var Conditions = new List<StageConditionVM>();
                            foreach (var selectedCondition in selectedstage.Conditions)
                            {
                                var condition = new StageConditionVM
                                {
                                    DisplayOrder = selectedCondition.DisplayOrder,
                                    Id = Guid.NewGuid(),
                                    PH = selectedCondition.PH,
                                    PH_TYPE = selectedCondition.PH_TYPE,
                                    Pressure = selectedCondition.Pressure,
                                    PRESSURE_TYPE = selectedCondition.PRESSURE_TYPE,
                                    StageId = newStageToAdd.Id,
                                    Temperature = selectedCondition.Temperature,
                                    TEMP_TYPE = selectedCondition.TEMP_TYPE,
                                    Time = selectedCondition.Time,
                                    TIME_TYPE = selectedCondition.TIME_TYPE
                                };
                                Conditions.Add(condition);
                            }
                            newStageToAdd.SetConditions(Conditions);
                        }
                        else
                            newStageToAdd.Conditions = new ObservableCollection<StageConditionVM>();
                        var stageParticipants = (from sp in allParticipants where sp.StageVM.Id == selectedstage.Id select sp).ToList();
                        foreach (var participant in stageParticipants)
                        {
                            var newParticipant = new ReactionParticipantVM
                            {
                                ChemicalType = participant.ChemicalType,
                                DisplayOrder = participant.DisplayOrder,
                                Name = participant.Name,
                                Num = participant.Num,
                                ParticipantType = participant.ParticipantType,
                                ReactionVM = selectedreaction,
                                Reg = participant.Reg,
                                StageVM = newStageToAdd,
                                TanChemicalId = participant.TanChemicalId,
                                Yield = participant.Yield,
                                Id = Guid.NewGuid()
                            };
                            tanParticipants.Add(newParticipant);
                        }
                        if (IsRSNSelected)
                        {
                            var stagersnList = (from rsn in mainViewModel.TanVM.Rsns where rsn.Reaction.Id == selectedreaction.Id && rsn.Stage != null && rsn.Stage.Id == selectedstage.Id select rsn).ToList();
                            foreach (var rsn in stagersnList)
                            {
                                var newRsn = new RsnVM
                                {
                                    CvtText = rsn.CvtText,
                                    Reaction = selectedreaction,
                                    IsRXN = false,
                                    Stage = newStageToAdd,
                                    FreeText = rsn.FreeText,
                                    Id = Guid.NewGuid(),
                                    DisplayOrder = rsn.DisplayOrder,
                                    IsIgnorableInDelivery = rsn.IsIgnorableInDelivery,
                                    ReactionParticipantId = rsn.ReactionParticipantId,
                                    SelectedChemical = rsn.SelectedChemical
                                };
                                tanRsns.Add(newRsn);
                            }
                        }
                        stages.Add(newStageToAdd);
                    }
                }
                mainViewModel.TanVM.EnablePreview = false;
                foreach (var participant in tanParticipants)
                    mainViewModel.TanVM.ReactionParticipants.Add(participant);
                foreach (var rsn in tanRsns)
                    mainViewModel.TanVM.Rsns.Add(rsn);
                mainViewModel.TanVM.EnablePreview = true;
                if (SelectedStageOption != CopyStageOptions.APPEND && mainViewModel.TanVM.SelectedReaction != null && mainViewModel.TanVM.SelectedReaction.SelectedStage != null)
                {
                    var stagesList = mainViewModel.TanVM.SelectedReaction.Stages.ToList();
                    var index = stagesList.Count() >= 1 ? stagesList.FindIndex(x => x.Id == mainViewModel.TanVM.SelectedReaction.SelectedStage.Id) : 0;
                    if (SelectedStageOption == CopyStageOptions.AFTER)
                        selectedreaction.SetStages(stages, index + 1,true);
                    else if (SelectedStageOption == CopyStageOptions.BEFORE)
                        selectedreaction.SetStages(stages, index,true);
                }
                else
                    selectedreaction.SetStages(stages);
                mainViewModel.TanVM.PerformAutoSave("Stage Added");
                Visible = System.Windows.Visibility.Hidden;
                AppInfoBox.ShowInfoMessage("Please Update the Stage Freetext Stage Information for newly created stages.");
            }
            else
                AppInfoBox.ShowInfoMessage("Please Select AtLeast One stage");

        }

        private ObservableCollection<StageVM> stages;
        private StageVM selectedStage;
        private bool isReactantsSelected;
        private bool isAgentsSelected;
        private bool isSolventsSelected;
        private bool isCatalystSelected;
        private bool isConditionsSelected;
        private bool isRSNSelected;
        private int stagesCountToCopy;
        private System.Windows.Visibility visible;
        private List<StageVM> selectedStages;
        private CopyStageOptions selectedStageOption;

        public ObservableCollection<StageVM> Stages
        {
            get { return stages; }
            set
            {
                SetProperty(ref stages, value);
            }
        }
        public StageVM SelectedStage
        {
            get { return selectedStage; }
            set
            {
                SetProperty(ref selectedStage, value);
            }
        }

        public List<StageVM> SelectedStages
        {
            get { return selectedStages; }
            set
            {
                SetProperty(ref selectedStages, value);
            }
        }
        public bool IsReactantsSelected { get { return isReactantsSelected; } set { SetProperty(ref isReactantsSelected, value); } }
        public bool IsAgentsSelected { get { return isAgentsSelected; } set { SetProperty(ref isAgentsSelected, value); } }
        public bool IsSolventsSelected { get { return isSolventsSelected; } set { SetProperty(ref isSolventsSelected, value); } }
        public bool IsCatalystSelected { get { return isCatalystSelected; } set { SetProperty(ref isCatalystSelected, value); } }
        public bool IsConditionsSelected { get { return isConditionsSelected; } set { SetProperty(ref isConditionsSelected, value); } }
        public bool IsRSNSelected { get { return isRSNSelected; } set { SetProperty(ref isRSNSelected, value); } }
        public int StagesCountToCopy { get { return stagesCountToCopy; } set { SetProperty(ref stagesCountToCopy, value); } }
        public System.Windows.Visibility Visible { get { return visible; } set { SetProperty(ref visible, value); } }
        public CopyStageOptions SelectedStageOption { get { return selectedStageOption; } set { SetProperty(ref selectedStageOption, value); } }
        public Command.DelegateCommand CopyStages { get; private set; }
    }

    public enum CopyStageOptions
    {
        [Description("Append stages to existing stages")]
        APPEND = 0,
        [Description("Insert Stage Before Selected stage")]
        BEFORE = 1,
        [Description("Insert Stage After Selected stage")]
        AFTER = 2
    }
}
