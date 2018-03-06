using Client.Common;
using Client.Logging;
using Client.Models;
using Client.ViewModels;
using Client.ViewModels.Core;
using DTO;
using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;
using System;
using chemaxon.util;

namespace Client.Views
{
    public partial class PopupWindow : Window
    {
        PopupVM popupViewModel;
        Edit8500Window edit8500Window;
        public bool EditChemicalString;

        public PopupWindow()
        {
            InitializeComponent();
            edit8500Window = new Edit8500Window();
            popupViewModel = new PopupVM();
            this.DataContext = popupViewModel;
            popupViewModel.EditS8500Chemical += PopupPanel_EditS8500Chemical;
            popupViewModel.ChemEditorStuctureChanged += PopupViewModel_ChemEditorStuctureChanged;
            popupViewModel.PerformClear8000EditArea += PopupViewModel_PerformClear8000EditArea;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                base.OnClosing(e);
                e.Cancel = true;
                if (App.Current.MainWindow != null)
                {
                    if ((((MainWindow)(App.Current.MainWindow)).DataContext as MainVM)?.TanVM != null)
                    {
                        (((MainWindow)(App.Current.MainWindow)).DataContext as MainVM).TanVM.EditingParticipant = null;
                    }
                    this.Hide();
                    (App.Current.MainWindow).Focus();
                }

            }
            catch (System.Exception ex)
            {
                Log.This(ex);
            }
        }

        public void show()
        {
            this.Height = 600;
            this.Width = 1000;
            chemEditor.MolfileString = string.Empty;
            this.Show();
        }


        private void ThisWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                if (e.Key == System.Windows.Input.Key.Escape)
                {
                    if ((((MainWindow)(App.Current.MainWindow)).DataContext as MainVM)?.TanVM != null)
                    {
                        (((MainWindow)(App.Current.MainWindow)).DataContext as MainVM).TanVM.EditingParticipant = null;
                    }
                    this.Hide();
                }
            }
            catch (System.Exception ex)
            {
                Log.This(ex);
            }
        }
        private void PopupViewModel_PerformClear8000EditArea(object sender, EventArgs e)
        {
            try
            {
                chemEditor.MolfileString = string.Empty;
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private void PopupViewModel_ChemEditorStuctureChanged(object sender, TanChemicalVM e)
        {
            try
            {
                if (e != null && e.ChemicalType == ChemicalType.S8000)
                {
                    EditChemicalString = true;
                    chemEditor.MolfileString = popupViewModel.MolString;
                    EditChemicalString = false;
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private void PopupPanel_EditS8500Chemical(object sender, EditChemicalName e)
        {
            try
            {
                var edit8500VM = new Edit8500VM();
                edit8500VM.ChemicalNameVM = e.ChemicalNameVM;
                edit8500VM.SelectableChemicalNames = new ObservableCollection<TanChemicalVM>(e.ChoosableChemicalVMs);
                edit8500Window.DataContext = edit8500VM;
                edit8500VM.Edit8500DialogChemicalSelected += Edit8500VM_Edit8500DialogChemicalSelected;
                edit8500Window.ShowDialog();
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private void Edit8500VM_Edit8500DialogChemicalSelected(object sender, TanChemicalVM e)
        {
            try
            {
                if (e != null && popupViewModel != null)
                    popupViewModel.Update8500(e);
                edit8500Window.Hide();
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private void SearchWebBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(popupViewModel.SearchString))
                {
                    string url = popupViewModel.SelectedSite.Url + popupViewModel.SearchString;
                    ((MainWindow)(App.Current.MainWindow)).BrowserUrlChange(popupViewModel.SearchString, url);
                }
                else if (popupViewModel.SelectedTanChemicalVM != null)
                {
                    string url = popupViewModel.SelectedSite.Url + popupViewModel.SelectedTanChemicalVM.Name;
                    ((MainWindow)(App.Current.MainWindow)).BrowserUrlChange(popupViewModel.SearchString, url);
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private void chemEditor_ComStructureChanged()
        {
            try
            {
                string MolFormula = GetMoleculeFormula(chemEditor.MolfileString);
                if (EditChemicalString && !String.IsNullOrEmpty(chemEditor.MolfileString))
                {
                    var inchiKey = MolToInchi.Mol2Inchi(chemEditor.MolfileString);
                    if (!string.IsNullOrEmpty(inchiKey))
                    {
                        var chemical = T.FindByInchiKey(inchiKey);
                        if (chemical == null)
                            chemical = popupViewModel.TanChemicalVMList.Where(cn => cn.InChiKey != null && cn.Id != popupViewModel.SelectedTanChemicalVM.Id && cn.InChiKey == inchiKey).FirstOrDefault();
                        if (chemical != null)
                        {
                            AppInfoBox.ShowInfoMessage(chemical.GetDuplicateChemicalString(inchiKey));
                            chemEditor.MolfileString = string.Empty;
                            return;
                        }
                    }
                }
                if (!String.IsNullOrEmpty(chemEditor.MolfileString) && !EditChemicalString)
                {
                    var inchiKey = MolToInchi.Mol2Inchi(chemEditor.MolfileString);
                    if (!String.IsNullOrEmpty(inchiKey))
                    {
                        var chemical = T.FindByInchiKey(inchiKey);
                        if (chemical == null)
                            chemical = popupViewModel.TanChemicalVMList.Where(cn => cn.InChiKey != null && cn.InChiKey == inchiKey).FirstOrDefault();
                        if (chemical != null)
                        {
                            AppInfoBox.ShowInfoMessage(chemical.GetDuplicateChemicalString(inchiKey));
                            chemEditor.MolfileString = string.Empty;
                            return;
                        }
                    }
                    popupViewModel.MolString = chemEditor.MolfileString;
                    popupViewModel.MolFormula = MolFormula;
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        public static string GetMoleculeFormula(string molstring)
        {
            //string strMolWeight = "";
            string strMolFormula = "";
            try
            {
                MolHandler mHandler = new MolHandler(molstring);
                //float molWeight = mHandler.calcMolWeight();

                //strMolFormula = mHandler.calcMolFormula();
                //strMolWeight = molWeight.ToString();

                //molweight_out = strMolWeight;
                return strMolFormula;
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
            //molweight_out = strMolWeight;
            return strMolFormula;
        }

    }
}
