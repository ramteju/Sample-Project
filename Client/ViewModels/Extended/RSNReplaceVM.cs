using Client.Command;
using Client.ViewModel;
using Client.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Forms;

namespace Client.ViewModels.Extended
{
    public class RSNReplaceVM : ViewModelBase
    {
        private ObservableCollection<FreetextPropertiesVM> tanFreetexts;
        private string targetText;
        private string replaceText;
        private ListCollectionView tanFreeTextsView;

        public ObservableCollection<FreetextPropertiesVM> TanFreetexts { get { return tanFreetexts; } set { SetProperty(ref tanFreetexts, value); } }
        public ListCollectionView TanFreeTextsView { get { return tanFreeTextsView; } set { SetProperty(ref tanFreeTextsView, value); } }
        public string TargetText { get { return targetText; } set { SetProperty(ref targetText, value); } }
        public string ReplaceText { get { return replaceText; } set { SetProperty(ref replaceText, value); } }
        public DelegateCommand Find { get; private set; }
        public DelegateCommand Replace { get; private set; }
        public DelegateCommand Swap { get; private set; }
        public RSNReplaceVM()
        {
            TanFreetexts = new ObservableCollection<FreetextPropertiesVM>();
            Replace = new DelegateCommand(this.DoReplace);
            Find = new DelegateCommand(this.DoFind);
            Swap = new DelegateCommand(this.DoSwap);
            TargetText = string.Empty;
            ReplaceText = string.Empty;
        }

        private void DoSwap(object obj)
        {
            string temp = TargetText;
            TargetText = ReplaceText;
            ReplaceText = temp;
        }

        private bool IsEditAreaValid(out StringBuilder result)
        {
            bool IsValid = true;
            result = new StringBuilder();
            if (string.IsNullOrEmpty(TargetText))
            {
                IsValid = false;
                result.AppendLine("Target Text is Mandatory");
            }
            if (string.IsNullOrEmpty(ReplaceText))
            {
                IsValid = false;
                result.AppendLine("Replace Text is Mandatory");
            }
            return IsValid;
        }

        private void DoFind(object obj)
        {
            if (TanFreeTextsView != null && !string.IsNullOrEmpty(TargetText))
            {
                TanFreeTextsView.Filter = (c) =>
                {
                    if (c != null)
                    {
                        var freetextProperty = c as FreetextPropertiesVM;
                        if (freetextProperty != null)
                        {
                            return String.IsNullOrEmpty(TargetText) ? true : !string.IsNullOrEmpty(freetextProperty.FreeText) && freetextProperty.FreeText.Contains(TargetText);
                        }
                    }
                    return false;
                };
            }
            else
                AppInfoBox.ShowInfoMessage("Data must be required to find");
        }

        private void DoReplace(object obj)
        {
            StringBuilder result = new StringBuilder();
            if (IsEditAreaValid(out result))
            {
                if (obj != null)
                {
                    var but = (System.Windows.Controls.Button)obj;
                    List<Guid> SelectedIds = TanFreetexts.Where(t => t.Selected).Select(t => t.RsnId).ToList();
                    if(but.Name == "ReplaceSelected" && (SelectedIds==null || !SelectedIds.Any()))
                    {
                        AppInfoBox.ShowInfoMessage("Please select items from the grid to replace freetext");
                        return;
                    }
                    if (MessageBox.Show($"Are you sure you want to Replace Freetexts in {(but.Name == "ReplaceSelected" ? "Selected RSNs" : "All Rsns")}", "Rsn Replace Window", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        if ((App.Current.MainWindow) as MainWindow != null && ((App.Current.MainWindow) as MainWindow).DataContext as MainVM != null && (((App.Current.MainWindow) as MainWindow).DataContext as MainVM).TanVM != null)
                        {
                            var mainVM = ((App.Current.MainWindow) as MainWindow).DataContext as MainVM;
                            string ResponceText = string.Empty;
                            if (mainVM.TanVM.ReplaceFreeTexts(TargetText, ReplaceText,mainVM, out ResponceText, but.Name == "ReplaceSelected" ? SelectedIds : null))
                                PrepareData(mainVM.TanVM);
                            else
                                AppInfoBox.ShowInfoMessage(ResponceText);
                        }
                    }
                }
            }
            else
                AppInfoBox.ShowInfoMessage(result.ToString());
        }

        public void PrepareData(TanVM tanVM)
        {
            var rsns = tanVM.Rsns.Where(rsn => !string.IsNullOrEmpty(rsn.FreeText));
            this.TanFreetexts = new ObservableCollection<FreetextPropertiesVM>();
            foreach (RsnVM rsnVM in rsns)
            {
                {
                    var reaction = new FreetextPropertiesVM();
                    reaction.RsnId = rsnVM.Id;
                    reaction.RXNName = rsnVM.Reaction.KeyProductSeq;
                    reaction.StageName = rsnVM.Stage != null ? rsnVM.Stage.Name : string.Empty;
                    reaction.FreeText = rsnVM.FreeText;
                    reaction.Selected = false;
                    this.TanFreetexts.Add(reaction);
                }
            }
            this.TanFreetexts.UpdateDisplayOrder();
            this.TanFreeTextsView = new ListCollectionView(this.TanFreetexts);
        }
    }
    public class FreetextPropertiesVM : OrderableVM
    {
        private string rXNName;
        private string stageName;
        private string freeText;
        private int displayOrder;
        private bool selected;
        private Guid rsnId;

        public Guid RsnId { get { return rsnId; } set { SetProperty(ref rsnId, value); } }
        public string RXNName { get { return rXNName; } set { SetProperty(ref rXNName, value); } }
        public string StageName { get { return stageName; } set { SetProperty(ref stageName, value); } }
        public string FreeText { get { return freeText; } set { SetProperty(ref freeText, value); } }
        public bool Selected { get { return selected; } set { SetProperty(ref selected, value); } }
        //Selected
        public override int DisplayOrder
        {
            get { return displayOrder; }
            set
            {
                SetProperty(ref displayOrder, value);
            }
        }
    }
}
