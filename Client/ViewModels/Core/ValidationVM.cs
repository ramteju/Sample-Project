using Client.Command;
using Client.Common;
using Client.Logging;
using Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Client.ViewModels
{
    public class ValidationVM : ViewModelBase
    {
        private ObservableCollection<ValidationError> reactionErrors;
        private ObservableCollection<ValidationError> validationWarnings;
        private String errorsInfo;
        private String warningsInfo;
        private Brush errorTabColor;
        private ValidationError selectedError;
        private ValidationError selectedWarning;
        private Brush warningTabColor;

        public ObservableCollection<ValidationError> ValidationErrors { get { return reactionErrors; } set { SetProperty(ref reactionErrors, value); } }
        public ObservableCollection<ValidationError> ValidationWarnings { get { return validationWarnings; } set { SetProperty(ref validationWarnings, value); } }
        public String ErrorsInfo { get { return errorsInfo; } set { SetProperty(ref errorsInfo, value); } }
        public String WarningInfo { get { return warningsInfo; } set { SetProperty(ref warningsInfo, value); } }

        public ValidationVM()
        {
            ValidationErrors = new ObservableCollection<ViewModels.ValidationError>();
            ValidationWarnings = new ObservableCollection<ViewModels.ValidationError>();
            ErrorsInfo = "Errors";
            WarningInfo = "Warnings";
            SwitchToSelectedError = new Command.DelegateCommand(this.GoToSelectedReaction);
            SwitchToSelectedWarning = new Command.DelegateCommand(this.GoToSelectedReaction);
        }

        private void GoToSelectedReaction(object obj)
        {

            try
            {
                if(SelectedError != null)
                {
                    var mainViewModel = (App.Current.MainWindow as MainWindow).DataContext as MainVM;
                    if (mainViewModel.TanVM != null && SelectedError != null)
                    {
                        mainViewModel.TanVM.SelectedReaction = SelectedError?.ReactionVM;
                        if (mainViewModel.TanVM.SelectedReaction != null)
                            mainViewModel.TanVM.SelectedReaction.SelectedStage = SelectedError?.StageVM;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        public Brush ErrorTabColor { get { return errorTabColor; } set { SetProperty(ref errorTabColor, value); } }
        public Brush WarningTabColor { get { return warningTabColor; } set { SetProperty(ref warningTabColor, value); } }
        public ValidationError SelectedError { get { return selectedError; } set { SetProperty(ref selectedError, value); } }
        public ValidationError SelectedWarning { get { return selectedWarning; } set { SetProperty(ref selectedWarning, value); } }

        public DelegateCommand SwitchToSelectedError { get; private set; }
        public DelegateCommand SwitchToSelectedWarning { get; private set; }
    }

    public class ValidationError : ViewModelBase
    {
        public static readonly String RXN = "Reaction";
        public static readonly String STAGE = "Stage";
        public static readonly String S8000 = "8000";
        public static readonly String RSN = "RSN";
        public static readonly String CONDITION = "Condition";

        private String message, category;
        private ReactionVM reactionVM;
        private StageVM stageVM;

        public ReactionVM ReactionVM { get { return reactionVM; } set { SetProperty(ref reactionVM, value); } }
        public StageVM StageVM { get { return stageVM; } set { SetProperty(ref stageVM, value); } }
        public String Message { get { return message; } set { SetProperty(ref message, value); } }
        public String Category { get { return category; } set { SetProperty(ref category, value); } }
    }

    //short notation for ValidationFactory
    public static class VF
    {
        public static ValidationError OfReaction(ReactionVM reaction, string message)
        {
            return new ValidationError { ReactionVM = reaction, Message = message, Category = ValidationError.RXN };
        }
        public static ValidationError OfRSN(ReactionVM reaction, string message, StageVM stageVM = null)
        {
            return new ValidationError { ReactionVM = reaction, StageVM = stageVM, Message = message, Category = ValidationError.RSN };
        }
        public static ValidationError OfStage(ReactionVM reaction, string message, StageVM stageVM)
        {
            return new ValidationError { ReactionVM = reaction, StageVM = stageVM, Message = message, Category = ValidationError.STAGE };
        }
    }
}
