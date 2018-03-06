using Client.Command;
using Client.Common;
using Client.Logging;
using Client.Models;
using Client.ViewModel;
using Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;

namespace Client.ViewModels
{
    public class RsnVM : OrderableVM
    {
        private Guid id;
        private bool isRXN;
        private Visibility chooseRXN;
        private CvtVM cvt;
        private string cvtText;
        private String freeText;
        private ReactionVM reaction;
        private StageVM stage;
        private int displayOrder;
        private TanChemicalVM selectedChemcial;
        private bool isIgnorableInDelivery;
        public DelegateCommand EditRsn { get; private set; }
        public DelegateCommand DeleteRsn { get; private set; }

        [JsonIgnore]
        public RSNWindowVM RSNWindowVM;
        public RsnVM()
        {
            Id = Guid.NewGuid();
            FreeText = String.Empty;
            if (checkStageSelected())
                IsRXN = true;
            BindEvents();
        }

        public RsnVM(RsnVM otherRsnVM)
        {
            this.Id = otherRsnVM.Id;
            this.IsRXN = otherRsnVM.IsRXN;
            this.ChooseRXN = otherRsnVM.ChooseRXN;
            this.CvtText = otherRsnVM.CvtText;
            this.FreeText = otherRsnVM.FreeText;
            this.Reaction = otherRsnVM.Reaction;
            this.Stage = otherRsnVM.Stage;
            this.DisplayOrder = otherRsnVM.DisplayOrder;
            this.SelectedChemical = otherRsnVM.SelectedChemical;
            this.ReactionParticipantId = otherRsnVM.ReactionParticipantId;
            this.IsIgnorableInDelivery = otherRsnVM.IsIgnorableInDelivery;
            BindEvents();
        }
        private void BindEvents()
        {

            try
            {
                EditRsn = new DelegateCommand(this.DoEditRsn);
                DeleteRsn = new DelegateCommand(this.DoDeleteRsn);
            }
            catch (Exception ex)
            {
                Log.This(ex);

            }
        }
        private void DoDeleteRsn(object obj)
        {

            try
            {
                if (RSNWindowVM != null && System.Windows.MessageBox.Show("Confirm RSN Deletion . .", "Reactions", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    RSNWindowVM.DeleteRsn(this);
            }
            catch (Exception ex)
            {
                Log.This(ex);

            }
        }
        private void DoEditRsn(object obj)
        {

            try
            {
                if (RSNWindowVM != null && System.Windows.MessageBox.Show("Confirm RSN Edit . .", "Reactions", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    RSNWindowVM.FreeTextMode = FreeTextMode.REPLACE;
                    RSNWindowVM.EditRsn(this);
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);

            }
        }

        public Guid Id { get { return id; } set { SetProperty(ref id, value); } }

        public string EditIcon
        {
            get
            {
                return "/Images/edit.png";
            }
        }
        public string DeleteIcon
        {
            get
            {
                return "/Images/edit-delete.png";
            }
        }
        public bool IsRXN
        {
            get { return isRXN; }
            set
            {
                SetProperty(ref isRXN, value);
            }
        }
        public string Level
        {
            get
            {
                if (Stage != null)
                    return $"Stage";
                else if (Reaction != null)
                    return $"Reaction";
                return String.Empty;
            }
        }
        public Visibility ChooseRXN { get { return chooseRXN; } set { SetProperty(ref chooseRXN, value); } }

        public String FreeText { get { return freeText; } set { SetProperty(ref freeText, value); } }
        public String FreeTextWithRxn
        {
            get
            {
                if (Stage == null)
                {
                    if (!String.IsNullOrEmpty(FreeText))
                        return $"{FreeText} (Reaction)";
                }
                return FreeText;
            }
        }
        public String CvtText { get { return cvtText; } set { SetProperty(ref cvtText, value); } }
        public String CvtTextWithRxn
        {
            get
            {
                if (Stage == null)
                {
                    if (!String.IsNullOrEmpty(CvtText))
                        return $"{CvtText} (Reaction)";
                }
                return CvtText;
            }
        }
        public ReactionVM Reaction { get { return reaction; } set { SetProperty(ref reaction, value); } }
        public StageVM Stage { get { return stage; } set { SetProperty(ref stage, value); } }
        public override int DisplayOrder
        {
            get { return displayOrder; }
            set
            {
                SetProperty(ref displayOrder, value);
            }
        }
        public bool checkStageSelected()
        {
            try
            {
                var mainViewModel = (App.Current.MainWindow as MainWindow).DataContext as MainVM;
                return mainViewModel.TanVM.SelectedReaction?.SelectedStage == null;
            }
            catch (Exception ex)
            {
                Log.This(ex);
                return false;
            }
        }
        public override bool Equals(Object other)
        {
            try
            {
                if (other == null || GetType() != other.GetType())
                    return false;

                var otherRsn = (RsnVM)other;
                return Id == otherRsn.id;
            }
            catch (Exception ex)
            {
                Log.This(ex);
                return false;
            }
        }
        //public bool IsIgnorableInDelivery { get; set; }
        public List<Guid> ReactionParticipantId { get; set; }
        public TanChemicalVM SelectedChemical { get { return selectedChemcial; } set { SetProperty(ref selectedChemcial, value); } }
        public bool IsIgnorableInDelivery { get { return isIgnorableInDelivery; } set { SetProperty(ref isIgnorableInDelivery, value); } }
    }
}
