using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace Client.ViewModels
{
    public class StageViewVM : ViewModelBase
    {
        public StageViewVM()
        {
            Temperature = Time = String.Empty;
            Pressure = Ph = String.Empty;
            StageVM = null;
            AgentsViews = new ObservableCollection<ReactionParticipantViewVM>();
            SolventsViews = new ObservableCollection<ReactionParticipantViewVM>();
            ReactantsViews = new ObservableCollection<ReactionParticipantViewVM>();
            CatalystsViews = new ObservableCollection<ReactionParticipantViewVM>();
        }

        public void Clear()
        {
            Temperature = Time = String.Empty;
            Pressure = Ph = String.Empty;
            StageVM = null;
            AgentsViews.Clear();
            ReactantsViews.Clear();
            SolventsViews.Clear();
            CatalystsViews.Clear();
        }

        private StageVM stageVM;
        private string freeText, cvt;
        private ObservableCollection<ReactionParticipantViewVM> agentViews, solventViews, catalystsViews;
        private string temperature;
        private string time;
        private string pressure;
        private string ph;
        private Visibility showSolvents, showAgents, showCatalysts, showTemperature, showTime, showPressure, showPh, showCVT, showFreeText;
        private Visibility showReactants;
        private ObservableCollection<ReactionParticipantViewVM> reactantViews;
        private Visibility conditionsVisibility;
        public StageVM StageVM { get { return stageVM; } set { SetProperty(ref stageVM, value); } }
        public string FreeText
        {
            get { return freeText; }
            set
            {
                SetProperty(ref freeText, value);
                ShowFreeText = !String.IsNullOrEmpty(value) ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        public string CVT
        {
            get { return cvt; }
            set
            {
                SetProperty(ref cvt, value);
                ShowCVT = !String.IsNullOrEmpty(value) ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        public string Temperature
        {
            get { return temperature; }
            set
            {
                SetProperty(ref temperature, value);
                ShowTemperature = !String.IsNullOrEmpty(value) ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        public string Time
        {
            get { return time; }
            set
            {
                SetProperty(ref time, value);
                showTime = !String.IsNullOrEmpty(value) ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        public string Pressure
        {
            get { return pressure; }
            set
            {
                SetProperty(ref pressure, value);
                ShowPressure = !String.IsNullOrEmpty(value) ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        public string Ph
        {
            get { return ph; }
            set
            {
                SetProperty(ref ph, value);
                ShowPh = !String.IsNullOrEmpty(value) ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        public ObservableCollection<ReactionParticipantViewVM> AgentsViews { get { return agentViews; } set { SetProperty(ref agentViews, value); } }
        public ObservableCollection<ReactionParticipantViewVM> ReactantsViews { get { return reactantViews; } set { SetProperty(ref reactantViews, value); } }
        public ObservableCollection<ReactionParticipantViewVM> SolventsViews { get { return solventViews; } set { SetProperty(ref solventViews, value); } }
        public ObservableCollection<ReactionParticipantViewVM> CatalystsViews { get { return catalystsViews; } set { SetProperty(ref catalystsViews, value); } }


        public Visibility ShowReactants { get { return showReactants; } set { SetProperty(ref showReactants, value); } }
        public Visibility ShowSolvents { get { return showSolvents; } set { SetProperty(ref showSolvents, value); } }
        public Visibility ShowAgents { get { return showAgents; } set { SetProperty(ref showAgents, value); } }
        public Visibility ShowCatalysts { get { return showCatalysts; } set { SetProperty(ref showCatalysts, value); } }
        public Visibility ShowTemperature { get { return showTemperature; } set { SetProperty(ref showTemperature, value); } }
        public Visibility ShowTime { get { return showTime; } set { SetProperty(ref showTime, value); } }
        public Visibility ShowPressure { get { return showPressure; } set { SetProperty(ref showPressure, value); } }
        public Visibility ShowPh { get { return showPh; } set { SetProperty(ref showPh, value); } }
        public Visibility ShowCVT { get { return showCVT; } set { SetProperty(ref showCVT, value); } }
        public Visibility ShowFreeText { get { return showFreeText; } set { SetProperty(ref showFreeText, value); } }
        public Visibility ConditionsVisibility { get { return conditionsVisibility; } set { SetProperty(ref conditionsVisibility, value); } }

        public Visibility GetConditionsVisibility()
        {
            if (string.IsNullOrEmpty(Pressure) && string.IsNullOrEmpty(Time) && string.IsNullOrEmpty(Ph) && string.IsNullOrEmpty(Temperature))
                return Visibility.Collapsed;
            return Visibility.Visible;
        }
    }
}
