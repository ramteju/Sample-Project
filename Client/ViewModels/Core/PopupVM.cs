using Client.Models;
using Client.Util;
using Client.XML;
using Client.Command;
using DTO;
using ProductTracking.Models.Core;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Client.Common;
using System.Text;
using Entities;
using Client.Views;
using Excelra.Utils.Library;
using Client.Logging;

namespace Client.ViewModels
{
    public class PopupVM : ViewModelBase
    {

        private ChemicalNameComparer chemicalNameComparer = new ChemicalNameComparer();
        private ParticipantType participantType;
        private bool isProductEnable;
        public event EventHandler<EditChemicalName> EditS8500Chemical = delegate { };
        public event EventHandler<TanChemicalVM> ChemEditorStuctureChanged = delegate { };
        public event EventHandler PerformClear8000EditArea = delegate { };

        public PopupVM()
        {
            TanChemicalVMList = new ObservableCollection<TanChemicalVM>();
            SearchSites = new ObservableCollection<SearchSite>()
            {
                new SearchSite {Code="CAS",Url= @"http://www.commonchemistry.org/search.aspx?terms=" },
                new SearchSite {Code="WIKI",Url= @"https://en.wikipedia.org/wiki/" }
            };
            S8000Metas = new ObservableCollection<S8000MetaVM>();
            SelectedChemicalType = ChemicalType.NUM;

            SelectedSite = SearchSites.First();
            DisableAdd();
            (App.Current.MainWindow as MainWindow).PublishNUMAssign += PopupViewModel_PublishNUMAssign;
            (App.Current.MainWindow as MainWindow).SerializedTanLoaded += PopupViewModel_TANLoaded;
            //mainViewModelParticipantTypeChanged
            EditChemical = new Command.DelegateCommand(this.EditTanChemical);
            DeleteChemical = new Command.DelegateCommand(this.DeleteTanChemical);
            AddS8000Chemical = new Command.DelegateCommand(this.Add8000SeriesChemical);
            OpenEditViewCommand = new Command.DelegateCommand(this.OpenEditView);
            AddChemical = new Command.DelegateCommand(this.AddParticipant);
            AddAsParticipant = true;
            SelectedTabIndex = 0;
        }

        private void PopupVM_StageSelectedEvent(object sender, StageVM e)
        {
            if (e != null)
            {
                if (e.DisplayOrder > 1)
                {
                    IsProductEnable = false;
                    ParticipantType = ParticipantType == ParticipantType.Product ? ParticipantType.Reactant : ParticipantType;
                }
                else
                {
                    IsProductEnable = true;
                    ParticipantType = ParticipantType.Product;
                }
            }
        }

        public ParticipantType ParticipantType
        {
            get { return participantType; }
            set
            {
                SetProperty(ref participantType, value);
                var mainVM = (App.Current.MainWindow as MainWindow).DataContext as MainVM;
                mainVM.ParticipantType = value;
            }
        }
        public bool IsProductEnable { get { return isProductEnable; } set { SetProperty(ref isProductEnable, value); } }

        private void DeleteTanChemical(object obj)
        {
            try
            {
                var mainViewModel = (App.Current.MainWindow as MainWindow).DataContext as MainVM;
                var EditedTanChemical = (from c in mainViewModel.TanVM.TanChemicals where c.Id == SelectedTanChemicalVM.Id select c).FirstOrDefault();
                var EditedParticipant = (from p in mainViewModel.TanVM.ReactionParticipants where p.TanChemicalId == SelectedTanChemicalVM.Id select p);
                if (SelectedTanChemicalVM.ChemicalType != ChemicalType.S9000 && SelectedTanChemicalVM.ChemicalType != ChemicalType.NUM && (EditedParticipant == null || !EditedParticipant.Any()))
                {
                    DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("Are you Sure You want to Delete selected Chemical", "Reactions", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (dialogResult == DialogResult.Yes)
                    {
                        mainViewModel.TanVM.TanChemicals.Remove(EditedTanChemical);
                        var EditedChemical = TanChemicalVMList.Where(i => i.Id == SelectedTanChemicalVM.Id).FirstOrDefault();
                        if (EditedChemical.ChemicalType == ChemicalType.S8000)
                            TanChemicalVMList.Remove(EditedChemical);
                        else if (EditedChemical.ChemicalType == ChemicalType.S8500)
                            EditedChemical.NUM = 0;
                        mainViewModel.TanVM.UpdateUnusedNUMs();
                        mainViewModel.TanVM.UpdateNumsView();
                        mainViewModel.TanVM.PerformAutoSave($"{EditedChemical.ChemicalType} chemical deleted");
                    }
                }
                else if (EditedParticipant != null && EditedParticipant.Any())
                {
                    mainViewModel.PrepareInfo(EditedParticipant.FirstOrDefault());
                    mainViewModel.ChemicalUsedPlacesVM.PreviewTabIndex = 0;
                    mainViewModel.CallChemicalUsedPlacesWindow(mainViewModel.ChemicalUsedPlacesVM);
                    //AppInfoBox.ShowInfoMessage($"Selected Chemical already involved in : {string.Join(",", EditedParticipant.Select(ep => ep.ReactionVM.DisplayName).ToList())} as {string.Join(",", EditedParticipant.Select(ep => ep.ParticipantType.ToString()).ToList())} Respectively. If You want to delete Please delete the participant In reaction.");
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private void EditTanChemical(object s)
        {

            try
            {
                var mainViewModel = (App.Current.MainWindow as MainWindow).DataContext as MainVM;
                if (SelectedTanChemicalVM != null)
                {
                    if (SelectedTanChemicalVM.ChemicalType == ChemicalType.S8000)
                    {
                        EditingChemicalId = SelectedTanChemicalVM.Id;
                        SubstanceName = SelectedTanChemicalVM.Name;
                        CompoundNum = SelectedTanChemicalVM.CompoundNo;
                        GenericName = SelectedTanChemicalVM.GenericName;
                        MolString = SelectedTanChemicalVM.MolString;
                        List<S8000MetaVM> MetaList = new List<S8000MetaVM>();
                        foreach (var item in SelectedTanChemicalVM.ChemicalmataData)
                        {
                            var metavm = new S8000MetaVM
                            {
                                Column = item.ColumnNo,
                                Figure = item.Figure,
                                FootNote = item.FootNote,
                                Id = item.Id,
                                Line = item.LineNo,
                                Other = item.OtherNotes,
                                Page = item.PageNo,
                                Para = item.ParaNo,
                                Scheme = item.Scheme,
                                Sheet = item.Sheet,
                                Table = item.TableNo
                            };
                            MetaList.Add(metavm);
                        }
                        S8000Metas = new ObservableCollection<ViewModels.S8000MetaVM>(MetaList);
                        SelectedTabIndex = 1;
                    }
                    else if (SelectedTanChemicalVM.ChemicalType == ChemicalType.S8500)
                    {
                        if (SelectedTanChemicalVM.NUM > 8500)
                        {

                            var s8500Chemicals = new List<TanChemicalVM>();
                            var chemicalName = new TanChemicalVM(SelectedTanChemicalVM);
                            foreach (var e in TanChemicalVMList.Where(c => c.ChemicalType == ChemicalType.S8500 && c.NUM == 0))
                                s8500Chemicals.Add(new TanChemicalVM(e));
                            EditS8500Chemical.Invoke(this, new EditChemicalName { ChemicalNameVM = chemicalName, ChoosableChemicalVMs = s8500Chemicals });
                            var AllNumsList = TanChemicalVMList.Where(tc => tc.NUM > 0).Select(tc => tc.NUM);
                            var duplicateNumsList = AllNumsList.GroupBy(num => num);
                            if (AllNumsList.Count() != duplicateNumsList.Count())
                                Log.This(new Exception("Duplicate Nums were generated"));
                        }
                        else
                            AppInfoBox.ShowInfoMessage("NUM Not Generated For This Chemical . .");
                    }
                    else
                        AppInfoBox.ShowInfoMessage("Only 8500 And 8000 NUMs Are Editable . .");
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        public void Update8500(TanChemicalVM newChemicalNameVM)
        {

            try
            {
                if (SelectedTanChemicalVM != null && SelectedTanChemicalVM.ChemicalType == ChemicalType.S8500 && SelectedTanChemicalVM.NUM > 8000)
                {
                    var newlySelectedTanChemicalVM = TanChemicalVMList.Where(c => c.RegNumber == newChemicalNameVM.RegNumber).FirstOrDefault();
                    if (newlySelectedTanChemicalVM != null)
                    {
                        newlySelectedTanChemicalVM.NUM = SelectedTanChemicalVM.NUM;
                        newlySelectedTanChemicalVM.Id = SelectedTanChemicalVM.Id;

                        SelectedTanChemicalVM.NUM = 0;
                        SelectedTanChemicalVM.Id = Guid.Empty;
                        SelectedTanChemicalVM = null;

                        SelectedTanChemicalVM = newlySelectedTanChemicalVM;
                        UpdateChemicalName(newlySelectedTanChemicalVM);
                        UpdateChemicalsView();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        public void UpdateChemicalName(TanChemicalVM newlySelectedTanChemicalVM)
        {

            try
            {
                var mainViewModel = (App.Current.MainWindow as MainWindow).DataContext as MainVM;
                if (mainViewModel != null && mainViewModel.TanVM != null && mainViewModel.TanVM.TanChemicals != null)
                {
                    var currentTanChemical = mainViewModel.TanVM.TanChemicals.Where(tc => tc.Id == newlySelectedTanChemicalVM.Id).FirstOrDefault();
                    if (currentTanChemical != null)
                    {
                        currentTanChemical.CompoundNo = newlySelectedTanChemicalVM.CompoundNo;
                        currentTanChemical.GenericName = newlySelectedTanChemicalVM.GenericName;
                        currentTanChemical.OtherName = newlySelectedTanChemicalVM.SearchName;
                        currentTanChemical.ImagePath = newlySelectedTanChemicalVM.ImagePath;
                        currentTanChemical.MolString = newlySelectedTanChemicalVM.MolString;
                        currentTanChemical.Name = newlySelectedTanChemicalVM.Name;
                        currentTanChemical.RegNumber = newlySelectedTanChemicalVM.RegNumber;
                        currentTanChemical.MetaData = newlySelectedTanChemicalVM.ChemicalmataData;
                    }
                    var reactionParticipants = mainViewModel.TanVM.ReactionParticipants.OfNum(newlySelectedTanChemicalVM.NUM);
                    if (reactionParticipants != null)
                        foreach (var participant in reactionParticipants)
                        {
                            participant.Name = newlySelectedTanChemicalVM.Name;
                            participant.Reg = newlySelectedTanChemicalVM.RegNumber;
                        }
                    mainViewModel.TanVM.PerformAutoSave($"{newlySelectedTanChemicalVM.ChemicalType.ToString()} Chemical Updated");
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private void Add8000SeriesChemical(object s)
        {

            try
            {
                if (!string.IsNullOrEmpty(MolString))
                {
                    if (S8000Metas != null && S8000Metas.Count > 0 && S8000Metas.Where(meta => !meta.ValidClass()).Count() == 0 &&
                        (!string.IsNullOrEmpty(SubstanceName) || !string.IsNullOrEmpty(CompoundNum) || !string.IsNullOrEmpty(GenericName)))
                    {
                        if ((EditingChemicalId == Guid.Empty && !string.IsNullOrEmpty(SubstanceName) && (from cn in TanChemicalVMList where cn.Name.SafeEqualsLower(SubstanceName) select cn).Any()))
                        {
                            AppInfoBox.ShowInfoMessage("Substance name Already exist in Chemical Names List");
                            return;
                        }
                        if ((EditingChemicalId == Guid.Empty && !string.IsNullOrEmpty(GenericName) && (from cn in TanChemicalVMList where cn.GenericName.SafeEqualsLower(GenericName) select cn).Any()))
                        {
                            AppInfoBox.ShowInfoMessage("GenericName Already exist in Chemical Names List");
                            return;
                        }
                        if ((EditingChemicalId == Guid.Empty && !string.IsNullOrEmpty(CompoundNum) && (from cn in TanChemicalVMList where cn.GenericName.SafeEqualsLower(CompoundNum) select cn).Any()))
                        {
                            AppInfoBox.ShowInfoMessage("CompoundNum Already exist in Chemical Names List");
                            return;
                        }
                        var inchiKey = MolToInchi.Mol2Inchi(MolString);
                        if (!String.IsNullOrEmpty(inchiKey))
                        {
                            var chemical = T.FindByInchiKey(inchiKey);
                            if (chemical != null)
                            {
                                AppInfoBox.ShowInfoMessage(chemical.GetDuplicateChemicalString(inchiKey));
                                return;
                            }
                            chemical = TanChemicalVMList.Where(cn => cn.InChiKey != null && cn.InChiKey == inchiKey).FirstOrDefault();
                            if (chemical != null && EditingChemicalId == Guid.Empty)
                            {
                                AppInfoBox.ShowInfoMessage(chemical.GetDuplicateChemicalString(inchiKey));
                                return;
                            }
                        }
                        var mainViewModel = (App.Current.MainWindow as MainWindow).DataContext as MainVM;
                        var MetaDataList = new List<TanChemicalMetaData>();
                        foreach (var item in S8000Metas)
                        {
                            var tanChemicalMeta = new TanChemicalMetaData
                            {
                                Id = Guid.NewGuid(),
                                ColumnNo = item.Column,
                                Figure = item.Figure,
                                FootNote = item.FootNote,
                                LineNo = item.Line,
                                OtherNotes = item.Other,
                                PageNo = item.Page,
                                ParaNo = item.Para,
                                Scheme = item.Scheme,
                                Sheet = item.Sheet,
                                TableNo = item.Table
                            };
                            MetaDataList.Add(tanChemicalMeta);
                        }
                        if (EditingChemicalId != Guid.Empty)
                        {
                            var EditedChemical = TanChemicalVMList.Where(i => i.Id == EditingChemicalId).FirstOrDefault();
                            if (EditedChemical != null)
                            {
                                EditedChemical.Name = SubstanceName;
                                EditedChemical.SearchName = string.IsNullOrEmpty(SubstanceName) ? (string.IsNullOrEmpty(GenericName) ? CompoundNum : GenericName) : SubstanceName;
                                EditedChemical.ChemicalType = ChemicalType.S8000;
                                EditedChemical.CompoundNo = CompoundNum;
                                EditedChemical.GenericName = GenericName;
                                EditedChemical.StructureType = StructureType.CGM;
                                EditedChemical.ChemicalmataData = MetaDataList;
                                EditedChemical.MolString = MolString;
                                EditedChemical.MolFormula = MolFormula;
                                EditedChemical.InChiKey = inchiKey;
                            }
                            UpdateChemicalName(EditedChemical);
                            SelectedTanChemicalVM = null;
                            SelectedTanChemicalVM = EditedChemical;
                            mainViewModel.TanVM.UpdateUnusedNUMs();
                            mainViewModel.TanVM.UpdateNumsView();
                            Clear8000EditArea();
                            SelectedTabIndex = 0;
                        }
                        else
                        {
                            var newChemical = new TanChemicalVM
                            {
                                Id = Guid.NewGuid(),
                                Name = SubstanceName,
                                SearchName = SubstanceName,
                                ChemicalType = ChemicalType.S8000,
                                CompoundNo = CompoundNum,
                                GenericName = GenericName,
                                StructureType = StructureType.CGM,
                                ChemicalmataData = MetaDataList,
                                MolString = MolString,
                                MolFormula = MolFormula,
                                InChiKey = inchiKey
                            };
                            foreach (var item in MetaDataList)
                                item.TanChemicalId = newChemical.Id;
                            Add8000Chemical(newChemical);
                        }
                        EditingChemicalId = Guid.Empty;
                    }
                    else
                        AppInfoBox.ShowInfoMessage("New Chemical data missed. To save new chemical SubstanceName/CompoundNum/GenericName and PageNo are mandatory.");
                }
                else
                    AppInfoBox.ShowInfoMessage("Please draw structure to add");
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }


        private void OpenEditView(object s)
        {
            try
            {
                SelectedTabIndex = 1;
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private void PopupViewModel_TANLoaded(object sender, Tan tan)
        {

            try
            {
                var mainViewModel = (App.Current.MainWindow as MainWindow).DataContext as MainVM;
                var startTime = DateTime.Now;
                var list = new List<TanChemicalVM>();
                T.ResetTanChemicalDict();
                foreach (var tanChemical in tan.TanChemicals)
                {
                    string imagePath = null;
                    if (tanChemical.Substancepaths != null && tanChemical.Substancepaths.Count > 0)
                        imagePath = tanChemical.Substancepaths.First().ImagePath;
                    var chemical = new TanChemicalVM
                    {
                        ChemicalType = tanChemical.ChemicalType,
                        Id = tanChemical.Id,
                        ImagePath = imagePath,
                        Name = tanChemical.Name,
                        NUM = tanChemical.NUM,
                        RegNumber = tanChemical.RegNumber,
                        SearchName = tanChemical.Name,
                        CompoundNo = tanChemical.CompoundNo,
                        GenericName = tanChemical.GenericName,
                        StructureType = StructureType.MOL,
                        MolString = tanChemical.MolString,
                        MolFormula = tanChemical.Formula,
                        AllImagePaths = tanChemical.Substancepaths.Select(s => s.ImagePath).Distinct().ToList(),
                        ChemicalmataData = tanChemical.MetaData != null ? new List<TanChemicalMetaData>(tanChemical.MetaData) : null,
                        InChiKey = tanChemical.InchiKey
                    };
                    list.Add(chemical);
                }
                var regNumbersOfTanChemicals = tan.TanChemicals.Where(c => c.ChemicalType != ChemicalType.NUM).Select(c => c.RegNumber).ToList();
                var chemicalDict = S.ChemicalDict;
                var dictOfchemicalsExceptTanNums = from storeChmical in chemicalDict where !regNumbersOfTanChemicals.Contains(storeChmical.Key) select storeChmical;

                var S8500ChemicalsFrom = dictOfchemicalsExceptTanNums.Select(s => s.Value).Where(tc => tc.ChemicalType == ChemicalType.S8500 && tc.NUM > 0);
                if (S8500ChemicalsFrom.Any())
                {
                    AppInfoBox.ShowInfoMessage("Static store chemical has updated");
                }
                var participants = tan.TanChemicals.Where(p => p.ChemicalType == ChemicalType.S8000 && tan.Participants.Select(rp => rp.ParticipantId).ToList().Contains(p.Id)).Select(p => p.Id).ToList();
                foreach (var chemical in list)
                {
                    if (chemical.ChemicalType != ChemicalType.S8000)
                    {
                        var OrgRefChemical = S.Find(chemical.RegNumber);
                        if (!string.IsNullOrEmpty(chemical.RegNumber) && OrgRefChemical != null && chemical.ChemicalType != ChemicalType.NUM)
                            chemical.SearchName = OrgRefChemical.SearchName;
                    }
                }

                foreach (var entry in dictOfchemicalsExceptTanNums)
                {
                    var tanChemicalVM = new TanChemicalVM
                    {
                        AllImagePaths = entry.Value.AllImagePaths,
                        ChemicalmataData = entry.Value.ChemicalmataData,
                        ChemicalType = entry.Value.ChemicalType,
                        CompoundNo = entry.Value.CompoundNo,
                        GenericName = entry.Value.GenericName,
                        Id = entry.Value.Id,
                        ImagePath = entry.Value.ImagePath,
                        Images = entry.Value.Images,
                        InChiKey = entry.Value.InChiKey,
                        MolString = entry.Value.MolString,
                        Name = entry.Value.Name,
                        NUM = entry.Value.NUM,
                        RegNumber = entry.Value.RegNumber,
                        SearchName = entry.Value.SearchName,
                        StructureType = entry.Value.StructureType,
                        MolFormula = entry.Value.MolFormula
                    };
                    list.Add(tanChemicalVM);
                }

                TanChemicalVMList = new ObservableCollection<TanChemicalVM>(list);
                UpdateChemicalsView();
                mainViewModel.S8000Chemicals = new ObservableCollection<Models.TanChemicalVM>(list.Where(chem => chem.ChemicalType == ChemicalType.S8000 && participants.Contains(chem.Id)).ToList());
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private void PopupViewModel_PublishNUMAssign(object sender, TanChemicalVM e)
        {

            try
            {
                var sameRegChemical = TanChemicalVMList.Where(c => c.RegNumber == e.RegNumber).FirstOrDefault();
                if (sameRegChemical != null)
                    sameRegChemical.NUM = e.NUM;
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private ObservableCollection<TanChemicalVM> chemicalNames;
        private ListCollectionView chemicalNamesView;
        private TanChemicalVM selectedChemicalName;
        private BitmapImage imageSource;
        private String searchString;
        private ObservableCollection<SearchSite> searchSites;
        private SearchSite selectedSite;
        private Visibility enableAddButton;
        private ObservableCollection<S8000MetaVM> s8000MetaVm;
        private ChemicalType selectedChemicalType;
        private int selectedTabIndex;
        private string molString;
        private Guid editingChemicalId;
        private bool addAsParticipant;

        #region 8000 series tab
        private string substanceName, compoundNum, genericName;
        private string molFormula;
        private string selectedMolFormula;

        public String SubstanceName { get { return substanceName; } set { SetProperty(ref substanceName, value); } }
        public Guid EditingChemicalId { get { return editingChemicalId; } set { SetProperty(ref editingChemicalId, value); } }
        public String CompoundNum { get { return compoundNum; } set { SetProperty(ref compoundNum, value); } }
        public String GenericName { get { return genericName; } set { SetProperty(ref genericName, value); } }
        public String MolString
        {
            get { return molString; }
            set
            {
                SetProperty(ref molString, value);
                ChemEditorStuctureChanged.Invoke(this, SelectedTanChemicalVM);
            }
        }
        public String MolFormula { get { return molFormula; } set { SetProperty(ref molFormula, value); } }

        public int SelectedTabIndex
        {
            get { return selectedTabIndex; }
            set
            {
                SetProperty(ref selectedTabIndex, value);
            }
        }
        #endregion

        public ObservableCollection<TanChemicalVM> TanChemicalVMList
        {
            get { return chemicalNames; }
            set
            {
                SetProperty(ref chemicalNames, value);
                TanChemicalListView = new ListCollectionView(TanChemicalVMList);
                TanChemicalListView.CustomSort = chemicalNameComparer;
                TanChemicalListView.Refresh();
            }
        }
        public ListCollectionView TanChemicalListView { get { return chemicalNamesView; } set { SetProperty(ref chemicalNamesView, value); } }
        public BitmapImage ChemicalBitMapImage { get { return imageSource; } set { SetProperty(ref imageSource, value); } }
        public string SelectedMolFormula { get { return selectedMolFormula; } set { SetProperty(ref selectedMolFormula, value); } }
        public Visibility EnableAddButton { get { return enableAddButton; } set { SetProperty(ref enableAddButton, value); } }
        public String SearchString
        {
            get { return searchString; }
            set
            {
                SetProperty(ref searchString, value);
                UpdateChemicalsView();
            }
        }
        public ObservableCollection<SearchSite> SearchSites { get { return searchSites; } set { SetProperty(ref searchSites, value); } }
        public ObservableCollection<S8000MetaVM> S8000Metas { get { return s8000MetaVm; } set { SetProperty(ref s8000MetaVm, value); } }
        public SearchSite SelectedSite { get { return selectedSite; } set { SetProperty(ref selectedSite, value); } }
        public bool AddAsParticipant { get { return addAsParticipant; } set { SetProperty(ref addAsParticipant, value); } }
        public ChemicalType SelectedChemicalType
        {
            get { return selectedChemicalType; }
            set
            {
                SetProperty(ref selectedChemicalType, value);
                if (SelectedChemicalType == ChemicalType.S8500)
                    AddAsParticipant = false;
                else
                    AddAsParticipant = true;
                UpdateChemicalsView();
            }
        }

        public TanChemicalVM SelectedTanChemicalVM
        {
            get { return selectedChemicalName; }
            set
            {
                SetProperty(ref selectedChemicalName, value);
                ChemicalBitMapImage = null;
                if (value != null)
                {
                    ChemicalBitMapImage = value.BigImage;
                    SelectedMolFormula = value.MolFormula;
                }
            }
        }


        void EnableAdd()
        {
            EnableAddButton = Visibility.Visible;
        }
        void DisableAdd()
        {
            EnableAddButton = Visibility.Hidden;
        }

        void UpdateChemicalsView()
        {

            try
            {
                TanChemicalListView.Filter = (c) =>
                    {
                        if (c != null)
                        {
                            var chemicalName = c as TanChemicalVM;
                            if (chemicalName != null)
                            {
                                return
                            (chemicalName.ChemicalType == SelectedChemicalType)
                            &&
                            (
                            String.IsNullOrEmpty(SearchString) ? true :
                            chemicalName.NUM.ToString().StartsWith(SearchString) ||
                                   chemicalName.RegNumber.StartsWith(SearchString) ||
                                    (chemicalName.SearchName != null && chemicalName.SearchName.IndexOf(SearchString, StringComparison.CurrentCultureIgnoreCase) >= 0)
                            );
                            }
                        }
                        return false;
                    };
                //ChemicalNamesView.CustomSort = chemicalNameComparer;
                TanChemicalListView.Refresh();
                if (TanChemicalListView.Count == 0)
                    EnableAdd();
                else
                    DisableAdd();
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        public bool VerifyNewStructure(string inchiKey)
        {

            try
            {
                return TanChemicalVMList.Where(c => c.InChiKey == inchiKey).Any();
            }
            catch (Exception ex)
            {
                Log.This(ex);
                return false;
            }
        }

        public void AddParticipant(object participant)
        {
            try
            {
                var StartTime = DateTime.Now;
                Debug.WriteLine($"Add Participant Started at {StartTime.DMYHMT()}");
                var mainViewModel = (App.Current.MainWindow as MainWindow).DataContext as MainVM;
                if (SelectedTanChemicalVM != null && mainViewModel.TanVM != null && mainViewModel.IsParticipatTypeSelected)
                {
                    if (S.AllowedDuplicateChemicals.Where(c => SelectedTanChemicalVM.SearchName.Contains(c)).Count() > 0)
                    {
                        var ListOfNames = SelectedTanChemicalVM.SearchName.Split(new string[] { "; " }, StringSplitOptions.RemoveEmptyEntries);
                        DuplicateNamesList dnList = new Views.DuplicateNamesList();
                        (dnList.DataContext as DuplicateNamesVM).DuplicateNamesView = new ObservableCollection<Names>(ListOfNames.Select(s => new Names { Name = s }));
                        dnList.ShowDialog();
                        if ((dnList.DataContext as DuplicateNamesVM).DialogStatus)
                            SelectedTanChemicalVM.Name = (dnList.DataContext as DuplicateNamesVM).SelectedName.Name;
                        else
                            return;
                    }
                    if (SelectedTanChemicalVM.RegNumber == "5137553")
                    {
                        AppInfoBox.ShowInfoMessage("You can't add Aliquat 336. If still you want please Add RSN as 'Aliquat 336 Used'");
                        return;
                    }
                    if (SelectedTanChemicalVM.RegNumber == "69849452")
                    {
                        AppInfoBox.ShowInfoMessage("Davis reagent (REGNUM: 69849452) cannot be used in RSD, please capture in RSN");
                        return;
                    }
                    //this is required only for 8500 series. All other series will have Num values.
                    if (SelectedTanChemicalVM != null && SelectedTanChemicalVM.NUM == 0)
                    {
                        var numChemical = TanChemicalVMList.Where(cn => cn.ChemicalType == ChemicalType.NUM && cn.RegNumber == SelectedTanChemicalVM.RegNumber).FirstOrDefault();
                        if (numChemical != null)
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.Append($"Selected chemical already Present in NUMS section with num {numChemical.NUM}");
                            AppInfoBox.ShowInfoMessage(sb.ToString());
                            return;
                        }
                        SelectedTanChemicalVM.Id = Guid.NewGuid();
                        var maxNum = TanChemicalVMList.
                            Where(rp => rp.ChemicalType == ChemicalType.S8500).
                            Select(rp => rp.NUM).
                            Max();
                        if (maxNum > 0)
                            SelectedTanChemicalVM.NUM = maxNum + 1;
                        else
                            SelectedTanChemicalVM.NUM = 8501;
                        TanChemical tanchemical = (from p in mainViewModel.TanVM.TanChemicals where p.RegNumber == SelectedTanChemicalVM.RegNumber select p).FirstOrDefault();
                        if (tanchemical == null)
                            TanChemicalsCrud.AddTanChemicalToList(mainViewModel.TanVM.TanChemicals, SelectedTanChemicalVM, mainViewModel.TanVM.Id);
                        if (!AddAsParticipant)
                            mainViewModel.TanVM.PerformAutoSave("8500 Chemical Added");
                    }
                    var chemical = TanChemicalVMList.Where(cn => cn.ChemicalType == ChemicalType.NUM && cn.RegNumber == SelectedTanChemicalVM.RegNumber && SelectedTanChemicalVM.ChemicalType != ChemicalType.NUM).FirstOrDefault();
                    if (chemical != null)
                        SelectedTanChemicalVM = chemical;
                    if (SelectedTanChemicalVM.RegNumber == "30525894" && SelectedTanChemicalVM.ChemicalType == ChemicalType.NUM)
                    {
                        SelectedTanChemicalVM = TanChemicalVMList.Where(cn => cn.ChemicalType == ChemicalType.S9000 && cn.RegNumber == "50000").FirstOrDefault();
                        SelectedTanChemicalVM.Name = S.PARA_FORM;
                    }
                    Debug.WriteLine($"Before adding Participant to Json {System.DateTime.Now}");
                    if (SelectedTanChemicalVM != null && AddAsParticipant)
                        (App.Current.MainWindow as MainWindow).ChemicalNameAdded(SelectedTanChemicalVM);
                    ReactionValidations.AddJonesReAgentRSN(mainViewModel.TanVM);
                    Debug.WriteLine($"After adding Participant to Json {System.DateTime.Now}");
                    Debug.WriteLine($"Add Participant Done in {(DateTime.Now - StartTime).TotalSeconds} seconds");
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        public DelegateCommand EditChemical { get; private set; }
        public DelegateCommand DeleteChemical { get; private set; }
        public DelegateCommand AddS8000Chemical { get; private set; }
        public DelegateCommand OpenEditViewCommand { get; private set; }
        public DelegateCommand AddChemical { get; private set; }
        public void Add8000Chemical(TanChemicalVM newChemical)
        {
            try
            {
                var mainViewModel = (App.Current.MainWindow as MainWindow).DataContext as MainVM;
                if (TanChemicalVMList != null)
                {
                    //generate new 8000 number
                    int seriesNumber = 8001;
                    var maxSeriesNumChecmical = TanChemicalVMList.
                        Where(rp => rp.ChemicalType == ChemicalType.S8000)
                        .OrderBy(rp => rp.NUM).LastOrDefault();
                    if (maxSeriesNumChecmical != null && maxSeriesNumChecmical.NUM < 8499)
                        seriesNumber = maxSeriesNumChecmical.NUM + 1;
                    else if (maxSeriesNumChecmical != null && maxSeriesNumChecmical.NUM >= 8499)
                    {
                        seriesNumber = 7001;
                        var _7000SeriesNumChecmical = TanChemicalVMList.
                        Where(rp => rp.ChemicalType == ChemicalType.S8000 && rp.NUM < 8000)
                        .OrderBy(rp => rp.NUM).LastOrDefault();
                        if (_7000SeriesNumChecmical != null && _7000SeriesNumChecmical.NUM < 8500)
                            seriesNumber = _7000SeriesNumChecmical.NUM + 1;
                    }

                    TanChemicalVM c = new TanChemicalVM();
                    c.ChemicalType = ChemicalType.S8000;
                    c.RegNumber = String.Empty;
                    c.SearchName = string.IsNullOrEmpty(newChemical.Name) ? (string.IsNullOrEmpty(newChemical.GenericName) ? newChemical.CompoundNo : newChemical.GenericName) : newChemical.Name;
                    c.Name = string.IsNullOrEmpty(newChemical.Name) ? (string.IsNullOrEmpty(newChemical.GenericName) ? newChemical.CompoundNo : newChemical.GenericName) : newChemical.Name;
                    c.NUM = seriesNumber;
                    c.InChiKey = newChemical.InChiKey;
                    c.ChemicalmataData = newChemical.ChemicalmataData;
                    c.CompoundNo = newChemical.CompoundNo;
                    c.GenericName = newChemical.GenericName;
                    c.Id = newChemical.Id;
                    c.ChemicalmataData = newChemical.ChemicalmataData;
                    c.MolString = newChemical.MolString;
                    c.MolFormula = newChemical.MolFormula;
                    TanChemicalVMList.Add(c);
                    mainViewModel.S8000Chemicals.Add(c);
                    TanChemical tanchemical = (from p in mainViewModel.TanVM.TanChemicals where p.Id == newChemical.Id select p).FirstOrDefault();
                    if (tanchemical == null)
                        TanChemicalsCrud.AddTanChemicalToList(mainViewModel.TanVM.TanChemicals, c, mainViewModel.TanVM.Id);
                    mainViewModel.TanVM.PerformAutoSave("8000 chemical generated");
                    Clear8000EditArea();
                    SelectedTabIndex = 0;
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private void Clear8000EditArea()
        {

            try
            {
                GenericName = "";
                CompoundNum = "";
                SubstanceName = "";
                MolString = "";
                EditingChemicalId = Guid.Empty;
                S8000Metas.Clear();
                PerformClear8000EditArea.Invoke(this, null);
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }
    }
    public class S8000MetaVM : ViewModelBase
    {
        private Guid id;
        private string page;
        private int line, para, column, table, figure, sheet;
        private string scheme, footNote, other;

        public Guid Id { get { return id; } set { SetProperty(ref id, value); } }
        public string Page { get { return page; } set { SetProperty(ref page, value); } }
        public int Line { get { return line; } set { SetProperty(ref line, value); } }
        public int Para { get { return para; } set { SetProperty(ref para, value); } }
        public int Column { get { return column; } set { SetProperty(ref column, value); } }
        public int Table { get { return table; } set { SetProperty(ref table, value); } }
        public int Figure { get { return figure; } set { SetProperty(ref figure, value); } }
        public string Scheme { get { return scheme; } set { SetProperty(ref scheme, value); } }
        public int Sheet { get { return sheet; } set { SetProperty(ref sheet, value); } }
        public String FootNote { get { return footNote; } set { SetProperty(ref footNote, value); } }
        public String Other { get { return other; } set { SetProperty(ref other, value); } }

        public bool ValidClass()
        {
            bool result = true;
            if (String.IsNullOrEmpty(Page))
                result = false;
            return result;
        }

    }

    public class SearchSite : ViewModelBase
    {
        public String Code { get; set; }
        public String Url { get; set; }
    }
}
