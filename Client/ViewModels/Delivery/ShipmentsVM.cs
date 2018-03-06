using Client.Command;
using Client.Common;
using Client.Logging;
using Client.Views;
using Client.Views.Delivery;
using Entities;
using Entities.DTO;
using Entities.DTO.Delivery;
using Excelra.Utils.Library;
using ProductTracking.Models.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Telerik.Windows.Data;

namespace Client.ViewModels.Delivery
{
    public class ShipmentsVM : ViewModelBase
    {
        private bool loadingRSNs, loading8085Comments, loading8000Data, workInProgress;
        private Visibility deliveryInProgress;
        private BatchVM fromBatch, toBatch;
        private DeliveryBatchVM selectedDeliveryBatch;
        private DeliveryBatchVM deliveryBatch;
        private ObservableCollection<BatchVM> fromBatches, toBatches;
        private ObservableCollection<DeliveryBatchVM> deliveryBatches;
        private ObservableCollection<BatchTanVM> batchTans;
        private ObservableCollection<object> selectedTans;
        private BatchTanVM selectedTan;
        private QueryableCollectionView batchTansView;
        private TanCategoryVM fromCategory, toCategory;
        private ObservableCollection<TanCategoryVM> fromCategories, toCategories;
        private int totalTans, extraStages, zeroRXNs;
        private int curationAssigned, curationProgress, curationDone, reviewAssigned, reviewProgress, qcAssigned, qcProgress, qcCompleted,
            rxnCurationAssigned, rxnCurationProgress, rxnReviewAssigned, rxnReviewProgress, rxnQcAssigned, rxnQcProgress, rxnQcCompleted;
        private int notAssigned;
        private int maxZipSize;

        public bool WorkInProgress { get { return workInProgress; } set { SetProperty(ref workInProgress, value); } }
        public int MaxZipSize { get { return maxZipSize; } set { SetProperty(ref maxZipSize, value); } }
        public bool LoadingRSNs { get { return loadingRSNs; } set { SetProperty(ref loadingRSNs, value); } }
        public bool Loading8085Comments { get { return loading8085Comments; } set { SetProperty(ref loading8085Comments, value); } }
        public bool Loading8000Data { get { return loading8000Data; } set { SetProperty(ref loading8000Data, value); } }
        public int NotAssigned { get { return notAssigned; } set { SetProperty(ref notAssigned, value); } }
        public ObservableCollection<BatchVM> FromBatches { get { return fromBatches; } set { SetProperty(ref fromBatches, value); } }
        public BatchVM FromBatch
        {
            get { return fromBatch; }
            set
            {
                SetProperty(ref fromBatch, value);
                ClearTans();
            }
        }
        public ObservableCollection<DeliveryBatchVM> DeliveryBatches { get { return deliveryBatches; } set { SetProperty(ref deliveryBatches, value); } }
        public ObservableCollection<BatchVM> ToBatches { get { return toBatches; } set { SetProperty(ref toBatches, value); } }
        public BatchVM ToBatch
        {
            get { return toBatch; }
            set
            {
                SetProperty(ref toBatch, value);
                ClearTans();
            }
        }
        public DeliveryBatchVM DeliveryBatch
        {
            get { return deliveryBatch; }
            set
            {
                SetProperty(ref deliveryBatch, value);
                ClearTans();
            }
        }

        public DeliveryBatchVM SelectedDeliveryBatch
        {
            get { return selectedDeliveryBatch; }
            set
            {
                SetProperty(ref selectedDeliveryBatch, value);
                ClearTans();
            }
        }
        public ObservableCollection<BatchTanVM> BatchTans { get { return batchTans; } set { SetProperty(ref batchTans, value); } }
        public ObservableCollection<TanCategoryVM> FromCategories { get { return fromCategories; } set { SetProperty(ref fromCategories, value); } }
        public ObservableCollection<TanCategoryVM> ToCategories { get { return toCategories; } set { SetProperty(ref toCategories, value); } }
        public TanCategoryVM FromCategory
        {
            get { return fromCategory; }
            set
            {
                SetProperty(ref fromCategory, value);
                ClearTans();
            }
        }

        public TanCategoryVM ToCategory { get { return toCategory; } set { SetProperty(ref toCategory, value); } }
        public int TotalTans { get { return totalTans; } set { SetProperty(ref totalTans, value); } }
        public int ExtraStages { get { return extraStages; } set { SetProperty(ref extraStages, value); } }
        public int ZeroRXNs { get { return zeroRXNs; } set { SetProperty(ref zeroRXNs, value); } }
        public int CurationAssigned { get { return curationAssigned; } set { SetProperty(ref curationAssigned, value); } }
        public int CurationProgress { get { return curationProgress; } set { SetProperty(ref curationProgress, value); } }
        public int CurationDone { get { return curationDone; } set { SetProperty(ref curationDone, value); } }
        public int ReviewAssigned { get { return reviewAssigned; } set { SetProperty(ref reviewAssigned, value); } }
        public int ReviewProgress { get { return reviewProgress; } set { SetProperty(ref reviewProgress, value); } }
        public int ReviewDone { get { return reviewDone; } set { SetProperty(ref reviewDone, value); } }
        public int QCAssigned { get { return qcAssigned; } set { SetProperty(ref qcAssigned, value); } }
        public int QCProgress { get { return qcProgress; } set { SetProperty(ref qcProgress, value); } }
        public int QCCompleted { get { return qcCompleted; } set { SetProperty(ref qcCompleted, value); } }
        public int RXNCurationAssigned { get { return rxnCurationAssigned; } set { SetProperty(ref rxnCurationAssigned, value); } }
        public int RXNCurationProgress { get { return rxnCurationProgress; } set { SetProperty(ref rxnCurationProgress, value); } }
        public int RXNCurationDone { get { return rXNCurationDone; } set { SetProperty(ref rXNCurationDone, value); } }
        public int RXNReviewAssigned { get { return rxnReviewAssigned; } set { SetProperty(ref rxnReviewAssigned, value); } }
        public int RXNReviewProgress { get { return rxnReviewProgress; } set { SetProperty(ref rxnReviewProgress, value); } }
        public int RXNReviewDone { get { return rXNReviewDone; } set { SetProperty(ref rXNReviewDone, value); } }
        public int RXNQCAssigned { get { return rxnQcAssigned; } set { SetProperty(ref rxnQcAssigned, value); } }
        public int RXNQCProgress { get { return rxnQcProgress; } set { SetProperty(ref rxnQcProgress, value); } }
        public int RXNQCCompleted { get { return rxnQcCompleted; } set { SetProperty(ref rxnQcCompleted, value); } }
        public int NextBatch { get { return nextBatch; } set { SetProperty(ref nextBatch, value); } }//NextBatch
        public Visibility DeliveryInProgress { get { return deliveryInProgress; } set { SetProperty(ref deliveryInProgress, value); } }
        public BatchTanVM SelectedTan { get { return selectedTan; } set { SetProperty(ref selectedTan, value); } }
        public ObservableCollection<object> SelectedTans { get { return selectedTans; } set { SetProperty(ref selectedTans, value); } }
        public DelegateCommand SearchTans { get; private set; }
        public DelegateCommand GenerateXML { get; private set; }
        public DelegateCommand GenerateZip { get; private set; }
        public DelegateCommand GenerateMail { get; private set; }
        public DelegateCommand MoveToCategory { get; private set; }
        public DelegateCommand MoveToDelivery { get; private set; }
        public DelegateCommand Show8000NameLocations { get; private set; }
        public DelegateCommand ShowComments { get; private set; }
        public DelegateCommand ExtractRsns { get; private set; }
        public DelegateCommand SaveRsns { get; private set; }
        public DelegateCommand GenerateNextBatchNumber { get; private set; }
        public DelegateCommand RsnReplace { get; private set; }
        public DelegateCommand GenerateBatch { get; private set; }
        public DelegateCommand MarkAsDelivered { get; set; }

        #region 8000 name locations
        private ObservableCollection<S8000NameLocationVM> s8000NameLocations;
        private ListCollectionView s8000NameLocationView;
        public ObservableCollection<S8000NameLocationVM> S8000NameLocations { get { return s8000NameLocations; } set { SetProperty(ref s8000NameLocations, value); } }
        public ListCollectionView S8000NameLocationView { get { return s8000NameLocationView; } set { SetProperty(ref s8000NameLocationView, value); } }
        private async void DoShow8000NameLocations(object obj)
        {

            try
            {
                Loading8000Data = true;
                S8000NameLocationView = null;
                var result = await RestHub.S8000NameLocations(SelectedDeliveryBatch.Id, FromCategory.Value);
                if (result.HttpCode == System.Net.HttpStatusCode.OK)
                {
                    List<S8000NameLocationDTO> dtos = (List<S8000NameLocationDTO>)result.UserObject;
                    S8000NameLocations = new ObservableCollection<S8000NameLocationVM>();
                    foreach (var dto in dtos)
                    {
                        S8000NameLocations.Add(new S8000NameLocationVM
                        {
                            TanNumber = dto.TanNumber,
                            TanSeries = dto.TanSeries,
                            TanCategory = dto.TanCategory,
                            SubstanceName = dto.SubstanceName,
                            SubstanceLocation = dto.SubstanceLocation
                        });
                    }
                    S8000NameLocationView = new ListCollectionView(S8000NameLocations);
                    S8000NameLocationView.SortDescriptions.Add(new SortDescription("TanNumber", ListSortDirection.Ascending));

                }
                else
                    MessageBox.Show(result.StatusMessage);
            }
            catch (Exception ex)
            {
                Log.This(ex);
                AppErrorBox.ShowErrorMessage("Error while loading 8000 names", ex.ToString());

            }
            finally
            {
                Loading8000Data = false;
            }
        }
        #endregion

        #region 8580 Comments        
        private ObservableCollection<S8580CommentsVM> s8580Comments;
        private ListCollectionView s8580CommentsView;
        public ObservableCollection<S8580CommentsVM> S8580Comments { get { return s8580Comments; } set { SetProperty(ref s8580Comments, value); } }
        public ListCollectionView S8580CommentsView { get { return s8580CommentsView; } set { SetProperty(ref s8580CommentsView, value); } }
        private async void DoShowComments(object obj)
        {

            try
            {
                Loading8085Comments = true;
                S8580CommentsView = null;
                if (FromBatch != null)
                {
                    var result = await RestHub.S8580Comments(SelectedDeliveryBatch.Id, FromCategory.Value);
                    if (result.HttpCode == System.Net.HttpStatusCode.OK)
                    {
                        List<S8580CommentsDTO> dtos = (List<S8580CommentsDTO>)result.UserObject;
                        S8580Comments = new ObservableCollection<S8580CommentsVM>();
                        foreach (var dto in dtos)
                        {
                            S8580Comments.Add(new S8580CommentsVM
                            {
                                TanNumber = dto.TanNumber,
                                TanCategory = dto.TanCategory,
                                Comment = dto.Comment,
                                CommentType = dto.CommentType,
                                UserComment = dto.UserComment
                            });
                        }
                        S8580CommentsView = new ListCollectionView(S8580Comments);
                        S8580CommentsView.SortDescriptions.Add(new SortDescription("TanNumber", ListSortDirection.Ascending));
                    }
                    else
                        MessageBox.Show(result.StatusMessage);
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
                AppErrorBox.ShowErrorMessage("Error while loading 8580 Comments", ex.ToString());

            }
            finally
            {
                Loading8085Comments = false;
            }
        }
        #endregion

        #region Extract RSN view
        private ObservableCollection<ExtractRsnVM> extractedRsns;
        private ListCollectionView extractRsnView;
        private int nextBatch;
        private int reviewDone;
        private int rXNCurationDone;
        private int rXNReviewDone;

        public ObservableCollection<ExtractRsnVM> ExtractedRsns { get { return extractedRsns; } set { SetProperty(ref extractedRsns, value); } }
        public ListCollectionView ExtractRsnView { get { return extractRsnView; } set { SetProperty(ref extractRsnView, value); } }
        private async void DoExtractRsns(object obj)
        {

            try
            {
                LoadingRSNs = true;
                ExtractRsnView = null;
                if (FromBatch != null && FromCategory != null)
                {
                    var result = await RestHub.ExtractRsn(SelectedDeliveryBatch.Id, FromCategory.Value);
                    if (result.HttpCode == System.Net.HttpStatusCode.OK)
                    {
                        List<ExtractRSNDto> dtos = (List<ExtractRSNDto>)result.UserObject;
                        ExtractedRsns = new ObservableCollection<ExtractRsnVM>();
                        int count = 1;
                        Dictionary<string, Dictionary<int, int>> tanWiseRxnIdWiseTotalLengths = new Dictionary<string, Dictionary<int, int>>();
                        var tanNumbers = dtos.Select(dto => dto.TanNumber).Distinct();
                        foreach (var tan in tanNumbers)
                        {
                            tanWiseRxnIdWiseTotalLengths[tan] = dtos
                                .Where(d => d.TanNumber == tan)
                                .GroupBy(d => d.RXNSno)
                                .ToDictionary(d => d.Key, d => d.Select(dto => dto.CVT.SafeLength() + dto.FreeText.SafeLength()).Sum());
                        }
                        foreach (var dto in dtos)
                        {
                            var rxnIdWiseTotalLengths = tanWiseRxnIdWiseTotalLengths[dto.TanNumber];
                            ExtractRsnVM vm = new ExtractRsnVM
                            {
                                TanNumber = dto.TanNumber,
                                RXNSno = dto.RXNSno,
                                ProductNumber = dto.ProductNumber,
                                RxnSeq = dto.RxnSeq,
                                Stage = dto.Stage,
                                CVT = dto.CVT,
                                FreeText = dto.FreeText,
                                Level = dto.RSNType,
                                Id = dto.Id,
                                DisplayOrder = count++
                            };
                            if (rxnIdWiseTotalLengths != null && rxnIdWiseTotalLengths.ContainsKey(dto.RXNSno))
                            {
                                vm.TotalLength = rxnIdWiseTotalLengths[dto.RXNSno];
                                vm.Comment = $"Reaction {dto.RXNSno} Info.";
                            }
                            ExtractedRsns.Add(vm);
                        }
                        ExtractRsnView = new ListCollectionView(ExtractedRsns);
                        ExtractRsnView.SortDescriptions.Add(new SortDescription("DisplayOrder", ListSortDirection.Ascending));
                        ExtractRsnView.SortDescriptions.Add(new SortDescription("TanNumber", ListSortDirection.Ascending));
                        ExtractRsnView.SortDescriptions.Add(new SortDescription("Sno", ListSortDirection.Ascending));
                        ExtractRsnView.SortDescriptions.Add(new SortDescription("RxnSeq", ListSortDirection.Ascending));
                        ExtractRsnView.SortDescriptions.Add(new SortDescription("Stage", ListSortDirection.Ascending));
                    }
                    else
                        AppErrorBox.ShowErrorMessage("Can't Update RSN", result.StatusMessage);
                }
                else
                    AppInfoBox.ShowInfoMessage("From Batch and Category Are Required");

            }
            catch (Exception ex)
            {
                Log.This(ex);
                AppErrorBox.ShowErrorMessage("Error while Extract RSNs", ex.ToString());
            }
            finally
            {
                LoadingRSNs = false;
            }
        }
        private async void DoSaveRsns(object obj)
        {

            try
            {
                LoadingRSNs = true;
                if (ExtractedRsns != null && MessageBox.Show("Confirm Update . .", "Confirm Save", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    List<FreeTextUpdateDto> dtos = new List<FreeTextUpdateDto>();
                    foreach (var rsn in ExtractedRsns)
                        dtos.Add(new FreeTextUpdateDto { Id = rsn.Id, FreeText = rsn.FreeText });
                    FreeTextBulkDto bulkDto = new FreeTextBulkDto();
                    bulkDto.BatchId = SelectedDeliveryBatch.Id;
                    bulkDto.CategoryId = FromCategory.Value;
                    bulkDto.Dtos = dtos;
                    var status = await RestHub.UpdateFreeTextBulk(bulkDto);
                    if (status.HttpCode == System.Net.HttpStatusCode.OK)
                    {
                        ExtractRsns.Execute(this);
                        var result = MessageBox.Show("RSNs Updated Successfully . .", "Status", MessageBoxButton.OKCancel);
                    }
                    else
                        AppErrorBox.ShowErrorMessage("Can't Update RSNs . .", status.HttpResponse);
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
                AppErrorBox.ShowErrorMessage("Error while Updating RSNs", ex.ToString());

            }
            finally
            {
                LoadingRSNs = false;
            }
        }
        #endregion

        public ShipmentsVM()
        {
            SearchTans = new DelegateCommand(DoSearchTans);
            MoveToDelivery = new DelegateCommand(DoMoveToDelivery);
            MoveToCategory = new DelegateCommand(DoMoveToCategory);
            Show8000NameLocations = new DelegateCommand(DoShow8000NameLocations);
            ShowComments = new DelegateCommand(DoShowComments);
            ExtractRsns = new DelegateCommand(DoExtractRsns);
            SaveRsns = new DelegateCommand(DoSaveRsns);
            GenerateNextBatchNumber = new DelegateCommand(DoGenerateNextBatchNumber);
            GenerateXML = new DelegateCommand(DoGenerateXML);
            GenerateZip = new DelegateCommand(DoGenerateZip);
            GenerateMail = new DelegateCommand(DoGenerateMail);
            RsnReplace = new DelegateCommand(DoRsnReplace);
            GenerateBatch = new DelegateCommand(DoGenerateBatch);
            MarkAsDelivered = new DelegateCommand(DoMarkAsDelivered);
            DeliveryInProgress = Visibility.Hidden;
            MaxZipSize = 1400;
        }

        private async void DoMarkAsDelivered(object obj)
        {
            if (DeliveryBatch != null)
            {
                try
                {
                    WorkInProgress = true;
                    var status = await RestHub.UpdateDeliveryStatus(DeliveryBatch.Id);
                    if (status.HttpCode == System.Net.HttpStatusCode.OK)
                        AppInfoBox.ShowInfoMessage(status.StatusMessage);
                    else
                        AppErrorBox.ShowErrorMessage("Can't Update delivery status . .", status.HttpResponse);
                }
                catch (Exception ex)
                {
                    AppErrorBox.ShowErrorMessage("Can't Update delivery status . .", ex.ToString());
                }
                finally
                {
                    WorkInProgress = false;
                }
            }
            else
                AppInfoBox.ShowInfoMessage("Please select atleast one batch to update delivery status");
        }

        private void DoGenerateBatch(object obj)
        {
            if (NextBatch != 0)
            {
                var existingBatches = DeliveryBatches.OrderBy(d => d.BatchNumber).Select(t => t.BatchNumber);
                if (existingBatches.Any() && existingBatches.ToList().Contains(NextBatch))
                {
                    AppInfoBox.ShowInfoMessage("Selected batch Already exist");
                    return;
                }
                DoGenerateNextBatchNumber(null);
            }
            else
                AppInfoBox.ShowInfoMessage("Enter Batch number to Generate new delivery batch");
        }

        private void DoRsnReplace(object obj)
        {
            FreetextReplaceVM vm = new FreetextReplaceVM();
            vm.Rsns = new List<ExtractRsnVM>(ExtractedRsns);
            vm.PrepareData();
            DeliveryFreetextReplace.ShowFreetexts(vm);
            ExtractRsnView = new ListCollectionView(vm.Rsns);
        }

        private async void DoGenerateNextBatchNumber(object obj)
        {
            try
            {
                WorkInProgress = true;
                var status = await RestHub.GenerateNextBatchNumber(NextBatch);
                if (status.HttpCode == System.Net.HttpStatusCode.OK)
                {
                    AppInfoBox.ShowInfoMessage(status.StatusMessage);
                    LoadDeliveryBatches();
                }
                else
                    AppErrorBox.ShowErrorMessage("Can't Generate New Batch Number. .", status.HttpResponse);
            }
            catch (Exception ex)
            {
                AppErrorBox.ShowErrorMessage("Can't Generate New Batch Number . .", ex.ToString());
            }
            finally
            {
                WorkInProgress = false;
            }
        }

        async private void DoGenerateMail(object obj)
        {
            try
            {
                WorkInProgress = true;
                if (DeliveryBatch != null)
                {
                    var result = await RestHub.GenerateEmail(DeliveryBatch.Id);
                    if (result.HttpCode == System.Net.HttpStatusCode.OK)
                    {
                        try
                        {
                            Microsoft.Office.Interop.Outlook.Application oApp = new Microsoft.Office.Interop.Outlook.Application();
                            Microsoft.Office.Interop.Outlook.MailItem mailItem = (Microsoft.Office.Interop.Outlook.MailItem)oApp.CreateItem(Microsoft.Office.Interop.Outlook.OlItemType.olMailItem);

                            mailItem.To = "user@cas.org";
                            mailItem.CC = "user1@excelra.com;user2@excelra.com";
                            mailItem.Subject = "Reactions Mail";
                            mailItem.BodyFormat = Microsoft.Office.Interop.Outlook.OlBodyFormat.olFormatHTML;
                            mailItem.HTMLBody = result.UserObject.ToString();
                            mailItem.Display(false);
                        }
                        catch (Exception ex)
                        {
                            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"{DeliveryBatch.BatchNumber}.html");
                            File.WriteAllText(path, result.UserObject.ToString());
                            AppInfoBox.ShowInfoMessage($"Unfortunately Outlook is not accessible. The prepared report will open in web browser, you may copy and paste in email manually.");
                            Process.Start(path);
                        }
                    }
                    else
                        AppErrorBox.ShowErrorMessage("Error While Generating Email . .", Environment.NewLine + result.HttpResponse);
                    DeliveryInProgress = Visibility.Hidden;
                }
                else
                    MessageBox.Show("From Batch, From Category, Delivery Batch Number Is Required . . .");
                WorkInProgress = false;
            }
            catch (Exception ex)
            {
                AppErrorBox.ShowErrorMessage("Can't generate mail report . .", ex.ToString());
            }
            finally
            {
                WorkInProgress = false;
            }
        }

        async private void DoGenerateZip(object obj)
        {

            try
            {
                WorkInProgress = true;
                if (DeliveryBatch != null)
                {
                    DeliveryInProgress = Visibility.Visible;
                    var result = await RestHub.GenerateZIP(DeliveryBatch.Id, MaxZipSize * 1000 * 1000);
                    if (result.HttpCode == System.Net.HttpStatusCode.OK)
                    {
                        ZipResultDTO zipResult = (ZipResultDTO)result.UserObject;
                        ZipResult.ShowResult(zipResult);
                    }
                    else
                        AppErrorBox.ShowErrorMessage("Error While Generating ZIP . .", result.HttpResponse);
                    DeliveryInProgress = Visibility.Hidden;
                }
                else
                    MessageBox.Show("From Batch, From Category, Delivery Batch Number Is Required . . .");
                WorkInProgress = false;
            }
            catch (Exception ex)
            {
                Log.This(ex);

            }
        }

        async private void DoGenerateXML(object obj)
        {

            try
            {
                WorkInProgress = true;
                if (DeliveryBatch != null)
                {
                    DeliveryInProgress = Visibility.Visible;
                    var result = await RestHub.GenerateXML(DeliveryBatch.Id);
                    if (result.HttpCode == System.Net.HttpStatusCode.OK)
                    {
                        var dto = result.UserObject as GenerateXMLDTO;
                        if (dto.IsSuccess)
                            GenerateXMLSuccessBox.ShowStatus(dto.OutXmlPath);
                    }
                    else
                        AppErrorBox.ShowErrorMessage(result.StatusMessage, result.HttpResponse);
                    DeliveryInProgress = Visibility.Hidden;
                }
                else
                    MessageBox.Show("From Batch, From Category, Delivery Batch Number Is Required . . .");
            }
            catch (Exception ex)
            {
                Log.This(ex);
                AppErrorBox.ShowErrorMessage("Can't Generate XML", ex.ToString());
            }
            finally
            {
                WorkInProgress = false;
            }
        }

        public QueryableCollectionView BatchTansView
        {
            get { return batchTansView; }
            set { SetProperty(ref batchTansView, value); }
        }

        private async void DoMoveToCategory(object obj)
        {

            try
            {
                WorkInProgress = true;
                if (BatchTans != null && SelectedTans != null && SelectedTans.Count() > 0)
                {
                    if (FromBatch != null && ToCategory != null)
                    {
                        var moveTansDto = new MoveTansDTO();
                        moveTansDto.TanIds = SelectedTans.Select(t => ((BatchTanVM)t).Id).ToList();
                        moveTansDto.TargetCategory = ToCategory.Value;
                        var status = await RestHub.MoveToCategory(moveTansDto);
                        if (status.HttpCode == System.Net.HttpStatusCode.OK)
                        {
                            SearchTans.Execute(this);
                            var result = MessageBox.Show(status.StatusMessage, "Status", MessageBoxButton.OKCancel);
                        }
                        else
                            AppErrorBox.ShowErrorMessage("Can't Move TANs To Category . .", status.HttpResponse);
                    }
                    else
                        AppInfoBox.ShowInfoMessage("From Batch, To Category Should Be Selected, And Must Be Different To Proceed. .");
                }
                else
                    AppInfoBox.ShowInfoMessage("Please Load and Select TANs To Proceed . .");
            }
            catch (Exception ex)
            {
                Log.This(ex);
                AppErrorBox.ShowErrorMessage("Can't Move TANs To Category . .", ex.ToString());
            }
            finally
            {
                WorkInProgress = false;
            }
        }

        
        private async void DoMoveToDelivery(object obj)
        {

            try
            {
                WorkInProgress = true;
                if (BatchTans != null && SelectedTans != null && SelectedTans.Count() > 0)
                {
                    var InValidTans = SelectedTans.Where(t => (((BatchTanVM)t).TanState.HasValue && S.UnDelivarableTanStates.Contains(((BatchTanVM)t).TanState.Value)) || ((BatchTanVM)t).IsDoubtRaised.ToLower().Equals("true")).ToList();
                    if(InValidTans.Any())
                    {
                        AppInfoBox.ShowInfoMessage("Selected Tan Contains NotAssigned/Curation In progress/Doubt Raised tans.");
                        return;
                    }
                    if (DeliveryBatch != null)
                    {
                        var moveTansDto = new MoveTansDTO();
                        moveTansDto.TanIds = SelectedTans.Select(t => ((BatchTanVM)t).Id).ToList();
                        moveTansDto.TargetCategory = DeliveryBatch.Id;
                        var status = await RestHub.MoveToDelivery(moveTansDto);
                        if (status.HttpCode == System.Net.HttpStatusCode.OK)
                        {
                            SearchTans.Execute(this);
                            var result = MessageBox.Show(status.StatusMessage, "Status", MessageBoxButton.OKCancel);
                        }
                        else
                            AppErrorBox.ShowErrorMessage("Can't Move TANs To Delivery . .", status.HttpResponse);
                    }
                    else
                        AppInfoBox.ShowInfoMessage("Delivery Batch Not Selected.");
                }
                else
                    AppInfoBox.ShowInfoMessage("Please Load and Select TANs To Proceed . .");
                WorkInProgress = false;
            }
            catch (Exception ex)
            {
                Log.This(ex);
                AppErrorBox.ShowErrorMessage("Can't Move TANs To Delivery . .", ex.ToString());
            }
            finally
            {
                WorkInProgress = false;
            }
        }

        private async void DoSearchTans(object obj)
        {

            try
            {
                WorkInProgress = true;
                BatchTansView = null;
                TotalTans = 0;
                BatchTans = new ObservableCollection<BatchTanVM>();
                BatchTans.CollectionChanged += BatchTans_CollectionChanged;
                if (FromBatch != null && ToBatch != null && FromCategory != null)
                {
                    var result = await RestHub.TansBetweenBatches(FromBatch.Name, ToBatch.Name, (int)FromCategory.Value);
                    if (result.HttpCode == System.Net.HttpStatusCode.OK)
                    {
                        var tans = (List<BatchTanDto>)result.UserObject;
                        foreach (var tan in tans)
                        {
                            BatchTans.Add(new BatchTanVM
                            {
                                Id = tan.Id,
                                BatchNum = tan.BatchNumber,
                                TanNumber = tan.TanNumber,
                                TanCategory = new TanCategoryVM { Value = (int)tan.TanCategory, Description = tan.TanCategory.DescriptionAttribute() },
                                TanType = tan.TanType,
                                Nums = tan.Nums,
                                Rxns = tan.Rxns,
                                Stages = tan.Stages,
                                Curator = tan.Curator,
                                Reviewer = tan.Reviewer,
                                QC = tan.QC,
                                IsDoubtRaised = tan.IsDoubtRaised.ToString(),
                                TanState = tan.TanState
                            });
                        }
                        BatchTansView = new QueryableCollectionView(BatchTans);
                    }
                    else
                        AppErrorBox.ShowErrorMessage(result.StatusMessage, result.HttpResponse);
                }
                else
                    MessageBox.Show("From Batch, To Batch, Category Are Required . .");
                UpdateSummary(BatchTans);
            }
            catch (Exception ex)
            {
                Log.This(ex);
                AppErrorBox.ShowErrorMessage("Error while searching TANs", ex.ToString());
            }
            finally
            {
                WorkInProgress = false;
            }
        }

        private void BatchTans_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {

        }

        public void UpdateSummary(ObservableCollection<BatchTanVM> batchtans)
        {

            try
            {
                if (batchtans != null)
                {
                    TotalTans = batchtans.Count;
                    NotAssigned = batchtans.Count(t => t.TanState == null || t.TanState == TanState.Not_Assigned);

                    CurationAssigned = batchtans.Count(t => t.TanState != null && t.TanState == TanState.Curation_Assigned);
                    ZeroRXNs = batchtans.Where(t=> t.TanState.HasValue && !S.UnDelivarableTanStates.Contains(t.TanState.Value)).Count(t => t.Rxns == 0);
                    ExtraStages = batchtans.Sum(t => t.Stages);
                    NotAssigned = batchtans.Count(t => t.TanState == TanState.Not_Assigned && string.IsNullOrEmpty(t.Curator));
                    RXNCurationAssigned = batchtans.Where(t => t.TanState != null && t.TanState == TanState.Curation_Assigned).Sum(t => t.Rxns);
                    CurationProgress = batchtans.Count(t => t.TanState != null && t.TanState == TanState.Curation_InProgress);
                    CurationDone = batchtans.Count(t => t.TanState != null && t.TanState == TanState.Curation_Submitted);

                    RXNCurationProgress = batchtans.Where(t => t.TanState != null && t.TanState == TanState.Curation_InProgress).Sum(t => t.Rxns);
                    RXNCurationDone = batchtans.Where(t => t.TanState != null && t.TanState == TanState.Curation_Submitted).Sum(t => t.Rxns);
                    ReviewAssigned = batchtans.Count(t => t.TanState != null && t.TanState == TanState.Review_Assigned);
                    ReviewDone = batchtans.Count(t => t.TanState != null && t.TanState == TanState.Review_Accepted);
                    RXNReviewAssigned = batchtans.Where(t => t.TanState != null && t.TanState == TanState.Review_Assigned).Sum(t => t.Rxns);
                    ReviewProgress = batchtans.Count(t => t.TanState != null && t.TanState == TanState.Review_InProgress);
                    RXNReviewProgress = batchtans.Where(t => t.TanState != null && t.TanState == TanState.Review_InProgress).Sum(t => t.Rxns);
                    RXNReviewDone = batchtans.Where(t => t.TanState != null && t.TanState == TanState.Review_Accepted).Sum(t => t.Rxns);

                    QCAssigned = batchtans.Count(t => t.TanState != null && t.TanState == TanState.QC_Assigned);
                    RXNQCAssigned = batchtans.Where(t => t.TanState != null && t.TanState == TanState.QC_Assigned).Sum(t => t.Rxns);
                    QCProgress = batchtans.Count(t => t.TanState != null && t.TanState == TanState.QC_InProgress);
                    RXNQCProgress = batchtans.Where(t => t.TanState != null && t.TanState == TanState.QC_InProgress).Sum(t => t.Rxns);
                    QCCompleted = batchtans.Count(t => t.TanState != null && t.TanState == TanState.QC_Accepted);
                    RXNQCCompleted = batchtans.Where(t => t.TanState != null && t.TanState == TanState.QC_Accepted).Sum(t => t.Rxns);
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);

            }
        }

        public void ClearState()
        {

            try
            {
                FromBatches = null;
                ToBatches = null;
                DeliveryInProgress = Visibility.Hidden;
                ClearTans();
            }
            catch (Exception ex)
            {
                Log.This(ex);

            }
        }
        private void ClearTans()
        {

            try
            {
                BatchTans = null;
                BatchTansView = null;
                TotalTans = 0;
                ExtraStages = 0;
                NotAssigned = 0;
                ZeroRXNs = 0;

                CurationAssigned = 0;
                CurationProgress = 0;
                ReviewAssigned = 0;
                ReviewProgress = 0;
                QCAssigned = 0;
                QCProgress = 0;
                QCCompleted = 0;

                RXNCurationAssigned = 0;
                RXNCurationProgress = 0;
                RXNReviewAssigned = 0;
                RXNReviewProgress = 0;
                RXNQCAssigned = 0;
                RXNQCProgress = 0;
                RXNQCCompleted = 0;
            }
            catch (Exception ex)
            {
                Log.This(ex);

            }
        }

        public async void LoadData()
        {
            try
            {
                #region Batches
                FromBatches = new ObservableCollection<BatchVM>();
                ToBatches = new ObservableCollection<BatchVM>();
                DeliveryBatches = new ObservableCollection<DeliveryBatchVM>();

                #region Shipment Batches
                var result = await RestHub.Batches();
                if (result.HttpCode == System.Net.HttpStatusCode.OK)
                {
                    List<BatchDTO> batches = (List<BatchDTO>)result.UserObject;
                    batches = batches.OrderByDescending(b => b.Name).ToList();
                    foreach (var batchDto in batches)
                        FromBatches.Add(new BatchVM
                        {
                            Id = batchDto.Id,
                            DateCreated = batchDto.DateCreated,
                            Name = batchDto.Name,
                            DocumentsPath = batchDto.DocumentsPath
                        });
                    FromBatch = FromBatches.FirstOrDefault();
                    foreach (var batchDto in batches)
                        ToBatches.Add(new BatchVM
                        {
                            Id = batchDto.Id,
                            DateCreated = batchDto.DateCreated,
                            Name = batchDto.Name,
                            DocumentsPath = batchDto.DocumentsPath
                        });
                    ToBatch = ToBatches.FirstOrDefault();
                    UpdateSummary(BatchTans);
                }
                else
                    AppErrorBox.ShowErrorMessage("Can't Load Batches . .", result.StatusMessage);
                #endregion

                #region Delivery Batches
                LoadDeliveryBatches();
                #endregion
                #endregion

                #region Categories
                FromCategories = new ObservableCollection<TanCategoryVM>();
                ToCategories = new ObservableCollection<TanCategoryVM>();

                foreach (TanCategory tanCategory in Enum.GetValues(typeof(TanCategory)))
                    FromCategories.Add(new TanCategoryVM { Value = (int)tanCategory, Description = tanCategory.DescriptionAttribute() });
                FromCategory = new TanCategoryVM { Value = (int)TanCategory.Progress, Description = TanCategory.Progress.DescriptionAttribute() };

                ToCategories.Add(new TanCategoryVM { Value = (int)TanCategory.Patents, Description = TanCategory.Patents.DescriptionAttribute() });
                ToCategories.Add(new TanCategoryVM { Value = (int)TanCategory.Journals, Description = TanCategory.Journals.DescriptionAttribute() });
                ToCategory = null;
                #endregion
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private async void LoadDeliveryBatches()
        {
            DeliveryBatches = new ObservableCollection<DeliveryBatchVM>();
            var deliveryResult = await RestHub.DeleveryBatches();
            if (deliveryResult.HttpCode == System.Net.HttpStatusCode.OK)
            {
                List<DeliveryBatchDTO> deliveryBatches = (List<DeliveryBatchDTO>)deliveryResult.UserObject;
                foreach (var batchDto in deliveryBatches)
                    DeliveryBatches.Add(new DeliveryBatchVM
                    {
                        Id = batchDto.Id,
                        BatchNumber = batchDto.BatchNumber
                    });
                DeliveryBatch = DeliveryBatches.FirstOrDefault();
            }
            else
                AppErrorBox.ShowErrorMessage("Can't Load Delivery Batches . .", deliveryResult.StatusMessage);
        }
    }
}
