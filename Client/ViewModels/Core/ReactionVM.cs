using Client.Command;
using Client.Common;
using Client.Logging;
using Client.Util;
using Client.ViewModel;
using Client.Views;
using DTO;
using Entities;
using ProductTracking.Models.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client.ViewModels
{
    public class ReactionVM : OrderableVM
    {
        private Guid id;
        private StageVM selectedStage;
        private ObservableCollection<StageVM> stages;
        private String stageLevelTitle, name;
        private Guid? analogousVMId;
        private TanVM tanVm;
        private int displayOrder;
        private bool enableUpdate;
        private bool isCurationCompleted;
        private bool isReviewCompleted;
        private string keyProductSeq;
        private int yield;
        private DateTime lastupdatedDate;
        private DateTime reviewlastupdatedDate;
        private DateTime qclastupdatedDate;
        private DateTime curatorCreatedDate;
        private DateTime reviewerCreatedDate;
        private DateTime curatorCompletedDate;
        private DateTime reviewerCompletedDate;
        private DateTime qcCompletedDate;

        public DelegateCommand DeleteStage { get; private set; }
        public event EventHandler<StageVM> StageSelectedEvent = delegate { };


        public Guid Id { get { return id; } set { SetProperty(ref id, value); } }
        public Guid? AnalogousVMId { get { return analogousVMId; } set { SetProperty(ref analogousVMId, value); } }
        public int Yield { get { return yield; } set { SetProperty(ref yield, value); } }
        public string KeyProductSeq { get { return keyProductSeq; } set { SetProperty(ref keyProductSeq, value); } }
        public String Name
        {
            get { return name; }
            private set { SetProperty(ref name, value); }
        }
        public bool IsCurationCompleted
        {
            get { return isCurationCompleted; }
            set
            {
                var mainVM = (App.Current.MainWindow as MainWindow).DataContext as MainVM;
                if (value && mainVM!=null && mainVM.TanVM != null && mainVM.TanVM.SelectedReaction != null)
                {
                    if (mainVM.Validate(true))
                        SetProperty(ref isCurationCompleted, value);
                    else
                        AppInfoBox.ShowInfoMessage("Please solve the reaction validations");
                }
                else
                    SetProperty(ref isCurationCompleted, value);
            }
        }
        public bool IsReviewCompleted
        {
            get { return isReviewCompleted; }
            set
            {
                if (value)
                {
                    if (TanVM != null && (TanVM.ReactionParticipants.Count == 0 || ReactionValidations.IsValidReaction(this,null,null,null).Count == 0))
                        SetProperty(ref isReviewCompleted, value);
                    else
                        AppInfoBox.ShowInfoMessage("Please solve the reaction validations");
                }
                SetProperty(ref isReviewCompleted, value);
            }
        }
        public ObservableCollection<StageVM> Stages
        {
            get { return stages; }
            private set
            {
                SetProperty(ref stages, value);
            }
        }
        public void SetStages(List<StageVM> stages, int InsertPosition = 0, bool fromCopyStages = false, bool fromAnalogous = false)
        {
            try
            {
                int IndexToInsert = InsertPosition;
                enableUpdate = false;
                foreach (var stage in stages)
                {
                    if (InsertPosition == 0 && !fromCopyStages)
                        Stages.Add(stage);
                    else
                    {
                        TanVM.SelectedReaction.Stages.Insert(IndexToInsert, stage);
                        IndexToInsert++;
                    }
                }
                Stages.UpdateDisplayOrder();
                enableUpdate = true;
                if (!fromAnalogous)
                    Stages_CollectionChanged(null, null);
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        public override int DisplayOrder
        {
            get { return displayOrder; }
            set
            {
                SetProperty(ref displayOrder, value);
                //RefreshName();
            }
        }
        public string DisplayName { get { return $"Reaction {DisplayOrder}"; } }

        public void RefreshName()
        {
            var start = DateTime.Now;
            try
            {
                StringBuilder nameBuilder = new StringBuilder();
                if (TanVM != null && TanVM.ReactionParticipants != null)
                {
                    var products = TanVM.ReactionParticipants.OfReactionOfType(this.Id, ParticipantType.Product);
                    nameBuilder.Append(DisplayOrder);
                    if (products != null)
                    {
                        foreach (var product in products)
                        {
                            var allReactionsOfThisProduct = TanVM.ReactionParticipants.Where(rp => rp.ParticipantType == ParticipantType.Product && rp.KeyProduct && rp.Num == product.Num).Select(rp => rp.ReactionVM).OrderBy(r => r.DisplayOrder).ToList();
                            var thisProductSequenceOrder = allReactionsOfThisProduct.FindIndex(r => r.Id == product.ReactionVM.Id) + 1;
                            nameBuilder.Append("[");
                            nameBuilder.Append(product.Num);
                            if (product.KeyProduct)
                                nameBuilder.Append("-" + thisProductSequenceOrder);
                            nameBuilder.Append("]");
                            if (product.KeyProduct)
                            {
                                KeyProductSeq = product.Num + "-" + thisProductSequenceOrder;
                                product.KeyProductSeqWithOutNum = thisProductSequenceOrder;
                            }
                        }
                        foreach (var product in products)
                        {
                            if (!string.IsNullOrEmpty(product.Yield) && product.Yield != "0")
                                nameBuilder.Append("(" + product.Yield + ")");
                        }
                    }
                    if (AnalogousVMId != null && AnalogousVMId != Guid.Empty)
                    {
                        nameBuilder.Append(" :: A ");
                        var masterReaction = TanVM.Reactions.FirstOrDefault(r => r.Id == AnalogousVMId);
                        if (masterReaction != null)
                        {
                            var masterFirstProduct = TanVM.ReactionParticipants.OfReaction(masterReaction.Id).OrderBy(rp => rp.DisplayOrder).FirstOrDefault();
                            nameBuilder.Append(masterReaction.DisplayOrder);
                            nameBuilder.Append("[");
                            if (masterFirstProduct != null)
                                nameBuilder.Append(masterFirstProduct.Num);
                            nameBuilder.Append("]");
                            //if (masterFirstProduct != null && !string.IsNullOrEmpty(masterFirstProduct.Yield) && masterFirstProduct.Yield != "0")
                            //    nameBuilder.Append("(" + masterFirstProduct.Yield + ")");
                        }
                    }
                    //Debug.WriteLine($"RefreshReactionName Done at {DateTime.Now} in {(DateTime.Now - start).TotalSeconds} Seconds");
                }
                Name = nameBuilder.ToString();
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }
        public String StageLevelTitle { get { return stageLevelTitle; } set { SetProperty(ref stageLevelTitle, value); } }
        public DateTime CuratorCreatedDate { get { return curatorCreatedDate; } set { SetProperty(ref curatorCreatedDate, value); } }
        public DateTime LastupdatedDate { get { return lastupdatedDate; } set { SetProperty(ref lastupdatedDate, value); } }
        public DateTime CuratorCompletedDate { get { return curatorCompletedDate; } set { SetProperty(ref curatorCompletedDate, value); } }
        public DateTime ReviewLastupdatedDate { get { return reviewlastupdatedDate; } set { SetProperty(ref reviewlastupdatedDate, value); } }
        public DateTime ReviewerCompletedDate { get { return reviewerCompletedDate; } set { SetProperty(ref reviewerCompletedDate, value); } }
        public DateTime ReviewerCreatedDate { get { return reviewerCreatedDate; } set { SetProperty(ref reviewerCreatedDate, value); } }
        public DateTime QCLastupdatedDate { get { return qclastupdatedDate; } set { SetProperty(ref qclastupdatedDate, value); } }
        public DateTime QcCompletedDate { get { return qcCompletedDate; } set { SetProperty(ref qcCompletedDate, value); } }

        public TanVM TanVM
        {
            get { return tanVm; }
            set
            {
                SetProperty(ref tanVm, value);
                RefreshName();
            }
        }

        public StageVM SelectedStage
        {
            get { return selectedStage; }
            set
            {
                var startTime = DateTime.Now;
                SetProperty(ref selectedStage, value);
                TanVM.UpdateParticipantsView();
                ((App.Current.MainWindow as MainWindow).FindName("MainEditor") as WorkingArea).ShowPopupWindow(true);
                Debug.WriteLine("Stage Selection : " + (DateTime.Now - startTime).TotalSeconds + "Seconds");
            }
        }
        public ReactionVM()
        {
            enableUpdate = true;
            Stages = new ObservableCollection<StageVM>();
            Stages.CollectionChanged += Stages_CollectionChanged;
            DeleteStage = new Command.DelegateCommand(this.RemoveStage);
        }

        private void RemoveStage(object obj)
        {
            try
            {
                ((App.Current.MainWindow as MainWindow).DataContext as MainVM).DeleteStage.Execute(this);
                if (U.RoleId == 1)
                    TanVM.SelectedReaction.IsCurationCompleted = false;
                else if (U.RoleId == 2)
                    TanVM.SelectedReaction.IsReviewCompleted = false;
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private void Stages_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            try
            {
                if (enableUpdate)
                {
                    Stages.UpdateDisplayOrder();
                    //if (U.RoleId == 1)
                    //    LastupdatedDate = DateTime.Now;
                    //else if (U.RoleId == 2)
                    //    ReviewLastupdatedDate = DateTime.Now;
                    //else if (U.RoleId == 3)
                    //    QCLastupdatedDate = DateTime.Now;
                    TanVM.UpdateParticipantsView();
                    TanVM.UpdateReactionPreview();
                    if (TanVM.ReactionParticipants != null && TanVM.SelectedReaction != null)
                    {
                        string RSDString = Common.ReactionValidations.GetRSDString(TanVM.ReactionParticipants.Where(rp => rp.ReactionVM.Id == TanVM.SelectedReaction.Id).ToList(), TanVM.SelectedReaction);
                        TanVM.Rsd = RSDString;
                        if (U.RoleId == 1)
                            TanVM.SelectedReaction.IsCurationCompleted = false;
                        else if (U.RoleId == 2)
                            TanVM.SelectedReaction.IsReviewCompleted = false;
                        ((App.Current.MainWindow as MainWindow).DataContext as MainVM).Validate(true);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }
    }
}
