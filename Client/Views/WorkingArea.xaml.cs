using Client.ViewModels;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Telerik.Windows.Controls;
using System.IO;
using ProductTracking.Models.Core;
using System.Drawing;
using System.Collections.Generic;
using Client.ViewModels.Core;
using System.Text;
using Client.Logging;
using Client.ViewModels.Extended;
using Excelra.Utils.Library;
using Client.Views.Pdf;
using AxFoxitPDFSDKProLib;
using Client.Common;
using Client.Notify;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace Client.Views
{
    public partial class WorkingArea : UserControl
    {
        RSNDialog rsnWindow;
        Conditions conditions;
        PopupWindow popupWindow;
        TemperatureCodeVM temperatureVM;
        TimeComboCodevalues timeComboCodevalues;
        TimeCombovalues timeCombovalues;
        PressureComboValues pressureComboValues;
        ConditionVM conditionVM;
        ChemicalUsedPlacesWindow chemicalUsedPlacesWindow;
        bool SaveOnAnnoteDeleted = true;
        public WorkingArea()
        {
            InitializeComponent();
            try
            {
                popupWindow = new Views.PopupWindow();
                rsnWindow = new RSNDialog();
                conditions = new Conditions();
                conditionVM = new ConditionVM();
                (App.Current.MainWindow as MainWindow).MasterTanLoaded += MasterTanLoaded;
                ((App.Current.MainWindow as MainWindow).DataContext as MainVM).BaforeTanSubmiting += WorkingArea_BaforeTanSubmiting;
                (App.Current.MainWindow as MainWindow).TANSaved += WorkingArea_TANSaved;
                ((App.Current.MainWindow as MainWindow).DataContext as MainVM).TanClosed += WorkingArea_TanClosed;
                (App.Current.MainWindow as MainWindow).TanClosed += WorkingArea_TanClosed;
                ((App.Current.MainWindow as MainWindow).DataContext as MainVM).RowDoubleClicked += WorkingArea_RowDoubleClicked;
                ((App.Current.MainWindow as MainWindow).DataContext as MainVM).ChooseClicked += WorkingArea_ChooseClicked;
                ((App.Current.MainWindow as MainWindow).DataContext as MainVM).ShowNumUsedInfoClicked += WorkingArea_ShowNumUsedInfoClicked;
                (App.Current.MainWindow as MainWindow).PreviewKeyDown += WorkingArea_PreviewKeyDown;

                #region Pdf
                var foxit = PdfHost.Child as AxFoxitPDFSDK;
                if (foxit != null)
                {
                    foxit.UnLockActiveX(C.LICENCE_ID, C.UNLOCK_CODE);
                    foxit.OnDbClick += Foxit_OnDoubleClick;
                    foxit.OnAnnotLButtonDown += Foxit_OnAnnotLButtonDown;
                    foxit.OnAnnotDeleted += Foxit_OnAnnotDeleted;
                    foxit.ShowTitleBar(false);
                    foxit.ShowToolBar(false);
                    foxit.ShowNavigationPanels(true);
                    foxit.SetShowSavePrompt(false, 0);
                    PdfSearch.foxit = foxit;
                }
                PdfAnnotations.AnnotationSelected += PdfAnnotations_AnnotationSelected;
                #endregion
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }
        private void Foxit_OnAnnotLButtonDown(object sender, _DFoxitPDFSDKEvents_OnAnnotLButtonDownEvent e)
        {
            e.bDefault = false;
            AppInfoBox.ShowInfoMessage("Annotation Can't Be Editied");
        }

        private void Foxit_OnAnnotDeleted(object sender, _DFoxitPDFSDKEvents_OnAnnotDeletedEvent e)
        {
            var foxit = PdfHost.Child as AxFoxitPDFSDK;
            if (foxit != null && SaveOnAnnoteDeleted)
                foxit.Save();
        }

        private void PdfAnnotations_AnnotationSelected(object sender, ViewModels.Pdf.PdfAnnotationResultVM e)
        {
            var foxit = PdfHost.Child as AxFoxitPDFSDK;
            if (foxit != null)
                foxit.GoToPage(e.PageNum - 1);
        }
        private void WorkingArea_ShowNumUsedInfoClicked(object sender, ViewModels.Extended.ChemicalUsedPlacesVM e)
        {
            try
            {
                if (chemicalUsedPlacesWindow == null)
                    chemicalUsedPlacesWindow = new Views.ChemicalUsedPlacesWindow();
                chemicalUsedPlacesWindow.DataContext = e;
                chemicalUsedPlacesWindow.Show();
                chemicalUsedPlacesWindow.Activate();
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private void Foxit_OnDoubleClick(object sender, AxFoxitPDFSDKProLib._DFoxitPDFSDKEvents_OnDbClickEvent e)
        {
            var startTime = DateTime.Now;
            Debug.WriteLine($"Before tan submitting method started at {startTime}");
            try
            {
                var mainVM = ((App.Current.MainWindow as MainWindow).DataContext as MainVM);
                List<string> RXNMarkings = new List<string>();
                int RXNAnnotsCount = 0;
                var foxit = PdfHost.Child as AxFoxitPDFSDK;
                if (foxit != null && mainVM != null && mainVM.TanVM != null)
                    if ((ListPdfDocuments.SelectedItem as TanDocumentsVM).KeyPath)
                    {
                        var rxn = mainVM.TanVM.SelectedReaction;
                        if (rxn != null)
                        {
                            string contents = string.Empty;
                            if (mainVM.AddS8000Marking)
                            {
                                if (mainVM.Selected8000Chemical != null)
                                    contents = mainVM.Selected8000Chemical.NUM.ToString();
                                else
                                    AppInfoBox.ShowInfoMessage("Please select 8000 num From the Bottom list");
                            }
                            else
                                contents = $"{C.RXN_MRK_TEXT} {rxn.KeyProductSeq}";
                            for (int i = 0; i < foxit.PageCount; i++)
                            {
                                var annots = foxit.GetPageAnnots(i);
                                for (int j = 0; j < annots.GetAnnotsCount(); j++)
                                {
                                    var content = annots.GetAnnot(j).GetContents();
                                    if (content != null && content.ToString().Equals(contents) && string.IsNullOrEmpty(annots.GetAnnot(j).Author))
                                    {
                                        RXNAnnotsCount += 1;
                                        RXNMarkings.Add($"{content.ToString()} (Page - {i + 1})");
                                    }
                                }
                            }
                            if (RXNAnnotsCount == 0)
                            {
                                var annotations = foxit.GetPageAnnots(foxit.CurPage);
                                float left = 0, bottom = 0;
                                var pageIndex = foxit.CurPage;
                                if (foxit.ConvertClientCoordToPageCoord(e.clientX, e.clientY, ref pageIndex, ref left, ref bottom))
                                {
                                    var annotation = annotations.AddAnnot(null, C.ANNOT_TYPE, left, bottom + 10, left + 100, bottom);
                                    if (annotation != null)
                                    {
                                        var formatTool = foxit.GetFormatTool();
                                        formatTool.SetFontSize(C.ANNOT_FONT_SIZE);
                                        formatTool.SetFontName(C.ANNOT_FONT_NAME);
                                        annotation.Color = (uint)ColorTranslator.ToOle(Color.Transparent);
                                        annotation.FillColor = (uint)ColorTranslator.ToOle(Color.Transparent);
                                        annotation.Author = string.Empty;
                                        formatTool.SetFontColor((uint)ColorTranslator.ToOle(C.ANNOT_FONT_COLOR));
                                        annotation.SetContents(contents);
                                        //annotation.Locked = true;
                                        foxit.CurrentTool = C.HAND_TOOL;
                                        foxit.Save();
                                    }
                                }
                            }
                            else
                            {
                                StringBuilder sb = new StringBuilder();
                                sb.Append($"Selected Annoation {contents} already marked in Document at {RXNMarkings.FirstOrDefault()}");
                                AppInfoBox.ShowInfoMessage(sb.ToString());
                            }
                        }
                    }
                    else
                        AppInfoBox.ShowInfoMessage("You Can't add Annotation for Support Documents. Please Select First Document from the left side Files List");
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
            var endTime = DateTime.Now;
            Debug.WriteLine($"Before tan submitting method started at {startTime} and end at {endTime}. Total time taken to finish: {(endTime - startTime).TotalSeconds} seconds");
        }

        private void WorkingArea_BaforeTanSubmiting(object sender, Common.Action action)
        {
            try
            {
                var startTime = DateTime.Now;
                Debug.WriteLine("Before Tan Submitting strated");
                var mainVM = ((App.Current.MainWindow) as MainWindow).DataContext as MainVM;
                List<string> S8000Markings = new List<string>();
                List<string> S8000MarkingsWithoutPageInfo = new List<string>();
                List<string> RXNMarkings = new List<string>();
                List<string> RXNMarkingsWithoutPageInfo = new List<string>();
                int RXNAnnotsCount = 0;
                int S8000AnnotsCount = 0;
                var foxit = PdfHost.Child as AxFoxitPDFSDK;
                if (ListPdfDocuments != null && ListPdfDocuments.Items != null)
                {
                    foreach (var item in ListPdfDocuments.Items)
                    {
                        if ((item as TanDocumentsVM).KeyPath)
                        {
                            ListPdfDocuments.SelectedItem = item;
                            ListPdfDocuments_SelectionChanged(null, null);
                            break;
                        }
                    }
                }
                if (!string.IsNullOrEmpty(foxit.FilePath))
                {
                    foxit.Save();
                    for (int i = 0; i < foxit.PageCount; i++)
                    {
                        List<FoxitPDFSDKProLib.PDFAnnot> annotsToRemove = new List<FoxitPDFSDKProLib.PDFAnnot>();

                        var annots = foxit.GetPageAnnots(i);
                        int annotsCount = annots.GetAnnotsCount();
                        for (int j = 0; j < annotsCount; j++)
                        {
                            if (annots.GetAnnot(j).GetContents() != null && string.IsNullOrEmpty(annots.GetAnnot(j).GetContents().ToString()))
                                annotsToRemove.Add(annots.GetAnnot(j));
                        }
                        SaveOnAnnoteDeleted = false;
                        foreach (var ann in annotsToRemove)
                            annots.RemoveAnnot(ann);
                        SaveOnAnnoteDeleted = true;

                        for (int j = 0; j < annots.GetAnnotsCount(); j++)
                        {
                            var content = annots.GetAnnot(j).GetContents();
                            if (content != null)
                            {
                                string text = content.ToString();
                                if (text.Contains(C.RXN_MRK_TEXT) && Regex.IsMatch(text, S.RXN_MARKUP_REG_EXP) && string.IsNullOrEmpty(annots.GetAnnot(j).Author))
                                {
                                    RXNAnnotsCount += 1;
                                    RXNMarkings.Add($"{text} (Page - {i + 1})");
                                    RXNMarkingsWithoutPageInfo.Add(text);
                                }
                                if (text.Length == 4 && Regex.IsMatch(text, S.S8000_MARKUP_REG_EXP))
                                {
                                    S8000AnnotsCount += 1;
                                    S8000Markings.Add($"{text} (Page - {i + 1})");
                                    S8000MarkingsWithoutPageInfo.Add($"{text}");
                                }
                            }

                        }
                    }
                    List<string> ReactionList = (mainVM != null && mainVM.TanVM != null) ? mainVM.TanVM.Reactions.Select(r => $"rxn {r.KeyProductSeq}").OrderBy(x => x).ToList() : new List<string>();
                    if (mainVM.TanVM != null && mainVM.TanVM.Reactions?.Count == 0 || mainVM.TanVM.Reactions?.Count == RXNAnnotsCount)
                    {
                        ReactionList = mainVM.TanVM.Reactions.Select(r => $"rxn {r.KeyProductSeq}").OrderBy(x => x).ToList();
                        if (!RXNMarkingsWithoutPageInfo.Except(ReactionList).Any() && !ReactionList.Except(RXNMarkingsWithoutPageInfo).Any())
                        {
                            var createdNums = mainVM.TanVM.ReactionParticipants.Where(tc => tc.ChemicalType == DTO.ChemicalType.S8000).Select(t => t.Num.ToString()).Distinct().ToList();
                            if (!mainVM.TanVM.ReactionParticipants.Where(tc => tc.ChemicalType == DTO.ChemicalType.S8000).Any() ||
                                mainVM.TanVM.ReactionParticipants.Where(tc => tc.ChemicalType == DTO.ChemicalType.S8000).Select(t => t.Num).Distinct().Count() == S8000AnnotsCount)
                            {
                                if (!createdNums.Except(S8000MarkingsWithoutPageInfo).Any() && !S8000MarkingsWithoutPageInfo.Except(createdNums).Any() && action != Common.Action.SHOW)
                                {
                                    foxit.Save();
                                    var mainDocument = mainVM.TanVM.PdfDocumentsList.Where(td => td.KeyPath).FirstOrDefault();
                                    if (mainDocument != null)
                                    {
                                        ListPdfDocuments.SelectedItem = mainDocument;
                                        ListPdfDocuments_SelectionChanged(null, null);
                                    }
                                    string tempPAth = Path.Combine(Path.GetDirectoryName(foxit.FilePath), $"{Path.GetFileNameWithoutExtension(foxit.FilePath)}_Modified.pdf");
                                    PdfAnnotsReplacer.ApplyProperties(foxit.FilePath, tempPAth);
                                    if (File.Exists(tempPAth))
                                        File.Copy(tempPAth, Path.Combine(C.SHAREDPATH, mainVM.TanVM.DocumentPath), true);
                                    else
                                    {
                                        AppInfoBox.ShowInfoMessage("Annotations Updated pdf not found.");
                                        return;
                                    }
                                    if (action == Common.Action.SUBMIT)
                                        ((App.Current.MainWindow) as MainWindow).SubmitTan();
                                    else if (action == Common.Action.APPROVE)
                                        ((App.Current.MainWindow) as MainWindow).ApproveTan();
                                }
                                else
                                    ShowPdfReactionViewWindow(createdNums, S8000Markings, false);
                            }
                            else
                                ShowPdfReactionViewWindow(createdNums, S8000Markings, false);
                        }
                        else
                        {
                            var PdfReactions = RXNMarkings.OrderBy(s => s).ToList();
                            var CreatedReactions = ReactionList.OrderBy(s => s).ToList();
                            ShowPdfReactionViewWindow(CreatedReactions, PdfReactions);
                        }
                    }
                    else
                    {
                        var PdfReactions = RXNMarkings.OrderBy(s => s).ToList();
                        var CreatedReactions = ReactionList.OrderBy(s => s).ToList();
                        ShowPdfReactionViewWindow(CreatedReactions, PdfReactions);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
                AppErrorBox.ShowErrorMessage("Error in Submitting Tan Please Try after some time", ex.ToString());
            }

        }

        private void ShowPdfReactionViewWindow(List<String> CreatedList, List<string> markedList, bool ReactionsList = true)
        {
            var StartTime = DateTime.Now;
            Debug.WriteLine($"ShowPdfReactionViewWindow started at: {StartTime}");
            var CreatedReactionsToShow = new List<ListToShow>();
            var markedReactionsToShow = new List<ListToShow>();
            foreach (var entry in CreatedList)
                CreatedReactionsToShow.Add(new ListToShow { TextToShow = entry, IsValid = markedList.Where(s => s.SafeContainsLower(entry)).Any() });
            foreach (var entry in markedList)
                markedReactionsToShow.Add(new ListToShow { TextToShow = entry, IsValid = CreatedList.Contains(entry.Substring(0, entry.IndexOf("(")).Trim()) });

            PdfReactionsView.ShowWindow(CreatedReactionsToShow, markedReactionsToShow, ReactionsList ? "PDF Reactions vs Created Reactions" : "PDF 8000 List vs Created 8000 List", ReactionsList ? "Created Reaction" : "Created 8000 List", ReactionsList ? "PDF Reactions" : "PDF 8000 List");
        }

        private void WorkingArea_ChooseClicked(object sender, EventArgs e)
        {
            try
            {
                ShowPopupWindow();
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private void WorkingArea_RowDoubleClicked(object sender, ReactionParticipantVM e)
        {
            try
            {
                ShowPopupWindow();
                var popupvm = popupWindow.DataContext as PopupVM;
                popupvm.SelectedChemicalType = e.ChemicalType;
                popupvm.ParticipantType = e.ParticipantType;
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private void WorkingArea_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                var mainVM = (App.Current.MainWindow as MainWindow).DataContext as MainVM;
                if (e.Key == Key.Escape)
                {
                    if ((((MainWindow)(App.Current.MainWindow)).DataContext as MainVM)?.TanVM != null)
                    {
                        (((MainWindow)(App.Current.MainWindow)).DataContext as MainVM).TanVM.EditingParticipant = null;
                    }
                    popupWindow.Hide();
                }
                else if (e.Key == Key.F && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                    NumFilter.Focus();
                else if (e.Key == Key.F && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                    SnoFilter.Focus();
                else if (e.Key == Key.F6 && mainVM!=null && mainVM.TanVM != null && U.RoleId == 2)
                {
                    WorkingArea_BaforeTanSubmiting(null,Common.Action.SHOW);
                }
                else if (e.Key == Key.F4 && !popupWindow.IsVisible && mainVM.IsTanLoaded == Visibility.Visible && mainVM.PreviewTabIndex == 0)
                    ShowPopupWindow();

            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        public void ShowPopupWindow(bool alreadyOpen = false)
        {
            try
            {
                var mainVM = (App.Current.MainWindow as MainWindow).DataContext as MainVM;
                if (mainVM != null && popupWindow != null)
                {
                    popupWindow.Left = System.Windows.SystemParameters.WorkArea.Width - popupWindow.Width - 25;
                    popupWindow.Top = System.Windows.SystemParameters.WorkArea.Height - popupWindow.Height - 150;
                    var popupVM = popupWindow.DataContext as PopupVM;
                    if (popupVM != null)
                    {
                        if (mainVM.TanVM != null && mainVM.TanVM.SelectedReaction != null && mainVM.TanVM.SelectedReaction?.SelectedStage != null && mainVM.TanVM.SelectedReaction?.SelectedStage.DisplayOrder > 1)
                        {
                            popupVM.IsProductEnable = false;
                            popupVM.ParticipantType = DTO.ParticipantType.Reactant;
                        }
                        else
                        {
                            popupVM.IsProductEnable = true;
                            popupVM.ParticipantType = DTO.ParticipantType.Product;
                        }
                        popupVM.SearchString = string.Empty;
                        popupVM.SelectedTabIndex = 0;
                        popupVM.MolString = string.Empty;
                        popupVM.S8000Metas.Clear();
                        popupVM.SubstanceName = string.Empty;
                        popupVM.CompoundNum = string.Empty;
                        popupVM.GenericName = string.Empty;
                    }
                    popupWindow.Owner = (App.Current.MainWindow);
                    if (!alreadyOpen)
                        popupWindow.show();
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private void ChooseLink_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ShowPopupWindow();
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private void ChemicalsGrid_DataLoaded(object sender, EventArgs e)
        {
            try
            {
                ChemicalsGrid.ExpandAllGroups();
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }
        private void CVTAutoComplete_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                var ac = (sender as RadAutoCompleteBox);
                if (ac != null && !ac.IsDropDownOpen)
                    ac.Populate(ac.SearchText);
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private void ConditionGridView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var mainViewModel = ((MainWindow)(App.Current.MainWindow)).DataContext as MainVM;
                if (mainViewModel != null && mainViewModel.TanVM != null && mainViewModel.TanVM.SelectedReaction != null && mainViewModel.TanVM.SelectedReaction.SelectedStage != null)
                {
                    conditions.DataContext = conditionVM;
                    StageConditionVM stageCondition = mainViewModel.TanVM.SelectedReaction.SelectedStage.SelectedCondition;


                    if (stageCondition != null)
                    {

                        if (stageCondition.TEMP_TYPE != null)
                        {
                            UnselectRadiobuttion();

                            string[] splitter = { "]" };
                            string[] strVals = stageCondition.Temperature.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                            string strTPUnits = "";


                            if (stageCondition.TEMP_TYPE == TemperatureEnum.Temp.ToString())
                            {
                                conditionVM.SelectTempTemperature = conditionVM.GetTemperatureAndUnitsFromString(strVals[0], out strTPUnits);
                                temperatureVM = StaticCollection.TemperatureCodes.ToList().Where(x => x.Value == strTPUnits).SingleOrDefault();
                                conditionVM.SelectedComboValue = temperatureVM;

                            }
                            else if (stageCondition.TEMP_TYPE == TemperatureEnum.RoomTemptoReflux.ToString())
                            {
                                conditionVM.SelectradioRoomTemptoReflux = "True";

                            }
                            else if (stageCondition.TEMP_TYPE == TemperatureEnum.Roomtempto.ToString())
                            {

                                conditionVM.SelectRoomtempto = conditionVM.GetTemperatureAndUnitsFromString(strVals[1], out strTPUnits);
                                temperatureVM = StaticCollection.TemperatureCodes.ToList().Where(x => x.Value == strTPUnits).SingleOrDefault();
                                conditionVM.SelectedComboRoomtempto = temperatureVM;
                            }
                            else if (stageCondition.TEMP_TYPE == TemperatureEnum.toRoomTemp.ToString())
                            {
                                conditionVM.SelecttoRoomTemp = conditionVM.GetTemperatureAndUnitsFromString(strVals[0], out strTPUnits);
                                temperatureVM = StaticCollection.TemperatureCodes.ToList().Where(x => x.Value == strTPUnits).SingleOrDefault();
                                conditionVM.SelectedCombotoRoomTemp = temperatureVM;

                            }
                            else if (stageCondition.TEMP_TYPE == TemperatureEnum.Directional.ToString())
                            {
                                string[] splitterRange = { "]" };
                                string[] strValsRange = stageCondition.Temperature.Split(splitterRange, StringSplitOptions.RemoveEmptyEntries);
                                string strTPUnitsRange = "";
                                conditionVM.SelectDirectional1 = conditionVM.GetTemperatureAndUnitsFromString(strValsRange[0], out strTPUnitsRange);
                                conditionVM.SelectDirectional2 = conditionVM.GetTemperatureAndUnitsFromString(strValsRange[1], out strTPUnitsRange);
                                temperatureVM = StaticCollection.TemperatureCodes.ToList().Where(x => x.Value == strTPUnitsRange).SingleOrDefault();
                                conditionVM.SelectedComboDirectional = temperatureVM;
                            }
                            else if (stageCondition.TEMP_TYPE == TemperatureEnum.Rang.ToString())
                            {
                                string[] splitterRange = { "-" };
                                string[] strValsRange = stageCondition.Temperature.Split(splitterRange, StringSplitOptions.RemoveEmptyEntries);
                                string strTPUnitsRange = "";
                                //     string strRngTo;
                                //string   strRngFrom = GetRangeValuesFromString(splitterRange, out strRngTo);


                                if (stageCondition.Temperature.StartsWith($"-{strValsRange[0]}"))
                                    strValsRange[0] = $"-{strValsRange[0]}";
                                if (stageCondition.Temperature.Contains($"--{strValsRange[1]}"))
                                    strValsRange[1] = $"-{strValsRange[1]}";
                                conditionVM.SelectRang1 = conditionVM.GetTemperatureAndUnitsFromString(strValsRange[0], out strTPUnitsRange);
                                conditionVM.SelectRang2 = conditionVM.GetTemperatureAndUnitsFromString(strValsRange[1], out strTPUnitsRange);
                                temperatureVM = StaticCollection.TemperatureCodes.ToList().Where(x => x.Value == strTPUnitsRange).SingleOrDefault();
                                conditionVM.SelectedComboRang = temperatureVM;
                            }
                            else if (stageCondition.TEMP_TYPE == TemperatureEnum.Refluxto.ToString())
                            {
                                conditionVM.SelectRefluxto = conditionVM.GetTemperatureAndUnitsFromString(strVals[1], out strTPUnits);
                                temperatureVM = StaticCollection.TemperatureCodes.ToList().Where(x => x.Value == strTPUnits).SingleOrDefault();
                                conditionVM.SelectedComboRefluxto = temperatureVM;

                            }
                            else if (stageCondition.TEMP_TYPE == TemperatureEnum.toReflux.ToString())
                            {
                                conditionVM.SelecttoReflux = conditionVM.GetTemperatureAndUnitsFromString(strVals[0], out strTPUnits);
                                temperatureVM = StaticCollection.TemperatureCodes.ToList().Where(x => x.Value == strTPUnits).SingleOrDefault();
                                conditionVM.SelectedCombotoReflux = temperatureVM;

                            }
                            else if (stageCondition.TEMP_TYPE == TemperatureEnum.pluseDevminus.ToString())
                            {

                                string[] splitterPlus = { "+/-" };
                                string[] strValsPlus = stageCondition.Temperature.Split(splitterPlus, StringSplitOptions.RemoveEmptyEntries);
                                conditionVM.SelectPlessMinues1 = conditionVM.GetTemperatureAndUnitsFromString(strValsPlus[0], out strTPUnits);
                                temperatureVM = StaticCollection.TemperatureCodes.ToList().Where(x => x.Value == strTPUnits).SingleOrDefault();
                                conditionVM.SelectPlessMinues2 = conditionVM.GetTemperatureAndUnitsFromString(strValsPlus[1], out strTPUnits);
                                conditionVM.SelectedComboPluessorMinnus = temperatureVM;
                            }
                            else if (stageCondition.TEMP_TYPE == TemperatureEnum.RefluxBoiled.ToString())
                            {
                                conditionVM.SelectradioRefluxBoiled = "True";
                            }
                            else if (stageCondition.TEMP_TYPE == TemperatureEnum.Roomtemp.ToString())
                            {
                                conditionVM.SelectradioRoomTemperature = "True";
                            }
                            else if (stageCondition.TEMP_TYPE == TemperatureEnum.RefluxtoRoomTemp.ToString())
                            {
                                conditionVM.SelectradioReflextoRoomTemp = "True";
                            }

                            else if (stageCondition.TEMP_TYPE == TemperatureEnum.Cool.ToString())
                            {
                                conditionVM.SelectradioCool = "True";
                            }
                            else if (stageCondition.TEMP_TYPE == TemperatureEnum.equels.ToString())
                            {
                                conditionVM.Selectradioequals = "True";
                            }
                            else if (stageCondition.TEMP_TYPE == TemperatureEnum.Heated.ToString())
                            {
                                conditionVM.SelectradioHeated = "True";
                            }
                            else if (stageCondition.TEMP_TYPE == TemperatureEnum.LessthanRoomTemp.ToString())
                            {
                                conditionVM.SelectradioLessthanRoomTemp = "True";
                            }
                            else if (stageCondition.TEMP_TYPE == TemperatureEnum.RoomTempgraterthan.ToString())
                            {
                                conditionVM.SelectradioGreaterthanRoomTemp = "True";
                            }
                            else
                            {
                                conditionVM.SelectradioNoon = "True";
                            }


                        }
                        if (stageCondition.TIME_TYPE != null)
                        {
                            UnselectTime();
                            if (stageCondition.TIME_TYPE == TimeEnum.Time.ToString())
                            {

                                string strTPUnits = "";
                                conditionVM.SelectTimeValue = conditionVM.GetTimeAndUnitsFromString(stageCondition.Time, out strTPUnits);
                                timeComboCodevalues = StaticCollection.TimeComboCodevalues.ToList().Where(x => x.Value == strTPUnits).SingleOrDefault();
                                conditionVM.SelectedTimeComboValue = timeComboCodevalues;

                            }
                            else if (stageCondition.TIME_TYPE == TimeEnum.Rang.ToString())
                            {

                                string[] splitter = { "-" };
                                string[] strVals = stageCondition.Time.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                                string strTPUnits = "";
                                conditionVM.SelectTimeRangetext1 = conditionVM.GetTimeAndUnitsFromString(strVals[0], out strTPUnits);
                                conditionVM.SelectTimeRangetext2 = conditionVM.GetTimeAndUnitsFromString(strVals[1], out strTPUnits);
                                timeComboCodevalues = StaticCollection.TimeComboCodevalues.ToList().Where(x => x.Value == strTPUnits).SingleOrDefault();
                                conditionVM.SelectedCoboTimeRange = timeComboCodevalues;

                            }
                            else if (stageCondition.TIME_TYPE.Contains(TimeEnum.InExactTime.ToString()))
                            {
                                string[] splitter = { "_" };
                                string[] strTMVals = stageCondition.TIME_TYPE.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                                string INExactText = conditionVM.GetSelectedINExactTimeFromString(strTMVals[1]);
                                timeCombovalues = StaticCollection.TimeCombovalues.ToList().Where(x => x.Name == INExactText).FirstOrDefault();
                                conditionVM.SelectedCoboTimeInExact = timeCombovalues;


                            }
                            else if (stageCondition.TIME_TYPE == TimeEnum.equals.ToString())
                            {
                                conditionVM.SelectedradioTimeEquals = "True";
                            }
                            else if (stageCondition.TIME_TYPE == TimeEnum.Overnight.ToString())
                            {
                                conditionVM.SelectedradioTimeOvernight = "True";
                            }
                            else
                            {
                                conditionVM.SelectedradioTimeNone = "True";
                            }


                        }

                        if (stageCondition.PH_TYPE != null)
                        {
                            UnselectpHControls();
                            if (stageCondition.PH_TYPE == PHEnum.pH.ToString())
                            {
                                conditionVM.SelectpH = stageCondition.PH;

                            }
                            else if (stageCondition.PH_TYPE == PHEnum.Range.ToString())
                            {

                                string[] splitter = { "-" };
                                string[] strVals = stageCondition.PH.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                                conditionVM.SelectPHRange1 = strVals[0];
                                conditionVM.SelectPHRange2 = strVals[1];


                            }
                            else if (stageCondition.PH_TYPE == PHEnum.AcidA.ToString())
                            {
                                conditionVM.SelectedradiopHAcid = "True";
                            }
                            else if (stageCondition.PH_TYPE == PHEnum.Base.ToString())
                            {
                                conditionVM.SelectedradiopHBase = "True";
                            }
                            else if (stageCondition.PH_TYPE == PHEnum.Neutral.ToString())
                            {
                                conditionVM.SelectedradiopHNeutral = "True";
                            }
                            else if (stageCondition.PH_TYPE == PHEnum.equal.ToString())
                            {
                                conditionVM.SelectedradiopHequal = "True";
                            }
                            else
                            {
                                conditionVM.SelectedradiopHNone = "True";
                            }
                        }
                        if (stageCondition.PRESSURE_TYPE != null)
                        {
                            UnselectPressure();
                            if (stageCondition.PRESSURE_TYPE == PressureEnum.Pressure.ToString())
                            {


                                string strTPUnits = "";
                                conditionVM.SelectPressure = conditionVM.GetPressureAndUnitsFromString(stageCondition.Pressure, out strTPUnits);
                                pressureComboValues = StaticCollection.PressureComboValuess.ToList().Where(x => x.Value == strTPUnits).SingleOrDefault();
                                conditionVM.SelectedPresureComboPressure = pressureComboValues;

                            }
                            else if (stageCondition.PRESSURE_TYPE == PressureEnum.Directional.ToString())
                            {

                                string[] splitter = { "]" };
                                string[] strVals = stageCondition.Pressure.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                                string strTPUnits = "";
                                conditionVM.SelectPressureDirectional1 = conditionVM.GetPressureAndUnitsFromString(strVals[0], out strTPUnits);
                                conditionVM.SelectPressureDirectional2 = conditionVM.GetPressureAndUnitsFromString(strVals[1], out strTPUnits);
                                pressureComboValues = StaticCollection.PressureComboValuess.ToList().Where(x => x.Value == strTPUnits).SingleOrDefault();
                                conditionVM.SelectedPressureComboDirectional = pressureComboValues;


                            }
                            else if (stageCondition.PRESSURE_TYPE == PressureEnum.Range.ToString())
                            {

                                string[] splitter = { "-" };
                                string[] strVals = stageCondition.Pressure.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                                string strTPUnits = "";
                                conditionVM.SelectPressureRangetext1 = conditionVM.GetPressureAndUnitsFromString(strVals[0], out strTPUnits);
                                conditionVM.SelectPressureRangetext2 = conditionVM.GetPressureAndUnitsFromString(strVals[1], out strTPUnits);
                                pressureComboValues = StaticCollection.PressureComboValuess.ToList().Where(x => x.Value == strTPUnits).SingleOrDefault();
                                conditionVM.SelectedPressureCoboRange = pressureComboValues;

                            }
                            else if (stageCondition.PRESSURE_TYPE == PressureEnum.equal.ToString())
                            {
                                conditionVM.SelectedradioPressureequal = "True";
                            }
                            else
                            {
                                conditionVM.SelectedradioPressureNone = "True";
                            }
                        }

                        else
                        {
                            conditionVM.SelectedConditionId = Guid.Empty;
                            UnselectRadiobuttion();
                            UnselectTime();
                            UnselectpHControls();
                            UnselectPressure();
                            conditionVM.SelectradioNoon = "True";
                            conditionVM.SelectedradioPressureNone = "True";
                            conditionVM.SelectedradiopHNone = "True";
                            conditionVM.SelectedradioTimeNone = "True";
                        }
                        if (stageCondition.Id != Guid.Empty)
                        {
                            conditionVM.SelectedConditionId = stageCondition.Id;
                        }
                        else
                        {
                            conditionVM.SelectedConditionId = Guid.Empty;
                        }
                    }
                    else
                    {
                        conditionVM.SelectedConditionId = Guid.Empty;
                        UnselectRadiobuttion();
                        UnselectTime();
                        UnselectpHControls();
                        UnselectPressure();
                        conditionVM.SelectradioNoon = "True";
                        conditionVM.SelectedradioPressureNone = "True";
                        conditionVM.SelectedradiopHNone = "True";
                        conditionVM.SelectedradioTimeNone = "True";
                    }
                    conditions.Owner = (App.Current.MainWindow);
                    conditions.show();
                }
                else
                {
                    MessageBox.Show("Please Select Stage", "Conditions", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private void UnselectRadiobuttion()
        {
            try
            {

                conditionVM.SelectTempTemperature = "";

                temperatureVM = StaticCollection.TemperatureCodes.ToList().Where(x => x.Value == "c").SingleOrDefault();
                conditionVM.SelectedComboValue = temperatureVM;

                conditionVM.SelectRoomtempto = "";
                conditionVM.SelectedComboRoomtempto = temperatureVM;
                conditionVM.SelectedCombotoRoomTemp = temperatureVM;
                conditionVM.SelecttoRoomTemp = "";
                conditionVM.SelectDirectional1 = "";
                conditionVM.SelectDirectional2 = "";
                conditionVM.SelectedComboDirectional = temperatureVM;
                conditionVM.SelectRang1 = "";
                conditionVM.SelectRang2 = "";
                conditionVM.SelecttoReflux = "";
                conditionVM.SelectedComboRang = temperatureVM;
                conditionVM.SelectedCombotoReflux = temperatureVM;
                conditionVM.SelectRefluxto = "";
                conditionVM.SelectedComboRefluxto = temperatureVM;
                conditionVM.SelectPlessMinues1 = "";
                conditionVM.SelectPlessMinues2 = "";
                conditionVM.SelectedComboPluessorMinnus = temperatureVM;
                conditionVM.SelectradioLessthanRoomTemp = "false";
                conditionVM.SelectradioHeated = "false";
                conditionVM.Selectradioequals = "false";
                conditionVM.SelectradioCool = "false";
                conditionVM.SelectradioReflextoRoomTemp = "false";
                conditionVM.SelectradioRoomTemptoReflux = "false";
                conditionVM.SelectradioRefluxBoiled = "false";
                //conditionVM.SelectradioRoomTemptoReflux = "false";
                conditionVM.SelectradioGreaterthanRoomTemp = "false";
                conditionVM.SelectradioNoon = "false";
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private void UnselectTime()
        {
            try
            {
                timeCombovalues = StaticCollection.TimeCombovalues.ToList().Where(x => x.Name == "Select").SingleOrDefault();
                timeComboCodevalues = StaticCollection.TimeComboCodevalues.ToList().Where(x => x.Value == "h").SingleOrDefault();
                conditionVM.SelectTimeValue = "";
                conditionVM.SelectedTimeComboValue = timeComboCodevalues;
                conditionVM.SelectTimeRangetext1 = "";
                conditionVM.SelectTimeRangetext2 = "";
                conditionVM.SelectedCoboTimeRange = timeComboCodevalues;
                conditionVM.SelectedCoboTimeInExact = timeCombovalues;
                conditionVM.SelectedradioTimeEquals = "false";
                conditionVM.SelectedradioTimeOvernight = "false";
                conditionVM.SelectedradioTimeNone = "false";
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }

        }

        private void UnselectpHControls()
        {
            try
            {
                conditionVM.SelectpH = "";
                conditionVM.SelectPHRange1 = "";
                conditionVM.SelectPHRange2 = "";
                conditionVM.SelectedradiopHAcid = "false";
                conditionVM.SelectedradiopHBase = "false";
                conditionVM.SelectedradiopHNeutral = "false";
                conditionVM.SelectedradiopHequal = "false";
                conditionVM.SelectedradiopHNone = "false";
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private void UnselectPressure()
        {
            try
            {
                pressureComboValues = StaticCollection.PressureComboValuess.ToList().Where(x => x.Value == "a").SingleOrDefault();
                conditionVM.SelectPressure = "";
                conditionVM.SelectedPresureComboPressure = pressureComboValues;
                conditionVM.SelectPressureDirectional1 = "";
                conditionVM.SelectPressureDirectional2 = "";
                conditionVM.SelectedPressureComboDirectional = pressureComboValues;
                conditionVM.SelectPressureRangetext1 = "";
                conditionVM.SelectPressureRangetext2 = "";
                conditionVM.SelectedPressureCoboRange = pressureComboValues;
                conditionVM.SelectedradioPressureequal = "false";
                conditionVM.SelectedradioPressureNone = "false";
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }

        }

        void MasterTanLoaded(object s, Tan tan)
        {
            try
            {
                var mainVM = ((App.Current.MainWindow as MainWindow).DataContext as MainVM);
                if (mainVM != null && mainVM.TanVM != null)
                {
                    string DocumentPath = Path.Combine(T.TanFolderpath, Path.GetFileName((ListPdfDocuments.SelectedItem as TanDocumentsVM).Path));
                    foreach (var item in ListPdfDocuments.ItemsSource)
                    {
                        string localDocumentPath = Path.Combine(T.TanFolderpath, Path.GetFileName((item as TanDocumentsVM).Path));
                        if (!Directory.Exists(Path.GetDirectoryName(localDocumentPath)))
                            Directory.CreateDirectory(Path.GetDirectoryName(localDocumentPath));
                        if (!File.Exists(localDocumentPath) || (App.Current.MainWindow as MainWindow).LoadServerVersion)
                            File.Copy((item as TanDocumentsVM).Path, localDocumentPath, true);
                        var document = mainVM.TanVM.PdfDocumentsList.Where(pdf => pdf.Path == (item as TanDocumentsVM).Path).FirstOrDefault();
                        document.LocalPath = localDocumentPath;
                    }
                    tan.LocalDocumentPath = DocumentPath;
                    mainVM.TanVM.LocalDocumentPath = DocumentPath;
                    var foxit = PdfHost.Child as AxFoxitPDFSDK;
                    if (foxit != null)
                    {
                        foxit.ShowTitleBar(false);
                        foxit.ShowToolBar(false);
                        foxit.ShowNavigationPanels(true);
                        if (foxit.OpenFile(DocumentPath, String.Empty))
                        {
                            foxit.RemoveEvaluationMark();
                            PdfAnnotations.FilePath = DocumentPath;
                            foxit.GoToPage(tan.DocumentCurrentPage);
                        }
                        else
                            AppErrorBox.ShowErrorMessage("Can't open pdf file", $"{DocumentPath} Can't be opened");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
                AppErrorBox.ShowErrorMessage("Error While TAN Loading", ex.ToString());
            }
        }

        private void WorkingArea_TanClosed(object sender, TanVM e)
        {
            try
            {
                var foxit = PdfHost.Child as AxFoxitPDFSDK;
                if (foxit != null)
                {
                    foxit.CloseFile();
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private void WorkingArea_TANSaved(object sender, EventArgs e)
        {
            try
            {
                var foxit = PdfHost.Child as AxFoxitPDFSDK;
                if (ListPdfDocuments != null && ListPdfDocuments.Items != null)
                {
                    foreach (var item in ListPdfDocuments.Items)
                    {
                        if ((item as TanDocumentsVM).KeyPath)
                        {
                            ListPdfDocuments.SelectedItem = item;
                            ListPdfDocuments_SelectionChanged(null, null);
                            break;
                        }
                    }
                }
                if (foxit != null)
                    foxit.Save();
                var mainVM = (App.Current.MainWindow as MainWindow).DataContext as MainVM;
                if (mainVM != null && mainVM.TanVM != null)
                    File.Copy(foxit.FilePath, Path.Combine(C.SHAREDPATH, mainVM.TanVM.DocumentPath), true);
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private void AfterInsertCondition_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var mainViewModel = ((MainWindow)(App.Current.MainWindow)).DataContext as MainVM;
                if (mainViewModel.TanVM != null && mainViewModel.TanVM.SelectedReaction != null && mainViewModel.TanVM.SelectedReaction.SelectedStage != null)
                {
                    ShowConditionsWindow();
                    if (mainViewModel.TanVM.SelectedReaction.SelectedStage.SelectedCondition != null)
                    {
                        mainViewModel.TanVM.SelectedReaction.SelectedStage.BeforeInsert = false;
                        mainViewModel.TanVM.SelectedReaction.SelectedStage.AfterInsert = true;
                    }
                }
                else
                    AppInfoBox.ShowInfoMessage("Please Select stage to add Conditions");

            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private void BeforeInsertCondition_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var mainViewModel = ((MainWindow)(App.Current.MainWindow)).DataContext as MainVM;
                if (mainViewModel.TanVM != null && mainViewModel.TanVM.SelectedReaction != null && mainViewModel.TanVM.SelectedReaction.SelectedStage != null)
                {
                    ShowConditionsWindow();
                    if (mainViewModel.TanVM.SelectedReaction.SelectedStage.SelectedCondition != null)
                    {
                        mainViewModel.TanVM.SelectedReaction.SelectedStage.BeforeInsert = true;
                        mainViewModel.TanVM.SelectedReaction.SelectedStage.AfterInsert = false;
                    }
                }
                else
                    AppInfoBox.ShowInfoMessage("Please Select stage to add Conditions");
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private void AddConditionBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ShowConditionsWindow();
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private void ShowConditionsWindow()
        {
            try
            {
                var mainViewModel = ((MainWindow)(App.Current.MainWindow)).DataContext as MainVM;
                if (mainViewModel.TanVM != null && mainViewModel.TanVM.SelectedReaction != null && mainViewModel.TanVM.SelectedReaction.SelectedStage != null)
                {
                    conditions.DataContext = conditionVM;
                    conditionVM.SelectedConditionId = Guid.Empty;
                    UnselectRadiobuttion();
                    UnselectTime();
                    UnselectpHControls();
                    UnselectPressure();
                    conditionVM.SelectradioNoon = "True";
                    conditionVM.SelectedradioPressureNone = "True";
                    conditionVM.SelectedradiopHNone = "True";
                    conditionVM.SelectedradioTimeNone = "True";
                    conditions.Owner = (App.Current.MainWindow);
                    conditions.show();
                }
                else
                    AppInfoBox.ShowInfoMessage("Please Select stage to add Conditions");
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private void ChooseRSNBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var mainVM = ((App.Current.MainWindow as MainWindow).DataContext as MainVM);
                if (mainVM.TanVM != null && mainVM.TanVM.SelectedReaction != null && mainVM.TanVM.SelectedReaction.SelectedStage != null)
                    ChooseRsnWindow(mainVM);
                else
                    AppInfoBox.ShowInfoMessage("Please Select Reaction and stage To Work With RSN . .");
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private void ChooseRsnWindow(MainVM mainVM)
        {
            try
            {
                List<RsnVM> rsns = new List<RsnVM>();
                foreach (var rsnVM in mainVM.TanVM.Rsns)
                    rsns.Add(new RsnVM(rsnVM));
                rsnWindow.SetData(new Tuple<List<RsnVM>, ReactionVM, StageVM>(rsns.Where(rsn => rsn.Reaction.Id == mainVM.TanVM.SelectedReaction.Id).ToList(), mainVM.TanVM.SelectedReaction, mainVM.TanVM.SelectedReaction?.SelectedStage));
                rsnWindow.DialogStatus = false;
                (rsnWindow.DataContext as RSNWindowVM).ClearEditForm.Execute(null);
                rsnWindow.show(mainVM.TanVM);
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private void ListPdfDocuments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (ListPdfDocuments != null && ListPdfDocuments.SelectedItem != null)
                {
                    var tanDocumentsVM = ListPdfDocuments.SelectedItem as TanDocumentsVM;
                    if (tanDocumentsVM != null && tanDocumentsVM.TanId > 0 && !string.IsNullOrEmpty(tanDocumentsVM.Path) && !string.IsNullOrEmpty(T.TanFolderpath))
                    {
                        string DocumentPath = Path.Combine(T.TanFolderpath, Path.GetFileName(tanDocumentsVM.Path));
                        if (!Directory.Exists(Path.GetDirectoryName(DocumentPath)))
                            Directory.CreateDirectory(Path.GetDirectoryName(DocumentPath));
                        if (!File.Exists(DocumentPath))
                            File.Copy(tanDocumentsVM.Path, DocumentPath);
                        var foxit = PdfHost.Child as AxFoxitPDFSDK;
                        if (foxit != null && File.Exists(DocumentPath))
                        {
                            foxit.CloseFile();
                            foxit.OpenFile(DocumentPath, String.Empty);
                            foxit.RemoveEvaluationMark();
                            var mainVM = (App.Current.MainWindow as MainWindow).DataContext as MainVM;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            var mainVM = (App.Current.MainWindow as MainWindow).DataContext as MainVM;
            if (mainVM != null && mainVM.TanVM != null && mainVM.TanVM.SelectedReaction != null)
            {
                if (U.RoleId == 1)
                {
                    if (mainVM.TanVM.SelectedReaction.CuratorCreatedDate == DateTime.MinValue)
                        mainVM.TanVM.SelectedReaction.CuratorCreatedDate = DateTime.Now;
                    if (mainVM.TanVM.SelectedReaction.CuratorCompletedDate == DateTime.MinValue)
                        mainVM.TanVM.SelectedReaction.CuratorCompletedDate = DateTime.Now;
                    mainVM.TanVM.SelectedReaction.LastupdatedDate = DateTime.Now;
                }
                mainVM.TanVM.PerformAutoSave("Curation Completed");
            }
        }

        private void HandPdfBtn_Click(object sender, RoutedEventArgs e)
        {
            var foxit = PdfHost.Child as AxFoxitPDFSDK;
            if (foxit != null)
                foxit.CurrentTool = C.HAND_TOOL;
        }

        private void TextPdfBtn_Click(object sender, RoutedEventArgs e)
        {
            var foxit = PdfHost.Child as AxFoxitPDFSDK;
            if (foxit != null)
                foxit.CurrentTool = C.TEXT_TOOL;
        }

        private void ChemicalsGrid_CellEditEnded(object sender, GridViewCellEditEndedEventArgs e)
        {
            var mainVM = (App.Current.MainWindow as MainWindow).DataContext as MainVM;
            if (mainVM != null && mainVM.TanVM != null)
                mainVM.TanVM.PerformAutoSave("Yield Edited");
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            var foxit = PdfHost.Child as AxFoxitPDFSDK;
            string strSelected = string.Empty;
            if (foxit != null && !string.IsNullOrEmpty(foxit.GetSelectedText()))
            {
                strSelected = foxit.GetSelectedText();
                string url = $"{S.PDF_GOOGLE_SEARCH_URL}{strSelected}";
                ((MainWindow)(App.Current.MainWindow)).BrowserUrlChange($"Google Search - {strSelected.SafeSubstring(10)}", url);
            }
            else
                AppInfoBox.ShowInfoMessage("Please select text from Pdf Document");
        }

        private void SearchText_Click(object sender, RoutedEventArgs e)
        {
            var foxit = PdfHost.Child as AxFoxitPDFSDK;
            if (foxit != null)
            {
                PdfSearch.foxit = foxit;
                PdfSearch.ShowWindow();
            }
        }

        private void SearchAnnotations_Click(object sender, RoutedEventArgs e)
        {
            var foxit = PdfHost.Child as AxFoxitPDFSDK;
            if (foxit != null && !string.IsNullOrEmpty(foxit.FilePath))
            {
                PdfAnnotations.FilePath = foxit.FilePath;
                PdfAnnotations.ShowWindow();
            }
        }
    }
}
