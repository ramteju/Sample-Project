using Client.Command;
using Client.Common;
using Client.Logging;
using Client.ViewModel;
using DTO;
using Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace Client.ViewModels
{
    public class StageVM : OrderableVM
    {
        private Guid id;
        private string name;
        private int displayOrder;
        private ReactionVM reactionVm;
        private ObservableCollection<StageConditionVM> conditions;
        private StageConditionVM selectedCondition;
        private bool beforeInsert;
        private bool afterInsert;

        public Guid Id { get { return id; } set { SetProperty(ref id, value); } }
        public override int DisplayOrder
        {
            get { return displayOrder; }
            set
            {
                SetProperty(ref displayOrder, value);
                RefreshName();
            }
        }
        public String Name
        {
            get { return name; }
            private set { SetProperty(ref name, value); }
        }
        public ReactionVM ReactionVm
        {
            get { return reactionVm; }
            set { SetProperty(ref reactionVm, value); }
        }
        public ObservableCollection<StageConditionVM> Conditions { get { return conditions; } set { SetProperty(ref conditions, value); } }
        public StageConditionVM SelectedCondition { get { return selectedCondition; } set { SetProperty(ref selectedCondition, value); } }
        public bool BeforeInsert { get { return beforeInsert; } set { SetProperty(ref beforeInsert, value); } }
        public bool AfterInsert { get { return afterInsert; } set { SetProperty(ref afterInsert, value); } }

        public DelegateCommand DeleteStageCondition { get; private set; }

        public StageVM()
        {
            Conditions = new ObservableCollection<ViewModels.StageConditionVM>();
            Conditions.CollectionChanged += Conditions_CollectionChanged;
            DeleteStageCondition = new Command.DelegateCommand(this.DoDeleteStageCondition);
        }

        private void DoDeleteStageCondition(object obj)
        {

            try
            {
                if (SelectedCondition != null)
                {
                    DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("Are you sure you want to remove the Selected Condition", "Conditions", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (dialogResult == DialogResult.Yes)
                    {
                        Conditions.Remove(SelectedCondition);
                        var mainVM = (App.Current.MainWindow as MainWindow).DataContext as MainVM;
                        if (mainVM != null && mainVM.TanVM != null && mainVM.TanVM.SelectedReaction != null)
                        {
                            if (U.RoleId == 1)
                                mainVM.TanVM.SelectedReaction.LastupdatedDate = DateTime.Now;
                            else if (U.RoleId == 2)
                                mainVM.TanVM.SelectedReaction.ReviewLastupdatedDate = DateTime.Now;
                            else if (U.RoleId == 3)
                                mainVM.TanVM.SelectedReaction.QCLastupdatedDate = DateTime.Now;
                            mainVM.TanVM.PerformAutoSave("Deleted Stage Condition");
                        }
                    }
                }
                else
                    System.Windows.Forms.MessageBox.Show("Please Select Condition to delete", "Conditions");
            }
            catch (Exception ex)
            {
                Log.This(ex);

            }
        }

        public void SetConditions(List<StageConditionVM> conditions)
        {
            try
            {
                foreach (var condition in conditions)
                    Conditions.Add(condition);
                Conditions.UpdateDisplayOrder();
            }
            catch (Exception ex)
            {
                Log.This(ex);

            }
        }

        private void Conditions_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {

            try
            {
                Conditions.UpdateDisplayOrder();
            }
            catch (Exception ex)
            {
                Log.This(ex);

            }
        }

        public override string ToString()
        {
            return Name;
        }

        public void RefreshName()
        {

            try
            {
                Name = $"Stage-{DisplayOrder}";
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }
    }
}
