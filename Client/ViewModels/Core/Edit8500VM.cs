using Client.Command;
using Client.Common;
using Client.Logging;
using Client.Models;
using Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Client.ViewModels.Core
{
    public class Edit8500VM : ViewModelBase
    {
        private TanChemicalVM chemicalNameVM;
        private TanChemicalVM selectedChemicalNameVM;
        private ObservableCollection<TanChemicalVM> selectableChemicalNames;
        private Visibility showSearch;
        private string searchText;
        private ListCollectionView selectedChemicalnamesView;
        public DelegateCommand ApplyEdit { get; private set; }
        public DelegateCommand ShowSearchBar { get; private set; }

        public TanChemicalVM ChemicalNameVM { get { return chemicalNameVM; } set { SetProperty(ref chemicalNameVM, value); } }
        public TanChemicalVM SelectedChemicalNameVM { get { return selectedChemicalNameVM; } set { SetProperty(ref selectedChemicalNameVM, value); } }
        public ObservableCollection<TanChemicalVM> SelectableChemicalNames
        {
            get { return selectableChemicalNames; }
            set
            {
                SetProperty(ref selectableChemicalNames, value);
                SelectableChemicalNamesView = new ListCollectionView(value);
            }
        }
        public ListCollectionView SelectableChemicalNamesView { get { return selectedChemicalnamesView; } set { SetProperty(ref selectedChemicalnamesView, value); } }
        public Visibility ShowSearch { get { return showSearch; } set { SetProperty(ref showSearch, value); } }
        public string SearchText
        {
            get { return searchText; }
            set
            {
                SetProperty(ref searchText, value);
                SelectableChemicalNamesView.Filter = (c) =>
                {
                    if (c != null)
                    {
                        var chemicalName = c as TanChemicalVM;
                        if (chemicalName != null)
                        {
                            return String.IsNullOrEmpty(value) ? true : chemicalName.RegNumber.StartsWith(value) || (chemicalName.Name != null && chemicalName.Name.IndexOf(value, StringComparison.CurrentCultureIgnoreCase) >= 0);
                        }
                    }
                    return false;
                };
            }
        }


        public event EventHandler<TanChemicalVM> Edit8500DialogChemicalSelected = delegate { };
        public Edit8500VM()
        {
            ApplyEdit = new DelegateCommand(DoApplyEdit);
            ShowSearchBar = new DelegateCommand(DoShowSearchBar);
        }

        private void DoShowSearchBar(object obj)
        {
            ShowSearch = ShowSearch == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
        }

        private void DoApplyEdit(object obj)
        {

            try
            {
                Edit8500DialogChemicalSelected.Invoke(this, SelectedChemicalNameVM);
            }
            catch (Exception ex)
            {
                Log.This(ex);

            }
        }
    }
}
